using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Models.Customer
{
    public class CorporateCustomerCompanyOwnersModel
    {
        public int CorporateCustomer_Id { get; set; }
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full Name can't be empty.")]
        
        public string FullName { get; set; }
        [Display(Name = "Date Of Birth")]
        [Required(ErrorMessage = "Date Of Birth can't be empty.")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Private Address")]
        [Required(ErrorMessage = "Private Address can't be empty.")]
        public string PrivateAddress { get; set; }
        [Display(Name = "Post Code")]
        [Required(ErrorMessage = "Post code can't be empty.")]
        public string Postcode { get; set; }
        [Display(Name = "Driving Licence No")]
        [Required(ErrorMessage = "Driving Licence No can't be empty.")]
        public string DriversLicenceNo { get; set; }
        [Display(Name = "Phone No")]
        [Required(ErrorMessage = "Phone No can't be empty.")]
        public string PhoneNo { get; set; }
        [Display(Name = "Mobile No")]
        [Required(ErrorMessage = "Mobile No can't be empty.")]
        public string MobileNo { get; set; }
    }
}
