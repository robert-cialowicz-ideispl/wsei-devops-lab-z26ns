# Laboratorium 2 – Linux i GitHub: repozytorium, gałęzie i pull requesty

## Cel

Przećwiczyć podstawowe polecenia terminala Linux/macOS oraz typowy przepływ pracy z Git i GitHub: sklonować repozytorium kursowe, utworzyć własną gałąź, zapisać commit, wypchnąć go na GitHub i zgłosić pracę do oceny przez Pull Request.

## Ważne informacje

- Pracuj na swoim koncie GitHub. Nie używaj wspólnych kont ani tokenów i nigdy nie zapisuj danych uwierzytelniających w plikach repozytorium.
- Repozytorium kursowe jest wspólne dla grupy. Przed zajęciami prowadzący musi nadać studentom uprawnienia zapisu. Gałąź `main` powinna być chroniona przed bezpośrednim pushowaniem — pracę oddaje się przez Pull Request.
- Nazwy `twoj-login` w poleceniach zastąp swoim loginem GitHub, bez znaku `@`. Dzięki temu każdy student będzie pracował na osobnej gałęzi i doda własny plik.
- Jeśli nie masz uprawnień do wypchnięcia gałęzi, zatrzymaj się i poproś prowadzącego o dostęp. Nie zmieniaj adresu repozytorium na cudzy fork ani nie używaj cudzego tokenu.

## 1. Sklonuj repozytorium kursowe

Otwórz terminal Linux lub macOS. Sklonuj repozytorium przeznaczone dla studiów niestacjonarnych i przejdź do jego katalogu:

```bash
git clone https://github.com/robert-cialowicz-ideispl/wsei-devops-lab-z26ns.git
cd wsei-devops-lab-z26ns
```

Sprawdź wersję Git, katalog roboczy, stan repozytorium i skonfigurowany adres zdalny:

```bash
git --version
pwd
git status --short --branch
git remote -v
```

Jeśli Git nie zna jeszcze Twojego autora commitów, ustaw nazwę i adres e-mail lokalnie dla tego repozytorium (bez `--global`). Możesz użyć adresu no-reply widocznego w ustawieniach e-mail GitHub:

```bash
git config user.name "Twoje imię i nazwisko"
git config user.email "12345678+twoj-login@users.noreply.github.com"
```

Zastąp przykładowy adres e-mail adresem no-reply z ustawień swojego konta GitHub.

## 2. Utwórz własną gałąź

Pracuj na gałęzi `lab2/twoj-login`, zastępując `twoj-login` swoim loginem GitHub:

```bash
git switch -c lab2/twoj-login
git branch --show-current
```

Upewnij się, że druga komenda wyświetla nazwę Twojej gałęzi.

## 3. Przećwicz polecenia Linux

Wykonaj polecenia i sprawdź, co robią:

```bash
pwd
ls -la
mkdir -p lab2/submissions

practice_file=$(mktemp)
printf 'Pierwsza linia\nDruga linia\n' > "$practice_file"
cat "$practice_file"
grep -n 'Druga' "$practice_file"
wc -l "$practice_file"
rm "$practice_file"
```

`pwd` pokazuje bieżący katalog, `ls -la` wyświetla jego zawartość, `mkdir -p` tworzy katalog, `printf` zapisuje tekst do pliku, `cat` odczytuje plik, `grep -n` wyszukuje tekst wraz z numerem wiersza, a `wc -l` liczy wiersze. Plik tymczasowy jest usuwany na końcu ćwiczenia.

## 4. Przygotuj plik do oceny

Utwórz plik `lab2/submissions/twoj-login.md`, zastępując `twoj-login` swoim loginem. Uzupełnij tabelę własnymi obserwacjami z terminala:

```bash
cat > lab2/submissions/twoj-login.md <<'EOF'
# Lab 2 – Linux i GitHub

Login GitHub: twoj-login

## Polecenia Linux

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | Uzupełnij |
| `ls -la` | Wyświetla pliki, w tym ukryte. | Uzupełnij |
| `mkdir -p` | Tworzy katalogi. | Uzupełnij |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | Uzupełnij |
| `wc -l` | Liczy wiersze pliku. | Uzupełnij |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/twoj-login`
EOF
```

W poleceniu zastąp `twoj-login` swoim loginem GitHub. Następnie otwórz plik w edytorze tekstowym i uzupełnij tabelę własnymi obserwacjami; nie kopiuj przykładowych wyników jako własnych.

## 5. Zapisz i wypchnij zmianę

Sprawdź stan repozytorium i dodaj do commita wyłącznie swój plik — nie używaj `git add .`:

```bash
git status --short
git add lab2/submissions/twoj-login.md
git diff --cached --check
git diff --cached --stat
```

Jeśli w podsumowaniu widzisz wyłącznie swój plik, utwórz commit i wypchnij gałąź:

```bash
git commit -m "Add Lab 2 submission for twoj-login"
git log -1 --oneline
git push --set-upstream origin lab2/twoj-login
```

Jeżeli GitHub odrzuca push z powodu braku uprawnień, poproś prowadzącego o dostęp. Do uwierzytelniania używaj własnego konta GitHub i skonfigurowanej metody logowania; nie umieszczaj tokenu w poleceniu ani w repozytorium.

## 6. Utwórz Pull Request

1. Otwórz w przeglądarce repozytorium kursowe.
2. Przejdź do **Pull requests** i wybierz **New pull request**. Możesz też skorzystać z przycisku **Compare & pull request**, jeśli GitHub go wyświetli.
3. Ustaw bazę (`base`) na `main`, a porównywaną gałąź (`compare`) na `lab2/twoj-login`.
4. Nadaj PR tytuł `Lab 2 – twoj-login` i krótko opisz wykonaną pracę.
5. Przed wysłaniem sprawdź, czy PR zawiera tylko `lab2/submissions/twoj-login.md`, a jego autorem jest Twoje konto.
6. Utwórz PR i pozostaw go otwartego do oceny. Nie scalaj go samodzielnie.

Przekaż prowadzącemu URL utworzonego Pull Requesta w miejscu wskazanym na zajęciach.

## Artefakt do oddania

- URL Pull Requesta skierowanego do `main` repozytorium kursowego.
