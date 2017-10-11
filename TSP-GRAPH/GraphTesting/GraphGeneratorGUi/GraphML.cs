using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace GraphTesting
{
    public static class GraphML
    {
        /// <summary>
        /// Reads a graph from a given xml file. (Using GraphML Scheme)
        /// </summary>
        /// <param name="address">The location of the xml file.</param>
        /// <returns>The graph.</returns>
        public static Graph Read(string address)
        {
            return Read(address, false);
        }

        /// <summary>
        /// Reads a graph from a given xml file. (Using GraphML Scheme)
        /// </summary>
        /// <param name="address">The location of the xml file.</param>
        /// <param name="DEBUG_ON">Whether to show debug information in console.</param>
        /// <returns>The graph.</returns>
        public static Graph Read(string address, bool DEBUG_ON)
        {
            Graph graph = null;
            string sName = "";
            try
            {
                XmlTextReader reader = new XmlTextReader(address);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            sName = reader.Name;
                            switch (sName)
                            {
                                case "graph":
                                    
                                    graph = new Graph(reader.GetAttribute("id"));
                                    if(DEBUG_ON)
                                        Console.WriteLine("Graph: {0}", graph.name);
                                    break;
                                case "node":
                                    graph.AddVertex(reader.GetAttribute("id"));
                                    if (DEBUG_ON)
                                        Console.WriteLine("Vertex: {0}", reader.GetAttribute("id"));
                                    break;
                                case "edge":
                                    graph.AddEdge(reader.GetAttribute("source"), reader.GetAttribute("target"));
                                    if (DEBUG_ON)
                                        Console.WriteLine("Edge: from {0} to {1}", reader.GetAttribute("source"), reader.GetAttribute("target"));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Exception at GraphMLReader.Read: ", ex);
            }

            if (DEBUG_ON)
            {
                int vertices = graph.adjList.Keys.Count();
                int edges = 0;
                foreach (string key in graph.adjList.Keys)
                {
                    edges += graph.adjList[key].Count;
                }

                Console.WriteLine("Total: Graph {0}, {1} Vertices, {2} Edges.", graph.name, vertices, edges);
            }

            return graph;
        }

        /// <summary>
        /// Outputs a graph into an xml file. (Using GraphML Scheme)
        /// </summary>
        /// <param name="g">The graph.</param>
        public static void Write(Graph g)
        {
            StreamWriter sw = new StreamWriter(g.name + ".xml");

            sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
            sw.Write("<graphml xmlns=\"http://graphml.graphdrawing.org/xmlns\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://graphml.graphdrawing.org/xmlns   http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd\">\n");
            sw.Write("<graph id=\""+g.name+"\" edgedefault=\"directed\">\n");
            //Write Vertices
            foreach (string key in g.adjList.Keys)
            {
                sw.Write("  <node id=\"" + key + "\"/>\n");
            }
            //Write Edges
            foreach (string source in g.adjList.Keys)
            {
                foreach (string target in g.adjList[source])
                {
                    sw.Write("  <edge source=\""+source+"\" target=\""+target+"\"/>\n");
                }
            }
            sw.Write("</graph>\n");
            sw.Write("</graphml>");

            sw.Close();
        }
    }
}
