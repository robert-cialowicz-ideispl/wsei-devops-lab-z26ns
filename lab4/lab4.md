# Laboratorium 4 – Docker Compose: usługi, sieci i wolumeny

Zbudujesz i uruchomisz aplikację z czterech usług, a następnie sprawdzisz trwałość danych, gotowość bazy i izolację sieci. Na koniec zmienisz stronę działającą w kontenerze przez Compose Watch.

## Prerekwizyty i dostęp

1. Korzystaj z tej samej maszyny Ubuntu i bramy WWW co na [lab3](../lab3/lab3.md). Adres IP bramy, numer maszyny `N` i dane logowania podaje prowadzący.
2. Otwórz `https://20.166.92.179/terminal/N`, np. `https://20.166.92.179/terminal/23`, i zaloguj się jako `studentN`. Wszystkie polecenia poniżej wykonuj w **Bash na zdalnym Ubuntu**.
3. Aplikację otwieraj w osobnej karcie pod **`https://20.166.92.179/site/N`**. Brama kieruje ten adres na port **8080** Twojej maszyny. Nie zmieniaj `N`, aby korzystać z cudzej maszyny.
4. Sprawdź narzędzia:

   ```bash
   docker --version
   docker compose version
   docker ps
   free -h
   df -h /
   ```

**Schowek w terminalu WWW:** `Ctrl` + `Alt` + `Shift` otwiera menu Guacamole z polem „Clipboard”. Ten sam skrót zamyka menu.
Przy pracy na własnym komputerze z Dockerem używaj `http://localhost:8080/`. Alternatywny dostęp SSH opisuje lab3.

## 0. Przygotuj własny katalog pracy

**Cel:** przygotujesz własną kopię aplikacji i obrazy potrzebne w kolejnych ćwiczeniach. Swoje zmiany zapiszesz w osobnym katalogu pracy.

```bash
git clone https://github.com/robert-cialowicz-ideispl/wsei-devops-lab-z26ns.git ~/wsei-devops-lab-z26ns-lab4
cd ~/wsei-devops-lab-z26ns-lab4
git status --short
```

Zastąp **`twoj-login` swoim loginem GitHub** we wszystkich dalszych poleceniach:

```bash
mkdir -p lab4/submissions/twoj-login
cp -R lab4/sample-app lab4/submissions/twoj-login/
cd lab4/submissions/twoj-login/sample-app
```

Edytuj wyłącznie swoją kopię w `lab4/submissions/twoj-login/`. Wszystkie dalsze polecenia Docker wykonuj w jej katalogu `sample-app`, chyba że instrukcja mówi inaczej.

```text
sample-app/
├── backend/             # API .NET 8; zapis i odczyt produktów
├── frontend/            # strona HTML/JS w Nginx
├── gateway/             # Nginx kierujący żądania do frontendu i API
└── docker-compose.yml   # konfiguracja STARTOWA do rozbudowania
```

Droga żądania:

```text
przeglądarka → brama uczelniana /site/N/ → gateway :8080
                                          ├── /           → frontend :80
                                          └── /api/...    → backend :8080 → db :1433
```

Przeglądarka komunikuje się z gateway. Backend korzysta z bazy pod nazwą `db`. Port bazy nie jest publikowany na hoście. Frontend używa względnego adresu `api/products`, dzięki czemu działa również pod `/site/N/`.

Zweryfikuj plik i przygotuj obrazy:

```bash
docker compose config --quiet
docker compose build
docker pull python:3.12-alpine
```

Pierwsze pobieranie SQL Server i budowanie .NET może potrwać kilka minut.

Projekt Compose ma nazwę **`lab4`**. Uruchamiaj jedną jego wersję naraz na swojej maszynie. Jawne tagi `lab4-backend:local`, `lab4-frontend:local` i `lab4-gateway:local` pozwalają wykorzystać te same obrazy przy uruchamianiu ręcznym.

Hasło `Lab4Compose!2026` jest wspólną, demonstracyjną wartością do tej bazy laboratoryjnej. Nie używaj tutaj prawdziwych haseł ani danych osobowych; strona pod `/site/N/` jest publiczna.

