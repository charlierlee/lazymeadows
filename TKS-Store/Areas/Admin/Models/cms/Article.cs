using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace TKS.Areas.Admin.Models.CMS {
	public class Article {
		#region Fields
		private int _ArticleSerial = 0;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		private int _ArticleCategorySerial = 0;
		private string _ArticleTitle = "";
		private string _ShortDescription = "";
		private string _Content = "";
		private int _SortOrder = 0;
		#endregion

		#region Constructor
		public Article(int ArticleSerial) {
			_ArticleSerial = ArticleSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID, Locale, ArticleCategorySerial, ArticleTitle, ShortDescription, Content, SortOrder FROM cms_Article WHERE ArticleSerial = @ArticleSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_ArticleCategorySerial = dr.IsDBNull(2) ? 0 : dr.GetInt32(2);
						_ArticleTitle = dr[3].ToString();
						_ShortDescription = dr[4].ToString();
						_Content = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
					} else {
						_ArticleSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_Article WHERE ArticleSerial = @ArticleSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public int ArticleSerial { get { return _ArticleSerial; } }
		public int ArticleCategorySerial {
			get { return _ArticleCategorySerial; }
			set {
				if (_ArticleCategorySerial != value) {
					_ArticleCategorySerial = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Article SET ArticleCategorySerial = @Value WHERE ArticleSerial = @ArticleSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;
							cmd.Parameters.Add("Value", SqlDbType.Int).Value = value > 0 ? value : SqlInt32.Null;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string ArticleTitle {
			get { return _ArticleTitle; }
			set {
				if (_ArticleTitle != value) {
					_ArticleTitle = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Article SET ArticleTitle = @Value WHERE ArticleSerial = @ArticleSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string ShortDescription {
			get { return _ShortDescription; }
			set {
				if (_ShortDescription != value) {
					_ShortDescription = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Article SET ShortDescription = @Value WHERE ArticleSerial = @ArticleSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string Content {
			get { return _Content; }
			set {
				if (_Content != value) {
					_Content = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Article SET Content = @Value WHERE ArticleSerial = @ArticleSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Article SET SortOrder = @Value WHERE ArticleSerial = @ArticleSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleSerial", SqlDbType.Int).Value = _ArticleSerial;
							cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		#endregion
	}
	public class ArticleSet {
		#region Fields
		private Guid _PageID = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public ArticleSet(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		public ArticleSet(Guid PageID, string ModuleName, string Locale) {
			_PageID = PageID;
			_Locale = Locale;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID FROM cms_Module WHERE PageID = @PageID AND ModuleName = @ModuleName", cn)) {
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("ModuleName", SqlDbType.VarChar, 50).Value = ModuleName;
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void Add(ArticleViewModel articleViewModel) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_Article (ModuleID, Locale, ArticleCategorySerial, ArticleTitle, ShortDescription, Content, SortOrder) VALUES (@ModuleID, @Locale, @ArticleCategorySerial, @ArticleTitle, @ShortDescription, @Content, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("ArticleCategorySerial", SqlDbType.Int).Value = articleViewModel.ArticleCategorySerial > 0 ? articleViewModel.ArticleCategorySerial : SqlInt32.Null;
					cmd.Parameters.Add("ArticleTitle", SqlDbType.NVarChar).Value = articleViewModel.ArticleTitle;
					cmd.Parameters.Add("ShortDescription", SqlDbType.NVarChar).Value = articleViewModel.ShortDescription;
					cmd.Parameters.Add("Content", SqlDbType.NVarChar).Value = articleViewModel.Content;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = articleViewModel.SortOrder;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public List<ArticleViewModel> Articles() {
			List<ArticleViewModel> l = new List<ArticleViewModel>();

			if (_ModuleID != Guid.Empty) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(b.CategoryName, '') AS CategoryName, b.SortOrder AS ArticleCategorySortOrder, " + 
															"a.ArticleSerial, a.ArticleTitle, a.SortOrder " + 
															"FROM cms_Article a LEFT JOIN cms_ArticleCategory b ON a.ArticleCategorySerial = b.ArticleCategorySerial " + 
															"WHERE a.ModuleID = @ModuleID AND a.Locale = @Locale " +
															"ORDER BY b.SortOrder, b.CategoryName, a.SortOrder, a.ArticleTitle", cn)) {
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							ArticleViewModel art = new ArticleViewModel();
							art.ArticleCategory = dr.GetString(0);
							art.SortOrder = dr.GetInt32(4);
							art.ArticleSerial = dr.GetInt32(2);
							art.ArticleTitle = dr.GetString(3);

							l.Add(art);
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		#endregion

		#region Properties
		#endregion
	}
	public class ArticleViewModel {
		public int ArticleSerial { get; set; }
		public int ArticleCategorySerial { get; set; }
		public string ArticleCategory { get; set; }
		public string ArticleTitle { get; set; }
		public string ShortDescription { get; set; }
		public string Content { get; set; }
		public int SortOrder { get; set; }
	}

	public class ArticleCategory {
		#region Fields
		private int _ArticleCategorySerial = 0;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		private string _CategoryName = "";
		private int _SortOrder = 0;
		#endregion

		#region Constructor
		public ArticleCategory() { }
		public ArticleCategory(int ArticleCategorySerial) {
			_ArticleCategorySerial = ArticleCategorySerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID, Locale, CategoryName, SortOrder FROM cms_ArticleCategory WHERE ArticleCategorySerial = @ArticleCategorySerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ArticleCategorySerial", SqlDbType.Int).Value = _ArticleCategorySerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_CategoryName = dr.GetString(2);
						_SortOrder = dr.GetInt32(3);
					} else {
						_ArticleCategorySerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_Article SET ArticleCategorySerial = NULL WHERE ArticleCategorySerial = @ArticleCategorySerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ArticleCategorySerial", SqlDbType.Int).Value = _ArticleCategorySerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "DELETE FROM cms_ArticleCategory WHERE ArticleCategorySerial=@ArticleCategorySerial";
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public int ArticleCategorySerial { get { return _ArticleCategorySerial; } }
		public string CategoryName {
			get { return _CategoryName; }
			set {
				if (_CategoryName != value) {
					_CategoryName = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_ArticleCategory SET CategoryName = @value WHERE ArticleCategorySerial = @ArticleCategorySerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleCategorySerial", SqlDbType.Int).Value = _ArticleCategorySerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_ArticleCategory SET SortOrder = @Value WHERE ArticleCategorySerial = @ArticleCategorySerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("ArticleCategorySerial", SqlDbType.Int).Value = _ArticleCategorySerial;
							cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		#endregion	
	}
	public class ArticleCategories {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public ArticleCategories(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		#endregion

		#region Private Methods
		#endregion

		#region Public Methods
		public void Add(ArticleCategoryViewModel articleViewModel) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_ArticleCategory (ModuleID, Locale, CategoryName, SortOrder) VALUES (@ModuleID, @Locale, @CategoryName, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("CategoryName", SqlDbType.NVarChar).Value = articleViewModel.CategoryName;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = articleViewModel.SortOrder;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public List<ArticleCategoryViewModel> Categories() {
			List<ArticleCategoryViewModel> l = new List<ArticleCategoryViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ArticleCategorySerial, CategoryName, SortOrder FROM cms_ArticleCategory WHERE ModuleID = @ModuleID AND Locale = @Locale ORDER BY SortOrder, CategoryName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						ArticleCategoryViewModel cat = new ArticleCategoryViewModel();
						cat.ArticleCategorySerial = dr.GetInt32(0);
						cat.ModuleID = _ModuleID;
						cat.CategoryName = dr.GetString(1);
						cat.SortOrder = dr.GetInt32(2);
						l.Add(cat);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion
	}

	public class ArticleCategoryViewModel {
		public int ArticleCategorySerial { get; set; }
		public Guid ModuleID { get; set; }
		public string Locale { get; set; }
		public string CategoryName { get; set; }
		public int SortOrder { get; set; }
	}
}