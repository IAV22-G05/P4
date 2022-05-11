using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace es.ucm.fdi.iav.rts
{
    public class InfluenceMap : GraphGrid
    {
        [SerializeField]
        float strengthThreshold = 1;

        public override void Load()
        {
            numCols = (int)(RTSScenarioManager.Instance.Scenario.terrainData.size.x / 10);
            numRows = (int)(RTSScenarioManager.Instance.Scenario.terrainData.size.z / 10);

            vertices = new List<Vertex>(numRows * numCols);
            neighbors = new List<List<Vertex>>(numRows * numCols);
            costs = new List<List<float>>(numRows * numCols);
            vertexObjs = new GameObject[numRows * numCols];
            mapVertices = new bool[numRows, numCols];

            Vector3 position = Vector3.zero;
            int id;
            for (int i = 0; i < numRows; ++i)
            {
                for (int j = 0; j < numCols; ++j)
                {
                    position.x = j * cellSize;
                    position.z = i * cellSize;
                    id = GridToId(i, j);

                    vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity) as GameObject;
                    vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                    Vertex v = vertexObjs[id].AddComponent<Vertex>();
                    v.id = id;
                    v.strength = 5;
                    vertices.Add(v);
                    neighbors.Add(new List<Vertex>());
                    Debug.Log(id);
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
                start.strength = strengthFunction(start.controller, start) + 5;
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
                        if (neighborRecord.controller != v.controller && neighborRecord.strength < strength)
                            continue;
                    }
                    else if (open.Contains(v))
                    {
                        neighborRecord = open.Find(v);
                        if (neighborRecord.strength < strength)
                            continue;
                    }
                        
                    neighborRecord = new Vertex();
                    neighborRecord.controller = current.controller;
                    neighborRecord.strength = strength;
                    open.Add(neighborRecord);
                }

                open.Remove(current);
                closed.Add(current);
            }

            return closed;
        }

        public float strengthFunction(int controller, Vertex v)
        {
            float strength = 0;
            List<BaseFacility> bases = RTSGameManager.Instance.GetBaseFacilities(controller);
            if (bases.Count <= 0)
                return 0;

            Vector3 vertexPos = IdToGrid(v.id);
            // xd
            vertexPos.z = vertexPos.y;
            vertexPos.y = 0;
            for (int i = 0; i < bases.Count; ++i)
                strength += 5 / Vector3.Distance(vertexPos, bases[i].transform.position);

            List<DestructionUnit> destructionUnits = RTSGameManager.Instance.GetDestructionUnits(controller);
            for (int i = 0; i < destructionUnits.Count; ++i)
                strength += 3 / Vector3.Distance(vertexPos, destructionUnits[i].transform.position);

            List<ExplorationUnit> explorationUnits = RTSGameManager.Instance.GetExplorationUnits(controller);
            for (int i = 0; i < explorationUnits.Count; ++i)
                strength += 1 / Vector3.Distance(vertexPos, explorationUnits[i].transform.position);

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

                    //Azul
                    //5, 3, 1
                    float colorFactor = vertices[id].strength / 5;
                    Color newColor = new Color(0, colorFactor, 0, 0.3f);
                    mat.SetColor("_Color", newColor);
                }
            }
        }
    }
}
