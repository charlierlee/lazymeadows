using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentVideoController : Controller
    {
		// GET: Admin/ContentVideo/Draft/<ModuleID>
		public ActionResult Draft(Guid id) {
			ContentVideo ct = new ContentVideo(id);
			ContentVideoDraftViewModel contentText = new ContentVideoDraftViewModel(ct);

			return View(contentText);
		}

		// POST: Admin/ContentVideo/Draft/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Draft(ContentVideoDraftViewModel contentTextDraftViewModel) {
			try {
				if (ModelState.IsValid) {
					ContentVideo contentText = new ContentVideo(contentTextDraftViewModel);
					contentText.SaveChanges(contentTextDraftViewModel);
				}

				return RedirectToAction("content", "pages", new { id = contentTextDraftViewModel.PageID });
			} catch {
				return View();
			}
		}

		// GET: Admin/ContentVideo/Edit/<ModuleID>
		public ActionResult Edit(Guid id) {
			return View(new ContentVideoViewModel(new ContentVideo(id)));
        }
		// POST: Admin/ContentVideo/Edit/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(ContentVideoViewModel data) {
            try {
				if (ModelState.IsValid) {
					ContentVideo contentText = new ContentVideo(data);
					contentText.SaveChanges(data);
				}

				return RedirectToAction("edit", "pages", new { id = data.PageID });
            }
            catch
            {
				ContentVideo ct = new ContentVideo(data.ContentID);
				ContentVideoViewModel contentText = new ContentVideoViewModel(ct);
				return View(contentText);
            }
        }

		// GET: Admin/ContentVideo/history/<ModuleID>
		public ActionResult History(Guid id) {
			ContentVideo contentText = new ContentVideo(id);

			return View(contentText);
		}
		[HttpPost]
		public ActionResult HistoryMakeCurrent(ContentVideoHistoryViewModel data) {
			try {
				if (ModelState.IsValid) {
					ContentVideo contentText = new ContentVideo(data.ModuleID);
					contentText.MakeCurrent(data.ContentID);
				}

				return RedirectToAction("edit", "contenttext", new { id = data.ModuleID.ToString() });
			} catch {
				ContentVideo ct = new ContentVideo(data.ModuleID);
				return View(ct);
			}
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult HistoryMakeDraft(ContentVideoHistoryViewModel data) {
			try {
				if (ModelState.IsValid) {
					ContentVideo contentText = new ContentVideo(data.ModuleID);
					contentText.MakeDraft(data.ContentID);
				}

				return RedirectToAction("draft", "contenttext", new { id = data.ModuleID });
			} catch {
				ContentVideo ct = new ContentVideo(data.ContentID);
				return View(ct);
			}
		}
	}
}
