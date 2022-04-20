# INTELIGENCIA ARTIFICIAL PARA VIDEOJUEGOS - PRÁCTICA 4 - GRUPO 05

Sergio Molinero Aparicio - Andrés Carnicero Esteban

Video: 

## RESUMEN
Esta práctica se basa en el manejo de la IA de un RTS ambientado en el universo de Dune, en el que se enfrentarán las tropas Fremen contra las Harkonnen.

En el prototipo y punto de partida otorgado nos encontramos con varios elementos, sean así pertenecientes al propio escenario como dunas o campos de "especia",
edificios de control de tropas y las propias unidades. Ahora se procede a explicar cada elemento.

### Explicación elementos RTS
Como hemos mencionado antes, tendremos varios elementos, que tendrán que coordinarse entre sí.

#### Escenario de Arrakis
Arrakis es el planeta donde transcurre gran parte de Dune, aquí se ha representado un campo de batalla con la API de terrenos de Unity.

De este terreno cuelgan:
##### -Dunas:
Son elementos estáticos que funcionan simplemente como obstáculos del terreno.
##### -Campos de "Especia":
La especia es el material más valioso en el universo Dune. En nuestro prototipo hay pequeñas zonas de las que algunas tropas pueden extraer ese material para
posteriormente transformarla en dinero y comprar más tropas.
Estos campos llevan un script:

     // Limited Access //
     (Info extraída del propio script)
     componente que se asocia a cualquier objeto que sólo pueda ser usado por una unidad de extracción a la vez, 
     como los recursos extraíbles o las intalaciones de procesamiento.
     
     Lleva varias variables de control:
     Max_Positions = numero de unidades que pueden estar en esa zona a la vez
     Radius = distancia a la que se quedan las tropas situadas al llegar
     
##### -Campamentos "Graben":
Los graben son un pueblo supuestamente neutral, disponen de varias edificaciones que afectarán tanto a un bando como otro.
###### +Poblado (GrabenVillage)
Una estructura que tiene 2 scripts:

     // Health //
     (Info extraída del propio script)
     Se asocia a cualquier objeto que pueda sufrir daños y ser destruido (instalaciones, unidades, pueblos, torres...)
     Es un gestor de la variable de vida del objeto.
     Health = 10
   -
   
     // Village //
     (Info extraída del propio script)
     Son edificios que las tropas pueden destruir, no se mueven ni atacan de ninguna manera.
     
     Tiene unas variables de control
     Max_Positions
     Radius
     Health
     AttackerUnits = cuantas unidades le están atacando en ese momento
     
     Tiene funciones para gestionar el recibir daño y destruirse
     OnTriggerEnter y DestroySelf
     
###### +Torres de combate (GrabenTower)
Una estructura de combate que tiene 2 scripts:

     // Health //
     Es un gestor de la variable de vida del objeto. 
     Health = 20
   -
    
      // Tower //
     (Info extraída del propio script)
     Son edificios que atacan a tropas de un bando u otro si se sienten invadidos.
     Disparan proyectiles, y también pueden ser destruidas por las tropas.
     
     Tiene unas variables de control
     Max_Positions
     Radius
     Health
     AttackerUnits
     
     AttackDamage = 1 (daño que realiza cada proyectil)
     AttackTime = 0.5 (cadencia)
     AttackDistance = 15     
     
     Tiene funciones para gestionar el recibir daño y destruirse
     OnTriggerEnter y DestroySelf
     
     Usa un ÁRBOL DE COMPORTAMIENTO -> DEFENSE
     Para decidir cómo y cuando defenderse.
     Los árboles se explicarán posteriormente en un apartado externo.
      
