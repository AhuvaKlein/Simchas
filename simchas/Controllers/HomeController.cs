using simchas.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using simchas.Models;

namespace simchas.Controllers
{
    public class HomeController : Controller
    {
        Manager mgr = new Manager(Properties.Settings.Default.ConStr);

        public ActionResult Index()
        {
            SimchaViewModel vm = new SimchaViewModel();
            vm.Simchas = mgr.GetSimchas();
            vm.Contributors = mgr.GetTotalContributors();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha simcha)
        {
            mgr.AddSimcha(simcha);
            return Redirect("/home/index");
        }

        public ActionResult Contributions(int id)
        {
            IEnumerable<Contributor> contributors = mgr.GetContributors();
            IEnumerable<SimchaContributor> contributed = mgr.GetContributorsThatContributed(id);
            IEnumerable<SimchaContributor> contributorsMapped = contributors.Select(c => new SimchaContributor
            {
                ContributorId = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                AlwaysJoin = c.AlwaysJoin,
                Amount = 5,
                Contributed = false,
                Balance = mgr.GetContributorBalance(c.Id)
            }).ToList();

            foreach (SimchaContributor sc in contributorsMapped)
            {
                foreach (SimchaContributor s in contributed)
                {
                    if (s.ContributorId == sc.ContributorId)
                    {
                        sc.Contributed = true;
                        sc.Amount = s.Amount;
                        break;
                    }
                }
            }

            ContributionsViewModel vm = new ContributionsViewModel();
            vm.Simcha = mgr.GetSimcha(id);
            vm.Contributors = contributorsMapped;

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateContributions(IEnumerable<SimchaContributor> contributors, int id)
        {
            mgr.DeleteContributions(id);
            mgr.UpdateContributions(contributors, id);
            return Redirect($"/home/index");
        }

    }
}