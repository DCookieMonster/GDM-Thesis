﻿#pragma checksum "C:\Users\Ofra\Documents\Visual Studio 2010\Projects\GDMVisualization\GDMVisualization\Views\Experiment.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "29BEB712C3167BCAA317CA03CE26626F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace GDMVisualization {
    
    
    public partial class Experiment : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Canvas canvas1;
        
        internal System.Windows.Controls.Button SubmitExperimentBtn;
        
        internal System.Windows.Controls.ContextMenu cm;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/GDMVisualization;component/Views/Experiment.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.canvas1 = ((System.Windows.Controls.Canvas)(this.FindName("canvas1")));
            this.SubmitExperimentBtn = ((System.Windows.Controls.Button)(this.FindName("SubmitExperimentBtn")));
            this.cm = ((System.Windows.Controls.ContextMenu)(this.FindName("cm")));
        }
    }
}

