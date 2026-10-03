# Lab 2 – Linux i GitHub

Login GitHub: Konrad-Francuz

## Polecenia Linux

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | C:\Users\Student\wsei-devops-lab-z26ns |
| `ls -la` | Wyświetla pliki, w tym ukryte. | $ ls -la
total 16
drwxr-xr-x 1 Student 197121   0 Oct  3 16:21 ./
drwxr-xr-x 1 Student 197121   0 Oct  3 16:21 ../
drwxr-xr-x 1 Student 197121   0 Oct  3 16:35 .git/
-rw-r--r-- 1 Student 197121 907 Oct  3 16:21 README.md
drwxr-xr-x 1 Student 197121   0 Oct  3 16:21 lab1/
drwxr-xr-x 1 Student 197121   0 Oct  3 16:28 lab2/
 |
| `mkdir -p` | Tworzy katalogi. | $ mkdir -p lab2/submissions

Student@DESKTOP-EUG2G68 MINGW64 ~/wsei-devops-lab-z26ns (lab2/Konrad-Francuz)
 |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | $ grep -n 'Druga' "$practice_file"
2:Druga linia
 |
| `wc -l` | Liczy wiersze pliku. | $ wc -l "$practice_file"
2 /tmp/tmp.r0OwVbs402
 |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/Konrad-Francuz`
