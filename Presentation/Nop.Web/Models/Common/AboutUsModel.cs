using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Common
{
    public partial class AboutUsModel : BaseNopModel
    {
       

        public bool DisplayCaptcha { get; set; }
    }
}