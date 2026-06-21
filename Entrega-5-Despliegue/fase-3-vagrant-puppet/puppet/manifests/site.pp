# site.pp — Punto de entrada de Puppet para BurgerPOS
# Aplica los modulos docker y burgerpos en orden

node default {
  class { 'docker': } ->
  class { 'burgerpos': }
}
