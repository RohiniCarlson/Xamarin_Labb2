using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Labb2
{
    [Activity(Label = "@string/main_activity_label", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private Button newButton, showButton, reportButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "activity_main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Get our views from the layout resource, and attach events as needed
            newButton = FindViewById<Button>(Resource.Id.new_button);
            showButton = FindViewById<Button>(Resource.Id.show_button);
            reportButton = FindViewById<Button>(Resource.Id.report_button);

            newButton.Click += delegate { CreateNewEntryActivity(); };            
            showButton.Click += delegate { ShowAllEntriesActivity(); };          
            reportButton.Click += delegate { CreateReportsActivity(); };
        }

        private void CreateNewEntryActivity()
        {
            Intent i = new Intent(this, typeof(CreateNewEntryActivity));
            i.PutExtra("activityType", "new");
            StartActivity(i);
        }

        private void ShowAllEntriesActivity()
        {
            Intent i = new Intent(this, typeof(ShowAllEntriesActivity));
            StartActivity(i);
        }

        private void CreateReportsActivity()
        {
            Intent i = new Intent(this, typeof(CreateReportsActivity));
            StartActivity(i);
        }
    }
}