## 1. Uruchom aplikację ręcznie

**Cel:** uruchomisz cztery współpracujące kontenery i zobaczysz, ile poleceń oraz parametrów trzeba do tego przygotować ręcznie. Za chwilę tę samą pracę przejmie Compose. Korzystamy z już zbudowanych obrazów.

### 1.1 Utwórz sieć i uruchom bazę

```bash
docker network create lab4-manual
docker run -d --name lab4-manual-db --network lab4-manual --network-alias db \
  -e ACCEPT_EULA=Y -e MSSQL_PID=Express -e 'MSSQL_SA_PASSWORD=Lab4Compose!2026' \
  mcr.microsoft.com/mssql/server:2022-latest
docker logs --tail 30 lab4-manual-db
```

Poczekaj na komunikat `SQL Server is now ready for client connections`. Sprawdź, czy baza rzeczywiście przyjmuje zapytania:

```bash
docker exec lab4-manual-db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Lab4Compose!2026' -C -Q 'SELECT 1' -b
```

Jeśli baza jeszcze startuje, powtórz sprawdzenie po chwili. Dopiero poprawny wynik zapytania oznacza gotowość do kolejnego kroku.

### 1.2 Uruchom pozostałe usługi

```bash
docker run -d --name lab4-manual-backend --network lab4-manual --network-alias backend \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e 'ConnectionStrings__DefaultConnection=Server=db;Database=ProductsDB;User Id=sa;Password=Lab4Compose!2026;TrustServerCertificate=True;' \
  lab4-backend:local
docker run -d --name lab4-manual-frontend --network lab4-manual --network-alias frontend \
  lab4-frontend:local
docker run -d --name lab4-manual-gateway --network lab4-manual -p 8080:80 \
  lab4-gateway:local
docker ps --filter network=lab4-manual
curl -fsS http://localhost:8080/api/products
```

Jeśli API jeszcze startuje, sprawdź `docker logs lab4-manual-backend` i ponów żądanie po chwili. Otwórz `https://20.166.92.179/site/N/` i dodaj produkt `Test ręczny`.

Zwróć uwagę, jakie nazwy i parametry trzeba było powtórzyć, aby połączyć cztery usługi.

### 1.3 Zwolnij port i usuń wyłącznie zasoby tego ćwiczenia

```bash
docker stop lab4-manual-gateway lab4-manual-frontend lab4-manual-backend lab4-manual-db
docker rm lab4-manual-gateway lab4-manual-frontend lab4-manual-backend lab4-manual-db
docker network rm lab4-manual
```

Obrazy zachowaj — wykorzysta je Compose. Produkt z ręcznie uruchomionej bazy nie jest przenoszony do nowego zestawu kontenerów.

## 2. Uruchom aplikację przez Compose

**Cel:** uruchomisz całą aplikację na podstawie jednego pliku Compose oraz sprawdzisz stan usług i ich logi. Porównasz to z ręcznym uruchamianiem kontenerów z ćwiczenia 1.

Mapowanie `8080:80` udostępnia port `80` kontenera gateway na porcie `8080` maszyny Ubuntu, do którego kieruje brama WWW. Pozostaw to mapowanie przez całe laboratorium.

```bash
cat docker-compose.yml
docker compose up -d
docker compose ps -a
docker compose logs --tail 30 backend db
```

Wersja startowa ma cztery usługi, jedną automatyczną sieć i `restart: unless-stopped`. **Nie ma jeszcze wolumenu bazy, healthchecka ani podziału sieci.** Krótkie `depends_on: [db]` określa kolejność uruchamiania, ale nie czeka na gotowość SQL Server. Backend może zakończyć się błędem podczas startu; polityka restartu pozwoli mu spróbować ponownie. Nie musi to wystąpić przy każdym uruchomieniu.

Jeśli baza jest gotowa, ale backend jeszcze nie odpowiada, podejrzyj logi i poczekaj na jego wznowienie. W razie potrzeby po potwierdzeniu gotowości bazy wykonaj `docker compose restart backend gateway`.

