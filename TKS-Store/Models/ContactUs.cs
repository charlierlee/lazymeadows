using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Newtonsoft.Json;
using TKS.Areas.Admin.Models;

namespace TKS.Models.CMS {
	public class ContactUs {
		public string Name { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Comments { get; set; }
		public string Referrer { get; set; }

		public void Send() {
			//---------------- Sending Email
			string strBody = "<html><body>===========================================" + "<br />" +
								"Contact Request" + "<br />" +
								"===========================================" + "<br />" +
								"Name: " + Name + "<br />" +
								"Email: " + Email + "<br />" +
								"Phone: " + Phone + "<br />" +
								"Page: " + Referrer + "<br />" +
								"Message:<br />" + Comments + "<br />" +
								"</body></html>";

			MailMessage mm = new MailMessage();
			if (tksUtil.IsValidEmail(Email)) {
				mm.From = new MailAddress(Email);
			} else {
				mm.From = new MailAddress(Global.SiteEmail);
			}
			mm.Subject = "Contact Request - " + Global.TitleSuffix;
			mm.To.Add(Global.SiteEmail);
			mm.Bcc.Add("tom@eerieglow.com");

			AlternateView plainView = AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(strBody, @"<(.|\n)*?>", string.Empty), System.Text.Encoding.GetEncoding("utf-8"), "text/plain");
			AlternateView htmlView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.GetEncoding("utf-8"), "text/html");
			mm.AlternateViews.Add(plainView);
			mm.AlternateViews.Add(htmlView);

			SmtpClient smtp = new SmtpClient();
			smtp.Send(mm);

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO [cms_Contact] " +
							"		   ([Source] " +
							"		   ,[ContactIP] " +
							"		   ,[FullName] " +
							"		   ,[Message] " +
							"		   ,[Email] " +
							"		   ,[Phone]) " +
							"	 VALUES " +
							"		   (@Source, " +
							"		   @ContactIP, " +
							"		   @FullName, " +
							"		   @Message, " +
							"		   @Email, " +
							"		   @Phone)";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Source", SqlDbType.VarChar, 150).Value = Referrer;
					cmd.Parameters.Add("ContactIP", SqlDbType.VarChar, 15).Value = HttpContext.Current.Request.UserHostAddress;
					cmd.Parameters.Add("FullName", SqlDbType.VarChar, 250).Value = Name ?? "";
					cmd.Parameters.Add("Message", SqlDbType.VarChar).Value = Comments ?? "";
					cmd.Parameters.Add("Email", SqlDbType.VarChar, 250).Value = Email ?? "";
					cmd.Parameters.Add("Phone", SqlDbType.VarChar, 250).Value = Phone ?? "";

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
	}

	public class CaptchaResponse {
		[JsonProperty("success")]
		public string Success { get; set; }

		[JsonProperty("error-codes")]
		public List<string> ErrorCodes { get; set; }
	}
}