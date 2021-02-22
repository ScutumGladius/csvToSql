# Datenimport nach DataCollection

## Import mittels `Import-Excel.ps1`

`CsvToSql.exe` ist ein c#-based Programm, welches CSV- oder Excel-Dateien importiert. Soll die Zieldatenbank  angepasst werden, muss dies momentan noch am Anfang des Codes geändert werden (`$sqlServer`)

## Voraussetzung

* Net.Core 3 Framework

### Programmparameter

CsvToSql.exe --settings ../../../settings/settings.json --truncate 

* `settings`: Pfad zur verwendeten Settings-Datei, default ist _settings.json_
* -`database`: Datenbank in die importiert werden soll, default ist _DataCollection_
* -`noID`: Gibt an, dass die Tabelle, in die importiert wird, keine ID-Spalte mit einem GUID-Defaultwert hat. [Historisch gewachsen, sollte besser auf das gegenteiliges Verhalten umgestellt werden]
* `truncate`: Sollen die Inhalte der Tabelle vor dem Import komplett gelöscht werden. Beinhaltet die Zieltabelle die Spalte _DCImportDate_, muss dieser Switch angegeben werden
* `importDate`: Falls das _DCImportDate_ auf einen gewünschten Wert gesetzt werden soll

### Aufbau der JSON-Settings-Datei

Die Settings-Datei besteht aus einer Liste zu importierender Dateien im Schlüssel `importFiles`, die Dateien müssen dabei nicht notwendigerweise in dieselbe Tabelle importiert werden:

```JSON
    "importFiles": [
        {
            "file": "../CMDB _OMS_July_2020.xlsx",
            "sheet": "Ovierview",
            "row": 3,
            "col": 2,
            "skipline": 0,
            "table": "OMS",
            "prefix": "oms:",
            "key": "SerialNumber",
            "macFix":  "macAddress",
            "delimiter": ";",
            "batchSize": 100,
            "truncate": true,
            "quoting" : "",
            "saveMode": false,
            "comment": "Lorem Ipsum",
            "retryPolicyNumRetries": 5,
            "retryPolicyDelayRetries": 5000,
            "uniqueOnly": false,
            "columnMapping": [
                {
                    "SAL code": "SAL",
                    "site (city, street, zip code)": "City",
                    "serialNumber": "SerialNumber",
                    "macAddress": "MacAdress",
                    "country": "CountryCode",
                    "building": "Building",
                    "room": "Room",
                    "IP Address": "IpAddress",
                    "printerAccountName": "PrinterName",
                    "printerDomainLongName": "PrinterDomainLongName",
                    "statusFlag": "Status",
                    "ownerGID": "OwnerGID"
                }
            ]
        },
        {
            "file": "z:/20200911132530_20200907_Clients_SE.csv",
            "csv": true,
            "table": "ADPL",
            "prefix": "apdl:",
            "key": "host_name",
            "truncate": false,
            "columnMapping": [
                {
                    "host_name": "host_name",
                    "status": "status",
                    "err_msg": "err_msg",
                    "source_vlan": "source_vlan",
                    "target_vlan": "target_vlan",
                    "confirmation": "confirmation",
                    "**20200911132530_20200907_Clients_SE.csv": "DCSource"
                }
            ]
        }
    ]
}
```

#### Allgemeine Einstellungen

* connectionString: ConnectionString zum SQL-Server
* file: Absoluter oder relativer Pfad der zu importierenden Datei
* table: In welche Tabelle importiert werden soll
* macFix: [optional] Gibt den Spaltenamen aus Excel/CSV an, in der sich eine Mac-Adresse befindet. Wird dieser angegeben, so werden alle gelesenen Mac-Adressen auf das Format XX-XX-XX-XX-XX-XX normiert
* locationFix: [optional] Gibt den Spaltenamen aus Excel/CSV an, in der sich eine Location befindet. Wird dieser angegeben, so werden von allen gelesenen Locations der Teil vor dem Minus entfernt.
* prefix: Historisch gewachsen, wird eigentlich nicht benötigt, muss aber trotzdem vorhanden sein und mit einem Doppelpunkt enden
* key: Historisch gewachsen, wird eigentlich nicht benötigt, muss aber trotzdem vorhanden sein
* truncate: Soll die bisherigen Inhalt der SQL-Tabelle gelöscht werden? (default - false)
* batchSize: Die Anzahl der Insert-Befehlen in einer SQL-Abfrage. (default - 1000)
* saveMode: Prevent error "String or binary data would be truncated." Adjust the length of the Data to the field size. (default - false)
* forceCreateTable: Die alte SQL-Tabelle wird gelöscht und die Neue angelegt. (default - false)
* comment: [optional] Kommentar für Table "TABLESTATUS".  
* retryPolicyNumRetries: Wie oft wird es versucht die erzeugte SQL-Query auszuführen.(default - 3)
* retryPolicyDelayRetries: Zeitspanne zwischen den Fehlversuchen in Millisekunden (default - 1000)
* uniqueOnly: Nur die unique Zeile werden importiert.(default - false)

##### Excel

* sheet: Name des Tabellenblatts, von dem importiert werden soll
* row: Zeile, ab der die Daten beginnen, meist wohl 1
* col: Spalte, ab der die Daten beginnen, meist wohl 1
* skipline: [optional] Wieviele Zeilen sollen beim Einlesen übersprungen werden

