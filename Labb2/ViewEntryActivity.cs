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
using AUri = Android.Net.Uri;
using Android.Graphics;


namespace Labb2
{
    [Activity(Label = "@string/view_entry_label", Theme = "@style/Theme.AppCompat")]
    public class ViewEntryActivity : ActionBarActivity
    {
        private BookKeeperManager bookKeeperManager;
        private TextView date, description, transactionAccount, moneyAccount, amount, taxRate;
        private ImageView receiptImage;
        private int entryId;
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
            receiptImage = FindViewById<ImageView>(Resource.Id.view_receipt_image);

            entryId = Intent.GetIntExtra("EntryId", -1);
            if (entryId != -1)
            {
                Entry e = bookKeeperManager.GetEntry(entryId);               
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
                if ((e.ImagePath != null) && (e.ImagePath.Length > 0))
                {
                    int height = Resources.DisplayMetrics.HeightPixels;
                    int width = receiptImage.Width;
                    Bitmap bitmap = ImageUtils.LoadAndScaleBitmap(e.ImagePath, width, height);
                    receiptImage.SetImageBitmap(bitmap);
                    //receiptImage.SetImageURI(AUri.Parse(e.ImagePath));
                    //Toast.MakeText(this, "stored image path is: " + e.ImagePath, ToastLength.Long).Show();
                }                
            }
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            base.OnCreateOptionsMenu (menu);
            MenuInflater inflater = new MenuInflater(this);
            inflater.Inflate(Resource.Menu.activity_view_entry_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            base.OnOptionsItemSelected (item);

            switch(item.ItemId)
            {
                case Resource.Id.edit:
                    {
                        GoToUpdateActivity();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return true;
        }

        private void GoToUpdateActivity()
        {
            Intent i = new Intent(this, typeof(CreateNewEntryActivity));
            i.PutExtra("activityType", "update");
            i.PutExtra("entryId", entryId);
            StartActivity(i);
            Finish();
        }
    }
}