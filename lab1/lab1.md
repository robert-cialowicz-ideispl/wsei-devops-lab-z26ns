# Laboratorium 1 – Wstęp do Azure

## Cel

Utworzyć grupę zasobów nazwaną nazwiskiem i numerem indeksu, utworzyć Storage Account, a następnie opublikować w nim statyczną stronę HTML.

W każdym ćwiczeniu wybierz jeden wariant: **Azure PowerShell** albo **Azure CLI**. Polecenia Azure CLI są przygotowane dla terminala Linux (Bash) i macOS; na obu systemach użyj wspólnej sekcji **Azure CLI — Linux / macOS**.

## Ćwiczenie 1 – Logowanie do Azure

<details>
<summary>Azure PowerShell</summary>

1. Uruchom **PowerShell**.
2. Zainstaluj moduł Azure, jeśli nie jest jeszcze zainstalowany:
   ```powershell
   Install-Module -Name Az -Scope CurrentUser -Repository PSGallery
   ```
3. Zaloguj się i wybierz subskrypcję:
   ```powershell
   Connect-AzAccount
   Get-AzSubscription
   Set-AzContext -Subscription "<your-subscription-id>"
   ```

</details>

<details>
<summary>Azure CLI — Linux / macOS</summary>

Jeśli Azure CLI nie jest zainstalowane, skorzystaj z [instrukcji instalacji](https://learn.microsoft.com/cli/azure/install-azure-cli).

```
az login
az account list --output table
az account set --subscription "<your-subscription-id>"
```

</details>

## Ćwiczenie 2 – Utworzenie Resource Group

Ustaw nazwisko (bez spacji) i numer indeksu. Nazwa Resource Group musi zawierać oba te elementy:

W wariancie Azure CLI zastąp w poleceniach `rg-kowalski-123456` swoją nazwą Resource Group zawierającą nazwisko i numer indeksu. Tę samą nazwę wykorzystaj w kolejnych poleceniach.

<details>
<summary>Azure PowerShell</summary>

```powershell
$surname = "kowalski"
$studentId = "123456"
$resourceGroupName = "rg-$surname-$studentId"
$location = "westeurope"

New-AzResourceGroup -Name $resourceGroupName -Location $location
Get-AzResourceGroup -Name $resourceGroupName
```

</details>

<details>
<summary>Azure CLI — Linux / macOS</summary>

```
az group create --name rg-kowalski-123456 --location westeurope
az group show --name rg-kowalski-123456 --output table
```

</details>

Sprawdź, czy grupa zasobów pojawiła się w Azure Portal w sekcji **Resource groups**.

## Ćwiczenie 3 – Utworzenie Storage Account

Wybierz unikalną nazwę konta. Nazwa Storage Account musi mieć od 3 do 24 znaków i zawierać wyłącznie małe litery oraz cyfry. Poniższa nazwa jest przykładem — zastąp ją własną:

W wariancie Azure CLI zastąp `rg-kowalski-123456` swoją nazwą Resource Group, a `stkowalski123456` własną unikalną nazwą Storage Account. Używaj tych samych nazw w kolejnych poleceniach.

<details>
<summary>Azure PowerShell</summary>

```powershell
$storageName = "stkowalski123456"

New-AzStorageAccount -ResourceGroupName $resourceGroupName `
                     -Name $storageName `
                     -Location $location `
                     -SkuName Standard_LRS `
                     -Kind StorageV2

Get-AzStorageAccount -ResourceGroupName $resourceGroupName
```

Jeśli pojawi się błąd informujący o niezarejestrowanym providerze, sprawdź i zarejestruj `Microsoft.Storage`:

```powershell
Get-AzResourceProvider -ProviderNamespace Microsoft.Storage
Register-AzResourceProvider -ProviderNamespace Microsoft.Storage
```

Rejestracja może potrwać kilkadziesiąt sekund. Sprawdzisz jej stan poleceniem:

```powershell
Get-AzResourceProvider -ProviderNamespace Microsoft.Storage |
    Select-Object ProviderNamespace, RegistrationState
```

Po uzyskaniu stanu `Registered` ponów tworzenie Storage Account.

</details>

<details>
<summary>Azure CLI — Linux / macOS</summary>

```
az storage account create --resource-group rg-kowalski-123456 --name stkowalski123456 --location westeurope --sku Standard_LRS --kind StorageV2
az storage account show --resource-group rg-kowalski-123456 --name stkowalski123456 --output table
```

Jeśli pojawi się błąd informujący o niezarejestrowanym providerze, sprawdź stan `Microsoft.Storage`:

```
az provider show --namespace Microsoft.Storage --query registrationState --output tsv
```

Jeżeli stan jest inny niż `Registered`, zarejestruj providera:

```
az provider register --namespace Microsoft.Storage
az provider show --namespace Microsoft.Storage --query registrationState --output tsv
```

Rejestracja może potrwać kilkadziesiąt sekund. Po uzyskaniu stanu `Registered` ponów polecenie `az storage account create`.

</details>

## Ćwiczenie 4 – Weryfikacja w portalu

1. W Azure Portal otwórz **Resource groups**.
2. Wybierz grupę `rg-<nazwisko>-<numer-indeksu>`.
3. Otwórz Storage Account i sprawdź jego konfigurację.

## Ćwiczenie 5 – Publikacja statycznej strony HTML

Użyj narzędzia AI, aby wygenerować pojedynczy plik `index.html` o dowolnej treści. Zapisz go w wybranym miejscu i w poleceniu przesyłania podaj ścieżkę do pliku. Poproś o kompletną stronę HTML, która nie wymaga procesu budowania.

W poleceniach Azure CLI poniżej użyj tych samych nazw Resource Group i Storage Account co w ćwiczeniach 2–3; zastąp przykładowe `rg-kowalski-123456` i `stkowalski123456` swoimi wartościami.

<details>
<summary>Azure PowerShell</summary>

1. Pobierz kontekst połączenia ze Storage Account:
   ```powershell
   $ctx = (Get-AzStorageAccount -ResourceGroupName $resourceGroupName `
                              -Name $storageName).Context
   ```
2. Włącz funkcję **Static website** i ustaw `index.html` jako stronę startową:
   ```powershell
   Enable-AzStorageStaticWebsite -Context $ctx -IndexDocument "index.html"
   ```
   Włączenie tej funkcji tworzy specjalny kontener **`$web`**. Nie twórz w tym celu zwykłego kontenera, np. `studentdata`.
3. Prześlij plik `index.html` do kontenera `$web`:
   ```powershell
   Set-AzStorageBlobContent -File "<pełna-ścieżka-do-index.html>" `
                            -Container '$web' `
                            -Blob "index.html" `
                            -Context $ctx `
                            -Properties @{ ContentType = "text/html" } `
                            -Force
   ```
   Wartość `'$web'` jest ujęta w apostrofy, aby PowerShell potraktował ją dosłownie.
