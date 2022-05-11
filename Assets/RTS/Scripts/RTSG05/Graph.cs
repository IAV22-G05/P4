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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>

    public abstract class Graph : MonoBehaviour
    {
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbors;
        protected List<List<float>> costs;
        //protected Dictionary<int, int> instIdToId;

        //// this is for informed search like A*
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;
        //public bool isFinished;
        private Vertex minotauroVertex;

        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }


        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbors, null) || neighbors.Count == 0)
                return new Vertex[0];
            if (v.id < 0 || v.id >= neighbors.Count)
                return new Vertex[0];
            return neighbors[v.id].ToArray();
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex[] neighbours;
            Queue<Vertex> q = new Queue<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id; // El vértice que tenga de previo a sí mismo, es el vértice origen
            q.Enqueue(src);
            while (q.Count != 0)
            {
                v = q.Dequeue();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id; // El vecino n tiene de 'padre' a v
                    q.Enqueue(n);
                }
            }
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            if (srcO == null || dstO == null)
                return new List<Vertex>();
            Vertex src = GetNearestVertex(srcO.transform.position);
            Vertex dst = GetNearestVertex(dstO.transform.position);
            Vertex[] neighbours;
            Vertex v;
            int[] previous = new int[vertices.Count];
            for (int i = 0; i < previous.Length; i++)
                previous[i] = -1;
            previous[src.id] = src.id;
            Stack<Vertex> s = new Stack<Vertex>();
            s.Push(src);
            while (s.Count != 0)
            {
                v = s.Pop();
                if (ReferenceEquals(v, dst))
                {
                    return BuildPath(src.id, v.id, ref previous);
                }

                neighbours = GetNeighbours(v);
                foreach (Vertex n in neighbours)
                {
                    if (previous[n.id] != -1)
                        continue;
                    previous[n.id] = v.id;
                    s.Push(n);
                }
            }

            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
        {
            //if (srcO == null || dstO == null)
            //    return new List<Vertex>();
            ////Cogemos los nodos de inicio y final
            //Vertex start = GetNearestVertex(srcO.transform.position);
            //Vertex end = GetNearestVertex(dstO.transform.position);

            //BinaryHeap<Vertex> open = new BinaryHeap<Vertex>();
            //// Vector con el indice que indica el vertice anterior para cada vértice
            //int[] previous = new int[vertices.Count];
            //// Vector que guarda el menor coste del inicio a cada vértice
            //float[] gScore = new float[vertices.Count];
            //// Vector que guarda el menor coste estimado de inicio a fin pasando por el vértice
            //float[] fScore = new float[vertices.Count];
            //for (int i = 0; i < previous.Length; i++)
            //{
            //    previous[i] = -1;
            //    gScore[i] = Mathf.Infinity;
            //    fScore[i] = Mathf.Infinity;
            //}
            //previous[start.id] = start.id;
            //gScore[start.id] = 0;
            //fScore[start.id] = h(start, end);

            //open.Add(start);
            //Vertex current;
            //float tentativeGScore;
            //while (open.Count > 0)
            //{
            //    current = open.Top;
            //    if (current == end)
            //        return BuildPath(start.id, end.id, ref previous);

            //    open.Remove();
            //    List<Vertex> neighbours = neighbors[current.id];
            //    foreach (Vertex v in neighbours)
            //    {
            //        tentativeGScore = gScore[current.id] + FindCost(current, v);
            //        if (tentativeGScore < gScore[v.id] && v != minotauroVertex)
            //        {
            //            previous[v.id] = current.id;
            //            gScore[v.id] = tentativeGScore;
            //            fScore[v.id] = tentativeGScore + h(v, end);
            //            if (!open.Contains(v))
            //                open.Add(v);
            //        }
            //    }
            //}
            return new List<Vertex>();
        }

        private float FindCost(Vertex from, Vertex to)
        {
            for (int i = 0; i < neighbors[from.id].Count; ++i)
                if (to.id == neighbors[from.id][i].id)
                    return costs[from.id][i];
            return 0;
        }

        public List<Vertex> Smooth(List<Vertex> path)
        {
            // Si el camino solo contiene dos nodos no puede suavizarlo.
            if (path.Count == 2)
                return path;

            // Compila un camino de salida.
            List<Vertex> outputPath = new List<Vertex>();
            outputPath.Add(path[0]);

            // Guarda la posición en el camino de entrada, empieza en 2 porque se assume que 2 nodos adyacentes pasarán el ray cast.
            int inputIndex = 2;

            // Itera hasta encontrar el último item de la entrada.
            while (inputIndex < path.Count)
            {
                // Ray cast.
                Vertex fromPt = outputPath[outputPath.Count - 1];
                Vertex toPt = path[inputIndex];
                Vector3 fromPos = fromPt.gameObject.transform.position;
                Vector3 toPos = toPt.gameObject.transform.position;
                fromPos.y += 1;
                toPos.y += 1;

                Vector3 dir = toPos - fromPos;
                //float distance = dir.magnitude;
                float distance = Vector3.Distance(fromPos, toPos);

                if (Physics.Raycast(fromPos, dir, distance))
                {
                    // Añade el último nodo que superó el ray cast al camino de salida.
                    outputPath.Add(path[inputIndex - 1]);

                    Debug.Log("Me cago en mi vieja deja de chocarte");
                }

                // Considera el siguiente nodo.
                inputIndex++;
            }

            // Añade el último nodo al camino de salida y lo devuelve.
            outputPath.Add(path[path.Count - 1]);

            //Devolvemos el camino suavizado
            return outputPath;
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();
            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }

        // Sí me parece razonable que la heurística trabaje con la escena de Unity
        // Heurística de distancia euclídea
        public float EuclidDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Vector3.Distance(posA, posB);
        }

        // Heurística de distancia Manhattan
        public float ManhattanDist(Vertex a, Vertex b)
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }

        public void UpdateCosts(Vertex last, Vertex next)
        {
            for (int i = 0; i < costs[last.id].Count; ++i)
                costs[last.id][i] = 1;

            for (int i = 0; i < costs[next.id].Count; ++i)
                costs[next.id][i] = 5;

            minotauroVertex = next;
        }
    }
}