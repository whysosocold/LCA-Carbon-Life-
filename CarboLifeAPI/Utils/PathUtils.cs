﻿using CarboLifeAPI.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows;

namespace CarboLifeAPI
{
    public static class PathUtils
    {
        /// <summary>
        /// The current location from where the application runs
        /// </summary>
        /// <returns>The application path </returns>
        public static string getAssemblyPath()
        {
            string _path = Assembly.GetExecutingAssembly().Location;
            string myPath = Path.GetDirectoryName(_path);
            return myPath;
        }
        /// <summary>
        /// Sets and prepares the usermaterials and settings File
        /// </summary>
        [Obsolete]
        public static void CheckFileLocations()
        {
            string log = "";

            string pathDatabase = Utils.getAssemblyPath() + "\\db\\";
            string bufferPath = pathDatabase + "MaterialBuffer.cxml";
            string targetPath = pathDatabase + "UserMaterials.cxml";

            //This bit will prevent the user files to be ever overwritten
            if (!(File.Exists(targetPath)))
            {
                if (File.Exists(bufferPath))
                {
                    File.Copy(bufferPath, targetPath);
                }
            }

            //All userfiles need to move to the local folder:
            string appdatafolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\";
            string TemplateFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\UserMaterials.cxml";
            string SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\CarboSettings.xml";
            string RevitSettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\RevitImportSettings.xml";
            //string GroupSettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\GroupSettings.xml";

            bool firstLaunch = false;

            try
            {
                //Directory
                if (!Directory.Exists(appdatafolder))
                {
                    //This must be the first time you load Carbo Life Calc, we will be copying the template file to:
                    MessageBox.Show("Hi, this must be the first time you are using Carbo Life Calculator, I will now copy a material template file to: " + appdatafolder + Environment.NewLine + " This will be your default template file ", "Welcome", MessageBoxButton.OK);
                    Directory.CreateDirectory(appdatafolder);
                    Directory.CreateDirectory(appdatafolder + "\\online\\");

                    log += appdatafolder + " created" + Environment.NewLine;
                    log += appdatafolder + "\\online\\" + " created" + Environment.NewLine;

                    firstLaunch = true;
                }

                //SettingsFile
                if (!(File.Exists(SettingsFile)))
                {
                    if (File.Exists(pathDatabase + "CarboSettings.xml"))
                    {
                        File.Copy(pathDatabase + "CarboSettings.xml", SettingsFile);
                        log += SettingsFile + " Copied" + Environment.NewLine;
                    }
                }

                //Revit Import Settings
                if (!(File.Exists(RevitSettingsFile)))
                {
                    if (File.Exists(pathDatabase + "RevitImportSettings.xml"))
                    {
                        File.Copy(pathDatabase + "RevitImportSettings.xml", RevitSettingsFile);
                        log += RevitSettingsFile + " Copied" + Environment.NewLine;

                    }
                }

                //Material Template
                if (!(File.Exists(TemplateFile)))
                {
                    if (File.Exists(bufferPath))
                    {
                        File.Copy(bufferPath, TemplateFile);
                        log += TemplateFile + " Copied" + Environment.NewLine;

                    }
                }

                //Set the path to the template, ONLY when it's the first launch.
                if (firstLaunch == true)
                {
                        CarboSettings settings = new CarboSettings().Load();
                        settings.templatePath = appdatafolder + "UserMaterials.cxml";
                        settings.Save();
                    log += settings.templatePath + " Set as template file" + Environment.NewLine;

                }
                if (log != "")
                {
                    log += "Successfully Created Project Files: " + Environment.NewLine;
                    MessageBox.Show(log);
                }

            }
            catch (Exception ex)
            {
                log += "Error: " + ex.Message + Environment.NewLine;

                MessageBox.Show(log);
            }

        }


        /// <summary>
        /// Sets and prepares the usermaterials and settings File
        /// </summary>
        public static void CheckFileLocationsNew()
        {
            bool okSettingFile = false;
            bool okTemplate = false;
            bool okRevitSettingFile = false;
            bool firstLaunch = false;

            string log = "";

            string pathDatabase = Utils.getAssemblyPath() + "\\db\\";

            string bufferPath = pathDatabase + "MaterialBuffer.cxml";


            //All the following three files bust be present:
            string TemplateFile = pathDatabase + "UserMaterials.cxml";
            string SettingsFile = pathDatabase + "CarboSettings.xml";
            string RevitSettingsFile = pathDatabase  + "RevitImportSettings.xml";
            //string GroupSettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\GroupSettings.xml";


            try
            {
                //This bit will prevent the user files to be ever overwritten
                if (!(File.Exists(TemplateFile)))
                {
                    //If we dont have a template, create one:
                    if (File.Exists(bufferPath))
                    {
                        File.Copy(bufferPath, TemplateFile);
                        okTemplate = true;
                        firstLaunch = true;
                    }
                }

                //SettingsFile
                if (File.Exists(SettingsFile))
                {
                    okSettingFile = true;
                    CarboSettings settings = new CarboSettings().Load();
                    if (settings.firstLaunch == true)
                    {
                        firstLaunch = true;
                    }
                }
                else
                {
                    log += SettingsFile + " Not Found" + Environment.NewLine;
                }

                //Revit Import Settings
                if (File.Exists(RevitSettingsFile))
                {
                    okRevitSettingFile = true;
                }
                else
                {
                    log += RevitSettingsFile + " Not Found" + Environment.NewLine;
                }

                //If the app was downloaded from the Autodesk store we need to set the template file as it will always be part of the application:


                //Set the path to the template, ONLY when it's the first launch.
                if (firstLaunch == true)
                {
                    CarboSettings settings = new CarboSettings().Load();
                    settings.templatePath = TemplateFile;
                    settings.Save();
                    log += "Hi, this is most likely the first time you started Carbo Life Calculator. " + Environment.NewLine + " To store all your embodied carbon materials this program needs a template file." + Environment.NewLine + "This has been set to the following file: " + Environment.NewLine
                        + settings.templatePath + Environment.NewLine 
                        + "You are all good to go now, good luck!";

                    settings.firstLaunch = false;
                    settings.Save();
                }

                if (log != "")
                {
                    MessageBox.Show(log);
                }

            }
            catch (Exception ex)
            {
                log += "Error: " + ex.Message + Environment.NewLine;

                MessageBox.Show(log);
            }

        }


