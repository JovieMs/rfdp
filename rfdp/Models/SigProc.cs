using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rfdp.Models
{
    public class SigProc
    {
        public string datafile0 { get; set; }
        public string datafile1 { get; set; }
        public double fs { get; set; }
        public double mean { get; set; }
        public double rms { get; set; }
        public double pwr { get; set; }
        public double peak { get; set; }
        public double bottom { get; set; }

    }
}