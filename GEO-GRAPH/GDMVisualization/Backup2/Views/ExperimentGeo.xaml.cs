using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Xml;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Browser;
using NLog;
using System.Reflection;
using System.Windows.Controls.Primitives;
using NLog.Targets;



namespace GDMVisualization
{
    /// <summary>
    /// In order to use experimentGeo  you will need a graph created in yEd that
    /// has a “wining option” marked in the xml with <node id=”ni” win=”true”>
    /// and one start node indicated by the id “n0”.
    /// 
    /// Most of the code is replicated from Experiment, addons are fully documented.
    /// </summary>
    public partial class ExperimentGeo : Page
    {

        #region Internal Global Attributes and Properties

        #region Geography Game Addons
        /*   Constants   */
        private const string START_NODE = "n0";
        private static Color HIGHLIGHT_EDGE = Colors.Yellow;
        private static Color DEFAULT_EDGE = Colors.White;
        private static Color PLAYER_A_COLOR = Colors.Green;
        private static Color PLAYER_B_COLOR = Colors.Blue;

        private const double ARROW_HEIGHT = 15.0;
        private const double ARROW_WIDTH = 3.0;

        //which folder holds the graphs
        private const String GRAPH_FOLDER = "GDMVisualization.Resources";
        //which graph to choose from the folder GDMVisualization.Resources
        private const String GRAPH ="g5.graphml";

        //Wining Option - The <node> tag that has the attribute win="true", f.e: <node id="n5" win="true">
        private string win_option = "";

        //Player Indicator
        private string player = "A";

        //History Stack - used for UNDO
        Stack<string> history = new Stack<string>();

        // Arrow Heads Dictionary <id, polygon>
        Dictionary<string, Polygon> m_arrow_list = new Dictionary<string, Polygon>();

        // For every edge keeps source and destination
        Dictionary<string, Tuple<string, string>> edgesInformation = new Dictionary<string, Tuple<string, string>>();

        #endregion

        // Current experiment information retrieved from the DB        
        string ExperimentGraph;
        byte[] SolutionGraph;

        // Defines if to run the experiment with solution graph 
        bool load_solution_graph;

        // Thread to show the correct answer to the user
        int timeToAnswer = 3;// Time to pass before showing the correct answer to the user        
        DispatcherTimer myDispatcherTimer = new DispatcherTimer();
        int timer = 0;// Counts the time passed        

        // Lists to hold the graph elements
        List<VertexControl> m_ellipse_list = new List<VertexControl>();
        List<Line> m_line_list = new List<Line>();

        int vertex_radius;// the radius for all the vertex in the graph

        // Mouse coordinates on the grid at any given moment
        private double _mX = 0.0;
        private double _mY = 0.0;

        // Image for the graph solution as retrivede from the DB
        BitmapImage imageSource = new BitmapImage();

        // The element that was Right clicked - Used to determine which element to manipulate accross functions
        UIElement toManipulate;        

        // For each Vertex holds all the Edges connected to it
        Dictionary<string, List<string>> vertexEdges = new Dictionary<string, List<string>>();      
     
        // My screen resolution (with which the graphs were created)
        const double my_width_resolution = 1500.00;
        const double my_height_resolution = 643.00;
        
        // Dictonary of thicker edges
        Dictionary<string, List<string>> thickerEdges = new Dictionary<string, List<string>>();

        // Enables to write user ID to subject's disk (instead of Session Object)
        IsolatedStorageSettings i_storage = IsolatedStorageSettings.ApplicationSettings;   

        // Add refernce to Web Service        
        ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        
        //
        DateTime m_StartTime;

        // Current experiment id
        int experimentID;

        #endregion

        

        #region Constructors

