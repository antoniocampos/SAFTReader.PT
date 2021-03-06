﻿using System;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.WinForms.Controls;

namespace SAFT_Reader.UI
{
    public partial class SplashForm : SfForm
    {
        public bool IsSplash { get; set; }

        private int _counter;

        public SplashForm()
        {
            InitializeComponent();
        }

        private void InitializeView()
        {
            if (!IsSplash)
            {
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
            }

            timer1.Enabled = IsSplash;
            lblVersion.Text = $"{Globals.VersionLabel}";
            lblReader.ForeColor = ColorTranslator.FromHtml("#00BFA6");
            lblCopy.Text = "Copyright 2020, Rui Ribeiro. Todos os direitos reservados.";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _counter++;

            if (_counter == 50)
            {
                timer1.Stop();
                this.Close();
            }
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            InitializeView();
        }
    }
}