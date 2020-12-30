using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class PagesController : Controller {
		// GET: Admin/Pages
		public ActionResult Index() {
			List<ContentPageViewModel> pages = ContentPages.Pages();
			return View(pages);
		}

		// GET: Admin/Pages/edit/<id>
		public ActionResult Edit(Guid id) {
			ContentPage page = new ContentPage(id);
			ViewBag.PageID = id.ToString();
			ViewBag.Locale = page.Locale;
			ViewBag.VirtualPath = page.VirtualPath;

			return View();
		}

		// JSON: Admin/Pages/content/<id>
		public ActionResult Content(Guid id) {
			ContentPage page = new ContentPage(id);
			ViewBag.PageID = id.ToString();
			ViewBag.Locale = page.Locale;
			ViewBag.VirtualPath = page.VirtualPath;

			return PartialView(page.Sections);
		}

		// GET: Admin/Pages/Create
		public ActionResult Create() {
			ViewBag.PageTypeID = new ContentPageTypes().GetSelectList(Guid.Empty);
			ViewBag.Locale = new Locales().GetSelectList();

			return View(new ContentPageViewModel());
		}
		// POST: Admin/Pages/Create
		[HttpPost]
		public ActionResult Create(ContentPageViewModel contentPage) {
			Guid PageID = ContentPages.Add(contentPage);

			return Redirect("/admin/pages/edit/" + PageID.ToString() + "#tabs-4");
		}

		#region Structure
		// GET: Admin/Pages/structure/<PageID>
		public ActionResult Structure(Guid id) {
			ContentPageViewModel contentPage = new ContentPageViewModel(new ContentPage(id));

			ViewBag.PageTypeID = new ContentPageTypes().GetSelectList(contentPage.PageTypeID);
			ViewBag.Locale = new Locales().GetSelectList(contentPage.Locale);

			return PartialView(contentPage);
		}
		// POST: Admin/Pages/structure/<PageID>
		[HttpPost]
		public ActionResult Structure(Guid id, ContentPageViewModel data) {
			ContentPage contentPage = new ContentPage(id);

			contentPage.IncludeInSitemap = data.IncludeInSitemap;
			contentPage.PageTypeID = data.PageTypeID;
			contentPage.VirtualPath = data.VirtualPath;
			contentPage.Locale = data.Locale;

			return Json(true);
		}

		// POST: Admin/Pages/Delete/<id>
		[HttpPost]
		public ActionResult Delete(Guid id) {
			ContentPage contentPage = new ContentPage(id);
			contentPage.Delete();

			return Json(true);
		}
		#endregion

		#region SEO
		//AJAX: Admin/Blog/SEO/<PostSerial>
		public ActionResult SEO(Guid id) {
			ContentPage page = new ContentPage(id);
			SEO seo = page.seo;
			ViewBag.TypeOG = seo.GetOGOptions(seo.ogType);
			ViewBag.PostURL = "/admin/pages/seo";
			ViewBag.ImageBase = "/blog";
			SEOViewModel seoViewModel = new SEOViewModel(seo);

			return PartialView("_SEO", seoViewModel);
		}
		// JSON: Admin/Blog/SEO/<PostSerial>
		[HttpPost]
		public JsonResult SEO(SEOViewModel data) {
			//TODO: look into image uploads.
			//TODO: Look into sizing images
			//TODO: Show image samples
			ContentPage page = new ContentPage(data.EntityID);
			SEO seo = page.seo;

			seo.gDescription = data.gDescription;
			seo.gImage = data.gImage;
			seo.gName = data.gName;
			seo.MetaDescription = data.MetaDescription;
			seo.MetaKeywords = data.MetaKeywords;
			seo.MetaTitle = data.MetaTitle;
			seo.ogDescription = data.ogDescription;
			seo.ogImage = data.ogImage;
			seo.ogTitle = data.ogTitle;
			seo.ogType = data.ogType;
			seo.twitterDescription = data.twitterDescription;
			seo.twitterImage = data.twitterImage;
			seo.twitterTitle = data.twitterTitle;

			return Json(true);
		}
		#endregion

		#region Module
		// GET: Admin/Pages/module/<PageID>
		public ActionResult Module(Guid id) {
			ContentPage contentPage = new ContentPage(id);

			ViewBag.PageID = id;
			ViewBag.PageSectionID = new Sections(contentPage.PageTypeID).GetSelectList(Guid.Empty);
			ViewBag.ModuleTypeID = new Modules().GetStandardModuleSelectList(Guid.Empty);

			ViewBag.AssignedModules = new PageModules(id).GetPageModules();

			return PartialView(contentPage.Sections);
		}
		// POST: Admin/Pages/module/<PageID>
		[HttpPost]
		public ActionResult Module(Guid id, ContentPageViewModel data) {
			ContentPage contentPage = new ContentPage(id);

			contentPage.IncludeInSitemap = data.IncludeInSitemap;
			contentPage.PageTypeID = data.PageTypeID;
			contentPage.VirtualPath = data.VirtualPath;
			contentPage.Locale = data.Locale;

			return Json(true);
		}

		//POST: Admin/Pages/ModuleCreate
		[HttpPost]
		public ActionResult ModuleCreate(ModuleViewModel data) {
			ContentPage cp = new ContentPage(data.PageID);
			cp.AddModule(data);

			return Json(true);
		}

		// POST: Admin/Pages/ModuleDelete
		[HttpPost]
		public ActionResult ModuleDelete(Guid id, Guid ModuleID) {
			ContentPage cp = new ContentPage(id);
			cp.RemoveModule(ModuleID);

			return Json(true);
		}

		// POST: Admin/Pages/ModuleReorder
		[HttpPost]
		public ActionResult ModuleReorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&row[]=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string ModuleID in arOrder) {
				PageModule module = new PageModule(new Guid(ModuleID));
				module.SortOrder = pos;
				pos++;
			}

			return Json("success");
		}
		#endregion

	}
}
