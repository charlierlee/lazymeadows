using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class Language {
		#region Fields
		private string _Locale = "";
		private string _Language = "";
		#endregion

		#region Constructor
		public Language(string Locale) {
			_Locale = Locale;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Language FROM cms_Language WHERE Locale = @Locale", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_Language = dr[0].ToString();
					} else {
						_Locale = "";
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public string Locale { get { return _Locale; } }
		public string LanguageName { get { return _Language; } }
		#endregion
	}
}