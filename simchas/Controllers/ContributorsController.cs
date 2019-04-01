using simchas.data;
using simchas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace simchas.Controllers
{
    public class ContributorsController : Controller
    {
        Manager mgr = new Manager(Properties.Settings.Default.ConStr);

        // GET: Contributors
        public ActionResult DisplayContributors()
        {
            DisplayContributorViewModel vm = new DisplayContributorViewModel();
            IEnumerable<Contributor> c = mgr.GetContributors();
            vm.Contributors = c;
            vm.TotalBalance = mgr.GetTotalBalance();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddContributor(Contributor c, Deposit deposit)
        {
            deposit.ContributorId = mgr.AddContributor(c);
            deposit.Date = c.DateJoined;
            mgr.AddDeposit(deposit);

            return Redirect("/contributors/displayContributors");
        }

        [HttpPost]
        public ActionResult AddDeposit(Deposit d)
        {
            mgr.AddDeposit(d);
            return Redirect("/contributors/displayContributors");
        }

        public ActionResult History(int id)
        {
            HistoryViewModel vm = new HistoryViewModel();

            vm.Contributor = mgr.GetContributor(id);
            IEnumerable<ContributorHistory> deposits = mgr.GetDepositHistory(id).ToList();
            IEnumerable<ContributorHistory> contributions = mgr.GetContributionHistory(id).ToList();
            List<ContributorHistory> actions = new List<ContributorHistory>();
            foreach(ContributorHistory c in deposits)
            {
                actions.Add(c);
            }
            foreach (ContributorHistory c in contributions)
            {
                actions.Add(c);
            }

            vm.Actions = actions.OrderBy(c => c.Date); ;
            return View(vm);
        }

        public ActionResult EditContributor(Contributor c)
        {
            mgr.EditContributor(c);
            return Redirect("/contributors/displayContributors");
        }
    }
}

