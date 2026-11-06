# Laboratorium 3 – Docker Fundamentals

## Prerekwizyty
1. Połącz się z maszyną wirtualną używając SSH:

	```
	ssh studentideis@20.107.202.106 -p (50000 + N)
    np. ssh studentideis@20.107.202.106 -p 50023
	```

---

## Ćwiczenie 1 – Weryfikacja instalacji Dockera

1. Sprawdź wersję Dockera:
	```
	docker --version
	```
2. Upewnij się, że możesz uruchamiać kontenery:
	```
	docker run hello-world
	```
3. Sprawdź listę dostępnych lokalnie obrazów:
	```
	docker images
	```
4. Sprawdź lokalnie status kontenerów:
	```
	docker ps -a
	```

---

## Ćwiczenie 2 – Uruchomienie kontenera Nginx

1. Pobierz obraz Nginx:
	```
	docker pull nginx
	```
2. Uruchom kontener:
	```
	docker run -d -p 8080:80 --name webserver nginx
	```
	- `-d` – tryb detached
	- `-p 8080:80` – mapowanie portu: host 8080 → kontener 80
	- `--name webserver` – nazwa kontenera
3. Sprawdź działanie kontenera:
	```
	docker ps
	```
4. Otwórz stronę w przeglądarce (http://localhost:8080 jeśli pracujesz lokalnie)
   (Środowisko zdalne / VM) Jeśli korzystasz z maszyny współdzielonej z numerem studenta `N`, i obowiązuje formuła portu `51000 + N`, otwórz:
	- `http://20.107.202.106:(51000 + N)`

---

## Ćwiczenie 3 – Modyfikacja strony (bind mount)

1. Stwórz katalog na własną stronę (PowerShell / Bash):
	```
	mkdir ~/myweb
	echo "<h1>Hello from student Imie Nazwisko </h1>" > ~/myweb/index.html
	```
2. Zatrzymaj poprzedni kontener (jeśli działa):
	```
	docker stop webserver
	docker rm webserver
	```
3. Uruchom nowy kontener z podmontowanym katalogiem (bind mount):
	```
	docker run -d -p 8080:80 -v ~/myweb:/usr/share/nginx/html --name customweb nginx
	```
	- `-v lokalny_katalog:ścieżka_w_kontenerze` – podmontowanie katalogu z hosta
4. Podejrzyj zawartość kontenera (interaktywnie, tylko do odczytu / inspekcji):
	```
	docker exec -it customweb /bin/sh
	```
	Wewnątrz kontenera:
	```
	ls -l /usr/share/nginx/html
	cat /usr/share/nginx/html/index.html
	exit
	```
5. Odśwież stronę w przeglądarce: http://localhost:8080 (lub port z formuły 51000+N na środowisku zdalnym). Powinno wyświetlać Twój własny HTML.
6. Zmień treść pliku i zobacz efekt bez restartu kontenera:
	```
	echo "<h1>Hello from student Imie Nazwisko NumerAlbumu</h1>" > ~/myweb/index.html
	```
	Odśwież stronę ponownie.

---

## Ćwiczenie 4 – Operacje na istniejącym kontenerze (utrzymanie)

Korzystamy z kontenera `customweb` utworzonego w ćwiczeniu 3. Nie tworzymy nowych obrazów – ćwiczymy diagnostykę i administrację.

1. Wyświetl listę kontenerów (działających):
	```
	docker ps
	```
2. Wyświetl wszystkie kontenery (również zatrzymane):
	```
	docker ps -a
	```
3. Sprawdź logi kontenera (Nginx może mieć minimalne logi access/error):
	```
	docker logs customweb
	```
4. Podejrzyj szczegółowe informacje (JSON) – porty, wolumeny, zmienne:
	```
	docker inspect customweb
	```
5. Sprawdź użycie zasobów (krótkie ‘top’ kontenerów):
	```
	docker stats --no-stream customweb
	```
6. Przetestuj restart kontenera (powinien wrócić szybko) i sprawdź status:
	```
	docker restart customweb
    docker ps -a
	```

---

## Ćwiczenie 5 – Własny obraz Nginx z własną stroną HTML

**Cel:** Porównaj bind mount z kopiowaniem plików do obrazu. Zbudujesz dwie wersje własnego obrazu i sprawdzisz, kiedy zmiana pliku HTML pojawia się w kontenerze.

Polecenia wykonuj na maszynie Ubuntu, w tej samej sesji SSH co poprzednie ćwiczenia. Przewidywany czas: 15–20 minut.

1. Przejdź do katalogu ze stroną z ćwiczenia 3:

   ```bash
   cd ~/myweb
   cat index.html
   ```

2. Utwórz plik o nazwie `Dockerfile` (bez rozszerzenia):

   ```bash
   cat > Dockerfile <<'EOF'
   FROM nginx:stable-alpine
   COPY index.html /usr/share/nginx/html/index.html
   EOF
   ```

3. Zbuduj obraz z tagiem `v1`:

   ```bash
   docker build -t moja-strona:v1 .
   docker images
   ```

4. Wykonaj wymagany zrzut ekranu z działającym `customweb` i stroną w przeglądarce. Następnie zatrzymaj ten kontener, aby zwolnić port `8080`, i uruchom własny obraz:

   ```bash
   docker stop customweb
   docker run -d -p 8080:80 --name imageweb moja-strona:v1
   docker ps
   ```

5. Otwórz ten sam adres strony co wcześniej: `http://20.107.202.106:(51000 + N)`, zastępując port obliczoną wartością, np. dla `N = 23`: `http://20.107.202.106:51023`. Powinna wyświetlić się Twoja strona.

6. Zmień lokalny plik HTML i odśwież stronę w przeglądarce:

   ```bash
   echo '<h1>Wersja v2 - Imie Nazwisko NumerAlbumu</h1>' > index.html
   cat index.html
   docker exec imageweb cat /usr/share/nginx/html/index.html
   ```

   Porównaj zawartość obu plików. Strona w kontenerze nadal zawiera wersję `v1`, ponieważ zmiana lokalnego pliku nie zmienia już zbudowanego obrazu.

7. Zbuduj obraz z tagiem `v2`:

   ```bash
   docker build -t moja-strona:v2 .
   ```

   Odśwież stronę ponownie. Samo zbudowanie nowego obrazu nie zmienia działającego kontenera — `imageweb` nadal korzysta z wersji `v1`.

8. Zastąp kontener nowym, uruchomionym z obrazu `v2`:

   ```bash
   docker stop imageweb
   docker rm imageweb
   docker run -d -p 8080:80 --name imageweb moja-strona:v2
   docker ps
   ```

   Odśwież stronę. Teraz powinna wyświetlać treść wersji `v2`.

---

## Artefakty wymagane do zaliczenia

- Zrzut ekranu wykonany przed zatrzymaniem `customweb` w ćwiczeniu 5, zawierający działający kontener Nginx `customweb` z własnym `index.html`:
  - `docker ps -a`
  - okno przeglądarki
- Odpowiedz krótko: **Dlaczego zmiana lokalnego HTML nie zmieniła strony działającej w kontenerze? Czym różni się to od bind mount z ćwiczenia 3?**
