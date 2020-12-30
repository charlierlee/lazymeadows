using System;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentEventsController : Controller
    {
		#region Index
		// GET: Admin/ContentEvent/<ModuleID>
		public ActionResult Index(Guid id) {
			EventSet set = new EventSet(id);

			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(set.Events());
		}
		#endregion

		#region General
		// GET: Admin/ContentEvent/Edit/<EventSerial>
		public ActionResult Edit(int id) {
			Event e = new Event(id);

			PageModule module = new PageModule(e.ModuleID);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(new EventViewModel(e));
		}

		// JSON: Admin/ContentEvent/General/<EventSerial>
		[HttpGet]
		public ActionResult General(int id) {
			Event e = new Event(id);

			return PartialView(new EventViewModel(e));
		}
		// POST: Admin/ContentEvent/General/<EventSerial>
		[HttpPost, ValidateInput(false)]
		public ActionResult General(int id, EventViewModel data) {
			try {
				//if (ModelState.IsValid) {
					Event e = new Event(id);
					e.EventDate = data.EventDate;
					e.EventLink = data.EventLink;
					e.EventLocation = data.EventLocation;
					e.EventPlayback = data.EventPlayback;
					e.EventTimespan = data.EventTimespan;
					e.FullDescription = data.FullDescription;
					e.Headline = data.Headline;
					e.IconFileName = data.IconFileName;
					e.IconFileName2 = data.IconFileName2;
					e.PreEventDescription = data.PreEventDescription;
					e.ShortDescription = data.ShortDescription;

					data.ModuleID = e.ModuleID;
				//}

				return RedirectToAction("Index", new { id = data.ModuleID });
			} catch {
				return View(data);
			}
		}
		#endregion

		// GET: Admin/ContentEvent/Create/<ModuleID>
		public ActionResult Create(Guid id) {
			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			EventViewModel e = new EventViewModel();
			e.EventDate = DateTime.Now;
			return View(e);
		}
		// POST: Admin/ContentEvent/Create/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Create(Guid id, EventViewModel data) {
			EventSet e = new EventSet(id);
			data.ModuleID = id;
			int EventSerial = e.Add(data);

			return RedirectToAction("Index", new { id = data.ModuleID });
		}

		// POST: Admin/ContentEvent/Delete/<EventSerial>
		[HttpPost]
		public ActionResult Delete(int id) {
			Event e = new Event(id);
			e.Delete();

			return Json(true);
		}

        // POST: Admin/ContentEvent/Copy/<EventSerial>
        [HttpPost]
        public ActionResult Copy(int id) {
            Event e = new Event(id);
            int NewEventSerial = e.Copy();

            return Json(NewEventSerial);
        }

        #region SEO
        //AJAX: Admin/Listing/SEO/<ListingSerial>
        public ActionResult SEO(int id) {
			Event e = new Event(id);
			SEO seo = e.seo;
			ViewBag.TypeOG = seo.GetOGOptions(seo.ogType);
			ViewBag.PostURL = "/admin/contentevent/updateseo/" + e.EventSerial;
			ViewBag.ImageBase = "/img/listing";
			SEOViewModel seoViewModel = new SEOViewModel(seo);
			if (string.IsNullOrEmpty(seoViewModel.MetaTitle)) { seoViewModel.MetaTitle = e.Headline; }
			if (string.IsNullOrEmpty(seoViewModel.MetaDescription)) { seoViewModel.MetaDescription = e.ShortDescription; }
			return PartialView("_SEO", seoViewModel);
		}

		// POST: Admin/Listing/UpdateSEO/<ListingSerial>
		[HttpPost]
		public ActionResult UpdateSEO(int id, SEOViewModel data) {
			//TODO: look into image uploads.
			//TODO: Look into sizing images
			//TODO: Show image samples
			Event e = new Event(id);
			SEO seo = new SEO(e.EventID);

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