4. Sprawdź, czy plik znajduje się w `$web`:
   ```powershell
   Get-AzStorageBlob -Container '$web' -Context $ctx |
       Select-Object Name, Length, ContentType
   ```
5. Odczytaj adres statycznej strony i otwórz go w przeglądarce:
   ```powershell
   $storageAccount = Get-AzStorageAccount -ResourceGroupName $resourceGroupName `
                                          -Name $storageName
   $storageAccount.PrimaryEndpoints.Web
   ```

</details>

<details>
<summary>Azure CLI — Linux / macOS</summary>

1. Włącz funkcję **Static website** i ustaw `index.html` jako stronę startową:
   ```bash
   az storage blob service-properties update --account-name stkowalski123456 --static-website true --index-document index.html --auth-mode key
   ```
   Włączenie tej funkcji tworzy specjalny kontener **`$web`**. Nie twórz w tym celu zwykłego kontenera, np. `studentdata`.
2. Prześlij plik `index.html` do kontenera `$web`:
   ```bash
   az storage blob upload --account-name stkowalski123456 --auth-mode key --container-name '$web' --name index.html --file "/path/to/index.html" --content-type "text/html" --overwrite true
   ```
3. Sprawdź, czy plik znajduje się w `$web`:
   ```bash
   az storage blob list --account-name stkowalski123456 --auth-mode key --container-name '$web' --output table
   ```
4. Odczytaj adres statycznej strony:
   ```bash
   az storage account show --resource-group rg-kowalski-123456 --name stkowalski123456 --query "primaryEndpoints.web" --output tsv
   ```
</details>

Otwórz **Primary endpoint** z sekcji **Static website** w portalu albo adres wypisany przez PowerShell lub Azure CLI. Strona powinna wyświetlić zawartość `index.html`. Do testu używaj endpointu WWW, a nie adresu kontenera Blob.

## Artefakt do oddania

- URL utworzonej strony statycznej (**Primary endpoint**).
