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
    public class EntriesAdapter: BaseAdapter<Entry>
    {
        private List<Entry> entryList;
        private Activity activity;
        private BookKeeperManager bookKeeperManager;

        public EntriesAdapter(Activity activity, List<Entry> entryList)
        {
            this.activity = activity;
            this.entryList = entryList;
        }

        public override int Count
        {
            get { return entryList.Count; }
        }

        public override Entry this[int position]
        {
            get { return entryList[position]; }
        }

        public override long GetItemId(int position)
        {
            return entryList[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.entry_list_item, parent, false);
            TextView entryDate = view.FindViewById<TextView>(Resource.Id.entry_date_text);
            TextView entryDescription = view.FindViewById<TextView>(Resource.Id.entry_desc_text);
            TextView entryAmount = view.FindViewById<TextView>(Resource.Id.entry_amount_text);
            entryDate.Text = entryList[position].Date;            
            entryDescription.Text = entryList[position].Description;
            entryAmount.Text = entryList[position].TotalAmount + "  " + activity.GetString(Resource.String.currency);

            bookKeeperManager = BookKeeperManager.Instance;
            Account account = bookKeeperManager.GetAccount(entryList[position].AccountNumber);
            // Set different colors according to type of transaction Expense/Income
            if (account.Type == Account.AccountType.Income)
            {
                entryDate.SetTextColor(Android.Graphics.Color.PaleTurquoise);
                entryDescription.SetTextColor(Android.Graphics.Color.PaleTurquoise);
                entryAmount.SetTextColor(Android.Graphics.Color.PaleTurquoise);
            }
            else if (account.Type == Account.AccountType.Expense)
            {
                entryDate.SetTextColor(Android.Graphics.Color.Orange);
                entryDescription.SetTextColor(Android.Graphics.Color.Orange);
                entryAmount.SetTextColor(Android.Graphics.Color.Orange);
            }
            return view;
        }
    }
}