using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContentTextController : Controller
    {
		// GET: Admin/ContentText/Draft/<ModuleID>
		public ActionResult Draft(Guid id) {
			ContentText ct = new ContentText(id);
			ContentTextDraftViewModel contentText = new ContentTextDraftViewModel(ct);

			return View(contentText);
		}

		// POST: Admin/ContentText/Draft/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Draft(ContentTextDraftViewModel contentTextDraftViewModel) {
			try {
				if (ModelState.IsValid) {
					ContentText contentText = new ContentText(contentTextDraftViewModel);
					contentText.SaveChanges(contentTextDraftViewModel);
				}

				return RedirectToAction("edit", "pages", new { id = contentTextDraftViewModel.PageID });
			} catch {
				return View();
			}
		}

		// GET: Admin/ContentText/Edit/<ModuleID>
		public ActionResult Edit(Guid id) {
			return View(new ContentTextViewModel(new ContentText(id)));
        }
		// POST: Admin/ContentText/Edit/<ModuleID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(ContentTextViewModel contentTextViewModel) {
            try {
				if (ModelState.IsValid) {
					ContentText contentText = new ContentText(contentTextViewModel);
					contentText.SaveChanges(contentTextViewModel);
				}

				return RedirectToAction("edit", "pages", new { id = contentTextViewModel.PageID });
			} catch {
				ContentText ct = new ContentText(contentTextViewModel.ContentID);
				ContentTextViewModel contentText = new ContentTextViewModel(ct);
				return View(contentText);
			}
        }

		// GET: Admin/ContentText/history/<ModuleID>
		public ActionResult History(Guid id) {
			ContentText contentText = new ContentText(id);

			return View(contentText);
		}
		[HttpPost]
		public ActionResult HistoryMakeCurrent(ContentTextHistoryViewModel data) {
			try {
				if (ModelState.IsValid) {
					ContentText contentText = new ContentText(data.ModuleID);
					contentText.MakeCurrent(data.ContentID);
				}

				return RedirectToAction("edit", "contenttext", new { id = data.ModuleID.ToString() });
			} catch {
				ContentText ct = new ContentText(data.ModuleID);
				return View(ct);
			}
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult HistoryMakeDraft(ContentTextHistoryViewModel data) {
			try {
				if (ModelState.IsValid) {
					ContentText contentText = new ContentText(data.ModuleID);
					contentText.MakeDraft(data.ContentID);
				}

				return RedirectToAction("draft", "contenttext", new { id = data.ModuleID });
			} catch {
				ContentText ct = new ContentText(data.ContentID);
				return View(ct);
			}
		}
	}
}
