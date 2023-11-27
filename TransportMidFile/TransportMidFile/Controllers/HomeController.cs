using System;
using System.Linq;
using System.Web.Mvc;
using TransportMidFile.Models;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Web.Security;


using System.Data.Entity.Validation;
using System.Configuration;

namespace TransportMidFile.Controllers
{
    public class HomeController : Controller
    {



        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public ActionResult RegisterVehicle()
        {

            return View();
        }
        public ActionResult RegisterVehicle(Vehiclelist s)
        {
            if (ModelState.IsValid)
            {
                EasyTravelsEntities db = new EasyTravelsEntities();
                db.Vehiclelists.Add(s);
                db.SaveChanges();
                return RedirectToAction("RegisterVehicle", "Home");

            }
            return View();
        }




        public ActionResult VehicleShow()
        {
            var db = new EasyTravelsEntities();
            var products = db.Vehiclelists.ToList();
            if (products != null)
            {
                return View(products);
            }
            return View();

        }



        [HttpGet]
        public ActionResult VehicleEdit(int id)
        {
            var db = new EasyTravelsEntities();
            var product = (from p in db.Vehiclelists where p.VehicleID == id select p).SingleOrDefault();
            if (product != null)
            {
                return View(product);
            }
            return View();



        }
        [HttpPost]
        public ActionResult VehicleEdit(Vehiclelist s1)
        {

            EasyTravelsEntities db = new EasyTravelsEntities();
            var user = (from p in db.Vehiclelists where p.VehicleID == s1.VehicleID select p).FirstOrDefault();

            user.VehicleName = s1.VehicleName;
            user.Type = s1.Type;
            user.StartingPoint = s1.StartingPoint;
            user.FinishingPoint = s1.FinishingPoint;
            user.TotalSeat = s1.TotalSeat;
            user.Price = s1.Price;
            user.Offer = s1.Offer;
            user.ExtraInfo = s1.ExtraInfo;


            try
            {
                db.SaveChanges();
                return RedirectToAction("VehicleShow");
            }
            catch (DbEntityValidationException s)
            {
                return View(s1);
            }


        }
        public ActionResult delete(int id)
        {
            var db = new EasyTravelsEntities();
            var user = (from p in db.Vehiclelists where p.VehicleID == id select p).SingleOrDefault();
            db.Vehiclelists.Remove(user);
            db.SaveChanges();
            return RedirectToAction("VehicleShow");
        }


        public ActionResult InqIndex()
        {
            EasyTravelsEntities db = new EasyTravelsEntities();
            var data = db.Inqs.ToList();
            return View(data);
        }

        [HttpGet]
        public ActionResult Inq()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Inq(Inq h)
        {
            if (ModelState.IsValid)
            {
                EasyTravelsEntities db = new EasyTravelsEntities();
                db.Inqs.Add(h);
                db.SaveChanges();
                return RedirectToAction("InqIndex");

            }
            return View();
        }

        
       
    }
}