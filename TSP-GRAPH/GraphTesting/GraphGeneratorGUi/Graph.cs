using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphTesting
{
    public class Graph : ICloneable
    {
        //Graph Name
        public string name;
        // vertex->neighbours list
        public Dictionary<string, List<string>> adjList;

        public Graph(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Graph Name cannot be null nor empty");

            this.name = name;
            adjList = new Dictionary<string, List<string>>();
        }

        public Graph(Graph g)
        {
            this.name = g.name;
            adjList = new Dictionary<string, List<string>>(g.adjList);
        }

        public void AddEdge(string from, string to)
        {
            if(string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                throw new Exception("Source & Destination cannot be null nor empty");

            if (adjList.ContainsKey(from))
                adjList[from].Add(to);
            else
            {
                adjList.Add(from, new List<string>());
                adjList[from].Add(to);
            }
        }

        public void AddVertex(string v)
        {
            if (string.IsNullOrEmpty(v))
                throw new Exception("Vertex name cannot be null nor empty");

            if (!adjList.ContainsKey(v))
                adjList.Add(v, new List<string>());
        }

        public Graph Remove(string v)
        {
            Graph g = new Graph(name);
            g.adjList = new Dictionary<string, List<string>>(adjList);

            g.adjList.Remove(v);
            foreach (string key in g.adjList.Keys)
            {
                if (g.adjList[key].Contains(v))
                    g.adjList[key].Remove(v);
            }

            return g;
        }


        public object Clone()
        {
            Graph other = new Graph(this.name);
            other.adjList = new Dictionary<string, List<string>>(this.adjList);
            return other;
        }
    }
}
