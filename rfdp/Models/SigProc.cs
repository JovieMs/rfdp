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
        public string[] DataFile { get; set; }
        public double SamplingFrequency { get; set; }
        public double[] Mean { get; set; }
        public double[] RMS { get; set; }
        public double[] Power { get; set; }
        public double[] Max { get; set; }
        public double[] Min { get; set; }
        public int[] Length { get; set; }
        public double Duration { get; set; }
        public string[] JsonData { get; set; }
        public AnalysisMode analysisMode { get; set; }
        public ChannelMode channelMode { get; set; }
        public int ActiveChan { get; set; }

        public SigData[] Signal;


        public SigProc()
        {
            DataFile = new string[2] { "", "" };
            Signal = new SigData[2];
            Signal[0] = new SigData();
            Signal[1] = new SigData();
            SamplingFrequency = 0;
            Mean = new double[2] { 0, 0 };
            RMS = new double[2] { 0, 0 };
            Power = new double[2] { 0, 0 };
            Max = new double[2] { 0, 0 };
            Min = new double[2] { 0, 0 };
            Length = new int[2] { 0, 0 };
            JsonData = new string[5] { "[]", "[]", "[]", "[]", "[]" };
        }

        public void ProcessStat()
        {
            string appdata;
            Signal[0].SamplingFrequency = SamplingFrequency;
            Signal[1].SamplingFrequency = SamplingFrequency;

            if (analysisMode == AnalysisMode.Histogram ||
                analysisMode == AnalysisMode.FrequencyDomain)
            {
                appdata = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Data") + "\\" + DataFile[ActiveChan];
                if (File.Exists(appdata))
                {
                    Signal[ActiveChan].ReadData(appdata);
                    Signal[ActiveChan].DoStatistic();
                }

            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    appdata = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Data") + "\\" + DataFile[i];
                    if (File.Exists(appdata))
                    {
                        Signal[i].ReadData(appdata);
                        Signal[i].DoStatistic();
                    }
                }
            }


            if (analysisMode == AnalysisMode.Histogram)
            {
                Signal[ActiveChan].DoHistogram();
            }
            else if (analysisMode == AnalysisMode.FrequencyDomain)
            {
                Signal[ActiveChan].DoFFT();
            }
        }

        private String ConvertDualSignalToJson()
        {
            string json = "[]";
            int len = Math.Min(Signal[0].Length, Signal[1].Length);
            if (len > 0)
            {
                json = "{\"x\":" + Signal[0].Data[0].ToString("F2") + ", \"y\":" + Signal[1].Data[0].ToString("F2") + "} ";

                for (int i = 1; i < len; i++)
                {
                    json += ",{\"x\":" + Signal[0].Data[i].ToString("F2") + ", \"y\":" + Signal[1].Data[i].ToString("F2") + "} ";
                }
                json = "[" + json + "]";
            }

            return json;
        }

        public void ReturnStat()
        {
            for (int i = 0; i < 2; i++)
            {
                this.Mean[i] = (Signal[i].Statistic)["Mean"];
                this.Power[i] = Signal[i].Statistic["Power"];
                this.RMS[i] = Signal[i].Statistic["RMS"];
                this.Max[i] = Signal[i].Statistic["Max"];
                this.Min[i] = Signal[i].Statistic["Min"];
                this.Length[i] = Signal[i].Length;
            }

            if (analysisMode == AnalysisMode.TimeDomain)
            {
                JsonData[0] = Signal[0].ConvertDataToJson();
                JsonData[1] = Signal[1].ConvertDataToJson();
            }
            else if (analysisMode == AnalysisMode.Scatter)
            {
                JsonData[2] = ConvertDualSignalToJson();
            }
            else if (analysisMode == AnalysisMode.Histogram)
            {
                JsonData[3] = Signal[ActiveChan].ConvertBinsToJson();
            }
            else if (analysisMode == AnalysisMode.FrequencyDomain)
            {
                JsonData[4] = Signal[ActiveChan].ConvertSpectrumToJson();
            }
        }

    }


}

