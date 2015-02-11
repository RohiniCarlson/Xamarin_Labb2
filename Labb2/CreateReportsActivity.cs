using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Labb2
{
    [Activity(Label = "@string/create_reports_activity_label")]
    public class CreateReportsActivity : Activity
    {
        private Button taxReportButton, accountReportButton;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "activity_create_reports" layout resource
            SetContentView(Resource.Layout.activity_create_reports);
            taxReportButton = FindViewById<Button>(Resource.Id.tax_report_button);
            accountReportButton = FindViewById<Button>(Resource.Id.account_report_button);
            taxReportButton.Click += CreateTaxReport;
            accountReportButton.Click += CreateAccountReport;
        }

        private void CreateTaxReport(object sender, EventArgs e)
        {

        }

        private void CreateAccountReport(object sender, EventArgs e)
        {

        }
    }
}