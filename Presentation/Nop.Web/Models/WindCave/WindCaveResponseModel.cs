using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Models.WindCave
{
    public class WindCaveResponseModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class WindCaveResponseListModel
    {
        public List<WindCaveResponseModel> WindCaveResponses { get; set; }
    }
}
