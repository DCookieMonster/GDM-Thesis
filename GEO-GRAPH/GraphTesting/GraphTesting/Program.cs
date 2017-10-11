using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphTesting
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Test 1
            //Graph graph = GraphML.Read("g5.xml", false);
            //Delimit();
            //AnalyzeGraph.PrintRoutes(graph);
            ////
            ////Console.Clear();
            ////nalyzeGraph.PrintRoutes(graph, "n14");
            ////
            //Delimit();
            ////Console.WriteLine("Guerentee Exist? {0} ", AnalyzeGraph.GuerenteeExist(graph));
            //Delimit();
            //string result = AnalyzeGraph.Play(graph, "n3,");
            //Console.WriteLine(result + " [Result: {0}]", (CountActions(result) % 2 == 0) ? "Lose." : "Win");
            //Delimit();


            //Test 2
            //Dictionary<int, int> restrictions = new Dictionary<int, int>();
            //restrictions.Add(0, 0);
            //Graph g = GraphGenerator.Generate("testGenerator2", 13, 3, 3, restrictions, true);
            //GraphML.Write(g);

            //Console.WriteLine("Done.");
            //Console.ReadLine();

            Form1 form = new Form1();
            form.ShowDialog();

        }

        private static void Delimit()
        {
            Console.WriteLine("------------------------------------");
        }

        private static int CountActions(string history)
        {
            return history.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }


    }
}