##### CSV

Wird eine CSV-Datei eingelesen, so werden neben den allgemeinen Einstellungen und dem ColumnMapping nur folgende Werte benötigt:

* csv: true
* delimiter: [optional] Trennzeichen der CSV-Datei

#### ColumnMapping

In ColumnMapping steht ein Dictionary, das angibt, welche Spalte aus der Quelldatei auf welche Tabellenspalte abgebildet wird. In den Keys stehen dabei die Spaltennamen aus Excel oder CSV und die Values bilden die Tabellenspalten ab. Beginnt ein Key mit zwei Sternen (`**`), wird kein Inhalt aus der Datei herangenommen, sondern der Wert, der nach diesen beiden Sternen folgt in die entsprechende Tabellenspalte (aus dem Value) geschrieben. Dies findet hauptsächlich für die Tabellenspalte `DCSource` Anwendung.

```JSON
{
    "host_name": "host_name",
    "status": "status",
    "err_msg": "err_msg",
    "source_vlan": "source_vlan",
    "target_vlan": "target_vlan",
    "confirmation": "confirmation",
    "**20200911132530_20200907_Clients_SE.csv": "DCSource"
}
```

Beginnt ein Key mit ##ImportDate, wird kein Inhalt aus der Datei herangenommen, sondern das aktuelle ImportDate.

```JSON
{
    "host_name": "host_name",
    "status": "status",
    "err_msg": "err_msg",
    "source_vlan": "source_vlan",
    "target_vlan": "target_vlan",
    "confirmation": "confirmation",
    "##ImportDate": "DCImportDate",
    "##ImportFileName": "DCSource"
}
```

## Import mittels `Get-ADPL.ps1`

Für die im Share `\\defthwa0is0sto.ad001.siemens.net\grp$\FSA01068101\Out` abgelegten ADPL-CSV-Dateien gibt es das Skript `Get-ADPL.ps1`. Parameter sind für dieses Skript keine nötig, es wird allerdings nach dem Passwort des Service-Accounts zum Zugriff auf den Share gefragt.

Das Skript liest die Dateinamen aller bereits importierten Dateien aus der Datenbank (_DCSource_ in der Tabelle _ADPL_) und ermittelt somit, welche Dateien neu zu importieren sind. Danach wird für jede dieser neuen Dateien `Import-Excel.ps1` mit einer neu erzeugten Settings-Datei aufgerufen.

## Import mittels `Get-PPMDChangeOwner.ps1`

Bei OMS-Geräten kann es vorkommen, dass das zugehörige PPMD einen Owner in der falschen Company hat. Um diese Owner zu ändern gibt es ein von **** angebotenen Prozess, bei dem ein Excel mit _ciuuid_ der zu ändernden Geräte und _gid_ der neuen Owner bereitzustellen ist. Um dieses Excel zu erzeugen, werden aus der Datenbank die entsprechenden Geräte ausgelesen, in ein Excel geschrieben und den Verantwortlichen bei Siemens und Siemens Energy (C. Tirico, A. Aschenbrenner, J. Greiner, M. Kleinemeier) zur Freigabe vorgelegt.

Dafür werde in der Datenbank die Views `vOmsPpmdSag2Se`, `vOmsPpmdSe2Sag` und `vPpmdOwnerChangeMinMaxDate`sowie die Tabelle `PPMDOWNERCHANGE` verwendet.

Unser Prozess um diese Listen in der Datenbank nachverfolgen zu können sieht folgendermaßen aus:

* Die Vorschlagsliste bekommt den Namen ChangePersonOnDeviceUUID-SourceCompany-YYcwCW (also beispielswiese `ChangePersonOnDeviceUUID-SE-21cw03` oder `ChangePersonOnDeviceUUID-SAG-21cw03.xlsx`) und wird im Verzeichnis `ppmd\changeowner` abgelegt. Sollten aus dieser Liste Geräte gestrichen werden, wird eine neue Datei mit einem aufsteigendem alphabetischen Suffix erzeugt (`ChangePersonOnDeviceUUID-SAG-21cw03a.xlsx`). Die für den Import in die Datenbank wichtigen Daten liegen dabei im Arbeitsblatt `ChangeOwner`. Zur Namensgebung für die Dateien muss noch gesagt werden, dass der Teil nach dem letzten `-` in die Spalte `DCCW` der Tabelle `PPMDOWNERCHANGE` geschrieben wird. Der Name sollte also so gewählt werden, dass dort etwas einigermaßen sinnvolles steht.

* Sobald diese Excels im Verzeichnis liegen, kann `Get-PPMDChangeOwner.ps` ausgeführt werden

* Wenn alle Geräte die Freigabe erhalten haben, wird eine neue Excel-Datei mit dem Namenssuffix `-clean` (`ChangePersonOnDeviceUUID-SAG-21cw03a-clean.xlsx`) erstellt und an ******  geschickt, die neue Datei kann bereits in das changeowner-Verzeichnis gelegt werden, sollte aber in der Dateiendung umbenannt werden (also etwa von `.xlsx` nach `.xls_`), damit sie beim der Ausführung des Skripts noch nicht berüksichtigt wird.

* Sobald **** Vollzug meldet, kann die Datei wieder richtig benannt werden (`.xlsx`) und das Skript erneut ausgeführt werden.
