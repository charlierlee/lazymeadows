using System;
using System.IO;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.LazyMeadows;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class SoldController : Controller
    {
		#region Index
		// GET: Admin/favorites
		public ActionResult Index() {
			return View(new SoldSet().GetList());
		}

		//AJAX: Admin/Category/Reorder/<CategoryID>
		[HttpPost]
		public ActionResult Reorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&row[]=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string ID in arOrder) {
				new Sold(Convert.ToInt32(ID)).SortOrder = pos;
				pos++;
			}

			return Json("success");
		}

		[HttpPost]
		public ActionResult Delete(int id) {
			new Sold(id).Delete();

			return Json(true);
		}
		#endregion

		#region Edit
		public ActionResult Edit(int id) {
			return View(new Sold(id));
		}
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(int id, SoldViewModel data) {
            Sold sold = new Sold(id);
            sold.Content = data.Content;

            string originalFile = Server.MapPath(@"/assets/images/sold/" + id.ToString() + "/");
            Directory.CreateDirectory(originalFile);

            if(Request.Files["Thumbnail"].HasFile()) {
                Request.Files["Thumbnail"].SaveAs(originalFile + Request.Files["Thumbnail"].FileName);
                sold.Thumbnail = Request.Files["Thumbnail"].FileName;
            }
            if(Request.Files["Interior"].HasFile()) {
                Request.Files["Interior"].SaveAs(originalFile + Request.Files["Interior"].FileName);
                sold.Interior = Request.Files["Interior"].FileName;
            }
            if(Request.Files["Exterior"].HasFile()) {
                Request.Files["Exterior"].SaveAs(originalFile + Request.Files["Exterior"].FileName);
                sold.Exterior = Request.Files["Exterior"].FileName;
            }

            return Redirect("/admin/sold");
		}
		#endregion

		#region Create
		public ActionResult Create() {
			return View(new SoldViewModel());
		}
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(SoldViewModel data) {
            if(Request.Files["Thumbnail"].HasFile()) {
                data.Thumbnail = Request.Files["Thumbnail"].FileName;
            }
            if(Request.Files["Interior"].HasFile()) {
                data.Interior = Request.Files["Interior"].FileName;
            }
            if(Request.Files["Exterior"].HasFile()) {
                data.Exterior = Request.Files["Exterior"].FileName;
            }
            int ItemSerial = new SoldSet().Add(data);

            string originalFile = Server.MapPath(@"/assets/images/sold/" + ItemSerial.ToString() + "/");
            Directory.CreateDirectory(originalFile);

			if (Request.Files["Thumbnail"].HasFile()) {
                Request.Files["Thumbnail"].SaveAs(originalFile + Request.Files["Thumbnail"].FileName);
            }
            if(Request.Files["Interior"].HasFile()) {
                Request.Files["Interior"].SaveAs(originalFile + Request.Files["Interior"].FileName);
            }
            if(Request.Files["Exterior"].HasFile()) {
                Request.Files["Exterior"].SaveAs(originalFile + Request.Files["Exterior"].FileName);
            }

            return Redirect("/admin/sold");
		}
		#endregion
	}
}
