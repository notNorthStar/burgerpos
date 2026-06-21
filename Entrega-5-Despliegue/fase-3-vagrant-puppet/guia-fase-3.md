# Fase 3 — Aprovisionamiento Automatico con Vagrant + Puppet

## Que hace esta fase

Con un solo comando (`vagrant up`) Vagrant:
1. Descarga y crea una VM Ubuntu 24.04 LTS en VirtualBox
2. Sincroniza el codigo fuente al directorio `/burgerpos` dentro de la VM
3. Instala Puppet Agent via shell provisioner
4. Ejecuta Puppet, que automaticamente:
   - Instala Docker CE + docker-compose-plugin
   - Agrega al usuario `vagrant` al grupo `docker`
   - Ejecuta `docker compose -f /burgerpos/docker-compose.prod.yml up -d --build`

## Estructura de archivos

```
fase-3-vagrant-puppet/
├── Vagrantfile
├── guia-fase-3.md
└── puppet/
    ├── manifests/
    │   └── site.pp              ← punto de entrada Puppet
    └── modules/
        ├── docker/
        │   └── manifests/
        │       └── init.pp      ← instala Docker CE
        └── burgerpos/
            └── manifests/
                └── init.pp      ← levanta los contenedores
```

---

## Requisitos en Windows (host)

- VirtualBox instalado
- Vagrant instalado (https://developer.hashicorp.com/vagrant/downloads)
  - Verificar: `vagrant --version`

---

## Paso 1 — Abrir terminal en esta carpeta

```powershell
cd "C:\Users\David\Desktop\ClaudeCode\ProyectoFinal-PDS\Entrega-5-Despliegue\fase-3-vagrant-puppet"
```

**Captura:** terminal en la carpeta correcta con `dir` mostrando el Vagrantfile.

---

## Paso 2 — Iniciar la VM y aprovisionamiento automatico

```bash
vagrant up
```

Vagrant descargara la box `bento/ubuntu-24.04` la primera vez (~1GB).
Luego ejecutara los provisioners (shell + puppet).
El build de Docker puede tardar 10-15 minutos la primera vez.

**Captura:** terminal con `vagrant up` mostrando las lineas de Puppet:
- "Notice: Compiled catalog..."
- "Notice: /Stage[main]/Docker/..."
- "Notice: /Stage[main]/Burgerpos/..."

---

## Paso 3 — Verificar el aprovisionamiento

```bash
vagrant ssh
```

Dentro de la VM:
```bash
docker compose -f /burgerpos/docker-compose.prod.yml ps
```

Debe mostrar `burgerpos_db` y `burgerpos_app` en estado `Up`.

**Captura:** `docker compose ps` con los dos contenedores corriendo.

---

## Paso 4 — Abrir la aplicacion en el navegador

Desde Windows: `http://localhost:8080`

**Captura:** pantalla del navegador con el login de BurgerPOS (aprovisionado automaticamente).

---

## Paso 5 — Detener la VM

```bash
exit                 # salir del SSH
vagrant halt         # apagar la VM
```

Para destruirla completamente:
```bash
vagrant destroy -f
```

---

## Resumen de capturas requeridas

| # | Captura |
|---|---------|
| 1 | `dir` en la carpeta con el Vagrantfile |
| 2 | `vagrant up` con output de Puppet (Docker instalado, contenedores levantados) |
| 3 | `vagrant ssh` + `docker compose ps` (ambos Up) |
| 4 | Login de BurgerPOS en navegador (aprovisionado automaticamente) |
