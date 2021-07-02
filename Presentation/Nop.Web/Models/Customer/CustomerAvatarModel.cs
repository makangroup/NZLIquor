using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer
{
    public partial class CustomerAvatarModel : BaseNopModel
    {
        public string AvatarUrl { get; set; }

        public string Avatar2Url { get; set; }
    }
}