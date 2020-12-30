using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;
using TKS;

namespace TKS.Areas.Admin.Models.CMS {
	public class Audit {
		#region Fields
		private ContentPage _ContentPage = new ContentPage();
		private string _Locale = "en-US";
		private int _DescriptionTagMax = 155;
		private HtmlDocument _HtmlDocument;
		private int _LinksInternal = 0;
		private int _LinksExternal = 0;
		#endregion

		#region Constructor
		public Audit(ContentPage contentPage, string Locale = "en-US")
		{
			_ContentPage = contentPage;
			_Locale = Locale;

			var webGet = new HtmlWeb();
			_HtmlDocument = webGet.Load(Global.BaseURL + _ContentPage.VirtualPath);
		}
		#endregion

		#region Public Methods
		public List<ContentPage> DescriptionDuplicatePages() {
			List<ContentPage> l = new List<ContentPage>();

			if (!MetaDescriptionDuplicateTest) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT EntityGUID FROM [cms_SEO] WHERE MetaDescription = @MetaValue", cn)) {
						cmd.CommandType = CommandType.Text;
						//cmd.Parameters.Add("MetaValue", SqlDbType.VarChar).Value = _ContentPage.GetTag("Description", _Locale);

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new ContentPage(dr.GetGuid(0)));
						}
						cmd.Connection.Close();
					}
				}
			}
			return l;
		}
		public string H1Tags() {
			string ret = "";
			HtmlNodeCollection nodes = _HtmlDocument.DocumentNode.SelectNodes("//h1");
			if (nodes != null) {
				foreach (var tag in nodes) {
					ret += "<div>" + tag.InnerText + "</div>";
				}
			}
			return ret;
		}
		public string Hrefs() {
			string ret = "";
			HtmlNodeCollection nodes = _HtmlDocument.DocumentNode.SelectNodes("//a");
			if (nodes != null) {
				SortedSet<string> links = new SortedSet<string>();
				foreach (var tag in nodes) {
					if (tag.Attributes["href"] != null) {
						string href = tag.Attributes["href"].Value.ToLower();
						if (!href.StartsWith("#") && !href.StartsWith("javascript") && !href.StartsWith("mailto")) {
							if (href.StartsWith("//")) {
								href = "http:" + href;
							} else if (href.StartsWith("/")) {
								href = Global.BaseURL + href;
							}

							if (href.StartsWith(Global.BaseURL)) {
								_LinksInternal++;
							} else {
								_LinksExternal++;
							}
							if (!links.Contains(href)) { links.Add(href); }
						}
					}
				}

				foreach(string link in links) {
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
					request.Timeout = 15000;
					request.Method = "HEAD";
					try {
						using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
							if (response.StatusCode == HttpStatusCode.OK) {
								ret += "<div><a href='" + link + "' target='_blank'>" + link + "</a></div>";
							} else {
								ret += "<div style='color:#f00;font-weight:bold'>" + response.StatusDescription + " - <a href='" + link + "' target='_blank'>" + link + "</a></div>";
							}
						}
					} catch (WebException ex) {
						ret += "<div style='color:#f00'>" + ex.Message + " - <a href='" + link + "' target='_blank'>" + link + "</a></div>";
					} 
				}
			}
			return ret;
		}
		public string Images() {
			string ret = "";
			HtmlNodeCollection nodes = _HtmlDocument.DocumentNode.SelectNodes("//img");
			if (nodes != null) {
				foreach (var tag in nodes) {
					if (tag.Attributes["src"] != null) {
						string src = tag.Attributes["src"].Value.ToLower();
						ret += "<p>" + src + "<br />";

						if (src.StartsWith("//")) {
							src = "http:" + src;
						} else if (src.StartsWith("/")) {
							src = Global.BaseURL + src;
						}

						if (tag.Attributes["alt"] == null) {
							ret += "<span style='color:#f00'>No ALT text.</span><br />";
						} else if (string.IsNullOrEmpty(tag.Attributes["alt"].Value.ToString())) {
							ret += "<span style='color:#f00'>No ALT text.</span><br />";
						}


						HttpWebRequest request = (HttpWebRequest)WebRequest.Create(src);
						request.Timeout = 15000;
						request.Method = "HEAD";
						try {
							using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
								if (response.StatusCode != HttpStatusCode.OK) {
									ret += "<span style='color:#f00;font-weight:bold'>" + response.StatusDescription + " - <a href='" + src + "' target='_blank'>" + src + "</a></span>";
								}
							}
						} catch (WebException ex) {
							ret += "<span style='color:#f00'>" + ex.Message + " - <a href='" + src + "' target='_blank'>" + src + "</a></span>";
						}

						ret += "</p>";
					}
				}
			}
			return ret;
		}
		public List<ContentPage> KeywordDuplicatePages() {
			List<ContentPage> l = new List<ContentPage>();

			if (!MetaKeywordDuplicateTest) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT EntityGUID FROM [cms_SEO] WHERE MetaKeywords = @MetaValue", cn)) {
						cmd.CommandType = CommandType.Text;
						//cmd.Parameters.Add("MetaValue", SqlDbType.VarChar).Value = _ContentPage.GetTag("Keywords", _Locale);

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new ContentPage(dr.GetGuid(0)));
						}
						cmd.Connection.Close();
					}
				}
			}
			return l;
		}
		public List<ContentPage> TitleDuplicatePages() {
			List<ContentPage> l = new List<ContentPage>();

			if (string.IsNullOrEmpty(_ContentPage.seo.MetaTitle)) {
			} else {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT EntityGUID FROM [cms_SEO] WHERE MetaTitle = @MetaValue", cn)) {
						cmd.CommandType = CommandType.Text;
						//cmd.Parameters.Add("MetaValue", SqlDbType.VarChar).Value = _ContentPage.GetTag("Title", _Locale);

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new ContentPage(dr.GetGuid(0)));
						}
						cmd.Connection.Close();
					}
				}
			}
			return l;
		}
		#endregion

		#region Properties
		public int H1Count {
			get {
				HtmlNodeCollection nodes = _HtmlDocument.DocumentNode.SelectNodes("//h1");
				if (nodes == null) {
					return 0;
				} else {
					return nodes.Count;
				}
			}
		}

		public int LinksExternal {
			get { 
				if (_LinksExternal == 0) { Hrefs(); }
				return _LinksExternal;
			}
		}
		public int LinksInternal {
			get {
				if (_LinksInternal == 0) { Hrefs(); }
				return _LinksInternal;
			}
		}

		public bool MetaDescriptionLengthTest {
			get { return _ContentPage.seo.MetaDescription.Length <= _DescriptionTagMax && _ContentPage.seo.MetaDescription.Length > 0; }
		}
		public string MetaDescriptionLengthText {
			get {
				if (MetaDescriptionLengthTest) {
					return string.Format("Description tag is no more than {0} characters.", _DescriptionTagMax);
				} else {
					if (_ContentPage.seo.MetaDescription.Length > _DescriptionTagMax) {
						return string.Format("Description tag exceeds {0} characters. Length is {1} characters.", _DescriptionTagMax, _ContentPage.seo.MetaDescription.Length);
					} else {
						return "Description tag not provided.";
					}
				}
			}
		}
		public bool MetaDescriptionDuplicateTest {
			get {
				if (string.IsNullOrEmpty(_ContentPage.seo.MetaDescription)) {
					return true;
				} else {
					bool ret = false;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM cms_SEO WHERE MetaDescription = @MetaValue", cn)) {
							cmd.CommandType = CommandType.Text;
							//cmd.Parameters.Add("MetaValue", SqlDbType.VarChar).Value = _ContentPage.GetTag("Description", _Locale);

							cmd.Connection.Open();
							int rows = (int)cmd.ExecuteScalar();
							cmd.Connection.Close();

							ret = rows <= 1;
						}
					}
					return ret;
				}
			}
		}
		public string MetaDescriptionDuplicateText {
			get {
				if (string.IsNullOrEmpty(_ContentPage.seo.MetaDescription)) {
					return "";
				} else if (MetaDescriptionDuplicateTest) {
					return "Description tag is unique.";
				} else {
					return "Description tag duplicated on other pages.";
				}
			}
		}

		public bool MetaKeywordLengthTest {
			get { return _ContentPage.seo.MetaKeywords.Length > 0; }
		}
		public string MetaKeywordLengthText {
			get {
				if (MetaKeywordLengthTest) {
					return "Keywords tag is used.";
				} else {
					return "Keywords tag is not used.";
				}
			}
		}
		public bool MetaKeywordDuplicateTest {
			get {
				if (string.IsNullOrEmpty(_ContentPage.seo.MetaKeywords)) {
					return true;
				} else {
					bool ret = false;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM cms_SEO WHERE MetaKeywords = @MetaValue", cn)) {
							cmd.CommandType = CommandType.Text;
							//cmd.Parameters.Add("MetaValue", SqlDbType.VarChar).Value = _ContentPage.GetTag("Keywords", _Locale);

							cmd.Connection.Open();
							int rows = (int)cmd.ExecuteScalar();
							cmd.Connection.Close();

							ret = rows <= 1;
						}
					}
					return ret;
				}
			}
		}
		public string MetaKeywordDuplicateText {
			get {
				if (string.IsNullOrEmpty(_ContentPage.seo.MetaDescription)) {
					return "";
				} else if (MetaKeywordDuplicateTest) {
					return "Keywords tag is unique.";
				} else {
					return "Keywords tag duplicated on other pages.";
				}
			}
		}
		
		public bool MetaTitleLengthTest {
			get { return _ContentPage.seo.MetaTitle.Length <= 60 && _ContentPage.seo.MetaTitle.Length > 0; }
		}
		public string MetaTitleLengthText {
			get {
				if (MetaTitleLengthTest) {
					return "Title tag is no more than 60 characters.";
				} else {
					if (_ContentPage.seo.MetaTitle.Length > 60) {
						return "Title tag exceeds 60 characters. Length is " + _ContentPage.seo.MetaTitle.Length + " characters.";
					} else {
						return "Title tag not provided.";
					}
				}
			}
		}
		public bool MetaTitleDuplicateTest {
			get {
				if (string.IsNullOrEmpty(_ContentPage.seo.MetaTitle)) {
					return false;
				} else {
					bool ret = false;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM cms_SEO WHERE MetaTitle = @MetaValue", cn)) {
							cmd.CommandType = CommandType.Text;
							//cmd.Parameters.Add("MetaValue", SqlDbType.VarChar).Value = _ContentPage.GetTag("Title", _Locale);

							cmd.Connection.Open();
							int rows = (int)cmd.ExecuteScalar();
							cmd.Connection.Close();

							ret = rows <= 1;
						}
					}
					return ret;
				}
			}
		}
		public string MetaTitleDuplicateText {
			get {
				if (string.IsNullOrEmpty(_ContentPage.seo.MetaTitle)) {
					return "Title tag is just the default.";
				} else if (MetaTitleDuplicateTest) {
					return "Title tag is unique.";
				} else {
					return "Title tag duplicated on other pages.";
				}
			}
		}
		#endregion
	}
}