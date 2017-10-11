using System;
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
using System.Windows.Navigation;
using System.Xml;
using System.IO;
using System.Text;
using System.Windows.Resources;
using System.Xml.Linq;
using System.Windows.Browser;

namespace GDMVisualization.Views
{
    public partial class Tutorial : Page
    {
        #region Internal Global Attributes and Properties

        // Lists to hold the graph elements
        List<VertexControl> m_ellipse_list = new List<VertexControl>();
        List<Line> m_line_list = new List<Line>();
        int vertex_radius;// the radius for all the vertex in the graph

        // Mouse coordinates on the grid at any given moment
        private double _mX = 0.0;
        private double _mY = 0.0;

        #endregion

        #region Constructors

        public Tutorial()
        {
            InitializeComponent();
            //GraphmlParser();

            double resolution_height = double.Parse(HtmlPage.Document.Body.GetProperty("clientHeight").ToString());
            double resolution_width = double.Parse(HtmlPage.Document.Body.GetProperty("clientWidth").ToString());
            //tutorial_video.Height = resolution_height;
            //tutorial_video.Width = resolution_width;            
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #endregion


        #region Private Methods

        private void GraphmlParser()
        {
            // holds the final vertex and edge coordinate lists
            Dictionary<string, Point> vertex_coordinate_list = new Dictionary<string, Point>();
            List<EdgePoints> edge_coordinate_list = new List<EdgePoints>();

            StreamResourceInfo st = Application.GetResourceStream(new Uri("Resources/tutorial.xml", UriKind.Relative));

            System.IO.Stream fileStream = st.Stream;
            //System.IO.StreamReader fileStream = new StreamReader("/Tutorial Graphs/unnamed0.graphml");
            byte[] textBytes = new byte[fileStream.Length];
            fileStream.Read(textBytes, 0, textBytes.Length);
            string graph_xml = UTF8Encoding.UTF8.GetString(textBytes, 0, textBytes.Length);

            ///----------------------------------- Vertex Begin -----------------------------------///
            // Create an XmlReader
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
                    //vertexEdges.Add(node_id, new List<string>());

                }
            }
            ///----------------------------------- Vertex Begin -----------------------------------///

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

        // Navigate to Experiemnt Page when selected
        private void experiment_button_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new Experiment();
        }

        /// <summary>
        /// Draws the graph according to the input XML file
        /// </summary>
        /// <param name="vertex_cordinate_list">A list of the Vertex cordinates</param>
        /// <param name="edge_cordinate_list">A list of the Edge cordinates</param>
        private void DrawGraph(Dictionary<string, Point> vertex_cordinate_list, List<EdgePoints> edge_cordinate_list)
        {

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
                m_ellipse_list[i].Vertex_X = vertex_cordinate_list.ElementAt(i).Value.X;
                m_ellipse_list[i].Vertex_Y = vertex_cordinate_list.ElementAt(i).Value.Y;

                // set Vertex location on Canvas                                 
                Canvas.SetLeft(m_ellipse_list[i], vertex_cordinate_list.ElementAt(i).Value.X);// X property
                Canvas.SetTop(m_ellipse_list[i], vertex_cordinate_list.ElementAt(i).Value.Y);// Y property

                // set event handlers to Vertex 
                //m_ellipse_list[i].MouseLeftButtonDown += new MouseButtonEventHandler(ellipse_MouseLeftButtonDown);
                //m_ellipse_list[i].MouseRightButtonUp += new MouseButtonEventHandler(tb_MouseRightButtonUp);
                //m_ellipse_list[i].MouseRightButtonDown += new MouseButtonEventHandler(tb_MouseRightButtonDown);

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

                //m_line_list[i].SetValue(Canvas.ZIndexProperty, 0);
                //m_line_list[i].Visibility = Visibility.Visible;              

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
                            m_line_list[i].X1 = vertex_cordinate_list.ElementAt(j).Value.X + vertex_radius / 2;
                            m_line_list[i].Y1 = vertex_cordinate_list.ElementAt(j).Value.Y + vertex_radius / 2;
                        }
                    if (Math.Abs(vertex_cordinate_list.ElementAt(j).Value.X - edge_cordinate_list[i].x2) < vertex_radius)
                        if (Math.Abs(vertex_cordinate_list.ElementAt(j).Value.Y - edge_cordinate_list[i].y2) < vertex_radius)
                        {
                            m_line_list[i].X2 = vertex_cordinate_list.ElementAt(j).Value.X + vertex_radius / 2;
                            m_line_list[i].Y2 = vertex_cordinate_list.ElementAt(j).Value.Y + vertex_radius / 2;
                        }
                }
                #endregion

                // add an Edge to the Canvas
                canvas1.Children.Add(m_line_list[i]);
            }
            // Start the timer to show the correct answer
            //CountDownToShowAnswer();

            // Make Begin Experiment Button visible
            //btnBeginExperiment.Visibility = Visibility.Visible;
        }

        private void tutorial_video_MediaEnded(object sender, RoutedEventArgs e)
        {
            tutorial_video.Visibility = Visibility.Collapsed;
            //btnBeginExperiment.Visibility = Visibility.Visible;
            //textBlock1.Visibility = Visibility.Visible;   

            this.Content = new ExperimentReady();
        }

        #endregion

        private void btnBeginExperiment_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new Experiment();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            tutorial_video.Play();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            tutorial_video.Pause();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tutorial_video.Stop();
        }


    }
}
