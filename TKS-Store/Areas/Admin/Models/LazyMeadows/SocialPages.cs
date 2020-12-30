using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace TKS.Areas.Admin.Models.LazyMeadows{
	public class SocialPages {
		public List<string> Towns() {
			List<string> l = new List<string>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Town FROM SocialPageHeader ORDER BY Town", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(dr[0].ToString());
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
	}

	public class SocialPage {
		#region Fields
		private string _Town = "";
		private string _TownFormatted = "";
		private string _Col1 = "";
		private string _Col2 = "";
		private string _Col3 = "";
		#endregion

		#region Constructor
		public SocialPage() { }
		public SocialPage(string town) {
			_Town = town;
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT Town, Col1, Col2, Col3 FROM [SocialPageHeader] WHERE Town = @Town";

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

		#region Private Methods
		private string UpdateStringProperty(string FieldName, int Length, string Value) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE SocialPageHeader SET " + FieldName + " = @Value WHERE Town = @Town", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Town", SqlDbType.VarChar, 50).Value = _Town;
					if (Length == Int32.MaxValue) {
						if (Value == null) {
							ret = null;
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
						}
					} else {
						if (Value == null) {
							ret = null;
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
		#endregion

		#region Public Methods
		public SocialSite GetPage(int SocialPageSerial) {
			SocialSite s = new SocialSite();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT Town, PageType, URL, TitleText, ImageURL FROM [SocialPage] WHERE SocialPageSerial = @SocialPageSerial";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = SocialPageSerial;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						if (dr.HasRows) {
							dr.Read();
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
							s.SocialPageSerial = SocialPageSerial;
							s.ImageURL = dr[4].ToString();
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return s;
		}
		public List<SocialSite> GetPages() {
			List<SocialSite> l = new List<SocialSite>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT Town, PageType, URL, TitleText, SocialPageSerial, ImageURL FROM [SocialPage] WHERE Town = @Town ORDER BY SortOrder, PageType, TitleText";

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
							s.SocialPageSerial = dr.GetInt32(4);
							s.ImageURL = dr[5].ToString();
							l.Add(s);
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return l;
		}
		public void UpdateSortOrder(int SocialPageSerial, int SortOrder) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand()) {
					cmd.Connection = cn;
					cmd.CommandType = CommandType.Text;

					cmd.CommandText = "UPDATE SocialPage SET SortOrder = @SortOrder WHERE SocialPageSerial = @SocialPageSerial";
					cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = SocialPageSerial;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = SortOrder;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void AddCuratedPage(SocialSite data) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand()) {
					cmd.Connection = cn;
					cmd.CommandType = CommandType.Text;

					//Make room at the top
					cmd.CommandText = "UPDATE SocialPage SET SortOrder = SortOrder + 1 WHERE Town = @Town";
					cmd.Parameters.Add("Town", SqlDbType.VarChar, 50).Value = data.Town;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "INSERT SocialPage (Town, PageType, URL, ImageURL, TitleText, SortOrder) " +
						"VALUES (@Town, @PageType, @URL, @ImageURL, @TitleText, 0)";
					cmd.Parameters.Add("PageType", SqlDbType.VarChar, 50).Value = data.PageType;
					cmd.Parameters.Add("URL", SqlDbType.VarChar).Value = (data.URL + "").Trim();
					cmd.Parameters.Add("ImageURL", SqlDbType.VarChar).Value = (data.ImageURL + "").Trim();
					cmd.Parameters.Add("TitleText", SqlDbType.VarChar, 250).Value = (data.TitleText + "").Trim();
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void EditCuratedPage(SocialSite data) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand()) {
					cmd.Connection = cn;
					cmd.CommandType = CommandType.Text;

					//Make room at the top
					cmd.CommandText = "UPDATE SocialPage SET PageType = @PageType, URL = @URL, ImageURL = @ImageURL, TitleText = @TitleText WHERE SocialPageSerial = @SocialPageSerial";
					cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = data.SocialPageSerial;
					cmd.Parameters.Add("PageType", SqlDbType.VarChar, 50).Value = data.PageType;
					cmd.Parameters.Add("URL", SqlDbType.VarChar).Value = data.URL;
					cmd.Parameters.Add("ImageURL", SqlDbType.VarChar).Value = data.ImageURL;
					cmd.Parameters.Add("TitleText", SqlDbType.VarChar, 250).Value = data.TitleText;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void RemoveCuratedPage(int SocialPageSerial) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand()) {
					cmd.Connection = cn;
					cmd.CommandType = CommandType.Text;

					//Make room at the top
					cmd.CommandText = "DELETE SocialPage WHERE SocialPageSerial = @SocialPageSerial";
					cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = SocialPageSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion


		#region Public Properties
		public string Town { get { return _Town; } }
		public string TownFormatted { get { return _TownFormatted; } }
		public string Col1 {
			get { return _Col1; }
			set {
				if (!_Col1.Equals(value)) {
					_Col1 = UpdateStringProperty("Col1", Int32.MaxValue, value);
				}
			}
		}
		public string Col2 {
			get { return _Col2; }
			set {
				if (!_Col2.Equals(value)) {
					_Col2 = UpdateStringProperty("Col2", Int32.MaxValue, value);
				}
			}
		}
		public string Col3 {
			get { return _Col3; }
			set {
				if (!_Col3.Equals(value)) {
					_Col3 = UpdateStringProperty("Col3", Int32.MaxValue, value);
				}
			}
		}
		#endregion
	}

	public class SocialSite {
		public int SocialPageSerial { get; set; }
		public string ImageURL { get; set; }
		public string PageType { get; set; }
		public string TitleText { get; set; }
		public string Town { get; set; }
		public string URL { get; set; }
	}
}