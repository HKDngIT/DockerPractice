# Docker Practice – WeatherAPI auf dem Raspberry Pi 5

Kurzanleitung zum Bauen, Übertragen und Ausführen einer einfachen WeatherAPI als Docker-Container auf einem Raspberry Pi 5 (ARM64).

## Voraussetzungen

- Docker mit Buildx-Unterstützung auf dem Build-Rechner (z. B. Mac/PC)
- Docker auf dem Raspberry Pi 5 installiert
- SSH-/SCP-Zugriff oder ein anderer Weg, Dateien auf den Pi zu übertragen

## 1. Image für ARM64 bauen

Im Projektordner ausführen:

```bash
docker buildx build --platform linux/arm64 -t weatherapi:arm64 --load .
```

## 2. Image als `.tar` exportieren

```bash
docker save weatherapi:arm64 -o weatherapi.tar
```

## 3. `.tar`-Datei auf den Raspberry Pi kopieren

Zum Beispiel per `scp`:

```bash
scp weatherapi.tar pi@<raspberry-pi-ip>:~/
```

## 4. Image auf dem Raspberry Pi laden

Im Verzeichnis mit der `.tar`-Datei:

```bash
docker load < weatherapi.tar
```

## 5. Container starten

```bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development weatherapi:arm64
```

## 6. API testen

Swagger UI im Browser öffnen:

```
http://localhost:8080/swagger/index.html
```

## Hinweise

- `--platform linux/arm64` sorgt dafür, dass das Image auch dann für den Pi gebaut wird, wenn der Build-Rechner eine andere Architektur hat (z. B. x86_64).
- `ASPNETCORE_ENVIRONMENT=Development` aktiviert u. a. die Swagger-UI; für den Produktivbetrieb sollte dieser Wert entsprechend angepasst oder entfernt werden.
