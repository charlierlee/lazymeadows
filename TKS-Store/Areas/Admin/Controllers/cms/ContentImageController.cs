using System;
using System.Web.Mvc;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentImageController : Controller
    {
		// GET: Admin/ContentImage/Edit/<ModuleID>
		public ActionResult Edit(Guid id)
        {
			ContentImage img = new ContentImage(id);
			ImageViewModel imageViewModel = new ImageViewModel(img);

			return View(imageViewModel);
		}

		// POST: Admin/ContentImage/Edit/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(ImageViewModel imageViewModel) {
			try {
				if (ModelState.IsValid) {
					ContentImage img = new ContentImage(imageViewModel.ModuleID);
					img.SaveChanges(imageViewModel);
				}

				return RedirectToAction("edit", "pages", new { id = imageViewModel.PageID });
			} catch {
				return View(imageViewModel);
			}
		}
    }
}
