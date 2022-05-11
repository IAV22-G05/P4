/*    
    Obra original:
        Copyright (c) 2018 Packt
        Unity 2018 Artificial Intelligence Cookbook - Second Edition, by Jorge Palacios
        https://github.com/PacktPublishing/Unity-2018-Artificial-Intelligence-Cookbook-Second-Edition
        MIT License

    Modificaciones:
        Copyright (C) 2020-2022 Federico Peinado
        http://www.federicopeinado.com

        Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
        Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
        Contacto: email@federicopeinado.com
*/
namespace es.ucm.fdi.iav.rts
{

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class GraphGrid : Graph
    {
        public float cellSize = 1f;
        [Range(0, Mathf.Infinity)]
        public float defaultCost = 1f;
        [Range(0, Mathf.Infinity)]
        public float maximumCost = Mathf.Infinity;

        protected int numCols;
        protected int numRows;
        protected GameObject[] vertexObjs;
        protected bool[,] mapVertices;

        //Convertidor de posicion a id para las listas
        protected int GridToId(int x, int y)
        {
            return Math.Max(numRows, numCols) * y + x;
        }

        //Convertidor de id a posicion 
        private Vector2 IdToGrid(int id)
        {
            Vector2 location = Vector2.zero;
            location.y = Mathf.Floor(id / numCols);
            location.x = Mathf.Floor(id % numCols);
            return location;
        }

        public override void Load()
        {
            //LoadMap(mapName);
        }

        //Se llama por cada vertice para setear sus vecinos
        protected void SetNeighbours(int x, int y, bool get8 = false)
        {
            //Obtenemos el vertice exacto
            int col = x;
            int row = y;
            int i, j;
            int vertexId = GridToId(x, y);

            //Introducimos una lista de vertices vecinos por cada nodo (creo que esto se hace arriba y realmente lo estamos sobrescribiendo)
            //Lo mismo para costes
            neighbors[vertexId] = new List<Vertex>();
            costs[vertexId] = new List<float>();

            //Creamos el array de posiciones vecinas
            Vector2[] pos = new Vector2[0];

            if (get8)
            {
                pos = new Vector2[8];
                int c = 0;
                for (i = row - 1; i <= row + 1; i++)
                {
                    for (j = col - 1; j <= col; j++)
                    {
                        pos[c] = new Vector2(j, i);
                        c++;
                    }
                }
            }
            else
            {
                //En principio deberia entrar aqui siempre
                //Cada vertice tiene 4 vecinos
                pos = new Vector2[4];
                pos[0] = new Vector2(col, row - 1);
                pos[1] = new Vector2(col - 1, row);
                pos[2] = new Vector2(col + 1, row);
                pos[3] = new Vector2(col, row + 1);
            }

            //Para cada posicion vecina comprobar que es valida
            //Que no esta fuera del mapa
            foreach (Vector2 p in pos)
            {
                i = (int)p.y;
                j = (int)p.x;
                if (i < 0 || j < 0)
                    continue;
                if (i >= numRows || j >= numCols)
                    continue;
                if (i == row && j == col)
                    continue;
                if (!mapVertices[i, j])
                    continue;

                //Si ha sido valida la introducimos definitivamente
                int id = GridToId(j, i);
                neighbors[vertexId].Add(vertices[id]);
                costs[vertexId].Add(defaultCost);
            }
        }

        //Devuelve el 
        public override Vertex GetNearestVertex(Vector3 position)
        {
            //Obtenemos el vertice de la posicion
            int col = (int)Math.Round(position.x / cellSize);
            int row = (int)Math.Round(position.z / cellSize);
            Vector2 p = new Vector2(col, row);

            //Creamos una lista de posiciones (nodos) explorados
            //Y una cola 
            List<Vector2> explored = new List<Vector2>();
            Queue<Vector2> queue = new Queue<Vector2>();
            queue.Enqueue(p);

            //
            do
            {
                p = queue.Dequeue();
                col = (int)p.x;
                row = (int)p.y;
                int id = GridToId(col, row);
                if (mapVertices[row, col])
                    return vertices[id];

                if (!explored.Contains(p))
                {
                    explored.Add(p);
                    int i, j;
                    for (i = row - 1; i <= row + 1; i++)
                    {
                        for (j = col - 1; j <= col + 1; j++)
                        {
                            if (i < 0 || j < 0)
                                continue;
                            if (j >= numCols || i >= numRows)
                                continue;
                            if (i == row && j == col)

                                queue.Enqueue(new Vector2(j, i));
                        }
                    }
                }
            } while (queue.Count != 0);
            return null;
        }

        public Vertex getRandomVertex()
        {
            Vertex v = null;
            int r = 0;
            Vector2 pos;
            do
            {
                r = UnityEngine.Random.Range(0, vertices.Count - 1);
                pos = IdToGrid(r);
                v = vertices[r];
            } while (vertexObjs[r].name == "ObstacleGrid" + r.ToString());

            if (v == null)
                Debug.Log("No funciona el random");
            return v;
        }
    }
}
