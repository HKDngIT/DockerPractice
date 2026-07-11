# Docker Practice – WeatherAPI on Raspberry Pi 5

A short guide for building, transferring, and running a simple WeatherAPI as a Docker container on a Raspberry Pi 5 (ARM64).

## Prerequisites

- Docker with Buildx support on the build machine (e.g. Mac/PC)
- Docker installed on the Raspberry Pi 5
- SSH/SCP access or another way to transfer files to the Pi

## 1. Build the image for ARM64

Run inside the project folder:

```bash
docker buildx build --platform linux/arm64 -t weatherapi:arm64 --load .
```

## 2. Export the image as a `.tar` file

```bash
docker save weatherapi:arm64 -o weatherapi.tar
```

## 3. Copy the `.tar` file to the Raspberry Pi

For example via `scp`:

```bash
scp weatherapi.tar pi@<raspberry-pi-ip>:~/
```

## 4. Load the image on the Raspberry Pi

In the directory containing the `.tar` file:

```bash
docker load < weatherapi.tar
```

## 5. Run the container

```bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development weatherapi:arm64
```

## 6. Test the API

Open the Swagger UI in your browser:

```
http://localhost:8080/swagger/index.html
```

## Notes

- `--platform linux/arm64` ensures the image is built for the Pi even if the build machine has a different architecture (e.g. x86_64).
- `ASPNETCORE_ENVIRONMENT=Development` enables the Swagger UI, among other things; for production use, this value should be adjusted or removed accordingly.
- Useful commands learned from https://raspberrytips.com/docker-on-raspberry-pi/

- Stop the container
    ```
    docker stop [container]
    ```

- Remove the container
    ```
    docker rm [container]
    ```

- start on the PI
    ```
    docker run -d -p 8080:8080 -p 8081:8081 -p 1883:1883 weatherapi:arm64
    ```

- Logging on the PI
    ```
    docker logs -f <container-id>
    ```

TEST