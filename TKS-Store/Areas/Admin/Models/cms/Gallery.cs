using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using ImageResizer;

namespace TKS.Areas.Admin.Models.CMS {
	public class GalleryItem {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private int _GalleryPhotoSerial = 0;
		private int _SortOrder = 0;
		private string _PhotoDescription = "";
		private string _PhotoLink = "";
		private string _PhotoPath = "";
		private string _PhotoTitle = "";
		#endregion

		#region Constructor
		public GalleryItem() { }
		public GalleryItem(int GalleryPhotoSerial) {
			_GalleryPhotoSerial = GalleryPhotoSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT [ModuleID], [PhotoPath], [PhotoTitle], [PhotoDescription], [PhotoLink], [SortOrder] " +
							"	FROM cms_GalleryPhoto WHERE GalleryPhotoSerial = @GalleryPhotoSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("GalleryPhotoSerial", SqlDbType.Int).Value = _GalleryPhotoSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_PhotoPath = dr[1].ToString();
						_PhotoTitle = dr[2].ToString();
						_PhotoDescription = dr[3].ToString();
						_PhotoLink = dr[4].ToString();
						_SortOrder = dr.GetInt32(5);
					} else {
						_GalleryPhotoSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateIntProperty(string FieldName, int? Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_GalleryPhoto] SET " + FieldName + " = @Value WHERE GalleryPhotoSerial = @GalleryPhotoSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("GalleryPhotoSerial", SqlDbType.Int).Value = GalleryPhotoSerial;
					if (Value == null) {
						cmd.Parameters.Add("Value", SqlDbType.Int).Value = SqlInt32.Null;
					} else {
						cmd.Parameters.Add("Value", SqlDbType.Int).Value = Value;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_GalleryPhoto] SET " + FieldName + " = @Value WHERE GalleryPhotoSerial = @GalleryPhotoSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("GalleryPhotoSerial", SqlDbType.Int).Value = GalleryPhotoSerial;
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
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_GalleryPhoto WHERE GalleryPhotoSerial = @GalleryPhotoSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("GalleryPhotoSerial", SqlDbType.Int).Value = _GalleryPhotoSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid ModuleID { get { return _ModuleID; } }
		public int GalleryPhotoSerial { get { return _GalleryPhotoSerial; } }
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					UpdateIntProperty("SortOrder", value);
				}
			}
		}
		public string PhotoDescription {
			get { return _PhotoDescription; }
			set {
				if (_PhotoDescription != value) {
					_PhotoDescription = UpdateStringProperty("PhotoDescription", Int32.MaxValue, value);
				}
			}
		}
		public string PhotoImageTag {
			get {
				if (PhotoPath.Length > 0) {
					return string.Format("<img alt=\"{0}\" src='{1}' class='img-responsive' />", PhotoTitle, "/assets/images/gallery/" + ModuleID.ToString() + "/" + PhotoPath);
				} else {
					return "";
				}
			}
		}
		public string PhotoLink {
			get { return _PhotoLink; }
			set {
				if (_PhotoLink != value) {
					_PhotoLink = UpdateStringProperty("PhotoLink", Int32.MaxValue, value);
				}
			}
		}
		public string PhotoPath {
			get { return _PhotoPath; }
			set {
				if (_PhotoPath != value) {
					_PhotoPath = UpdateStringProperty("PhotoPath ", 250, value);
				}
			}
		}
		public string PhotoTitle {
			get { return _PhotoTitle; }
			set {
				if (_PhotoTitle != value) {
					_PhotoTitle = UpdateStringProperty("PhotoTitle", Int32.MaxValue, value);
				}
			}
		}
		#endregion
	}

	public class GalleryItemModel {
		#region Constructor
		public GalleryItemModel() { }
		public GalleryItemModel(GalleryItem data) {
			this.GalleryPhotoSerial = data.GalleryPhotoSerial;
			this.ModuleID = data.ModuleID;
			this.PhotoDescription = data.PhotoDescription;
			this.PhotoImageTag = data.PhotoImageTag;
			this.PhotoLink = data.PhotoLink;
			this.PhotoPath = data.PhotoPath;
			this.PhotoTitle = data.PhotoTitle;
			this.SortOrder = data.SortOrder;
		}
		#endregion

		#region Properties
		public Guid ModuleID { get; set; }
		public int GalleryPhotoSerial { get; set; }
		public int SortOrder { get; set; }
		public string PhotoDescription { get; set; }
		public string PhotoImageTag { get; set; }
		public string PhotoLink { get; set; }
		public string PhotoPath { get; set; }
		public string PhotoTitle { get; set; }
		#endregion
	}

	public class Gallery {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		#endregion

		#region Constructor
		public Gallery(Guid ModuleID) {
			_ModuleID = ModuleID;
		}
		#endregion

		#region Public Methods
		public int Add(GalleryItemModel data) {
			int ret = 0;
			int sortOrder = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.SortOrder == -1) {
					using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(SortOrder), 0) FROM cms_GalleryPhoto WHERE ModuleID = @ModuleID", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = data.ModuleID;

						cmd.Connection.Open();
						sortOrder = (int)cmd.ExecuteScalar();
						sortOrder++;
						cmd.Connection.Close();
					}
				} else {
					sortOrder = data.SortOrder;
				}
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_GalleryPhoto (ModuleID, PhotoPath, PhotoTitle, PhotoDescription, PhotoLink, SortOrder) OUTPUT INSERTED.GalleryPhotoSerial VALUES (@ModuleID, @PhotoPath, @PhotoTitle, @PhotoDescription, @PhotoLink, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = data.ModuleID;
					cmd.Parameters.Add("PhotoPath", SqlDbType.VarChar, 250).Value = data.PhotoPath;
					cmd.Parameters.Add("PhotoTitle", SqlDbType.NVarChar).Value = data.PhotoTitle ?? "";
					cmd.Parameters.Add("PhotoDescription", SqlDbType.NVarChar).Value = data.PhotoDescription ?? "";
					cmd.Parameters.Add("PhotoLink", SqlDbType.VarChar).Value = data.PhotoLink ?? "";
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = sortOrder;

					cmd.Connection.Open();
					ret = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			
			return ret;
		}
		public List<GalleryItemModel> Slides() {
			List<GalleryItemModel> l = new List<GalleryItemModel>();

			if (_ModuleID != Guid.Empty) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT GalleryPhotoSerial FROM cms_GalleryPhoto WHERE ModuleID = @ModuleID ORDER BY SortOrder, PhotoTitle, PhotoPath", cn)) {
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new GalleryItemModel(new GalleryItem(dr.GetInt32(0))));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		#endregion
	}
}