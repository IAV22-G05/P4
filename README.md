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
     
#### Instalaciones
Las instalaciones son los edificios encargados de la logística del ejército.
Las instalaciones de las que dispone cada bando son las mismas, son estructuras para manejar recursos y tropas.
Hay 2 tipos de instalaciones, las "Base" y las de "Procesamiento". Ambas heredan de un script:

     // Facility Abstract Class //
     Es una clase base muy simple que solo gestiona la vida y destrucción de la instalación
     Health
     Max_Positions
     Radius
     
     También tiene una referencia al controlador al que va asociado (posterioremete se explica)
     RTSController Controller
     
     Tiene funciones para gestionar el recibir daño y destruirse
     OnTriggerEnter y DestroySelf
     
##### Instalación de Procesamiento
(Sacado del prototipo)
Es la refinería del ejército. Sirve para que las unidades de extracción de "Especia" vayan allí a descargar los recursos que han conseguido en los campos.
Esos recursos deberían poder convertirse en dinero aunque todavía no está implementado. 

Actualmente la instalación solo almacena "Especia".

Tiene un script que, como hemos dicho antes, hereda de "Facility"

     // Processing Facility //
     Resources (int) =  recursos de especia
     UnloadingTransform = posición a la que debe de ir las tropas a descargar la "especia"
     
##### Instalación Base
(Sacado del prototipo)
La instalación base hace las veces de barracón del ejército.
Tiene como función principal el crear nuevas unidades, que genera y situa de manera ordenada a su alrededor.

Tiene un script que, como hemos dicho antes, hereda de "Facility"

     // Base Facility //
     Transform SpawnTransform =  punto desde el que se empiezan a generar tropas (luego se generan las tropas por filas y columnas)
     UnitsPerRow = num unidades que PUEDEN aparecer e en una fila antes de pasar a la siguiente
     RowSpacing = distancia entre tropas por fila
     ColumnSpacing = distancia entre tropas por columna
     
     column y row (ints) =  conteo de las filas y columnas que se van usando
     
     Para la generación de unidades se usan funciones:
     -bool CanPlaceUnit() debería mirar si el espacio donde se va a poner una tropa está ocupado o no, aunque ahora devuelve siempre true
     
     -void PlaceUnit(RTSController Controller, Unit unit) 
          Comprueba que el controlador posea esa tropa (esté disponible en ese momento)
          Mira la fila y columna y situa la unidad (unit) en la posición adecuada
          
#### Unidades 
(Sacado del prototipo)
Las unidades son los agentes con movilidad y comportamientos inteligentes del ejército.
Tienen la responsabilidad de moverse, ya sea para obtener recursos o atacar,
y pueden sufrir daños e incluso ser destruidas por el enemigo o las amenazas del escenario.

Hay 3 tipos de unidades, extracción, exploración y destrucción. Todas heredan de un script:

     // Unit //
     Necesita obligatoriamente un componente Rigidbody y un Health
     
     Variables:
     Max_Positions =  igual que en todos, numero de enemigos que le pueden asaltar a la vez
     Radius
     Transform MoveTarget = posición a la que se debe mover la unidad (modificar para poder cambiarlo desde el árbol de comportamientos)
     Health
     int NextPosition = es un contador de siguientes posiciones visitadas
     
     BehaviourTree = cada tipo de unidad usa un arbol de comportamientos, que explicaremos más adelante.
     RTSController Controller = referencia al controlador asociado por unidad
     
     Se usan variables auxiliares de desplazamiento auxMoveTarget, para mover al agente entre las posiciones.
     
     Usa varias funciones:
     
     -VIRTUAL void Move(RTSController controller, Transform transform)
          De momento setea la siguiente posición de movimiento, MoveTarget. Cada unidad debe reinterpretarlo.
          
     -VIRTUAL void Stop(controller)
          Solicita la detención de esta unidad, anulando el objetivo de movimiento que pudiera haber establecido
          
     Usa funciones para gestionar la muerte, OnTriggerEnter, DestroySelf

##### Unidad de exploración
Las unidades de exploración son las más comunes y versátiles, ágiles y proactivas (persiguen, responden ataques...).
Atacan a todo enemigo que se encuentren ya sea del bando contrario o neutral, persiguen al enemigo hasta acabar con él.
Si otra tropa le agrede tiende a contestarla.

Se implementa con un script que herada de "Unit":

      // Exploration Unit //
      int attack_Damage = daño que causa al atacar
      int attack_Time = cadencia
      int attack_Distance
      
      Implementa varias funciones
      
      -bool IsMenaced()
          Indica si la unidad se siente amenazada (cuando ha perdido algo de vida)
          
     -Move()
          Llama al Move Base, asignando una posición en concreto a la que ir, el comportamiento se puede hacer algo más complejo.
     
     Se usa el árbol de comportamiento COMBAT.

          
##### Unidad de extracción
Las unidades de extracción sirven para obtener los recursos del escenario, no son capaces de atacar pero sí reciben daño.
Se mueven a la siguiente zona de "Especia" recogen el material y lo llevan de vuelta a la instalación de procesamiento.

Se implementa con un script que herada de "Unit":

      // Extraction Unit //
      int extractable_Amount = cantidad de recursos que puede extraer de golpe
      int Resources = recursos que transporta en ese momento
      
      Implementa 
          
     -Move()
          Llama al Move Base, asignando una posición en concreto a la que ir, el comportamiento se puede hacer algo más complejo.
          
     Se usa el árbol de comportamiento EXTRACTION.
     
##### Unidad de destrucción
Es similar a la exploradora, más poderosa y resistente, pero
también más lenta. Funciona de manera similar, aunque no persigue objetivos ni
contesta a agresores, tendiendo más a centrarse únicamente en su objetivo a abatir. pero
causa muchísimo más daño a los enemigos.

Se implementa con un script que herada de "Unit":

      // Destruction Unit //
      int attack_Damage = daño que causa al atacar
      int attack_Time = cadencia
      int attack_Distance
     
      Implementa 
          
     -bool IsMenaced()
          Indica si la unidad se siente amenazada, en este caso devuelve siempre false, pues nunca se va a sentir amenazada.
          
     -Move()
          Llama al Move Base, asignando una posición en concreto a la que ir, el comportamiento se puede hacer algo más complejo.
          
     -Attack()
          Como el Move pero indicando posiciones de enemigos.
          
          
     Se usa el árbol de comportamiento COMBAT.
                
     
     
     

     
      
