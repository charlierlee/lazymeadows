using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class Document {
		#region Fields
		private DateTime _DateUploaded = DateTime.MinValue;
		private string _Description = "";
		private string _DocumentCategory = "";
		private int _DocumentCategorySerial = 0;
		private Guid _DocumentID = Guid.Empty;
		private string _DocumentLink = "";
		private int _DocumentLinkSerial = 0;
		private int _DocumentSerial = 0;
		private string _DocumentTitle = "";
		private string _Filename = "";
		private string _IconFilename = "";
		private string _LinkText = "";
		private string _Locale = "en-US";
		private Guid _ModuleID = Guid.Empty;
		private Int64 _Size = 0;
		private int _SortOrder = 0;
		private string _URL = "";
		#endregion

		#region Constructor
		public Document() { }
		public Document(int DocumentSerial) {
			_DocumentSerial = DocumentSerial;
			Initialize();
		}
		public Document(Guid DocumentID) {
			_DocumentID = DocumentID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DocumentSerial FROM cms_Document WHERE DocumentID = @DocumentID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentID", SqlDbType.UniqueIdentifier).Value = _DocumentID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_DocumentSerial = dr.GetInt32(0);
					} else {
						_DocumentID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
			Initialize();
		}
		#endregion

		#region Private Methods
		private void Initialize() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT d.ModuleID, d.Locale, d.DocumentID, d.DocumentCategorySerial, " + 
								"d.DocumentTitle, d.Description, d.SortOrder, " +
								"dc.CategoryName, " +
								"dl.DocumentLink, dl.Filename, dl.LinkText, dl.DocumentSize, dl.IconFilename, dl.DateUploaded, dl.DocumentLinkSerial, " + 
								"p.VirtualPath " +
								"FROM cms_Document d LEFT JOIN cms_DocumentCategory dc ON d.DocumentCategorySerial = dc.DocumentCategorySerial " + 
								"JOIN cms_DocumentLink dl ON d.DocumentSerial = dl.DocumentSerial " +
								"JOIN cms_Module m ON d.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID " +
								"WHERE d.DocumentSerial = @DocumentSerial";
				//" LEFT JOIN cms_FAQCategory c ON f.FAQCategorySerial = c.FAQCategorySerial WHERE FaqID = @FaqID", cn)) {
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_DocumentID = dr.GetGuid(2);
						_DocumentCategorySerial = dr.IsDBNull(3) ? 0 : dr.GetInt32(3);
						_DocumentTitle = dr[4].ToString();
						_Description = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
						_DocumentCategory = dr[7].ToString();
						_DocumentLink = dr[8].ToString();
						_Filename = dr[9].ToString();
						_LinkText = dr[10].ToString();
						_Size = dr.IsDBNull(11) ? 0 : dr.GetInt64(11);
						_IconFilename = dr[12].ToString();
						_DateUploaded = dr.GetDateTime(13);
						_DocumentLinkSerial = dr.GetInt32(14);
						_URL = dr[15].ToString();
					} else {
						_DocumentSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		private void UpdateIndex() {
			// Update Lucene search index
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			if (!string.IsNullOrEmpty(DocumentTitle)) {
				li.UpdateWebPage(DocumentID.ToString(), URL, DocumentTitle, Description, "Document");
			} else {
				li.UpdateWebPage(DocumentID.ToString(), URL, LinkText, Description, "Document");
			}
			li.Close();
			li.IndexWords();
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

			// Remove from Lucene search index
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			li.Delete(DocumentID.ToString());
			li.Close();
		}
		#endregion

		#region Properties
		public DateTime DateUploaded { get { return _DateUploaded; } }
		public string Description {
			get { return _Description; }
			set {
				if (_Description != value) {
					_Description = value;
					if (_DocumentSerial > 0) {
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
						UpdateIndex();
					}
				}
			}
		}
		public string DocumentCategory { get { return _DocumentCategory; } }
		public int DocumentCategorySerial {
			get { return _DocumentCategorySerial; }
			set {
				if (_DocumentCategorySerial != value) {
					_DocumentCategorySerial = value;
					if (_DocumentSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Document SET DocumentCategorySerial = @Value WHERE DocumentSerial = @DocumentSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = _DocumentSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value > 0 ? value : SqlInt32.Null;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
							// Update Document Category Name
							if (value > 0) {
								using (SqlCommand cmd = new SqlCommand("SELECT CategoryName FROM cms_DocumentCategory WHERE DocumentCategorySerial = @Value", cn)) {
									cmd.CommandType = CommandType.Text;
									cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

									cmd.Connection.Open();
									_DocumentCategory = cmd.ExecuteScalar().ToString();
									cmd.Connection.Close();
								}
							} else {
								_DocumentCategory = "";
							}
						}
					}
				}
			}
		}
		public Guid DocumentID { get { return _DocumentID; } }
		public string DocumentLink {
			get { return _DocumentLink.ToLower(); }
			set {
				if (_DocumentLink != value) {
					_DocumentLink = value;
					if (_DocumentLinkSerial > 0) {
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
						UpdateIndex();
					}
				}
			}
		}
		public int DocumentSerial { get { return _DocumentSerial; } }
		public string DocumentTitle {
			get { return _DocumentTitle; }
			set {
				if (_DocumentTitle != value) {
					_DocumentTitle = value;
					if (_DocumentSerial > 0) {
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
						UpdateIndex();
					}
				}
			}
		}
		public string Filename {
			get { return _Filename.ToLower(); }
			set {
				if (_Filename != value) {
					_Filename = value;
					if (_DocumentLinkSerial > 0) {
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
						UpdateIndex();
					}
				}
			}
		}
		public string IconFilename {
			get { return _IconFilename; }
			set {
				if (_IconFilename != value) {
					_IconFilename = value;
					if (_DocumentLinkSerial > 0) {
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
		}
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
			get {
				if (!string.IsNullOrEmpty(DocumentLink)) {
					return DocumentLink;
				} else if (!string.IsNullOrEmpty(Filename)) {
					return Filename;
				} else {
					return "";
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
					if (_DocumentLinkSerial > 0) {
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
						UpdateIndex();
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
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					if (_DocumentSerial > 0) {
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
		}
		public string URL { get { return _URL; } }
		#endregion
	}
	public class DocumentSet {
		#region Fields
		private Guid _PageID = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public DocumentSet(string Locale = "en-US") {
			_Locale = Locale;
		}
		public DocumentSet(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		public DocumentSet(Guid PageID, string ModuleName, string Locale) {
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
		public int Add(Document document) {
			int DocumentSerial = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_Document (ModuleID, Locale, DocumentCategorySerial, DocumentTitle, Description, SortOrder) OUTPUT Inserted.DocumentSerial VALUES (@ModuleID, @Locale, @DocumentCategorySerial, @DocumentTitle, @Description, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("DocumentCategorySerial", SqlDbType.Int).Value = document.DocumentCategorySerial > 0 ? document.DocumentCategorySerial : SqlInt32.Null;
					cmd.Parameters.Add("DocumentTitle", SqlDbType.NVarChar).Value = document.DocumentTitle;
					cmd.Parameters.Add("Description", SqlDbType.NVarChar).Value = document.Description;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = document.SortOrder;

					cmd.Connection.Open();
					DocumentSerial = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_DocumentLink (DocumentSerial, DocumentLink, FileName, LinkText, DocumentSize, IconFilename) VALUES (@DocumentSerial, @DocumentLink, @FileName, @LinkText, @DocumentSize, @IconFilename)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentSerial", SqlDbType.Int).Value = DocumentSerial;
					cmd.Parameters.Add("DocumentLink", SqlDbType.NVarChar, 350).Value = document.DocumentLink.Length > 0 ? document.DocumentLink : SqlString.Null;
					cmd.Parameters.Add("FileName", SqlDbType.NVarChar, 250).Value = document.Filename.Length > 0 ? document.Filename : SqlString.Null;
					cmd.Parameters.Add("LinkText", SqlDbType.NVarChar, 250).Value = document.LinkText.Length > 0 ? document.LinkText : SqlString.Null;
					cmd.Parameters.Add("DocumentSize", SqlDbType.BigInt).Value = document.Size > 0 ? document.Size : SqlInt64.Null;
					cmd.Parameters.Add("IconFilename", SqlDbType.NVarChar, 250).Value = document.IconFilename.Length > 0 ? document.IconFilename : SqlString.Null; ;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			// insert into Lucene search index
			Document newDoc = new Document(DocumentSerial);
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			if (!string.IsNullOrEmpty(newDoc.DocumentTitle)) {
				li.UpdateWebPage(newDoc.DocumentID.ToString(), newDoc.URL, newDoc.DocumentTitle, newDoc.Description, "Document");
			} else {
				li.UpdateWebPage(newDoc.DocumentID.ToString(), newDoc.URL, newDoc.LinkText, newDoc.Description, "Document");
			}
			li.Close();
			li.IndexWords();

			return DocumentSerial;
		}
		public List<Document> Documents() {
			List<Document> l = new List<Document>();

			if (_ModuleID != Guid.Empty) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT a.DocumentSerial " + 
															"FROM cms_Document a LEFT JOIN cms_DocumentCategory b ON a.DocumentCategorySerial = b.DocumentCategorySerial " + 
															"WHERE a.ModuleID = @ModuleID AND a.Locale = @Locale " +
															"ORDER BY b.SortOrder, b.CategoryName, a.SortOrder, a.DocumentSerial DESC", cn)) {
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new Document(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			} else {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT a.DocumentSerial " +
															"FROM cms_Document a LEFT JOIN cms_DocumentCategory b ON a.DocumentCategorySerial = b.DocumentCategorySerial " +
															"WHERE a.Locale = @Locale " +
															"ORDER BY b.SortOrder, b.CategoryName, a.SortOrder, a.DocumentSerial DESC", cn)) {
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new Document(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		#endregion
	}

	public class DocumentCategory {
		#region Fields
		private int _DocumentCategorySerial = 0;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		private string _CategoryName = "";
		private int _SortOrder = 0;
		#endregion

		#region Constructor
		public DocumentCategory() { }
		public DocumentCategory(int DocumentCategorySerial) {
			_DocumentCategorySerial = DocumentCategorySerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID, Locale, CategoryName, SortOrder FROM cms_DocumentCategory WHERE DocumentCategorySerial = @DocumentCategorySerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("DocumentCategorySerial", SqlDbType.Int).Value = _DocumentCategorySerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_CategoryName = dr.GetString(2);
						_SortOrder = dr.GetInt32(3);
					} else {
						_DocumentCategorySerial = 0;
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
					SqlCommand cmd = new SqlCommand("UPDATE cms_Document SET DocumentCategorySerial = NULL WHERE DocumentCategorySerial = @DocumentCategorySerial", cn, transaction);
					cmd.Parameters.Add("DocumentCategorySerial",SqlDbType.Int).Value = _DocumentCategorySerial;
					cmd.ExecuteNonQuery();
					cmd.CommandText = "DELETE FROM cms_DocumentCategory WHERE DocumentCategorySerial = @DocumentCategorySerial";
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
		public int DocumentCategorySerial { get { return _DocumentCategorySerial; } }
		public string CategoryName {
			get { return _CategoryName; }
			set {
				if (_CategoryName != value) {
					_CategoryName = value;
					if (_DocumentCategorySerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_DocumentCategory SET CategoryName = @value WHERE DocumentCategorySerial = @DocumentCategorySerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("DocumentCategorySerial", SqlDbType.Int).Value = _DocumentCategorySerial;
								cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
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
					if (_DocumentCategorySerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_DocumentCategory SET SortOrder = @Value WHERE DocumentCategorySerial = @DocumentCategorySerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("DocumentCategorySerial", SqlDbType.Int).Value = _DocumentCategorySerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		#endregion	
	}
	public class DocumentCategories {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public DocumentCategories(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		#endregion

		#region Public Methods
		public void Add(DocumentCategory documentCategory) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_DocumentCategory (ModuleID, Locale, CategoryName, SortOrder) VALUES (@ModuleID, @Locale, @CategoryName, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("CategoryName", SqlDbType.NVarChar).Value = documentCategory.CategoryName;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = documentCategory.SortOrder;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public List<DocumentCategory> Categories() {
			List<DocumentCategory> l = new List<DocumentCategory>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DocumentCategorySerial FROM cms_DocumentCategory WHERE ModuleID = @ModuleID AND Locale = @Locale ORDER BY SortOrder, CategoryName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new DocumentCategory(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion
	}
}