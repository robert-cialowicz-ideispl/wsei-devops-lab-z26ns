# Lab 2 – Linux i GitHub

Login GitHub: catchychoruses

## Polecenia Linux

| Polecenie | Do czego służy            | Moja obserwacja lub wynik                              |
| --------- | ------------------------- | ------------------------------------------------------ |
| `pwd`     | Pokazuje bieżący katalog. | /Users/karol/Documents/DEV_STUFF/wsei-devops-lab-z26ns |

| `ls -la` | Wyświetla pliki, w tym ukryte. | total 8 |

drwxr-xr-x@ 6 karol staff 192 Oct 17 12:09 .
drwxr-xrwx@ 47 karol staff 1504 Oct 17 12:09 ..
drwxr-xr-x@ 12 karol staff 384 Oct 17 12:10 .git
drwxr-xr-x@ 3 karol staff 96 Oct 17 12:09 lab1
drwxr-xr-x@ 3 karol staff 96 Oct 17 12:09 lab2
-rw-r--r--@ 1 karol staff 891 Oct 17 12:09 README.md |

| `mkdir -p` | Tworzy katalogi. | nic nie wypluwa do konsoli |

| `grep -n` | Wyszukuje tekst i pokazuje numer wiersza. |

grep -n 'Druga' "$practice_file"
2:Druga linia
|

| `wc -l` | Liczy wiersze pliku. |

wc -l "$practice_file"

2 /var/folders/rd/1fszvrf52svgc6pk6kmq208c0000gn/T/tmp.baLaWWnpMR |

## Git i Pull Request

- Nazwa mojej gałęzi: `lab2/catchychoruses`
