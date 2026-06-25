# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  # Ubuntu 24.04 LTS (Noble Numbat)
  config.vm.box = "bento/ubuntu-24.04"
  config.vm.hostname = "burgerpos-server"

  # Red: acceso desde host en http://localhost:8081 (8080 lo usa la VM de fase 1/2)
  config.vm.network "forwarded_port", guest: 8080, host: 8081
  # Red privada para acceso por IP directa (opcional)
  config.vm.network "private_network", ip: "192.168.56.11"

  config.vm.provider "virtualbox" do |vb|
    vb.name   = "BurgerPOS-Vagrant"
    vb.memory = 2048
    vb.cpus   = 2
    vb.gui    = false
  end

  # Sincronizar el codigo fuente al VM
  # El Dockerfile y docker-compose.prod.yml quedan en /burgerpos
  config.vm.synced_folder "./Codigo", "/burgerpos"

  # Paso 1: instalar Puppet Agent (necesario antes del provisioner puppet)
  config.vm.provision "shell", name: "install-puppet", inline: <<-SHELL
    apt-get update -qq
    apt-get install -y puppet
    echo "[vagrant] Puppet instalado: $(puppet --version)"
  SHELL

  # Paso 2: Puppet aprovisiona Docker y levanta la app
  config.vm.provision "puppet" do |puppet|
    puppet.manifests_path = "puppet/manifests"
    puppet.manifest_file  = "site.pp"
    puppet.module_path    = "puppet/modules"
    puppet.options        = "--verbose"
  end
end
