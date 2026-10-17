# Lab 2 – Linux i GitHub

Login GitHub: Mino0z

## Polecenia Linux

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | Wyświetliło pełną ścieżkę kończącą się na `wsei-devops-lab-z26ns`, czyli jestem w katalogu sklonowanego repozytorium. |
| `ls -la` | Wyświetla pliki, w tym ukryte. | Pokazało `README.md`, katalogi `lab1`, `lab2` oraz ukryty katalog `.git` (bez `-a` nie byłby widoczny), wraz z uprawnieniami, właścicielem, rozmiarem i datą. |
| `mkdir -p` | Tworzy katalogi. | Utworzyło `lab2/submissions` bez komunikatu; `-p` nie zgłasza błędu, gdy katalog już istnieje, i tworzy brakujące katalogi nadrzędne. |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | Wynik `2:Druga linia` – znaleziony tekst jest w 2. wierszu pliku tymczasowego. |
| `wc -l` | Liczy wiersze pliku. | Wynik `2` oraz ścieżka pliku tymczasowego z `mktemp` (w katalogu `/var/folders/.../T/` na macOS). |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/jakub-minorczyk`
