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
    [Activity(Label = "Alla Händelser")]   
    public class ShowAllEntriesActivity : Activity
    {
        private ListView entriesListView;
        private EntriesAdapter entriesAdapter;
        private BookKeeperManager bookKeeperManager;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            // Set our view from the "activity_show_all_entries" layout resource
            SetContentView(Resource.Layout.activity_show_all_entries);
            bookKeeperManager = BookKeeperManager.Instance;
            entriesAdapter = new EntriesAdapter(this, bookKeeperManager.Entries);
            entriesListView = FindViewById<ListView>(Resource.Id.entries_list_view);
            entriesListView.Adapter = entriesAdapter;
            entriesListView.ItemClick += entriesListView_ItemClick;
        }

        private void entriesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Entry entry = entriesAdapter[e.Position];
           // Toast.MakeText(this, "You have clicked on the entry with Id:" + entry.Id,ToastLength.Short).Show();
            Intent i = new Intent(this, typeof(ViewEntryActivity));
            i.PutExtra("EntryId", entry.Id);
            StartActivity(i);
        }
    }
}