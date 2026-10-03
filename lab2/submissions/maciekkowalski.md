# Lab 2 – Linux i GitHub

Login GitHub: maciekkowalski

## Polecenia Linux

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | Wypisało ścieżkę do sklonowanego repozytorium wsei-devops-lab-z26ns. |
| `ls -la` | Wyświetla pliki, w tym ukryte. | Pokazało listę z uprawnieniami i rozmiarami oraz ukryty katalog `.git`, którego zwykłe `ls` nie wyświetla. |
| `mkdir -p` | Tworzy katalogi. | Utworzyło `lab2/submissions` bez błędu; opcja `-p` tworzy też katalogi nadrzędne i nie zgłasza błędu, gdy katalog już istnieje. |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | Dla frazy `Druga` zwróciło `2:Druga linia`, czyli numer wiersza i jego treść. |
| `wc -l` | Liczy wiersze pliku. | Zwróciło `2` wraz ze ścieżką pliku tymczasowego, bo plik miał dwie linie. |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/maciekkowalski`
