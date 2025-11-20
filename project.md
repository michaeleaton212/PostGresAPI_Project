# Hotel Adula Projekt
**Autor:** Michael Eaton

# Anleitung für die Übersetzung einer Seite

## i18n-Attribute setzen
###Auf jeder Seite, die übersetzt werden soll, beim jeweiligen Text das passende i18n-Attribut einsetzen.

## i18n-Attribute extrahieren
### Im Terminal den entsprechenden Befehl ausführen, um alle i18n-Tags in eine Übersetzungsdatei zu extrahieren.
```bash
ng extract-i18n --output-path src/locale
```

## Datei für Sprache erstellen/aktualisieren
## Im Terminal den entsprechenden Befehl ausführen, um eine Übersetzungsdatei für die Zielsprache zu erstellen oder zu aktualisieren.
```bash
cp src/locale/messages.xlf src/locale/messages.es.xlf
```

## Target-Übersetzungen in der Sprachdatei setzen
In der Sprachdatei bei allen Einträgen ein <target>-Element ergänzen und darin die Übersetzung für die Zielsprache eintragen.

# Startanleitung 

## Backend starten
```bash
cd \PostGresAPI_Project\PostGresAPI_Project\PostGresAPI
dotnet run
```

## Frontend bauen und lokalen i18n-Server starten
```bash
cd \PostGresAPI_Project\PostGresAPI_Project\Frontend\frontend\my-app
ng build
node local-i18n-server.mjs
```
