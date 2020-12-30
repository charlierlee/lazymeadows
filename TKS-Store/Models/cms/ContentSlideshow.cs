using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Text;
using System.Web;

namespace TKS.Models.CMS {
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
					var imageSaveModel = new TKS.Areas.Admin.Models.CMS.ImageSaveModel();
					System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
					doc.LoadXml(_SlideImage);
					System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(doc.DocumentElement);
					System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(imageSaveModel.GetType());
					imageSaveModel = (TKS.Areas.Admin.Models.CMS.ImageSaveModel)ser.Deserialize(reader);

					_SlideImageFile = imageSaveModel.Src;
					_DimHeight = Convert.ToInt32(imageSaveModel.Height);
					_DimWidth = Convert.ToInt32(imageSaveModel.Width);
				}
			}
		}
		#endregion

		#region Properties
		public Guid ModuleID { get { return _ModuleID; } }
		public int SlideSerial { get { return _SlideSerial; } }
		public int SortOrder {
			get { return _SortOrder; }
		}
		public string SlideContents {
			get { return _SlideContents; }
		}
		public string SlideImageFile {
			get { return _SlideImageFile; }
		}
		public string SlideImageTag {
			get {
				if (SlideImageFile.Length > 0) {
					return string.Format("<img alt=\"{0}\" src='{1}' />", SlideTitle, "/assets/images/slideshow/" + ModuleID.ToString() + "/" + SlideImageFile);
				} else {
					return "";
				}
			}
		}
		public string SlideLink {
			get { return _SlideLink; }
		}
		public string SlideTitle {
			get { return _SlideTitle; }
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
		public string Flexslider() {
			StringBuilder sb = new StringBuilder();
			List<ContentSlideModel> slides = AllSlides();
			if (slides.Count > 0) {
				sb.AppendLine("<div class='flexslider'>");
				sb.AppendLine("<ul class='slides'>");
				foreach (ContentSlideModel slide in slides) {
					if (!string.IsNullOrEmpty(slide.SlideContents)) {
						sb.AppendLine("<li>" + slide.SlideContents + "</li>");
					} else {
						sb.AppendLine("<li>" + slide.SlideImageTag + "</li>");
					}
				}
				sb.AppendLine("</ul>");
				sb.AppendLine("</div>");
			}

			return sb.ToString();
		}
		public string Nivoslider() {
			StringBuilder sb = new StringBuilder();
			List<ContentSlideModel> slides = AllSlides();
			if (slides.Count > 0) {

				sb.AppendLine("<div id='slider' class='nivoSlider'>");
				foreach (ContentSlideModel slide in slides) {
					string s = "";
					if (!string.IsNullOrEmpty(slide.SlideLink)) {
						s = "<a href='" + slide.SlideLink + "'>";
					} else {
						s = "<a href='#'>";
					}
					s += "<img src='/assets/images/slideshow/" + slide.ModuleID.ToString() + "/" + slide.SlideImageFile + "' alt='' title='#caption_" + slide.SlideSerial.ToString() + "' /></a>";
					sb.AppendLine(s);
				}
				sb.AppendLine("</div>");
				sb.AppendLine("<div class='caption'>");
				sb.AppendLine("	<div id='PlayPause'></div>");
				sb.AppendLine("</div>");
				foreach (ContentSlideModel slide in slides) {
					string s = "<div id='caption_" + slide.SlideSerial.ToString() + "' class='nivo-html-caption'>";
					if (!string.IsNullOrEmpty(slide.SlideTitle)) {
						string[] t = slide.SlideTitle.Split(new char[] {'|'});
						if (t.Length > 1) {
							s += "<div class='captiontitle'>" + t[0].Trim() + "</div><p>" + t[1].Trim() + "</p>";
						} else {
							s += "<div class='captiontitle'>" + slide.SlideTitle + "</div><p></p>";
						}
					} else {
						s += "<div class='captiontitle'></div><p></p>";
					}
					s += "</div>";
					sb.AppendLine(s);
				}
			}

			return sb.ToString();
		}
		#endregion
	}
}
