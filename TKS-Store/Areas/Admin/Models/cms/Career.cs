using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Microsoft.SqlServer.Server;

namespace TKS.Areas.Admin.Models.CMS {
	public class Career {
		#region Fields
		private int _CareerID = 0;
		private string _CareerTitle = "";
		private string _LocationState = "";
		private string _LocationCity = "";
		private string _Description = "";
		private int _Priority = 0;
		private string _URL = "";
		private DateTime _DatePosted;
		private bool _IsActive = true;
		#endregion

		#region Constructor
		public Career(int CareerID) {
			_CareerID = CareerID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM cms_Career WHERE CareerID = @CareerID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("CareerID", SqlDbType.Int).Value = _CareerID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_CareerTitle = dr["CareerTitle"].ToString();
						_LocationState = dr["LocationState"].ToString();
						_LocationCity = dr["LocationCity"].ToString();
						_Description = dr["Description"].ToString();
						_Priority = dr.GetInt32(dr.GetOrdinal("Priority"));
						_URL = dr["URL"].ToString();
						_DatePosted = dr.GetDateTime(dr.GetOrdinal("DatePosted"));
						_IsActive = dr.GetBoolean(dr.GetOrdinal("IsActive"));
					} else {
						_CareerID = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public int CareerID { get { return _CareerID; } }
		public string CareerTitle {
			get { return _CareerTitle; }
			set {
				if (_CareerTitle != value) {
					_CareerTitle = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET CareerTitle = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string CareerURL {
			get { return "/careers/" + this.CareerID + "/" +  tksUtil.FormatRouteURL(this.Location + "-" + this.CareerTitle); }
		}
		public string Description {
			get { return _Description; }
			set {
				if (_Description != value) {
					_Description = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET Description = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.VarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string LocationCity {
			get { return _LocationCity; }
			set {
				if (_LocationCity != value) {
					_LocationCity = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET LocationCity = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string LocationState {
			get { return _LocationState; }
			set {
				if (_LocationState != value) {
					_LocationState = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET LocationState = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string Location {
			get {
				string ret = "";
				if (!string.IsNullOrEmpty(LocationCity)) {
					ret += LocationCity;
					if (!string.IsNullOrEmpty(LocationState)) {
						ret += ", " + LocationState;
					}
				} else {
					ret += LocationState;
				}
				return ret;
			}
		}
		public int Priority {
			get { return _Priority; }
			set {
				if (_Priority != value) {
					_Priority = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET Priority = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.Int).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string URL {
			get { return _URL; }
			set {
				if (_URL != value) {
					_URL = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET URL = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public DateTime DatePosted {
			get { return _DatePosted; }
			set {
				if (_DatePosted != value) {
					_DatePosted = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET DatePosted = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.DateTime).Value = value != DateTime.MinValue ? value : DateTime.Now;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public bool IsActive {
			get { return _IsActive; }
			set {
				if (_IsActive != value) {
					_IsActive = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Career SET IsActive = @Value WHERE CareerID = @ID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;
							cmd.Parameters.Add("@Value", SqlDbType.Bit).Value = value ? 1 : 0;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}

		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE cms_Career WHERE CareerID = @ID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@ID", SqlDbType.Int).Value = _CareerID;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion
	}

	public class CareerSet {
		#region Public Methods
		public void Add(CareerViewModel careerViewModel) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO cms_Career (" +
								"[CareerTitle], [LocationState], [LocationCity], [Description], [Priority], [URL], [DatePosted], [IsActive] " +
								") VALUES (" +
								"@CareerTitle, @LocationState, @LocationCity, @Description, @Priority, @URL, @DatePosted, @IsActive " +
								")";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@CareerTitle", SqlDbType.VarChar, 250).Value = careerViewModel.CareerTitle;
					cmd.Parameters.Add("@LocationState", SqlDbType.VarChar, 2).Value = careerViewModel.LocationState;
					cmd.Parameters.Add("@LocationCity", SqlDbType.VarChar, 250).Value = careerViewModel.LocationCity;
					cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = careerViewModel.Description;
					cmd.Parameters.Add("@Priority", SqlDbType.Int).Value = careerViewModel.Priority;
					cmd.Parameters.Add("@URL", SqlDbType.VarChar, 250).Value = careerViewModel.URL;
					if (careerViewModel.DatePosted != DateTime.MinValue) {
						cmd.Parameters.Add("@DatePosted", SqlDbType.DateTime).Value = careerViewModel.DatePosted;
					} else {
						cmd.Parameters.Add("@DatePosted", SqlDbType.DateTime).Value = DateTime.Now;
					}
					cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = careerViewModel.IsActive ? 1 : 0;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}

		public List<Career> Careers(bool IncludeInactive = false) {
			List<Career> list = new List<Career>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL;
				if (IncludeInactive) {
					SQL = "SELECT [CareerID] FROM cms_Career ORDER BY Priority DESC, DatePosted DESC";
				} else {
					SQL = "SELECT [CareerID] FROM cms_Career WHERE IsActive = 1 ORDER BY Priority DESC, DatePosted DESC";
				}
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						list.Add(new Career(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
			}
			return list;
		}
		#endregion
	}

	public class CareerViewModel {
		public int CareerID { get; set; }
		public string CareerTitle { get; set; }
		public string LocationCity { get; set; }
		public string LocationState { get; set; }
		public string Description { get; set; }
		public int Priority { get; set; }
		public string URL { get; set; }
		public DateTime DatePosted { get; set; }
		public bool IsActive { get; set; }
	}
}