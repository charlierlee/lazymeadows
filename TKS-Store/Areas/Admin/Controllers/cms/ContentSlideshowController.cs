using System;
using System.IO;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentSlideshowController : Controller
    {
		// GET: Admin/ContentSlideshow/<ModuleID>
		public ActionResult Index(Guid id) {
			ContentSlideshow set = new ContentSlideshow(id);

			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(set.AllSlides());
		}

		// GET: Admin/ContentSlideshow/Edit/<SlideSerial>
		public ActionResult Edit(int id)
        {
			ContentSlide slide = new ContentSlide(id);

			PageModule module = new PageModule(slide.ModuleID);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(new ContentSlideModel(slide));
		}
		// POST: Admin/ContentSlideshow/Edit/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(int id, ContentSlideModel data) {
			try {
				if (ModelState.IsValid) {
					ContentSlide slide = new ContentSlide(id);
					slide.SlideLink = data.SlideLink;
					slide.SlideTitle = data.SlideTitle;
					if (Request.Files["SlideImageFile"].HasFile()) {
						string baseDirectory = Server.MapPath("/assets/images/slideshow/");
						if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }
						baseDirectory += slide.ModuleID.ToString() + "/";
						if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }

						string FileName = Request.Files["SlideImageFile"].FileName;

						// Save image file to the retrieved directory
						Request.Files["SlideImageFile"].SaveAs(Path.Combine(baseDirectory, FileName));
						slide.SlideImageFile = FileName;
					}

					data.ModuleID = slide.ModuleID;
				}

				return RedirectToAction("Index", new { id = data.ModuleID });
			} catch {
				return View(data);
			}
		}

		// GET: Admin/ContentSlideshow/Create/<ModuleID>
		public ActionResult Create(Guid id) {
			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(new ContentSlideModel());
		}
		// POST: Admin/ContentSlideshow/Create/<ModuleID>
		[HttpPost]
		public ActionResult Create(Guid id, ContentSlideModel data) {
			ContentSlideshow show = new ContentSlideshow(id);

			if (Request.Files["SlideImageFile"].HasFile()) {
				string baseDirectory = Server.MapPath("/assets/images/slideshow/");
				if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }
				baseDirectory += id.ToString() + "/";
				if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }

				string FileName = tksUtil.MakeValidFileName(Request.Files["SlideImageFile"].FileName).ToLower();

				// Save image file to the retrieved directory
				Request.Files["SlideImageFile"].SaveAs(Path.Combine(baseDirectory, FileName));

				data.SlideImageFile = FileName;
				data.ModuleID = id;
			}

			int SlideSerial = show.Add(data);

			return RedirectToAction("Index", new { id = data.ModuleID });
		}

		// POST: Admin/ContentSlideshow/Delete/<SlideSerial>
		[HttpPost]
		public ActionResult Delete(int id) {
			ContentSlide slide = new ContentSlide(id);
			slide.Delete();

			return Json(true);
		}

		// POST: Admin/ContentSlideshow/Reorder
		[HttpPost]
		public ActionResult Reorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&id[]=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string SlideSerial in arOrder) {
				ContentSlide slide = new ContentSlide(Convert.ToInt32(SlideSerial));
				slide.SortOrder = pos; ;
				pos++;
			}

			return Json("success");
		}
	}
}
