# Lab 2 – Linux i GitHub

Login GitHub: Korea77

## Polecenia Linux

| Polecenie | Do czego służy | Moja obserwacja lub wynik |
|---|---|---|
| `pwd` | Pokazuje bieżący katalog. | /c/Users/Student/wsei-devops-lab-z26ns |
| `ls -la` | Wyświetla pliki, w tym ukryte. | 
        drwxr-xr-x 1 Student 197121   0 Oct 17 12:22 ./
        drwxr-xr-x 1 Student 197121   0 Oct 17 12:24 ../
        drwxr-xr-x 1 Student 197121   0 Oct 17 12:22 .git/
        -rw-r--r-- 1 Student 197121 907 Oct 17 12:22 README.md
        drwxr-xr-x 1 Student 197121   0 Oct 17 12:22 lab1/
        drwxr-xr-x 1 Student 197121   0 Oct 17 12:23 lab2/

| `mkdir -p` | Tworzy katalogi. | $ mkdir -p lab2/submissions |
| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. | 
    $ grep -n 'Druga' "$practice_file"
    2:Druga linia
| `wc -l` | Liczy wiersze pliku. | 
    $ wc -l "$practice_file"
    2 /tmp/tmp.XAnLJwPxty

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/Korea77`
