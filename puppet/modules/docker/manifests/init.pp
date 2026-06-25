# Modulo docker: instala Docker CE y docker-compose-plugin en Ubuntu 24.04
class docker {

  # Dependencias del repositorio oficial de Docker
  package { ['apt-transport-https', 'ca-certificates', 'curl', 'gnupg']:
    ensure => installed,
  }

  # GPG key del repositorio Docker
  exec { 'add-docker-gpg-key':
    command => '/usr/bin/curl -fsSL https://download.docker.com/linux/ubuntu/gpg | /usr/bin/gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg',
    creates => '/usr/share/keyrings/docker-archive-keyring.gpg',
    require => Package['curl', 'gnupg'],
  }

  # Repositorio APT de Docker para noble (Ubuntu 24.04)
  exec { 'add-docker-apt-repo':
    command => '/bin/bash -c "echo \"deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu noble stable\" > /etc/apt/sources.list.d/docker.list"',
    creates => '/etc/apt/sources.list.d/docker.list',
    require => Exec['add-docker-gpg-key'],
  }

  # Actualizar cache apt despues de agregar el repo
  exec { 'apt-update-for-docker':
    command     => '/usr/bin/apt-get update',
    subscribe   => Exec['add-docker-apt-repo'],
    refreshonly => true,
  }

  # Instalar Docker CE y plugin de compose
  package { ['docker-ce', 'docker-ce-cli', 'containerd.io', 'docker-buildx-plugin', 'docker-compose-plugin']:
    ensure  => installed,
    require => Exec['apt-update-for-docker'],
  }

  # Habilitar y arrancar el servicio Docker
  service { 'docker':
    ensure  => running,
    enable  => true,
    require => Package['docker-ce'],
  }

  # Agregar el usuario vagrant al grupo docker (sin sudo)
  exec { 'add-vagrant-to-docker-group':
    command => '/usr/sbin/usermod -aG docker vagrant',
    unless  => '/usr/bin/id -nG vagrant | /bin/grep -qw docker',
    require => Package['docker-ce'],
  }
}
