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

namespace GDMVisualization.Views
{
    public partial class ExperimentReady : Page
    {
        public ExperimentReady()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnRunExperiment_Checked(object sender, RoutedEventArgs e)
        {
            this.Content = new Experiment(); 
        }

        private void tutorial_button_Click(object sender, RoutedEventArgs e)
        {
            this.Content = new Tutorial(); 
        }

    }
}
