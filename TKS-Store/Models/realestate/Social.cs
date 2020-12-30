using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Models.realestate {
	public class SocialSites {
		//Client ID 	56ac4fdcb0ca4e70b13a70d68adee30a
		//Client Secret 	0cb38718e05c48b086ee59998b0e89dc
		//Website URL 	http://www.lazymeadowsrealty.com
		//Redirect URI 	http://www.lazymeadowsrealty.com
		//Support Email 	tom@techknowsys.com
		//https://instagram.com/oauth/authorize/?display=touch&client_id=56ac4fdcb0ca4e70b13a70d68adee30a&redirect_uri=http://www.lazymeadowsrealty.com/&response_type=token
		//https://api.instagram.com/v1/tags/SEARCH-TAG/media/recent?client_id=56ac4fdcb0ca4e70b13a70d68adee30a&callback=http://www.lazymeadowsrealty.com

		#region Fields
		private string _Town = "";
		private string _TownFormatted = "";
		private string _Col1 = "";
		private string _Col2 = "";
		private string _Col3 = "";
		#endregion

		#region Constructor
		public SocialSites(string town) {
			_Town = town.Replace("-", " ");
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT TownFormatted, Col1, Col2, Col3 FROM [SocialPageHeader] WHERE Town = @Town";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Parameters.Add("Town", SqlDbType.VarChar, 50).Value = _Town;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						if (dr.HasRows) {
							dr.Read();
							_TownFormatted = dr[0].ToString();
							_Col1 = dr[1].ToString();
							_Col2 = dr[2].ToString();
							_Col3 = dr[3].ToString();
						} else {
							_Town = "";
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
				_Town = "";
			}
		}
		#endregion

		#region Public Methods
		public List<SocialSite> GetPages() {
			List<SocialSite> l = new List<SocialSite>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT Town, PageType, URL, TitleText, ImageURL FROM [SocialPage] WHERE Town = @Town ORDER BY SortOrder, PageType, TitleText";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Parameters.Add("Town", SqlDbType.VarChar, 50).Value = _Town;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							SocialSite s = new SocialSite();
							s.Town = dr[0].ToString();
							s.PageType = dr[1].ToString();
							if (s.PageType == "InstagramEmbed") {
								string temp = dr[2].ToString();
								if (temp.IndexOf("<script") > 0) {
									s.URL = temp.Substring(0, temp.IndexOf("<script"));
								} else {
									s.URL = temp;
								}
							} else {
								s.URL = dr[2].ToString();
							}
							s.TitleText = dr[3].ToString();
                            s.ImageURL = dr[4].ToString();
							l.Add(s);
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception e) {
				Console.Write(e.Message);
			}
			return l;
		}
		#endregion

		#region Public Properties
		public string TownFormatted { get { return _TownFormatted; } }
		public string Col1 { get { return _Col1; } }
		public string Col2 { get { return _Col2; } }
		public string Col3 { get { return _Col3; } }
		#endregion
	}

	public class SocialSite {
		public string Town { get; set; }
		public string PageType { get; set; }
		public string URL { get; set; }
		public string TitleText { get; set; }
        public string ImageURL { get; set; }
	}
}