using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace es.ucm.fdi.iav.rts
{
    public class InfluenceMap : GraphGrid
    {
        List<Unit> units;
        [SerializeField]
        float strengthThreshold = 1;

        [SerializeField]
        //true Harkonen, false Fremen
        bool bando = true;

        const int MAX_STRENGTH = 5;

        public override void Load()
        {
            units = new List<Unit>();
            numCols = (int)(RTSScenarioManager.Instance.Scenario.terrainData.size.x / cellSize);
            numRows = (int)(RTSScenarioManager.Instance.Scenario.terrainData.size.z / cellSize);

            vertices = new List<Vertex>(numRows * numCols);
            neighbors = new List<List<Vertex>>(numRows * numCols);
            vertexObjs = new GameObject[numRows * numCols];

            Vector3 position = Vector3.zero;
            int id;
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numCols; ++j)
                {
                    position.x = j * cellSize + RTSScenarioManager.Instance.Scenario.GetPosition().x;
                    position.z = i * cellSize + RTSScenarioManager.Instance.Scenario.GetPosition().z;
                    id = GridToId(i, j);
                    bool isVertex = (RTSScenarioManager.Instance.Scenario.SampleHeight(position) == 0);
                    vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity) as GameObject;
                    vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                    Vertex v = vertexObjs[id].AddComponent<Vertex>();
                    v.id = id;
                    v.influence = 5;
                    vertices.Add(v);
                    neighbors.Add(new List<Vertex>());
                    if (!isVertex)
                    {
                        vertexObjs[id].SetActive(false);
                    }
                }
            }
            Influence_Debug();
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    SetNeighbours(j, i);
                }
            }

        }

        public BinaryHeap<Vertex> mapFloodDijkstra(int index)
        {
            BinaryHeap<Vertex> open = new BinaryHeap<Vertex>();
            BinaryHeap<Vertex> closed = new BinaryHeap<Vertex>();
            Vertex start;
            Vertex current;
            Vertex neighborRecord;
            List<BaseFacility> bases = RTSGameManager.Instance.GetBaseFacilities(index);
            float strength;

            for (int i = 0; i < bases.Count(); ++i)
            {
                start = GetNearestVertex(bases[i].transform.position);
                start.controller = index;
                start.influence = strengthFunction(start.controller, start) + 5;
                open.Add(start);
            }

            while (open.Count > 0)
            {
                current = open.Top;
                open.Remove();

                List<Vertex> neighbours = neighbors[current.id];

                foreach (Vertex v in neighbours)
                {
                    strength = strengthFunction(current.controller, v);

                    if (strength < strengthThreshold)
                        continue;
                    else if (closed.Contains(v))
                    {
                        neighborRecord = closed.Find(v);
                        if (neighborRecord.controller != v.controller && neighborRecord.influence < strength)
                            continue;
                    }
                    else if (open.Contains(v))
                    {
                        neighborRecord = open.Find(v);
                        if (neighborRecord.influence < strength)
                            continue;
                    }

                    neighborRecord = new Vertex();
                    neighborRecord.controller = current.controller;
                    neighborRecord.influence = strength;
                    open.Add(neighborRecord);
                }

                open.Remove(current);
                closed.Add(current);
            }

            return closed;
        }

        public void ComputeInfluence()
        {
            List<Vertex> open = new List<Vertex>();
            List<Vertex> closed = new List<Vertex>();
            List<Vertex> frontier;
            Vertex[] neighbours;

            foreach(Unit u in units)
            {
                Vertex v = GetNearestVertex(u.transform.position);
                open.Add(v);

                for(int i = 1; i <= u.Radius; ++i)
                {
                    frontier = new List<Vertex>();
                    foreach(Vertex o in open)
                    {
                        if (closed.Contains(o))
                            continue;
                        closed.Add(o);
                        vertices[o.id].influence += u.DropOff(i);
                        neighbours = GetNeighbours(v);
                        frontier.AddRange(neighbours);
                    }
                    open = new List<Vertex>(frontier);
                }
            }
        }

        public float strengthFunction(int controller, Vertex v)
        {
            float strength = 0;
            List<BaseFacility> bases = RTSGameManager.Instance.GetBaseFacilities(controller);
            if (bases.Count <= 0)
                return 0;

            Vector3 vertexPos = IdToGrid(v.id);
            vertexPos.z = vertexPos.y;
            vertexPos.y = 0;
            for (int i = 0; i < bases.Count; ++i)
                strength += 50 / Vector3.Distance(vertexPos, bases[i].transform.position);

            List<DestructionUnit> destructionUnits = RTSGameManager.Instance.GetDestructionUnits(controller);
            for (int i = 0; i < destructionUnits.Count; ++i)
                strength += 30 / Vector3.Distance(vertexPos, destructionUnits[i].transform.position);

            List<ExplorationUnit> explorationUnits = RTSGameManager.Instance.GetExplorationUnits(controller);
            for (int i = 0; i < explorationUnits.Count; ++i)
                strength += 10 / Vector3.Distance(vertexPos, explorationUnits[i].transform.position);

            return strength;
        }


        public void Influence_Debug()
        {
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numCols; ++j)
                {
                    //Setear el material en funcion de la fuerza de 
                    int id = GridToId(i, j);
                    Material mat = vertexObjs[id].GetComponent<MeshRenderer>().material;

                    float colorFactor = vertices[id].influence / MAX_STRENGTH;
                    float inverse = 1 - colorFactor;
                    Color newColor;

                    //Harkonen Azul
                    if (bando)
                        newColor = new Color(inverse, inverse, 1, 0.3f);
                    //Fremen Amarillo
                    else
                        newColor = new Color(1, 1, inverse, 0.3f);

                    mat.SetColor("_Color", newColor);
                }
            }
        }

        public void AddUnit(Unit u)
        {
            if (units.Contains(u))
                return;
            units.Add(u);
        }

        public void RemoveUnit(Unit u)
        {
            units.Remove(u);
        }
    }
}


