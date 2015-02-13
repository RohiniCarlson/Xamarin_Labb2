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
using AUri = Android.Net.Uri;

namespace Labb2
{
    [Activity(Label = "@string/create_reports_activity_label")]
    public class CreateReportsActivity : Activity
    {
        private Button taxReportButton, accountReportButton;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_create_reports);
            taxReportButton = FindViewById<Button>(Resource.Id.tax_report_button);
            accountReportButton = FindViewById<Button>(Resource.Id.account_report_button);
            taxReportButton.Click += CreateTaxReport;
            accountReportButton.Click += CreateAccountReport;
        }

        private void CreateTaxReport(object sender, EventArgs e)
        {
            string reportText = BookKeeperManager.Instance.GetTaxReport();
            string subject = GetString(Resource.String.tax_report);
            AUri uri = AUri.Parse("mailto:default@recipient.com");
            Intent emailIntent = new Intent(Intent.ActionSendto, uri);
            emailIntent.PutExtra(Intent.ExtraSubject, subject);
            emailIntent.PutExtra(Intent.ExtraText, reportText);
            StartActivity(emailIntent);
        }

        private void CreateAccountReport(object sender, EventArgs e)
        {
            string reportText = BookKeeperManager.Instance.GetAccountReport();
            string subject = GetString(Resource.String.account_report);
            AUri uri = AUri.Parse("mailto:default@recipient.com");
            Intent emailIntent = new Intent(Intent.ActionSendto, uri);
            emailIntent.PutExtra(Intent.ExtraSubject, subject);
            emailIntent.PutExtra(Intent.ExtraText, reportText);
            StartActivity(emailIntent);
        }
    }
}