```bash
curl -fsS http://localhost:8080/api/products
```

Oczekiwany wynik dla nowej bazy to `[]`. Otwórz stronę i dodaj produkt `Compose start`.

## 3. Sprawdź trwałość danych i dodaj wolumen

**Cel:** zabezpieczysz dane bazy przed utratą przy usuwaniu i odtwarzaniu kontenera. Nazwany wolumen przechowuje pliki bazy niezależnie od kontenera, dzięki czemu nowy kontener może ponownie z nich skorzystać.

### 3.1 Usuń kontener bazy bez wolumenu

W działającej aplikacji dodaj produkt **`Bez wolumenu`**. Następnie:

```bash
docker compose down
```

Kontener bazy został usunięty. Nie uruchamiaj go jeszcze — od razu dodamy trwałe miejsce na dane. Produkt `Bez wolumenu` nie zostanie odzyskany przez nową bazę.

Samo zatrzymanie lub restart tego samego kontenera nie usuwa jego danych. Tutaj wykonaliśmy `down`, które usuwa kontenery projektu.

### 3.2 Dodaj wolumen i uruchom aplikację

W swoim `docker-compose.yml` dopisz do **istniejącej usługi `db`**:

```yaml
    volumes:
      - db-data:/var/opt/mssql
```

Na końcu pliku, **na poziomie `services:`**, dodaj deklarację wolumenu:

```yaml
volumes:
  db-data:
```

Fragmenty należy włączyć do istniejącej konfiguracji. Nie twórz drugiego klucza `services:` ani drugiej definicji `db`. W YAML używaj spacji, nie tabulatorów.

```bash
docker compose config --quiet
docker compose up -d
docker volume inspect lab4_db-data
```

Poczekaj na dostępność API i odśwież stronę. Produktu `Bez wolumenu` nie powinno być — nowy wolumen nie odzyskuje danych starego kontenera.

Dodaj produkt **`Z wolumenem`**. **Nie zatrzymuj teraz aplikacji tylko po to, aby wykonać dodatkowy test.** W ćwiczeniu 4 i tak odtworzymy kontenery po zmianie konfiguracji; wtedy sprawdzisz, czy produkt pozostał.

Zwykłe `down` pozostawia nazwany wolumen. Opcja **`down -v` usunęłaby także wolumen i dane tego projektu** — nie wykonuj jej podczas ćwiczeń.

## 4. Poczekaj na gotowość bazy

**Cel:** backend powinien startować wtedy, gdy baza przyjmuje zapytania. Samo uruchomienie kontenera SQL Server jeszcze tego nie oznacza. Healthcheck sprawdzi gotowość bazy, a Compose poczeka na jego pozytywny wynik.

Zatrzymaj projekt przez `docker compose down`, zachowując wolumen. Do **istniejącej usługi `db`**, na tym samym poziomie co `image`, `environment` i `volumes`, dodaj:

```yaml
    healthcheck:
      test:
        - CMD-SHELL
        - >-
          /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa
          -P 'Lab4Compose!2026' -C -Q 'SELECT 1' -b
      interval: 10s
      timeout: 5s
      retries: 12
      start_period: 30s
```

`sqlcmd` wykonuje rzeczywiste zapytanie. `-b` ustawia niezerowy kod zakończenia przy błędzie SQL, a `-C` pozwala użyć certyfikatu demonstracyjnej bazy. Zapis `>-` składa dwie linie polecenia w jedną komendę powłoki.

W usłudze `backend` **zastąp dotychczasowe `depends_on`**:

```yaml
    depends_on:
      db:
        condition: service_healthy
```

Kompletną poprawną konfigurację na tym etapie znajdziesz w [compose.healthcheck.yml](sample-app/compose.healthcheck.yml). Porównaj z nią swój plik, jeśli walidacja zgłasza błąd. Zachowaj `name: lab4` na początku pliku — kolejne ćwiczenie korzysta z tego prefiksu nazw sieci.

