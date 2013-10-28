using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public double max { get; set; }
        public double min { get; set; }
        public string msg { get; set; }
        public int len { get; set; }
        public double duration { get; set; }
        public string jasondata { get; set; }

        public SigProc()
        {
            init();
        }

        private void calc(double data)
        {
            len++;
            mean += data;
            max = max > data ? max : data;
            min = min < data ? min : data;
            rms += data * data;
            pwr = rms;
            jasondata = (len == 1) ? "" : jasondata + ", ";
            Debug.WriteLine(fs * len);
            jasondata += "{\"x\": " + Convert.ToString(fs * len) + ", \"y\": " + Convert.ToString(data) + "}";
            
        }

        private void update()
        {
            if (len != 0)
            {
                mean /= len;
                rms = Math.Sqrt(rms / len);
                pwr /= len;
                duration = len / fs;
                jasondata = "[ " + jasondata + " ]";
            }
        }

        private void init()
        {
            mean = 0;
            rms = 0;
            pwr = 0;
            max = 0;
            min = 0;
            msg = "";
            len = 0;
            jasondata = "";
        }


        public void ProcessStat()
        {
            try
            {
                string appdata = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Data") + "\\" + datafile0;
                using (StreamReader sr = new StreamReader(appdata))
                {
                    init();
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == string.Empty) { continue; }
                        double data = Convert.ToDouble(line);
                        calc(data);
                    }
                    update();
                }
            }
            catch (Exception e)
            {
                Debug.Write("The file could not be read:");
                Debug.Write(e.Message);
            }

        }

    }


}