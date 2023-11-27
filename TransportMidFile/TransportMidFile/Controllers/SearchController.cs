using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportMidFile.Models;

namespace TransportMidFile.Controllers
{
    public class SearchController : Controller
    {
        EasyTravelsEntities db = new EasyTravelsEntities();
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Search(string searching)
        {
            return View(db.Vehiclelists.Where(x => x.VehicleName.Contains(searching) || searching == null).ToList());

        }
    }
}