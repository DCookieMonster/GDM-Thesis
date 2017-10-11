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



namespace GDMVisualization
{
    public partial class Experiment : Page
    {

        #region Internal Global Attributes and Properties

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
        const double my_width_resolution = 1366.00;
        const double my_height_resolution = 643.00;
        
        // Dictonary of thicker edges
        Dictionary<string, List<string>> thickerEdges = new Dictionary<string, List<string>>();

        // Enables to write user ID to subject's disk (instead of Session Object)
        IsolatedStorageSettings i_storage = IsolatedStorageSettings.ApplicationSettings;   

        // Add refernce to Web Service        
        ServiceReference1.Service1Client client = new ServiceReference1.Service1Client();
        
        // Current experiment id
        int experimentID;

        #endregion

        

        #region Constructors

        public Experiment()
        {
            InitializeComponent();




            SubmitExperimentBtn.Visibility = System.Windows.Visibility.Collapsed;
            IDictionary<string, string> QueryString = HtmlPage.Document.QueryString;
            string workerId="";

            //Logger logger = LogManager.GetCurrentClassLogger();
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
            if (e.Error == null)
            {
                experimentID = e.Result;
            }
            else
                experimentID = 12;
        }    

        /// <summary>
        /// When Webservice completes writing userID to the DB and returns the id 
        /// writes it to Isolated Storage on user disk
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_AddParticipantCompleted(object sender, ServiceReference1.AddParticipantCompletedEventArgs e)
        {
           // SubmitExperimentBtn.Visibility = Visibility.Visible;
            int userID;
            if (e.Error == null)
            {
                userID = e.Result;
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

                Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream("GDMVisualization.Resources.g5.graphml");
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

                        // Add the Edge to both Vertex
                        vertexEdges[edge_source].Add(edge_id);
                        vertexEdges[edge_target].Add(edge_id);
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
                m_ellipse_list.Add(new VertexControl());
                m_ellipse_list[i].Height = vertex_radius;
                m_ellipse_list[i].Width = vertex_radius;
                m_ellipse_list[i].SetValue(Canvas.ZIndexProperty, 10);
                m_ellipse_list[i].Green_Ellipse.Visibility = Visibility.Collapsed;
                m_ellipse_list[i].VertexID = vertex_cordinate_list.ElementAt(i).Key;
                m_ellipse_list[i].VertexColor = "red";
                m_ellipse_list[i].Vertex_X = vertex_cordinate_list.ElementAt(i).Value.X * proportion_width;
                m_ellipse_list[i].Vertex_Y = vertex_cordinate_list.ElementAt(i).Value.Y * proportion_height;

                // set Vertex location on Canvas                                 
                Canvas.SetLeft(m_ellipse_list[i], vertex_cordinate_list.ElementAt(i).Value.X * proportion_width);// X property
                Canvas.SetTop(m_ellipse_list[i], vertex_cordinate_list.ElementAt(i).Value.Y * proportion_height);// Y property				

                // set event handlers to Vertex 
                m_ellipse_list[i].MouseLeftButtonDown += new MouseButtonEventHandler(ellipse_MouseLeftButtonDown);
                m_ellipse_list[i].MouseRightButtonUp += new MouseButtonEventHandler(tb_MouseRightButtonUp);
                m_ellipse_list[i].MouseRightButtonDown += new MouseButtonEventHandler(tb_MouseRightButtonDown);

                // add the Vertex to the Canvas
                canvas1.Children.Add(m_ellipse_list[i]);				 
            }

            ////-------------------- EDGE --------------------////
            for (int i = 0; i < edge_cordinate_list.Count; i++)
            {
                // set Edge appearence
                m_line_list.Add(new Line());
                m_line_list[i].Stroke = new SolidColorBrush(Colors.White);
                m_line_list[i].StrokeThickness = 4;
                m_line_list[i].Name = edge_cordinate_list[i].id;// Get Vertex name from parser                         

                #region set Edge location on Canvas - without setting location to middle of Vertex //
                //m_line_list[i].X1 = edge_cordinate_list[i].x1;
                //m_line_list[i].Y1 = edge_cordinate_list[i].y1;
                //m_line_list[i].X2 = edge_cordinate_list[i].x2;
                //m_line_list[i].Y2 = edge_cordinate_list[i].y2;
                #endregion

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

                // add an Edge to the Canvas
                canvas1.Children.Add(m_line_list[i]);				
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
        /// Change the Edges thickness when alligned Vertex pressed
        /// </summary>
        /// <param name="ellipse">The Ellipese pressed</param>
        /// <param name="increase">Variable indicating whether to increase or decrease the thickness</param>
        void ChangeEdgeThickness(VertexControl ellipse, bool increase)
        {
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

                if (increase)
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
                    lckf1.Value = Colors.White;

                    LinearColorKeyFrame lckf2 = new LinearColorKeyFrame();
                    lckf2.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0,1900));
                    lckf2.Value = Colors.Red;

                    LinearColorKeyFrame lckf3 = new LinearColorKeyFrame();
                    lckf3.KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0,2300));
                    lckf3.Value = Colors.White;

                    caukf.KeyFrames.Add(lckf0);
                    caukf.KeyFrames.Add(lckf1);
                    //caukf.KeyFrames.Add(lckf2);
                    //caukf.KeyFrames.Add(lckf3);

