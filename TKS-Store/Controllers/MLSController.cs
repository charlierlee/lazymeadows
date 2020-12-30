using System;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Mvc;
using TKS.Models.realestate;

namespace TKS.Controllers
{
    public class MLSController : Controller
    {
        // GET: MLS
        public ActionResult Index(string todo = "") {
			string access_token = "fded13cccfd9e14b47b0fb1818c67954";//"a57e5dfd4ce945cc52206e83bf534718";
			string msg = "Started " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "<br />";
			if (todo == "go") {
				Server.ScriptTimeout = 300;
				MLSManager mgr = new MLSManager();
				mgr.PreUpdate();
				MLS mls = new MLS();
				int offset = 0;
				int recordsRes = 0;
				int recordsLand = 0;
				int recordsComm = 0;
				int deleted = 0;
				System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

				using (HttpClient _httpClient = new HttpClient()) {
					_httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

					//Residential
					HttpResponseMessage result = _httpClient.GetAsync(new Uri("https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token="+ access_token + "&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&PropertyType=Residential")).Result;
					if (result.IsSuccessStatusCode) {
						mls = Newtonsoft.Json.JsonConvert.DeserializeObject<MLS>(result.Content.ReadAsStringAsync().Result);
						recordsRes = mls.total;
					}
					foreach (var prop in mls.bundle) { mgr.SaveProp(prop); }
					while (offset < recordsRes) {
						offset += 200;
						result = _httpClient.GetAsync(new Uri("https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=" + access_token + "&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&offset=" + offset.ToString() + "&PropertyType=Residential")).Result;
						if (result.IsSuccessStatusCode) { mls = Newtonsoft.Json.JsonConvert.DeserializeObject<MLS>(result.Content.ReadAsStringAsync().Result); }
						foreach (var prop in mls.bundle) { mgr.SaveProp(prop); }
					}

					//Land
					offset = 0;
					result = _httpClient.GetAsync(new Uri("https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=" + access_token + "&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&PropertyType=Land")).Result;
					if (result.IsSuccessStatusCode) {
						mls = Newtonsoft.Json.JsonConvert.DeserializeObject<MLS>(result.Content.ReadAsStringAsync().Result);
						recordsLand = mls.total;
					}
					foreach (var prop in mls.bundle) { mgr.SaveProp(prop); }
					while (offset < recordsLand) {
						offset += 200;
						result = _httpClient.GetAsync(new Uri("https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=" + access_token + "&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&offset=" + offset.ToString() + "&PropertyType=Land")).Result;
						if (result.IsSuccessStatusCode) { mls = Newtonsoft.Json.JsonConvert.DeserializeObject<MLS>(result.Content.ReadAsStringAsync().Result); }
						foreach (var prop in mls.bundle) { mgr.SaveProp(prop); }
					}

					//Commercial
					offset = 0;
					result = _httpClient.GetAsync(new Uri("https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=" + access_token + "&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&PropertyType=Commercial Sale")).Result;
					if (result.IsSuccessStatusCode) {
						mls = Newtonsoft.Json.JsonConvert.DeserializeObject<MLS>(result.Content.ReadAsStringAsync().Result);
						recordsComm = mls.total;
					}
					foreach (var prop in mls.bundle) { mgr.SaveProp(prop); }
					while (offset < recordsComm) {
						offset += 200;
						result = _httpClient.GetAsync(new Uri("https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=" + access_token + "&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&offset=" + offset.ToString() + "&PropertyType=Commercial Sale")).Result;
						if (result.IsSuccessStatusCode) { mls = Newtonsoft.Json.JsonConvert.DeserializeObject<MLS>(result.Content.ReadAsStringAsync().Result); }
						foreach (var prop in mls.bundle) { mgr.SaveProp(prop); }
					}
				}

				deleted = mgr.PostUpdate();
				Gaiocorp.Geocoding.BingGeocoder.UpdateGeoCoding3();

				msg += recordsRes.ToString() + " residential records completed.<br />" +
					recordsLand.ToString() + " land records completed.<br />" +
					recordsComm.ToString() + " commercial records completed.<br />" +
					deleted.ToString() + " deleted<br />" +
					DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
				MailMessage mm = new MailMessage();
				mm.Body = msg;
				mm.IsBodyHtml = true;
				mm.From = new MailAddress("web@thecatskillfarms.com");
				mm.Subject = "Lazy Meadows Upload";
				mm.To.Add("tom@eerieglow.com");
				SmtpClient smtp = new SmtpClient();
				smtp.Send(mm);
			}
			return Content(msg);
        }
    }
}