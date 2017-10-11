using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphTesting
{
    public static class AnalyzeGraph
    {
        /// <summary>
        /// All possible routes.
        /// </summary>
        private static Dictionary<string, bool> routes = null;

        /// <summary>
        /// Print any possible route.
        /// </summary>
        /// <param name="g">The graph.</param>
        public static void PrintRoutes(Graph g)
        {
            if (g == null)
                throw new Exception("Graph cannot be null");

            //Go through the options
            foreach (string option in g.adjList["n0"])
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("--------Option {0} -------", option);
                Console.WriteLine("-------------------------");
                DFS(g, option, new LinkedList<string>(), true, true);
                Console.WriteLine("-------------------------");
            }
        }

        /// <summary>
        /// Print any possible route starting from a specific option.
        /// </summary>
        /// <param name="g">The graph.</param>
        /// <param name="option">The vertex.</param>
        public static void PrintRoutes(Graph g, string option)
        {
            if (g == null)
                throw new Exception("Graph cannot be null");


            Console.WriteLine("-------------------------");
            Console.WriteLine("--------Option {0} -------", option);
            Console.WriteLine("-------------------------");
            DFS(g, option, new LinkedList<string>(), false, true);
            Console.WriteLine("-------------------------");
        }

        /// <summary>
        /// DFS scan in order to discover every game scenerio.
        /// </summary>
        /// <param name="g">The graph.</param>
        /// <param name="v">The starting vertex.</param>
        /// <param name="explored">List of explored vertices.</param>
        /// <param name="updateRoutes">Whether to update the routes data structure.</param>
        /// <param name="DEBUG_OUTPUT">Whether to print debug information on console.</param>
        public static void DFS(Graph g, string v, LinkedList<string> explored, bool updateRoutes, bool DEBUG_OUTPUT)
        {
            
            explored.AddLast(v);
            bool end = true;
            foreach (string adj in g.adjList[v])
            {
                if (!explored.Contains(adj))
                {
                    end = false;
                    DFS(g, adj, new LinkedList<string>(explored.AsEnumerable<string>()), updateRoutes, DEBUG_OUTPUT);
                }
            }

            if (end)
            {
                string ans = "";
                bool win = false;
                LinkedListNode<string> it = explored.First;
                while (it != null)
                {
                    win = win ^ true;
                    ans += string.Format("{0}", it.Value);
                    it = it.Next;
                    if (it != null)
                        ans += ",";
                }

                explored.Remove(v);

                if (updateRoutes)
                {
                    if (routes == null)
                        routes = new Dictionary<string, bool>();

                    routes.Add(ans, win);
                }

                if (DEBUG_OUTPUT)
                {
                    Console.WriteLine("{0} : {1}", (win) ? "Win!" : "Lose.", ans);
                }
            }

        }     


        /// <summary>
        /// This method produces all the options and the corresponding probabilites of wining
        /// according to the given history and the current node.
        /// </summary>
        /// <param name="history">The given history "nodeX,nY..."</param>
        /// <returns>Dictionary of string node, double P(Wining) </returns>
        public static Dictionary<string, double> MinimizeToOptions(string history)
        {
            string[] h = history.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            
            //Used to calculate according to player
            bool playerB = true;
            if (h.Length % 2 == 0)
                playerB = false;

            Dictionary<string, double> options = new Dictionary<string, double>();

            Dictionary<string, int> normalizeFactors = new Dictionary<string, int>();

            foreach (string route in routes.Keys)
            {
                if (history == "" || route.IndexOf(history) == 0)
                {
                    //The next action in the current node to consider
                    string action = route.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[h.Length];
                    
                    //Check if action is in history
                    bool exploited = false;
                    foreach(string sub in h){
                        if(sub.Equals(action)){
                            exploited = true;
                            break;
                        }
                    }
                    if(exploited)
                        continue;

                    //If its a new option
                    if (!options.ContainsKey(action))
                    {
                        options.Add(action, 0.0);
                        normalizeFactors.Add(action, 0);
                    }

                    //The xor is used to distinct between the wining of player 1 in contrast to the wining of player 2 
                    //(Inverts wining boolean values kept in routes dictionary)
                    bool routeResult = routes[route] ^ playerB;

                    //If its a win for the current player then count it as a win
                    if (routeResult)
                        options[action] += 1;

                    //normalizing counters
                    normalizeFactors[action]++;
                }

            }

            //Normalize the values
            foreach (string option in normalizeFactors.Keys)
            {
                options[option] = options[option] / normalizeFactors[option];
            }

            return options;
        }

        /// <summary>
        /// Choose the next action according to the given set of options.
        /// Choose the actions that maximizes P(Wining), in case of multiple choose randomly.
        /// </summary>
        /// <param name="options">Dictionary of string node, double P(Wining)</param>
        /// <returns>The selected option</returns>
        private static string ChooseOption(Dictionary<string, double> options)
        {
            double max = double.NegativeInfinity;
            HashSet<string> maxOptions = null;

            foreach(string option in options.Keys){
                if (options[option] > max)
                {
                    maxOptions = new HashSet<string>();
                    maxOptions.Add(option);

                    max = options[option];
                }
                else if (options[option] == max)
                    maxOptions.Add(option);

                
            }

            Random rand = new Random();

            return maxOptions.ToArray<string>()[rand.Next(0, maxOptions.Count)];
        }

        /// <summary>
        /// Simulates the game between two players.
        /// </summary>
        /// <param name="g">The graph being used.</param>
        /// <param name="history">Optional, In case we want to simulate after a set of selections, Empty String otherwise.</param>
        /// <returns>A string that specifies the path nX,...,nY</returns>
        public static string Play(Graph g, string history)
        {
            string ans = history;

            bool won = false;
            bool playerA = history.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Length%2==0;
            while (!won)
            {
                Dictionary<string, double> options = MinimizeToOptions(ans);

                string action = "";

                if (options.Count == 0)
                    break;
                else
                    action = ChooseOption(options);

                ans += string.Format("{0},", action);



                Console.WriteLine("{0} chose {1}", (playerA) ? "Player A" : "Player B" , action);
                playerA = playerA ^ true;
            }

            return ans;
        }


        //True - Win, False - Lose
        /// <summary>
        /// This method is similar to "Play" and checks if from a given history there is a guerentee win for the first player.
        /// </summary>
        /// <param name="g">The graph</param>
        /// <param name="history">The given history in form of "ni,nj,nk,..."</param>
        /// <returns>True if there is a guerentee win, False otherwise.</returns>
        public static bool CheckIfGuerenteeWin(Graph g, string history)
        {
            if (g == null)
                throw new Exception("Graph cannot be null");

            if (routes == null)
            {
                //Go through the options
                foreach (string option in g.adjList["n0"])
                {
                    DFS(g, option, new LinkedList<string>(), true, false);
                }
            }

            string ans = history;

            bool won = false;
            bool playerA = history.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length % 2 == 0;
            while (!won)
            {
                Dictionary<string, double> options = MinimizeToOptions(ans);

                string action = "";

                if (options.Count == 0)
                    break;
                else
                    action = ChooseOption(options);

                ans += string.Format("{0},", action);

                playerA = playerA ^ true;
            }

            if (ans.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length % 2 == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// This method clears the routes dictionary (needed when working on a different graph)
        /// </summary>
        public static void ResetRoutes()
        {
            if (routes != null)
            {
                routes.Clear();
                routes = null;
            }
        }
    }
}
