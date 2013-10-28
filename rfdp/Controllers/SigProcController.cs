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

        public ActionResult Index(string datalist0 = null, string datalist1 = null, double fs = 1000)
        {
            List<SelectListItem> DataSetList0 = new List<SelectListItem>();
            DataSetList0.Add(new SelectListItem { Text = "data0.txt", Value = "data0.txt" });
            DataSetList0.Add(new SelectListItem { Text = "data1.txt", Value = "data1.txt" });
            DataSetList0.Add(new SelectListItem { Text = "data2.txt", Value = "data2.txt" });
            DataSetList0.Add(new SelectListItem { Text = "data3.txt", Value = "data3.txt" });

            List<SelectListItem> DataSetList1 = new List<SelectListItem>();
            DataSetList1.Add(new SelectListItem { Text = "data4.txt", Value = "data4.txt" });
            DataSetList1.Add(new SelectListItem { Text = "data5.txt", Value = "data5.txt" });
            DataSetList1.Add(new SelectListItem { Text = "data6.txt", Value = "data6.txt" });
            DataSetList1.Add(new SelectListItem { Text = "data7.txt", Value = "data7.txt" });

            ViewBag.datalist0 = DataSetList0;
            ViewBag.datalist1 = DataSetList1;

            SigProc sig = new SigProc();
            sig.datafile0 = datalist0;
            sig.datafile1 = datalist1;
            sig.fs = fs;
            sig.ProcessStat();
            return View(sig);
        }

    }
}
