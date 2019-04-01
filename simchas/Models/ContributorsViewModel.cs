using simchas.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simchas.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public IEnumerable<SimchaContributor> Contributors { get; set; }
    }
}