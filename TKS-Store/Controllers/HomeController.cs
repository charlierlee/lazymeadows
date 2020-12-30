using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKS.Models;
using TKS.Models.CMS;
using TKS.Models.realestate;

namespace TKS.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() {
			string qString = "/";

			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;
			}

			ViewBag.Favorites = new TKS.Areas.Admin.Models.GlobalLM().Favorites;
			ViewBag.City = new MLSListings().GetCitySelectList("");
			//ViewBag.Featured = new Listings3().GetFeatured();
			ViewBag.Featured = new MLSListings().GetFeatured();

			return View();
		}
	}
}