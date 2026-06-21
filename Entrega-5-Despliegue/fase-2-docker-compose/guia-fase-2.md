# Fase 2 — Despliegue con Docker Compose

Esta fase puede realizarse en la misma VM de la Fase 1, o en una VM nueva.

## Arquitectura

```
┌─────────────────────────────────────────┐
│  VM Linux (Ubuntu 24.04)                │
│                                         │
│  ┌──────────────┐   ┌───────────────┐   │
│  │  app         │   │  db           │   │
│  │  :8080       │──▶│  postgres:16  │   │
│  │  BurgerPOS   │   │  burgerpos_db │   │
│  └──────────────┘   └───────────────┘   │
│         │                               │
│         ▼ red: burgerpos_net            │
└─────────────────────────────────────────┘
          │ puerto 8080:8080
          ▼
   Navegador del host (Windows)
```

---

## Paso 1 — Instalar Docker en la VM

```bash
# Dependencias
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg

# GPG key oficial de Docker
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | \
  sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# Repositorio Docker para Ubuntu 24.04 (noble)
echo "deb [arch=amd64 signed-by=/etc/apt/keyrings/docker.gpg] \
  https://download.docker.com/linux/ubuntu noble stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Instalar Docker CE + compose plugin
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io \
    docker-buildx-plugin docker-compose-plugin

# Agregar usuario al grupo docker (sin sudo)
sudo usermod -aG docker $USER
newgrp docker

# Verificar
docker --version
docker compose version
```

**Captura:** `docker --version` y `docker compose version` en terminal.

---

## Paso 2 — Clonar el repositorio (si no existe)

```bash
git clone https://github.com/notNorthStar/burgerpos.git ~/burgerpos
cd ~/burgerpos/Codigo
```

**Captura:** `git clone` completado (o `git pull` si ya existe).

---

## Paso 3 — Levantar los contenedores

```bash
cd ~/burgerpos/Codigo

# Construir imagen y levantar db + app en segundo plano
docker compose -f docker-compose.prod.yml up -d --build
```

El primer `up --build` tarda varios minutos (descarga SDK .NET 10 ~700MB).

**Captura:** terminal con el proceso de build mostrando capas descargadas.

---

## Paso 4 — Verificar que los contenedores estan corriendo

```bash
docker compose -f docker-compose.prod.yml ps
```

Debe mostrar `burgerpos_db` y `burgerpos_app` con estado `Up`.

```bash
# Ver logs de la app para confirmar que arranco
docker logs burgerpos_app --tail 20
```

**Captura:** `docker compose ps` con ambos contenedores `Up` + logs mostrando "Now listening on http://[::]:8080".

---

## Paso 5 — Verificar desde el navegador

Desde Windows: `http://localhost:8080` (el Vagrantfile redirige el puerto 8080).

**Captura:** pantalla del navegador mostrando el login de BurgerPOS.

---

## Paso 6 — Detener los contenedores (cuando termines)

```bash
docker compose -f docker-compose.prod.yml down
```

---

## Resumen de capturas requeridas

| # | Captura |
|---|---------|
| 1 | docker --version + docker compose version |
| 2 | git clone completado |
| 3 | docker compose up --build (build en progreso) |
| 4 | docker compose ps (ambos Up) + docker logs (app escuchando) |
| 5 | Login de BurgerPOS en navegador |