        public ExperimentGeo()
        {
            InitializeComponent();

            //where to put the submit button
            double screen_resolution = double.Parse(HtmlPage.Document.Body.GetProperty("clientWidth").ToString());
            double screen_resolution_y = double.Parse(HtmlPage.Document.Body.GetProperty("clientHeight").ToString()) - 80;   
            double x = screen_resolution - ((Button)canvas1.FindName("SubmitExperimentBtn")).Width-8;
            double y = 8;
            double x_text = x/5;
            double x_radio = x_text*0.8;
            double y_res = 50;

            m_StartTime = new DateTime();
            m_StartTime = DateTime.Now;

            //whete to put all the buttons and text on the cnavas.
            ((RadioButton)canvas1.FindName("radioButton1")).SetValue(Canvas.LeftProperty, x_radio);
            ((RadioButton)canvas1.FindName("radioButton1")).SetValue(Canvas.TopProperty, y);
            ((RadioButton)canvas1.FindName("radioButton2")).SetValue(Canvas.LeftProperty, x_radio+100);
            ((RadioButton)canvas1.FindName("radioButton2")).SetValue(Canvas.TopProperty, y);
            ((RadioButton)canvas1.FindName("radioButton3")).SetValue(Canvas.LeftProperty, x_radio+200);
            ((RadioButton)canvas1.FindName("radioButton3")).SetValue(Canvas.TopProperty, y);
            ((TextBlock)canvas1.FindName("textBlock")).SetValue(Canvas.LeftProperty, x_text);
            ((TextBlock)canvas1.FindName("textBlock")).SetValue(Canvas.TopProperty,y); 
            ((Button)canvas1.FindName("SubmitExperimentBtn")).SetValue(Canvas.LeftProperty, x);
            ((Button)canvas1.FindName("SubmitExperimentBtn")).SetValue(Canvas.TopProperty, y);
            ((Button)canvas1.FindName("resetBtn")).SetValue(Canvas.LeftProperty, y);
            ((Button)canvas1.FindName("resetBtn")).SetValue(Canvas.TopProperty, y_res);
            ((Button)canvas1.FindName("SubmitExperimentBtn")).SetValue(Canvas.LeftProperty, x);
            ((Button)canvas1.FindName("SubmitExperimentBtn")).SetValue(Canvas.TopProperty, y);
           // SubmitExperimentBtn.Visibility = System.Windows.Visibility.Collapsed;
          
            //test (?)
            textBlock.Visibility = Visibility.Collapsed;
            radioButton1.Visibility = Visibility.Collapsed;
            radioButton2.Visibility = Visibility.Collapsed;
            radioButton3.Visibility = Visibility.Collapsed;
            SubmitExperimentBtn.Visibility = Visibility.Collapsed;


            //Green's / Blue's trun
            ((TextBlock)canvas1.FindName("GreenBox")).SetValue(Canvas.LeftProperty, x/2);
            ((TextBlock)canvas1.FindName("GreenBox")).SetValue(Canvas.TopProperty, screen_resolution_y);
            ((TextBlock)canvas1.FindName("BlueBox")).SetValue(Canvas.LeftProperty, x/2);
            ((TextBlock)canvas1.FindName("BlueBox")).SetValue(Canvas.TopProperty, screen_resolution_y);
            BlueBox.Visibility = System.Windows.Visibility.Collapsed;
            
            

            IDictionary<string, string> QueryString = HtmlPage.Document.QueryString;
            string workerId="";

            Logger loggerGDM = LogManager.GetCurrentClassLogger();
            //Logger logger = LogManager.GetLogger("GDM");
            if (QueryString.ContainsKey("workerId"))
                workerId = QueryString["workerId"];
            bool logging = LogManager.IsLoggingEnabled();
            
          

            // Add a new user to the DB 
            client.AddParticipantCompleted += new EventHandler<ServiceReference1.AddParticipantCompletedEventArgs>(client_AddParticipantCompleted);
            client.AddParticipantAsync(workerId);
            
            // Gets the id for the current experiment
            client.GetCurrentExperimentIDCompleted += new EventHandler<ServiceReference1.GetCurrentExperimentIDCompletedEventArgs>(client_GetCurrentExperimentIDCompleted);
            client.GetCurrentExperimentIDAsync();   

            // Check if this experiment needs to run with a solution - load the solution if needed
            client.GetUseSolutionGraphCompleted += new EventHandler<ServiceReference1.GetUseSolutionGraphCompletedEventArgs>(client_GetUseSolutionGraphCompleted);
            client.GetUseSolutionGraphAsync();

            // Get graph for the current experiment
            client.GetExperimentGraphCompleted += new EventHandler<ServiceReference1.GetExperimentGraphCompletedEventArgs>(client_GetExperimentGraphCompleted);
            client.GetExperimentGraphAsync();                                    

            // Synchronize tables in both DB's            
            //client.PerformBulkCopyAsync();
        }                 
       
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e) {}
       
        #endregion


        #region Private Methods       

        /// <summary>
        /// Gets current experiment id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_GetCurrentExperimentIDCompleted(object sender, ServiceReference1.GetCurrentExperimentIDCompletedEventArgs e)
        {
         /*   if (e.Error == null)
            {
                experimentID = e.Result;
            }
            else
                experimentID = 13;
          */
            experimentID = 0;
        }    

