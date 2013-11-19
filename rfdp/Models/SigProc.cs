using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;


namespace rfdp.Models
{
    public enum AnalysisMode { TimeDomain, FrequencyDomain, Scatter, Histogram, EyePattern };
    public enum ChannelMode { Single = 1, Dual };

    public class SigProc
    {
        public string[] datafile { get; set; }
        public double fs { get; set; }
        public double[] mean { get; set; }
        public double[] rms { get; set; }
        public double[] pwr { get; set; }
        public double[] max { get; set; }
        public double[] min { get; set; }
        public string msg { get; set; }
        public int[] len { get; set; }
        public double duration { get; set; }
        public string[] jasondata { get; set; }
        public AnalysisMode analysisMode { get; set; }
        public ChannelMode channelMode { get; set; }
        public int chan { get; set; }
        public int bin_no { get; set; }
        public int[] x_bins { get; set; }
        public int[] y_bins { get; set; }
        public List<SigData> sig;
        
        private double[] x;
        private double[] y;



        public SigProc()
        {
            init();
            datafile = new string[2] { "", "" };
        }

        private void calc(int chan, double data)
        {
            mean[chan] += data;
            max[chan] = max[chan] > data ? max[chan] : data;
            min[chan] = min[chan] < data ? min[chan] : data;
            rms[chan] += data * data;
            pwr[chan] = rms[chan];
            jasondata[chan] = (len[chan] == 0) ? "" : jasondata[chan] + ", ";
            jasondata[chan] += "{\"x\": " + Convert.ToString(len[chan] * 1000 / fs) + ", \"y\": " + Convert.ToString(data) + "}";
            if (chan == 0)
            {
                sig.Add(new SigData { x = data, y = 0 });
            } 
            else 
            {
                sig[len[chan]].y = data;
            }
            len[chan]++;
        }

        private void update(int chan)
        {
            if (len[chan] != 0)
            {
                mean[chan] /= len[chan];
                rms[chan] = Math.Sqrt(rms[chan] / len[chan]);
                pwr[chan] /= len[chan];
                duration = len[chan] / fs;
                jasondata[chan] = "[ " + jasondata[chan] + " ]";
            }
            jasondata[2] = JsonConvert.SerializeObject(sig);
            if (chan == 1)
            {
                ProcessFFT();
            }
        }


        private void init()
        {
            mean = new double[2] {0, 0};
            rms = new double[2] { 0, 0 };
            pwr = new double[2] { 0, 0 };
            max = new double[2] { 0, 0 };
            min = new double[2] { 0, 0 };
            msg = "";
            len = new int[2] {0, 0};
            jasondata = new string[6] { "[]", "[]", "[]", "[]", "[]", "[]" };
            sig = new List<SigData>();
            bin_no = 20;
        }


        public void ProcessStat()
        {
            init();
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    string appdata = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Data") + "\\" + datafile[i];
                    using (StreamReader sr = new StreamReader(appdata))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line == string.Empty) { continue; }
                            double data = Convert.ToDouble(line);
                            calc(i, data);
                        }
                        update(i);
                    }
                }
                catch (Exception e)
                {
                    Debug.Write("The file could not be read:");
                    Debug.Write(e.Message);
                }
                
            }
            

            if (analysisMode == AnalysisMode.Histogram)
            {
                ProcessHist();
                ProcessFFT();
            }
                
        }
        public void ProcessHist()
        {
            x_bins = new int[bin_no];
            y_bins = new int[bin_no];
            double x_min = this.min[0];
            double y_min = this.min[1];
            double x_max = this.max[0];
            double y_max = this.max[1];
            double x_step = (x_max - x_min) / bin_no;
            double y_step = (y_max - y_min) / bin_no;

            for (int i = 0; i < bin_no; i++)
            {
                x_bins[i] = 0;
                y_bins[i] = 0;
            }

            foreach (SigData d in sig)
            {
                for (int i = 1; i <= bin_no; i++)
                {
                    if (d.x <= x_min + x_step * i)
                    {
                        x_bins[i-1]++;
                        break;
                    }
                }

                for (int i = 1; i <= bin_no; i++)
                {
                    if (d.y <= y_min + y_step * i)
                    {
                        y_bins[i-1]++;
                        break;
                    }
                }
            }
            jasondata[3] = "[{\"values\": [";
            for (int i = 0; i < bin_no-1; i++)
            {
                jasondata[3] += "{\"label\":" + (x_min +x_step*i).ToString("F2") + ", \"value\":" + x_bins[i].ToString("F0") + "},";
            }
            jasondata[3] += "{\"label\":" + (x_min + x_step * (bin_no - 1)).ToString("F2") + ", \"value\":" + x_bins[bin_no - 1].ToString("F0") + "}";
            jasondata[3] += "]}]";
        }

        private void ProcessFFT()
        {
            x = new double[len[0]];
            int i = 0;
            foreach (SigData d in sig)
            {
                x[i] = d.x;
                i++;
            }

            alglib.complex[] f;
            alglib.fftr1d(x, out f);
            double tmp;
            jasondata[4] = "";
            foreach (alglib.complex item in f)
            {
                tmp = alglib.math.abscomplex(item);
                if (jasondata[4] == "")
                {
                    jasondata[4] += "{\"x\": " + Convert.ToString(len[0] * 1000 / fs) + ", \"y\": " + Convert.ToString(tmp) + "}";

                }
                else
                {
                    jasondata[4] += ",{\"x\": " + Convert.ToString(len[0] * 1000 / fs) + ", \"y\": " + Convert.ToString(tmp) + "}";
                }
                
            }
            jasondata[4] = "[" + jasondata[4] + "]";
        }
    }


}
