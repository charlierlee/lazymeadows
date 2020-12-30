using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class Contact {
		#region Private Fields
		private DateTime _SubmittedOn = DateTime.MinValue;
		private int _ContactSerial = 0;
		private int _Subscribe = 0;
		private string _Address1 = "";
		private string _Address2 = "";
		private string _City = "";
		private string _Company = "";
		private string _ContactIP = "";
		private string _Email = "";
		private string _FirstName = "";
		private string _FullName = "";
		private string _LastName = "";
		private string _Message = "";
		private string _Phone = "";
		private string _Source = "";
		private string _State = "";
		private string _Title = "";
		private string _URL = "";
		private string _ZIPCode = "";
		#endregion

		#region Constructor
		public Contact() { }
		public Contact(int contactSerial) {
			_ContactSerial = contactSerial;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT * FROM cms_Contact WHERE ContactSerial = @ContactSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ContactSerial", SqlDbType.Int).Value = ContactSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_Address1 = dr["Address1"].ToString();
						_Address2 = dr["Address2"].ToString();
						_City = dr["City"].ToString();
						_Company = dr["Company"].ToString();
						_ContactIP = dr["ContactIP"].ToString();
						_Email = dr["Email"].ToString();
						_FirstName = dr["FirstName"].ToString();
						_FullName = dr["FullName"].ToString();
						_LastName = dr["LastName"].ToString();
						_Message = dr["Message"].ToString();
						_Phone = dr["Phone"].ToString();
						_Source = dr["Source"].ToString();
						_State = dr["State"].ToString();
						_SubmittedOn = Convert.ToDateTime(dr["SubmittedOn"]);
						_Subscribe = Convert.ToInt32(dr["Subscribe"]);
						_Title = dr["Title"].ToString();
						_URL = dr["URL"].ToString();
						_ZIPCode = dr["ZIPCode"].ToString();
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateIntProperty(string FieldName, int? Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Contact] SET " + FieldName + " = @Value WHERE ContactSerial = @ContactSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ContactSerial", SqlDbType.Int).Value = ContactSerial;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Contact] SET " + FieldName + " = @Value WHERE ContactSerial = @ContactSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ContactSerial", SqlDbType.Int).Value = ContactSerial;
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
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_Contact WHERE ContactSerial = @ContactSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ContactSerial", SqlDbType.Int).Value = ContactSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Properties
		public int ContactSerial { get { return _ContactSerial; } }
		public DateTime SubmittedOn { get { return _SubmittedOn; } }
		public string Address1 {
			get { return _Address1; }
			set {
				if (_Address1 != value) {
					_Address1 = UpdateStringProperty("Address1", 250, value);
				}
			}
		}
		public string Address2 {
			get { return _Address2; }
			set {
				if (_Address2 != value) {
					_Address2 = UpdateStringProperty("Address2", 250, value);
				}
			}
		}
		public string City {
			get { return _City; }
			set {
				if (_City != value) {
					_City = UpdateStringProperty("City", 250, value);
				}
			}
		}
		public string Company {
			get { return _Company; }
			set {
				if (_Company != value) {
					_Company = UpdateStringProperty("Company", 250, value);
				}
			}
		}
		public string ContactIP {
			get { return _ContactIP; }
			set {
				if (_ContactIP != value) {
					_ContactIP = UpdateStringProperty("ContactIP", 15, value);
				}
			}
		}
		public string Email {
			get { return _Email; }
			set {
				if (_Email != value) {
					_Email = UpdateStringProperty("Email", 250, value);
				}
			}
		}
		public string FirstName {
			get { return _FirstName; }
			set {
				if (_FirstName != value) {
					_FirstName = UpdateStringProperty("FirstName", 250, value);
				}
			}
		}
		public string FullName {
			get { return _FullName; }
			set {
				if (_FullName != value) {
					_FullName = UpdateStringProperty("FullName", 250, value);
				}
			}
		}
		public string LastName {
			get { return _LastName; }
			set {
				if (_LastName != value) {
					_LastName = UpdateStringProperty("LastName", 250, value);
				}
			}
		}
		public string Message {
			get { return _Message; }
			set {
				if (_Message != value) {
					_Message = UpdateStringProperty("Message", Int32.MaxValue, value);
				}
			}
		}
		public string Phone {
			get { return _Phone; }
			set {
				if (_Phone != value) {
					_Phone = UpdateStringProperty("Phone", 250, value);
				}
			}
		}
		public string Source {
			get { return _Source; }
			set {
				if (_Source != value) {
					_Source = UpdateStringProperty("Source", 150, value);
				}
			}
		}
		public string State {
			get { return _State; }
			set {
				if (_State != value) {
					_State = UpdateStringProperty("State", 250, value);
				}
			}
		}
		public string Title {
			get { return _Title; }
			set {
				if (_Title != value) {
					_Title = UpdateStringProperty("Title", 250, value);
				}
			}
		}
		public string URL {
			get { return _URL; }
			set {
				if (_URL != value) {
					_URL = UpdateStringProperty("URL", 250, value);
				}
			}
		}
		public string ZIPCode {
			get { return _ZIPCode; }
			set {
				if (_ZIPCode != value) {
					_ZIPCode = UpdateStringProperty("ZIPCode", 250, value);
				}
			}
		}
		public int Subscribe {
			get { return _Subscribe; }
			set {
				if (_Subscribe != value) {
					_Subscribe = value;
					UpdateIntProperty("Subscribe", value);
				}
			}
		}
		#endregion
	}

	public class Contacts {
		public int Add(ContactViewModel size) {
			int SizeID = 0;
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	string SQL = "INSERT INTO Size (Size, SKUExtension, SortOrder) " +
			//				"OUTPUT INSERTED.SizeID " +
			//				"VALUES (@Size, @SKUExtension, @SortOrder) ";
			//	using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("Size", SqlDbType.VarChar, 50).Value = size.SizeName;
			//		cmd.Parameters.Add("SKUExtension", SqlDbType.VarChar, 50).Value = size.SKUExtension;
			//		cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = size.SortOrder;

			//		cmd.Connection.Open();
			//		SizeID = (int)cmd.ExecuteScalar();
			//		cmd.Connection.Close();
			//	}
			//}

			return SizeID;
		}

		public List<ContactViewModel> GetList() {
			try {
				List<ContactViewModel> sizes = new List<ContactViewModel>();
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT ContactSerial FROM cms_Contact ORDER BY ContactSerial DESC";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Connection.Open();

						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							sizes.Add(new ContactViewModel(new Contact(dr.GetInt32(0))));
						}
						cmd.Connection.Close();
					}
				}

				return sizes;
			} catch (Exception) {
				return null;
			}
		}

		public string GetContacts() {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT * FROM cms_Contact ORDER BY ContactSerial DESC";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					DataTable tempData;

					cmd.CommandType = CommandType.Text;

					SqlDataAdapter ad = new SqlDataAdapter(cmd);
					ad.Fill(tempData = new DataTable());
					cmd.Dispose();
					ad.Dispose();

					for (int i = 0; i < tempData.Columns.Count; i++) {
						sb.Append(tempData.Columns[i].ColumnName + ',');
					}
					sb.Append("\n");

					foreach (DataRow row in tempData.Rows) {
						for (int i = 0; i < tempData.Columns.Count; i++) {
							sb.Append(row[i].ToString() + ',');
						}
						sb.Append("\n");
					}
				}
			}
			return sb.ToString();
		}
	}

	public class ContactViewModel {
		#region Constructor
		public ContactViewModel() { }
		public ContactViewModel(Contact data) {
			this.Address1 = data.Address1;
			this.Address2 = data.Address2;
			this.City = data.City;
			this.Company = data.Company;
			this.ContactIP = data.ContactIP;
			this.ContactSerial = data.ContactSerial;
			this.Email = data.Email;
			this.FirstName = data.FirstName;
			this.FullName = data.FullName;
			this.LastName = data.LastName;
			this.Message = data.Message;
			this.Phone = data.Phone;
			this.Source = data.Source;
			this.State = data.State;
			this.SubmittedOn = data.SubmittedOn;
			this.Subscribe = data.Subscribe;
			this.Title = data.Title;
			this.URL = data.URL;
			this.ZIPCode = data.ZIPCode;
		}
		#endregion

		#region Public Properties
		public int ContactSerial { get; set; }
		public string Source { get; set; }
		public DateTime SubmittedOn { get; set; }
		public string ContactIP { get; set; }
		public string FullName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Title { get; set; }
		public string Company { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZIPCode { get; set; }
		public string Email { get; set; }
		public string URL { get; set; }
		public string Phone { get; set; }
		public string Message { get; set; }
		public int Subscribe { get; set; }
		#endregion
	}
}