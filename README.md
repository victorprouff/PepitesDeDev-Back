# Pépites de Dev

## Description
Ce projet m'est venu de discussion avec des anciens collègues. On a prit l'habitude de se partager des morceaux de code rencontrer ici, des relectures foireuses de PR, bref, des moments de vie de dev.
Des moments drôles comme d'autres plus agaçants. Mais cela ne restait qu'entre nous et cherchant un petit projet sur lequel bosser en dehors du boulôt pour tester des choses, je me suis dit que j'allais en faire le coeur de ce projet.

[Voila](https://pepites-de-dev.fr/) ce que ça donne :)

Le [repo](https://github.com/victorprouff/PepitesDeDev-Back) backend. </br>
Le [repo](https://github.com/victorprouff/PepitesDeDev-Front) frontend.

## Technos
- .Net pour l'API
- Angular pour le front
- Postgresql pour la DB

## Hébergement

- l'api en .net sur [Clever Cloud](https://www.clever-cloud.com/fr/)
- les images sur [Cellar](https://www.clever-cloud.com/doc/deploy/addon/cellar/) de Clever Cloud, compatible S3
- le front est déployé sur [infomaniak](https://www.infomaniak.com/fr/) (j'ai un petit souci pour déployé de angular sur Clever)

## Sources utilisés :
- Authentification : https://jasonwatmore.com/net-7-csharp-jwt-authentication-tutorial-without-aspnet-core-identity
- Gestion image : https://code-maze.com/upload-files-dot-net-core-angular/