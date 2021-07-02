using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.CorporateCustomer
{
    public partial class CorporateCustomer : BaseEntity
    {        
        public int Customer_Id { get; set; }
        public int CustomerType { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string DriversLicenceNo { get; set; }
        public DateTime? DateIncorporated { get; set; }
        public string CompanyNo { get; set; }
        public string NatureOfBusiness { get; set; }
        public string GSTNo { get; set; }
        public decimal? PaidUpCapital { get; set; }
        public decimal? EstimatedMonthlyPurchase { get; set; }
        public decimal? CreditLimtRequired { get; set; }
        public int PrincipalPlaceOfBusiness { get; set; }
        public string LiquoredFullNameLicensee { get; set; }
        public string LiquoredCompanyNo { get; set; }
        public string LiquoredLicenceNo { get; set; }
        public DateTime LiquoredLicenceExpiryDate { get; set; }
        public string LiquoredTobaccoLicenceNo { get; set; }
        public Nullable<DateTime> LiquoredTobaccoLicenceExpiryDate { get; set; }
        public string LiquoredStreet { get; set; }
        public string LiquoredPhone { get; set; }
        public string LiquoredSuburb { get; set; }
        public string LiquoredPostCode { get; set; }
        public string LiquoredFacsimilieNumber { get; set; }
        public string LiquoredLandlordName { get; set; }
        public DateTime LiquoredDateLeasedFrom { get; set; }
        public DateTime LiquoredDateLeasedTo { get; set; }
        public string LiquoredPreviousLicenceDetails { get; set; }
        public int AccountTerms { get; set; }
        public bool PurchaseOrderRequired{ get;set;}
        public bool AccountsToBeEmailed { get; set; }
        public string AccountsEmailAddress { get; set; }
        public string AccountsContact { get; set; }
        public string PhoneNo { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string AccountNo { get; set; }
        public string CreditFormFile { get; set; }
        public string LiquoreLicenseFile { get; set; }

        /// <summary>
        /// Get or set the credit limit of the corporate customer.
        /// </summary>
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Get or set the credit period of the customer
        /// </summary>
        public int CreditPeriod { get; set; }
    }
}
