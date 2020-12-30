using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class ContentSlide {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private int _DimHeight = 0;
		private int _DimWidth = 0;
		private int _SlideSerial = 0;
		private int _SortOrder  = 0;
		private string _SlideContents = "";
		private string _SlideImage = "";
		private string _SlideImageFile = "";
		private string _SlideLink = "";
		private string _SlideTitle = "";
		#endregion

		#region Constructor
		public ContentSlide() { }
		public ContentSlide(int SlideSerial) {
			_SlideSerial = SlideSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT [ModuleID], [SlideTitle], [SlideContents], [SlideImage], [SlideLink], [SortOrder] " +
								"FROM [cms_Slideshow] " +
								"WHERE SlideSerial = @SlideSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("SlideSerial", SqlDbType.Int).Value = SlideSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_SlideTitle = dr[1].ToString();
						_SlideContents = dr[2].ToString();
						_SlideImage = dr[3].ToString();
						_SlideLink = dr[4].ToString();
						_SortOrder = dr.GetInt32(5);
					}
					cmd.Connection.Close();
				}

				if (!string.IsNullOrEmpty(_SlideImage)) {
					ImageSaveModel imageSaveModel = new ImageSaveModel();
					System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
					doc.LoadXml(_SlideImage);
					System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(doc.DocumentElement);
					System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
					imageSaveModel = (ImageSaveModel)ser.Deserialize(reader);

					_SlideImageFile = imageSaveModel.Src;
					_DimHeight = Convert.ToInt32(imageSaveModel.Height);
					_DimWidth = Convert.ToInt32(imageSaveModel.Width);
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateIntProperty(string FieldName, int? Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Slideshow] SET " + FieldName + " = @Value WHERE SlideSerial = @SlideSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("SlideSerial", SqlDbType.Int).Value = SlideSerial;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Slideshow] SET " + FieldName + " = @Value WHERE SlideSerial = @SlideSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("SlideSerial", SqlDbType.Int).Value = SlideSerial;
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
				using (SqlCommand cmd = new SqlCommand("DELETE cms_Slideshow WHERE SlideSerial = @SlideSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("SlideSerial", SqlDbType.Int).Value = _SlideSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid ModuleID { get { return _ModuleID; } }
		public int SlideSerial { get { return _SlideSerial; } }
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					UpdateIntProperty("SortOrder", value);
				}
			}
		}
		public string SlideContents {
			get { return _SlideContents; }
			set {
				if (_SlideContents != value) {
					_SlideContents = UpdateStringProperty("SlideContents", Int32.MaxValue, value);
				}
			}
		}
		public string SlideImageFile {
			get { return _SlideImageFile; }
			set {
				if (_SlideImageFile != value) {
					_SlideImageFile = value;
					string baseDirectory = "/assets/images/slideshow/" + ModuleID + "/";
					Image objImage = Image.FromFile(HttpContext.Current.Server.MapPath(baseDirectory + _SlideImageFile));

					ImageSaveModel imageSaveModel = new ImageSaveModel();
					imageSaveModel.Height = objImage.Height.ToString();
					imageSaveModel.Width = objImage.Width.ToString();
					imageSaveModel.AltText = _SlideTitle;
					imageSaveModel.Src = _SlideImageFile;

					System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					System.IO.StringWriter writer = new System.IO.StringWriter(sb);
					ser.Serialize(writer, imageSaveModel);

					UpdateStringProperty("SlideImage", Int32.MaxValue, sb.ToString());
				}
			}
		}
		public string SlideImageTag {
			get {
				if (SlideImageFile.Length > 0) {
					return string.Format("<img alt=\"{0}\" src='{1}' height='{2}' width='{3}' class='img-responsive' />", SlideTitle, "/assets/images/slideshow/" + ModuleID.ToString() + "/" + SlideImageFile, _DimHeight, _DimWidth);
				} else {
					return "";
				}
			}
		}
		public string SlideLink {
			get { return _SlideLink; }
			set {
				if (_SlideLink != value) {
					_SlideLink = UpdateStringProperty("SlideLink", Int32.MaxValue, value);
				}
			}
		}
		public string SlideTitle {
			get { return _SlideTitle; }
			set {
				if (_SlideTitle != value) {
					_SlideTitle = UpdateStringProperty("SlideTitle", Int32.MaxValue, value);

					if (!string.IsNullOrEmpty(_SlideImageFile)) {
						//Update the image file's ALT attribute
						string baseDirectory = "/assets/images/slideshow/" + ModuleID + "/";
						Image objImage = Image.FromFile(HttpContext.Current.Server.MapPath(baseDirectory + _SlideImageFile));

						ImageSaveModel imageSaveModel = new ImageSaveModel();
						imageSaveModel.Height = objImage.Height.ToString();
						imageSaveModel.Width = objImage.Width.ToString();
						imageSaveModel.AltText = _SlideTitle;
						imageSaveModel.Src = _SlideImageFile;

						System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
						System.Text.StringBuilder sb = new System.Text.StringBuilder();
						System.IO.StringWriter writer = new System.IO.StringWriter(sb);
						ser.Serialize(writer, imageSaveModel);

						UpdateStringProperty("SlideImage", Int32.MaxValue, sb.ToString());
					}
				}
			}
		}
		#endregion
	}

	public class ContentSlideModel {
		#region Constructor
		public ContentSlideModel() { }
		public ContentSlideModel(ContentSlide data) {
			this.ModuleID = data.ModuleID;
			this.SlideContents = data.SlideContents;
			this.SlideImageFile = data.SlideImageFile;
			this.SlideImageTag = data.SlideImageTag;
			this.SlideLink = data.SlideLink;
			this.SlideSerial = data.SlideSerial;
			this.SlideTitle = data.SlideTitle;
			this.SortOrder = data.SortOrder;
		}
		#endregion

		#region Properties
		public Guid ModuleID { get; set; }
		public int SlideSerial { get; set; }
		public int SortOrder { get; set; }
		public string SlideContents { get; set; }
		public string SlideImageFile { get; set; }
		public string SlideImageTag { get; set; }
		public string SlideLink { get; set; }
		public string SlideTitle { get; set; }
		#endregion
	}

	public class ContentSlideshow {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		#endregion

		#region Constructor
		public ContentSlideshow(Guid ModuleID) {
			_ModuleID = ModuleID;
		}
		#endregion

		#region Public Methods
		public int Add(ContentSlideModel data) {
			int SlideSerial = 0;
			string slideImage = "";
			string slideContents = "";

			if (!string.IsNullOrEmpty(data.SlideImageFile)) {
				string baseDirectory = "/assets/images/slideshow/" + data.ModuleID + "/";
				Image objImage = Image.FromFile(HttpContext.Current.Server.MapPath(baseDirectory + data.SlideImageFile));

				ImageSaveModel imageSaveModel = new ImageSaveModel();
				imageSaveModel.Height = objImage.Height.ToString();
				imageSaveModel.Width = objImage.Width.ToString();
				imageSaveModel.AltText = data.SlideTitle;
				imageSaveModel.Src = data.SlideImageFile;

				System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				System.IO.StringWriter writer = new System.IO.StringWriter(sb);
				ser.Serialize(writer, imageSaveModel);

				slideImage = sb.ToString();
			} else {
				slideContents = data.SlideContents;
			}

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO cms_Slideshow (ModuleID, SlideTitle, SlideContents, SlideImage, SlideLink, SortOrder) " +
							"OUTPUT INSERTED.SlideSerial " +
							"VALUES (@ModuleID, @SlideTitle, @SlideContents, @SlideImage, @SlideLink, @SortOrder) ";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = data.ModuleID;
					cmd.Parameters.Add("SlideTitle", SqlDbType.VarChar).Value = data.SlideTitle ?? SqlString.Null;
					cmd.Parameters.Add("SlideContents", SqlDbType.VarChar).Value = slideContents;
					cmd.Parameters.Add("SlideImage", SqlDbType.VarChar).Value = slideImage;
					cmd.Parameters.Add("SlideLink", SqlDbType.VarChar).Value = data.SlideLink ?? SqlString.Null;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = 0;

					cmd.Connection.Open();
					SlideSerial = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}

			return SlideSerial;
		}
		public List<ContentSlideModel> AllSlides() {
			List<ContentSlideModel> l = new List<ContentSlideModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT SlideSerial FROM [cms_Slideshow] WHERE ModuleID = @ModuleID ORDER BY SortOrder, SlideTitle, SlideSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new ContentSlideModel(new ContentSlide(dr.GetInt32(0))));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		#endregion
	}
}
