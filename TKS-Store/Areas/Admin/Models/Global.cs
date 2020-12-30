using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Areas.Admin.Models {
	public static class Global {
		public static string BaseURL { get { return string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, HttpContext.Current.Request.Url.Port == 80 ? string.Empty : ":" + HttpContext.Current.Request.Url.Port); } }
		public static string AdminRelURL { get { return ConfigString("AdminRelURL"); } }
		public static string BlogCommentNotificationAddress { get { return ConfigString("BlogCommentNotificationAddress"); } }
		public static string BlogDirectory { get { return ConfigString("BlogDirectory"); } }
		public static string ColorPrompt { get { return ConfigString("ColorPrompt"); } }
		public static string FacebookAdmin { get { return ConfigString("FacebookAdmin"); } }
		public static string Favicon { get { return ConfigString("Favicon"); } }
		public static string FeedDescription { get { return ConfigString("FeedDescription"); } }
		public static string FeedTitle { get { return ConfigString("FeedTitle"); } }
		public static string SiteName { get { return ConfigString("SiteName"); } }
		public static string SiteNameAlternate { get { return ConfigString("SiteNameAlternate"); } }

		public static int mnuPosReports { get { return ConfigInt("mnuPosReports"); } }
		public static int mnuPosProducts { get { return ConfigInt("mnuPosProducts"); } }
		public static int mnuPosContent { get { return ConfigInt("mnuPosContent"); } }
		public static int mnuPosStore { get { return ConfigInt("mnuPosStore"); } }
		public static int mnuPosContacts { get { return ConfigInt("mnuPosContacts"); } }
		public static int mnuPosBlog { get { return ConfigInt("mnuPosBlog"); } }
		public static int mnuPosPayment { get { return ConfigInt("mnuPosPayment"); } }
		public static int mnuPosShipping { get { return ConfigInt("mnuPosShipping"); } }
		public static int mnuPosGeography { get { return ConfigInt("mnuPosGeography"); } }
		public static int mnuPosAccounts { get { return ConfigInt("mnuPosAccounts"); } }

		public static bool FreeShipping { get { return ConfigBool("FreeShipping"); } }
		public static decimal FreeShippingMinimum { get { return ConfigDecimal("FreeShippingMinimum"); } }
		public static decimal ShippingHandling { get { return ConfigDecimal("ShippingHandling"); } }
		public static decimal ShippingInsuranceThreshold { get { return ConfigDecimal("ShippingInsuranceThreshold"); } }
		public static decimal ShippingInsurancePercent { get { return ConfigDecimal("ShippingInsurancePercent"); } }

		public static int GalleryLgHeight { get { return ConfigInt("GalleryLgHeight"); } }
		public static int GalleryLgWidth { get { return ConfigInt("GalleryLgWidth"); } }
		public static int GalleryMedHeight { get { return ConfigInt("GalleryMedHeight"); } }
		public static int GalleryMedWidth { get { return ConfigInt("GalleryMedWidth"); } }
		public static int GallerySmHeight { get { return ConfigInt("GallerySmHeight"); } }
		public static int GallerySmWidth { get { return ConfigInt("GallerySmWidth"); } }
		public static int GalleryThumbHeight { get { return ConfigInt("GalleryThumbHeight"); } }
		public static string GalleryThumbMode { get { return ConfigString("GalleryThumbMode"); } }
		public static string GalleryThumbScale { get { return ConfigString("GalleryThumbScale"); } }
		public static int GalleryThumbWidth { get { return ConfigInt("GalleryThumbWidth"); } }

		public static string GooglePublisher { get { return ConfigString("GooglePublisher"); } }
		public static string GoogleAuthor { get { return ConfigString("GoogleAuthor"); } }
		public static string MobileRelative { get { return ConfigString("MobileRelative"); } }
		public static bool ModerateReviews { get { return ConfigBool("ModerateReviews"); } }
		public static string NonMobileRelative { get { return ConfigString("NonMobileRelative"); } }
		public static bool NoShipping { get { return ConfigBool("NoShipping"); } }
		public static string OrderNoticeEmail { get { return ConfigString("OrderNoticeEmail"); } }
		public static bool RatingsAllow { get { return ConfigBool("RatingsAllow"); } }
		public static int RecentlyViewedToShow { get { return ConfigInt("RecentlyViewedToShow"); } }
		public static bool ReviewsAllow { get { return ConfigBool("ReviewsAllow"); } }
		public static bool ReviewsModerate { get { return ConfigBool("ReviewsModerate"); } }
		public static string RSSFeedURL { get { return BaseURL + ConfigString("RSSFeedURL"); } }
		public static bool ShowBuyButton { get { return ConfigBool("ShowBuyButton"); } }
		public static string SizePrompt { get { return ConfigString("SizePrompt"); } }
		public static string SiteEmail { get { return ConfigString("SiteEmail"); } }
		public static string TitleSuffix { get { return ConfigString("TitleSuffix"); } }
		public static string TitleSuffixSep { get { return ConfigString("TitleSuffixSep"); } }
		public static string TwitterName { get { return ConfigString("TwitterName"); } }
		public static string TwitterCreator { get { return ConfigString("TwitterCreator"); } }
		public static string VerificationBing { get { return ConfigString("VerificationBing"); } }
		public static string VerificationGoogle { get { return ConfigString("VerificationGoogle"); } }

		public static string UPS_ShipperCity { get { return ConfigString("UPS_ShipperCity"); } }
		public static string UPS_ShipperCountryCode { get { return ConfigString("UPS_ShipperCountryCode"); } }
		public static string UPS_ShipperStateProvinceCode { get { return ConfigString("UPS_ShipperStateProvinceCode"); } }
		public static string UPS_ShipperNumber { get { return ConfigString("UPS_ShipperNumber"); } }
		public static string UPS_ShipperPostalCode { get { return ConfigString("UPS_ShipperPostalCode"); } }
		public static string UPS_UserID { get { return ConfigString("UPS_UserID"); } }
		public static string UPS_AccessLicenseNumber { get { return ConfigString("UPS_AccessLicenseNumber"); } }
		public static string UPS_Password { get { return ConfigString("UPS_Password"); } }
		public static string UPS_WebURL { get { return ConfigString("UPS_WebURL"); } }

		public static string reCAPTCHASiteKey { get { return ConfigString("reCAPTCHASiteKey"); } }
		public static string reCAPTCHASecretKey { get { return ConfigString("reCAPTCHASecretKey"); } }

		public static string NewsletterRedirect { get { return ConfigString("NewsletterRedirect"); } }
		public static string NewsletterMailChimpAPIKey { get { return ConfigString("NewsletterMailChimpAPIKey"); } }
		public static string NewsletterStreamSendAPIPassword { get { return ConfigString("NewsletterStreamSendAPIPassword"); } }
		public static string NewsletterStreamSendAPIUsername { get { return ConfigString("NewsletterStreamSendAPIUsername"); } }

		private static string ConfigString(string key) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Value FROM Config WHERE Name = @Name", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Name", SqlDbType.VarChar, 50).Value = key;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						ret = dr[0].ToString();
					}
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		private static bool ConfigBool(string key) {
			return ConfigString(key) == "1" ? true : false;
		}
		private static int ConfigInt(string key) {
			string ret = ConfigString(key);
			int x = 0;
			if (Int32.TryParse(ret, out x)) {
				return x;
			} else {
				return 0;
			}
		}
		private static decimal ConfigDecimal(string key) {
			string ret = ConfigString(key);
			decimal x = 0;
			if (decimal.TryParse(ret, out x)) {
				return x;
			} else {
				return 0;
			}
		}
	}
}