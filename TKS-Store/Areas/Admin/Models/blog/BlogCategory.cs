using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web;

namespace TKS.Areas.Admin.Models.Blog {
	public class BlogCategory {
		#region Fields
		private Guid _CategoryID = Guid.Empty;
		private int _CategorySerial = 0;
		private string _Locale = "en-US";
		private Guid _ParentID = Guid.Empty;
		private string _CategoryName = "";
		private string _Description = "";
		#endregion

		#region Constructor
		public BlogCategory() { }
		public BlogCategory(Guid CategoryID) {
			_CategoryID = CategoryID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT CategorySerial, Locale, ParentID, CategoryName, Description FROM cms_BlogCategory WHERE CategoryID = @CategoryID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = _CategoryID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_CategorySerial = dr.GetInt32(0);
						_Locale = dr[1].ToString();
						_ParentID = !dr.IsDBNull(2) ? dr.GetGuid(2) : Guid.Empty;
						_CategoryName = dr[3].ToString();
						_Description = dr[4].ToString();
					} else {
						_CategoryID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateGuidProperty(string FieldName, Guid Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogCategory SET " + FieldName + " = @Value WHERE CategoryID = @CategoryID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = CategoryID;
					if (Value == Guid.Empty) {
						cmd.Parameters.Add("Value", SqlDbType.UniqueIdentifier).Value = SqlGuid.Null;
					} else {
						cmd.Parameters.Add("Value", SqlDbType.UniqueIdentifier).Value = Value;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogCategory SET " + FieldName + " = @Value WHERE CategoryID = @CategoryID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = CategoryID;
					if (Length == Int32.MaxValue) {
						if (Value == null) {
							ret = null;
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
						}
					} else {
						if (Value == null) {
							ret = null;
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							if (ret.Length > Length) {
								ret = ret.Substring(0, Length);
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
				SqlTransaction transaction;
				cn.Open();
				transaction = cn.BeginTransaction();
				try {
					SqlCommand cmd = new SqlCommand("DELETE FROM cms_BlogPost_Category_xref WHERE CategoryID = @CategoryID", cn, transaction);
					cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = _CategoryID;
					cmd.ExecuteNonQuery();
					cmd.CommandText = "DELETE FROM cms_BlogCategory WHERE CategoryID = @CategoryID";
					cmd.ExecuteNonQuery();
					transaction.Commit();
				} catch (SqlException sqlError) {
					transaction.Rollback();
					Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(sqlError));
				}
				cn.Close();
			}
		}
		#endregion

		#region Properties
		public Guid CategoryID { get { return _CategoryID; } }
		public int CategorySerial { get { return _CategorySerial; } }
		public Guid ParentID {
			get { return _ParentID; }
			set {
				if (!_ParentID.Equals(value)) {
					_ParentID = value;
					UpdateGuidProperty("ParentID", value);
				}
			}
		}
		public string CategoryName {
			get { return _CategoryName; }
			set {
				if (!_CategoryName.Equals(value)) {
					_CategoryName = UpdateStringProperty("CategoryName", 50, value);
				}
			}
		}
		public string Description {
			get { return _Description; }
			set {
				if (!_Description.Equals(value)) {
					_Description = UpdateStringProperty("Description", 200, value);
				}
			}
		}
		public int PostCount {
			get {
				int count = 0;
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT COUNT(*) FROM cms_BlogPost_Category_xref c JOIN cms_BlogPost p ON c.PostID = p.PostID " +
						"WHERE CategoryID = @CategoryID AND p.IsDeleted = 0 AND p.IsPublished = 1 AND p.PublishDate <= GETDATE()";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = _CategoryID;

						cmd.Connection.Open();
						count = (int)cmd.ExecuteScalar();
						cmd.Connection.Close();
					}
				}
				return count;
			}
		}
		#endregion
	}
	public class BlogCategories {
		#region Fields
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public BlogCategories(string Locale = "en-us") {
			_Locale = Locale;
		}
		#endregion

