using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentGalleryController : Controller
    {
		// GET: Admin/ContentGalleryController/<ModuleID>
		public ActionResult Index(Guid id) {
			Gallery set = new Gallery(id);

			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(set.Slides());
		}

		// GET: Admin/ContentGalleryController/Edit/<SlideSerial>
		public ActionResult Edit(int id)
        {
			GalleryItem slide = new GalleryItem(id);

			PageModule module = new PageModule(slide.ModuleID);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(new GalleryItemModel(slide));
		}
		// POST: Admin/ContentGalleryController/Edit/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(int id, GalleryItemModel data) {
			try {
				if (ModelState.IsValid) {
					GalleryItem slide = new GalleryItem(id);
					slide.PhotoLink = data.PhotoLink;
					slide.PhotoTitle = data.PhotoTitle;
					if (Request.Files["PhotoPath"].HasFile()) {
						string baseDirectory = Server.MapPath("/assets/images/gallery/");
						if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }
						baseDirectory += slide.ModuleID.ToString() + "/";
						if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }

						string FileName = tksUtil.MakeValidFileName(Request.Files["PhotoPath"].FileName).ToLower();

						// Save image file to the retrieved directory
						Request.Files["PhotoPath"].SaveAs(Path.Combine(baseDirectory, FileName));
						slide.PhotoPath = FileName;
					}

					data.ModuleID = slide.ModuleID;
				}

				return RedirectToAction("Index", new { id = data.ModuleID });
			} catch {
				return View(data);
			}
		}

		// GET: Admin/ContentGalleryController/Create/<ModuleID>
		public ActionResult Create(Guid id) {
			PageModule module = new PageModule(id);
			ViewBag.Locale = module.Locale;
			ViewBag.URL = module.VirtualPath;
			ViewBag.PageID = module.PageID;
			ViewBag.PageSectionName = module.PageSectionName;
			ViewBag.ModuleTypeName = module.ModuleTypeName;
			ViewBag.ModuleName = module.ModuleName;
			ViewBag.ModuleID = module.ModuleID;

			return View(new GalleryItemModel());
		}
		// POST: Admin/ContentGalleryController/Create/<ModuleID>
		[HttpPost]
		public ActionResult Create(Guid id, GalleryItemModel data) {
			try {
				Gallery gallery = new Gallery(id);
				if (Request.Files["PhotoPath"].HasFile()) {
					string baseDirectory = Server.MapPath("/assets/images/gallery/");
					if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }
					baseDirectory += id.ToString() + "/";
					if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }

					string FileName = tksUtil.MakeValidFileName(Request.Files["PhotoPath"].FileName).ToLower();

					// Save image file to the retrieved directory
					Request.Files["PhotoPath"].SaveAs(Path.Combine(baseDirectory, FileName));
					data.PhotoPath = FileName;
					data.ModuleID = id;
					data.SortOrder = -1;
				}

				int galleryID = gallery.Add(data);

				return RedirectToAction("Index", new { id = data.ModuleID });
			} catch {
				return View(data);
			}
		}

		// POST: Admin/ContentGalleryController/Delete/<SlideSerial>
		[HttpPost]
		public ActionResult Delete(int id) {
			GalleryItem slide = new GalleryItem(id);
			slide.Delete();

			return Json(true);
		}

		public JsonResult UploadBulk(Guid id) {
			GalleryItemModel data = new GalleryItemModel();
			data.ModuleID = id;

			try {
				if (Request.Files["PhotoPath"].HasFile()) {
					Gallery gallery = new Gallery(id);

					string baseDirectory = Server.MapPath("/assets/images/gallery/");
					if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }
					baseDirectory += id.ToString() + "/";
					if (!Directory.Exists(baseDirectory)) { Directory.CreateDirectory(baseDirectory); }

					for (int i = 0; i < Request.Files.Count; i++) {
						HttpPostedFileBase file = Request.Files[i];
						if (file.ContentLength > 0) {
							string FileName = tksUtil.MakeValidFileName(Request.Files["PhotoPath"].FileName).ToLower();

							// Save image file to the retrieved directory
							Request.Files["PhotoPath"].SaveAs(Path.Combine(baseDirectory, FileName));
							data.PhotoPath = FileName;
							data.SortOrder = -1;

							int galleryID = gallery.Add(data);
						}
					} 					
				}

				return Json(true);
			} catch {
				return Json(false);
			}

			//Gallery gallery = new Gallery(id);

			//string fileName1 = "";
			//string FullName1 = "";
			////HttpFileCollection uploads = Request.Files;
			////for (int fileCount = 0; fileCount < uploads.Count; fileCount++)
			//foreach (var file in Request.Files) {
			//	fileName1 = file.FileName; 
			//}
			////for (int fileCount = 1; fileCount < 6; fileCount++) {
			////	if (fileCount < uploads.Count) {
			////		HttpPostedFile uploadedFile = uploads[fileCount];
			////		fileName1 = Path.GetFileName(uploadedFile.FileName);
			////		if (uploadedFile.ContentLength > 0) {
			////			string[] a = new string[1];
			////			a = uploadedFile.FileName.Split('.');
			////			fileName1 = a.GetValue(0).ToString() +
			////			"." + a.GetValue(1).ToString();
			////			uploadedFile.SaveAs(Server.MapPath
			////			("mobile_image/mob_img/" + fileName1));
			////		}
			////	}
			////} 
			//return View();
		}

		// POST: Admin/ContentGalleryController/Reorder
		[HttpPost]
		public ActionResult Reorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&id[]=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string SlideSerial in arOrder) {
				GalleryItem slide = new GalleryItem(Convert.ToInt32(SlideSerial));
				slide.SortOrder = pos; ;
				pos++;
			}

			return Json("success");
		}
	}
}
