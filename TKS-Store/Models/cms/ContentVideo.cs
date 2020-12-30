using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace TKS.Models.CMS {
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
		public ContentVideo(Guid ModuleID) {
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
							_PageURL = dr[0].ToString();
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
		#endregion

		#region Properties
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

	public class VideoSaveModel {
		public string Src { get; set; }
		public string URL { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
	}

	public class YouTube {
		#region Fields
		private string _Data = "";

	//To hide the related videos from appearing after the video is done:
	//[youtube=http://www.youtube.com/watch?v=JaNH56Vpg-A&rel=0]
	//To start at a certain point in the video, convert the time of that point from minutes and seconds to all seconds, then add that number as shown (using an example start point of 1 minute 15 seconds):
	//[youtube=http://www.youtube.com/watch?v=JaNH56Vpg-A&start=75]
	//To specify a start and end time for a video, do the same as the above but add the end time as shown: [youtube=http://www.youtube.com/watch?v=JaNH56Vpg-A&start=75&end=85]
	//To hide the top information bar:
	//[youtube=http://www.youtube.com/watch?v=JaNH56Vpg-A&showinfo=0]
	//To change the look of the player:
	//[youtube=http://www.youtube.com/watch?v=JaNH56Vpg-A&theme=light]
		#endregion

		#region Constructor
		public YouTube(string data) {
			_Data = data;
		}
		#endregion

		#region Public Methods
		public string GetCode() {
			string _VideoURL = "";
			int _Height = 0;
			int _Width = 0;
			bool _ShowRelated = false;
			string ret = "";
		//https://www.youtube.com/watch?v=GDfqCFFvfag
			ret = "<iframe src='https://www.youtube.com/embed/GDfqCFFvfag?rel=0&amp;controls=0&amp;showinfo=0' frameborder='0' allowfullscreen></iframe>";
			//ret = "<iframe width='560' height='315' src='https://www.youtube.com/embed/GDfqCFFvfag?rel=0&amp;controls=0&amp;showinfo=0' frameborder='0' allowfullscreen></iframe>";

			return ret;
		}
		#endregion
	}
}