        /// <summary>
        /// When Webservice completes writing userID to the DB and returns the id 
        /// writes it to Isolated Storage on user disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_AddParticipantCompleted(object sender, ServiceReference1.AddParticipantCompletedEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
        //    SubmitExperimentBtn.Visibility = Visibility.Visible;
            int userID;
            if (e.Error == null)
            {
                userID = e.Result;
                logger.Trace("userID: " + userID);
                // Write user ID to IsolatedStorage
                if (!i_storage.Contains("userID"))
                {
                    i_storage.Add("userID", userID);
                 //   SubmitExperimentBtn.Content = "if" + i_storage["userID"].ToString() ;
                }
                else
                {
                    i_storage.Remove("userID");
                    i_storage.Add("userID", userID);
                    
                  //  SubmitExperimentBtn.Content = "else" + i_storage["userID"].ToString();
                }
            }

           
            //SubmitExperimentBtn.Visibility = System.Windows.Visibility.Visible;
            //SubmitExperimentBtn.Content = e.Result.ToString()+"here";
        }

        /// <summary>
        /// Loads the graph for the current experiment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_GetExperimentGraphCompleted(object sender, ServiceReference1.GetExperimentGraphCompletedEventArgs e)
        {
            //removed db reading graph since not always working
            if (e.Error == null)
            {
                ExperimentGraph = e.Result;
                // Load the graph for the current experiment (as recieved from the DB)
                if (!ExperimentGraph.StartsWith("error"))
                    LoadExperimentGraph();
                else
                {
                    LoadExperimentGraph();
                    //instead, read from local file

                   // string result = string.Empty;

                   //// StreamResourceInfo info = Application.GetResourceStream(new Uri(@"/Assets/" + "14graph.xml", UriKind.Relative));
                   // //StreamResourceInfo info =  Application.GetResourceStream(new Uri("/GDMVisualization;component/14graph.xml", UriKind.Relative));
                   // Uri u = new Uri("Resources/ExperimentGraph.xml", UriKind.Relative);
                   // StreamResourceInfo st = Application.GetResourceStream(new Uri("Resources/ExperimentGraph.xml", UriKind.Relative));
                    
                    
                   //     using (StreamReader sr = new StreamReader(st.Stream))
                   //     {
                   //         result = sr.ReadToEnd();
                   //     }
                    

                   // ExperimentGraph = result;
                    //client.GetExperimentGraphAsync();
                }
                //SubmitExperimentBtn.Visibility = System.Windows.Visibility.Visible;
                //SubmitExperimentBtn.Content = e.Result.ToString();
            }

            else
            {
                //instead, read from local file
                client.GetExperimentGraphAsync();
            }
            

        
            //ExperimentGraph = System.IO.File.ReadAllText(Resources.);
        }
        
        /// <summary>
        /// Checks if there is a solution graph for the current experiment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_GetUseSolutionGraphCompleted(object sender, ServiceReference1.GetUseSolutionGraphCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    //load_solution_graph = e.Result;                
                    timeToAnswer = int.Parse(e.Result);
                    if (timeToAnswer == 0)
                        load_solution_graph = false;
                    else
                        load_solution_graph = true;

                    // Load the solution graph for the current experiment (as recieved from the DB)                
                    if (load_solution_graph == true)
                    {
                        // Get solution graph for the current experiment
                        client.GetSolutionGraphCompleted += new EventHandler<ServiceReference1.GetSolutionGraphCompletedEventArgs>(client_GetSolutionGraphCompleted);
                        client.GetSolutionGraphAsync();
                    }
                }
            }
            catch
            {
                load_solution_graph = false;
            }
        }

        /// <summary>
        /// Loads the solution graph for the current experiment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_GetSolutionGraphCompleted(object sender, ServiceReference1.GetSolutionGraphCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SolutionGraph = e.Result;
                // Load the solution graph for the current experiment (as recieved from the DB)
                LoadSolutionGraph();
            }
        }
               
        /// <summary>
        /// Loads Solution Graph as defined in the DB
        /// </summary>
        private void LoadSolutionGraph()
        {
            if (SolutionGraph != null)
            {
                Stream stream = new MemoryStream(SolutionGraph);
                imageSource.SetSource(stream);
            }
        }    
        
        /// <summary>
        ///  Loads Experiment graph as defined in the DB 
        ///  For graphs created in GraphCreator
        /// </summary>
        private void LoadExperimentGraphMyFormat()
        {
            // holds the final vertex and edge coordinate lists
            Dictionary<string, Point> vertex_coordinate_list = new Dictionary<string, Point>();
            List<EdgePoints> edge_coordinate_list = new List<EdgePoints>();

            ///----------------------------------- Vertex Begin -----------------------------------///
            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(ExperimentGraph)))
            {
                string node_id;
                Double node_x, node_y;
                while (reader.ReadToFollowing("Vertex"))
                {
                    reader.MoveToFirstAttribute();
                    node_id = reader.Value;

                    reader.ReadToFollowing("Coordinate");
                    reader.MoveToAttribute("X");
                    node_x = reader.ReadContentAsDouble();
                    reader.MoveToAttribute("Y");
                    node_y = reader.ReadContentAsDouble();

                    vertex_coordinate_list.Add(node_id, new Point(node_x, node_y));

                    // Add all the Vertecies to the Collection as Keys
                    vertexEdges.Add(node_id, new List<string>());
                }
            }
            ///----------------------------------- Vertex End -----------------------------------///

            ///----------------------------------- Edge Begin -----------------------------------///
            using (XmlReader reader = XmlReader.Create(new StringReader(ExperimentGraph)))
            {
                string edge_id, edge_source, edge_target;                
                Point edge_start, edge_end;
                double edge_x, edge_y;

                while (reader.ReadToFollowing("Edge"))
                {
                    reader.MoveToFirstAttribute();
                    edge_id = reader.Value;

                    //reader.MoveToAttribute("Coordinate");
                    reader.MoveToAttribute("X1");
                    edge_x = reader.ReadContentAsDouble();
                    reader.MoveToAttribute("Y1");
                    edge_y = reader.ReadContentAsDouble();                    
                    edge_start = new Point(edge_x, edge_y);

                    reader.MoveToAttribute("X2");
                    edge_x = reader.ReadContentAsDouble();
                    reader.MoveToAttribute("Y2");
                    edge_y = reader.ReadContentAsDouble();
                    edge_end = new Point(edge_x, edge_y);

                    edge_coordinate_list.Add(new EdgePoints(edge_start, edge_end, edge_id));

                    // Add the Edge to both Vertex
                    //vertexEdges[edge_source].Add(edge_id);
                    //vertexEdges[edge_target].Add(edge_id);
                }
            }
            ///----------------------------------- Edge end -----------------------------------///                

            // TODO : obtain from yEd 
            vertex_radius = 45;

            // draw the graph according to the cordinates                
            DrawGraph(vertex_coordinate_list, edge_coordinate_list);
        }

        /// <summary>
        /// Loads Experiment graph as defined in the DB
        /// For graphs created in yEd
        /// </summary>         
        private void LoadExperimentGraph()
        {
            // holds the final vertex and edge coordinate lists
            Dictionary<string, Point> vertex_coordinate_list = new Dictionary<string, Point>();
            List<EdgePoints> edge_coordinate_list = new List<EdgePoints>();
                 
            ///----------------------------------- Vertex Begin -----------------------------------///
            // Create an XmlReader
            //if (!ExperimentGraph.StartsWith("error"))
            //{
            //    using (XmlReader reader = XmlReader.Create(new StringReader(ExperimentGraph)))
            //    {
            //        string node_id;
            //        Double node_x, node_y;
            //        while (reader.ReadToFollowing("node"))
            //        {
            //            reader.MoveToFirstAttribute();
            //            node_id = reader.Value;

            //            reader.ReadToFollowing("y:Geometry");
            //            reader.MoveToAttribute("x");
            //            node_x = reader.ReadContentAsDouble();
            //            reader.MoveToAttribute("y");
            //            node_y = reader.ReadContentAsDouble();

            //            vertex_coordinate_list.Add(node_id, new Point(node_x, node_y));

            //            // Add all the Vertecies to the Collection as Keys
            //            vertexEdges.Add(node_id, new List<string>());
            //        }
            //    }


            //    ///----------------------------------- Vertex End -----------------------------------///

            //    ///----------------------------------- Edge Begin -----------------------------------///
            //    using (XmlReader reader = XmlReader.Create(new StringReader(ExperimentGraph)))
            //    {
            //        string edge_id, edge_source, edge_target;
            //        Point edge_start, edge_end;

            //        while (reader.ReadToFollowing("edge"))
            //        {
            //            reader.MoveToFirstAttribute();
            //            edge_id = reader.Value;

            //            reader.MoveToAttribute("source");
            //            edge_source = reader.Value;
            //            edge_start = vertex_coordinate_list[edge_source];

            //            reader.MoveToAttribute("target");
            //            edge_target = reader.Value;
            //            edge_end = vertex_coordinate_list[edge_target];

            //            edge_coordinate_list.Add(new EdgePoints(edge_start, edge_end, edge_id));

            //            // Add the Edge to both Vertex
            //            vertexEdges[edge_source].Add(edge_id);
            //            vertexEdges[edge_target].Add(edge_id);
            //        }
            //    }
            //    ///----------------------------------- Edge end -----------------------------------///                

            //}
            //else
            //{
                
                //StreamResourceInfo st = Application.GetResourceStream(new Uri("/Resources/tutorial.xml", UriKind.Relative));
            
            //load the graph to a stream of assembly
                Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(GRAPH_FOLDER+"."+GRAPH);
                System.IO.Stream fileStream = st;
                //System.IO.StreamReader fileStream = new StreamReader("/Tutorial Graphs/unnamed0.graphml");
                byte[] textBytes = new byte[fileStream.Length];
                fileStream.Read(textBytes, 0, textBytes.Length);
                string graph_xml = UTF8Encoding.UTF8.GetString(textBytes, 0, textBytes.Length);

                using (XmlReader reader = XmlReader.Create(new StringReader(graph_xml)))
                {
                    string node_id;
                    Double node_x, node_y;
                    while (reader.ReadToFollowing("node"))
                    {
                        reader.MoveToFirstAttribute();
                        node_id = reader.Value;

                        if (reader.HasAttributes && (reader.GetAttribute("win") == "true"))
                        {
                            win_option = node_id;
                        }


                        reader.ReadToFollowing("y:Geometry");
                        reader.MoveToAttribute("x");
                        node_x = reader.ReadContentAsDouble();
                        reader.MoveToAttribute("y");
                        node_y = reader.ReadContentAsDouble();

                        vertex_coordinate_list.Add(node_id, new Point(node_x, node_y));

                        // Add all the Vertecies to the Collection as Keys
                        vertexEdges.Add(node_id, new List<string>());
                    }
                }


                ///----------------------------------- Vertex End -----------------------------------///

                ///----------------------------------- Edge Begin -----------------------------------///
                using (XmlReader reader = XmlReader.Create(new StringReader(graph_xml)))
                {
                    string edge_id, edge_source, edge_target;
                    Point edge_start, edge_end;

                    while (reader.ReadToFollowing("edge"))
                    {
                        reader.MoveToFirstAttribute();
                        edge_id = reader.Value;

                        reader.MoveToAttribute("source");
                        edge_source = reader.Value;
                        edge_start = vertex_coordinate_list[edge_source];

                        reader.MoveToAttribute("target");
                        edge_target = reader.Value;
                        edge_end = vertex_coordinate_list[edge_target];

                        edge_coordinate_list.Add(new EdgePoints(edge_start, edge_end, edge_id));

                        //Keeps Source and Destination vertices ids for every edge
                        edgesInformation.Add(edge_id, new Tuple<string,string>(edge_source, edge_target));

                        // Add the Edge to one Vertex (Directed Graph)
                        vertexEdges[edge_source].Add(edge_id);
                    }
                }
        //    }
            // TODO : obtain from yEd 
            vertex_radius = 45;

            // draw the graph according to the cordinates                
            DrawGraph(vertex_coordinate_list, edge_coordinate_list);
        }
       
        /// <summary>
        /// Draws the graph according to the input XML file
        /// </summary>
        /// <param name="vertex_cordinate_list">A list of the Vertex cordinates</param>
        /// <param name="edge_cordinate_list">A list of the Edge cordinates</param>        
        private void DrawGraph(Dictionary<string, Point> vertex_cordinate_list, List<EdgePoints> edge_cordinate_list)        
        {                                    
            // Get current user screen resolution                        
			double resolution_height = double.Parse(HtmlPage.Document.Body.GetProperty("clientHeight").ToString()) - 80;			
            double resolution_width = double.Parse(HtmlPage.Document.Body.GetProperty("clientWidth").ToString());          

            // Calculate proportions between reasercher's and client's resolutions
            double proportion_width = resolution_width / my_width_resolution;// Decrease Width      
            double proportion_height = resolution_height / my_height_resolution;// Decrease Height

            ////-------------------- VERTEX --------------------////
            for (int i = 0; i < vertex_cordinate_list.Count; i++)
            {
                // set Vertex appearence
                m_ellipse_list.Add(new VertexControl("dor"));
                m_ellipse_list[i].Height = vertex_radius;
                m_ellipse_list[i].Width = vertex_radius;
                m_ellipse_list[i].SetValue(Canvas.ZIndexProperty, 10);
                m_ellipse_list[i].Green_Ellipse.Visibility = Visibility.Collapsed;
                m_ellipse_list[i].Purple_Ellipse.Visibility = Visibility.Collapsed;
                m_ellipse_list[i].VertexID = vertex_cordinate_list.ElementAt(i).Key;
                m_ellipse_list[i].VertexColor = "red";
                m_ellipse_list[i].Vertex_X = vertex_cordinate_list.ElementAt(i).Value.X * proportion_width;
                m_ellipse_list[i].Vertex_Y = vertex_cordinate_list.ElementAt(i).Value.Y * proportion_height;

                // set Vertex location on Canvas 
                Canvas.SetLeft(m_ellipse_list[i], vertex_cordinate_list.ElementAt(i).Value.X * proportion_width);// X property
                Canvas.SetTop(m_ellipse_list[i], vertex_cordinate_list.ElementAt(i).Value.Y * proportion_height);// Y property				

                // set event handlers to Vertex 
                m_ellipse_list[i].MouseLeftButtonDown += new MouseButtonEventHandler(ellipse_MouseLeftButtonDown);

                // add the Vertex to the Canvas
                canvas1.Children.Add(m_ellipse_list[i]);
            }

            ////-------------------- EDGE --------------------////
            for (int i = 0; i < edge_cordinate_list.Count; i++)
            {
                // set Edge appearence
                m_line_list.Add(new Line());
                m_line_list[i].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                m_line_list[i].StrokeThickness = 4;
                m_line_list[i].Name = edge_cordinate_list[i].id;// Get Vertex name from parser 

                //Arrows
                string arrow_id = edge_cordinate_list[i].id;
                m_arrow_list.Add(arrow_id, new Polygon());
                m_arrow_list[arrow_id].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                m_arrow_list[arrow_id].StrokeThickness = 4;
                m_arrow_list[arrow_id].Name = arrow_id + "_arrow";


                #region set Edge location on Canvas - with setting location to middle of Vertex
                for (int j = 0; j < vertex_cordinate_list.Count; j++)
                {
                    if (Math.Abs(vertex_cordinate_list.ElementAt(j).Value.X - edge_cordinate_list[i].x1) < vertex_radius)
                        if (Math.Abs(vertex_cordinate_list.ElementAt(j).Value.Y - edge_cordinate_list[i].y1) < vertex_radius)
                        {
                            m_line_list[i].X1 = vertex_cordinate_list.ElementAt(j).Value.X * proportion_width + vertex_radius / 2;
                            m_line_list[i].Y1 = vertex_cordinate_list.ElementAt(j).Value.Y * proportion_height + vertex_radius / 2;
                        }
                    if (Math.Abs(vertex_cordinate_list.ElementAt(j).Value.X - edge_cordinate_list[i].x2) < vertex_radius)
                        if (Math.Abs(vertex_cordinate_list.ElementAt(j).Value.Y - edge_cordinate_list[i].y2) < vertex_radius)
                        {
                            m_line_list[i].X2 = vertex_cordinate_list.ElementAt(j).Value.X * proportion_width + vertex_radius / 2;
                            m_line_list[i].Y2 = vertex_cordinate_list.ElementAt(j).Value.Y * proportion_height + vertex_radius / 2;
                        }
                }
                #endregion

                #region Set Arrow Head Position
                //I address the edge as a linear line y=mx+n
                //Assuming edge lengths will not be 0.
                //m
                double slope = (m_line_list[i].Y2 - m_line_list[i].Y1) / (m_line_list[i].X2 - m_line_list[i].X1);
                //n
                double cutting_point = m_line_list[i].Y2 - slope * m_line_list[i].X2;

                //Arrow Head
                //Aligning the arrow to the border of the vertex (Taking into consideration the vertex radius)
                
                //Eq.1:  distance = [ (y-y0)^2 + (x-x0)^2 ]^0.5
                //Eq.2:  slope = (y-y0) / (x-x0)

                //Eq.2 into Eq.1 while isolating x (vertex_edge coordinate on the edge) yields:
                // x= x0 +- [ (vertex_radius)^2 / (slope^2 +1) ] ^0.5
                double root = Math.Sqrt((Math.Pow((vertex_radius)/2, 2)) / (Math.Pow(slope, 2) + 1));

                //candidate1 and candidate2 are a shortcut to find which of the intersection 
                //between the vertex and the linear function are the correct one. (It depends on the edge angle)
                Point candidate1 = new Point((m_line_list[i].X2 - root), (m_line_list[i].X2 - root) * slope + cutting_point);
                Point candidate2 = new Point((m_line_list[i].X2 + root), (m_line_list[i].X2 + root) * slope + cutting_point);
                
                // Edge starting point
                Point lineStarting = new Point(m_line_list[i].X1, m_line_list[i].Y1);

                // Reference Point is the tip of the arrow.
                Point referencePoint;
                if (CalculateDistance(lineStarting, candidate1) < CalculateDistance(lineStarting, candidate2))
                {
                    referencePoint = candidate1;
                }
                else
                {
                    referencePoint = candidate2;
                }

                //Rotating the Arrow to align with the edge
                //Calculating the angle
                double rads = Math.Atan(slope);
                double degrees = rads * (180 / Math.PI);

                //Rotating
                RotateTransform rotation = new RotateTransform();
                rotation.CenterX = referencePoint.X;
                rotation.CenterY = referencePoint.Y;
                if(m_line_list[i].X2>m_line_list[i].X1){
                    rotation.Angle = degrees;
                }
                else{
                    rotation.Angle = 180 + degrees;
                }

                // The arrow's 3-tips
                Point first = rotation.Transform(new Point(referencePoint.X - ARROW_HEIGHT, referencePoint.Y - ARROW_WIDTH));
                Point second = referencePoint;
                Point third = rotation.Transform(new Point(referencePoint.X - ARROW_HEIGHT, referencePoint.Y + ARROW_WIDTH));

                //Drawing the complete arrow
                Polygon poly = new Polygon();
                m_arrow_list[arrow_id].Points.Add(first);
                m_arrow_list[arrow_id].Points.Add(second);
                m_arrow_list[arrow_id].Points.Add(third);
                m_arrow_list[arrow_id].Fill = new SolidColorBrush(DEFAULT_EDGE);
                m_arrow_list[arrow_id].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                m_arrow_list[arrow_id].StrokeThickness = 4;

                #endregion

                //// add an Edge & Arrow to the Canvas
                canvas1.Children.Add(m_arrow_list[arrow_id]);
                canvas1.Children.Add(m_line_list[i]);

            }

            //Mark "n0" as starting vertex and highlight options
            foreach (VertexControl v in m_ellipse_list)
            {
                if (v.VertexID.ToLower() == START_NODE)
                {
                    v.Black_Ellipse.Visibility = Visibility.Visible;
                    v.Red_Ellipse.Visibility = Visibility.Collapsed;

                    foreach (string edge in vertexEdges[START_NODE])
                    {
                        Line line = (Line)canvas1.FindName(edge);

                        line.StrokeThickness = 6;
                        line.Stroke = new SolidColorBrush((player=="A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);

                        m_arrow_list[edge].Fill = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);
                        m_arrow_list[edge].Stroke = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);
                        m_arrow_list[edge].StrokeThickness = 6;
                    }

                    //Add start node to history
                    history.Push(START_NODE);
                    break;
                }
            }


            // Start the timer to show the correct answer - if defiened to do so
            if (load_solution_graph)                
                CountDownToShowAnswer();          
        }

        /// <summary>
        /// Shows the correct answer to the user in a separate window
        /// </summary>
        private void CountDownToShowAnswer()
        {
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);// 1 Seconds  
            myDispatcherTimer.Tick += new EventHandler(Each_Tick);
            myDispatcherTimer.Start();
        }

        /// <summary>
        /// Fires every 1 second while the DispatcherTimer is active
        /// </summary>
        /// <param name="o"></param>
        /// <param name="sender"></param>
        private void Each_Tick(object o, EventArgs sender)
        {
            if (timer < timeToAnswer)
            {
                timer++;
            }
            else
            {
                myDispatcherTimer.Stop();
                CorrectAnsWindow win = new CorrectAnsWindow(imageSource);
                win.Show();
            }
        }

        /// <summary>
        /// Calculates distance between two points.
        /// </summary>
        /// <param name="p1">First Point</param>
        /// <param name="p2">Second Point</param>
        /// <returns>Double distance</returns>
        private double CalculateDistance(Point p1, Point p2)
        {
            double x1 = Convert.ToDouble(p1.X);
            double y1 = Convert.ToDouble(p1.Y);

            double x2 = Convert.ToDouble(p2.X);
            double y2 = Convert.ToDouble(p2.Y);

            double dvalue = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
            return Math.Sqrt(dvalue);
        }


        /// <summary>
        /// Change the Edges thickness when alligned Vertex pressed
        /// </summary>
        /// <param name="ellipse">The Ellipese pressed</param>
        /// <param name="increase">Variable indicating whether to increase or decrease the thickness</param>
        void ChangeEdgeThickness(VertexControl ellipse, bool increase)
        {
            //Change unselected edges back to default
            //History should always contain one or more (Since we aren't going to undo the starting vertex)
            if (history.Count != 0)
            {
                string selectedEdge = null;
                double minDistance = double.PositiveInfinity;

                //Identify Selected edge
                foreach (var item in vertexEdges[history.Peek()])
                {
                    Line edge = (System.Windows.Shapes.Line)canvas1.FindName(item);
                    double distance = CalculateDistance(new Point(edge.X2,edge.Y2), new Point(ellipse.Vertex_X, ellipse.Vertex_Y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        selectedEdge = item;
                    }
                }

                //Revert the previously highlighted edges
                foreach (var item in vertexEdges[history.Peek()])
                {
                    Line edge = (System.Windows.Shapes.Line)canvas1.FindName(item);
                    //If wasn't selected - revert to default
                    if (item != selectedEdge)
                    {
                        edge.StrokeThickness = 4;
                        edge.Stroke = new SolidColorBrush(DEFAULT_EDGE);

                        m_arrow_list[edge.Name].Fill = new SolidColorBrush(DEFAULT_EDGE);
                        m_arrow_list[edge.Name].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                        m_arrow_list[edge.Name].StrokeThickness = 4;
                    }
                    //If selected - mark as selected
                    else
                    {
                        edge.StrokeThickness = 9;
                        edge.Stroke = (player == "A") ? new SolidColorBrush(PLAYER_A_COLOR) : new SolidColorBrush(PLAYER_B_COLOR);

                        m_arrow_list[edge.Name].Fill = (player == "A") ? new SolidColorBrush(PLAYER_A_COLOR) : new SolidColorBrush(PLAYER_B_COLOR);
                        m_arrow_list[edge.Name].Stroke = (player == "A") ? new SolidColorBrush(PLAYER_A_COLOR) : new SolidColorBrush(PLAYER_B_COLOR);
                        m_arrow_list[edge.Name].StrokeThickness = 9;
                    }
                }
  
            }

            //Add to history and change player
            history.Push(ellipse.VertexID);
            // click on vertex!
            player = (player == "A") ? "B" : "A";

            // Change Edge thickness according to number of aligned Vertices selected
            System.Windows.Shapes.Line line;
            // Iterate through all the edges connected to clicked vertex
            foreach (var item in vertexEdges[ellipse.VertexID])
            {                                
                // if edge wasn't selected before - add it to list
                if (!(thickerEdges.ContainsKey(item)))
                    thickerEdges.Add(item, new List<string>());                
                
                // get the edge instance on the canvas
                line = (System.Windows.Shapes.Line)canvas1.FindName(item);								
                line.Visibility = Visibility.Collapsed;

                //Check if target vertex was selected
                VertexControl sourceVertex = null;
                foreach (VertexControl vertex in m_ellipse_list)
                {
                    if (vertex.VertexID == edgesInformation[item].Item2)
                    {
                        sourceVertex = vertex;
                        break;
                    }
                }

                //If Increase and clicked on an unclicked vertex:
                if (increase && sourceVertex.Red_Ellipse.Visibility != Visibility.Collapsed)
                {                  
                    // foreach edge holds the two verteces connected to it (as known for now)
                    thickerEdges[item].Add(ellipse.VertexID);               
                    
                    #region Story Board
                    SolidColorBrush myAnimatedBrush = new SolidColorBrush();
                    Storyboard colorStoryboard = new Storyboard();
                    Storyboard.SetTargetProperty(colorStoryboard, new PropertyPath("Color"));//(SolidColorBrush.ColorProperty));
                    Storyboard.SetTarget(colorStoryboard, myAnimatedBrush);
                    ColorAnimationUsingKeyFrames caukf = new ColorAnimationUsingKeyFrames();
                    // caukf.BeginTime = new TimeSpan(0, 0, 0, 0);

                    LinearColorKeyFrame lckf0 = new LinearColorKeyFrame();
                    lckf0.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0,900));
                    lckf0.Value = Colors.Red;

                    LinearColorKeyFrame lckf1 = new LinearColorKeyFrame();
                    lckf1.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0,1900));
                    lckf1.Value = DEFAULT_EDGE;

                    LinearColorKeyFrame lckf2 = new LinearColorKeyFrame();
                    lckf2.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0,1900));
                    lckf2.Value = Colors.Red;

                    LinearColorKeyFrame lckf3 = new LinearColorKeyFrame();
                    lckf3.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0,2300));
                    lckf3.Value = DEFAULT_EDGE;

                    caukf.KeyFrames.Add(lckf0);
                    caukf.KeyFrames.Add(lckf1);
                    //caukf.KeyFrames.Add(lckf2);
                    //caukf.KeyFrames.Add(lckf3);

                    colorStoryboard.Children.Add(caukf);
                    colorStoryboard.Begin();
                    #endregion

                    line.Stroke = myAnimatedBrush;

                    line.Stroke = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);

                    m_arrow_list[line.Name].Fill = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);
                    m_arrow_list[line.Name].Stroke = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);
                    m_arrow_list[line.Name].StrokeThickness = 9;

                    // increase Edge thickness
                    if (line.StrokeThickness == 4)
                        line.StrokeThickness += 2;
                }

                else// The edge's thickness needs to be decreased (the vertex was unselected)
                {
                    // Holds all the thick Edges 
                    List<string> EdgeVerticesList = thickerEdges[item];
                    List<string> copy = new List<string>();

                    if ((line.StrokeThickness > 4))
                    {
                        foreach (string s in EdgeVerticesList)
                        {
                            if (s != ellipse.VertexID)
                                copy.Add(s);
                        }

                        thickerEdges[item] = copy;
                       
                        if(copy.Count==0){
                            line.StrokeThickness -= 2;
						}

                        line.Stroke = new SolidColorBrush(DEFAULT_EDGE);

                        m_arrow_list[line.Name].Fill = new SolidColorBrush(DEFAULT_EDGE);
                        m_arrow_list[line.Name].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                        m_arrow_list[line.Name].StrokeThickness = 4;
                    }
                }
                line.Visibility = Visibility.Visible;
            }

            // Check if graph solved correctly 
            bool solved_correctly = history.Contains(win_option);
            // Write conclusion for graph solution to the DB
       //     client.SolvedProblemGraphAsync((int)i_storage["userID"], solved_correctly, DateTime.Now);                                    
        }

        #endregion


        #region Event Handlers

        /// <summary>
        /// Set mouse position at any moment on the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Update mouse coordinates
            _mX = e.GetPosition(sender as UIElement).X;
            _mY = e.GetPosition(sender as UIElement).Y;
        }

        /// <summary>
        /// Show the node was selected by changing its color and selection to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            VertexControl ellipse = sender as VertexControl;// The clicked Vertex

            if (history.Count > 0 && history.Peek() == ellipse.VertexID)
            {
                undoBtn_Click(sender, e);
            }
            else
            {
                //Verify that the selected vertex is an option.
                if (history.Count > 0)
                {
                    bool neighbour = false;
                    foreach (string edge in vertexEdges[history.Peek()])
                    {
                        if (edgesInformation[edge].Item2 == ellipse.VertexID)
                        {
                            neighbour = true;
                            break;
                        }
                    }

                    if (!neighbour)
                        return;
                }


                if (ellipse.Red_Ellipse.Visibility == Visibility.Visible)
                {
                    ellipse.Red_Ellipse.Visibility = Visibility.Collapsed;
                    if (player == "A")
                    {
                        ellipse.Purple_Ellipse.Visibility = Visibility.Collapsed;
                        ellipse.Green_Ellipse.Visibility = Visibility.Visible;
                        ellipse.VertexColor = "Green";
                        GreenBox.Visibility = System.Windows.Visibility.Collapsed;
                        BlueBox.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        ellipse.Purple_Ellipse.Visibility = Visibility.Visible;
                        ellipse.Green_Ellipse.Visibility = Visibility.Collapsed;
                        ellipse.VertexColor = "Purple";
                        GreenBox.Visibility = System.Windows.Visibility.Visible;
                        BlueBox.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    ChangeEdgeThickness(ellipse, true);// the selected Vertex and true to increase thickness
                }
                else
                {
                    //Do not do anything since you cannot select the same node more then once.
                    return;
                }

                // Retrive userId from Isolated Storage
                int userID = 0;
                if (i_storage.Contains("userID"))
                    userID = (int)i_storage["userID"];

                // call Web Service Method Asynchronicly for each log entery   

                client.InsertIntoExperimentActionsAsync(experimentID, userID, ellipse.VertexID, ellipse.VertexColor, DateTime.Now);
            }
        }





        /// <summary>
        /// Shows the user a message and closes experiment window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitExperimentBtn_Click(object sender, RoutedEventArgs e)
        {

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Submit");
            bool solved_correctly = false;
            if (radioButton3.IsChecked == true)
            {
                solved_correctly = true;
            }

            TimeSpan tmp = DateTime.Now.Subtract(m_StartTime);
            DateTime tmp1 = new DateTime(2014, 1, 13);
            DateTime tmp2 = tmp1 + tmp;

            //witring the time for the solution like this
            // it's start in 12.1.2014 in 00:00
            // and when you subtract from the date in the DB you will get the write time he solved
            client.SolvedProblemGraphAsync((int)i_storage["userID"], solved_correctly, tmp2 );  

//            MessageBoxResult result =
  //                   MessageBox.Show("Thank you for participating in the" + '"' + " Experiment" + '"',
    //                  "Experiment Submission", MessageBoxButton.OK);
      //      if (result == MessageBoxResult.OK)
        //    {
               // System.Windows.Browser.HtmlPage.Window.Invoke("close");
                textBlock.Visibility = Visibility.Collapsed;
                radioButton1.Visibility = Visibility.Collapsed;
                radioButton2.Visibility = Visibility.Collapsed;
                radioButton3.Visibility = Visibility.Collapsed;
                SubmitExperimentBtn.Visibility = Visibility.Collapsed;

          //  }
        } 

        #endregion        

        /// <summary>
        /// this method is resonsible for the radio buttons
        /// do noting
        /// </summary>
        /// <param name="sender">Radio Button</param>
        /// <param name="e"></param>
       private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
         
           
            
        }

        /// <summary>
        /// rest the graph to zero
        /// get a clean graph
        /// </summary>
        /// <param name="sender">click the button</param>
        /// <param name="e">void</param>
       private void resetBn_Click(object sender, RoutedEventArgs e)
       {
           while (history.Count > 1)
           {
               undoBtn_Click(sender, e);
           }
       }


        /// <summary>
        /// This method is responsible for undoing
        /// </summary>
        /// <param name="sender">Undo Button</param>
        /// <param name="e">Event Args</param>
        /// 

        private void undoBtn_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count == 1)
                return;

            string toCancel = history.Pop();
            string mostRecent = history.Peek();

            //Revert selected vertex to Red
            VertexControl vertex = null;
            foreach (VertexControl v in m_ellipse_list)
            {
                if (v.VertexID == toCancel)
                {
                    vertex = v;
                    break;
                }
            }
            vertex.Red_Ellipse.Visibility = Visibility.Visible;
            vertex.VertexColor = "Red";
            vertex.Purple_Ellipse.Visibility = Visibility.Collapsed;
            vertex.Green_Ellipse.Visibility = Visibility.Collapsed;
            


            //Revert selected edge to default
            foreach (string edge in vertexEdges[toCancel])
            {
                Line line = ((Line)canvas1.FindName(edge));
                line.Stroke = new SolidColorBrush(DEFAULT_EDGE);
                line.StrokeThickness = 4;

                m_arrow_list[line.Name].Fill = new SolidColorBrush(DEFAULT_EDGE);
                m_arrow_list[line.Name].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                m_arrow_list[line.Name].StrokeThickness = 4;
            }

            foreach (string edge in vertexEdges[mostRecent])
            {
                if (edgesInformation[edge].Item2 == toCancel)
                {
                    Line line = ((Line)canvas1.FindName(edge));
                    line.Stroke = new SolidColorBrush(DEFAULT_EDGE);
                    line.StrokeThickness = 4;

                    m_arrow_list[line.Name].Fill = new SolidColorBrush(DEFAULT_EDGE);
                    m_arrow_list[line.Name].Stroke = new SolidColorBrush(DEFAULT_EDGE);
                    m_arrow_list[line.Name].StrokeThickness = 4;
                    break;
                }
            }

            //Highlight options
            //Update current player
            player = (player == "A") ? "B" : "A";

            foreach (string edge in vertexEdges[mostRecent])
            {
                //If edge leads to an unselected option
                if (!history.Contains(edgesInformation[edge].Item2))
                {
                    Line line = ((Line)canvas1.FindName(edge));
                    line.StrokeThickness = 6;
                    line.Stroke = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);

                    m_arrow_list[edge].Fill = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);
                    m_arrow_list[edge].Stroke = new SolidColorBrush((player == "A") ? PLAYER_A_COLOR : PLAYER_B_COLOR);
                    m_arrow_list[edge].StrokeThickness = 6;
                    //changing the box trun on the screen
                    if (player == "A")
                    {
                        GreenBox.Visibility = Visibility.Visible;
                        BlueBox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        GreenBox.Visibility = Visibility.Collapsed;
                        BlueBox.Visibility = Visibility.Visible;
                    }
                }
                
            }

            //Write to DB
            // Retrive userId from Isolated Storage
            int userID = 0;
            if (i_storage.Contains("userID"))
                userID = (int)i_storage["userID"];

            client.InsertIntoExperimentActionsAsync(experimentID, userID, vertex.VertexID, vertex.VertexColor, DateTime.Now);                                                                                   
        }

    }
}
