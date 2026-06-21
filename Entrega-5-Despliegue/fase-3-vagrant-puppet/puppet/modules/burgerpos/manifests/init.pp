# Modulo burgerpos: construye la imagen y levanta los contenedores con Docker Compose
class burgerpos {

  # Construir imagen y levantar contenedores (db + app)
  # usa /burgerpos/docker-compose.prod.yml sincronizado por Vagrant
  exec { 'burgerpos-docker-compose-up':
    command     => '/usr/bin/docker compose -f /burgerpos/docker-compose.prod.yml up -d --build',
    environment => ['HOME=/root'],
    timeout     => 600,
    require     => Service['docker'],
  }
}
