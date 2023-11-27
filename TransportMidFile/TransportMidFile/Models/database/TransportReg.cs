using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TransportMidFile.Models
{

    [MetadataType(typeof(Userdata))]
   
    public partial class TransportReg
    {
        public string ConfirmPassword { get; set; }
    }

    public class Userdata
    {
       

        [Display(Name = "Company Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Company Name required")]
        public string CompanyName { get; set; }


        [Display(Name = "Email ID")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email ID required")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Phone is required")]
        [DataType(DataType.PhoneNumber)]
        [MinLength(11, ErrorMessage = "Minimum 11 Digit required")]
        public string Phone { get; set; }


        [Display(Name = "Input Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Type is required")]
        public string Type { get; set; }



        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minimum 6 characters required")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password and password do not match")]
        public string ConfirmPassword { get; set; }

    }

   
}