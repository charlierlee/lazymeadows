using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TKS.Models {
	public class Address {
		#region Fields
		private int m_AddressID = 0;
		private Guid m_AddressGUID = Guid.Empty;
		private string m_NickName = "";
		private string m_StateProvinceName = "";
		private string m_CountryName = "";
		private DateTime m_CreateDate = DateTime.MinValue;
		#endregion

		#region Constructor
		public Address() { }
		public Address(int addressId) {
			m_AddressID = addressId;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT a.*, s.Abbreviation AS StateName, c.Name AS CountryName, c.AddressOrderFormat " +
										 "FROM [Address] a LEFT JOIN lu_Country c ON a.countryid = c.countryid " +
										 "LEFT JOIN lu_StateProvince s ON a.StateProvinceID = s.StateProvinceID " +
										 "WHERE a.AddressID = @AddressID";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@AddressID", SqlDbType.Int).Value = AddressID;
					cmd.Connection.Open();

					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						m_AddressGUID = new Guid(dr["AddressGUID"].ToString());
						NickName = dr["NickName"].ToString();
						FirstName = dr["FirstName"].ToString();
						LastName = dr["LastName"].ToString();
						Company = dr["Company"].ToString();
						Address1 = dr["AddressLine1"].ToString();
						Address2 = dr["AddressLine2"].ToString();
						Suite = dr["Suite"].ToString();
						City = dr["City"].ToString();
						StateProvinceID = int.Parse(dr["StateProvinceID"].ToString());
						m_StateProvinceName = dr["StateName"].ToString();
						ZipPostal = dr["ZipPostal"].ToString();
						CountryID = int.Parse(dr["CountryID"].ToString());
						m_CountryName = dr["CountryName"].ToString();
						IsResidential = int.Parse(dr["IsResidential"].ToString()) == 1;
						Phone = dr["Phone"].ToString();
						IsActive = bool.Parse(dr["IsActive"].ToString());
						m_CreateDate = DateTime.Parse(dr["CreateDate"].ToString());
					}
					cmd.Connection.Close();
				}
			}
		}
		public Address(Guid addressGuid) {
			m_AddressGUID = addressGuid;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT a.*, s.Abbreviation AS StateName, c.Name AS CountryName, c.AddressOrderFormat " +
										 "FROM [Address] a LEFT JOIN lu_Country c ON a.countryid = c.countryid " +
										 "LEFT JOIN lu_StateProvince s ON a.StateProvinceID = s.StateProvinceID " +
										 "WHERE a.AddressGUID = @AddressGUID";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@AddressGUID", SqlDbType.UniqueIdentifier).Value = m_AddressGUID;
					cmd.Connection.Open();

					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						m_AddressID = dr.GetInt32(dr.GetOrdinal("AddressId"));
						NickName = dr["NickName"].ToString();
						FirstName = dr["FirstName"].ToString();
						LastName = dr["LastName"].ToString();
						Company = dr["Company"].ToString();
						Address1 = dr["AddressLine1"].ToString();
						Address2 = dr["AddressLine2"].ToString();
						Suite = dr["Suite"].ToString();
						City = dr["City"].ToString();
						StateProvinceID = int.Parse(dr["StateProvinceID"].ToString());
						m_StateProvinceName = dr["StateName"].ToString();
						ZipPostal = dr["ZipPostal"].ToString();
						CountryID = int.Parse(dr["CountryID"].ToString());
						m_CountryName = dr["CountryName"].ToString();
						IsResidential = int.Parse(dr["IsResidential"].ToString()) == 1;
						Phone = dr["Phone"].ToString();
						IsActive = bool.Parse(dr["IsActive"].ToString());
						m_CreateDate = DateTime.Parse(dr["CreateDate"].ToString());
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void New() {
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql =
								"INSERT INTO Address(UserId,NickName,FirstName,LastName,Company,AddressLine1,AddressLine2,Suite,City,StateProvinceID,ZipPostal,CountryID,IsResidential,Phone,IsActive) " +
								"OUTPUT INSERTED.AddressID " +
								"VALUES(@UserId,@NickName,@FirstName,@LastName,@Company,@Address1,@Address2,@Suite,@City,@StateProvinceID,@ZipPostal,@CountryID,@IsResidential,@Phone,@IsActive);";

					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = UserId;
						cmd.Parameters.Add("NickName", SqlDbType.NVarChar, 100).Value = NickName;
						cmd.Parameters.Add("FirstName", SqlDbType.NVarChar, 100).Value = FirstName;
						cmd.Parameters.Add("LastName", SqlDbType.NVarChar, 100).Value = LastName;
						cmd.Parameters.Add("Company", SqlDbType.NVarChar, 100).Value = Company;
						cmd.Parameters.Add("Address1", SqlDbType.NVarChar, 100).Value = Address1;
						cmd.Parameters.Add("Address2", SqlDbType.NVarChar, 100).Value = Address2;
						cmd.Parameters.Add("Suite", SqlDbType.NVarChar, 50).Value = Suite;
						cmd.Parameters.Add("City", SqlDbType.NVarChar, 100).Value = City;
						cmd.Parameters.Add("StateProvinceID", SqlDbType.Int).Value = StateProvinceID;
						cmd.Parameters.Add("ZipPostal", SqlDbType.NVarChar, 10).Value = ZipPostal.ToUpper();
						cmd.Parameters.Add("CountryID", SqlDbType.Int).Value = CountryID;
						cmd.Parameters.Add("IsResidential", SqlDbType.Int).Value = IsResidential ? 1 : 0;
						cmd.Parameters.Add("Phone", SqlDbType.NVarChar, 25).Value = Phone;
						cmd.Parameters.Add("IsActive", SqlDbType.Bit).Value = IsActive;

						cmd.Connection.Open();
						m_AddressID = (int)cmd.ExecuteScalar();
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
				throw;
			}
		}

		public void Update(int addressId) {
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql =
								"UPDATE Address SET UserId=@UserId,NickName=@NickName,FirstName=@FirstName,LastName=@LastName,Company=@Company,AddressLine1=@Address1,AddressLine2=@Address2,Suite=@Suite,City=@City," +
								"StateProvinceID=@StateProvinceID,ZipPostal=@ZipPostal,CountryID=@CountryID,IsResidential=@IsResidential,Phone=@Phone,IsActive=@IsActive " +
								"WHERE AddressID=@AddressID";

					m_AddressID = addressId;
					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("AddressID", SqlDbType.Int).Value = AddressID;
						cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = UserId;
						cmd.Parameters.Add("NickName", SqlDbType.NVarChar, 100).Value = NickName;
						cmd.Parameters.Add("FirstName", SqlDbType.NVarChar, 100).Value = FirstName;
						cmd.Parameters.Add("LastName", SqlDbType.NVarChar, 100).Value = LastName;
						cmd.Parameters.Add("Company", SqlDbType.NVarChar, 100).Value = Company;
						cmd.Parameters.Add("Address1", SqlDbType.NVarChar, 100).Value = Address1;
						cmd.Parameters.Add("Address2", SqlDbType.NVarChar, 100).Value = Address2;
						cmd.Parameters.Add("Suite", SqlDbType.NVarChar, 50).Value = Suite;
						cmd.Parameters.Add("City", SqlDbType.NVarChar, 100).Value = City;
						cmd.Parameters.Add("StateProvinceID", SqlDbType.Int).Value = StateProvinceID;
						cmd.Parameters.Add("ZipPostal", SqlDbType.NVarChar, 10).Value = ZipPostal.ToUpper();
						cmd.Parameters.Add("CountryID", SqlDbType.Int).Value = CountryID;
						cmd.Parameters.Add("IsResidential", SqlDbType.Int).Value = IsResidential ? 1 : 0;
						cmd.Parameters.Add("Phone", SqlDbType.NVarChar, 25).Value = Phone;
						cmd.Parameters.Add("IsActive", SqlDbType.Bit).Value = IsActive;

						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
				throw;
			}
		}

		public void Delete() {
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("DELETE Address WHERE AddressID = @AddressID", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("AddressID", SqlDbType.Int).Value = AddressID;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
				throw;
			}
		}
		#endregion

		#region Public Properties
		public int AddressID { get { return m_AddressID; } }
		public Guid AddressGUID { get { return m_AddressGUID; } }
		public Guid UserId { get; set; }
		public string NickName {
			get {
				return !string.IsNullOrEmpty(m_NickName) ? m_NickName : FormattedAddressShort;
			}
			set { m_NickName = value; }
		}
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Company { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string Suite { get; set; }
		public string City { get; set; }
		public int StateProvinceID { get; set; }
		public string StateProvinceName { get { return m_StateProvinceName; } }
		public string ZipPostal { get; set; }
		public int CountryID { get; set; }
		public string CountryName { get { return m_CountryName; } }
		public bool IsResidential { get; set; }
		public string Phone { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreateDate { get { return m_CreateDate; } }
		public string FormattedAddressShort {
			get {
				string format = "{0}, {1}, {2} {3}";
				return string.Format(format, Address1, City, StateProvinceName, ZipPostal, CountryName);
			}
		}
		#endregion
	}

	public class Addresses {
		#region Fields

		private Guid m_UserId = Guid.Empty;

		#endregion

		#region Constructor

		public Addresses() { }

		public Addresses(Guid userId) {
			m_UserId = userId;
		}

		#endregion

		#region Public Methods

		public void Add(Address address) {
			address.UserId = m_UserId;
			address.New();
		}

		public int Count() {
			int totalCount = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT COUNT(*) FROM Address WHERE UserId = @UserId";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = m_UserId;
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					totalCount = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}

			return totalCount;
		}

		public List<Address> GetList() {
			try {
				List<Address> addressList = new List<Address>();
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql = "SELECT AddressID FROM [Address] WHERE UserId = @UserId";

					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = m_UserId;
						cmd.Connection.Open();

						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							Address address = new Address(dr.GetInt32(0));
							addressList.Add(address);
						}
						cmd.Connection.Close();
					}
				}

				return addressList;
			} catch (Exception) {
				return null;
			}
		}
		public List<SelectListItem> GetSelectList() {
			List<SelectListItem> addressList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql = "SELECT AddressID FROM [Address] WHERE UserId = @UserId";

					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = m_UserId;
						cmd.Connection.Open();

						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							Address address = new Address(dr.GetInt32(0));
							addressList.Add(new SelectListItem {
								Value = address.AddressGUID.ToString(),
								Text = address.NickName
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return addressList;
		}

		#endregion
	}

	public class GeoState {
		#region Public Properties
		public int StateProvinceID { get; set; }
		public string Name { get; set; }
		public string Abbreviation { get; set; }
		#endregion
	}
	public class GeoStates {
		#region Public Methods
		public List<GeoState> GetList(int CountryID) {
			List<GeoState> stateList = new List<GeoState>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql = "SELECT StateProvinceID, Name, Abbreviation FROM lu_StateProvince WHERE CountryID = @CountryID ORDER BY SortOrder, Abbreviation, Name";
					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;
						cmd.Connection.Open();

						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							stateList.Add(new GeoState { StateProvinceID = dr.GetInt32(0), Name = dr[1].ToString(), Abbreviation = dr[2].ToString() });
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return stateList;
		}
		public List<SelectListItem> GetSelectList(int CountryID, int SelectedStateID) {
			List<SelectListItem> items = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql = "SELECT StateProvinceID, Name, Abbreviation FROM lu_StateProvince WHERE CountryID = @CountryID ORDER BY SortOrder, Abbreviation, Name";
					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;
						cmd.Connection.Open();

						SqlDataReader dr = cmd.ExecuteReader();
						if (dr.HasRows) {
							items.Add(new SelectListItem { Value = "", Text = "Please Select" });
							while (dr.Read()) {
								items.Add(new SelectListItem {
									Value = dr[0].ToString(),
									Text = dr[2].ToString() + " - " + dr[1].ToString(),
									Selected = (dr.GetInt32(0) == SelectedStateID)
								});
							}
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return items;
		}
		#endregion
	}
}