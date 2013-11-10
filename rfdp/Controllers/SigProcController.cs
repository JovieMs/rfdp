using rfdp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace rfdp.Controllers
{
    public class SigProcController : Controller
    {
        //
        // GET: /SigProc/

        public ActionResult Index(string datalist0 = null, string datalist1 = null, double fs = 1000, int SelChan = 1, string SelPanel = "tdom_anlaysis")
        {
            List<SelectListItem> DataSetList0 = new List<SelectListItem>();
            DataSetList0.Add(new SelectListItem { Text = "sine wave", Value = "sin.txt" });
            DataSetList0.Add(new SelectListItem { Text = "square wave", Value = "data1.txt" });
            DataSetList0.Add(new SelectListItem { Text = "eye pattern", Value = "data2.txt" });
            DataSetList0.Add(new SelectListItem { Text = "scatter plot chan 0", Value = "scatter0.txt" });

            List<SelectListItem> DataSetList1 = new List<SelectListItem>();
            DataSetList1.Add(new SelectListItem { Text = "cos wave", Value = "cos.txt" });
            DataSetList1.Add(new SelectListItem { Text = "data5", Value = "data5.txt" });
            DataSetList1.Add(new SelectListItem { Text = "data6", Value = "data6.txt" });
            DataSetList1.Add(new SelectListItem { Text = "scatter plot chan 1", Value = "scatter1.txt" });

            ViewBag.datalist0 = DataSetList0;
            ViewBag.datalist1 = DataSetList1;

            SigProc sig = new SigProc();
            sig.datafile[0] = datalist0;
            sig.datafile[1] = datalist1;
            sig.fs = fs;
            sig.ProcessStat();
            sig.mode = SelPanel;
            sig.chan = SelChan;
            return View(sig);
        }

    }
}
