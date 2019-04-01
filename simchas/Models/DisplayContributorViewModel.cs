using simchas.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simchas.Models
{
    public class DisplayContributorViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public decimal TotalBalance { get; set; }
    }
}