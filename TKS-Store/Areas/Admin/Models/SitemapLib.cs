/***************************************************************************************
 *  File: SitemapLib.cs
 *  Author: nathanbuggia.com
 *  Date: 4/15/2007
 *  
 *  Use this library to generate a standard XML Sitemap for your website. Follows the 
 *  sitemap standard here: http://sitemaps.org. There are no copyrights or limitations
 *  on use for this library. Use it, sell it, claim it as your own, all okay by me.
 * 
 *  One last thing, I'm not a professional developer, I'm a marketer. Use this code at 
 *  your own risk, because I might be committing some horrible mistake that causes the 
 *  world to end.
 * *************************************************************************************/

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.Net;

//
// Implements the sitemap standard, http://sitemaps.org
//

namespace SitemapLib {
	public enum ChangeFrequency {
		Always = 0,
		Hourly,
		Daily,
		Weekly,
		Monthly,
		Yearly,
		Never,
		DontUseThisField
	}

	public class SiteMapItem {
		private String _loc;
		public String Loc {
			get { return _loc; }
			set { _loc = value; }
		}

		private DateTime _lastmod;
		public DateTime LastMod {
			get { return _lastmod; }
			set { _lastmod = value; }
		}

		private String _priority;
		public String Priority {
			get { return _priority; }
			set { _priority = value; }
		}

		private ChangeFrequency _changeFreq;
		public ChangeFrequency ChangeFreq {
			get { return _changeFreq; }
			set { _changeFreq = value; }
		}

		public SiteMapItem() { }
	}

	/// <summary>
	/// Summary description for SitemapLib
	/// </summary>
	public class Sitemap {
		List<SiteMapItem> SiteMapList = null;

		private String _entityEscape(String s) {
			return s.Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;").Replace(">", "&gt;").Replace("<", "&lt;");
		}

		public void AddLocation(String location) {
			this.AddLocation(location, new DateTime(0L), "", ChangeFrequency.DontUseThisField);
		}

		public void AddLocation(String location, DateTime lastmod) {
			this.AddLocation(location, lastmod, "", ChangeFrequency.DontUseThisField);
		}

		public void AddLocation(String location, DateTime lastmod, ChangeFrequency changeFreq) {
			this.AddLocation(location, lastmod, "", changeFreq);
		}

		public void AddLocation(String location, DateTime lastmod, String priority, ChangeFrequency changeFreq) {
			SiteMapItem item = new SiteMapItem();
			item.Loc = location;
			item.LastMod = lastmod;
			item.Priority = priority;
			item.ChangeFreq = changeFreq;

			this.AddLocation(item);
		}

		public void AddLocation(SiteMapItem item) {
			if (null == SiteMapList)
				SiteMapList = new List<SiteMapItem>();

			SiteMapList.Add(item);
		}

		public String GenerateSitemapXML() {
			StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

			sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

			// ERROR CHECK: see if there are more than 50k URLs in file
			if (SiteMapList.Count > 50000) {
				throw new Exception("Sitemap file cannot contain more than 50,000 URLs. Refer to http://sitemaps.org for details");
			}

			foreach (SiteMapItem item in SiteMapList) {
				// ERROR CHECK: Make sure a URL was entered
				if (item.Loc.Length < 9) {
					throw new Exception("Sitemap entry must include URL. Refer to http://sitemaps.org for details");
				}

				// ERROR CHECK: URL must include protocol (http, https)
				if (!item.Loc.Substring(0, 7).ToLower().Equals("http://") &&
					!item.Loc.Substring(0, 8).ToLower().Equals("https://") &&
					!item.Loc.Substring(0, 7).ToLower().Equals("feed://")) {
					throw new Exception("Sitemap URLs must include protocol (e.g. http://). Refer to http://sitemaps.org for details");
				}

				// ERROR CHECK: URL must be smaller than 2048 characters
				if (item.Loc.Length >= 2048) {
					throw new Exception("Sitemap URLs cannot have more than 2048 characters. Refer to http://sitemaps.org for details");
				}

				sb.Append("<url>");

				// LOCATION FIELD
				sb.Append("<loc>");
				sb.Append(_entityEscape(item.Loc));
				sb.Append("</loc>");

				// LAST MODIFIED FIELD
				if (0L != item.LastMod.Ticks) {
					sb.Append("<lastmod>");
					sb.Append(item.LastMod.ToString("yyyy-MM-dd"));
					sb.Append("</lastmod>");
				}

				// CHANGE FREQUENCY FIELD
				if (ChangeFrequency.DontUseThisField != item.ChangeFreq) {
					sb.Append("<changefreq>");
					switch (item.ChangeFreq) {
						case ChangeFrequency.Always:
							sb.Append("always");
							break;
						case ChangeFrequency.Daily:
							sb.Append("daily");
							break;
						case ChangeFrequency.Hourly:
							sb.Append("hourly");
							break;
						case ChangeFrequency.Monthly:
							sb.Append("monthly");
							break;
						case ChangeFrequency.Never:
							sb.Append("never");
							break;
						case ChangeFrequency.Weekly:
							sb.Append("weekly");
							break;
						case ChangeFrequency.Yearly:
							sb.Append("yearly");
							break;
					}
					sb.Append("</changefreq>");
				}

				// PRIORITY FIELD
				if (item.Priority.Length > 0) {
					sb.Append("<priority>");
					sb.Append(item.Priority);
					sb.Append("</priority>");
				}

				sb.Append("</url>");
			}

			sb.Append("</urlset>");

			// ERROR CHECK: check the filesize to make sure it doesn't exceed the maximum allowed
			if (sb.Length > 10485760) {
				throw new Exception("Sitemap file cannot be larger than 10MB. Refer to http://sitemaps.org for details");
			}

			return sb.ToString();
		}

		public static void Ping(String sitemapFileURL, string yahooAppID) {
			string STD_PING_PATH = "/ping?sitemap=" + sitemapFileURL;

			// GOOGLE
			try {
				WebRequest request = System.Net.HttpWebRequest.Create("http://www.google.com/webmasters/tools" + STD_PING_PATH);
				WebResponse response = request.GetResponse();
			} catch (Exception e) {
				Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(e));
			}

			// YAHOO
			try {
				// notice how they are not following the standard....
				WebRequest request = System.Net.HttpWebRequest.Create("http://search.yahooapis.com/SiteExplorerService/V1/updateNotification?appid=" + yahooAppID + "&url=" + sitemapFileURL);
				WebResponse response = request.GetResponse();
			} catch (Exception e2) {
				Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(e2));
			}

			// ASK.COM
			try {
				WebRequest request = System.Net.HttpWebRequest.Create("http://submissions.ask.com" + STD_PING_PATH);
				WebResponse response = request.GetResponse();
			} catch (Exception e3) {
				Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(e3));
			}
		}

		public Sitemap() { }
	}


}