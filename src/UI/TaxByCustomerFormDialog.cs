﻿using Syncfusion.Data;
using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;

using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SAFT_Reader.UI
{
    public partial class TaxByCustomerFormDialog :SfForm
    {
        public SfDataGrid DataGrid
        {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxByCustomerFormDialog"/> class.
        /// </summary>
        public TaxByCustomerFormDialog()
        {
            InitializeComponent();
            InitializeView();
        }

        /// <summary>
        /// Initializes the view of the TaxByCustomerFormDialog.
        /// </summary>
        private void InitializeView()
        {
            var records = Globals.AuditFile.SourceDocuments
                                                .SalesInvoices
                                                .Invoice
                                                .Select(z => new
                                                {
                                                    NIF = Globals.AuditFile.MasterFiles
                                                                                        .Customer
                                                                                        .Where(x => x.CustomerID.Equals(z.CustomerID))
                                                                                        .FirstOrDefault()
                                                                                        .CustomerTaxID,
                                                    Cliente = Globals.AuditFile.MasterFiles
                                                                                        .Customer
                                                                                        .Where(x => x.CustomerID.Equals(z.CustomerID))
                                                                                        .FirstOrDefault()
                                                                                        .CompanyName,
                                                })
                                                .GroupBy(p => p.NIF)
                                                .Select(p => p.First())
                                                .ToList();

            this.multiColumnComboBox1.DataSource = records;
            this.multiColumnComboBox1.DisplayMember = "Cliente";
            this.multiColumnComboBox1.ValueMember = "NIF";
        }

        /// <summary>
        /// Handles the click event of the OK button in the TaxByCustomerFormDialog.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            var filter = multiColumnComboBox1.SelectedValue;
            DataGrid.ClearFilters();
            DataGrid.ClearGrouping();
            DataGrid.ClearSorting();

            DataGrid.Columns["CustomerTaxID"].FilterPredicates.Add(new FilterPredicate()
            {
                FilterType = FilterType.Equals,
                FilterValue = filter
            });
            if (chkOnlyNormal.Checked)
            {
                DataGrid.Columns["InvoiceStatus"].FilterPredicates.Add(new FilterPredicate()
                {
                    FilterType = FilterType.Equals,
                    FilterValue = "N"
                });
            }
            DataGrid.GroupColumnDescriptions.Add(new GroupColumnDescription()
            {
                ColumnName = "TaxCode"
            });
            //DataGrid.ExpandAllGroup();
            DataGrid.AutoFitGroupDropAreaItem = true;

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Handles the click event of the Cancel button in the TaxByCustomerFormDialog.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Close();
            Cursor.Current = Cursors.Default;
        }
    }
}