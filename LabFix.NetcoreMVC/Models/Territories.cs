using System;
using System.Collections.Generic;

namespace LabFix.NetcoreMVC.Models
{
    public partial class Territories
    {
        public Territories()
        {
            Employeeterritories = new HashSet<Employeeterritories>();
        }

        public string Territoryid { get; set; }
        public char Territorydescription { get; set; }
        public short Regionid { get; set; }

        public Region Region { get; set; }
        public ICollection<Employeeterritories> Employeeterritories { get; set; }
    }
}
