using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace TKS.Models.CMS {
	public class GalleryItem {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private int _GalleryPhotoSerial = 0;
		private int _SortOrder = 0;
		private string _FullscreenPath = "";
		private string _PhotoDescription = "";
		private string _PhotoLink = "";
		private string _PhotoPath = "";
		private string _PhotoTitle = "";
		private string _ThumbPath = "";
		#endregion
 
		#region Constructor
		public GalleryItem() { }
		public GalleryItem(int GalleryPhotoSerial) {
			_GalleryPhotoSerial = GalleryPhotoSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT [ModuleID], [ThumbPath], [PhotoPath], [FullscreenPath], [PhotoTitle], [PhotoDescription], [PhotoLink], [SortOrder] " +
							"	FROM cms_GalleryPhoto WHERE GalleryPhotoSerial = @GalleryPhotoSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("GalleryPhotoSerial", SqlDbType.Int).Value = _GalleryPhotoSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_ThumbPath = dr[1].ToString();
						_PhotoPath = dr[2].ToString();
						_FullscreenPath = dr[3].ToString();
						_PhotoTitle = dr[4].ToString();
						_PhotoDescription = dr[5].ToString();
						_PhotoLink = dr[6].ToString();
						_SortOrder = dr.GetInt32(7);
					} else {
						_GalleryPhotoSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid ModuleID { get { return _ModuleID; } }
		public int GalleryPhotoSerial { get { return _GalleryPhotoSerial; } }
		public int SortOrder { get {return _SortOrder; } }
		public string FullscreenPath { get { return _FullscreenPath; } }
		public string MetaTags { 
			get {
				return ""; // new SEO(ModuleID).GetTags(URL, PhotoTitle, PhotoDescription); 
			} 
		}
		public string PhotoDescription { get { return _PhotoDescription; } }
		public string PhotoImageTag {
			get {
				if (PhotoPath.Length > 0) {
					return string.Format("<img alt=\"{0}\" src='{1}' class='img-responsive' />", PhotoTitle, "/assets/images/gallery/" + ModuleID.ToString() + "/" + PhotoPath);
				} else if (FullscreenPath.Length > 0) {
					return string.Format("<img alt=\"{0}\" src='{1}' class='img-responsive' />", PhotoTitle, "/assets/images/gallery/" + ModuleID.ToString() + "/full/" + FullscreenPath);
				} else if (ThumbPath.Length > 0) {
					return string.Format("<img alt=\"{0}\" src='{1}' class='img-responsive' />", PhotoTitle, "/assets/images/gallery/" + ModuleID.ToString() + "/thumb/" + ThumbPath);
				} else {
					return "";
				}
			}
		}
		public string PhotoLink { get { return _PhotoLink; } }
		public string PhotoPath { get { return _PhotoPath; } }
		public string PhotoTitle { get { return _PhotoTitle; } }
		public string ThumbPath { get { return _ThumbPath; } }
		#endregion
	}

	public class GalleryItemModel {
		#region Constructor
		public GalleryItemModel() { }
		public GalleryItemModel(GalleryItem data) {
			this.FullscreenPath = data.FullscreenPath;
			this.GalleryPhotoSerial = data.GalleryPhotoSerial;
			this.MetaTags = data.MetaTags;
			this.ModuleID = data.ModuleID;
			this.PhotoDescription = data.PhotoDescription;
			this.PhotoImageTag = data.PhotoImageTag;
			this.PhotoLink = data.PhotoLink;
			this.PhotoPath = data.PhotoPath;
			this.PhotoTitle = data.PhotoTitle;
			this.SortOrder = data.SortOrder;
			this.ThumbPath = data.ThumbPath;
		}
		#endregion

		#region Properties
		public Guid ModuleID { get; set; }
		public int GalleryPhotoSerial { get; set; }
		public int SortOrder { get; set; }
		public string FullscreenPath { get; set; }
		public string MetaTags { get; set; }
		public string PhotoDescription { get; set; }
		public string PhotoImageTag { get; set; }
		public string PhotoLink { get; set; }
		public string PhotoPath { get; set; }
		public string PhotoTitle { get; set; }
		public string ThumbPath { get; set; }
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
		public List<GalleryItemModel> GetImages(int Count = 0) {
			List<GalleryItemModel> list = new List<GalleryItemModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (Count > 0) {
					SQL = "SELECT TOP " + Count.ToString() + " GalleryPhotoSerial FROM cms_GalleryPhoto WHERE ModuleID = @ModuleID ORDER BY SortOrder, PhotoTitle, GalleryPhotoSerial";
				} else {
					SQL = "SELECT GalleryPhotoSerial FROM cms_GalleryPhoto WHERE ModuleID = @ModuleID ORDER BY SortOrder, PhotoTitle, GalleryPhotoSerial";
				}
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						list.Add(new GalleryItemModel(new GalleryItem(dr.GetInt32(0))));
					}
					cmd.Connection.Close();
				}
			}
			return list;
		}
		#endregion
	}

	public class GalleryPage {
		#region Fields
		private Guid _PageID = Guid.Empty;
		#endregion

		#region Constructor
		public GalleryPage(Guid PageID) {
			_PageID = PageID;
		}
		#endregion

		#region Public Methods
		public List<GalleryItemModel> GetImages(int Count = 0) {
			List<GalleryItemModel> list = new List<GalleryItemModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (Count > 0) {
					SQL = "SELECT TOP " + Count.ToString() + " GalleryPhotoSerial FROM cms_GalleryPhoto g JOIN cms_Module m on g.ModuleID = m.ModuleID WHERE m.PageID = @PageID ORDER BY g.SortOrder, PhotoTitle, GalleryPhotoSerial";
				} else {
					SQL = "SELECT GalleryPhotoSerial FROM cms_GalleryPhoto g JOIN cms_Module m on g.ModuleID = m.ModuleID WHERE m.PageID = @PageID ORDER BY g.SortOrder, PhotoTitle, GalleryPhotoSerial";
				}
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = _PageID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						list.Add(new GalleryItemModel(new GalleryItem(dr.GetInt32(0))));
					}
					cmd.Connection.Close();
				}
			}
			return list;
		}
		#endregion
	}
}