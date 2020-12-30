using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using TKS.Areas.Admin.Models;

namespace TKS.Models.CMS {
	public class News {
		#region Fields
		private bool _IsFeatured = false;
		private bool _IsPublished = true;
		private DateTime _DateReleased = DateTime.Now;
		private Guid _ModuleID = Guid.Empty;
		private Guid _NewsID = Guid.Empty;
		private int _NewsSerial = 0;
		private string _AttachedArticle = "";
		private string _Content = "";
		private string _HeaderImage = "";
		private string _Headline = "";
		private string _LinkedArticle = "";
		private string _ShortDescription = "";
		private string _URL = "";
		#endregion

		#region Constructor
		public News() { }
		public News(int NewsSerial) {
			_NewsSerial = NewsSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT n.[NewsID], n.[ModuleID], n.[Headline], n.[HeaderImage], n.[ShortDescription], n.[Content], n.[AttachedArticle], n.[LinkedArticle], n.[IsFeatured], n.[IsPublished], n.[DateReleased], " +
							"	p.VirtualPath " +
							"	FROM [cms_News] n JOIN cms_Module m ON n.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID " +
							"	WHERE n.NewsSerial = @NewsSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("NewsSerial", SqlDbType.Int).Value = _NewsSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_NewsID = dr.GetGuid(0);
						_ModuleID = dr.GetGuid(1);
						_Headline = dr[2].ToString();
						_HeaderImage = dr[3].ToString();
						_DateReleased = dr.GetDateTime(10);
						_ShortDescription = dr[4].ToString();
						_Content = dr[5].ToString();
						_AttachedArticle = dr[6].ToString();
						_LinkedArticle = dr[7].ToString();
						_IsFeatured = dr.GetBoolean(8);
						_IsPublished = dr.GetBoolean(9);
						_URL = dr[11].ToString() + "/" + _NewsSerial.ToString() + "/" + tksUtil.FormatRouteURL(_Headline);
					} else {
						_NewsSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public News(Guid NewsID) {
			_NewsID = NewsID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT n.[NewsID], n.[ModuleID], n.[Headline], n.[HeaderImage], n.[ShortDescription], n.[Content], n.[AttachedArticle], n.[LinkedArticle], n.[IsFeatured], n.[IsPublished], n.[DateReleased], " +
							"	p.VirtualPath " +
							"	FROM [cms_News] n JOIN cms_Module m ON n.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID " +
							"	WHERE n.NewsID = @NewsID";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("NewsID", SqlDbType.UniqueIdentifier).Value = _NewsID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_NewsID = dr.GetGuid(0);
						_ModuleID = dr.GetGuid(1);
						_Headline = dr[2].ToString();
						_HeaderImage = dr[3].ToString();
						_DateReleased = dr.GetDateTime(10);
						_ShortDescription = dr[4].ToString();
						_Content = dr[5].ToString();
						_AttachedArticle = dr[6].ToString();
						_LinkedArticle = dr[7].ToString();
						_IsFeatured = dr.GetBoolean(8);
						_IsPublished = dr.GetBoolean(9);
						_URL = dr[11].ToString() + "/" + _NewsSerial.ToString() + "/" + tksUtil.FormatRouteURL(_Headline);
					} else {
						_NewsID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public string AttachedArticle { get { return _AttachedArticle; } }
		public string Content { get { return _Content; } }
		public DateTime DateReleased { get { return _DateReleased; } }
		public bool IsFeatured { get { return _IsFeatured; } }
		public bool IsPublished { get { return _IsPublished; } }
		public string HeaderImage { get { return _HeaderImage; } }
		public string Headline { get { return _Headline; } }
		public string LinkedArticle { get { return _LinkedArticle; } }
		public Guid ModuleID { get { return _ModuleID; } }
		public Guid NewsID { get { return _NewsID; } }
		public int NewsSerial { get { return _NewsSerial; } }
		public string ShortDescription { get { return _ShortDescription; } }
		public string URL { get { return _URL; } }
		public string MetaTags {
			get {
				return new SEO(NewsID).GetTags("/support/news/" + NewsSerial.ToString() + "/" + tksUtil.FormatRouteURL(Headline), Headline, ShortDescription);
			}
		}
		#endregion
	}

	public class NewsModel {
		#region Constructor
		public NewsModel() { }
		public NewsModel(News data) {
			this.AttachedArticle = data.AttachedArticle;
			this.Content = data.Content;
			this.DateReleased = data.DateReleased;
			this.HeaderImage = data.HeaderImage;
			this.Headline = data.Headline;
			this.IsFeatured = data.IsFeatured;
			this.IsPublished = data.IsPublished;
			this.LinkedArticle = data.LinkedArticle;
			this.ModuleID = data.ModuleID;
			this.NewsID = data.NewsID;
			this.NewsSerial = data.NewsSerial;
			this.ShortDescription = data.ShortDescription;
			this.URL = data.URL;
		}
		#endregion

		#region Properties
		public string AttachedArticle { get; set; }
		public string Content { get; set; }
		public DateTime DateReleased { get; set; }
		public string HeaderImage { get; set; }
		public string Headline { get; set; }
		public bool IsFeatured { get; set; }
		public bool IsPublished { get; set; }
		public string LinkedArticle { get; set; }
		public Guid ModuleID { get; set; }
		public Guid NewsID { get; set; }
		public int NewsSerial { get; set; }
		public string ShortDescription { get; set; }
		public string URL { get; set; }
		#endregion
	}

	public class NewsSet {
		#region Fields
		private Guid _PageID = Guid.Empty;
		#endregion

		#region Constructor
		public NewsSet() { }
		public NewsSet(Guid PageID) {
			_PageID = PageID;
		}
		#endregion

		#region Public Methods
		public List<NewsModel> News(int Count = 0) {
			List<NewsModel> list = new List<NewsModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (_PageID == Guid.Empty) {
					if (Count > 0) {
						SQL = "SELECT TOP " + Count.ToString() + " NewsSerial FROM cms_News ORDER BY DateReleased DESC";
					} else {
						SQL = "SELECT NewsSerial FROM cms_News ORDER BY DateReleased DESC";
					}
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							list.Add(new NewsModel(new News(dr.GetInt32(0))));
						}
						cmd.Connection.Close();
					}
				} else {
					if (Count > 0) {
						SQL = "SELECT TOP " + Count.ToString() + " NewsSerial FROM cms_News n JOIN cms_Module m on n.ModuleID = m.ModuleID WHERE m.PageID = @PageID ORDER BY n.DateReleased DESC";
					} else {
						SQL = "SELECT NewsSerial FROM cms_News n JOIN cms_Module m on n.ModuleID = m.ModuleID WHERE m.PageID = @PageID ORDER BY n.DateReleased DESC";
					}
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							list.Add(new NewsModel(new News(dr.GetInt32(0))));
						}
						cmd.Connection.Close();
					}
				}
			}
			return list;
		}
		#endregion
	}
}