using System;
using System.Collections.Generic;

namespace LabFix.NetcoreMVC.Models
{
    public partial class Customerdemographics
    {
        public Customerdemographics()
        {
            Customercustomerdemo = new HashSet<Customercustomerdemo>();
        }

        public string Customertypeid { get; set; }
        public string Customerdesc { get; set; }

        public ICollection<Customercustomerdemo> Customercustomerdemo { get; set; }
    }
}
