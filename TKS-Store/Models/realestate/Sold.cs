using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TKS.Models.realestate {
	public class SoldSet {
		#region Fields
		private List<Sold> _Properties = new List<Sold>();
		#endregion

		#region Constructor
		public SoldSet() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT ItemSerial, Content, Thumbnail, Interior, Exterior FROM [Sold] ORDER BY SortOrder, ItemSerial DESC";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
                        while(dr.Read()) {
                            _Properties.Add(new Sold {
                                Serial = dr.GetInt32(0),
                                Content = dr[1].ToString(),
                                Thumbnail = dr[2].ToString(),
                                Interior = dr[3].ToString(),
                                Exterior = dr[4].ToString()
                            });
                        }
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Properties
		public List<Sold> Properties { get { return _Properties; } }
		#endregion
	}

	public class Sold {
        public int Serial { get; set; }
		public string Content { get; set; }
		public string Thumbnail { get; set; }
		public string Interior { get; set; }
		public string Exterior { get; set; }
	}
}