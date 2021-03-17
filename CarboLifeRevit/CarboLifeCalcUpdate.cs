﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CarboLifeAPI.Data;
using CarboLifeUI.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CarboLifeRevit
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class CarboLifeCalcUpdate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Carbo Life Project File (*.xml)|*.xml|All files (*.*)|*.*";

                var path = openFileDialog.ShowDialog();

                if (openFileDialog.FileName != "")
                {
                    CarboRevitImportSettings importSettings = new CarboRevitImportSettings();
                    importSettings = importSettings.DeSerializeXML();

                    //Import the Files
                    CarboLifeRevitImport.ImportElements(app, importSettings, openFileDialog.FileName);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        






            return Result.Succeeded;
        }
    }
}
