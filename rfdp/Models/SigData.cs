using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;

namespace rfdp.Models
{
    public class SigData
    {
        public double[] Data { get; set; }
        public double SamplingFrequency { get; set; }
        public int DataBinNo { get; set; }
        public Dictionary<string, double> Statistic{ get; set; }
        public int Length { get; set; }

        private double Mean;
        private double RMS;
        private double Power;
        private double Max;
        private double Min;
        private double Duration;
        private alglib.complex[] ComplexSpectrum;
        private int[] DataBins;
        private List<double> DataList;

        public SigData()
        {
            Statistic = new Dictionary<string, double>();
            Statistic.Add("Duration", 0);
            Statistic.Add("Mean", 0);
            Statistic.Add("RMS", 0);
            Statistic.Add("Power", 0);
            Statistic.Add("Max", 0);
            Statistic.Add("Min", 0);

            Mean = 0;
            Max = 0;
            Min = 0;
            RMS = 0;
            Power = 0;

            DataBinNo = 40;
            SamplingFrequency = 0;
        }

        public void DoStatistic() {
            foreach (double data in Data)
            {
                Mean += data;
                Max = Max < data ? data : Max;
                Min = Min > data ? data : Min;
                RMS += data * data;
            }
            Mean /= Length;
            Power = RMS / Length;
            RMS = Math.Sqrt(RMS / Length);
            Duration = Length / SamplingFrequency;

            Statistic["Duration"] = Duration;
            Statistic["Mean"] = Mean;
            Statistic["RMS"] = RMS;
            Statistic["Power"] = Power;
            Statistic["Max"] = Max;
            Statistic["Min"] = Min;
        }

        public void DoFFT()
        {
            alglib.fftr1d(Data, out ComplexSpectrum, 128);
        }

        public void DoHistogram()
        {
            DataBins = new int[DataBinNo];
            double step = (Max - Min) / DataBinNo;

            for (int i = 0; i < DataBinNo; i++)
            {
                DataBins[i] = 0;
            }

            foreach (double d in Data)
            {
                for (int i = 1; i <= DataBinNo; i++)
                {
                    if (d <= Min + step * i)
                    {
                        DataBins[i - 1]++;
                        break;
                    }
                }
            }
        }

        public void ReadData(string filePath)
        {
            Length = 0;
            DataList = new List<double>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == string.Empty) { continue; }
                        DataList.Add(Convert.ToDouble(line));
                        Length++;
                    }
                }

                Data = new double[Length];
                int i = 0;
                foreach (double data in DataList)
                {
                    Data[i++] = data;
                }

            }
            catch (Exception e)
            {
                Debug.Write("The file could not be read:");
                Debug.Write(e.Message);
            }


        }

        public string ConvertDataToJson()
        {
            string json = "[]";
            if (Length != 0)
            {
                json = "{\"x\": " + Convert.ToString(0) + ", \"y\": " + Convert.ToString(Data[0]) + "}";
                for (int i = 1; i < Length; i++)
                {
                    json += ",{\"x\": " + Convert.ToString(i * 1000 / SamplingFrequency) + ", \"y\": " + Convert.ToString(Data[i]) + "}";
                }
                json = "[" + json + "]";
            }
            
            
            return json;
        }
        public string ConvertBinsToJson()
        {
            string json = "[]";
            if (Length != 0)
            {
                double step = (Max - Min) / DataBinNo;
                json = "[{\"values\": [";
                for (int i = 0; i < DataBinNo - 1; i++)
                {
                    json += "{\"label\":" + (Min + step * i).ToString("F2") + ", \"value\":" + DataBins[i].ToString("F0") + "},";
                }
                json += "{\"label\":" + (Min + step * (DataBinNo - 1)).ToString("F2") + ", \"value\":" + DataBins[DataBinNo - 1].ToString("F0") + "}";
                json += "]}]";
            }
            
            return json;
        }
        public string ConvertSpectrumToJson()
        {
            string json = "[]";
            if (Length != 0)
            {
                json = "";
                double tmp;
                double freq_interval = SamplingFrequency / (2 * Length);
                for (int i = 0; i < (ComplexSpectrum.Length / 2); i++)
                {
                    tmp = alglib.math.abscomplex(ComplexSpectrum[i]);
                    if (json == "")
                    {
                        json = "{\"x\": " + Convert.ToString(freq_interval * i) + ", \"y\": " + Convert.ToString(tmp) + "}";

                    }
                    else
                    {
                        json += ",{\"x\": " + Convert.ToString(freq_interval * i) + ", \"y\": " + Convert.ToString(tmp) + "}";
                    }
                }
                json = "[" + json + "]";
            }
            
            return json;
        }

        public string ConvertSpectrumBinToJson()
        {
            string json = "[]";
            if (Length != 0)
            {
                json = "[{\"values\": [";
                double tmp;
                double freq_interval = SamplingFrequency / (2 * Length);
                for (int i = 0; i < (ComplexSpectrum.Length / 2); i++)
                {
                    tmp = alglib.math.abscomplex(ComplexSpectrum[i]);
                    if (i == 0)
                    {
                        json += "{\"label\": " + Convert.ToString(freq_interval * i) + ", \"value\": " + Convert.ToString(tmp) + "}";

                    }
                    else
                    {
                        json += ",{\"label\": " + Convert.ToString(freq_interval * i) + ", \"value\": " + Convert.ToString(tmp) + "}";
                    }

                }
                json += "]}]";
            }

            return json;
        }
    }
}