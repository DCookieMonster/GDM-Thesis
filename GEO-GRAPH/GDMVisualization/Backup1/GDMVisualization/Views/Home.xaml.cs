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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GDMVisualization.Views;

namespace GDMVisualization
{
    public partial class Home : Page
    {
        //CHENGED: Experiment to ExperimentGEO
        public Home()
        {
            InitializeComponent();
            this.Content = new ExperimentGeo(); 
            
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Content = new Experiment(); 
        }

        // Navigate to Tutorial Page when selected
        private void tutorial_button_Click(object sender, RoutedEventArgs e)
        {

            this.Content = new Tutorial(); 
        }

        // Navigate to Experiemnt Page when selected
        //private void experiment_button_Click(object sender, RoutedEventArgs e)
        //{
        //    this.Content = new Experiment(); 
        //}

        // Navigate to Experiemnt Page when selected
        private void btnRunExperiment_Checked(object sender, RoutedEventArgs e)
        {
            this.Content = new Experiment(); 
            
        }

    }
}