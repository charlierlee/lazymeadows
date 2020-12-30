using System;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentNewsController : Controller
    {
		#region Index
		// GET: Admin/ContentNews/<ModuleID>
		public ActionResult Index(Guid id) {
			NewsSet set = new NewsSet(id);

			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(set.News());
		}
		#endregion

		#region General
		// GET: Admin/ContentNews/Edit/<NewsSerial>
		public ActionResult Edit(int id)
        {
			News news = new News(id);

			PageModule module = new PageModule(news.ModuleID);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(new NewsModel(news));
		}

		// JSON: Admin/ContentNews/General/<NewsSerial>
		[HttpGet]
		public ActionResult General(int id) {
			News news = new News(id);

			return PartialView(new NewsModel(news));
		}
		// POST: Admin/ContentNews/General/<NewsSerial>
		[HttpPost, ValidateInput(false)]
		public ActionResult General(int id, NewsModel data) {
			try {
				//if (ModelState.IsValid) {
					News news = new News(id);
					news.AttachedArticle = data.AttachedArticle;
					news.Content = data.Content;
					news.DateReleased = data.DateReleased;
					news.HeaderImage = data.HeaderImage;
					news.Headline = data.Headline;
					news.IsFeatured = data.IsFeatured;
					news.IsPublished = data.IsPublished;
					news.LinkedArticle = data.LinkedArticle;
					news.ShortDescription = data.ShortDescription;

					data.ModuleID = news.ModuleID;
				//}

				return RedirectToAction("Index", new { id = data.ModuleID });
			} catch {
				return View(data);
			}
		}
		#endregion

		// GET: Admin/ContentNews/Create/<ModuleID>
		public ActionResult Create(Guid id) {
			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			NewsModel news = new NewsModel();
			news.IsPublished = true;
			news.DateReleased = DateTime.Now;
			return View(news);
		}
		// POST: Admin/ContentNews/Create/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Create(Guid id, NewsModel data) {
			NewsSet news = new NewsSet(id);
			data.ModuleID = id;
			int NewsSerial = news.Add(data);

			return RedirectToAction("Index", new { id = data.ModuleID });
		}

		// POST: Admin/ContentNews/Delete/<NewsSerial>
		[HttpPost]
		public ActionResult Delete(int id) {
			News news = new News(id);
			news.Delete();

			return Json(true);
		}

		#region SEO
		//AJAX: Admin/Listing/SEO/<ListingSerial>
		public ActionResult SEO(int id) {
			News news = new News(id);
			SEO seo = news.seo;
			ViewBag.TypeOG = seo.GetOGOptions(seo.ogType);
			ViewBag.PostURL = "/admin/contentnews/updateseo/" + news.NewsSerial;
			ViewBag.ImageBase = "/img/listing";
			SEOViewModel seoViewModel = new SEOViewModel(seo);
			if (string.IsNullOrEmpty(seoViewModel.MetaTitle)) { seoViewModel.MetaTitle = news.Headline; }
			if (string.IsNullOrEmpty(seoViewModel.MetaDescription)) { seoViewModel.MetaDescription = news.ShortDescription; }
			return PartialView("_SEO", seoViewModel);
		}

		// POST: Admin/Listing/UpdateSEO/<ListingSerial>
		[HttpPost]
		public ActionResult UpdateSEO(int id, SEOViewModel data) {
			//TODO: look into image uploads.
			//TODO: Look into sizing images
			//TODO: Show image samples
			News news = new News(id);
			SEO seo = new SEO(news.NewsID);

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


	}
}
