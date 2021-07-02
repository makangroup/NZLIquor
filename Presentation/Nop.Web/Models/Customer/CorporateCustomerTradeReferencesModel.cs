using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Models.Customer
{
    public class CorporateCustomerTradeReferencesModel
    {

        public int CorporateCustomer_Id { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name can't be empty.")]
        public string Name { get; set; }
        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address can't be empty.")]
        public string Address { get; set; }
        [Display(Name = "Contact")]
        [Required(ErrorMessage = "Contact can't be empty.")]
        public string PhoneFaxEmail { get; set; }
    }
}
