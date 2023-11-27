using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportMidFile.Models;
using System.Net.Mail;
using System.Net;
using System.Web.Security;





namespace TransportMidFile.Controllers
{
    public class TransportController : Controller
    {
        //Search
        
         //Registration Action
         [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration Post Action

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "EmailVarification,ActivationCode")] TransportReg user)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {

                #region //Email is already Exist 
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = AutoGenPassword.Hash(user.Password);
                user.ConfirmPassword = AutoGenPassword.Hash(user.ConfirmPassword); //
                #endregion
                user.EmailVarification = false;

                #region Save to Database
                using (EasyTravelsEntities dc = new EasyTravelsEntities())
                {
                    dc.TransportRegs.Add(user);
                    dc.SaveChanges();

                    //Send Email to User
                    SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + user.EmailID;
                    Status = true;
                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        

        //Verify Account  

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (EasyTravelsEntities dc = new EasyTravelsEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = dc.TransportRegs.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.EmailVarification = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }



        //Login

        [HttpGet]
        public ActionResult Login()
        {
            return View();
            
        }


        //Login Post


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            using (EasyTravelsEntities dc = new EasyTravelsEntities())
            {
                var v = dc.TransportRegs.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                if (v != null)
                {
                    if (!v.EmailVarification)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }

                    if (string.Compare(AutoGenPassword.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                       string encrypted = FormsAuthentication.Encrypt(ticket);
                       var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                       cookie.Expires = DateTime.Now.AddMinutes(timeout);
                       cookie.HttpOnly = true;
                       Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult TSPDashbord()
        {
            return View();
        }



        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Transport");
        }




        public ActionResult ForgotPassword()
        {

            return View();
        }



        //.........................................................................

        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (EasyTravelsEntities dc = new EasyTravelsEntities())
            {
                var v = dc.TransportRegs.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v != null;
            }
        }

       

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/Transport/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("miadabid99@gmail.com", "Tech Pro Baba");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "nnmfgyxmwagdnnxg"; // Replace with actual password
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)

            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            })
                smtp.Send(message);
                
        }







    }
}