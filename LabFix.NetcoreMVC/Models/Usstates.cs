using System;
using System.Collections.Generic;

namespace LabFix.NetcoreMVC.Models
{
    public partial class Usstates
    {
        public short Stateid { get; set; }
        public string Statename { get; set; }
        public string Stateabbr { get; set; }
        public string Stateregion { get; set; }
    }
}
