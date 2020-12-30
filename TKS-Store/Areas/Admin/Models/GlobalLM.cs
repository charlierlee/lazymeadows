using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Areas.Admin.Models {
	public class GlobalLM {
		public string Favorites { get { return ConfigString("Favorites"); } set { UpdateStringProperty("Favorites", value); } }
		public double DownPayment { get { return ConfigDouble("DownPayment"); } set { UpdateStringProperty("DownPayment", value.ToString()); } }
		public double Fixed30Year { get { return ConfigDouble("30YearFixed"); } set { UpdateStringProperty("30YearFixed", value.ToString()); } }
		public double Fixed15Year { get { return ConfigDouble("15YearFixed"); } set { UpdateStringProperty("15YearFixed", value.ToString()); } }
		public double ARM51 { get { return ConfigDouble("51ARM"); } set { UpdateStringProperty("51ARM", value.ToString()); } }

		private string ConfigString(string key) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Value FROM Config WHERE Name = @Name", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Name", SqlDbType.VarChar, 50).Value = key;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						ret = dr[0].ToString();
					}
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		private bool ConfigBool(string key) {
			return ConfigString(key) == "1" ? true : false;
		}
		private int ConfigInt(string key) {
			string ret = ConfigString(key);
			int x = 0;
			if (Int32.TryParse(ret, out x)) {
				return x;
			} else {
				return 0;
			}
		}
		private double ConfigDouble(string key) {
			string ret = ConfigString(key);
			double x = 0;
			if (double.TryParse(ret, out x)) {
				return x;
			} else {
				return 0;
			}
		}

		private string UpdateStringProperty(string key, string Value) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE Config SET [Value] = @Value WHERE [Name] = @key", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("key", SqlDbType.VarChar, 50).Value = key;
					if (Value == null) {
						cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = "";
					} else {
						ret = Value.Trim();
						cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
					}
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
			return ret;
		}
	}

	public class PaymentModel {
		public double DownPayment { get; set; }
		public double Fixed30Year { get; set; }
		public double Fixed15Year { get; set; }
		public double ARM51 { get; set; }
	}
}