                    colorStoryboard.Children.Add(caukf);
                    colorStoryboard.Begin();
                    #endregion

                    line.Stroke = myAnimatedBrush;

                    // increase Edge thickness
                    if (line.StrokeThickness == 4)
                        line.StrokeThickness += 5;
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
                            line.StrokeThickness -= 5;
                           // line.Stroke = new SolidColorBrush(Colors.White);
						}
                    }
                }
                line.Visibility = Visibility.Visible;
            }

            // Check if graph solved correctly 
            bool solved_correctly = true;
            foreach (var item in m_line_list)// Iterate on all edges and check thier thickness
	        {
                if (item.StrokeThickness <= 4)
                    solved_correctly = false;
	        }
            // Write conclusion for graph solution to the DB

            client.SolvedProblemGraphAsync((int)i_storage["userID"], solved_correctly, DateTime.Now);                                    
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

            if (ellipse.Red_Ellipse.Visibility == Visibility.Visible)
            {
                ellipse.Red_Ellipse.Visibility = Visibility.Collapsed;
                ellipse.Green_Ellipse.Visibility = Visibility.Visible;
                ellipse.VertexColor = "Green";
                ChangeEdgeThickness(ellipse, true);// the selected Vertex and true to increase thickness
            }
            else
            {
                ellipse.Red_Ellipse.Visibility = Visibility.Visible;
                ellipse.Green_Ellipse.Visibility = Visibility.Collapsed;
                ellipse.VertexColor = "Red";
                ChangeEdgeThickness(ellipse, false);// the selected Vertex and false to decrease thickness 
            }                  

            // Retrive userId from Isolated Storage
            int userID = 0;
            if (i_storage.Contains("userID"))            
                userID = (int)i_storage["userID"];
            
            // call Web Service Method Asynchronicly for each log entery   

            client.InsertIntoExperimentActionsAsync(experimentID, userID, ellipse.VertexID, ellipse.VertexColor, DateTime.Now);                                                                                   
        }      

        /// <summary>
        /// Show the node was selected by changing its color and selection to DB 
        /// when right mouse button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                toManipulate = (sender as UIElement);// Copy the element that was Right clicked

                // Determin the offset where to open the Menu
                if (sender.GetType() == typeof(GDMVisualization.VertexControl))
                {
                    cm.HorizontalOffset = e.GetPosition(sender as UIElement).X;
                    cm.VerticalOffset = e.GetPosition(sender as UIElement).Y;
                }
                else// incase it's an Edge
                {
                    cm.HorizontalOffset = 0;
                    cm.VerticalOffset = 0;
                }
                cm.IsOpen = true;// Sets a value to make the ContextMenu visible.
            }
        }

        void tb_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// When left mouse button clicked
        /// Opens menu from which user can select a coloring option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            VertexControl vertex = toManipulate as VertexControl;// the previosly clicked element

            if (vertex != null)
            {
                // get the shapes cordinates
                GeneralTransform gt = vertex.TransformToVisual(Application.Current.RootVisual as UIElement);
                Point vertex_loc = gt.Transform(new Point(0, 0));

                // The mouse pointer is inside the Vertex
                if (Math.Abs(vertex_loc.X - _mX) < vertex_radius)
                    if (Math.Abs(vertex_loc.Y - _mY) < vertex_radius)
                    {
                        // Retrive userId from Isolated Storage
                        int userID = 0;
                        if (i_storage.Contains("userID"))
                            userID = (int)i_storage["userID"];

                        switch (menuItem.Header.ToString())
                        {
                            case "Green":
                                if (vertex.Red_Ellipse.Visibility == Visibility.Visible)
                                {
                                    vertex.Red_Ellipse.Visibility = Visibility.Collapsed;
                                    vertex.Green_Ellipse.Visibility = Visibility.Visible;
                                    vertex.VertexColor = "Green";
                                    // call Web Service Method Asynchronicly for each log entery (so nothing will be logged for changing to same color)          
                                    client.InsertIntoExperimentActionsAsync(1, userID, vertex.VertexID, vertex.VertexColor, DateTime.Now);
                                    // the selected Vertex and true to increase thickness
                                    ChangeEdgeThickness(vertex, true);
                                }
                                break;
                            case "Red":
                                if (vertex.Green_Ellipse.Visibility == Visibility.Visible)
                                {
                                    vertex.Red_Ellipse.Visibility = Visibility.Visible;
                                    vertex.Green_Ellipse.Visibility = Visibility.Collapsed;
                                    vertex.VertexColor = "Red";
                                    // call Web Service Method Asynchronicly for each log entery           
                                    client.InsertIntoExperimentActionsAsync(1, userID, vertex.VertexID, vertex.VertexColor, DateTime.Now);
                                    // the selected Vertex and false to decrease thickness 
                                    ChangeEdgeThickness(vertex, false);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                cm.IsOpen = false;// sets a value to make the ContextMenu invisible
            }
        }

        /// <summary>
        /// Shows the user a message and closes experiment window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitExperimentBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                     MessageBox.Show("Thank you for participating in the" + '"' + " Experiment" + '"',
                      "Experiment Submission", MessageBoxButton.OK);
            if (result == MessageBoxResult.OK)
            {
                //System.Windows.Browser.HtmlPage.Window.Invoke("close");

            }
        } 

        #endregion        

    }
}
