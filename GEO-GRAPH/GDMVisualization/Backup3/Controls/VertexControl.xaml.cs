using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GDMVisualization
{
	public partial class VertexControl : UserControl
	{
        public string VertexID { get; set;}
        public string VertexColor { get; set; }
        public double Vertex_X { get; set; }
        public double Vertex_Y { get; set; }
        static public int num = 26;

		public VertexControl(string n="")
		{
			// Required to initialize variables
			InitializeComponent();
            //this.Red_Ellipse = new Ellipse();
            //this.Green_Ellipse = new Ellipse();
            DataContext = this;
            this.Black_Ellipse.Visibility = Visibility.Collapsed;
            num+=3;
        }

        public String dor 
        {
            get { return num.ToString(); }
        }
        
	}
}