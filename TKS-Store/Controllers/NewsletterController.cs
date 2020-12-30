using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;

namespace TKS.Controllers
{
    public class NewsletterController : Controller
    {
		// Post: Newsletter/StreamSend
		[HttpPost]
        public ActionResult Streamsend() {
			if (Request.Form["subscriber"] != null) {
				string subscriber = Request.Form["subscriber"].ToString();
				if (TKS.Models.Streamsend.EmailExist(subscriber) == 0) {
					TKS.Models.Streamsend.CreateNewSubscriber(subscriber);
				}
			}
 
			return Redirect(Global.NewsletterRedirect);
        }

		// Post: Newsletter/MailChimp
		[HttpPost]
		public ActionResult MailChimp() {
			if (Request.Form["subscriber"] != null) {
				string subscriber = Request.Form["subscriber"].ToString();
				//MailChimpManager mc = new MailChimpManager(Global.NewsletterMailChimpAPIKey);

				////  Create the email parameter
				//EmailParameter email = new EmailParameter() { Email = subscriber };

				//EmailParameter results = mc.Subscribe("27a2e037e8", email);
			}

			return Redirect(Global.NewsletterRedirect);
		}
	}
}