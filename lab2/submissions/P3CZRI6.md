# Lab 2 – Linux i GitHub

Login GitHub: P3CZRI6

## Polecenia Linux

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | Wyświetliło ścieżkę `/mnt/c/Users/user/Desktop/wsei-devops-lab-z26ns`, czyli katalog mojego repozytorium. |
| `ls -la` | Wyświetla pliki, w tym ukryte. | Pokazało 4 elementów, w tym ukryty katalog `.git` oraz wpisy `.` i `..`. W każdej linii widać uprawnienia, właściciela, rozmiar i datę modyfikacji. |
| `mkdir -p` | Tworzy katalogi. | Utworzyło od razu `lab2` i `lab2/submissions`. Ponowne uruchomienie nie zwróciło błędu, bo `-p` ignoruje istniejące katalogi. |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | Zwróciło `2:Druga linia`, czyli tekst „Druga” znajduje się w 2. wierszu. |
| `wc -l` | Liczy wiersze pliku. | Zwróciło 2, bo plik tymczasowy miał 2 wiersze. |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/P3CZRI6`
