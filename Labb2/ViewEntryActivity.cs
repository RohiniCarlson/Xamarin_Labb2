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
using Android.Support.V7.App;
using Android.Support.V4.View;


namespace Labb2
{
    [Activity(Label = "@string/view_entry_label", Theme = "@style/Theme.AppCompat")]
    public class ViewEntryActivity : ActionBarActivity
    {
        private BookKeeperManager bookKeeperManager;
        private TextView date, description, transactionAccount, moneyAccount, amount, taxRate;     
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Add action bar
            SupportRequestWindowFeature(WindowCompat.FeatureActionBar);

            // Set our view from the "activity_view_entry" layout resource
            SetContentView(Resource.Layout.activity_view_entry);
            
            bookKeeperManager = BookKeeperManager.Instance;

            // Get our views from the layout resource
            date = FindViewById<TextView>(Resource.Id.view_date_text);
            description = FindViewById<TextView>(Resource.Id.view_entry_description_text);
            transactionAccount = FindViewById<TextView>(Resource.Id.view_type_text);
            moneyAccount = FindViewById<TextView>(Resource.Id.view_to_from_account_text);
            amount = FindViewById<TextView>(Resource.Id.view_total_with_tax_text);
            taxRate = FindViewById<TextView>(Resource.Id.view_tax_text);


            int id = Intent.GetIntExtra("EntryId", -1);
            if (id != -1)
            {
                Entry e = bookKeeperManager.GetEntry(id);               
                Account account = bookKeeperManager.GetAccount(e.AccountNumber);
                Account cashAccount = bookKeeperManager.GetAccount(e.MoneyAccountNumber);
                TaxRate rate = bookKeeperManager.GetTaxRate(e.TaxId);
                date.Text = e.Date;
                description.Text = e.Description;
                transactionAccount.Text = account.ToString();
               /* if (account.Type == Account.AccountType.Income)
                {
                    transactionAccount.SetTextColor(Android.Graphics.Color.PaleTurquoise);
                }
                else if (account.Type == Account.AccountType.Expense)
                {
                    transactionAccount.SetTextColor(Android.Graphics.Color.Orange);
                }*/
                moneyAccount.Text = cashAccount.ToString();
                amount.Text = e.TotalAmount.ToString();
                taxRate.Text = rate.ToString();
            }
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            var actionItem = menu.Add(new Java.Lang.String("Action Button"));
            MenuItemCompat.SetShowAsAction(actionItem, MenuItemCompat.ShowAsActionIfRoom);
            actionItem.SetIcon(Android.Resource.Drawable.IcMenuEdit);
           // return true;
            return base.OnCreateOptionsMenu(menu);

            /*
             *  base.OnCreateOptionsMenu (menu);

      MenuInflater inflater = this.MenuInflater;

      inflater.Inflate (Resource.Menu.mainmenu, menu);

      return true;*/
        }

        /*
         * public override bool OnOptionsItemSelected (IMenuItem item)
    {
      base.OnOptionsItemSelected (item);

      switch (item.ItemId)
      {
      case Resource.Id.add:
        break;
      case Resource.Id.call:
        {
          Intent intent = new Intent (Intent.ActionDial);
          StartActivity (intent);
          break;
        }
      default:
        break;
      }

      return true;
    }
         * */
        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            Android.Widget.Toast.MakeText(this,
                "Selected Item: " +
                item.TitleFormatted,
                Android.Widget.ToastLength.Short).Show();

            return true;
        }
    }
}