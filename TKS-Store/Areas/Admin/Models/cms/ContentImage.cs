using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class ContentImage {
		#region Fields
		private string _AltText = "";
		private string _Contents = null;
		private Guid _ContentID = Guid.Empty;
		private string _Filename = "";
		private string _Height = "";
		private string _Locale = "en-US";
		private Guid _ModuleID = Guid.Empty;
		private string _ModuleTypeName = "";
		private string _ModuleName = "";
		private Guid _PageID = Guid.Empty;
		private string _PageSectionName = "";
		private string _PageTitle = "";
		private string _URL = "";
		private string _Width = "";
		#endregion

		#region Constructor
		public ContentImage() { }
		public ContentImage(Guid ModuleID) {
			_ModuleID = ModuleID;
			Initialize();
		}
		public ContentImage(ImageViewModel imageViewModel) {
			_ContentID = imageViewModel.ContentID;
			_ModuleID = imageViewModel.ModuleID;
			Initialize();
		}
		#endregion

		#region Private Methods
		private void Initialize() {
			if (_ContentID == Guid.Empty) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT ContentID " +
									"FROM [cms_Content] " +
									"WHERE ModuleID = @ModuleID " +
									"AND IsDraft = 0 " +
									"AND UpdateDate IS NULL";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = ModuleID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
						if (dr.HasRows) {
							dr.Read();
							_ContentID = dr.GetGuid(0);
						}
						cmd.Connection.Close();
					}
				}
			}

			if (_ContentID == Guid.Empty) {
				//still not found - at least get the containing page's information
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT p.VirtualPath, p.PageID, m.ModuleName, mt.ModuleTypeName, ps.PageSectionName, p.Locale " +
								"	FROM cms_Module m JOIN cms_Page p ON m.PageID = p.PageID " +
								"	JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
								"	JOIN cms_PageSection ps ON m.PageSectionID = ps.PageSectionID " +
								"	WHERE m.ModuleID = @ModuleID";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = ModuleID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
						if (dr.HasRows) {
							dr.Read();
							_Locale = dr[5].ToString();
							_ModuleName = dr[2].ToString();
							_ModuleTypeName = dr[3].ToString();
							_PageID = dr.GetGuid(1);
							_PageSectionName = dr[4].ToString();
							_URL = dr[0].ToString();
						}
						cmd.Connection.Close();
					}
				}
			} else {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT c.ModuleID, c.Contents, c.IsDraft, p.VirtualPath, p.PageID, " +
								"	s.MetaTitle, m.ModuleName, mt.ModuleTypeName, ps.PageSectionName, p.Locale " +
								"	FROM [cms_Content] c JOIN cms_Module m ON c.ModuleID = m.ModuleID " +
								"	JOIN cms_Page p ON m.PageID = p.PageID " +
								"	JOIN cms_ModuleType mt ON m.ModuleTypeID = mt.ModuleTypeID " +
								"	JOIN cms_PageSection ps ON m.PageSectionID = ps.PageSectionID " +
								"	LEFT JOIN cms_SEO s ON p.PageID = s.EntityGUID " +
								"	WHERE ContentID = @ContentID";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ContentID", SqlDbType.UniqueIdentifier).Value = _ContentID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
						if (dr.HasRows) {
							dr.Read();
							_Contents = dr[1].ToString();
							_Locale = dr[9].ToString();
							_ModuleID = dr.GetGuid(0);
							_ModuleName = dr[6].ToString();
							_ModuleTypeName = dr[7].ToString();
							_PageID = dr.GetGuid(4);
							_PageSectionName = dr[8].ToString();
							_PageTitle = dr[5].ToString();
							_URL = dr[3].ToString();
						} else {
							_ContentID = Guid.Empty;
						}
						cmd.Connection.Close();
					}
					if (!string.IsNullOrEmpty(_Contents)) {
						try {
							ImageSaveModel imageSaveModel = new ImageSaveModel();
							System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
							doc.LoadXml(_Contents);
							System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(doc.DocumentElement);
							System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
							//object obj = ser.Deserialize(reader);
							//imageSaveModel = (ImageSaveModel)obj;
							imageSaveModel = (ImageSaveModel)ser.Deserialize(reader);

							_AltText = imageSaveModel.AltText;
							_Filename = imageSaveModel.Src;
							_Height = imageSaveModel.Height;
							_Width = imageSaveModel.Width;
						} catch {
							_AltText = _Contents;
						}
					}
				}
			}
		}
		#endregion

		#region Public Methods
		public List<ContentTextHistoryViewModel> History() {
			List<ContentTextHistoryViewModel> l = new List<ContentTextHistoryViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ContentID, InsertDate FROM [cms_Content] WHERE ModuleID = @ModuleID AND IsDraft = 0 AND UpdateDate IS NOT NULL ORDER BY InsertDate DESC", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentTextHistoryViewModel() { ContentID = dr.GetGuid(0), InsertDate = dr.GetDateTime(1) });
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public void MakeCurrent(Guid ContentID) {
			//// Update the current content
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	using (SqlCommand cmd = new SqlCommand("UPDATE cms_Content SET UpdateDate = @UpdateDate, UpdatedBy = @UpdatedBy WHERE UpdateDate IS NULL AND ModuleID = @ModuleID AND Locale = @Locale AND IsDraft = @IsDraft", cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = Guid.Parse(ddlModuleHistory.SelectedItem.Value); ;
			//		cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = ddlLocaleHistory.SelectedItem.Value;
			//		cmd.Parameters.Add("IsDraft", SqlDbType.Int).Value = 0;
			//		cmd.Parameters.Add("UpdateDate", SqlDbType.DateTime).Value = DateTime.Now;
			//		cmd.Parameters.Add("UpdatedBy", SqlDbType.UniqueIdentifier).Value = new Guid(Membership.GetUser().ProviderUserKey.ToString());

			//		cmd.Connection.Open();
			//		cmd.ExecuteNonQuery();
			//		cmd.Connection.Close();
			//	}
			//}

			//// Set the current selected history as current
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	using (SqlCommand cmd = new SqlCommand("UPDATE cms_Content SET UpdateDate=NULL, UpdatedBy=NULL WHERE ContentID=@ContentID", cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("ContentID", SqlDbType.Int).Value = (int)ViewState["ContentID"];

			//		cmd.Connection.Open();
			//		cmd.ExecuteNonQuery();
			//		cmd.Connection.Close();
			//	}
			//}
		}
		public void SaveChanges(ImageViewModel imageViewModel) {
			System.Drawing.Image objImage = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(imageViewModel.Filename));

			ImageSaveModel imageSaveModel = new ImageSaveModel();
			imageSaveModel.Height = objImage.Height.ToString();
			imageSaveModel.Width = objImage.Width.ToString();
			imageSaveModel.AltText = imageViewModel.AltText;
			imageSaveModel.Src = imageViewModel.Filename;

			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(sb);
			ser.Serialize(writer, imageSaveModel);

			this.Contents = sb.ToString();
		}
		#endregion

		#region Properties
		public string AltText { get { return _AltText;} }
		public string Contents {
			get { return _Contents ?? ""; }
			set {
				if (_Contents != value) {
					if (_Contents == null && value != null) {
						//New value
						_Contents = value;
						_ContentID = Guid.NewGuid();
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_Content ( ContentID, ModuleID, Contents, IsDraft, InsertedBy ) VALUES ( @ContentID, @ModuleID, @Contents, 0, @InsertedBy)", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("ContentID", SqlDbType.UniqueIdentifier).Value = _ContentID;
								cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
								cmd.Parameters.Add("Contents", SqlDbType.VarChar).Value = value;
								cmd.Parameters.Add("IsDraft", SqlDbType.Int).Value = 0;
								cmd.Parameters.Add("InsertedBy", SqlDbType.UniqueIdentifier).Value = new User().UserKey;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					} else {
						//Editing
						_Contents = value;
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							cn.Open();

							// Start a local transaction.
							SqlTransaction sqlTran = cn.BeginTransaction();

							// Enlist a command in the current transaction.
							SqlCommand cmd = cn.CreateCommand();
							cmd.Transaction = sqlTran;

							try {
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = "UPDATE cms_Content " +
												"	SET UpdateDate = GETDATE(), " +
												"		UpdatedBy = @UserID " +
												"	WHERE ContentID = @ContentID";
								cmd.Parameters.Add("ContentID", SqlDbType.UniqueIdentifier).Value = _ContentID;
								cmd.Parameters.Add("UserID", SqlDbType.UniqueIdentifier).Value = new User().UserKey;
								cmd.ExecuteNonQuery();

								//// Remove from Lucene search index
								//Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
								//li.CreateIndexWriter();
								//li.Delete(ContentID.ToString());
								//li.Close();

								_ContentID = Guid.NewGuid();
								cmd.CommandText = "INSERT INTO cms_Content ( " +
												"	ContentID, ModuleID, Contents, IsDraft, InsertedBy " +
												"	) VALUES ( " +
												"	@ContentID, @ModuleID, @Contents, 0, @UserID " +
												"	)";
								cmd.Parameters["ContentID"].Value = _ContentID;
								cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
								cmd.Parameters.Add("Contents", SqlDbType.VarChar).Value = value;
								cmd.ExecuteNonQuery();

								// Commit the transaction.
								sqlTran.Commit();
							} catch (Exception ex) {
								//HttpContext.Current.Response.Write(ex.Message);
								// Handle the exception if the transaction fails to commit.
								//Console.WriteLine(ex.Message);
								Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
								try {
									// Attempt to roll back the transaction.
									sqlTran.Rollback();
								} catch (Exception exRollback) {
									// Throws an InvalidOperationException if the connection  
									// is closed or the transaction has already been rolled  
									// back on the server.
									Elmah.ErrorSignal.FromCurrentContext().Raise(exRollback);
								}
							} finally {
								cn.Close();
							}
						}
					}
				}
			}
		}
		public Guid ContentID { get { return _ContentID; } }
		public string DraftContents {
			get {
				string draft = "";
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT c.Contents " +
									"FROM [cms_Content] c " +
									"WHERE c.ModuleID = @ModuleID " +
									"AND c.IsDraft = 1 " +
									"AND c.UpdateDate IS NULL";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
						if (dr.HasRows) {
							dr.Read();
							draft = dr[0].ToString();
						}
						cmd.Connection.Close();
					}
				}
				return draft;
			}
			set {
				if (_Contents != value) {
					if (_Contents == null && value != null) {
						//New value
						_Contents = value;
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_Content ( ModuleID, Contents, IsDraft, InsertedBy ) VALUES ( @ModuleID, @Contents, 0, @InsertedBy)", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
								cmd.Parameters.Add("Contents", SqlDbType.VarChar).Value = value;
								cmd.Parameters.Add("IsDraft", SqlDbType.Int).Value = 1;
								cmd.Parameters.Add("InsertedBy", SqlDbType.UniqueIdentifier).Value = new User().UserKey;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					} else {
						//Editing
						_Contents = value;
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							cn.Open();

							// Start a local transaction.
							SqlTransaction sqlTran = cn.BeginTransaction();

							// Enlist a command in the current transaction.
							SqlCommand cmd = cn.CreateCommand();
							cmd.Transaction = sqlTran;

							try {
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = "UPDATE cms_Content " +
												"	SET UpdateDate = GETDATE(), " +
												"		UpdatedBy = @UserID " +
												"	WHERE ModuleID = @ModuleID " +
												"	AND UpdateDate IS NULL";
								cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
								cmd.Parameters.Add("UserID", SqlDbType.UniqueIdentifier).Value = new User().UserKey;
								cmd.ExecuteNonQuery();

								cmd.CommandText = "INSERT INTO cms_Content ( " +
												"	ModuleID, Contents, IsDraft, InsertedBy " +
												"	) VALUES ( " +
												"	@ModuleID, @Contents, 1, @UserID " +
												"	)";
								cmd.Parameters.Add("Contents", SqlDbType.VarChar).Value = value;
								cmd.ExecuteNonQuery();

								// Commit the transaction.
								sqlTran.Commit();
							} catch (Exception ex) {
								//HttpContext.Current.Response.Write(ex.Message);
								// Handle the exception if the transaction fails to commit.
								//Console.WriteLine(ex.Message);
								Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
								try {
									// Attempt to roll back the transaction.
									sqlTran.Rollback();
								} catch (Exception exRollback) {
									// Throws an InvalidOperationException if the connection  
									// is closed or the transaction has already been rolled  
									// back on the server.
									Elmah.ErrorSignal.FromCurrentContext().Raise(exRollback);
								}
							} finally {
								cn.Close();
							}
						}
					}
				}
			}
		}
		public string FileName { get {return _Filename; } }
		public string Height { get { return _Height; } }
		public string Locale { get { return _Locale; } }
		public Guid ModuleID { get { return _ModuleID; } }
		public string ModuleTypeName { get { return _ModuleTypeName; } }
		public string ModuleName { get { return _ModuleName; } }
		public Guid PageID { get { return _PageID; } }
		public string PageSectionName { get { return _PageSectionName; } }
		public string PageTitle { get { return _PageTitle; } }
		public string URL { get { return _URL; } }
		public string Width { get { return _Width; } }
		#endregion
	}
	public class ImageViewModel {
		#region Fields
		private string _AltText = "";
		private string _Contents = "";
		private Guid _ContentID = Guid.Empty;
		private string _DraftContents = "";
		private string _Filename = "";
		private string _Height = "";
		private string _Locale = "en-US";
		private Guid _ModuleID = Guid.Empty;
		private string _ModuleTypeName = "";
		private string _ModuleName = "";
		private Guid _PageID = Guid.Empty;
		private string _PageSectionName = "";
		private string _PageTitle = "";
		private string _URL = "";
		private string _Width = "";
		#endregion

		#region Constructor
		public ImageViewModel() { }
		public ImageViewModel(ContentImage image) {
			_AltText = image.AltText;
			_Contents = image.Contents;
			_ContentID = image.ContentID;
			//_DraftContents = image.DraftContents;
			_Filename = image.FileName;
			_Height = image.Height;
			_Locale = image.Locale;
			_ModuleID = image.ModuleID;
			_ModuleTypeName = image.ModuleTypeName;
			_ModuleName = image.ModuleName;
			_PageID = image.PageID;
			_PageSectionName = image.PageSectionName;
			_PageTitle = image.PageTitle;
			_URL = image.URL;
			_Width = image.Width;
		}
		#endregion

		#region Properties
		[DisplayName("Alt Text")]
		public string AltText { get { return _AltText; } set { _AltText = value; } }
		[DisplayName("Contents")]
		public string Contents { get { return _Contents; } set { _Contents = value; } }
		[DisplayName("Content ID")]
		public Guid ContentID { get { return _ContentID; } set { _ContentID = value; } }
		[DisplayName("Contents")]
		public string DraftContents { get { return _DraftContents; } set { _DraftContents = value; } }
		[DisplayName("Image")]
		public string Filename { get { return _Filename; } set { _Filename = value; } }
		[DisplayName("Height")]
		public string Height { get { return _Height; } set { _Height = value; } }
		public string ImgTag {
			get {
				return string.Format("<img alt=\"{0}\" src='{1}' height='{2}' width='{3}' />", AltText, Filename, Height, Width);
			}
		}
		[DisplayName("Locale")]
		public string Locale { get { return _Locale; } set { _Locale = value; } }
		[DisplayName("ModuleID")]
		public Guid ModuleID { get { return _ModuleID; } set { _ModuleID = value; } }
		public string ModuleTypeName { get { return _ModuleTypeName; } set { _ModuleTypeName = value; } }
		public string ModuleName { get { return _ModuleName; } set { _ModuleName = value; } }
		public Guid PageID { get { return _PageID; } set { _PageID = value; } }
		public string PageSectionName { get { return _PageSectionName; } set { _PageSectionName = value; } }
		[DisplayName("Page Title")]
		public string PageTitle { get { return _PageTitle; } set { _PageTitle = value; } }
		[DisplayName("URL")]
		public string URL { get { return _URL; } set { _URL = value; } }
		[DisplayName("Width")]
		public string Width { get { return _Width; } set { _Width = value; } }
		#endregion
	}
	public class ImageSaveModel {
		public string AltText { get; set; }
		public string Src { get; set; }
		public string Height { get; set; }
		public string Width { get; set; }
	}
}