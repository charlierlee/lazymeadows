using System;
using System.IO;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;
using TKS.Areas.Admin.Models.LazyMeadows;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class FavoritesController : Controller
    {
		#region Index
		// GET: Admin/favorites
		public ActionResult Index() {
			return View(new Favorites().GetList());
		}

		// AJAX: Admin/favorites
		[HttpPost, ValidateInput(false)]
		public ActionResult Index(string Favorites) {
			new GlobalLM().Favorites = Favorites;

			return Json(true);
		}

		//AJAX: Admin/Category/ProdReorder/<CategoryID>
		[HttpPost]
		public ActionResult Reorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&row[]=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string FavoriteID in arOrder) {
				new Favorite(Convert.ToInt32(FavoriteID)).SortOrder = pos;
				pos++;
			}

			return Json("success");
		}

		[HttpPost]
		public ActionResult Delete(int id) {
			new Favorite(id).Delete();

			return Json(true);
		}
		#endregion

		#region Edit
		public ActionResult Edit(int id) {
			return View(new Favorite(id));
		}
		[HttpPost]
		public ActionResult Edit(int id, Favorite data) {
			Favorite fav = new Favorite(id);
			if (string.IsNullOrEmpty(data.MLS)) {
				fav.Description = data.Description;
				fav.Link = data.Link;
				fav.Price = data.Price;
				fav.Town = data.Town;

				if (Request.Files["fuImage"].HasFile()) {
					string filename = Request.Files["fuImage"].FileName;
					string originalFile = Server.MapPath(@"/img/favorites/" + id.ToString() + "/");
					Directory.CreateDirectory(originalFile);
					Request.Files["fuImage"].SaveAs(originalFile + filename);

					fav.Photo = filename;
				}
			} else {
				fav.MLS = data.MLS;
			}

			return Redirect("/admin/favorites");
		}
		#endregion

		#region Create
		public ActionResult CreateCustom() {
			return View(new Favorite());
		}
		[HttpPost]
		public ActionResult CreateCustom(Favorite data) {
			string filename = "";
			if (Request.Files["fuImage"].HasFile()) {
				filename = Request.Files["fuImage"].FileName;
				data.Photo = filename;
			}

			int FavoriteID = new Favorites().Add(data);

			if (!string.IsNullOrEmpty(filename)) {
				string originalFile = Server.MapPath(@"/img/favorites/" + FavoriteID.ToString() + "/");
				Directory.CreateDirectory(originalFile);
				Request.Files["fuImage"].SaveAs(originalFile + filename);
			}

			return Redirect("/admin/favorites");
		}

		public ActionResult CreateMLS() {
			return View(new Favorite());
		}
		[HttpPost]
		public ActionResult CreateMLS(Favorite data) {
			int FavoriteID = new Favorites().Add(data);

			return Redirect("/admin/favorites");
		}
		#endregion
	}
}
