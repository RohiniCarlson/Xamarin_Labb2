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
using AEnvironment = Android.OS.Environment;
using AFile = Java.IO.File;
using AUri = Android.Net.Uri;
using Android.Provider;
using Android.Graphics;

namespace Labb2
{
    [Activity(Label = "Händelse")]
    public class CreateNewEntryActivity : Activity
    {
        const int DATE_DIALOG_ID = 0;
        const int CAMERA_REQUEST_CODE = 1;

        private EditText dateOfEntry, totalWithTax, entryDescription;
        private Button datePickerButton, saveButton, deleteButton, takePhotoButton;
        private DateTime date;
        private RadioButton income_radio, cost_radio;
        private Spinner typeSpinner, accountSpinner, taxSpinner;
        private ArrayAdapter typeSpinnerAdapter, accountSpinnerAdapter, taxSpinnerAdapter;
        private TextView totalWithoutTax;
        private ImageView receiptImage;
        private AUri imagePathUri;
        private string imagePath="";
        private string activityType;
        private BookKeeperManager bookKeeperManager;
        private int entryId;
        Entry entry;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "activity_create_new_entry" layout resource
            SetContentView(Resource.Layout.activity_create_new_entry);

            // Get an instance of BookKeeperManager
            bookKeeperManager = BookKeeperManager.Instance;

            dateOfEntry = FindViewById<EditText>(Resource.Id.date_edit);
            datePickerButton = FindViewById<Button>(Resource.Id.date_picker_button);
            entryDescription = FindViewById<EditText>(Resource.Id.entry_description_edit);
            typeSpinner = FindViewById<Spinner>(Resource.Id.type_spinner);
            income_radio = FindViewById<RadioButton>(Resource.Id.income_radio_button);
            cost_radio = FindViewById<RadioButton>(Resource.Id.cost_radio_button);
            accountSpinner = FindViewById<Spinner>(Resource.Id.account_spinner);
            taxSpinner = FindViewById<Spinner>(Resource.Id.tax_spinner);
            totalWithoutTax = FindViewById<TextView>(Resource.Id.calculated_total_without_tax_text);
            totalWithTax = FindViewById<EditText>(Resource.Id.total_with_tax_edit);
            takePhotoButton = FindViewById<Button>(Resource.Id.take_photo_button);
            receiptImage = FindViewById<ImageView>(Resource.Id.receipt_image);

            saveButton = FindViewById<Button>(Resource.Id.save_entry_button);
            deleteButton = FindViewById<Button>(Resource.Id.delete_entry_button);

