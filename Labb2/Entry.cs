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
    public class Entry
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; private set; }
        public string Date { get; set;}
        public string Description { get; set; }
        public int AccountNumber { get; set; }
        public int MoneyAccountNumber { get; set; }
        public double TotalAmount { get; set; }
        public int TaxId { get; set; }

        public override string ToString()
        {
            return Date + " " + Description + " " + TotalAmount + " kr";
        }
    }
}