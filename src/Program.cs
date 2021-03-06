﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SAFT_Reader.UI;

using Syncfusion.Licensing;

namespace SAFT_Reader
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            SyncfusionLicenseProvider.RegisterLicense(FindLicenseKey());
            CompositionRoot.Wire(new ApplicationModule());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Globals.AttachedFilePaths = new List<string>();
            //Globals.AttachedAuditFiles = new List<AuditFile>();

            Globals.AttachedFiles = new List<AttachedFile>();

            var splash = CompositionRoot.Resolve<SplashForm>();
            splash.IsSplash = true;
            splash.ShowDialog();

            var openFileDialog = CompositionRoot.Resolve<OpenFileDialogForm>();
            Application.Run(openFileDialog);
        }

        public static string FindLicenseKey()
        {
            string licenseKeyFile = "SyncfusionLicense.txt";
            for (int n = 0; n < 20; n++)
            {
                if (!System.IO.File.Exists(licenseKeyFile))
                {
                    licenseKeyFile = @"..\" + licenseKeyFile;
                    continue;
                }
                return System.IO.File.ReadAllText(licenseKeyFile);
            }
            return string.Empty;
        }
    }
}