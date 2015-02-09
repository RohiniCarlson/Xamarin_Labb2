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
using SQLite;

namespace Labb2
{
    public class BookKeeperManager
    {
        private static BookKeeperManager instance;
        private static List<TaxRate> taxRates;
        private static List<Account> incomeAccounts;
        private static List<Account> expenseAccounts;
        private static List<Account> moneyAccounts;
        private static List<Entry> entries;

        public string DBPath { get; private set; }
        public List<Account> IncomeAccounts 
        {
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                incomeAccounts = db.Table<Account>().Where(ic =>ic.Type==Account.AccountType.Income).ToList();
                db.Close();
                return incomeAccounts;
            }
        }
        public List<Account> ExpenseAccounts 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                expenseAccounts = db.Table<Account>().Where(ic => ic.Type == Account.AccountType.Expense).ToList();
                db.Close();
                return expenseAccounts;
            }
        }
        public List<Account> MoneyAccounts 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                moneyAccounts = db.Table<Account>().Where(ic => ic.Type == Account.AccountType.Money).ToList();
                db.Close();
                return moneyAccounts;
            }
        }
        public List<TaxRate> TaxRates 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                taxRates = db.Table<TaxRate>().ToList();
                db.Close();
                return taxRates;
            } 
        }
        public List<Entry> Entries 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                entries = db.Table<Entry>().ToList();
                db.Close();
                return entries;
            }
        }

        public int GetLastEntryId()
        {
            if (Entries.Count > 0)
            {
                Entry entry = Entries.ElementAt<Entry>(Entries.Count - 1);
                return entry.Id;
            }
            else
            {
                return 0;
            }            
        }

        public string GetTaxReport()
        {
            string report = "";
            double incomeTaxTotal = 0.0;
            double expenseTaxTotal = 0.0;
            double incomeTax = 0.0;
            double expenseTax = 0.0;

            SQLiteConnection db = new SQLiteConnection(DBPath);
            List<Entry> entryList = db.Table<Entry>().ToList();
            db.Close();

            foreach (Entry e in entryList)
            { 
                report = report + e.Date + " - " + e.Description +":  ";
               
                if(GetAccount(e.AccountNumber).Type == Account.AccountType.Expense)
                {
                    expenseTax = e.TotalAmount * (GetTaxRate(e.TaxId).Tax / 100);
                    report = report + " -" + expenseTax + " kr" + "\n";
                    expenseTaxTotal += expenseTax;
                }
                else if (GetAccount(e.AccountNumber).Type == Account.AccountType.Income)
                {
                    incomeTax = e.TotalAmount * (GetTaxRate(e.TaxId).Tax / 100);
                    report = report + " " + incomeTax + " kr" + "\n";
                    incomeTaxTotal += incomeTax;
                }
            }
            report = report + "Totalt att betala: " + (incomeTaxTotal - expenseTaxTotal) + " kr";
            db.Close();
            return report;
        }

        public string GetAccountReport()
        {
            StringBuilder reportText = new StringBuilder();
            double accountTotal = 0;

            SQLiteConnection db = new SQLiteConnection(DBPath);
            IEnumerable<Account> accountList = db.Table<Account>();

            foreach (Account a in accountList)
            {
                IEnumerable<Entry> entryList = db.Table<Entry>().Where(e => e.AccountNumber == a.Number);
                if (entryList.Count() > 0)
                {
                    StringBuilder accountDetails = new StringBuilder();
                   
                    foreach (Entry e in entryList)
                    {
                        accountTotal += e.TotalAmount;
                        if (GetAccount(a.Number).Type == Account.AccountType.Income)
                        {
                            accountDetails.Append(e.Date).Append(" - ").Append(e.Description);
                            accountDetails.Append(":  ").Append(e.TotalAmount).Append(" kr").Append("\n");
                        }
                        else
                        {
                            accountDetails.Append(e.Date).Append(" - ").Append(e.Description);
                            accountDetails.Append(":  -").Append(e.TotalAmount).Append(" kr").Append("\n");
                        }
                        
                    }
                    if (GetAccount(a.Number).Type == Account.AccountType.Income)
                    {
                        reportText.Append("*** ").Append(GetAccount(a.Number).Name).Append(" (").Append(a.Number);
                        reportText.Append(" (total: " + accountTotal + " kr)").Append("\n");
                    }
                    else
                    {
                        reportText.Append("*** ").Append(GetAccount(a.Number).Name).Append(" (").Append(a.Number);
                        reportText.Append(" (total: -" + accountTotal + " kr)").Append("\n");
                    }
                    reportText.Append(accountDetails);
                    reportText.Append("***").Append("\n");
                }
                else
                {
                    reportText.Append("*** ").Append(GetAccount(a.Number).Name).Append(" (").Append(a.Number);
                    reportText.Append(" (total: 0 kr").Append("\n").Append("***").Append("\n");
                }
            }
            return reportText.ToString();
        }

        /*private string CalculateTotalForAccount(IEnumerable<Entry> listEntries, int accountNumber, bool income)
        {
            string report = "";
            double total = 0;

            if (listEntries.Count() > 0)
            {
                total = listEntries.Select(e => e.TotalAmount).Sum();
                if (income)
                {
                    report = report + "*** " + GetAccount(accountNumber).Name + " (" + accountNumber + ")" + " (total: " + total + " kr)" + "\n";
                }
                else
                {
                    report = report + "*** " + GetAccount(accountNumber).Name + " (" + accountNumber + ")" + " (total: -" + total + " kr)" + "\n";
                }
                foreach (Entry e in listEntries)
                {
                    if (income)
                    {
                        report = report + e.Date + " - " + e.Description + ":  " + e.TotalAmount + " kr" + "\n";
                    }
                    else
                    {
                        report = report + e.Date + " - " + e.Description + ":  -" + e.TotalAmount + " kr" + "\n";
                    }                    
                }
                report = report + "***" + "\n";
            }
            return report;
        }*/

        private BookKeeperManager()
        {
            DBPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            DBPath += "\\database.db";
            SQLiteConnection db = new SQLiteConnection(DBPath);
            db.CreateTable<TaxRate>();
            if (db.Table<TaxRate>().Count() == 0)
            {
                TaxRate t = new TaxRate() { Tax = 6 };
                db.Insert(t);
                t = new TaxRate() { Tax = 12 };
                db.Insert(t);
                t = new TaxRate() { Tax = 25 };
                db.Insert(t);
            }
            db.CreateTable<Account>();
            if (db.Table<Account>().Count() == 0)
            {
                Account a = new Account() { Number = 3000, Name = "Försäljning", Type = Account.AccountType.Income };
                db.Insert(a);
                a = new Account() { Number = 3040, Name = "Försäljning av tjänster", Type = Account.AccountType.Income };
                db.Insert(a);

                a = new Account() { Number = 5400, Name = "Förbrukningsinventarier och förbrukningsmaterial", Type = Account.AccountType.Expense };
                db.Insert(a);
                a = new Account() { Number = 2013, Name = "Övriga egna uttag", Type = Account.AccountType.Expense };
                db.Insert(a);
                a = new Account() { Number = 5900, Name = "Reklam och PR", Type = Account.AccountType.Expense };
                db.Insert(a);

                a = new Account() { Number = 1910, Name = "Kassa", Type = Account.AccountType.Money };
                db.Insert(a);
                a = new Account() { Number = 1930, Name = "Företagskonto", Type = Account.AccountType.Money };
                db.Insert(a);
                a = new Account() { Number = 2018, Name = "Egna insättningar", Type = Account.AccountType.Money };
                db.Insert(a);                
            }
            db.CreateTable<Entry>();
            db.Close();        
        }

        public static BookKeeperManager Instance
        {
            get
            {
                // Different ways of doing the same thing
                // instance = instance ?? new BookKeeperManager();
                // return instance ?? new BookKeeperManager();

                if (instance == null)
                {
                    instance = new BookKeeperManager();   
                }
                return instance;
            }
        }

        public void AddEntry(string date, string description, int accountNumber, int moneyAccountNumber,                             
                             double total, int taxRateId, string imagePath)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            Entry e = new Entry { Date = date,
                                  Description = description,
                                  AccountNumber = accountNumber,
                                  MoneyAccountNumber = moneyAccountNumber,
                                  TotalAmount = total,
                                  TaxId = taxRateId,
                                  ImagePath = imagePath };
            db.Insert(e);
            db.Close();
        }

        public Entry GetEntry(int id)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            Entry e = db.Get<Entry>(id);
            db.Close();
            return e;
        }

        public void UpdateEntry(int id, string date, string description, int accountNumber, int moneyAccounNumber, double total, int taxId, string imagePath)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            Entry e = db.Get<Entry>(id);
            e.Date = date;
            e.Description = description;
            e.AccountNumber = accountNumber;
            e.MoneyAccountNumber = moneyAccounNumber;
            e.TotalAmount = total;
            e.TaxId = taxId;
            e.ImagePath = imagePath;
            db.Update(e);
            db.Close();
        }

        public void DeleteEntry(int id)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            db.Delete<Entry>(id);
            db.Close();
        }

        public TaxRate GetTaxRate(int id)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            TaxRate t = db.Get<TaxRate>(id);
            db.Close();
            return t;
        }

        public Account GetAccount(int id)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            Account a = db.Get<Account>(id);
            db.Close();
            return a;
        }
    }
}