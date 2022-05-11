using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace es.ucm.fdi.iav.rts.G05
{
    public class InfluenceMap : GraphGrid
    {
        struct LocationRecord
        {
            int controllerIndex;
            float strength;
        }

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
                    id = GridToId(j, i);

                    vertexObjs[id] = Instantiate(vertexPrefab, position, Quaternion.identity) as GameObject;
                    vertexObjs[id].name = vertexObjs[id].name.Replace("(Clone)", id.ToString());
                    Vertex v = vertexObjs[id].AddComponent<Vertex>();
                    v.id = id;
                    vertices.Add(v);
                    neighbors.Add(new List<Vertex>());
                } 
            }
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    SetNeighbours(j, i);
                }
            }
        }

        public void mapFloodDijkstra(int index)
        {
            BinaryHeap<Vertex> open = new BinaryHeap<Vertex>();
            BinaryHeap<Vertex> closed = new BinaryHeap<Vertex>();
            LocationRecord startRecord;
            for(int i = 0; i < 2; ++i)
            {

            }
        }
    }
}
