using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TKS;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Models {
	public class SEO {
		#region Fields
		private int _SeoID = 0;
		private Guid _EntityID = Guid.Empty;
		private string _fbAdmin = "";
		private string _gAuthor = "";
		private string _gDescription = "";
		private string _gImage = "";
		private string _gName = "";
		private string _gPublisher = "";
		private string _MetaDescription = "";
		private string _MetaKeywords = "";
		private string _MetaTitle = "";
		private string _ogDescription = "";
		private string _ogImage = "";
		private string _ogTitle = "";
		private string _ogType = "";
		private string _twitterCard = "";
		private string _twitterCreator = "";
		private string _twitterDescription = "";
		private string _twitterTitle = "";
		private string _twitterImage = "";
		#endregion

		#region Constructor
		public SEO(Guid EntityGUID) {
			_EntityID = EntityGUID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT [gAuthor], [gDescription], [gImage], [gName], [MetaKeywords], [MetaDescription], [MetaTitle], " +
							"	[ogDescription], [ogImage], [ogTitle], [ogType], [twitterCard], [twitterCreator], [twitterDescription], [twitterTitle], [twitterImage], [SeoID] " +
							"	FROM [cms_SEO] WHERE [EntityGUID] = @EntityGUID";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EntityGUID", SqlDbType.UniqueIdentifier).Value = _EntityID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
						dr.Read();
						_fbAdmin = TKS.Areas.Admin.Models.Global.FacebookAdmin;
						_gAuthor = dr[0].ToString();
						_gDescription = dr[1].ToString();
						_gImage = dr[2].ToString();
						_gName = dr[3].ToString();
						_gPublisher = TKS.Areas.Admin.Models.Global.GooglePublisher;
						_MetaDescription = dr[5].ToString();
						_MetaKeywords = dr[4].ToString();
						_MetaTitle = dr[6].ToString();
						_ogDescription = dr[7].ToString();
						_ogImage = dr[8].ToString();
						_ogTitle = dr[9].ToString();
						_ogType = dr[10].ToString();
						_twitterCard = dr[11].ToString();
						_twitterCreator = dr[12].ToString();
						_twitterDescription = dr[13].ToString();
						_twitterTitle = dr[14].ToString();
						_twitterImage = dr[15].ToString();
						_SeoID = dr.GetInt32(16);
					} else {
						_SeoID = 0;
					}
				}
			}
		}
		#endregion

		#region Private Methods
		private string UpdateStringProperty(string FieldName, int Length, string Value) {
			string ret = "";
			InitializeRecord();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_SEO SET " + FieldName + " = @Value WHERE EntityGUID = @EntityGUID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EntityGUID", SqlDbType.UniqueIdentifier).Value = EntityID;
					if (Length == Int32.MaxValue) {
						if (Value == null) {
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
						}
					} else {
						if (Value == null) {
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							if (ret.Length > Length) {
								ret = ret.Substring(0, Length);
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
		private void InitializeRecord() {
			if (_SeoID == 0) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "INSERT INTO cms_SEO (EntityGUID) " +
												"OUTPUT INSERTED.SeoID " +
												"VALUES (@EntityGUID) ";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("EntityGUID", SqlDbType.UniqueIdentifier).Value = EntityID;

						cmd.Connection.Open();
						_SeoID = (int)cmd.ExecuteScalar();
						cmd.Connection.Close();
					}
				}
			}
		}
		#endregion

		#region Public Method
		public List<SelectListItem> GetOGOptions(string SelectedItem) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			itemList.Add(new SelectListItem { Value = "website", Text = "website", Selected = ("website" == SelectedItem)});
			itemList.Add(new SelectListItem { Value = "article", Text = "article", Selected = ("article" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "book", Text = "book", Selected = ("book" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "profile", Text = "profile", Selected = ("profile" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "music.song", Text = "music.song", Selected = ("music.song" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "music.album", Text = "music.album", Selected = ("music.album" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "music.playlist", Text = "music.playlist", Selected = ("music.playlist" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "music.radio_station", Text = "music.radio_station", Selected = ("music.radio_station" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "video.movie", Text = "video.movie", Selected = ("video.movie" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "video.episode", Text = "video.episode", Selected = ("video.episode" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "video.tv_show", Text = "video.tv_show", Selected = ("video.tv_show" == SelectedItem) });
			itemList.Add(new SelectListItem { Value = "video.other", Text = "video.other", Selected = ("video.other" == SelectedItem) });

			return itemList;
		}
		public string GetTags(string URL, string DefaultTitle = "", string DefaultDescription = "") {
			StringBuilder sb = new StringBuilder();
			if (URL.Length > 0 && !URL.ToLower().StartsWith("http")) {
				URL = TKS.Areas.Admin.Models.Global.BaseURL + URL;
			} else {
				URL = TKS.Areas.Admin.Models.Global.BaseURL;
			}

			//Meta tags
			if (!string.IsNullOrEmpty(this.MetaTitle)) {
				sb.AppendLine(string.Format("<title>{0}</title>", this.MetaTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(DefaultTitle)) {
				sb.AppendLine(string.Format("<title>{0}</title>", DefaultTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else {
				sb.AppendLine(string.Format("<title>{0}</title>", TKS.Areas.Admin.Models.Global.TitleSuffix));
			}
			if (!string.IsNullOrEmpty(this.MetaDescription)) {
				sb.AppendLine(string.Format("<meta name='description' content=\"{0}\" />", this.MetaDescription));
			} else if (!string.IsNullOrEmpty(DefaultDescription)) {
				sb.AppendLine(string.Format("<meta name='description' content=\"{0}\" />", DefaultDescription));
			}
			if (!string.IsNullOrEmpty(this.MetaKeywords)) {
				sb.AppendLine(string.Format("<meta name='keywords' content=\"{0}\" />", this.MetaKeywords));
			}

			//OG / Facebook tags
			if (!string.IsNullOrEmpty(this.ogTitle)) {
				sb.AppendLine(string.Format("<meta property='og:title' content=\"{0}\" />", this.ogTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(DefaultTitle)) {
				sb.AppendLine(string.Format("<meta property='og:title' content=\"{0}\" />", DefaultTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			}
			if (!string.IsNullOrEmpty(this.ogDescription)) {
				sb.AppendLine(string.Format("<meta property='og:description' content=\"{0}\" />", this.ogDescription));
			} else if (!string.IsNullOrEmpty(DefaultDescription)) {
				sb.AppendLine(string.Format("<meta property='og:description' content=\"{0}\" />", DefaultDescription));
			}
			if (!string.IsNullOrEmpty(this.ogType)) {
				sb.AppendLine(string.Format("<meta property='og:type' content=\"{0}\" />", this.ogType));
			} else {
				sb.AppendLine("<meta property='og:type' content='website' />");
			}
			sb.AppendLine(string.Format("<meta property='og:url' content=\"{0}\" />", URL));
			if (!string.IsNullOrEmpty(this.ogImage)) {
				sb.AppendLine(string.Format("<meta property='og:image' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.BaseURL + this.ogImage));
			}
			sb.AppendLine(string.Format("<meta property='fb:admins' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.FacebookAdmin));
			
			//Google+ tags
			if (!string.IsNullOrEmpty(this.gAuthor)) { sb.AppendLine(string.Format("<link rel='author' href=\"{0}\" />", this.gAuthor)); }
			if (!string.IsNullOrEmpty(this.gPublisher)) { sb.AppendLine(string.Format("<link rel='publisher' href=\"{0}\" />", this.gPublisher)); }
			if (!string.IsNullOrEmpty(this.gName)) {
				sb.AppendLine(string.Format("<meta itemprop='name' content=\"{0}\" />", this.gName + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(this.ogTitle)) {
				sb.AppendLine(string.Format("<meta itemprop='name' content=\"{0}\" />", this.ogTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(DefaultTitle)) {
				sb.AppendLine(string.Format("<meta itemprop='name' content=\"{0}\" />", DefaultTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			}
			if (!string.IsNullOrEmpty(this.gDescription)) {
				sb.AppendLine(string.Format("<meta itemprop='description' content=\"{0}\" />", this.gDescription));
			} else if (!string.IsNullOrEmpty(this.ogDescription)) {
				sb.AppendLine(string.Format("<meta itemprop='description' content=\"{0}\" />", this.ogDescription));
			} else if (!string.IsNullOrEmpty(DefaultDescription)) {
				sb.AppendLine(string.Format("<meta itemprop='description' content=\"{0}\" />", DefaultDescription));
			}
			if (!string.IsNullOrEmpty(this.gImage)) {
				sb.AppendLine(string.Format("<meta itemprop='image' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.BaseURL + this.gImage));
			} else if (!string.IsNullOrEmpty(this.ogImage)) {
				sb.AppendLine(string.Format("<meta itemprop='image' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.BaseURL + this.gImage));
			}

			//Twitter tags
			sb.AppendLine("<meta name='twitter:card' content='summary'>");
			if (!string.IsNullOrEmpty(this.twitterTitle)) {
				sb.AppendLine(string.Format("<meta name='twitter:title' content=\"{0}\" />", this.twitterTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(this.ogTitle)) {
				sb.AppendLine(string.Format("<meta name='twitter:title' content=\"{0}\" />", this.ogTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(this.gName)) {
				sb.AppendLine(string.Format("<meta name='twitter:title' content=\"{0}\" />", this.gName + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			} else if (!string.IsNullOrEmpty(DefaultTitle)) {
				sb.AppendLine(string.Format("<meta name='twitter:title' content=\"{0}\" />", DefaultTitle + TKS.Areas.Admin.Models.Global.TitleSuffixSep + TKS.Areas.Admin.Models.Global.TitleSuffix));
			}
			if (!string.IsNullOrEmpty(this.twitterDescription)) {
				sb.AppendLine(string.Format("<meta name='twitter:description' content=\"{0}\" />", this.twitterDescription));
			} else if (!string.IsNullOrEmpty(this.ogDescription)) {
				sb.AppendLine(string.Format("<meta name='twitter:description' content=\"{0}\" />", this.ogDescription));
			} else if (!string.IsNullOrEmpty(this.gDescription)) {
				sb.AppendLine(string.Format("<meta name='twitter:description' content=\"{0}\" />", this.gDescription));
			} else if (!string.IsNullOrEmpty(DefaultDescription)) {
				sb.AppendLine(string.Format("<meta name='twitter:description' content=\"{0}\" />", DefaultDescription));
			}			
			if (!string.IsNullOrEmpty(this.twitterSite)) {
				sb.AppendLine(string.Format("<meta name='twitter:site' content=\"{0}\" />", this.twitterSite));
			}
			if (!string.IsNullOrEmpty(this.twitterCreator)) {
				sb.AppendLine(string.Format("<meta name='twitter:creator' content=\"{0}\" />", this.twitterCreator));
			}
			if (!string.IsNullOrEmpty(this.twitterImage)) {
				sb.AppendLine(string.Format("<meta name='twitter:image' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.BaseURL + this.twitterImage));
			} else if (!string.IsNullOrEmpty(this.ogImage)) {
				sb.AppendLine(string.Format("<meta name='twitter:image' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.BaseURL + this.ogImage));
			} else if (!string.IsNullOrEmpty(this.gImage)) {
				sb.AppendLine(string.Format("<meta name='twitter:image' content=\"{0}\" />", TKS.Areas.Admin.Models.Global.BaseURL + this.gImage));
			}

			//Other tags
			if (!string.IsNullOrEmpty(TKS.Areas.Admin.Models.Global.RSSFeedURL)) {
				sb.AppendLine("<link rel='alternate' type='application/rss+xml' title='" + TKS.Areas.Admin.Models.Global.FeedTitle + "'  href='" + TKS.Areas.Admin.Models.Global.RSSFeedURL + "' />");
			}
			if (!string.IsNullOrEmpty(TKS.Areas.Admin.Models.Global.NonMobileRelative)) {
				//sb.AppendLine("<link rel='canonical' href='" + Global.NonMobileRelative + this.URL + "' />");
			}
			if (!string.IsNullOrEmpty(TKS.Areas.Admin.Models.Global.MobileRelative)) {
				//sb.AppendLine("<link rel='alternate' media='only screen and (max-width: 640px)' href='" + Global.MobileRelative + this.URL + "' />");
			}

			return sb.ToString();
		}

		#endregion

		#region Public Properties
		public Guid EntityID { get { return _EntityID; } }
		public string fbAdmin { get { return _fbAdmin; } }
		public string gAuthor { get { return _gAuthor; } }
		public string gDescription {
			get { return _gDescription; }
			set {
				if (!_gDescription.Equals(value)) {
					_gDescription = UpdateStringProperty("gDescription", Int32.MaxValue, value);
				}
			}
		}
		public string gImage {
			get { return _gImage; }
			set {
				if (!_gImage.Equals(value)) {
					_gImage = UpdateStringProperty("gImage", 250, value);
				}
			}
		}
		public string gName {
			get { return _gName; }
			set {
				if (!_gName.Equals(value)) {
					_gName = UpdateStringProperty("gName", 400, value);
				}
			}
		}
		public string gPublisher { get { return _gPublisher; } }
		public string MetaDescription {
			get { return _MetaDescription; }
			set {
				if (!_MetaDescription.Equals(value)) {
					_MetaDescription = UpdateStringProperty("MetaDescription", Int32.MaxValue, value);
				}
			}
		}
		public string MetaKeywords {
			get { return _MetaKeywords; }
			set {
				if (!_MetaKeywords.Equals(value)) {
					_MetaKeywords = UpdateStringProperty("MetaKeywords", Int32.MaxValue, value);
				}
			}
		}
		public string MetaTitle {
			get { return _MetaTitle; }
			set {
				if (!_MetaTitle.Equals(value)) {
					_MetaTitle = UpdateStringProperty("MetaTitle", 400, value);
				}
			}
		}
		public string ogDescription {
			get { return _ogDescription; }
			set {
				if (!_ogDescription.Equals(value)) {
					_ogDescription = UpdateStringProperty("ogDescription", Int32.MaxValue, value);
				}
			}
		}
		public string ogImage {
			get { return _ogImage; }
			set {
				if (!_ogImage.Equals(value)) {
					_ogImage = UpdateStringProperty("ogImage", 250, value);
				}
			}
		}
		public string ogTitle {
			get { return _ogTitle; }
			set {
				if (!_ogTitle.Equals(value)) {
					_ogTitle = UpdateStringProperty("ogTitle", 400, value);
				}
			}
		}
		public string ogType {
			get { return _ogType; }
			set {
				if (!_ogType.Equals(value)) {
					_ogType = UpdateStringProperty("ogType", 100, value);
				}
			}
		}
		public string twitterCard {
			get { return _twitterCard; }
			set {
				if (!_twitterCard.Equals(value)) {
					_twitterCard = UpdateStringProperty("twitterCard", 150, value);
				}
			}
		}
		public string twitterCreator {
			get {
				if (!string.IsNullOrEmpty(TKS.Areas.Admin.Models.Global.TwitterCreator)) {
					if (TKS.Areas.Admin.Models.Global.TwitterCreator.StartsWith("@")) {
						return TKS.Areas.Admin.Models.Global.TwitterCreator;
					} else {
						return "@" + TKS.Areas.Admin.Models.Global.TwitterCreator;
					}
				}
				return "";
			}
		}
		public string twitterDescription {
			get { return _twitterDescription; }
			set {
				if (!_twitterDescription.Equals(value)) {
					_twitterDescription = UpdateStringProperty("twitterDescription", Int32.MaxValue, value);
				}
			}
		}
		public string twitterSite {
			get {
				if (!string.IsNullOrEmpty(TKS.Areas.Admin.Models.Global.TwitterName)) {
					if (TKS.Areas.Admin.Models.Global.TwitterCreator.StartsWith("@")) {
						return TKS.Areas.Admin.Models.Global.TwitterName;
					} else {
						return "@" + TKS.Areas.Admin.Models.Global.TwitterName;
					}
				}
				return "";
			}
		}
		public string twitterTitle {
			get { return _twitterTitle; }
			set {
				if (!_twitterTitle.Equals(value)) {
					_twitterTitle = UpdateStringProperty("twitterTitle", 400, value);
				}
			}
		}
		public string twitterImage {
			get { return _twitterImage; }
			set {
				if (!_twitterImage.Equals(value)) {
					_twitterImage = UpdateStringProperty("twitterImage", 250, value);
				}
			}
		}
		#endregion
	}

	public class SEOViewModel {
		#region Constructor
		public SEOViewModel() { }
		public SEOViewModel(SEO data) {
			this.EntityID = data.EntityID;
			this.fbAdmin = data.fbAdmin;
			this.gAuthor = data.gAuthor;
			this.gDescription = data.gDescription;
			this.gImage = data.gImage;
			this.gName = data.gName;
			this.gPublisher = data.gPublisher;
			this.MetaDescription = data.MetaDescription;
			this.MetaKeywords = data.MetaKeywords;
			this.MetaTitle = data.MetaTitle;
			this.ogDescription = data.ogDescription;
			this.ogImage = data.ogImage;
			this.ogTitle = data.ogTitle;
			this.ogType = data.ogType;
			this.twitterCard = data.twitterCard;
			this.twitterCreator = data.twitterCreator;
			this.twitterDescription = data.twitterDescription;
			this.twitterImage = data.twitterImage;
			this.twitterTitle = data.twitterTitle;
		}
		#endregion

		#region Public Properties
		public Guid EntityID { get; set; }
		public string fbAdmin { get; set; }
		public string gAuthor { get; set; }
		public string gDescription { get; set; }
		public string gImage { get; set; }
		public string gName { get; set; }
		public string gPublisher { get; set; }
		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string MetaTitle { get; set; }
		public string ogDescription { get; set; }
		public string ogImage { get; set; }
		public string ogTitle { get; set; }
		public string ogType { get; set; }
		public string twitterCard { get; set; }
		public string twitterCreator { get; set; }
		public string twitterDescription { get; set; }
		public string twitterTitle { get; set; }
		public string twitterImage { get; set; }
		#endregion
	}
}