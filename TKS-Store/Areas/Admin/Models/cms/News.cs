using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace TKS.Areas.Admin.Models.CMS {
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

		#region Private Methods
		private void UpdateIndex() {
			// Update Lucene search index
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			li.UpdateWebPage(NewsID.ToString(), URL, Headline, Content ?? "", "News");
			li.Close();
			li.IndexWords();
		}

		private void UpdateBoolProperty(string FieldName, bool Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_News] SET " + FieldName + " = @Value WHERE NewsSerial = @NewsSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("NewsSerial", SqlDbType.Int).Value = NewsSerial;
					cmd.Parameters.Add("Value", SqlDbType.Bit).Value = Value ? 1 : 0;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		private void UpdateDateProperty(string FieldName, DateTime Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_News] SET " + FieldName + " = @Value WHERE NewsSerial = @NewsSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("NewsSerial", SqlDbType.Int).Value = NewsSerial;
					if (Value == DateTime.MaxValue || Value == DateTime.MinValue) {
						cmd.Parameters.Add("Value", SqlDbType.DateTime).Value = SqlDateTime.Null;
					} else {
						cmd.Parameters.Add("Value", SqlDbType.DateTime).Value = Value;
					}
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

		}
		private string UpdateStringProperty(string FieldName, int Length, string Value) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_News] SET " + FieldName + " = @Value WHERE NewsSerial = @NewsSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("NewsSerial", SqlDbType.Int).Value = NewsSerial;
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

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_News WHERE NewsSerial = @NewsSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@NewsSerial", SqlDbType.Int).Value = _NewsSerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			// Remove from Lucene search index
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			li.Delete(NewsID.ToString());
			li.Close();
		}
		#endregion

		#region Properties
		public string AttachedArticle {
			get { return _AttachedArticle; }
			set {
				if (_AttachedArticle != value) {
					_AttachedArticle = value;
					UpdateStringProperty("AttachedArticle", Int32.MaxValue, value);
				}
			}
		}
		public string Content {
			get { return _Content; }
			set {
				if (_Content != value) {
					_Content = value;
					UpdateStringProperty("Content", Int32.MaxValue, value);
					UpdateIndex();
				}
			}
		}
		public DateTime DateReleased {
			get { return _DateReleased; }
			set {
				if (_DateReleased != value) {
					_DateReleased = value;
					UpdateDateProperty("DateReleased", value);
				}
			}
		}
		public bool IsFeatured {
			get { return _IsFeatured; }
			set {
				if (_IsFeatured != value) {
					_IsFeatured = value;
					UpdateBoolProperty("IsFeatured", value);
				}
			}
		}
		public bool IsPublished {
			get { return _IsPublished; }
			set {
				if (_IsPublished != value) {
					_IsPublished = value;
					UpdateBoolProperty("IsPublished", value);
				}
			}
		}
		public string HeaderImage {
			get { return _HeaderImage; }
			set {
				if (_HeaderImage != value) {
					_HeaderImage = value;
					UpdateStringProperty("HeaderImage", Int32.MaxValue, value);
				}
			}
		}
		public string Headline {
			get { return _Headline; }
			set {
				if (_Headline != value) {
					_Headline = value;
					UpdateStringProperty("Headline", Int32.MaxValue, value);
					UpdateIndex();
				}
			}
		}
		public string LinkedArticle {
			get { return _LinkedArticle; }
			set {
				if (_LinkedArticle != value) {
					_LinkedArticle = value;
					UpdateStringProperty("LinkedArticle", Int32.MaxValue, value);
				}
			}
		}
		public Guid ModuleID { get { return _ModuleID; } }
		public Guid NewsID { get { return _NewsID; } }
		public int NewsSerial { get { return _NewsSerial; } }
		public string ShortDescription {
			get { return _ShortDescription; }
			set {
				if (_ShortDescription != value) {
					_ShortDescription = value;
					UpdateStringProperty("ShortDescription", Int32.MaxValue, value);
					UpdateIndex();
				}
			}
		}
		public string URL { get { return _URL; } }
		public SEO seo { get { return new SEO(NewsID); } }
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
		private Guid _ModuleID = Guid.Empty;
		#endregion

		#region Constructor
		public NewsSet() { }
		public NewsSet(Guid ModuleID) {
			_ModuleID = ModuleID;
		}
		#endregion

		#region Public Methods
		public int Add(NewsModel news) {
			int NewsSerial = 0;
			if (_ModuleID != Guid.Empty) {
				Guid NewsID = Guid.NewGuid();
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "INSERT INTO cms_News (NewsID, ModuleID, Headline, HeaderImage, ShortDescription, Content, AttachedArticle, LinkedArticle, IsFeatured, IsPublished, DateReleased) " +
									"VALUES (@NewsID, @ModuleID, @Headline, @HeaderImage, @ShortDescription, @Content, @AttachedArticle, @LinkedArticle, @IsFeatured, @IsPublished, @DateReleased)";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@NewsID", SqlDbType.UniqueIdentifier).Value = NewsID;
						cmd.Parameters.Add("@ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("@Headline", SqlDbType.NVarChar).Value = news.Headline;
						cmd.Parameters.Add("@HeaderImage", SqlDbType.NVarChar).Value = news.HeaderImage ?? SqlString.Null;
						cmd.Parameters.Add("@ShortDescription", SqlDbType.NVarChar).Value = news.ShortDescription ?? SqlString.Null;
						cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = news.Content ?? SqlString.Null;
						cmd.Parameters.Add("@AttachedArticle", SqlDbType.NVarChar).Value = news.AttachedArticle ?? SqlString.Null;
						cmd.Parameters.Add("@LinkedArticle", SqlDbType.NVarChar).Value = news.LinkedArticle ?? SqlString.Null;
						cmd.Parameters.Add("@IsFeatured", SqlDbType.Bit).Value = news.IsFeatured ? 1 : 0;
						cmd.Parameters.Add("@IsPublished", SqlDbType.Bit).Value = news.IsPublished ? 1 : 0;
						cmd.Parameters.Add("@DateReleased", SqlDbType.Date).Value = news.DateReleased;

						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
				}

				// Update Lucene search index
				News newNews = new News(NewsID);
				NewsSerial = newNews.NewsSerial;
				Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
				li.CreateIndexWriter();
				li.UpdateWebPage(NewsID.ToString(), newNews.URL, newNews.Headline, newNews.Content, "News");
				li.Close();
				li.IndexWords();
			}
			return NewsSerial;
		}

		public List<NewsModel> News(int Count = 0) {
			List<NewsModel> list = new List<NewsModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (_ModuleID == Guid.Empty) {
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
						SQL = "SELECT TOP " + Count.ToString() + " NewsSerial FROM cms_News WHERE ModuleID = @ModuleID ORDER BY DateReleased DESC";
					} else {
						SQL = "SELECT NewsSerial FROM cms_News WHERE ModuleID = @ModuleID ORDER BY DateReleased DESC";
					}
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
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