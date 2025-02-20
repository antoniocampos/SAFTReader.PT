﻿using Programatica.Saft.Models;

using SAFT_Reader.Services;

using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;

using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SAFT_Reader.UI
{
    public partial class AttachedFilesFormDialog :SfForm
    {
        public SfDataGrid DataGrid
        {
            get; set;
        }

        private readonly IAuditService _auditService;

        /// <summary>
        /// Initializes a new instance of the AttachedFilesFormDialog class.
        /// </summary>
        /// <param name="auditService">An implementation of IAuditService for auditing operations.</param>
        public AttachedFilesFormDialog(IAuditService auditService)
        {
            _auditService = auditService;

            InitializeComponent();
            InitializeView();
        }

        /// <summary>
        /// Initializes the view of the AttachedFilesFormDialog.
        /// </summary>
        /// <remarks>
        /// This method sets the initial file path in the text box based on the principal attached file,
        /// and refreshes the grid view to display the attached files.
        /// </remarks>
        private void InitializeView()
        {
            this.txtFilePath.Text = Globals.AttachedFiles
                                            .Where(x => x.IsPrincipal == true)
                                            .FirstOrDefault()
                                            .FilePath;
            RefreshGridView();
        }

        /// <summary>
        /// Handles the click event of the OK button in the dialog.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Handles the click event of the Cancel button in the dialog.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Globals.AuditFile = _auditService.MergeAudits();
            this.DialogResult = DialogResult.OK;
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Refreshes the data displayed in the grid view of attached audit files.
        /// </summary>
        /// <remarks>
        /// This method clears the current items in the grid view and populates it with information
        /// about attached audit files, including their file paths, start dates, and end dates.
        /// Additionally, it enables the delete button if there are attached files.
        /// </remarks>
        private void RefreshGridView()
        {
            this.listView1.Items.Clear();
            foreach (var file in Globals.AttachedFiles.Where(x => x.IsPrincipal == false))
            {
                var i = listView1.Items.Add(file.FilePath);
                _ = i.SubItems.Add(file.AuditFile.Header.StartDate);
                _ = i.SubItems.Add(file.AuditFile.Header.EndDate);
            }
            if (Globals.AttachedFiles.Count > 0)
            {
                cmdDelete.Enabled = true;
            }
        }

        /// <summary>
        /// Handles the click event of the Add button to add an attached audit file.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void cmdAdd_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var subaudit = _auditService.OpenFile(openFileDialog1.FileName);

                if (ValidateFile(subaudit) == true)
                {
                    Globals.AttachedFiles.Add(new AttachedFile
                    {
                        ID = Guid.NewGuid(),
                        AuditFile = subaudit,
                        FilePath = openFileDialog1.FileName,
                        IsPrincipal = false
                    });

                    RefreshGridView();
                }
                else
                {
                    _ = MessageBox.Show("Ocorreu um erro ao abrir o ficheiro Saft-PT. \n\r" +
                        "Garanta que se trata de um ficheiro válido, no formato 1.04_01 " +
                        "e que corresponde à mesma empresa do ficheiro principal",
                        "Erro ao abrir ficheiro Saft-PT",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Validates whether the provided audit file matches the company of the principal audit file.
        /// </summary>
        /// <param name="b">The audit file to validate.</param>
        /// <returns>True if the provided audit file matches the company of the principal audit file; otherwise, false.</returns>
        private bool ValidateFile(AuditFile b)
        {
            bool r = true;
            var a = Globals.AttachedFiles.Where(x => x.IsPrincipal == true).FirstOrDefault().AuditFile;
            if (a.Header.CompanyID != b.Header.CompanyID)
            {
                r = false;
            }
            return r;
        }

        /// <summary>
        /// Handles the click event of the Delete button to remove an attached audit file.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void cmdDelete_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var itemToRemove = listView1.SelectedItems[0];
            if (itemToRemove != null)
            {
                var item = Globals.AttachedFiles.Where(x => x.FilePath.Equals(itemToRemove.Text)).FirstOrDefault();
                _ = Globals.AttachedFiles.Remove(item);
            }
            RefreshGridView();
            Cursor.Current = Cursors.Default;
        }
    }
}