```bash
docker compose config --quiet
docker compose up -d
docker compose ps
docker compose logs --tail 30 db backend
```

Oczekiwany rezultat: baza osiąga stan `healthy`, po czym Compose uruchamia backend. **Sprawdź teraz produkt `Z wolumenem` z ćwiczenia 3 — powinien pozostać w bazie mimo usunięcia i odtworzenia kontenera.** Pozostaw go w bazie do końcowego zrzutu strony.

## 5. Podziel sieć i sprawdź izolację

**Cel:** ograniczysz dostęp do bazy do usług, które go potrzebują. Backend będzie łączył obie sieci, a frontend i gateway nie uzyskają bezpośredniego dostępu do SQL Server. Sprawdzisz to dwoma testami połączenia.

Zatrzymaj projekt przez `docker compose down`. Dopisz na poziomie głównym pliku:

```yaml
networks:
  app-network:
  backend-network:
    internal: true
```

Przypisz każdej usłudze pole `networks` według tabeli. Przykład dla backendu:

```yaml
    networks:
      - app-network
      - backend-network
```

| Usługa | `app-network` | `backend-network` |
|---|---|---|
| `gateway` | tak | nie |
| `frontend` | tak | nie |
| `backend` | tak | tak |
| `db` | nie | tak |

```bash
docker compose config --quiet
docker compose up -d
docker network inspect lab4_app-network lab4_backend-network
```

Nazwy zasobów zawierają prefiks `lab4`, ponieważ tak nazywa się projekt. `internal: true` tworzy sieć izolowaną od zewnętrznej komunikacji. Oddzielenie frontendu od bazy wynika również z tego, że nie współdzielą żadnej sieci.

### 5.1 Połączenie z sieci bazy

Uruchom jednorazowy kontener diagnostyczny. Sprawdza on TCP pod nazwą usługi `db`, bez logowania do SQL Server:

```bash
docker run --rm --network lab4_backend-network python:3.12-alpine \
  python -c "import socket; s=socket.create_connection(('db',1433),3); print('OK: db:1433'); s.close()"
```

Oczekiwane: **`OK: db:1433`**.

### 5.2 To samo połączenie z sieci aplikacji

```bash
docker run --rm --network lab4_app-network python:3.12-alpine \
  python -c "import socket; s=socket.create_connection(('db',1433),3); print('OK: db:1433'); s.close()"
echo "Kod zakończenia: $?"
```

Oczekiwany jest **błąd wyszukania nazwy lub połączenia i niezerowy kod zakończenia**. To poprawny rezultat testu izolacji. Porównaj go z wynikiem poprzedniego testu.

Otwórz aplikację i dodaj kolejny produkt. Backend ma dostęp do obu sieci, dlatego aplikacja nadal działa.

## 6. Aktualizuj frontend przez Compose Watch

**Cel:** zmienisz stronę w działającym kontenerze bez ręcznego budowania nowego obrazu. Watch będzie kopiował zapisane zmiany HTML, podczas gdy backend i baza będą nadal pracować.

W usłudze `frontend`, obok `build` i `networks`, dodaj:

```yaml
    develop:
      watch:
        - action: sync
          path: ./frontend/index.html
          target: /usr/share/nginx/html/index.html
```

Ścieżka `path` wskazuje plik w Twojej kopii projektu na Ubuntu. `target` wskazuje plik w kontenerze frontendu. Nie zmieniaj pliku bezpośrednio wewnątrz kontenera.

```bash
docker compose config --quiet
docker compose up -d
docker compose watch </dev/null > /tmp/lab4-watch.log 2>&1 &
```

Ostatnie polecenie uruchamia Watch **w tle**.

Sprawdź zadanie w tle i jego komunikaty:

```bash
jobs
tail -n 20 /tmp/lab4-watch.log
```

Poczekaj na komunikat o włączeniu obserwowania, np. `Watch enabled`. Jeśli go jeszcze nie ma, po chwili ponów `tail`. Jeśli zadanie zakończyło się, sprawdź błąd w tym samym pliku.

W **tym samym terminalu**, nadal w swojej kopii `sample-app`, otwórz plik:

