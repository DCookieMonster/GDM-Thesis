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
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace GDMVisualization
{
    public partial class CorrectAnsWindow : ChildWindow
    {
        //private string title;
        //private string caption_2;

        ServiceReference1.Service1Client proxy = new ServiceReference1.Service1Client();
        // Enables to write user ID to subject's disk (instead of Session Object)
        IsolatedStorageSettings i_storage = IsolatedStorageSettings.ApplicationSettings;   

        public CorrectAnsWindow(BitmapImage imageSource)
        {
            InitializeComponent();
            image.Source = imageSource;
        }

        //public CorrectAnsWindow(string title, string caption, Uri uri)
        //    : this(title, caption)
        //{
        //    image.Source = new BitmapImage(uri);
        //}

        //public CorrectAnsWindow(string title, string caption_2)
        //{
        //    // TODO: Complete member initialization
        //    this.title = title;
        //    this.caption_2 = caption_2;
        //}

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
                        
            int userID = 0;
            if (i_storage.Contains("userID"))            
                userID = (int)i_storage["userID"];
            proxy.SolutionAcceptancePartialCompleted +=new EventHandler<ServiceReference1.SolutionAcceptancePartialCompletedEventArgs>(proxy_SolutionAcceptancePartialCompleted);

            // Update DB on participant accepting the proposed solution
            proxy.SolutionAcceptancePartialAsync(userID, 1, DateTime.Now, true);            
        }            

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            int userID = 0;
            if (i_storage.Contains("userID"))
                userID = (int)i_storage["userID"];
            // Update DB on participant declining the proposed solution
            proxy.SolutionAcceptancePartialAsync(userID, 1, DateTime.Now, false);
        }

        void proxy_SolutionAcceptancePartialCompleted(object sender, ServiceReference1.SolutionAcceptancePartialCompletedEventArgs e)
        {
            bool ans;
            if (e.Error == null)
                ans = e.Result;    
        }    
    }
}

