using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;
using TKS.Areas.Admin.Models.LazyMeadows;

namespace TKS.Areas.Admin.Controllers.LazyMeadows {
	[Authorize(Roles = "Admin")]
	public class SocialPagesController : Controller {
		// GET: Admin/SocialPages
		public ActionResult Index() {
			return View(new SocialPages());
		}

		#region News
		// GET: Admin/SocialPages/editnews/<id>
		public ActionResult EditNews(string id) {
			SocialPage page = new SocialPage(id);
			if (string.IsNullOrEmpty(page.TownFormatted)) { return RedirectToAction("index"); }

			return View(page);
		}
		// POST: Admin/SocialPages/editnews/<id>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult EditNews(string id, SocialPage data) {
			SocialPage page = new SocialPage(id);
			if (string.IsNullOrEmpty(page.TownFormatted)) { return RedirectToAction("index"); }

			page.Col1 = data.Col1;

			return RedirectToAction("index"); ;
		}
		#endregion

		#region Dining
		// GET: Admin/SocialPages/editdining/<id>
		public ActionResult EditDining(string id) {
			SocialPage page = new SocialPage(id);
			if (string.IsNullOrEmpty(page.TownFormatted)) { return RedirectToAction("index"); }

			return View(page);
		}
		// POST: Admin/SocialPages/editdining/<id>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult EditDining(string id, SocialPage data) {
			SocialPage page = new SocialPage(id);
			if (string.IsNullOrEmpty(page.TownFormatted)) { return RedirectToAction("index"); }

			page.Col2 = data.Col2;

			return RedirectToAction("index"); ;
		}
		#endregion

		#region Lodging
		// GET: Admin/SocialPages/editlodging/<id>
		public ActionResult EditLodging(string id) {
			SocialPage page = new SocialPage(id);
			if (string.IsNullOrEmpty(page.TownFormatted)) { return RedirectToAction("index"); }

			return View(page);
		}
		// POST: Admin/SocialPages/editlodging/<id>
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult EditLodging(string id, SocialPage data) {
			SocialPage page = new SocialPage(id);
			if (string.IsNullOrEmpty(page.TownFormatted)) { return RedirectToAction("index"); }

			page.Col3 = data.Col3;

			return RedirectToAction("index"); ;
		}
		#endregion

		#region CuratePages
		// GET: Admin/Pages/module/<PageID>
		public ActionResult CuratePages(string id) {
			SocialPage page = new SocialPage(id);
			ViewBag.Town = page.Town;
			ViewBag.TownFormatted = page.TownFormatted;

			return View(page.GetPages());
		}

		// POST: Admin/Pages/PageDelete
		[HttpPost]
		public ActionResult PageDelete(string id, string SocialPageSerial) {
			SocialPage page = new SocialPage(id);
			page.RemoveCuratedPage(Convert.ToInt32(SocialPageSerial));

			return Json(true);
		}

		// POST: Admin/Pages/PageReorder
		[HttpPost]
		public ActionResult PageReorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&row[]=" }, StringSplitOptions.RemoveEmptyEntries);
			SocialPage page = new SocialPage();
			foreach (string SocialPageSerial in arOrder) {
				page.UpdateSortOrder(Convert.ToInt32(SocialPageSerial), pos);
				pos++;
			}

			return Json("success");
		}
		#endregion

		// GET: Admin/SocialPages/AddCuratedPage/town
		public ActionResult AddCuratedPage(string id) {
			List<SelectListItem> typeList = new List<SelectListItem>();
			typeList.Add(new SelectListItem { Value = "Facebook", Text = "Facebook", Selected = false });
			typeList.Add(new SelectListItem { Value = "InstagramEmbed", Text = "InstagramEmbed", Selected = false });
            typeList.Add(new SelectListItem { Value = "InstagramTag", Text = "InstagramTag", Selected = false });
            typeList.Add(new SelectListItem { Value = "InstagramUser", Text = "InstagramUser", Selected = false });
            typeList.Add(new SelectListItem { Value = "PinterestBoard", Text = "PinterestBoard", Selected = false });
			typeList.Add(new SelectListItem { Value = "PinterestPin", Text = "PinterestPin", Selected = false });
			ViewBag.PageType = typeList;

			SocialSite site = new SocialSite();
			site.Town = id;
			return View(site);
		}
		// POST: Admin/Pages/AddCuratedPage
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult AddCuratedPage(string id, SocialSite data) {
			SocialPage page = new SocialPage(id);
			data.Town = id;
			page.AddCuratedPage(data);

			return Redirect("/admin/socialpages/curatepages/" + id);
		}


		// GET: Admin/SocialPages/AddCuratedPage/town
		public ActionResult EditCuratedPage(int id) {
			SocialPage page = new SocialPage();
			SocialSite site = page.GetPage(id);

			List<SelectListItem> typeList = new List<SelectListItem>();
			typeList.Add(new SelectListItem { Value = "Facebook", Text = "Facebook", Selected = (site.PageType == "Facebook") });
			typeList.Add(new SelectListItem { Value = "InstagramEmbed", Text = "InstagramEmbed", Selected = (site.PageType == "InstagramEmbed") });
			typeList.Add(new SelectListItem { Value = "InstagramTag", Text = "InstagramTag", Selected = (site.PageType == "InstagramTag") });
            typeList.Add(new SelectListItem { Value = "InstagramUser", Text = "InstagramUser", Selected = (site.PageType == "InstagramUser") });
            typeList.Add(new SelectListItem { Value = "PinterestBoard", Text = "PinterestBoard", Selected = (site.PageType == "PinterestBoard") });
			typeList.Add(new SelectListItem { Value = "PinterestPin", Text = "PinterestPin", Selected = (site.PageType == "PinterestPin") });
			ViewBag.PageType = typeList;

			return View(site);
		}
		// POST: Admin/Pages/AddCuratedPage
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult EditCuratedPage(int id, SocialSite data) {
			SocialPage page = new SocialPage();
			page.EditCuratedPage(data);

			return Redirect("/admin/socialpages/curatepages/" + data.Town);
		}
	}
}