```bash
nano frontend/index.html
```

Zmień widoczny nagłówek `Lista produktów — lab4` na **`Lista produktów — Imię Nazwisko — Watch`**. Zapisz plik i wyjdź z edytora. Sprawdź synchronizację, a następnie odśwież stronę:

```bash
tail -n 20 /tmp/lab4-watch.log
```

Używaj wyłącznie fikcyjnych nazw produktów. `tail` bez opcji `-f` wyświetla ostatnie wpisy i od razu oddaje terminal.

Sprawdź:

- Czy nowy nagłówek jest widoczny bez `docker compose build`?
- Czy produkty dodane w ćwiczeniach 3–5 nadal istnieją?
- Czy zmiana frontendu wymagała odtworzenia bazy?

`sync` kopiuje plik do działającego kontenera. Nginx odczytuje HTML przy kolejnym żądaniu, dlatego wystarcza odświeżenie strony. Samo kopiowanie źródła C# nie przebudowałoby skompilowanego backendu. Zmiany synchronizowane przez Watch nie zmieniają również wcześniej zbudowanego obrazu; przy kolejnym odtworzeniu frontendu zaktualizuj obraz przez `docker compose build frontend` lub ponownie uruchom Watch.

## Artefakty do oddania

Prześlij prowadzącemu cztery czytelne zrzuty ekranu:

1. **Poprawna konfiguracja Compose** — pokaż polecenia i wynik:

   ```bash
   docker compose config --quiet
   echo "Kod zakończenia: $?"
   ```

   Kod `0` oznacza poprawną walidację. Drugie polecenie wykonaj bezpośrednio po pierwszym.

2. **Działająca strona** — widoczny zmieniony nagłówek z ćwiczenia Watch oraz produkt `Z wolumenem`, który pozostał po odtworzeniu bazy w ćwiczeniu 4.
3. **Uruchomione kontenery** — wynik `docker ps`, z widocznymi czterema usługami aplikacji i stanem `healthy` przy bazie danych.
4. **Aktualny plik `docker-compose.yml`** — czytelny zrzut końcowej wersji pliku po wykonaniu ćwiczeń, z konfiguracją wolumenu, healthchecka, sieci i Compose Watch.

Zrzuty przekaż w miejscu wskazanym na zajęciach. Po ich wykonaniu możesz zatrzymać projekt przez `docker compose down`, będąc w swojej kopii `sample-app`. Wolumen pozostaw do czasu oceny.

## Gdy coś nie działa

| Objaw | Co sprawdzić |
|---|---|
| `port is already allocated` | `docker ps --filter publish=8080`; zatrzymaj swój poprzedni kontener lab3 lub ręczny gateway. |
| Brama zwraca 502/503 | Sprawdź mapowanie `8080:80`, `docker compose ps -a` i żądanie `curl` na Ubuntu; sprawdź końcowy ukośnik w `/site/N/`. |
| Strona się otwiera, ale API nie działa | `docker compose logs --tail 50 backend gateway db`; poczekaj na bazę. Nie dodawaj początkowego `/` do `api/products` we frontendzie. |
| Baza ma status `unhealthy` | `docker compose logs --tail 50 db`; sprawdź pamięć, hasło, ścieżkę `sqlcmd` i parametr `-C`. |
| Nginx zgłasza brak hosta upstream | Sprawdź przypisanie usług do sieci; po poprawieniu wykonaj `docker compose down` i `docker compose up -d`, bez `-v`. |
| Watch nie widzi zmian | Sprawdź `jobs`, komunikaty w `tail -n 20 /tmp/lab4-watch.log`, wersję Compose i edycję pliku w tej samej kopii projektu na Ubuntu. |
| YAML nie przechodzi walidacji | Sprawdź podział na linie, wcięcia i powtórzone klucze. Po ćwiczeniu 4 porównaj plik z [compose.healthcheck.yml](sample-app/compose.healthcheck.yml). `db` i `backend` są osobnymi usługami; parametry healthchecka należą do `db.healthcheck`. |
