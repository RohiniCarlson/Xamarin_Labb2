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

        public string DBPath { get; private set; }
        public List<Account> IncomeAccounts 
        {
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                List<Account> incomeAccounts = db.Table<Account>().Where(ic => ic.Type == Account.AccountType.Income).ToList();
                db.Close();
                return incomeAccounts;
            }
        }
        public List<Account> ExpenseAccounts 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                List<Account> expenseAccounts = db.Table<Account>().Where(ic => ic.Type == Account.AccountType.Expense).ToList();
                db.Close();
                return expenseAccounts;
            }
        }
        public List<Account> MoneyAccounts 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                List<Account>  moneyAccounts = db.Table<Account>().Where(ic => ic.Type == Account.AccountType.Money).ToList();
                db.Close();
                return moneyAccounts;
            }
        }
        public List<TaxRate> TaxRates 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                List<TaxRate> taxRates = db.Table<TaxRate>().ToList();
                db.Close();
                return taxRates;
            } 
        }
        public List<Entry> Entries 
        { 
            get
            {
                SQLiteConnection db = new SQLiteConnection(DBPath);
                List<Entry> entries = db.Table<Entry>().ToList();
                db.Close();
                return entries;
            }
        }

        public int GetLastEntryId()
        {
            int entryId = 0;
            SQLiteConnection db = new SQLiteConnection(DBPath);
            if (db.Table<Entry>().Count() > 0)
            {
                entryId = db.Table<Entry>().Last().Id;
            }
            db.Close();
            return entryId;            
        }

        public string GetTaxReport()
        {
            StringBuilder reportText = new StringBuilder();
            double incomeTaxTotal = 0.0;
            double expenseTaxTotal = 0.0;
            double incomeTax = 0.0;
            double expenseTax = 0.0;

            SQLiteConnection db = new SQLiteConnection(DBPath);
            List<Entry> entryList = db.Table<Entry>().ToList();
            if (entryList.Count > 0)
            {
                foreach (Entry e in entryList)
                {
                    reportText.Append(e.Date).Append(" - ").Append(e.Description).Append(":  ");

                    if (GetAccount(e.AccountNumber).Type == Account.AccountType.Expense)
                    {
                        expenseTax = Math.Round(e.TotalAmount-(e.TotalAmount / ((GetTaxRate(e.TaxId).Tax /100) + 1.0)), 2);
                        reportText.Append(" -").Append(expenseTax).Append(" kr").Append("\n");
                        expenseTaxTotal += expenseTax;
                    }
                    else if (GetAccount(e.AccountNumber).Type == Account.AccountType.Income)
                    {
                        //incomeTax = e.TotalAmount * (GetTaxRate(e.TaxId).Tax / 100);
                        incomeTax = Math.Round(e.TotalAmount - (e.TotalAmount / ((GetTaxRate(e.TaxId).Tax /100) + 1.0)), 2);
                        reportText.Append(" ").Append(incomeTax).Append(" kr").Append("\n");
                        incomeTaxTotal += incomeTax;
                    }
                }
                reportText.Append("Totalt att betala: ").Append(incomeTaxTotal - expenseTaxTotal).Append(" kr");
                db.Close();
            }
            else
            {
                reportText.Append("No Entries");
            }
            
            return reportText.ToString();
        }

       public string GetAccountReport()
        {
            StringBuilder reportText = new StringBuilder();
            double accountTotal = 0;

            SQLiteConnection db = new SQLiteConnection(DBPath);
            IEnumerable<Account> accountList = db.Table<Account>();
            IEnumerable<Account> moneyAccountList = accountList.Where(a => a.Type == Account.AccountType.Money);

            foreach (Account a in accountList)
            {
                IEnumerable<Entry> entryList = db.Table<Entry>().Where(e => e.AccountNumber == a.Number);
                if (entryList.Count() > 0)
                {
                    StringBuilder accountDetails = new StringBuilder();
                   
                    foreach (Entry e in entryList)
                    {
                        accountTotal += e.TotalAmount;
                        if (a.Type == Account.AccountType.Income)
                        {
                            accountDetails.Append(e.Date).Append(" - ").Append(e.Description);
                            accountDetails.Append(":  ").Append(e.TotalAmount).Append(" kr").Append("\n");
                        }
                        else if (a.Type == Account.AccountType.Expense)
                        {
                            accountDetails.Append(e.Date).Append(" - ").Append(e.Description);
                            accountDetails.Append(":  -").Append(e.TotalAmount).Append(" kr").Append("\n");
                        }
                    }
                    if (a.Type == Account.AccountType.Income)
                    {
                        reportText.Append("*** ").Append(a.Name).Append(" (").Append(a.Number);
                        reportText.Append(" (total: " + accountTotal + " kr)").Append("\n");
                    }
                    else if (a.Type == Account.AccountType.Expense)
                    {
                        reportText.Append("*** ").Append(a.Name).Append(" (").Append(a.Number);
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
            double incomeTotal;
            double expenseTotal;
            foreach (Account account in moneyAccountList)
            {
                IEnumerable<Entry> entryList = db.Table<Entry>().Where(e => e.AccountNumber == account.Number);
                if (entryList.Count() > 0)
                {
                    StringBuilder accountDetails = new StringBuilder();
                    incomeTotal = 0;
                    expenseTotal = 0;
                    foreach (Entry entry in entryList)
                    {
                        if (accountList.Where(e => e.Number == entry.AccountNumber).First().Type==Account.AccountType.Income)
                        {
                            incomeTotal =+ entry.TotalAmount;
                            accountDetails.Append(entry.Date).Append(" - ").Append(entry.Description);
                            accountDetails.Append(":  ").Append(entry.TotalAmount).Append(" kr").Append("\n");
                        }
                        else
                        {
                            expenseTotal =+ entry.TotalAmount;
                            accountDetails.Append(entry.Date).Append(" - ").Append(entry.Description);
                            accountDetails.Append(":  -").Append(entry.TotalAmount).Append(" kr").Append("\n");
                        }
                    }
                    reportText.Append("*** ").Append(account.Name).Append(" (").Append(account.Number);
                    reportText.Append(" (total: " + (incomeTotal - expenseTotal) + " kr)").Append("\n");                    
                }
                else
                {
                    reportText.Append("*** ").Append(GetAccount(account.Number).Name).Append(" (").Append(account.Number);
                    reportText.Append(" (total: 0 kr").Append("\n").Append("***").Append("\n");
                }
            }
            return reportText.ToString();
        }

       /* public string GetAccountReport()
        {
            StringBuilder reportText = new StringBuilder();        
            StringBuilder expenseAccountsText = new StringBuilder();
            StringBuilder incomeAccountsText = new StringBuilder();
            StringBuilder moneyAccountsText = new StringBuilder();
            
            //double accountTotal = 0;
            double expenseAccountTotal = 0;
            double incomeAccountTotal = 0;
            double moneyAccountTotal = 0;

            SQLiteConnection db = new SQLiteConnection(DBPath);
            IEnumerable<Account> accountList = db.Table<Account>();
            foreach (Account account in accountList)
            {
                IEnumerable<Entry> entryList = db.Table<Entry>().Where(e => e.AccountNumber == account.Number);
                if (account.Type == Account.AccountType.Income)
                {
                    if (entryList.Count() > 0)
                    {
                        foreach (Entry entry in entryList)
                        {
                            incomeAccountTotal += entry.TotalAmount;
                            incomeAccountsText.Append(entry.Date).Append(" - ").Append(entry.Description);
                            incomeAccountsText.Append(":  ").Append(entry.TotalAmount).Append(" kr").Append("\n");
                        }
                        reportText.Append("*** ").Append(GetAccount(account.Number).Name).Append(" (").Append(account.Number);
                        reportText.Append(" (total: -" + incomeAccountTotal + " kr)").Append("\n");
                        reportText.Append(incomeAccountsText);
                        reportText.Append("***").Append("\n");
                        incomeAccountTotal = 0;
                    }
                    else
                    {
                        incomeAccountsText.Append("*** ").Append(GetAccount(account.Number).Name).Append(" (").Append(account.Number);
                        incomeAccountsText.Append(" (total: 0 kr").Append("\n").Append("***").Append("\n");
                    }
                }
                else if (account.Type == Account.AccountType.Expense)
                {

                }
                else
                {

                }
                
                
            }


            /* SQLiteConnection db = new SQLiteConnection(DBPath);
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
                         else if (GetAccount(a.Number).Type == Account.AccountType.Expense)
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
                     else if (GetAccount(a.Number).Type == Account.AccountType.Expense)
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
             }*/
            //return reportText.ToString();
        //}

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
            if (db.Table<Entry>().Count() == 0)
            {
                Entry e = new Entry() { };
            }
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