using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Models.CMS{
	public class ContentPage {
		#region Fields
		private bool _IncludeInSitemap = true;
		private bool _IsActive = true;
		private string _Locale = "";
		private Guid _PageID = Guid.Empty;
		private Guid _PageTypeID = Guid.Empty;
		private string _PageTypeName = "";
		private string _VirtualPath = "";
		private string _RedirectURL = "";
		#endregion

		#region Constructor
		public ContentPage() { }
		public ContentPage(Guid PageID) {
			_PageID = PageID;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT p.Locale, p.PageTypeID, p.VirtualPath, p.IncludeInSitemap, t.PageTypeName, p.IsActive " +
														"FROM cms_Page p JOIN cms_PageType t ON p.PageTypeID = t.PageTypeID " +
														"WHERE PageID = @PageID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_Locale = dr[0].ToString();
						_PageTypeID = dr.GetGuid(1);
						_VirtualPath = dr[2].ToString();
						_IncludeInSitemap = dr.GetBoolean(3);
						_PageTypeName = dr[4].ToString();
						_IsActive = dr.GetBoolean(5);
					} else {
						_PageID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		public ContentPage(string VirtualPath) {
			_VirtualPath = VirtualPath;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT p.Locale, p.PageTypeID, p.PageID, p.IncludeInSitemap, t.PageTypeName, p.IsActive FROM cms_Page p JOIN cms_PageType t ON p.PageTypeID = t.PageTypeID WHERE VirtualPath = @VirtualPath", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("VirtualPath", SqlDbType.VarChar, 250).Value = _VirtualPath;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_Locale = dr[0].ToString();
						_PageID = dr.GetGuid(2);
						_PageTypeID = dr.GetGuid(1);
						_PageTypeName = dr[4].ToString();
						_IncludeInSitemap = dr.GetBoolean(3);
						_IsActive = dr.GetBoolean(5);
					} else {
						CheckRedirect();
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void CheckRedirect() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT RedirectTo FROM [cms_Redirect] WHERE RedirectFrom = @RedirectFrom", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("RedirectFrom", SqlDbType.VarChar, 500).Value = _VirtualPath;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_RedirectURL = dr[0].ToString();
					} else {
						_RedirectURL = "";
					}
					cmd.Connection.Close();
				}
			}
		}
		private string GetDocumentList() {
			List<TKS.Areas.Admin.Models.CMS.Document> documents = new TKS.Areas.Admin.Models.CMS.DocumentSet(PageID, "DocumentList", Locale).Documents();
			StringBuilder sb = new StringBuilder();
			string LastCategory = "";
			foreach (TKS.Areas.Admin.Models.CMS.Document doc in documents) {
				if (LastCategory != doc.DocumentCategory) {
					LastCategory = doc.DocumentCategory;
					sb.AppendLine(string.Format("<h2>{0}</h2>", doc.DocumentCategory));
				}
				if (!string.IsNullOrEmpty(doc.DocumentTitle)) {
					sb.AppendLine(string.Format("<h3>{0}</h3>", doc.DocumentTitle));
				}
				sb.AppendLine(string.Format("<div>{0}</div>", doc.Description));
				sb.AppendLine(string.Format("<div>{0}</div>", doc.Link));
				sb.AppendLine("<hr />");
			}
			return sb.ToString();
		}
		private string GetFAQ() {
			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();
			string lastCategory = "";
			int Hdr = 1;
			int Q = 1;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT c.FAQCategory, f.Question, f.Answer FROM cms_FAQ f JOIN cms_Module m ON f.ModuleID = m.ModuleID LEFT JOIN cms_FAQCategory c ON f.FAQCategorySerial = c.FAQCategorySerial WHERE m.PageID = @PageID AND m.ModuleName = 'FAQ' AND f.Locale = @Locale ORDER BY c.SortOrder, c.FAQCategory, f.SortOrder, f.Question";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = "en-US";
					cmd.Connection.Open();

					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
						//sb.AppendLine("<dl>");
						while (dr.Read()) {
							if (lastCategory != dr[0].ToString()) {
								if (sb.Length > 0) {
									sb.AppendLine("</p>");
									sb2.AppendLine("</dl>");
								}
								sb.AppendLine("<h2>" + dr[0].ToString() + "</h2><p class='noindent'>");
								sb2.AppendLine("<dl><h2 id='faqHdr" + Hdr.ToString() + "'>" + dr[0].ToString() + "</h2>");
								lastCategory = dr[0].ToString();
								Hdr++;
							}
							sb.AppendLine("<a href='#FAQ" + Q.ToString() + "'>" + dr[1].ToString() + "</a><br />");
							sb2.AppendLine("<dt id='FAQ" + Q.ToString() + "'>" + dr[1].ToString() + "</dt>");
							sb2.AppendLine("<dd>" + dr[2].ToString() + "</dd>");
							Q++;
						}
						sb.AppendLine("</p>");
					}
					cmd.Connection.Close();
				}
			}

			return sb.ToString() + sb2.ToString();

		}
		private string GetNews() {
			StringBuilder sb = new StringBuilder();
			TKS.Areas.Admin.Models.CMS.NewsSet newsSet = new TKS.Areas.Admin.Models.CMS.NewsSet(new Guid("64d6ad9c-84d0-4a47-9465-3e3c04db87d3"));
			foreach (TKS.Areas.Admin.Models.CMS.NewsModel news in newsSet.News()) {
				string link = "/support/news/" + news.NewsSerial + "/" + TKS.Areas.Admin.Models.tksUtil.FormatRouteURL(news.Headline);

				sb.AppendLine("<div class='ym-grid'>");
				sb.AppendLine("<div class='ym-g75 ym-gl'>");
				sb.AppendLine("<h2><a href='" + link + "'>" + news.Headline + "</a></h2>");
				sb.AppendLine("</div>");
				sb.AppendLine("<div class='ym-g25 ym-gr blog-date'>");
				sb.AppendLine("<span>" + news.DateReleased.ToShortDateString() + "</span>");
				sb.AppendLine("</div>");
				sb.AppendLine("</div>");
				sb.AppendLine("<div class='blog-intro'>");
				sb.AppendLine(news.ShortDescription);
				sb.AppendLine("<a href='" + link + "'>[read&nbsp;more]</a></p>");
				sb.AppendLine("</div>");
				sb.AppendLine("<hr />");
			}
			return sb.ToString();
		}
		private string GetFooterSection(Guid SectionID) {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT m.ModuleID, mt.ModuleTypeName " +
							"	FROM cms_Module m JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
							"	WHERE m.PageSectionID = @PageSectionID" +
							"	AND m.PageID = @PageID " +
							"	ORDER BY m.SortOrder, m.ModuleName";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = SectionID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						string moduleContents = GetFooterModule(dr.GetGuid(0), dr[1].ToString());
						if (!string.IsNullOrEmpty(moduleContents)) {
							sb.AppendLine(moduleContents);
						}
					}
					cmd.Connection.Close();
				}
			}
			return sb.ToString();
		}
		private string GetFooterModule(Guid ModuleID, string ModuleTypeName) {
			string ret = "";
			StringBuilder sb = new StringBuilder();
			switch (ModuleTypeName.ToLower()) {
				case "gallery":
					List<GalleryItemModel> gallery = new Gallery(ModuleID).GetImages();
					if (gallery.Count > 0) {
						string path = "/assets/images/gallery/" + ModuleID.ToString() + "/";
						string lg = "";
						string sm = "";
						foreach (GalleryItemModel item in gallery) {
							if (!string.IsNullOrEmpty(item.PhotoPath)) {
								lg = path + item.PhotoPath;
							} else {
								lg = path + "full/" + item.FullscreenPath;
							}
							if (!string.IsNullOrEmpty(item.ThumbPath)) {
								sm = path + "thumb/" + item.ThumbPath;
							} else if (!string.IsNullOrEmpty(item.PhotoPath)) {
								sm = path + item.PhotoPath;
							} else {
								sm = path + "full/" + item.FullscreenPath;
							}
							sb.AppendLine("{href:'" + lg + "',thumb :'" + sm + "',title:'" + item.PhotoTitle + "'},");
						}
						ret = sb.ToString();
						ret = ret.Substring(0, ret.Length - 3);
						ret = "$('.open-" + ModuleID.ToString() + "').click(function () { " +
								"	$.fancybox.open([" + ret;
						sb.Clear();
						sb.AppendLine(ret + "], {");
						sb.AppendLine("		nextEffect : 'fade',");
						sb.AppendLine("		prevEffect : 'fade',");
						sb.AppendLine("		padding    : 0,");
						sb.AppendLine("		closeBtn: false,");
						sb.AppendLine("		helpers: {");
						sb.AppendLine("			title : { type: 'over' },");
						sb.AppendLine("			buttons: {},");
						sb.AppendLine("			thumbs : {");
						sb.AppendLine("			source : function( item ) {if (item.thumb) {return item.thumb;} else {return item.href;}}");
						//sb.AppendLine("				width:" + TKS.Areas.Admin.Models.Global.GalleryThumbWidth + ", height:" + TKS.Areas.Admin.Models.Global.GalleryThumbHeight);
						sb.AppendLine("			}");
						sb.AppendLine("		}");
						sb.AppendLine("	});");
						sb.AppendLine("	return false;");
						sb.AppendLine("});");
					}
					ret = sb.ToString();
					break;
				case "slideshow":
					sb.AppendLine("$('.flexslider').flexslider({");
					sb.AppendLine("	animation: 'fade',");
					sb.AppendLine("	start: function (slider) { $('body').removeClass('loading'); }");
					sb.AppendLine("})");
					ret = sb.ToString();
					break;
			}
			return ret;
		}
		private string ResolveShortcodes(string content) {
			string found = "";
			string replace = "";

			string regularExpressionPattern = @"\[(.*?)\]";
			Regex re = new Regex(regularExpressionPattern);

			foreach (Match m in re.Matches(content)) {
				found = m.Value;
				if (found.StartsWith("[youtube ")) {
					found = found.Substring(("[youtube ").Length);
					found = found.Substring(0, found.Length - 1);
					replace = new YouTube(found).GetCode();
					content = content.Replace(m.Value, replace);
				}
			}

			return content;
		}
		#endregion

		#region Public Methods
		public string GetModule(Guid ModuleID, string ModuleTypeName) {
			string ret = "";
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT mt.ModuleTypeName, c.Contents FROM cms_Module m LEFT JOIN cms_Content c ON m.ModuleID = c.ModuleID JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID WHERE m.ModuleID = @ModuleID AND (IsDraft IS NULL OR IsDraft = @IsDraft) AND UpdateDate IS NULL";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = ModuleID;
					cmd.Parameters.Add("IsDraft", SqlDbType.VarChar, 50).Value = 0;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						switch (ModuleTypeName.ToLower()) {
							//case "gallery":
							//	List<GalleryItemModel> gallery = new Gallery(ModuleID).GetImages();
							//	if (gallery.Count > 0) {
							//		ret = "<div class='gallery-intro'>" + gallery[0].PhotoImageTag;
							//		ret += "<a href='#' class='open-" + ModuleID.ToString() + "'><img src='/assets/images/base/more-pictures.png' alt='More Pictures' height='36' width='147' /></a>";
							//		ret += "</div>";
							//	}
							//	break;
							case "slideshow":
								ret = new ContentSlideshow(ModuleID).Flexslider();
								break;
							case "youtube":
								ret = new ContentVideo(ModuleID).Video;
								break;
							case "image":
								ret = new ContentImage(ModuleID).ImgTag;
								break;
							case "testimonials":
								List<TKS.Areas.Admin.Models.CMS.Testimonial> testimonialList = new TKS.Areas.Admin.Models.CMS.TestimonialSet().TestimonialsRandom();
								foreach (TKS.Areas.Admin.Models.CMS.Testimonial testimonial in testimonialList) {
									sb.AppendLine(testimonial.Content);
									sb.AppendLine(testimonial.ReceivedFrom);
									sb.AppendLine("<hr />");
								}
								ret = sb.ToString();
								break;
							case "documentlist":
								ret = GetDocumentList();
								break;
							case "faq":
								ret = GetFAQ();
								break;
							default:
								//ret = ResolveShortcodes(dr[1].ToString());
								ret = dr[1].ToString();
								break;
						}
					}
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		public string GetSection(Guid SectionID) {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT m.ModuleID, mt.ModuleTypeName " +
							"	FROM cms_Module m JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
							"	WHERE m.PageSectionID = @PageSectionID" +
							"	AND m.PageID = @PageID " +
							"	ORDER BY m.SortOrder, m.ModuleName";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = SectionID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						string moduleContents = GetModule(dr.GetGuid(0), dr[1].ToString());
						if (!string.IsNullOrEmpty(moduleContents)) {
							sb.AppendLine(moduleContents);
						}
					}
					cmd.Connection.Close();
				}
			}
			return sb.ToString();
		}
		public string GetSection(Guid PageID, Guid SectionID) {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT m.ModuleID, mt.ModuleTypeName " +
							"	FROM cms_Module m JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
							"	WHERE m.PageSectionID = @PageSectionID" +
							"	AND m.PageID = @PageID " +
							"	ORDER BY m.SortOrder, m.ModuleName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = SectionID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						string moduleContents = GetModule(dr.GetGuid(0), dr[1].ToString());
						if (!string.IsNullOrEmpty(moduleContents)) {
							sb.AppendLine(moduleContents);
						}
					}
					cmd.Connection.Close();
				}
			}
			return sb.ToString();
		}
		public ViewDataDictionary GetSections() {
			ViewDataDictionary vd = new ViewDataDictionary();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT s.PageSectionID, s.PageSectionName " +
							"	FROM cms_Page p JOIN cms_PageSection s ON p.PageTypeID = s.PageTypeID " +
							"	WHERE p.PageID = @PageID" +
							"	ORDER BY s.PageSectionName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						string sectionContents = GetSection(dr.GetGuid(0));
						if (!string.IsNullOrEmpty(sectionContents)) {
							vd.Add(dr[1].ToString(), sectionContents);
						}
						string footerSectionContents = GetFooterSection(dr.GetGuid(0));
						if (!string.IsNullOrEmpty(footerSectionContents)) {
							if (vd.ContainsKey("FooterCode")) {
								string toAdd = vd["FooterCode"].ToString() + footerSectionContents;
								vd["FooterCode"] = toAdd;
							} else {
								vd.Add("FooterCode", footerSectionContents);
							}
						}
					}
					cmd.Connection.Close();

					//Get shared components that are part of the "Shared" pseudo-page
					cmd.CommandText = "SELECT s.PageSectionID, s.PageSectionName, p.PageID FROM cms_Page p JOIN cms_PageSection s ON p.PageTypeID = s.PageTypeID WHERE p.VirtualPath = 'Shared' ORDER BY s.PageSectionName";
					cmd.Parameters.Clear();
					cmd.Connection.Open();
					dr = cmd.ExecuteReader();
					while (dr.Read()) {
						string sectionContents = GetSection(dr.GetGuid(2), dr.GetGuid(0));
						if (!string.IsNullOrEmpty(sectionContents)) {
							vd.Add(dr[1].ToString(), sectionContents);
						}
					}
					cmd.Connection.Close();
				}
			}
			return vd;
		}
		#endregion

		#region Properties
		public bool IncludeInSitemap {
			get { return _IncludeInSitemap; }
			set {
				if (_IncludeInSitemap != value) {
					_IncludeInSitemap = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Page SET IncludeInSitemap = @Value WHERE PageID = @PageID", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
							cmd.Parameters.Add("Value", SqlDbType.Bit).Value = value ? 1 : 0;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public bool IsActive { get { return _IsActive; } }
		public string Locale {
			get { return _Locale; }
			set {
				if (_Locale != value) {
					_Locale = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Page SET Locale = @Value WHERE PageID = @PageID", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
							cmd.Parameters.Add("Value", SqlDbType.VarChar, 10).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string MetaTags {
			get {
				return new SEO(PageID).GetTags(VirtualPath, "", "");
			}
		}
		public Guid PageID { get { return _PageID; } }
		public Guid PageTypeID {
			get { return _PageTypeID; }
			set {
				if (_PageTypeID != value) {
					_PageTypeID = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Page SET PageTypeID = @Value WHERE PageID = @PageID", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
							cmd.Parameters.Add("Value", SqlDbType.UniqueIdentifier).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();

							//Update the stored PageTypeName
							cmd.CommandText = "SELECT PageTypeName FROM cms_PageType WHERE PageTypeID = @PageTypeID";
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Clear();
							cmd.Parameters.Add("PageTypeID", SqlDbType.UniqueIdentifier).Value = _PageTypeID;

							cmd.Connection.Open();
							SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
							if (dr.HasRows) {
								dr.Read();
								_PageTypeName = dr[0].ToString();
							}
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string PageTypeName {
			get { return _PageTypeName; }
		}
		public string RedirectURL { get { return _RedirectURL; } }
		public SEO seo { get { return new SEO(PageID); } }
		public string VirtualPath {
			get { return _VirtualPath; }
			set {
				if (_VirtualPath != value) {
					_VirtualPath = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Page SET VirtualPath = @Value WHERE PageID = @PageID", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
							cmd.Parameters.Add("Value", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}

		public List<ContentPageType> ContentPageTypeOptions {
			get {
				return ContentPageTypes.PageTypes();
			}
		}
		public virtual ICollection<PageModule> Modules {
			get {
				PageModules pm = new PageModules(_PageID);
				return (ICollection<PageModule>)pm.GetPageModules();
			}
		}
		#endregion
	}
	public class ContentPages {
		public static List<ContentPageBasicViewModel> Pages() {
			List<ContentPageBasicViewModel> l = new List<ContentPageBasicViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT PageID FROM [cms_Page] WHERE IsActive = 1 ORDER BY VirtualPath", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentPageBasicViewModel(new ContentPage(dr.GetGuid(0))));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
	}
	public class ContentPageType {
		public Guid PageTypeID { get; set; }
		public string PageTypeName { get; set; }

		public ContentPageType() {
		}
	}
	public class ContentPageTypes {
		public static List<ContentPageType> PageTypes() {
			List<ContentPageType> l = new List<ContentPageType>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT PageTypeID, PageTypeName FROM cms_PageType ORDER BY PageTypeName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentPageType() { PageTypeID = dr.GetGuid(0), PageTypeName = dr[1].ToString() });
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
	}

	public class PageModule {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		#endregion

		#region Constructor
		public PageModule() { }
		public PageModule(Guid ModuleID) {
			_ModuleID = ModuleID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT mt.ModuleTypeName,  ISNULL(m.ModuleName, mt.ModuleTypeName) AS ModuleName, m.PageID, ISNULL(mt.Controller, '') AS Controller, s.PageSectionName, m.SortOrder, p.Locale, p.VirtualPath " +
							"FROM cms_Module m JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
							"JOIN cms_PageSection s ON m.PageSectionID = s.PageSectionID " +
							"JOIN cms_Page p ON m.PageID = p.PageID " +
							"WHERE m.ModuleID = @ModuleID ";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						ModuleTypeName = dr[0].ToString();
						ModuleName = dr[1].ToString();
						PageID = dr.GetGuid(2);
						Controller = dr[3].ToString();
						PageSectionName = dr[4].ToString();
						SortOrder = dr.GetInt32(5);
						Locale = dr[6].ToString();
						VirtualPath = dr[7].ToString();
					} else {
						_ModuleID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid ContentID { get; set; }
		public string Controller { get; set; }
		public string Locale { get; set; }
		public Guid ModuleID {
			get { return _ModuleID; }
			set { _ModuleID = value; }
		}
		public string ModuleTypeName { get; set; }
		public string ModuleName { get; set; }
		public Guid PageID { get; set; }
		public string PageSectionName { get; set; }
		public ContentPage RelatedPage {
			get {
				ContentPage contentPage = new ContentPage();

				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT PageID FROM cms_Module WHERE ModuleID = @ModuleID", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
						while (dr.HasRows) {
							dr.Read();
							contentPage = new ContentPage(dr.GetGuid(0));
						}
						cmd.Connection.Close();
					}
				}
				return contentPage;
			}
		}
		public int SortOrder { get; set; }
		public string VirtualPath { get; set; }
		#endregion
	}
	public class PageModules {
		private Guid _PageID = Guid.Empty;

		public PageModules() { }
		public PageModules(Guid PageID) {
			_PageID = PageID;
		}
		public static List<PageModule> GetStandardModules() {
			List<PageModule> pm = new List<PageModule>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleTypeName, ModuleTypeID FROM [cms_ModuleType] ORDER BY ModuleTypeName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						pm.Add(new PageModule() { ModuleID = dr.GetGuid(1), ModuleName = dr[0].ToString() });
					}
					cmd.Connection.Close();
				}
			}
			return pm;
		}
		public List<PageModule> GetPageModules() {
			List<PageModule> pm = new List<PageModule>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT m.ModuleID " +
							"FROM cms_Module m JOIN cms_PageSection s ON m.PageSectionID = s.PageSectionID " +
							"JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
							"WHERE PageID = @PageID ORDER BY s.PageSectionName, m.SortOrder, mt.ModuleTypeName, ModuleName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						pm.Add(new PageModule(dr.GetGuid(0)));
					}
					cmd.Connection.Close();
				}
			}
			return pm;
		}
	}
	public class PageModuleViewModel {
		public Guid ModuleID { get; set; }
		public Guid PageID { get; set; }
		public Guid PageSectionID { get; set; }
		public Guid ModuleTypeID { get; set; }
		public string ModuleName { get; set; }
		public int SortOrder { get; set; }
	}

	public class PageSection {
		#region Fields
		private Guid _PageSectionID = Guid.Empty;
		private Guid _PageTypeID = Guid.Empty;
		private string _PageSectionName = "";
		#endregion

		#region Constructor
		public PageSection() { }
		public PageSection(Guid PageSectionID) {
			_PageSectionID = PageSectionID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT PageTypeID, PageSectionName " +
							"FROM cms_PageSection " +
							"WHERE PageSectionID = @PageSectionID";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = _PageSectionID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_PageTypeID = dr.GetGuid(0);
						_PageSectionName = dr[1].ToString();
					} else {
						_PageSectionID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid PageSectionID {
			get { return _PageSectionID; }
			set { _PageSectionID = value; }
		}
		public Guid PageTypeID {
			get { return _PageTypeID; }
			set { _PageTypeID = value; }
		}
		public string PageSectionName {
			get { return _PageSectionName; }
			set { _PageSectionName = value; }
		}
		#endregion
	}
	public class PageSections {
		private Guid _PageTypeID = Guid.Empty;

		public PageSections() { }
		public PageSections(Guid PageTypeID) {
			_PageTypeID = PageTypeID;
		}
		public List<PageSection> GetPageTypeSections() {
			List<PageSection> ps = new List<PageSection>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT PageSectionID, PageSectionName " +
							"FROM cms_PageSection " +
							"WHERE PageTypeID = @PageTypeID ORDER BY PageSectionName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageTypeID", SqlDbType.UniqueIdentifier).Value = _PageTypeID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						ps.Add(new PageSection { PageSectionID = dr.GetGuid(0), PageSectionName = dr[1].ToString() });
					}
					cmd.Connection.Close();
				}
			}
			return ps;
		}
	}

	public class Locale {
		public string LocaleName { get; set; }
		public string Language { get; set; }
	}
	public class Locales {
		public static List<Locale> GetStandardLocales() {
			List<Locale> pm = new List<Locale>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Locale, Language FROM [cms_Language] ORDER BY Language", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						pm.Add(new Locale() { LocaleName = dr.GetString(0), Language = dr.GetString(1) });
					}
					cmd.Connection.Close();
				}
			}
			return pm;
		}

	}

	public class ContentPageBasicViewModel {
		#region Constructor
		public ContentPageBasicViewModel() { }
		public ContentPageBasicViewModel(ContentPage contentPage) {
			IncludeInSitemap = contentPage.IncludeInSitemap;
			Locale = contentPage.Locale;
			PageID = contentPage.PageID;
			PageTitle = contentPage.seo.MetaTitle;
			PageTypeID = contentPage.PageTypeID;
			VirtualPath = contentPage.VirtualPath;
		}
		#endregion

		#region Properties
		public bool IncludeInSitemap { get; set; }
		public string Locale { get; set; }
		public Guid PageID { get; set; }
		public string PageTitle { get; set; }
		public Guid PageTypeID { get; set; }
		public string VirtualPath { get; set; }
		#endregion
	}

	public class SelectItem {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}