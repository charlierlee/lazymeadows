using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKS.Models;

namespace TKS.Controllers
{
    public class MyAccountController : Controller
    {
        // GET: MyAccount
        public ActionResult Index()
        {
			return View();
        }

		public ActionResult Email() {
			TKS.Models.User user = new TKS.Models.User();
			ViewBag.Email = user.Email;

			return View();
		}

		public ActionResult Password() {
			return View();
		}

		public ActionResult Address() {
			TKS.Models.User user = new TKS.Models.User();

			return View(new Addresses(user.UserKey).GetList());
		}
		//public ActionResult AddressCreate(string id) {
		//	ViewBag.UserName = id;

		//	return PartialView(new AddressViewModel());
		//}
	}
}