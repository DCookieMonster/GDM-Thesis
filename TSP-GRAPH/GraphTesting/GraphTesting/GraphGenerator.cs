using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphTesting
{
    public static class GraphGenerator
    {

        /// <summary>
        /// Generates a graph randomly.
        /// </summary>
        /// <param name="name">The name of the graph.</param>
        /// <param name="nodes">Amount of nodes in the graph.</param>
        /// <param name="options">Amount of options to choose (from 'n0')</param>
        /// <param name="maxOutDegree">Maximum out-degree in the graph.</param>
        /// <param name="restrictions">A set of restrictions of the form {...(xi,yi),(xi+1,yi+1)...} s.t there will be at most yi nodes with out-degree of xi.</param>
        /// <param name="singleSolution">Whether to enforce that only one option guerrentees a win for the first player.</param>
        /// <returns>The graph.</returns>
        public static Graph Generate(string name, int nodes, int options, int maxOutDegree, Dictionary<int, int> restrictions, bool singleSolution)
        {
            bool accept = false;

            if (string.IsNullOrEmpty(name) || nodes <= 0 || options <= 0 || maxOutDegree <= 0)
            {
                throw new Exception("Invalid Input - GraphGenerator.Generate");
            }

            Graph graph = null;

            while (!accept)
            {

                graph = new Graph(name);
                LinkedList<int> nodesList = new LinkedList<int>();

                //Create Nodes
                Console.WriteLine("(0/3) Generating Vertices...");
                for (int i = 0; i < nodes; i++)
                {
                    graph.AddVertex(string.Format("n{0}", i.ToString()));
                    if (i > 0)
                        nodesList.AddLast(i);
                }

                //Connect Options
                Console.WriteLine("(1/3) Connecting Options...");
                for (int i = 1; i <= options; i++)
                {
                    graph.AddEdge("n0", string.Format("n{0}", i.ToString()));
                }

                //Generate Edges
                Console.WriteLine("(2/3) Generating Edges...");
                Random rand = new Random();
                for (int i = 1; i < nodes; i++)
                {
                    string currentNode = "n" + i.ToString();

                    //Randomly select current out degree while taking into consideration the restrictions
                    int outDegree = rand.Next(0, maxOutDegree + 1);
                    while (restrictions.ContainsKey(outDegree) && restrictions[outDegree] == 0)
                        outDegree = rand.Next(0, maxOutDegree + 1);
                    if (restrictions.ContainsKey(outDegree))
                        restrictions[outDegree]--;

                    //Targets is used in order to prevent duplicate x->y
                    HashSet<int> targets = new HashSet<int>();
                    for (int j = 0; j < outDegree; j++)
                    {
                        int targetNode = -1;

                        targetNode = rand.Next(1, nodes);
                        while (targetNode == i || targets.Contains(targetNode))
                            targetNode = rand.Next(1, nodes);

                        targets.Add(targetNode);

                        graph.AddEdge(currentNode, "n" + targetNode);
                    }


                }


                //Verify that there isn't any case of 0 indegree
                bool empty_indegree = true;
                foreach (string vertex in graph.adjList.Keys)
                {
                    // starting vertex "n0" can have an in-degree of 0
                    if (vertex != "n0")
                    {
                        empty_indegree = true;
                        foreach (string source in graph.adjList.Keys)
                        {
                            if (source != vertex && graph.adjList[source].Contains(vertex))
                            {
                                empty_indegree = false;
                                break;
                            }
                        }
                        if (empty_indegree)
                        {
                            break;
                        }
                    }
                }

                if (!empty_indegree)
                {

                    //Verify that only 1 option guerentee a Win for the first player.
                    Console.WriteLine("(3/3) Testing for single Solution...");

                    if (singleSolution)
                    {
                        AnalyzeGraph.ResetRoutes();
                        int countWin = 0;
                        foreach (string option in graph.adjList["n0"])
                        {
                            if (AnalyzeGraph.CheckIfGuerenteeWin(graph, option + ","))
                                countWin++;

                            //If there is more then one solution, try to generate another graph
                            if (countWin > 1)
                            {
                                break;
                            }
                        }
                        if (countWin == 1)
                        {
                            accept = true;
                        }
                        else
                        {
                            Console.WriteLine("Failed.");
                        }
                    }
                    else
                    {
                        accept = true;
                    }
                }

            }
            return graph;
        }
    }
}
