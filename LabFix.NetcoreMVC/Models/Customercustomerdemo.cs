using System;
using System.Collections.Generic;

namespace LabFix.NetcoreMVC.Models
{
    public partial class Customercustomerdemo
    {
        public string Customerid { get; set; }
        public string Customertypeid { get; set; }

        public Customers Customer { get; set; }
        public Customerdemographics Customertype { get; set; }
    }
}
