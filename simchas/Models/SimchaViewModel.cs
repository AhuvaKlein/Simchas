using simchas.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simchas.Models
{
    public class SimchaViewModel
    {
        public IEnumerable<Simcha> Simchas { get; set; }
        public int Contributors { get; set; }
    }
}