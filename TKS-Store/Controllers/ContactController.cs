using System;
using System.Net;
using System.Web.Mvc;
using Newtonsoft.Json;
using TKS.Models.CMS;

namespace TKS.Controllers {
    public class ContactController : Controller
    {
		// GET: Contact
		public ActionResult Index() {
			string section = "contact-us";
			string qString = "/" + section;

			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;
			}

			return View(new ContactUs());
		}

		// POST: Contact
		[HttpPost]
		public ActionResult DoContact(ContactUs data) {
			if (TKS.Areas.Admin.Models.Global.reCAPTCHASecretKey.Length > 0) {
				var response = Request["g-recaptcha-response"];
				var client = new WebClient();
				var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", TKS.Areas.Admin.Models.Global.reCAPTCHASecretKey, response));

				var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);

				if (captchaResponse.Success.ToLower() == "true") {
					data.Send();

					return Redirect("/contact-tx");
				} else {
					//when response is false check for the error message
					if (captchaResponse.ErrorCodes.Count <= 0) return View(data);

					var error = captchaResponse.ErrorCodes[0].ToLower();
					switch (error) {
						case ("missing-input-secret"): ViewBag.Message = "The secret parameter is missing."; break;
						case ("invalid-input-secret"): ViewBag.Message = "The secret parameter is invalid or malformed."; break;
						case ("missing-input-response"): ViewBag.Message = "Please check the box to show you are human."; break;
						case ("invalid-input-response"): ViewBag.Message = "Please check the box to show you are human."; break;
						default: ViewBag.Message = "Please check the box to show you are human"; break;
					}
                    return Redirect("/contact-tx");
                    //return View(data);
				}
			} else {
				data.Send();

				return Redirect("/contact-tx");
			}
		}
	}
}