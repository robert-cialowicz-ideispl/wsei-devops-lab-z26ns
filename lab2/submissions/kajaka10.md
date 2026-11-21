# Lab 2 – Linux i GitHub

Autor: Wojciech Szlosek

Login GitHub: kajaka10

## Polecenia Linux

Polecenia wykonane w Git Bash (Git for Windows 2.56.1) na Windows 11.

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | Wyświetliło `/c/Users/wojci/wsei-devops-lab-z26ns` – Git Bash pokazuje dysk `C:` jako `/c/`. |
| `ls -la` | Wyświetla pliki, w tym ukryte. | Pokazało ukryty katalog `.git` (bez `-a` nie byłby widoczny), katalogi `lab1`–`lab4` i plik `README.md` (939 B), razem z uprawnieniami (`drwxr-xr-x`), właścicielem `wojci` i datą. |
| `mkdir -p` | Tworzy katalogi. | `mkdir -p lab2/submissions` utworzyło brakujący katalog `submissions` bez komunikatu; `-p` nie zgłasza błędu, gdy katalog już istnieje, i tworzy też katalogi nadrzędne. |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | `grep -n 'Druga'` zwróciło `2:Druga linia` – szukany tekst jest w drugim wierszu. |
| `wc -l` | Liczy wiersze pliku. | Wynik `2 /tmp/tmp.b8C6L2FsfY` – plik z `mktemp` miał 2 wiersze; po `rm` plik już nie istniał (`No such file or directory`). |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/kajaka10`
