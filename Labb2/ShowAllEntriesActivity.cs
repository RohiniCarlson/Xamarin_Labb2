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
    [Activity(Label = "@string/show_all_entries_activity_label")]   
    public class ShowAllEntriesActivity : Activity
    {
        private ListView entriesListView;
        private EntriesAdapter entriesAdapter;
        private BookKeeperManager bookKeeperManager;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "activity_show_all_entries" layout resource
            SetContentView(Resource.Layout.activity_show_all_entries);
            bookKeeperManager = BookKeeperManager.Instance;
            entriesListView = FindViewById<ListView>(Resource.Id.entries_list_view);
            entriesListView.ItemClick += entriesListView_ItemClick;
        }

        protected override void OnResume()
        {
            base.OnResume();
            entriesAdapter = new EntriesAdapter(this, bookKeeperManager.Entries);
            entriesListView.Adapter = entriesAdapter;
        }

        private void entriesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Entry entry = entriesAdapter[e.Position];
            Intent i = new Intent(this, typeof(ViewEntryActivity));
            i.PutExtra("EntryId", entry.Id);
            StartActivity(i);
        }
    }
}