        /// <summary>
        /// Finds the location of the Revit Import Settings File
        /// </summary>
        /// <returns>Revit Import Settings File path</returns>
        public static string getRevitImportSettingspath(bool local = true)
        {
            //Finds and returns the revit import settings in the AppData folder, or in local if not found in Appdata.

            string myPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\RevitImportSettings.xml";
            string myLocalPath = getAssemblyPath() + "\\db\\" + "RevitImportSettings.xml";

            if (local == true)
            {
                return myLocalPath;
            }
            else
            { 
                if (File.Exists(myPath))
                    return myPath;
                else if (File.Exists(myLocalPath))
                    return myLocalPath;
                else
                {
                    MessageBox.Show("Could not find a path reference to the RevitImportSettings.xml, you possibly have to re-install the software" + Environment.NewLine +
                            "Target: " + myPath + Environment.NewLine +
                            "Target: " + myLocalPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
                }
            }
        }

        /// <summary>
        /// Finds the location of the Carbo Life Calculator Settings File
        /// </summary>
        /// <returns>Settings File path</returns>
        internal static string getSettingsFilePath(bool local = true)
        {
            //string fileName = "db\\CarboSettings.xml";
            string myPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\CarboSettings.xml";
            string myLocalPath = getAssemblyPath() + "\\db\\" + "CarboSettings.xml";

            if (local == true)
            {
                return myLocalPath;
            }
            else
            {
                if (File.Exists(myPath))
                    return myPath;
                else if (File.Exists(myLocalPath))
                    return myLocalPath;
                else
                {
                    MessageBox.Show("Could not find a path reference to the CarboLifeCalculator Setting File, you possibly have to re-install the software" + Environment.NewLine +
                            "Target: " + myPath + Environment.NewLine +
                            "Target: " + myLocalPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
                }
            }
        }

        /// <summary>
        /// Finds the location of the Carbo Life Calculator Template File
        /// </summary>
        /// <returns>Template Path</returns>
        public static string getTemplateFolder(bool local = true)
        {
            try
            {
                CarboSettings settings = new CarboSettings().Load();

                string templatetarget = settings.templatePath;
                string myLocalPath = getAssemblyPath() + "\\db\\" + "UserMaterials.cxml";

                if (File.Exists(templatetarget))
                    return templatetarget;
                else if (File.Exists(myLocalPath))
                {
                    //Fix the setting;
                    settings.templatePath = myLocalPath;
                    settings.Save();
                    MessageBox.Show("The template file could not be found at: " + templatetarget + Environment.NewLine + "The local template file will be used at: " + myLocalPath, "Warning", MessageBoxButton.OK);
                    return myLocalPath;
                }
                else
                {
                    MessageBox.Show("Could not find a path reference to the template file, you possibly have to re-install the software" + Environment.NewLine +
                        "Target: " + templatetarget + Environment.NewLine +
                        "Target: " + myLocalPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
            //return "";
        }

        /// <summary>
        /// Finds the location of the Carbo Life Calculator Downloaded Files Path
        /// </summary>
        /// <returns>Download Path</returns>
        public static string getDownloadedPath(bool local = true)
        {
            //sourcePath = PathUtils.getDownloadedPath() + "\\db\\online\\" + selectedItem + ".cxml";


            string myPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CarboLifeCalc\\online\\";
            string myLocalPath = getAssemblyPath() + "\\db\\online\\";

            if (local == true)
            {
                return myLocalPath;
            }
            else
            { 
                if (Directory.Exists(myPath))
                    return myPath;
                else if (Directory.Exists(myLocalPath))
                    return myLocalPath;
                else
                {
                    MessageBox.Show("Could not find a path reference to the download path, you possibly have to re-install the software" + Environment.NewLine +
                            "Target: " + myPath + Environment.NewLine +
                            "Target: " + myLocalPath, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return "";
                }
            }
        }
    }
}