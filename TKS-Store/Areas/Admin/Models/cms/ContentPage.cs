using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;
using System.Web.Mvc;

namespace TKS.Areas.Admin.Models.CMS{
	public class ContentPages {
		public static Guid Add(int PageTypeID, string VirtualPath, bool IncludeInSitemap) {
			Guid PageID = Guid.NewGuid();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string sql = "INSERT INTO cms_Page(PageID, PageTypeID, VirtualPath, IncludeInSitemap) " +
										 "VALUES (@PageID, @PageTypeID, @VirtualPath, @IncludeInSitemap)";
				using (SqlCommand cmd = new SqlCommand(sql, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("@PageTypeID", SqlDbType.Int).Value = PageTypeID;
					cmd.Parameters.Add("@VirtualPath", SqlDbType.VarChar, 250).Value = VirtualPath;
					cmd.Parameters.Add("@IncludeInSitemap", SqlDbType.Bit).Value = IncludeInSitemap ? 1 : 0;

					cmd.Connection.Open();
					cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return PageID;
		}
		public static Guid Add(ContentPageViewModel contentPage) {
			Guid PageID = Guid.NewGuid();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string sql = "INSERT INTO cms_Page(PageID, Locale, PageTypeID, VirtualPath, IncludeInSitemap) " +
										 "VALUES (@PageID, @Locale, @PageTypeID, @VirtualPath, @IncludeInSitemap)";
				using (SqlCommand cmd = new SqlCommand(sql, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("@Locale", SqlDbType.VarChar, 10).Value = contentPage.Locale;
					cmd.Parameters.Add("@PageTypeID", SqlDbType.UniqueIdentifier).Value = contentPage.PageTypeID;
					cmd.Parameters.Add("@VirtualPath", SqlDbType.VarChar, 250).Value = contentPage.VirtualPath;
					cmd.Parameters.Add("@IncludeInSitemap", SqlDbType.Bit).Value = contentPage.IncludeInSitemap ? 1 : 0;

					cmd.Connection.Open();
					cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return PageID;
		}

		public static List<ContentPageViewModel> Pages() {
			List<ContentPageViewModel> l = new List<ContentPageViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT PageID FROM [cms_Page] WHERE IsActive = 1 ORDER BY VirtualPath", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentPageViewModel(new ContentPage(dr.GetGuid(0))));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
	}

	public class ContentPage {
		#region Fields
		private Audit _Audit;
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
						_PageTypeID = dr.GetGuid(1);
						_PageID = dr.GetGuid(2);
						_IncludeInSitemap = dr.GetBoolean(3);
						_PageTypeName = dr[4].ToString();
						_IsActive = dr.GetBoolean(5);
					} else {
						_VirtualPath = "";
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void CheckRedirect() {
			_RedirectURL = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT RedirectTo FROM [cms_Redirect] WHERE RedirectFrom = @RedirectFrom", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("RedirectFrom", SqlDbType.VarChar, 500).Value = _VirtualPath;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_RedirectURL = dr[0].ToString();
					}
					cmd.Connection.Close();
				}
			}
		}
		//private string GetDocumentList() {
		//	List<Document> documents = new DocumentSet(PageID, "DocumentList", Locale).Documents();
		//	StringBuilder sb = new StringBuilder();
		//	string LastCategory = "";
		//	foreach (Document doc in documents) {
		//		if (LastCategory != doc.DocumentCategory) {
		//			LastCategory = doc.DocumentCategory;
		//			sb.AppendLine(string.Format("<h2>{0}</h2>", doc.DocumentCategory));
		//		}
		//		if (!string.IsNullOrEmpty(doc.DocumentTitle)) {
		//			sb.AppendLine(string.Format("<h3>{0}</h3>", doc.DocumentTitle));
		//		}
		//		sb.AppendLine(string.Format("<div>{0}</div>", doc.Description));
		//		sb.AppendLine(string.Format("<div>{0}</div>", doc.Link));
		//		sb.AppendLine("<hr />");
		//	}
		//	return sb.ToString();
		//}
		//private string GetFAQ() {
		//	StringBuilder sb = new StringBuilder();
		//	StringBuilder sb2 = new StringBuilder();
		//	string lastCategory = "";
		//	int Hdr = 1;
		//	int Q = 1;
		//	using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
		//		string SQL = "SELECT c.FAQCategory, f.Question, f.Answer FROM cms_FAQ f JOIN cms_Module m ON f.ModuleID = m.ModuleID LEFT JOIN cms_FAQCategory c ON f.FAQCategorySerial = c.FAQCategorySerial WHERE m.PageID = @PageID AND m.ModuleName = 'FAQ' AND f.Locale = @Locale ORDER BY c.SortOrder, c.FAQCategory, f.SortOrder, f.Question";

		//		using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
		//			cmd.CommandType = CommandType.Text;
		//			cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
		//			cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = "en-US";
		//			cmd.Connection.Open();

		//			SqlDataReader dr = cmd.ExecuteReader();
		//			if (dr.HasRows) {
		//				//sb.AppendLine("<dl>");
		//				while (dr.Read()) {
		//					if (lastCategory != dr[0].ToString()) {
		//						if (sb.Length > 0) {
		//							sb.AppendLine("</p>");
		//							sb2.AppendLine("</dl>");
		//						}
		//						sb.AppendLine("<h2>" + dr[0].ToString() + "</h2><p class='noindent'>");
		//						sb2.AppendLine("<dl><h2 id='faqHdr" + Hdr.ToString() + "'>" + dr[0].ToString() + "</h2>");
		//						lastCategory = dr[0].ToString();
		//						Hdr++;
		//					}
		//					sb.AppendLine("<a href='#FAQ" + Q.ToString() + "'>" + dr[1].ToString() + "</a><br />");
		//					sb2.AppendLine("<dt id='FAQ" + Q.ToString() + "'>" + dr[1].ToString() + "</dt>");
		//					sb2.AppendLine("<dd>" + dr[2].ToString() + "</dd>");
		//					Q++;
		//				}
		//				sb.AppendLine("</p>");
		//			}
		//			cmd.Connection.Close();
		//		}
		//	}

		//	return sb.ToString() + sb2.ToString();

		//}
		//private string GetNews() {
		//	StringBuilder sb = new StringBuilder();
		//	NewsSet newsSet = new NewsSet(new Guid("64d6ad9c-84d0-4a47-9465-3e3c04db87d3"));
		//	foreach (NewsModel news in newsSet.News()) {
		//		string link = "/support/news/" + news.NewsSerial + "/" + tksUtil.FormatRouteURL(news.Headline);

		//		sb.AppendLine("<div class='ym-grid'>");
		//		sb.AppendLine("<div class='ym-g75 ym-gl'>");
		//		sb.AppendLine("<h2><a href='" + link + "'>" + news.Headline + "</a></h2>");
		//		sb.AppendLine("</div>");
		//		sb.AppendLine("<div class='ym-g25 ym-gr blog-date'>");
		//		sb.AppendLine("<span>" + news.DateReleased.ToShortDateString() + "</span>");
		//		sb.AppendLine("</div>");
		//		sb.AppendLine("</div>");
		//		sb.AppendLine("<div class='blog-intro'>");
		//		sb.AppendLine(news.ShortDescription);
		//		sb.AppendLine("<a href='" + link + "'>[read&nbsp;more]</a></p>");
		//		sb.AppendLine("</div>");
		//		sb.AppendLine("<hr />");
		//	}
		//	return sb.ToString();
		//}
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_Page SET IsActive = 0 WHERE PageID = @PageID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void AddModule(ModuleViewModel data) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand()) {
					cmd.Connection = cn;
					cmd.CommandType = CommandType.Text;

					cmd.CommandText = "INSERT cms_Module (PageID, PageSectionID, ModuleTypeID, ModuleName, SortOrder) " +
						"VALUES (@PageID, @PageSectionID, @ModuleTypeID, @ModuleName, @SortOrder)";
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = data.PageSectionID;
					cmd.Parameters.Add("ModuleTypeID", SqlDbType.UniqueIdentifier).Value = data.ModuleTypeID;
					cmd.Parameters.Add("ModuleName", SqlDbType.VarChar, 50).Value = data.ModuleName ?? "";
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = data.SortOrder;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public string GetModule(Guid ModuleID) {
			string ret = "";
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
						if (dr[0].ToString().ToLower() == "slideshow") {
							//ret = new ContentSlideshow(ModuleID, "en-US").SlideItems();
						} else if (dr[0].ToString().ToLower() == "image") {
							string imageTagFormatted = dr[1].ToString();
							if (!string.IsNullOrEmpty(imageTagFormatted)) {
								try {
									ImageSaveModel imageSaveModel = new ImageSaveModel();
									System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
									doc.LoadXml(imageTagFormatted);
									System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(doc.DocumentElement);
									System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
									imageSaveModel = (ImageSaveModel)ser.Deserialize(reader);

									ret = string.Format("<img alt=\"{0}\" src='{1}' height='{2}' width='{3}' />", imageSaveModel.AltText, imageSaveModel.Src, imageSaveModel.Height, imageSaveModel.Width);
								} catch {
								}
							}
						} else if (dr[0].ToString().ToLower() == "testimonials") {
							List<Testimonial> testimonialList = new TestimonialSet().TestimonialsRandom();
							StringBuilder sb = new StringBuilder();
							foreach (Testimonial testimonial in testimonialList) {
								sb.AppendLine(testimonial.Content);
								sb.AppendLine(testimonial.ReceivedFrom);
								sb.AppendLine("<hr />");
							}
							ret = sb.ToString();
						} else if (dr[0].ToString().ToLower() == "documentlist") {
							//ret = GetDocumentList();
						} else if (dr[0].ToString().ToLower() == "faq") {
							//ret = GetFAQ();
						} else if (dr[0].ToString().ToLower() == "news") {
							//ret = GetNews();
						} else {
							ret = dr[1].ToString();
						}
					}
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		public string GetSection(Guid SectionID) {
			return GetSection(_PageID, SectionID);
		}
		public string GetSection(Guid PageID, Guid SectionID) {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT ModuleID " +
							"	FROM cms_Module " +
							"	WHERE PageSectionID = @PageSectionID" +
							"	AND PageID = @PageID " +
							"	ORDER BY SortOrder, ModuleName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = SectionID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						string moduleContents = GetModule(dr.GetGuid(0));
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
		public void RemoveModule(Guid ModuleID) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand()) {
					cmd.Connection = cn;
					cmd.CommandType = CommandType.Text;

					cmd.CommandText = "DELETE cms_Content WHERE ModuleID = @ModuleID";
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = ModuleID;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "DELETE cms_Slideshow WHERE ModuleID = @ModuleID";
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "DELETE cms_GalleryPhoto WHERE ModuleID = @ModuleID";
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "DELETE cms_News WHERE ModuleID = @ModuleID";
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "DELETE cms_Module WHERE ModuleID = @ModuleID";
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Audit Audit {
			get {
				if (_Audit == null) {
					_Audit = new Audit(this);
				}
				return _Audit;
			}
		}
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
		public List<PageSection> Sections {
			get {
				return new PageSections(PageID).GetSections();
			}
		}
		//public List<ContentPageType> ContentPageTypeOptions {
		//	get {
		//		return ContentPageTypes.PageTypes();
		//	}
		//}
		//public virtual ICollection<PageModule> Modules {
		//	get {
		//		PageModules pm = new PageModules(_PageID);
		//		return (ICollection<PageModule>)pm.GetPageModules();
		//	}
		//}
		#endregion
	}
	public class ContentPageViewModel {
		#region Constructor
		public ContentPageViewModel() { }
		public ContentPageViewModel(ContentPage data) {
			IncludeInSitemap = data.IncludeInSitemap;
			Locale = data.Locale;
			PageID = data.PageID;
			PageTitle = data.seo.MetaTitle;
			PageTypeID = data.PageTypeID;
			VirtualPath = data.VirtualPath;
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

	public class PageSections {
		#region Fields
		private Guid _PageID = Guid.Empty;
		#endregion

		#region Constructor
		public PageSections(Guid PageID) {
			_PageID = PageID;
		}
		#endregion

		#region Public Methods
		public List<PageSection> GetSections() {
			List<PageSection> l = new List<PageSection>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT DISTINCT s.PageSectionID,  s.PageSectionName " +
							"FROM cms_Module m JOIN cms_PageSection s ON m.PageSectionID = s.PageSectionID " +
							"WHERE PageID = @PageID ORDER BY s.PageSectionName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new PageSection(_PageID, dr.GetGuid(0)));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		#endregion
	}

	public class PageSection {
		#region Fields
		private Guid _PageID = Guid.Empty;
		private Guid _PageSectionID = Guid.Empty;
		private Guid _PageTypeID = Guid.Empty;
		private string _PageSectionName = "";
		#endregion

		#region Constructor
		public PageSection(Guid PageID, Guid PageSectionID) {
			_PageID = PageID;
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
		public Guid PageID {
			get { return _PageID; }
			set { _PageID = value; }
		}
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
		public List<PageModule> Modules {
			get { return new SectionModules(PageID, PageSectionID).GetSectionModules(); }
		}
		#endregion
	}

	public class PageModule {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private int _SortOrder = 0;
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
						_SortOrder = dr.GetInt32(5);
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

		#region Private Methods
		private void UpdateIntProperty(string FieldName, int Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Module] SET " + FieldName + " = @Value WHERE ModuleID = @ModuleID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = ModuleID;
					cmd.Parameters.Add("Value", SqlDbType.Int).Value = Value;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		private string UpdateStringProperty(string FieldName, int Length, string Value) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Module] SET " + FieldName + " = @Value WHERE ModuleID = @ModuleID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = ModuleID;
					if (Length == Int32.MaxValue) {
						if (Value == null) {
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
						} else {
							ret = Value.Trim();
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
						}
					} else {
						if (Value == null) {
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = SqlString.Null;
						} else {
							if (Value.Trim().Length > Length) {
								ret = Value.Trim().Substring(0, Length);
							} else {
								ret = Value.Trim();
							}
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = ret;
						}
					}
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
			return ret;
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
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					UpdateIntProperty("SortOrder", value);
				}
			}
		}
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
	public class SectionModules {
		private Guid _PageID = Guid.Empty;
		private Guid _PageSectionID = Guid.Empty;

		public SectionModules(Guid PageID, Guid PageSectionID) {
			_PageID = PageID;
			_PageSectionID = PageSectionID;
		}
		public List<PageModule> GetSectionModules() {
			List<PageModule> pm = new List<PageModule>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT m.ModuleID " +
							"FROM cms_Module m  JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
							"WHERE PageID = @PageID AND PageSectionID = @PageSectionID ORDER BY m.SortOrder, mt.ModuleTypeName, ModuleName";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
					cmd.Parameters.Add("PageSectionID", SqlDbType.UniqueIdentifier).Value = _PageSectionID;

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
	public class ModuleViewModel {
		public Guid ModuleID { get; set; }
		public Guid PageID { get; set; }
		public Guid PageSectionID { get; set; }
		public Guid ModuleTypeID { get; set; }
		public string ModuleName { get; set; }
		public int SortOrder { get; set; }
	}

	//Drop-down classes
	public class ContentPageTypes {
		public List<SelectListItem> GetSelectList(Guid SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT PageTypeID, PageTypeName FROM cms_PageType ORDER BY PageTypeName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						itemList.Add(new SelectListItem {
							Value = dr[0].ToString(),
							Text = dr[1].ToString(),
							Selected = (dr.GetGuid(0) == SelectedID)
						});
					}
					cmd.Connection.Close();
				}
			}
			return itemList;
		}
	}

	public class Sections {
		private Guid _PageTypeID = Guid.Empty;

		//public Sections() { }
		public Sections(Guid PageTypeID) {
			_PageTypeID = PageTypeID;
		}
		public List<SelectListItem> GetSelectList(Guid SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT PageSectionID, PageSectionName FROM cms_PageSection WHERE PageTypeID = @PageTypeID ORDER BY PageSectionName", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PageTypeID", SqlDbType.UniqueIdentifier).Value = _PageTypeID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							itemList.Add(new SelectListItem {
								Value = dr[0].ToString(),
								Text = dr[1].ToString(),
								Selected = (dr.GetGuid(0) == SelectedID)
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return itemList;
		}
		//public List<SectionViewModel> GetPageTypeSections() {
		//	List<SectionViewModel> l = new List<SectionViewModel>();

		//	using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
		//		using (SqlCommand cmd = new SqlCommand("SELECT PageSectionID, PageSectionName FROM cms_PageSection WHERE PageTypeID = @PageTypeID ORDER BY PageSectionName", cn)) {
		//			cmd.CommandType = CommandType.Text;
		//			cmd.Parameters.Add("PageTypeID", SqlDbType.UniqueIdentifier).Value = _PageTypeID;

		//			cmd.Connection.Open();
		//			SqlDataReader dr = cmd.ExecuteReader();
		//			while (dr.Read()) {
		//				l.Add(new SectionViewModel { PageSectionID = dr.GetGuid(0), PageSectionName = dr[1].ToString() });
		//			}
		//			cmd.Connection.Close();
		//		}
		//	}
		//	return l;
		//}
	}

	public class Modules {
		public List<SelectListItem> GetStandardModuleSelectList(Guid SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleTypeID, ModuleTypeName FROM [cms_ModuleType] ORDER BY ModuleTypeName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						itemList.Add(new SelectListItem {
							Value = dr[0].ToString(),
							Text = dr[1].ToString(),
							Selected = (dr.GetGuid(0) == SelectedID)
						});
					}
					cmd.Connection.Close();
				}
			}
			return itemList;
		}
	}

	public class Locales {
		public List<SelectListItem> GetSelectList(string SelectedID = "") {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT Locale, Language FROM [cms_Language] ORDER BY Language", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							itemList.Add(new SelectListItem {
								Value = dr[0].ToString(),
								Text = dr[1].ToString(),
								Selected = (dr[0].ToString() == SelectedID)
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return itemList;
		}
	}
}