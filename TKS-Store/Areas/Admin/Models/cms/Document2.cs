using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class Document2 {
		#region Fields
		private int _DocumentSerial = 0;
		private Guid _Token = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		private int _DocumentCategorySerial = 0;
		private string _DocumentCategory = "";
		private string _DocumentTitle = "";
		private string _Description = "";
		private int _SortOrder = 0;
		#endregion

		#region Constructor
		public Document2() { }
		public Document2(int DocumentSerial) {
			_DocumentSerial = DocumentSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT d.ModuleID, d.Locale, d.DocumentToken, d.DocumentCategorySerial, d.DocumentTitle, d.Description, d.SortOrder, dc.CategoryName FROM cms_Document d LEFT JOIN cms_DocumentCategory dc ON d.DocumentCategorySerial = dc.DocumentCategorySerial WHERE d.DocumentSerial = @DocumentSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_Token = dr.GetGuid(2);
						_DocumentCategorySerial = dr.IsDBNull(3) ? 0 : dr.GetInt32(3);
						_DocumentTitle = dr[4].ToString();
						_Description = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
						_DocumentCategory = dr[7].ToString();
					} else {
						_DocumentSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public Document2(Guid Token) {
			_Token = Token;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT d.ModuleID, d.Locale, d.DocumentSerial, d.DocumentCategorySerial, d.DocumentTitle, d.Description, d.SortOrder, dc.CategoryName FROM cms_Document d LEFT JOIN cms_DocumentCategory dc ON d.DocumentCategorySerial = dc.DocumentCategorySerial WHERE d.DocumentToken = @Token", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Token", SqlDbType.UniqueIdentifier).Value = _Token;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_DocumentSerial = dr.GetInt32(2);
						_DocumentCategorySerial = dr.IsDBNull(3) ? 0 : dr.GetInt32(3);
						_DocumentTitle = dr[4].ToString();
						_Description = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
						_DocumentCategory = dr[7].ToString();
					} else {
						_Token = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				SqlTransaction transaction;
				cn.Open();
				transaction = cn.BeginTransaction();
				try {
					SqlCommand cmd = new SqlCommand("DELETE FROM cms_DocumentLink WHERE DocumentSerial = @DocumentSerial", cn, transaction);
					cmd.Parameters.Add("@DocumentSerial",SqlDbType.Int).Value = _DocumentSerial;
					cmd.ExecuteNonQuery();
					cmd.CommandText = "DELETE FROM cms_Document WHERE DocumentSerial = @DocumentSerial";
					cmd.ExecuteNonQuery();
					transaction.Commit();
				} catch (SqlException sqlError) {
					transaction.Rollback();
					Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(sqlError));
				}
				cn.Close();
			}
		}
		public List<DocumentLink2> DocumentLinks() {
			List<DocumentLink2> l = new List<DocumentLink2>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DocumentLinkSerial FROM cms_DocumentLink WHERE DocumentSerial = @DocumentSerial ORDER BY LinkText, Filename, DocumentLink", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new DocumentLink2(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public void AddDocumentLink(DocumentLink2 documentLink) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_DocumentLink (DocumentSerial, DocumentLink, FileName, LinkText, DocumentSize, IconFilename) VALUES (@DocumentSerial, @DocumentLink, @FileName, @LinkText, @DocumentSize, @IconFilename)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
					cmd.Parameters.Add("DocumentLink", SqlDbType.NVarChar, 350).Value = documentLink.DocumentLinkRaw.Length > 0 ? documentLink.DocumentLinkRaw : SqlString.Null;
					cmd.Parameters.Add("FileName", SqlDbType.NVarChar, 250).Value = documentLink.Filename.Length > 0 ? documentLink.Filename : SqlString.Null;
					cmd.Parameters.Add("LinkText", SqlDbType.NVarChar, 250).Value = documentLink.LinkText.Length > 0 ? documentLink.LinkText: SqlString.Null;
					cmd.Parameters.Add("DocumentSize", SqlDbType.BigInt).Value = documentLink.Size > 0 ? documentLink.Size : SqlInt64.Null;
					cmd.Parameters.Add("IconFilename", SqlDbType.NVarChar, 250).Value = documentLink.IconFilename.Length > 0 ? documentLink.IconFilename : SqlString.Null; ;
					
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

		}
		#endregion

		#region Properties
		public int DocumentSerial { get { return _DocumentSerial; } }
		public Guid Token { get { return _Token; } }
		public int DocumentCategorySerial {
			get { return _DocumentCategorySerial; }
			set {
				if (_DocumentCategorySerial != value) {
					_DocumentCategorySerial = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Document SET DocumentCategorySerial = @Value WHERE DocumentSerial = @DocumentSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
							cmd.Parameters.Add("Value", SqlDbType.Int).Value = value > 0 ? value : SqlInt32.Null;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string DocumentCategory { get { return _DocumentCategory; } }
		public string DocumentTitle {
			get { return _DocumentTitle; }
			set {
				if (_DocumentTitle != value) {
					_DocumentTitle = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Document SET DocumentTitle = @Value WHERE DocumentSerial = @DocumentSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string Description {
			get { return _Description; }
			set {
				if (_Description != value) {
					_Description = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Document SET Description = @Value WHERE DocumentSerial = @DocumentSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
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
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_Document SET SortOrder = @Value WHERE DocumentSerial = @DocumentSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
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
	public class DocumentSet2 {
		#region Fields
		private Guid _PageID = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public DocumentSet2(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		public DocumentSet2(Guid PageID, string ModuleName, string Locale) {
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
		public Guid Add(DocumentViewModel2 documentViewModel) {
			Guid Token = Guid.NewGuid();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_Document (ModuleID, Locale, DocumentToken, DocumentCategorySerial, DocumentTitle, Description, SortOrder) VALUES (@ModuleID, @Locale, @DocumentToken, @DocumentCategorySerial, @DocumentTitle, @Description, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("DocumentToken", SqlDbType.UniqueIdentifier).Value = Token;
					cmd.Parameters.Add("DocumentCategorySerial", SqlDbType.Int).Value = documentViewModel.DocumentCategorySerial > 0 ? documentViewModel.DocumentCategorySerial : SqlInt32.Null;
					cmd.Parameters.Add("DocumentTitle", SqlDbType.NVarChar).Value = documentViewModel.DocumentTitle;
					cmd.Parameters.Add("Description", SqlDbType.NVarChar).Value = documentViewModel.Description;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = documentViewModel.SortOrder;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
			return Token;
		}
		public List<Document2> Documents() {
			List<Document2> l = new List<Document2>();

			if (_ModuleID != Guid.Empty) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT a.DocumentSerial " + 
															"FROM cms_Document a LEFT JOIN cms_DocumentCategory b ON a.DocumentCategorySerial = b.DocumentCategorySerial " + 
															"WHERE a.ModuleID = @ModuleID AND a.Locale = @Locale " +
															"ORDER BY b.SortOrder, b.CategoryName, a.SortOrder, a.DocumentTitle", cn)) {
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new Document2(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		#endregion
	}
	public class DocumentViewModel2 {
		public int DocumentSerial { get; set; }
		public int DocumentCategorySerial { get; set; }
		public Guid DocumentToken { get; set; }
		public string DocumentCategory { get; set; }
		public string DocumentTitle { get; set; }
		public string Description { get; set; }
		public int SortOrder { get; set; }
	}

	public class DocumentLink2 {
		#region Fields
		private int _DocumentLinkSerial = 0;
		private int _DocumentSerial = 0;
		private Guid _Token = Guid.Empty;
		private string _DocumentLink = "";
		private string _Filename = "";
		private string _IconFilename = "";
		private string _LinkText = "";
		private Int64 _Size = 0;
		private DateTime _DateUploaded = DateTime.MinValue;
		#endregion

		#region Constructor
		public DocumentLink2() { }
		public DocumentLink2(int DocumentLinkSerial) {
			_DocumentLinkSerial = DocumentLinkSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DocumentSerial, DocumentLinkToken, DocumentLink, Filename, LinkText, DocumentSize, IconFilename, DateUploaded FROM cms_DocumentLink WHERE DocumentLinkSerial = @DocumentLinkSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_DocumentSerial = dr.GetInt32(0);
						_Token = dr.GetGuid(1);
						_DocumentLink = dr[2].ToString();
						_Filename = dr[3].ToString();
						_IconFilename = dr[6].ToString();
						_LinkText = dr[4].ToString();
						_Size = dr.IsDBNull(5) ? 0 : dr.GetInt64(5);
						_DateUploaded = dr.GetDateTime(7);
					} else {
						_DocumentLinkSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public DocumentLink2(Guid Token) {
			_Token = Token;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DocumentLinkSerial, DocumentSerial, DocumentLink, Filename, LinkText, DocumentSize, IconFilename, DateUploaded FROM cms_DocumentLink WHERE DocumentToken = @Token", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_DocumentLinkSerial = dr.GetInt32(0);
						_DocumentSerial = dr.GetInt32(1);
						_DocumentLink = dr[2].ToString();
						_Filename = dr[3].ToString();
						_IconFilename = dr[6].ToString();
						_LinkText = dr[4].ToString();
						_Size = dr.IsDBNull(5) ? 0 : dr.GetInt64(5);
						_DateUploaded = dr.GetDateTime(7);
					} else {
						_Token = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_DocumentLink WHERE DocumentLinkSerial = @DocumentLinkSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public string DocumentLinkRaw { 
			get { return _DocumentLink; }
			set {
				if (_DocumentLink != value) {
					_DocumentLink = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_DocumentLink SET DocumentLink = @Value WHERE DocumentLinkSerial = @DocumentLinkSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar, 250).Value = value.Length > 0 ? value : SqlString.Null;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public int DocumentLinkSerial { get { return _DocumentLinkSerial; } }
		public int DocumentSerial { get { return _DocumentSerial; } }
		public Guid Token { get { return _Token; } }
		public string Link {
			get {
				if (string.IsNullOrEmpty(LinkDestination)) {
					return LinkText;
				} else {
					return string.Format("<a href='{0}'>{1}</a>", LinkDestination, LinkText);
				}
			}
		}
		public string LinkDestination {
			set { _DocumentLink = value; }
			get {
				if (!string.IsNullOrEmpty(_DocumentLink)) {
					return _DocumentLink;
				} else if (!string.IsNullOrEmpty(_Filename)) {
					return _Filename;
				} else {
					return "";
				}
			} 
		}
		public string Filename {
			get { return _Filename; } 
			set {
				if (_Filename != value) {
					_Filename = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_DocumentLink SET Filename = @Value WHERE DocumentLinkSerial = @DocumentLinkSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar, 250).Value = value.Length > 0 ? value : SqlString.Null;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string IconFilename {
			get { return _IconFilename; } 
			set {
				if (_IconFilename != value) {
					_IconFilename = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_DocumentLink SET IconFilename = @Value WHERE DocumentLinkSerial = @DocumentLinkSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar, 250).Value = value.Length > 0 ? value : SqlString.Null;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string LinkText {
			get {
				if (!string.IsNullOrEmpty(_LinkText)) {
					return _LinkText;
				} else if (!string.IsNullOrEmpty(_Filename)) {
					if (_Filename.Contains("/")) {
						return _Filename.Substring(_Filename.LastIndexOf("/") + 1);
					} else {
						return _Filename;
					}
				} else if (!string.IsNullOrEmpty(_DocumentLink)) {
					return _DocumentLink;
				} else {
					return "Click Here";
				}
			} 
			set {
				if (_LinkText != value) {
					_LinkText = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						using (SqlCommand cmd = new SqlCommand("UPDATE cms_DocumentLink SET LinkText = @Value WHERE DocumentLinkSerial = @DocumentLinkSerial", cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("DocumentLinkSerial", SqlDbType.Int).Value = _DocumentLinkSerial;
							cmd.Parameters.Add("Value", SqlDbType.NVarChar, 250).Value = value.Length > 0 ? value : SqlString.Null;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public Int64 Size { 
			get {
				//if (!string.IsNullOrEmpty(_Filename)) {
				//	return new System.IO.FileInfo(HttpContext.Current.Server.MapPath(_Filename)).Length;
				//} else {
				//	return 0;
				//}
				return 0;
			} 
		}
		public DateTime DateUploaded { get { return _DateUploaded; } }
		#endregion
	}
}