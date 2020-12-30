using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using TKS.Models.realestate;

namespace TKS.Areas.Admin.Models.LazyMeadows {
	public class Favorite {
		#region Fields
		private int _FavoriteID = 0;
		private int _SortOrder = 0;
		private string _MLS = "";
		private string _Town = "";
		private string _Price = "";
		private string _Description = "";
		private string _Link = "";
		private string _Photo = "";
		#endregion

		#region Constructor
		public Favorite() { }
		public Favorite(int FavoriteID) {
			_FavoriteID = FavoriteID;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT [MLS],[Town],[Price],[Description],[Link],[Photo],[SortOrder] FROM [Favorites] WHERE FavoriteID = @FavoriteID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FavoriteID", SqlDbType.Int).Value = _FavoriteID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_MLS = dr[0].ToString();
						_Town = dr[1].ToString();
						_Price = dr[2].ToString();
						_Description = dr[3].ToString();
						_Link = dr[4].ToString();
						_Photo = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
						if (!string.IsNullOrEmpty(_MLS)) {
							Listing l = new Listing(_MLS);
							_Town = l.AddressOneLine;
						} else {
							_MLS = "Not in MLS";
						}
					} else {
						_FavoriteID = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private string UpdateStringProperty(string FieldName, int Length, string Value) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE Favorites SET " + FieldName + " = @Value WHERE FavoriteID = @FavoriteID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FavoriteID", SqlDbType.Int).Value = _FavoriteID;
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
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM Favorites WHERE FavoriteID = @FavoriteID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@FavoriteID", SqlDbType.Int).Value = _FavoriteID;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public int FavoriteID { get { return _FavoriteID; } }
		public string MLS {
			get { return _MLS; }
			set {
				if (!_MLS.Equals(value)) {
					_MLS = UpdateStringProperty("MLS", 50, value);
				}
			}
		}
		public int SortOrder {
			get { return _SortOrder; }
			set {
				_SortOrder = value;
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("UPDATE Favorites SET SortOrder = @SortOrder WHERE FavoriteID = @FavoriteID", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("FavoriteID", SqlDbType.Int).Value = _FavoriteID;
						cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = value;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
				}
			}
		}
		public string Description {
			get { return _Description; } 
			set {
				if (!_Description.Equals(value)) {
					_Description = UpdateStringProperty("Description", Int32.MaxValue, value);
				}
			}
		}
		public string Town {
			get { return _Town; }
			set {
				if (!_Town.Equals(value)) {
					_Town = UpdateStringProperty("Town", 50, value);
				}
			}
		}
		public string Price {
			get { return _Price; }
			set {
				if (!_Price.Equals(value)) {
					_Price = UpdateStringProperty("Price", 50, value);
				}
			}
		}
		public string Link {
			get { return _Link; }
			set {
				if (!_Link.Equals(value)) {
					_Link = UpdateStringProperty("Link", 250, value);
				}
			}
		}
		public string Photo {
			get { return _Photo; }
			set {
				if (!_Photo.Equals(value)) {
					_Photo = UpdateStringProperty("Photo", 250, value);
				}
			}
		}
		#endregion
	}

	public class Favorites {
		#region Fields
		#endregion

		#region Constructor
		public Favorites() { }
		#endregion

		#region Public Methods
		public int Add(Favorite data) {
			int FavoriteID = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO [Favorites] ( " +
							"	[MLS], [Town], [Price], [Description], [Link], [Photo], [SortOrder], [IsActive] " +
							") OUTPUT Inserted.FavoriteID VALUES ( " +
							"	@MLS, @Town, @Price, @Description, @Link, @Photo, 0, 1  " +
							")";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (string.IsNullOrEmpty(data.MLS)) {
						cmd.Parameters.Add("@MLS", SqlDbType.VarChar, 50).Value = SqlString.Null;
						cmd.Parameters.Add("@Town", SqlDbType.VarChar, 50).Value = data.Town;
						if (data.Price == null) {
							cmd.Parameters.Add("@Price", SqlDbType.VarChar, 50).Value = SqlString.Null;
						} else {
							cmd.Parameters.Add("@Price", SqlDbType.VarChar, 50).Value = data.Price;
						}
						cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = data.Description;
						cmd.Parameters.Add("@Link", SqlDbType.VarChar, 250).Value = data.Link;
						cmd.Parameters.Add("@Photo", SqlDbType.VarChar, 250).Value = data.Photo;
					} else {
						cmd.Parameters.Add("@MLS", SqlDbType.VarChar, 50).Value = data.MLS;
						cmd.Parameters.Add("@Town", SqlDbType.VarChar, 50).Value = SqlString.Null;
						cmd.Parameters.Add("@Price", SqlDbType.VarChar, 50).Value = SqlString.Null;
						cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = SqlString.Null;
						cmd.Parameters.Add("@Link", SqlDbType.VarChar, 250).Value = SqlString.Null;
						cmd.Parameters.Add("@Photo", SqlDbType.VarChar, 250).Value = SqlString.Null;
					}

					cmd.Connection.Open();
					FavoriteID = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return FavoriteID;
		}
		public List<Favorite> GetList() {
			List<Favorite> l = new List<Favorite>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT FavoriteID FROM Favorites ORDER BY SortOrder, MLS, Town";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new Favorite(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		#endregion

		#region Private Methods
		#endregion
	}
}