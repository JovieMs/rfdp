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

        public ActionResult Index(string DataSetLists = null, double fs = 1000)
        {
            List<SelectListItem> DataSetList = new List<SelectListItem>();
            DataSetList.Add(new SelectListItem { Text = "data0.txt", Value = "data0.txt" });
            DataSetList.Add(new SelectListItem { Text = "data1.txt", Value = "data1.txt" });
            DataSetList.Add(new SelectListItem { Text = "data2.txt", Value = "data2.txt" });
            DataSetList.Add(new SelectListItem { Text = "data3.txt", Value = "data3.txt" });

            ViewBag.DataSetLists = DataSetList;

            SigProc sig = new SigProc();
            sig.dataset = DataSetLists;
            sig.fs = fs;
            sig.mean = 23;
            sig.rms = 110;
            return View(sig);
        }

    }
}
