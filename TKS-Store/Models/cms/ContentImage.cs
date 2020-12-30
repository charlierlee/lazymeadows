using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Models.CMS {
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
		public ContentImage(Guid ModuleID) {
			_ModuleID = ModuleID;
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
								"	m.ModuleName, p.Locale " +
								"	FROM [cms_Content] c JOIN cms_Module m ON c.ModuleID = m.ModuleID " +
								"	JOIN cms_Page p ON m.PageID = p.PageID " +
								"	WHERE ContentID = @ContentID";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ContentID", SqlDbType.UniqueIdentifier).Value = _ContentID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
						if (dr.HasRows) {
							dr.Read();
							_Contents = dr[1].ToString();
							_Locale = dr[6].ToString();
							_ModuleID = dr.GetGuid(0);
							_ModuleName = dr[5].ToString();
							_ModuleTypeName = "";
							_PageID = dr.GetGuid(4);
							_PageSectionName = "";
							_PageTitle = "";
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

		#region Properties
		public string AltText { get { return _AltText;} }
		public string Contents {
			get { return _Contents ?? ""; }
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
		}
		public string FileName { get {return _Filename; } }
		public string Height { get { return _Height; } }
		public string ImgTag {
			get {
				return string.Format("<img alt=\"{0}\" src='{1}' height='{2}' width='{3}' />", AltText, FileName, Height, Width);
			}
		}
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
	public class ImageSaveModel {
		public string AltText { get; set; }
		public string Src { get; set; }
		public string Height { get; set; }
		public string Width { get; set; }
	}
}