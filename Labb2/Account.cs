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
    public class Account
    {
        public enum AccountType
        {
            Income,
            Expense,
            Money
        }
        [PrimaryKey]
        public int Number { get; set; }
        public string Name { get; set; }
        public Account.AccountType Type { get; set; }

        public override string ToString()
        {
            return Number + " " + Name;
        }
    }
}