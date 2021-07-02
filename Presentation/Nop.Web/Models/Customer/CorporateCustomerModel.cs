using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Models.Customer
{
    public class CorporateCustomerModel
    {

        public int Customer_Id { get; set; }
        [Display(Name = "Customer Type")]
        [Required]
        public int CustomerTypeEnum { get; set; }

        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Drivers Licence No")]
        [StringLength(15, ErrorMessage = "*")]
        public string DriversLicenceNo { get; set; }
        [Display(Name = "Date Incorporated")]
        [Required]
        public DateTime? DateIncorporated { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "*")]
        [Display(Name = "Company No")]

        public string CompanyNo { get; set; }
        [Display(Name = "Nature Of Business")]
        [Required]
        public string NatureOfBusiness { get; set; }
        [Display(Name = "GST No")]
        [Required]
        [StringLength(15, ErrorMessage = "*")]
        public string GSTNo { get; set; }
        [Display(Name = "Paid Up Capital")]
        [Required]
        public decimal? PaidUpCapital { get; set; }
        [Display(Name = "Estimated Monthly Purchase")]
        [Required]
        public decimal? EstimatedMonthlyPurchase { get; set; }
        [Display(Name = "Credit Limit Required")]
        [Required]
        public decimal? CreditLimtRequired { get; set; }
        [Display(Name = "Principal Place Of Business")]
        [Required]
        public int PrincipalPlaceOfBusinessEnum { get; set; }
        [Display(Name = "Full Name Licensee")]
        [Required]
        public string LiquoredFullNameLicensee { get; set; }
        [Display(Name = "Company No")]
        [StringLength(15, ErrorMessage = "*")]
        [Required]
        public string LiquoredCompanyNo { get; set; }
        [Display(Name = "Liquored Licence No")]
        [Required]
        [StringLength(15, ErrorMessage = "*")]
        public string LiquoredLicenceNo { get; set; }
        [Display(Name = "Liquored Licence Expiry Date")]
        [Required]
        public DateTime LiquoredLicenceExpiryDate { get; set; }

        [Display(Name = "Tobacco Licence No")]
        [StringLength(15, ErrorMessage = "*")]
        public string LiquoredTobaccoLicenceNo { get; set; }
        [Display(Name = "Tobacco Licence Expiry Date")]

        public DateTime LiquoredTobaccoLicenceExpiryDate { get; set; }
        [Display(Name = "Street")]
        [Required]
        public string LiquoredStreet { get; set; }
        [Display(Name = "Phone")]
        [Required]
        public string LiquoredPhone { get; set; }
        [Display(Name = "Suburb")]
        [Required]
        public string LiquoredSuburb { get; set; }
        [Display(Name = "Postcode")]
        [Required]
        
        public string LiquoredPostCode { get; set; }
        [Display(Name = "Facsimilie Number")]
        [StringLength(15, ErrorMessage = "*")]
        public string LiquoredFacsimilieNumber { get; set; }
        [Display(Name = "Landlord Name")]
        public string LiquoredLandlordName { get; set; }
        [Display(Name = "Date Leased From")]
        public DateTime? LiquoredDateLeasedFrom { get; set; }
        [Display(Name = "Liquored Date Leased To")]
        public DateTime? LiquoredDateLeasedTo { get; set; }
        [Display(Name = "Previous Licence Details")]
        public string LiquoredPreviousLicenceDetails { get; set; }

        [Display(Name = "Account Terms")]
        [Required]
        public int AccountTermsEnum { get; set; }
        [Display(Name = "Purchase Order Required?")]
        [Required]
        public bool PurchaseOrderRequired { get; set; }
        [Display(Name = "Accounts To Be Emailed?")]
        [Required]
        public bool AccountsToBeEmailed { get; set; }
        [Required]
        [Display(Name = "Accounts Email Address")]
        public string AccountsEmailAddress { get; set; }
        [Required]
        [Display(Name = "Accounts Contact")]
        public string AccountsContact { get; set; }
        [Required]
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "Bank")]
        public string Bank { get; set; }
        [Required]
        [Display(Name = "Branch")]

        public string Branch { get; set; }
        [Display(Name = "Account No")]
        [Required]
        public string AccountNo { get; set; }
        public string CorporateReferences { get; set; }
        public string CorporateOwners { get; set; }

    }


    public enum CustomerTypeEnum
    {
        BarsAndResturants = 1,
        SportsClubs = 2,
        PrivateFunctions = 3,
        FriendsAndFamily = 4
    }

    public enum PrincipalPlaceOfBusinessEnum
    {
        Rented = 1,
        Owened = 2,
        Mortgaged = 3
    }

    public enum AccountTermsEnum
    {
        Twenty_Days = 1,
        COD = 2,
        Other = 3
    }
}