		#region Public Methods
		public Guid Add(BlogCategoryViewModel categoryViewModel) {
			Guid CategoryID = Guid.NewGuid();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_BlogCategory (CategoryID, Locale, ParentID, CategoryName, Description) VALUES (@CategoryID, @Locale, @ParentID, @CategoryName, @Description)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = CategoryID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("ParentID", SqlDbType.UniqueIdentifier).Value = categoryViewModel.ParentID != null ? (Guid)categoryViewModel.ParentID : SqlGuid.Null;
					cmd.Parameters.Add("CategoryName", SqlDbType.NVarChar, 50).Value = !string.IsNullOrEmpty(categoryViewModel.CategoryName) ? categoryViewModel.CategoryName : "";
					cmd.Parameters.Add("Description", SqlDbType.NVarChar, 200).Value = !string.IsNullOrEmpty(categoryViewModel.Description) ? categoryViewModel.Description : SqlString.Null;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
			return CategoryID;
		}
		public List<BlogCategoryViewModel> Categories() {
			List<BlogCategoryViewModel> l = new List<BlogCategoryViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT c.CategoryID, CategorySerial, ParentID, CategoryName, [Description], ISNULL(cnt.PostCount, 0) AS PostCount FROM cms_BlogCategory c LEFT JOIN (SELECT CategoryID, COUNT(*) AS PostCount FROM cms_BlogPost_Category_xref GROUP BY CategoryID) cnt ON c.CategoryID = cnt.CategoryID WHERE Locale = @Locale AND ParentID IS NULL ORDER BY CategoryName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						BlogCategoryViewModel cat = new BlogCategoryViewModel();
						cat.CategoryID = dr.GetGuid(0);
						cat.CategorySerial = dr.GetInt32(1);
						if (dr[2].ToString().Length > 0) {
							cat.ParentID = dr.GetGuid(2);
						}
						cat.CategoryName = dr[3].ToString();
						cat.Description = dr[4].ToString();
						cat.PostCount = dr.GetInt32(5);
						cat.Locale = _Locale;

						l.Add(cat);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public List<BlogCategory> UtilizedCategories() {
			List<BlogCategory> l = new List<BlogCategory>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT bc.CategoryID FROM cms_BlogCategory bc JOIN cms_BlogPost_Category_xref x ON bc.CategoryID = x.CategoryID WHERE Locale = @Locale AND ParentID IS NULL ORDER BY CategoryName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogCategory(dr.GetGuid(0)));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion
	}
	public class BlogCategoryViewModel {
		#region Constructor
		public BlogCategoryViewModel() { }
		public BlogCategoryViewModel(BlogCategory data) {
			this.CategoryID = data.CategoryID;
			this.CategoryName = data.CategoryName;
			this.CategorySerial = data.CategorySerial;
			this.Description = data.Description;
			this.ParentID = data.ParentID;
			this.PostCount = data.PostCount;
		}
		#endregion

		#region Public Properties
		public Guid CategoryID { get; set; }
		public int CategorySerial { get; set; }
		public int PostCount { get; set; }
		public string Locale { get; set; }
		public Guid? ParentID { get; set; }
		public string CategoryName { get; set; }
		public string Description { get; set; }
		#endregion
	}

	public class BlogTags {
		#region Constructor
		public BlogTags() { }
		#endregion

		#region Public Methods
		public int Add(BlogTagViewModel data) {
			int PostTagID = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_BlogPostTag (PostID, Tag) OUTPUT Inserted.PostTagID VALUES (@PostID, @Tag)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = data.PostID;
					cmd.Parameters.Add("Tag", SqlDbType.VarChar, 50).Value = data.Tag;

					cmd.Connection.Open();
					PostTagID = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return PostTagID;
		}
		public void Delete(string Tag) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE cms_BlogPostTag WHERE Tag = @Tag", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Tag", SqlDbType.VarChar, 50).Value = Tag;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public List<BlogTagViewModel> Tags() {
			List<BlogTagViewModel> l = new List<BlogTagViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT t.Tag, COUNT(*) AS PostCount FROM cms_BlogPostTag t GROUP BY t.Tag ORDER BY t.Tag", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogTagViewModel {Tag = dr[0].ToString(), TagCount = dr.GetInt32(1)});
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion
	}
	public class BlogTagViewModel {
		public int PostTagID { get; set; }
		public Guid PostID { get; set; }
		public string Tag { get; set; }
		public int TagCount { get; set; }
	}
}