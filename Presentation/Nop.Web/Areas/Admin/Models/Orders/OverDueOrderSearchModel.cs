using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Areas.Admin.Models.Orders
{
    public class OverDueOrderSearchModel : BaseSearchModel
    {
        #region Property List
        [DisplayName("Start Datee")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Contact Person Name")]
        public string ContactPersonName { get; set; }
        #endregion
    }
}
