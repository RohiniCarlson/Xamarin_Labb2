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
    [Activity(Label = "Create Reports")]
    public class CreateReportsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "activity_create_reports" layout resource
            SetContentView(Resource.Layout.activity_create_reports);
        }
    }
}