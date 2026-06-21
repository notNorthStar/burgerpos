# Fase 1 — Despliegue Manual en Linux

## Requisitos previos
- VirtualBox instalado en Windows
- ISO de Ubuntu Server 24.04 LTS descargada

---

## Paso 1 — Crear la VM en VirtualBox

1. Abrir VirtualBox → Nuevo
2. Nombre: `BurgerPOS-Linux`, Tipo: Linux, Versión: Ubuntu (64-bit)
3. RAM: **2048 MB** mínimo
4. Disco duro: VDI, dinámico, **20 GB**
5. En Configuración → Red → Adaptador 1: NAT  
   Configuración → Red → Adaptador 2: Red solo anfitrión (host-only)
6. Montar la ISO en Almacenamiento → iniciar

**Captura:** pantalla de la VM recién creada en VirtualBox.

---

## Paso 2 — Instalar Ubuntu Server 24.04

Durante la instalación elegir:
- Idioma: English (o Español)
- Perfil de servidor: OpenSSH Server (marcar con espacio)
- Nombre de usuario: `vagrant` (o cualquier nombre, anotar)
- Contraseña: anotar

**Captura:** pantalla final de instalación completada ("Installation complete!").

---

## Paso 3 — Instalar dependencias del sistema

```bash
sudo apt-get update && sudo apt-get upgrade -y
sudo apt-get install -y git curl wget
```

**Captura:** terminal con `apt-get update` ejecutándose.

---

## Paso 4 — Instalar .NET 10 SDK

```bash
# Descargar el script de instalacion de Microsoft
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0

# Agregar al PATH
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$PATH:$HOME/.dotnet:$HOME/.dotnet/tools' >> ~/.bashrc
source ~/.bashrc

# Verificar
dotnet --version
```

**Captura:** salida de `dotnet --version` mostrando `10.x.x`.

---

## Paso 5 — Instalar PostgreSQL 16

```bash
sudo apt-get install -y postgresql-16

# Iniciar y habilitar el servicio
sudo systemctl enable postgresql
sudo systemctl start postgresql

# Verificar estado
sudo systemctl status postgresql
```

**Captura:** `systemctl status postgresql` mostrando "active (running)".

---

## Paso 6 — Configurar la base de datos

```bash
sudo -u postgres psql <<EOF
CREATE DATABASE burgerpos_prod;
CREATE USER burgerpos WITH PASSWORD 'Burgerpos_Prod_2026!';
GRANT ALL PRIVILEGES ON DATABASE burgerpos_prod TO burgerpos;
\q
EOF
```

**Captura:** terminal con los comandos ejecutados sin errores.

---

## Paso 7 — Clonar el repositorio

```bash
git clone https://github.com/notNorthStar/burgerpos.git ~/burgerpos
cd ~/burgerpos/Codigo
```

**Captura:** `git clone` completado.

---

## Paso 8 — Publicar la aplicacion

```bash
cd ~/burgerpos/Codigo
dotnet publish BurgerPOS.Web/BurgerPOS.Web.csproj \
    -c Release \
    -o ~/burgerpos-app
```

**Captura:** salida de `dotnet publish` con "Build succeeded".

---

## Paso 9 — Ejecutar la aplicacion

```bash
cd ~/burgerpos-app

# Sobreescribir connection string via variable de entorno
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=burgerpos_prod;Username=burgerpos;Password=Burgerpos_Prod_2026!"
export ASPNETCORE_URLS="http://+:8080"

dotnet BurgerPOS.Web.dll
```

**Captura:** terminal mostrando "Now listening on: http://[::]:8080".

---

## Paso 10 — Verificar desde el navegador del host

Desde Windows, abrir el navegador en `http://<IP-de-la-VM>:8080`

Para conocer la IP de la VM:
```bash
ip addr show | grep "inet "
```

**Captura:** pantalla del navegador mostrando el login de BurgerPOS.

---

## Resumen de capturas requeridas

| # | Captura |
|---|---------|
| 1 | VM creada en VirtualBox |
| 2 | Instalacion Ubuntu completada |
| 3 | apt-get update en terminal |
| 4 | dotnet --version mostrando 10.x |
| 5 | systemctl status postgresql (running) |
| 6 | Creacion de DB sin errores |
| 7 | git clone completado |
| 8 | dotnet publish con Build succeeded |
| 9 | App escuchando en puerto 8080 |
| 10 | Login de BurgerPOS en navegador |
