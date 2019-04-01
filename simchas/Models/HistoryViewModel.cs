using simchas.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simchas.Models
{
    public class HistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public IEnumerable<ContributorHistory> Actions { get; set; }
    }
}