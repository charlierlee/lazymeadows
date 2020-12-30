using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class ContentVideo {
		#region Fields
		private string _Contents = null;
		private Guid _ContentID = Guid.Empty;
		private string _Locale = "en-US";
		private Guid _ModuleID = Guid.Empty;
		private string _ModuleTypeName = "";
		private string _ModuleName = "";
		private Guid _PageID = Guid.Empty;
		private string _PageSectionName = "";
		private string _PageTitle = "";
		private string _PageURL = "";
		private string _VideoSource = "";
		private string _VideoURL = "";
		private int _Height = 0;
		private int _Width = 0;
		#endregion

		#region Constructor
		public ContentVideo() { }
		public ContentVideo(Guid ModuleID) {
			_ModuleID = ModuleID;
			Initialize();
		}
		public ContentVideo(ContentVideoViewModel data) {
			_ContentID = data.ContentID;
			_ModuleID = data.ModuleID;
			Initialize();
		}
		public ContentVideo(ContentVideoDraftViewModel data) {
			_ContentID = data.ContentID;
			_ModuleID = data.ModuleID;
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
							_PageURL = dr[0].ToString();
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
							_PageURL = dr[3].ToString();
						} else {
							_ContentID = Guid.Empty;
						}
						cmd.Connection.Close();
					}
					if (!string.IsNullOrEmpty(_Contents)) {
						try {
							VideoSaveModel vidSaveModel = new VideoSaveModel();
							System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
							doc.LoadXml(_Contents);
							System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(doc.DocumentElement);
							System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(vidSaveModel.GetType());
							vidSaveModel = (VideoSaveModel)ser.Deserialize(reader);

							_VideoURL = vidSaveModel.URL;
							_VideoSource = vidSaveModel.Src;
							_Height = vidSaveModel.Height;
							_Width = vidSaveModel.Width;
						} catch {
						}
					}
				}
			}
		}
		private void MakeDraftLive() {
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
									"	WHERE UpdateDate IS NULL AND ModuleID = @ModuleID AND IsDraft = 0";

					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("UserID", SqlDbType.UniqueIdentifier).Value = new User().UserKey;
					cmd.ExecuteNonQuery();

					cmd.CommandText = "UPDATE cms_Content " +
									"SET IsDraft = 0, " +
									"InsertedBy = @UserID, " +
									"InsertDate = GETDATE() " +
									"WHERE ModuleID = @ModuleID " +
									"AND IsDraft = 1 ";
					cmd.ExecuteNonQuery();	// Upgrade draft to production

					sqlTran.Commit();
				} catch (Exception ex) {
					// Handle the exception if the transaction fails to commit.
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
		private void UpdateIndex() {
			// Update Lucene search index
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			li.UpdateWebPage(ContentID.ToString(), PageURL, PageTitle, Contents, "Page");
			li.Close();
			li.IndexWords();
		}
		#endregion

		#region Public Methods
		public List<ContentVideoHistoryViewModel> History() {
			List<ContentVideoHistoryViewModel> l = new List<ContentVideoHistoryViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT ContentID, InsertDate " +
								"FROM [cms_Content] " +
								"WHERE ModuleID = @ModuleID " +
								"AND IsDraft=0 " +
								"AND Contents IS NOT NULL " +
								"AND UpdateDate IS NOT NULL " +
								"ORDER BY InsertDate DESC";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentVideoHistoryViewModel() { ContentID = dr.GetGuid(0), InsertDate = dr.GetDateTime(1), ModuleID = _ModuleID, PageID = _PageID });
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public void MakeDraft(Guid ContentID) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_Content WHERE ModuleID = @ModuleID AND IsDraft = 1", cn)) {
					// Delete the current draft
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					// Set the selected to be the draft
					cmd.CommandText = "INSERT INTO cms_Content (ModuleID, Contents, IsDraft, InsertedBy) SELECT ModuleID, Contents, 1, @UserID FROM cms_Content WHERE ContentID = @ContentID";
					cmd.Parameters.Clear();
					cmd.Parameters.Add("ContentID", SqlDbType.UniqueIdentifier).Value = ContentID;
					cmd.Parameters.Add("UserID", SqlDbType.UniqueIdentifier).Value = new User().UserKey;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void MakeCurrent(Guid ContentID) {
			// Update the current content
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_Content SET UpdateDate = GETDATE(), UpdatedBy = @UpdatedBy WHERE UpdateDate IS NULL AND ModuleID = @ModuleID AND IsDraft = 0", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("UpdatedBy", SqlDbType.UniqueIdentifier).Value = new User().UserKey;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					// Duplicate the current selected history to be the current
					cmd.CommandText = "INSERT INTO cms_Content (ModuleID, Contents, IsDraft, InsertedBy) SELECT ModuleID, Contents, 0, @InsertedBy FROM cms_Content WHERE ContentID = @ContentID";
					cmd.Parameters.Clear();
					cmd.Parameters.Add("ContentID", SqlDbType.UniqueIdentifier).Value = ContentID;
					cmd.Parameters.Add("InsertedBy", SqlDbType.UniqueIdentifier).Value = new User().UserKey;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void SaveChanges(ContentVideoViewModel data) {
			VideoSaveModel videoSaveModel = new VideoSaveModel();
			videoSaveModel.Src = data.VideoSource;
			videoSaveModel.URL = data.VideoURL;
			videoSaveModel.Height = data.Height;
			videoSaveModel.Width = data.Width;

			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(videoSaveModel.GetType());
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			System.IO.StringWriter writer = new System.IO.StringWriter(sb);
			ser.Serialize(writer, videoSaveModel);

			this.Contents = sb.ToString();
		}
		public void SaveChanges(ContentVideoDraftViewModel data) {
			this.DraftContents = data.DraftContents;
			if (data.MakeLive) {
				MakeDraftLive();
			}
		}
		#endregion

		#region Properties
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

								// Remove from Lucene search index
								Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
								li.CreateIndexWriter();
								li.Delete(ContentID.ToString());
								li.Close();

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

								UpdateIndex();
							} catch(Exception ex) {
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
				string draftContents = DraftContents;
				if (draftContents != value) {
					if (string.IsNullOrEmpty(draftContents) && value != null) {
						//New value
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_Content ( ModuleID, Contents, IsDraft, InsertedBy ) VALUES ( @ModuleID, @Contents, 1, @InsertedBy)", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
								cmd.Parameters.Add("Contents", SqlDbType.VarChar).Value = value;
								cmd.Parameters.Add("InsertedBy", SqlDbType.UniqueIdentifier).Value = new User().UserKey;
								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					} else {
						//Editing
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							cn.Open();

							// Start a local transaction.
							SqlTransaction sqlTran = cn.BeginTransaction();

							// Enlist a command in the current transaction.
							SqlCommand cmd = cn.CreateCommand();
							cmd.Transaction = sqlTran;

							try {
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = "DELETE FROM [cms_Content] " +
												"WHERE ModuleID = @ModuleID " +
												"AND IsDraft = 1";
								cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
								cmd.ExecuteNonQuery();	// Delete the old draft

								cmd.CommandText = "INSERT INTO cms_Content ( " +
												"	ModuleID, Contents, IsDraft, InsertedBy " +
												"	) VALUES ( " +
												"	@ModuleID, @Contents, 1, @UserID " +
												"	)";
								cmd.Parameters.Add("Contents", SqlDbType.VarChar).Value = value;
								cmd.Parameters.Add("UserID", SqlDbType.UniqueIdentifier).Value = new User().UserKey;
								cmd.ExecuteNonQuery();	// Add the new draft

								sqlTran.Commit();
							} catch (Exception ex) {
								// Handle the exception if the transaction fails to commit.
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
		public string Locale { get { return _Locale; } }
		public Guid ModuleID { get { return _ModuleID; } }
		public string ModuleName { get { return _ModuleName; } }
		public string ModuleTypeName { get { return _ModuleTypeName; } }
		public Guid PageID { get { return _PageID; } }
		public string PageSectionName { get { return _PageSectionName; } }
		public string PageTitle { get { return _PageTitle; } }
		public string PageURL { get { return _PageURL; } }
		public string Video {
			get {
				if (!string.IsNullOrEmpty(VideoURL)) {
					int hasEqual = VideoURL.IndexOf("=");
					if (hasEqual > 0) {
						return string.Format("<iframe width='{0}' height='{1}' src='//www.youtube-nocookie.com/embed/{2}?rel=0&amp;controls=0&amp;showinfo=0'></iframe>", Width, Height, VideoURL.Substring(hasEqual + 1));

					} else {
						return "";
					}
				} else {
					return "";
				}
			}
		}
		public string VideoSource { get { return _VideoSource; } }
		public string VideoURL { get { return _VideoURL; } }
		public int Height { get { return _Height; } }
		public int Width { get { return _Width; } }
		#endregion
	}
	public class ContentVideoViewModel {
		#region Constructor
		public ContentVideoViewModel() { }
		public ContentVideoViewModel(ContentVideo data) {
			this.Contents = data.Contents;
			this.ContentID = data.ContentID;
			this.Locale = data.Locale;
			this.ModuleID = data.ModuleID;
			this.ModuleName = data.ModuleName;
			this.ModuleTypeName = data.ModuleTypeName;
			this.PageID = data.PageID;
			this.PageSectionName = data.PageSectionName;
			this.PageTitle = data.PageTitle;
			this.PageURL = data.PageURL;
			this.Video = data.Video;
			this.VideoSource = data.VideoSource;
			this.VideoURL = data.VideoURL;
			this.Width = data.Width;
			this.Height = data.Height;
		}
		#endregion

		#region Properties
		public string Contents { get; set; }
		public Guid ContentID { get; set; }
		public string Locale { get; set; }
		public Guid ModuleID { get; set; }
		public string ModuleName { get; set; }
		public string ModuleTypeName { get; set; }
		public Guid PageID { get; set; }
		public string PageSectionName { get; set; }
		public string PageTitle { get; set; }
		public string PageURL { get; set; }
		public string Video { get; set; }
		public string VideoSource { get; set; }
		public string VideoURL { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		#endregion
	}

	public class ContentVideoDraftViewModel {
		#region Fields
		private Guid _ContentID = Guid.Empty;
		private string _DraftContents = "";
		private string _Locale = "en-US";
		private Guid _ModuleID = Guid.Empty;
		private string _ModuleName = "";
		private string _ModuleTypeName = "";
		private Guid _PageID = Guid.Empty;
		private string _PageSectionName = "";
		private string _PageTitle = "";
		private string _URL = "";
		#endregion

		#region Constructor
		public ContentVideoDraftViewModel() { }
		public ContentVideoDraftViewModel(ContentVideo contentText) {
			_ContentID = contentText.ContentID;
			_DraftContents = contentText.DraftContents;
			_Locale = contentText.Locale;
			_ModuleID = contentText.ModuleID;
			_ModuleName = contentText.ModuleName;
			_ModuleTypeName = contentText.ModuleTypeName;
			_PageID = contentText.PageID;
			_PageSectionName = contentText.PageSectionName;
			_PageTitle = contentText.PageTitle;
			_URL = contentText.PageURL;
		}
		#endregion

		#region Properties
		[DisplayName("Content ID")]
		public Guid ContentID { get { return _ContentID; } set { _ContentID = value; } }
		[DisplayName("Contents")]
		public string DraftContents { get { return _DraftContents; } set { _DraftContents = value; } }
		public string Locale { get { return _Locale; } }
		[DisplayName("Make draft live")]
		public bool MakeLive { get; set; }
		[DisplayName("ModuleID")]
		public Guid ModuleID { get { return _ModuleID; } set { _ModuleID = value; } }
		public string ModuleName { get { return _ModuleName; } }
		public string ModuleTypeName { get { return _ModuleTypeName; } }
		public Guid PageID { get { return _PageID; } set { _PageID = value; } }
		public string PageSectionName { get { return _PageSectionName; } }
		[DisplayName("Page Title")]
		public string PageTitle { get { return _PageTitle; } set { _PageTitle = value; } }
		[DisplayName("URL")]
		public string URL { get { return _URL; } set { _URL = value; } }
		#endregion
	}

	public class ContentVideoSet {
		#region Fields
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public ContentVideoSet(string Locale = "en-US") {
			_Locale = Locale;
		}
		#endregion

		#region Public Methods
		public List<ContentVideo> AllContentVideoBlocks() {
			List<ContentVideo> l = new List<ContentVideo>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT c.ContentID " +
							"	FROM [cms_Content] c JOIN cms_Module m ON c.ModuleID = m.ModuleID " +
							"	JOIN cms_Page p ON m.PageID = p.PageID " +
							"	WHERE c.IsDraft = 0 " +
							"	AND c.UpdateDate IS NULL " +
							"	AND p.IsActive = 1";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentVideo(dr.GetGuid(0)));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		#endregion
	}

	public class ContentVideoHistoryViewModel {
		public Guid ContentID { get; set; }
		public DateTime InsertDate { get; set; }
		public Guid ModuleID { get; set; }
		public Guid PageID { get; set; }
	}

	public class VideoSaveModel {
		public string Src { get; set; }
		public string URL { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
	}
}
