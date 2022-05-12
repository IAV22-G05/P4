using UnityEngine;

namespace es.ucm.fdi.iav.rts.G05
{
    public class RTSAIControllerG05: RTSAIController
    {
        public int MyIndex { get; set; }
        private int FirstEnemyIndex { get; set; }
        private BaseFacility MyFirstBaseFacility { get; set; }
        private ProcessingFacility MyFirstProcessingFacility { get; set; }
        private BaseFacility FirstEnemyFirstBaseFacility { get; set; }
        private ProcessingFacility FirstEnemyFirstProcessingFacility { get; set; }
        private InfluenceMap influenceMap;
        // N�mero de paso de pensamiento 
        private int ThinkStepNumber { get; set; } = 0;

        // �ltima unidad creada
        private Unit LastUnit { get; set; }

        // Despierta el controlado y configura toda estructura interna que sea necesaria
        private void Awake()
        {
            Name = "Final";
            Author = "Sergio Molinero-Andr�s Carnicero";
            influenceMap = GetComponent<InfluenceMap>();
            MyIndex = RTSGameManager.Instance.GetIndex(this);
        }

        // El m�todo de pensar que sobreescribe e implementa el controlador, para percibir (hacer mapas de influencia, etc.) y luego actuar.
        protected override void Think()
        {
            // Actualizo el mapa de influencia 
            influenceMap.ComputeInfluence();

            // Para las �rdenes aqu� estoy asumiendo que tengo dinero de sobra y que se dan las condiciones de todas las cosas...
            // (Ojo: esto no deber�a hacerse porque si me equivoco, causar� fallos en el juego... hay que comprobar que cada llamada tiene sentido y es posible hacerla)

            // Aqu� lo suyo ser�a elegir bien la acci�n a realizar. 
            // En este caso como es para probar, voy dando a cada vez una orden de cada tipo, todo de seguido y muy aleatorio... 
            switch (ThinkStepNumber)
            {
                case 0:
                    // Lo primer es conocer el �ndice que me ha asignado el gestor del juego

                    // Obtengo referencias a mis cosas
                    MyFirstBaseFacility = RTSGameManager.Instance.GetBaseFacilities(MyIndex)[0];
                    MyFirstProcessingFacility = RTSGameManager.Instance.GetProcessingFacilities(MyIndex)[0];

                    // Obtengo referencias a las cosas de mi enemigo
                    // ...
                    var indexList = RTSGameManager.Instance.GetIndexes();
                    indexList.Remove(MyIndex);
                    FirstEnemyIndex = indexList[0];
                    FirstEnemyFirstBaseFacility = RTSGameManager.Instance.GetBaseFacilities(FirstEnemyIndex)[0];
                    FirstEnemyFirstProcessingFacility = RTSGameManager.Instance.GetProcessingFacilities(FirstEnemyIndex)[0];

                    // Construyo por primera vez el mapa de influencia (con las 'capas' que necesite)
                    
                    break;

                case 1:

                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.EXPLORATION);
                    break;

                case 2:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.EXPLORATION);
                    break;

                case 3:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.DESTRUCTION);
                    break;

                case 4:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.DESTRUCTION);
                    break;

                case 5:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.EXTRACTION);
                    break;

                case 6:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.EXPLORATION);
                    break;

                case 7:
                    RTSGameManager.Instance.MoveUnit(this, LastUnit, MyFirstProcessingFacility.transform);
                    break;

                case 8:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.EXPLORATION);
                    break;

                case 9:
                    RTSGameManager.Instance.MoveUnit(this, LastUnit, MyFirstProcessingFacility.transform);
                    break;

                case 10:
                    LastUnit = RTSGameManager.Instance.CreateUnit(this, MyFirstBaseFacility, RTSGameManager.UnitType.EXPLORATION);
                    break;

                case 11:
                    RTSGameManager.Instance.MoveUnit(this, LastUnit, MyFirstProcessingFacility.transform);
                    // No lo hago... pero tambi�n se podr�an crear y mover varias unidades en el mismo momento, claro...
                    break;

                case 12:
                    Stop = true;
                    break;
            }
            ThinkStepNumber++;
        }
    }
}