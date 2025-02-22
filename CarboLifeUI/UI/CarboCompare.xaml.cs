﻿using CarboLifeAPI.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using CarboLifeAPI;
using Microsoft.Win32;

namespace CarboLifeUI.UI
{
    /// <summary>
    /// Interaction logic for DataViewer.xaml
    /// </summary>
    public partial class CarboCompare : UserControl
    {
        public CarboProject CarboLifeProject;

        List<CarboProject> projectListToCompareTo = new List<CarboProject>();


        public CarboCompare()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            CarboLifeProject = new CarboProject();
            projectListToCompareTo = new List<CarboProject>();

            InitializeComponent();
        }

        public CarboCompare(CarboProject carboLifeProject)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            InitializeComponent();

            CarboLifeProject = carboLifeProject;
            projectListToCompareTo = new List<CarboProject>();

        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {

                DependencyObject parent = VisualTreeHelper.GetParent(this);
                Window parentWindow = Window.GetWindow(parent);
                CarboLifeMainWindow mainViewer = parentWindow as CarboLifeMainWindow;

                if (mainViewer != null)
                    CarboLifeProject = mainViewer.getCarbonLifeProject();

                if (CarboLifeProject != null)
                {
                    //A project Is loaded, Proceed to next

                    RefreshInterFace();

                    if(cbb_GraphType.Items.Count <= 0)
                    {
                        cbb_GraphType.Items.Add("Materials");
                        cbb_GraphType.Items.Add("Totals");
                        cbb_GraphType.Text = "Totals";
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private void RefreshInterFace()
        {
            try
            {
                if (CarboLifeProject != null)
                {
                    chx_Project0.Content = CarboLifeProject.Name + " (Current) " + Environment.NewLine + Math.Round(CarboLifeProject.ECTotal,2) + " kgCO₂";


                    if (liv_Projects != null)
                    {
                        liv_Projects.ItemsSource = null;
                        liv_Projects.ItemsSource = projectListToCompareTo;
                    }
                    SeriesCollection currentProjectSeriesCollection = new SeriesCollection();
                    if (chx_Project0.IsChecked == true)
                    {
                        currentProjectSeriesCollection = GraphBuilder.BuildComparingTotalsBarGraph(CarboLifeProject, projectListToCompareTo);
                    }
                    else
                    {
                        currentProjectSeriesCollection = GraphBuilder.BuildComparingTotalsBarGraph(null, projectListToCompareTo);
                    }

                    Func<double, string> Formatter = value => value + " kgCO₂";

                    //Build the labels
                    List<string> projectlist = new List<string>();

                    if (chx_Project0.IsChecked == true)
                        projectlist.Add("Current");

                    foreach(CarboProject cp in projectListToCompareTo)
                    {
                        projectlist.Add(cp.Name);
                    }
                    //Labels = null;
                    Labels = projectlist.ToArray();

                    barchart.SeriesColors = GraphBuilder.getColours();
                    barchart.Series = currentProjectSeriesCollection;

                    DataContext = this;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //When assembly cant be find bind to current
        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            System.Reflection.Assembly ayResult = null;
            string sShortAssemblyName = args.Name.Split(',')[0];
            System.Reflection.Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (System.Reflection.Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
                {
                    ayResult = ayAssembly;
                    break;
                }
            }
            return ayResult;
        }

        private void cbb_GraphType_DropDownClosed(object sender, EventArgs e)
        {
            RefreshInterFace();
        }

        private void setProject(CarboProject clp)
        {
            if(clp != null)
            {
                projectListToCompareTo.Add(clp);
            }
            RefreshInterFace();
        }

        private CarboProject openNewProject()
        {
            CarboProject result = null;

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Carbo Life Project File (*.clcx)|*.clcx|Carbo Life Project File (*.xml)| *.xml|All files (*.*)|*.*";

                var path = openFileDialog.ShowDialog();

                if (openFileDialog.FileName != "")
                {
                    CarboProject buffer = new CarboProject();
                    result = buffer.DeSerializeXML(openFileDialog.FileName);

                    result.Audit();
                    result.CalculateProject();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

            return result;

        }

        private void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (liv_Projects.SelectedItems.Count > 0)
            {
                CarboProject selectedProject = liv_Projects.SelectedItem as CarboProject;
                projectListToCompareTo.Remove(selectedProject);
            }

            RefreshInterFace();

        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            CarboProject clp = openNewProject();
            if (clp != null)
                setProject(clp);
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshInterFace();
        }

        private void chx_Project0_Click(object sender, RoutedEventArgs e)
        {
            if (CarboLifeProject != null)
            {
                RefreshInterFace();
            }
        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            DataExportUtils.ExportComaringGraphs(CarboLifeProject, projectListToCompareTo);
        }
    }
}