            activityType = Intent.GetStringExtra("activityType");
            if ("new".Equals(activityType))
            {
                income_radio.Checked = true;
                date = DateTime.Today;
                UpdateDate();
                deleteButton.Enabled = false;
                populateTypeSpinner();
                PopulateAccountSpinner();
                PopulateTaxSpinner();
                HandleEvents(); 
            }
            else if ("update".Equals(activityType))
            {
                // As soon as user changes something, enable savebutton
                // saveButton.Enabled = false;
                entryId = Intent.GetIntExtra("entryId", -1);
                if (entryId != -1)
                {
                    SetUpEntryDataForUpdate();
                    HandleEvents();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.entry_not_found),ToastLength.Short).Show();
                }             
            }
            else 
            { 
                // Do nothing --- show toast ??
            }             
        }

        private void SetUpEntryDataForUpdate()
        {
            date = DateTime.Today;
            // Get entry via id and populate fields
            entry = bookKeeperManager.GetEntry(entryId);
            entryDescription.Text = entry.Description;
            dateOfEntry.Text = entry.Date;
            totalWithTax.Text = entry.TotalAmount.ToString();

            Account account = bookKeeperManager.GetAccount(entry.AccountNumber);
            if (account.Type == Account.AccountType.Expense)
            {
                cost_radio.Checked = true;
                populateTypeSpinner();
                typeSpinner.SetSelection(bookKeeperManager.ExpenseAccounts.FindIndex(a => a.Number == account.Number), true);
            }
            else if (account.Type == Account.AccountType.Income)
            {
                income_radio.Checked = true;
                populateTypeSpinner();
                typeSpinner.SetSelection(bookKeeperManager.MoneyAccounts.FindIndex(a => a.Number == account.Number), true);
            }
            PopulateAccountSpinner();
            account = bookKeeperManager.GetAccount(entry.MoneyAccountNumber);
            accountSpinner.SetSelection(bookKeeperManager.MoneyAccounts.FindIndex(a => a.Number == account.Number), true);
            
            PopulateTaxSpinner();
            TaxRate taxRate = bookKeeperManager.GetTaxRate(entry.TaxId);
            taxSpinner.SetSelection(bookKeeperManager.TaxRates.FindIndex(a => a.Id == taxRate.Id), true);
            receiptImage.SetImageURI(AUri.Parse(entry.ImagePath));
        }

        private void HandleEvents()
        {
            // Handle the click events on buttons and radio buttons
            datePickerButton.Click += button_DatePicker;
            saveButton.Click += button_SaveEntry;
            takePhotoButton.Click += button_TakePhoto;
            income_radio.Click += spinner_PopulateType;
            cost_radio.Click += spinner_PopulateType;            

            // Handle the ItemSelected event in the tax rates spinner
            taxSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_TaxSelected);

            // Handle the TextChanged event in the total amount with text edittext
            totalWithTax.TextChanged += edittext_UpdateTotalWitoutTax;
        }

        private void UpdateDate()
        {
            dateOfEntry.Text = date.ToString("yyyy-MM-dd");
        }

        private void populateTypeSpinner()
        {
            if (income_radio.Checked)
            {
                typeSpinnerAdapter = new ArrayAdapter<Account>(this, Android.Resource.Layout.SimpleSpinnerItem, bookKeeperManager.IncomeAccounts);

            }
            else if (cost_radio.Checked)
            {
                typeSpinnerAdapter = new ArrayAdapter<Account>(this, Android.Resource.Layout.SimpleSpinnerItem, bookKeeperManager.ExpenseAccounts);
            }
            typeSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            typeSpinner.Adapter = typeSpinnerAdapter;
        }

        private void PopulateAccountSpinner()
        {
            accountSpinnerAdapter = new ArrayAdapter<Account>(this, Android.Resource.Layout.SimpleSpinnerItem, bookKeeperManager.MoneyAccounts);
            accountSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            accountSpinner.Adapter = accountSpinnerAdapter;
        }

        private void PopulateTaxSpinner()
        {
            taxSpinnerAdapter = new ArrayAdapter<TaxRate>(this, Android.Resource.Layout.SimpleSpinnerItem, bookKeeperManager.TaxRates);
            taxSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            taxSpinner.Adapter = taxSpinnerAdapter;
        }

        private void button_DatePicker(object sender, EventArgs e)
        {
            ShowDialog(DATE_DIALOG_ID);
        }

        protected override Dialog OnCreateDialog(int id)
        {
            switch (id)
            {
                case DATE_DIALOG_ID:
                    return new DatePickerDialog(this, OnDateSet, date.Year, date.Month - 1, date.Day);
            }
            return null;
        }

        public void OnDateSet(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            date = e.Date;
            UpdateDate();
            //date = DateTime.Today;
        }

        private void button_TakePhoto(object sender, EventArgs e)
        { 
            // Button b = sender as Button;
            // Button b = (Button)sender;
            //b.setBackgroundColor();
            AFile picDir = AEnvironment.GetExternalStoragePublicDirectory(AEnvironment.DirectoryPictures);
            AFile receiptsDir = new AFile(picDir, "Bookkeeper");
            if (!receiptsDir.Exists())
            {
                receiptsDir.Mkdirs();
            }
            if ("new".Equals(activityType))
            {
                imagePath = "receiptId" +(bookKeeperManager.GetLastEntryId() + 1) +".jpg";
            } 
            else if ("update".Equals(activityType))
            {
                imagePath = "receiptId" + entry.Id + ".jpg";
            }
            
            AFile myFile = new AFile(receiptsDir, imagePath);

            imagePathUri = AUri.FromFile(myFile);
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            intent.PutExtra(MediaStore.ExtraOutput, imagePathUri);
            StartActivityForResult(intent, CAMERA_REQUEST_CODE);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == CAMERA_REQUEST_CODE && resultCode == Result.Ok)
            {
                int height = Resources.DisplayMetrics.HeightPixels;
                int width = receiptImage.Width;
                Bitmap bitmap = ImageUtils.LoadAndScaleBitmap(imagePathUri.Path, width, height);
                receiptImage.SetImageBitmap(bitmap);       
            }
            base.OnActivityResult(requestCode, resultCode, data);
            Toast.MakeText(this, "Pic saved at: " + imagePath, ToastLength.Long).Show();
        }
     
        private void button_SaveEntry(object sender, EventArgs e)
        {
            // Convert the selected spinner items to string and do necessary conversions
            string taxRate = taxSpinner.SelectedItem.ToString();
            taxRate = taxRate.Substring(0, taxRate.Length - 1); // Gets rid of the percentage sign at the end

            // Retrieve the Ids of the selected spinner items
            int accountId = GetAccountIdFromString(typeSpinner.SelectedItem.ToString());
            int moneyAccountId = GetAccountIdFromString(accountSpinner.SelectedItem.ToString());
            int taxRateId = bookKeeperManager.TaxRates.Find(t => t.Tax == Convert.ToDouble(taxRate)).Id;

            if("new".Equals(activityType))
            {
                // Request BookKeeperManger to save the entry
                bookKeeperManager.AddEntry(dateOfEntry.Text,
                                                    FindViewById<EditText>(Resource.Id.entry_description_edit).Text,
                                                    accountId,
                                                    moneyAccountId,
                                                    Convert.ToDouble(totalWithTax.Text),
                                                    taxRateId,
                                                    imagePathUri.ToString());
                Toast.MakeText(this, GetString(Resource.String.entry_created), ToastLength.Short).Show();
            }
            else if ("update".Equals(activityType))
            {
                // Request BookKeeperManger to update the entry
                bookKeeperManager.UpdateEntry(entryId,
                                              dateOfEntry.Text,
                                              FindViewById<EditText>(Resource.Id.entry_description_edit).Text,
                                              accountId,
                                              moneyAccountId,
                                              Convert.ToDouble(totalWithTax.Text),
                                              taxRateId,
                                              imagePathUri.ToString());
                Toast.MakeText(this, GetString(Resource.String.entry_updated), ToastLength.Short).Show();
            }
            resetEntries();
        }

        private int GetAccountIdFromString(string s)
        {
            int index = s.IndexOf(" ");
            return int.Parse(s.Substring(0,index));
        }    

        private void spinner_PopulateType(object sender, EventArgs e)
        {
            populateTypeSpinner();
        }

        private void spinner_TaxSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner s = (Spinner)sender;
            double totalIncludingTax = 0.0;

            if ((totalWithTax.Text.ToString().Length > 0) && (Convert.ToDouble(totalWithTax.Text.ToString()) > 0.0))
            {
                totalIncludingTax = Convert.ToDouble(totalWithTax.Text.ToString());
                totalWithoutTax.Text = Convert.ToString(CalculateAmountWithoutTax(totalIncludingTax, s.SelectedItem.ToString()));
            }
        }
      
        private void edittext_UpdateTotalWitoutTax(object sender, Android.Text.TextChangedEventArgs e)
        {
            if ((totalWithTax.Text.ToString().Length > 0) && (Convert.ToDouble(totalWithTax.Text.ToString()) > 0.0))
            {
                totalWithoutTax.Text = Convert.ToString(CalculateAmountWithoutTax(Convert.ToDouble(totalWithTax.Text.ToString()), taxSpinner.SelectedItem.ToString()));
            }
            else
            {
                totalWithoutTax.Text = "";
            }
        }

        private double CalculateAmountWithoutTax(double amountWithTax, string taxRate)
        {
            double tax = Convert.ToDouble(taxRate.Substring(0, taxRate.Length - 1));
            tax = tax / 100;
            return (amountWithTax - (amountWithTax * tax));
        }

        private void resetEntries()
        {
            // Reset date to current date
            dateOfEntry.Text = DateTime.Today.ToString("yyyy-MM-dd");
            //dateOfEntry.Text = DateTime.Today.ToString("d");

            // Select income radio button
            income_radio.Checked = true;

            // Clear description
            entryDescription.Text = "";

            // Reset type spinner
            populateTypeSpinner();

            // Reset account spinner
            accountSpinner.SetSelection(0, true);

            // Clear total amount
            totalWithTax.Text = "";

            // Reset tax spinner
            taxSpinner.SetSelection(0, true);

            // Clear total without tax
            totalWithoutTax.Text = "";
        }

        private int getIndex(Spinner spinner, String myString)
        {
            for (int i = 0; i < spinner.Count; i++)
            {
                if (spinner.GetItemAtPosition(i).Equals(myString))
                {
                   return i;
                }
            }
            return -1;
        }
    }
}