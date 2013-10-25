using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rfdp.Models
{
    public class SigProc
    {
        public string dataset { get; set; }
        public double fs { get; set; }
        public double mean { get; set; }
        public double rms { get; set; }
    }
}