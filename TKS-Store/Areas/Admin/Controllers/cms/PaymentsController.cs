using System;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class PaymentsController : Controller
    {
		#region Index
		// GET: Admin/payments
		public ActionResult Index() {
			PaymentModel data = new PaymentModel();
			GlobalLM payments = new GlobalLM();
			data.DownPayment = payments.DownPayment;
			data.Fixed15Year = payments.Fixed15Year;
			data.Fixed30Year = payments.Fixed30Year;
			data.ARM51 = payments.ARM51;

			return View(data);
		}

		// AJAX: Admin/payments
		[HttpPost, ValidateInput(false)]
		public ActionResult Index(PaymentModel data) {
			GlobalLM payments = new GlobalLM();
			payments.DownPayment = data.DownPayment;
			payments.Fixed15Year = data.Fixed15Year;
			payments.Fixed30Year = data.Fixed30Year;
			payments.ARM51 = data.ARM51;

			return Json(true);
		}
		#endregion
	}
}
