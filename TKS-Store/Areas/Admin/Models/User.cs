using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TKS.Areas.Admin.Models {
	public class User {		
		#region Private Properties
		private bool _IsApproved = false;
		private bool _IsAuthenticated = false;
		private bool _IsLockedOut = false;
		private bool _IsOnline = false;
		private DateTime _CreationDate = DateTime.MinValue;
		private DateTime _LastActivityDate = DateTime.MinValue;
		private DateTime _LastLockoutDate = DateTime.MinValue;
		private DateTime _LastLoginDate = DateTime.MinValue;
		private DateTime _LastPasswordChangedDate = DateTime.MinValue;
		private Guid _Guid = Guid.Empty;
		private int _AccountLevelID = 1;
		private List<string> _RolesAssigned = new List<string>();
		private MembershipUser _memUser;
		private string _Comment = "";
		private string _Email = "";
		private string _ErrorMsg = "";
		private string _FirstName = "";
		private string _LastName = "";
		private string _Phone = "";
		private string _UserName = "";
		#endregion

		#region Constructor
		public User() {
			if (HttpContext.Current.User.Identity.IsAuthenticated) {
				_IsAuthenticated = true;
				_UserName = HttpContext.Current.User.Identity.Name;
				_memUser = Membership.GetUser(UserName);
				_Guid = (Guid)_memUser.ProviderUserKey;
				Initialize();
			} else {
				_Guid = Guid.Empty;
				//Guid.TryParse(HttpContext.Current.Request.AnonymousID, out _Guid);
			}
		}
		public User(Guid userId) {
			_Guid = userId;
			_memUser = Membership.GetUser(userId);
			_UserName = _memUser.UserName;
			Initialize();
		}
		public User(string username) {
			_memUser = Membership.GetUser(username);
			Initialize();
		}
		public User(UserViewModel userViewModel) {
			_memUser = Membership.GetUser(userViewModel.UserName);
			Initialize();
		}
		private void Initialize() {
			if (_memUser != null) {
				_Comment = _memUser.Comment;
				_CreationDate = _memUser.CreationDate;
				_Email = _memUser.Email;
				_IsApproved = _memUser.IsApproved;
				_IsLockedOut = _memUser.IsLockedOut;
				_IsOnline = _memUser.IsOnline;
				_LastActivityDate = _memUser.LastActivityDate;
				_LastLockoutDate = _memUser.LastLockoutDate;
				_LastPasswordChangedDate = _memUser.LastPasswordChangedDate;
				//PasswordQuestion = _memUser.PasswordQuestion;
				//ProviderName = _memUser.ProviderName;
				_Guid = (Guid)_memUser.ProviderUserKey;
				_UserName = _memUser.UserName;
				_LastLoginDate = _memUser.LastLoginDate;
			}

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT FirstName, LastName, Phone, AccountLevelID FROM Account WHERE UserId = @UserId", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = this.UserKey;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_FirstName = dr[0].ToString();
						_LastName = dr[1].ToString();
						_Phone = dr[2].ToString();
						_AccountLevelID = dr.GetInt32(3);
					}
					cmd.Connection.Close();
				}
			}
			string[] arrRoles = System.Web.Security.Roles.GetRolesForUser(_UserName);
			foreach (string role in arrRoles) {
				_RolesAssigned.Add(role);
			}
		}
		#endregion

		#region Private Methods
		private void RoleAdd(string Role) {
			System.Web.Security.Roles.AddUserToRole(UserName, Role);
		}
		private void RolesClear() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM [aspnet_UsersInRoles] WHERE [UserId] = @UserId", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = this.UserKey;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		private void UpdateIntProperty(string FieldName, int? Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE Account SET " + FieldName + " = @Value WHERE UserId = @UserId", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = this.UserKey;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE Account SET " + FieldName + " = @Value WHERE UserId = @UserId", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = this.UserKey;
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
				using (SqlCommand cmd = new SqlCommand("DELETE FROM [Account] WHERE [UserId] = @UserId", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = this.UserKey;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			Membership.DeleteUser(UserName, true);
		}
		public void Unlock() {
			_memUser.UnlockUser();
			_IsLockedOut = false;
		}
		public void UpdateRoles(string[] roles) {
			RolesClear();
			foreach (string role in roles) {
				RoleAdd(role);
			}
		}

		//public Addresses GetAddresses() {
		//	return new Addresses(m_Guid);
		//}
		#endregion

		#region Public Properties
		public bool IsApproved { 
			get { return _IsApproved; }
			set {
				if (_IsApproved != value) {
					_IsApproved = value;
					_memUser.IsApproved = value;
					Membership.UpdateUser(_memUser);
				}
			}
		}
		public bool IsAuthenticated { get { return _IsAuthenticated; } }
		public bool IsLockedOut { get { return _IsLockedOut; } }
		public bool IsOnline { get { return _IsOnline; } }
		public DateTime CreationDate { get { return _CreationDate; } }
		public DateTime LastActivityDate { get { return _LastActivityDate; } }
		public DateTime LastLockoutDate { get { return _LastLockoutDate; } }
		public DateTime LastLoginDate { get { return _LastLoginDate; } }
		public DateTime LastPasswordChangedDate { get { return _LastPasswordChangedDate; } }
		public Guid UserKey { get { return _Guid; } }
		public int AccountLevelID {
			get { return _AccountLevelID; }
			set {
				if (_AccountLevelID != value) {
					_AccountLevelID = value;
					UpdateIntProperty("AccountLevelID", value);
				}
			}
		}
		public string Comment {
			get { return _Comment; }
			set {
				if (_Comment != value) {
					_memUser.Comment = value;
					Membership.UpdateUser(_memUser);
				}
			}
		}
		public string Email {
			get { return _Email; }
			set {
				if (_Email != value) {
					_Email = value;
					_memUser.Email = value;
					Membership.UpdateUser(_memUser);
				}
			}
		}
		public string ErrorMsg { get { return _ErrorMsg; } }
		public string FirstName {
			get { return _FirstName; }
			set {
				if (_FirstName != value) {
					_FirstName = UpdateStringProperty("FirstName", 100, value);
				}
			}
		}
		public string LastName {
			get { return _LastName; }
			set {
				if (_LastName != value) {
					_LastName = UpdateStringProperty("LastName", 100, value);
				}
			}
		}
		public string Phone {
			get { return _Phone; }
			set {
				if (_Phone != value) {
					_Phone = UpdateStringProperty("Phone", 25, value);
				}
			}
		}
		public string UserName { get { return _UserName; } }
		public List<string> RolesAssigned { get { return _RolesAssigned; } }
		#endregion
	}
	public class CreateUserViewModel {
		#region Constructor
		public CreateUserViewModel() { }
		#endregion

		#region Public Properties
		public string Comment { get; set; }
		[Required(ErrorMessage = "Email address is required")]
		[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
							ErrorMessage = "Email is not valid")]
		public string Email { get; set; }
		public string ErrorMessage { get; set; }
		public bool IsApproved { get; set; }
		[DisplayName("First Name")]
		public string FirstName { get; set; }
		[DisplayName("Last Name")]
		public string LastName { get; set; }
		[DisplayName("Password")]
		[Required(ErrorMessage = "Password is required")]
		public string NewPassword { get; set; }
		public string Phone { get; set; }
		public string ReturnURL { get; set; }
		public string[] Roles { get; set; }
		[DisplayName("User Name")]
		[Required(ErrorMessage = "User Name is required")]
		public string NewUserName { get; set; }
		#endregion
	}
	public class UserViewModel {
		#region Private Properties
		private DateTime _CreationDate = DateTime.MinValue;
		private Guid _Guid = Guid.Empty;
		private DateTime _LastActivityDate = DateTime.MinValue;
		private DateTime _LastLockoutDate = DateTime.MinValue;
		private DateTime _LastLoginDate = DateTime.MinValue;
		private DateTime _LastPasswordChangedDate = DateTime.MinValue;
		#endregion

		#region Constructor
		public UserViewModel() { }
		public UserViewModel(User user) {
			this.AccountLevelID = user.AccountLevelID;
			this.CreationDate = user.CreationDate;
			this.Comment = user.Comment;
			this.Email = user.Email;
			this.FirstName = user.FirstName;
			this.UserKey = user.UserKey;
			this.IsApproved = user.IsApproved;
			this.IsAuthenticated = user.IsAuthenticated;
			this.IsLockedOut = user.IsLockedOut;
			this.IsOnline = user.IsOnline;
			this.LastActivityDate = user.LastActivityDate;
			this.LastLockoutDate = user.LastLockoutDate;
			this.LastLoginDate = user.LastLoginDate;
			this.LastName = user.LastName;
			this.LastPasswordChangedDate = user.LastPasswordChangedDate;
			//Password = user.LastPasswordChangedDate;
			this.Phone = user.Phone;
			this.UserName = user.UserName;
			this.RolesAssigned = user.RolesAssigned;
		}
		#endregion

		#region Public Methods
		public List<Role> AllRoles() {
			return new Roles().AllRoles();
		}
		#endregion

		#region Public Properties
		public bool IsApproved { get; set; }
		public bool IsAuthenticated { get; set; }
		public bool IsLockedOut { get; set; }
		public bool IsOnline { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime LastActivityDate { get; set; }
		public DateTime LastLockoutDate { get; set; }
		public DateTime LastLoginDate { get; set; }
		public DateTime LastPasswordChangedDate { get; set; }
		public Guid UserKey { get; set; }
		public int AccountLevelID { get; set; }
		public List<string> RolesAssigned { get; set; }
		public string Comment { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public string UserName { get; set; }
		public string[] Roles { get; set; }
		#endregion
	}
	public class LoginViewModel {
		public string ErrorMessage { get; set; }
		[DisplayName("User Name")]
		[Required(ErrorMessage = "User Name is required")]
		public string UserName { get; set; }
		[DisplayName("Password")]
		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }
		[DisplayName("Remember me next time")]
		public bool RememberMe { get; set; }
	}
	public class RetrievePasswordViewModel {
		public string ErrorMessage { get; set; }
		public string SuccessMessage { get; set; }
		[DisplayName("Username or email address")]
		[Required(ErrorMessage = "Username or email address is required")]
		public string RetrieveUserName { get; set; }
	}

	public class Users {
		#region Constructor
		public Users() { }
		#endregion

		#region Public Methods
		public bool Add(UserViewModel data) {
			bool ret = false;

			try {
				// Add User
				MembershipCreateStatus status;
				MembershipUser newUser = Membership.CreateUser(data.UserName, data.Password, data.Email, null, null, data.IsApproved, out status);
				if (status != MembershipCreateStatus.Success) { return false; }

				if (!string.IsNullOrEmpty(data.Comment)) {
					newUser.Comment = data.Comment;
					Membership.UpdateUser(newUser);
				}

				// Add Roles
				foreach (string role in data.Roles) {
					new Roles().AddUserToRole(data.UserName, role);
				}

				if (!string.IsNullOrEmpty(data.FirstName) || !string.IsNullOrEmpty(data.LastName)) {
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "INSERT INTO [Account] ([UserId], [AccountLevelID], [Email], [FirstName], [LastName], [Phone], [LastIPAddress]) VALUES (@UserId, @AccountLevelID, '', @FirstName, @LastName, @Phone, @LastIPAddress)";
						using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(SQL, cn)) {
							cmd.CommandType = System.Data.CommandType.Text;
							cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = (Guid)Membership.GetUser(data.UserName).ProviderUserKey;
							cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = data.AccountLevelID;
							cmd.Parameters.Add("FirstName", SqlDbType.VarChar, 100).Value = data.FirstName ?? "";
							cmd.Parameters.Add("LastName", SqlDbType.VarChar, 100).Value = data.LastName ?? "";
							cmd.Parameters.Add("Phone", SqlDbType.VarChar, 25).Value = data.Phone ?? "";
							cmd.Parameters.Add("LastIPAddress", SqlDbType.VarChar, 40).Value = HttpContext.Current.Request.UserHostAddress;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
				ret = true;
			} catch { }

			return ret;
		}
		public bool Add(CreateUserViewModel data) {
			bool ret = false;

			try {
				// Add User
				MembershipCreateStatus status;
				MembershipUser newUser = Membership.CreateUser(data.NewUserName, data.NewPassword, data.Email, null, null, data.IsApproved, out status);
				if (status != MembershipCreateStatus.Success) {
					data.ErrorMessage = GetErrorMessage(status);
					return false; 
				}

				newUser.Comment = data.Comment;
				Membership.UpdateUser(newUser);

				ret = true;
			} catch { }

			return ret;
		}
		public bool RecoverPassword(RetrievePasswordViewModel data) {
			return RetrievebyEmail(data.RetrieveUserName);
		}
		public static List<UserViewModel> AllUsers() {
			List<UserViewModel> l = new List<UserViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT UserID, Email, IsApproved, IsLockedOut, UserName, CreateDate, LastLoginDate, LastActivityDate FROM vw_aspnet_MembershipUsers ORDER BY UserName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						UserViewModel user = new UserViewModel();
						user.UserKey = dr.GetGuid(0);
						user.Email = dr[1].ToString();
						user.IsApproved = dr.GetBoolean(2);
						user.IsLockedOut = dr.GetBoolean(3);
						user.UserName = dr[4].ToString();
						user.CreationDate = dr.GetDateTime(5);
						user.LastLoginDate = dr.GetDateTime(6);
						user.LastActivityDate = dr.GetDateTime(7);

						l.Add(user);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public List<UserViewModel> DailyLogins(DateTime theDate) {
			List<UserViewModel> l = new List<UserViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT u.UserName, m.Email FROM ProductView pv JOIN aspnet_Users u ON pv.UserID = u.UserId JOIN aspnet_Membership m ON u.UserId = m.UserId WHERE ViewDate >= @TheDate AND ViewDate < DATEADD(d, 1, @TheDate) ORDER BY UserName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("TheDate", SqlDbType.DateTime).Value = theDate;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						UserViewModel user = new UserViewModel();
						user.UserName = dr[0].ToString();
						user.Email = dr[1].ToString();

						l.Add(user);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public static List<UserViewModel> IsActive(int isActive) {
			List<UserViewModel> l = new List<UserViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT UserID, Email, IsApproved, IsLockedOut, UserName, CreateDate, LastLoginDate, LastActivityDate FROM vw_aspnet_MembershipUsers WHERE IsApproved = @IsApproved ORDER BY UserName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("IsApproved", SqlDbType.Bit).Value = isActive < 1 ? 0 : 1;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						UserViewModel user = new UserViewModel();
						user.UserKey = dr.GetGuid(0);
						user.Email = dr[1].ToString();
						user.IsApproved = dr.GetBoolean(2);
						user.IsLockedOut = dr.GetBoolean(3);
						user.UserName = dr[4].ToString();
						user.CreationDate = dr.GetDateTime(5);
						user.LastLoginDate = dr.GetDateTime(6);
						user.LastActivityDate = dr.GetDateTime(7);

						l.Add(user);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public static List<UserViewModel> LockedUsers() {
			List<UserViewModel> l = new List<UserViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT UserID, Email, IsApproved, IsLockedOut, UserName, CreateDate, LastLoginDate, LastActivityDate FROM vw_aspnet_MembershipUsers WHERE IsLockedOut = 1 ORDER BY UserName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						UserViewModel user = new UserViewModel();
						user.UserKey = dr.GetGuid(0);
						user.Email = dr[1].ToString();
						user.IsApproved = dr.GetBoolean(2);
						user.IsLockedOut = dr.GetBoolean(3);
						user.UserName = dr[4].ToString();
						user.CreationDate = dr.GetDateTime(5);
						user.LastLoginDate = dr.GetDateTime(6);
						user.LastActivityDate = dr.GetDateTime(7);

						l.Add(user);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public static List<UserViewModel> OnlineUsers() {
			List<UserViewModel> l = new List<UserViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT UserID, Email, IsApproved, IsLockedOut, UserName, CreateDate, LastLoginDate, LastActivityDate FROM vw_aspnet_MembershipUsers WHERE DATEDIFF(MINUTE, LastActivityDate, GETDATE()) < 30  ORDER BY UserName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						UserViewModel user = new UserViewModel();
						user.UserKey = dr.GetGuid(0);
						user.Email = dr[1].ToString();
						user.IsApproved = dr.GetBoolean(2);
						user.IsLockedOut = dr.GetBoolean(3);
						user.UserName = dr[4].ToString();
						user.CreationDate = dr.GetDateTime(5);
						user.LastLoginDate = dr.GetDateTime(6);
						user.LastActivityDate = dr.GetDateTime(7);

						l.Add(user);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public static List<UserViewModel> UsersInRole(string Rolename = "") {
			List<UserViewModel> l = new List<UserViewModel>();
			if (Rolename == "") {
				l = AllUsers();
			} else {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("aspnet_UsersInRoles_GetUsersInRoles", cn)) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							UserViewModel user = new UserViewModel();
							user.UserKey = dr.GetGuid(0);
							user.Email = dr[1].ToString();
							user.IsApproved = dr.GetBoolean(2);
							user.IsLockedOut = dr.GetBoolean(3);
							user.UserName = dr[4].ToString();
							user.CreationDate = dr.GetDateTime(5);
							user.LastLoginDate = dr.GetDateTime(6);
							user.LastActivityDate = dr.GetDateTime(7);

							l.Add(user);
						}
						cmd.Connection.Close();
					}
				}
			}
			return l;
		}
		public string GetEmailAddresses() {
			StringBuilder sb = new StringBuilder();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string sql = "SELECT DISTINCT FirstName, LastName, Email FROM ( " +
							 "SELECT shipping_FirstName AS FirstName, shipping_LastName AS LastName, shipping_Email AS Email FROM OrderHeader " +
							 "UNION " +
							 "SELECT billing_FullName AS FirstName, billing_LastName AS LastName, billing_Email AS Email FROM OrderHeader) AS Emails	" +
							 "WHERE Email IS NOT NULL " +
							 "ORDER BY Email ASC";

				using (SqlCommand cmd = new SqlCommand(sql, cn)) {
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

		#endregion

		#region Private Methods
		private string GetErrorMessage(MembershipCreateStatus status) {
			switch (status) {
				case MembershipCreateStatus.DuplicateUserName:
					return "Username already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A username for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
		private bool RetrievebyEmail(string userName) {
			MembershipUser mu = Membership.GetUser(userName);

			if (mu != null) {
				SendResetEmail(mu);
				return true;
			} else {
				// Check if a known email address was entered
				string uname = Membership.GetUserNameByEmail(userName);
				if (uname == null) {
					return false;
				} else {
					mu = Membership.GetUser(uname);
					if (mu != null) {
						SendResetEmail(mu);
						return true;
					} else {
						return false;
					}
				}
			}
		}
		private void SendResetEmail(MembershipUser mu) {
			string pword = mu.GetPassword();

			//---------------- Sending Email
			string strBody = "<html><body>===========================================<br />Password Recovery Request<br />===========================================<br />" +
							  "Thank you for requesting your password for " + Global.TitleSuffix + "<br />" +
							  "UserName: " + mu.UserName + "<br />" +
							  "Password: " + pword + "<br /><br />" +
							  "After logging in you can <a href='" + Global.BaseURL + "/changepassword.aspx'>Change your password</a></body></html>";

			MailMessage mm = new MailMessage();
			mm.Subject = "Password Reminder";
			mm.To.Add(mu.Email);
			mm.Bcc.Add("tom@eerieglow.com");
			mm.From = new MailAddress(Global.SiteEmail, Global.TitleSuffix);
			mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

			AlternateView plainView = AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(strBody, @"<(.|\n)*?>", string.Empty), System.Text.Encoding.GetEncoding("utf-8"), "text/plain");
			AlternateView htmlView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.GetEncoding("utf-8"), "text/html");
			mm.AlternateViews.Add(plainView);
			mm.AlternateViews.Add(htmlView);

			SmtpClient smtp = new SmtpClient();
			smtp.Send(mm);
		}
		#endregion
	}

	public class Role {
		#region Private Properties
		private string _ErrorMsg = "";
		private Guid _RoleID = Guid.Empty;
		private string _RoleName = "";
		private int _UserCount = 0;
		#endregion

		#region Constructor
		public Role() { }
		public Role(string roleName) {
			_RoleName = roleName;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT RoleID FROM aspnet_Roles WHERE RoleName = @RoleName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("RoleName", SqlDbType.VarChar, 256).Value = _RoleName;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_RoleID = dr.GetGuid(0);
					}
					cmd.Connection.Close();
				}
			}
		}
		public Role(Guid roleid) {
			_RoleID = roleid;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT RoleName FROM aspnet_Roles WHERE RoleID = @RoleID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("RoleID", SqlDbType.UniqueIdentifier).Value = _RoleID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_RoleName = dr[0].ToString();
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public bool Delete() {
			try {
				System.Web.Security.Roles.DeleteRole(RoleName);
				return true;
			} catch (Exception ex) {
				_ErrorMsg = ex.Message;
				return false;
			}

		}
		public List<UserViewModel> GetUsers() {
			List<UserViewModel> l = new List<UserViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT u.UserName, u.UserId, u.LastActivityDate, m.Email, m.CreateDate, m.LastLoginDate FROM aspnet_Roles r JOIN aspnet_UsersInRoles uir ON r.RoleId = uir.RoleId JOIN aspnet_Users u ON uir.UserId = u.UserId JOIN aspnet_Membership m ON u.UserId = m.UserId WHERE r.RoleName = @RoleName ORDER BY u.UserName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("RoleName", SqlDbType.VarChar, 256).Value = _RoleName;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						UserViewModel user = new UserViewModel();
						user.UserName = dr[0].ToString();
						user.UserKey = dr.GetGuid(1);
						user.LastActivityDate = dr.GetDateTime(2);
						user.Email = dr[3].ToString();
						user.CreationDate = dr.GetDateTime(4);
						user.LastLoginDate = dr.GetDateTime(5);
						//user.IsApproved = dr.GetBoolean(2);
						//user.IsLockedOut = dr.GetBoolean(3);

						l.Add(user);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion

		#region Public Properties
		public string ErrorMsg { get { return _ErrorMsg; } }
		public Guid RoleID { get { return _RoleID; } set { _RoleID = value; } }
		public string RoleName { get { return _RoleName; } set { _RoleName = value; } }
		public int UserCount { get { return _UserCount; } set { _UserCount = value; } }
		#endregion
	}

	public class Roles {
		#region Private Properties
		private string _ErrorMsg = "";
		#endregion

		#region Public Methods
		public List<Role> AllRoles() {
			List<Role> l = new List<Role>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT RoleName, RoleID FROM aspnet_Roles ORDER BY RoleName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						Role role = new Role();
						role.RoleName = dr[0].ToString();
						role.RoleID = dr.GetGuid(1);
						role.UserCount = System.Web.Security.Roles.GetUsersInRole(role.RoleName).Length;
						l.Add(role);
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}

		public List<SelectListItem> GetSelectList(Guid SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT RoleName, RoleID FROM aspnet_Roles ORDER BY RoleName", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							itemList.Add(new SelectListItem {
								Value = dr[1].ToString(),
								Text = dr[0].ToString(),
								Selected = (dr.GetGuid(1) == SelectedID)
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return itemList;
		}
		public bool Add(string RoleName) {
			try {
				System.Web.Security.Roles.CreateRole(RoleName);
				return true;
			} catch (Exception ex) {
				_ErrorMsg = ex.Message;
				return false;
			}
		}
		public void AddUserToRole(string UserName, string Role) {
			System.Web.Security.Roles.AddUserToRole(UserName, Role);
		}
		#endregion

		#region Public Properties
		public string ErrorMsg { get { return _ErrorMsg; } }
		#endregion
	}

	public class AccountLevel {
		#region Private Properties
		private int _AccountLevelID = 0;
		private Guid _AccountLevelGUID = Guid.Empty;
		private string _Name = "";
		private double _LevelDiscountPercent = 0;
		private decimal _LevelDiscountAmount = 0;
		private bool _LevelHasFreeShipping = false;
		private bool _LevelAllowsQuantityDiscounts = true;
		private bool _LevelHasNoTax = false;
		private bool _LevelAllowsCoupons = true;
		private bool _LevelDiscountsApplyToExtendedPrices = false;
		private bool _LevelAllowsPO = false;
		private int _SortOrder = 1;
		private bool _IsActive = true;
		private DateTime _CreateDate = DateTime.MinValue;
		#endregion

		#region Constructor
		public AccountLevel() { }
		public AccountLevel(int accountLevelID) {
			_AccountLevelID = accountLevelID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT [Name], [LevelDiscountPercent], [LevelDiscountAmount], [LevelHasFreeShipping], [LevelAllowsQuantityDiscounts], [LevelHasNoTax], [LevelAllowsCoupons], [LevelDiscountsApplyToExtendedPrices],[LevelAllowsPO], [SortOrder], [IsActive], [CreateDate], [AccountLevelGUID] FROM AccountLevel WHERE AccountLevelID = @AccountLevelID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = _AccountLevelID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_Name = dr[0].ToString();
						_LevelDiscountPercent = dr.GetDouble(1);
						_LevelDiscountAmount = dr.GetDecimal(2);
						_LevelHasFreeShipping = dr.GetBoolean(3);
						_LevelAllowsQuantityDiscounts = dr.GetBoolean(4);
						_LevelHasNoTax = dr.GetBoolean(5);
						_LevelAllowsCoupons = dr.GetBoolean(6);
						_LevelDiscountsApplyToExtendedPrices = dr.GetBoolean(7);
						_LevelAllowsPO = dr.GetBoolean(8);
						_SortOrder = dr.GetInt32(9);
						_IsActive = dr.GetBoolean(10);
						_CreateDate = dr.GetDateTime(11);
						_AccountLevelGUID = dr.GetGuid(12);
					} else {
						_AccountLevelID = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateBoolProperty(string FieldName, bool Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE AccountLevel SET " + FieldName + " = @Value WHERE AccountLevelID = @AccountLevelID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = AccountLevelID;
					cmd.Parameters.Add("Value", SqlDbType.Bit).Value = Value ? 1 : 0;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

		}
		private void UpdateDecimalProperty(string FieldName, decimal Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE AccountLevel SET " + FieldName + " = @Value WHERE AccountLevelID = @AccountLevelID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = AccountLevelID;
					cmd.Parameters.Add("Value", SqlDbType.Decimal).Value = Value;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		private void UpdateDoubleProperty(string FieldName, double? Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE AccountLevel SET " + FieldName + " = @Value WHERE AccountLevelID = @AccountLevelID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = AccountLevelID;
					if (Value == null) {
						cmd.Parameters.Add("Value", SqlDbType.Decimal).Value = SqlDecimal.Null;
					} else {
						cmd.Parameters.Add("Value", SqlDbType.Decimal).Value = Value;
					}
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		private void UpdateIntProperty(string FieldName, int? Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE AccountLevel SET " + FieldName + " = @Value WHERE AccountLevelID = @AccountLevelID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = AccountLevelID;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE AccountLevel SET " + FieldName + " = @Value WHERE AccountLevelID = @AccountLevelID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = AccountLevelID;
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
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("UPDATE AccountLevel SET IsActive = 0 WHERE AccountLevelID = @AccountLevelID", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("AccountLevelID", SqlDbType.Int).Value = _AccountLevelID;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();

						//Reset account level to "Standard" if the account level is deleted
						cmd.CommandText = "UPDATE [Account] SET AccountLevelID = 2 WHERE AccountLevelID = @AccountLevelID";
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
		public int AccountLevelID { get { return _AccountLevelID; } }
		public Guid AccountLevelGUID { get { return _AccountLevelGUID; } }
		public string Name {
			get { return _Name; }
			set {
				if (_Name != value) {
					_Name = UpdateStringProperty("Name", 400, value);
				}
			}
		}
		public double LevelDiscountPercent {
			get { return _LevelDiscountPercent; }
			set {
				if (_LevelDiscountPercent != value) {
					_LevelDiscountPercent = value;
					UpdateDoubleProperty("LevelDiscountPercent", value);
				}
			}
		}
		public decimal LevelDiscountAmount {
			get { return _LevelDiscountAmount; }
			set {
				if (_LevelDiscountAmount != value) {
					_LevelDiscountAmount = value;
					UpdateDecimalProperty("LevelDiscountAmount", value);
				}
			}
		}
		public bool LevelHasFreeShipping {
			get { return _LevelHasFreeShipping; }
			set {
				if (_LevelHasFreeShipping != value) {
					_LevelHasFreeShipping = value;
					UpdateBoolProperty("LevelHasFreeShipping", value);
				}
			}
		}
		public bool LevelAllowsQuantityDiscounts {
			get { return _LevelAllowsQuantityDiscounts; }
			set {
				if (_LevelAllowsQuantityDiscounts != value) {
					_LevelAllowsQuantityDiscounts = value;
					UpdateBoolProperty("LevelAllowsQuantityDiscounts", value);
				}
			}
		}
		public bool LevelHasNoTax {
			get { return _LevelHasNoTax; }
			set {
				if (_LevelHasNoTax != value) {
					_LevelHasNoTax = value;
					UpdateBoolProperty("LevelHasNoTax", value);
				}
			}
		}
		public bool LevelAllowsCoupons {
			get { return _LevelAllowsCoupons; }
			set {
				if (_LevelAllowsCoupons != value) {
					_LevelAllowsCoupons = value;
					UpdateBoolProperty("LevelAllowsCoupons", value);
				}
			}
		}
		public bool LevelDiscountsApplyToExtendedPrices {
			get { return _LevelDiscountsApplyToExtendedPrices; }
			set {
				if (_LevelDiscountsApplyToExtendedPrices != value) {
					_LevelDiscountsApplyToExtendedPrices = value;
					UpdateBoolProperty("LevelDiscountsApplyToExtendedPrices", value);
				}
			}
		}
		public bool LevelAllowsPO {
			get { return _LevelAllowsPO; }
			set {
				if (_LevelAllowsPO != value) {
					_LevelAllowsPO = value;
					UpdateBoolProperty("LevelAllowsPO", value);
				}
			}
		}
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					UpdateIntProperty("SortOrder", value);
				}
			}
		}
		public bool IsActive {
			get { return _IsActive; }
			set {
				if (_IsActive != value) {
					_IsActive = value;
					UpdateBoolProperty("IsActive", value);
				}
			}
		}
		public DateTime CreateDate { get { return _CreateDate; } }
		#endregion
	}
	public class AccountLevels {
		#region Public Methods
		public int Add(AccountLevelViewModel data) {
			int AccountLevelID = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO [AccountLevel] ([Name], [LevelDiscountPercent], [LevelDiscountAmount], [LevelHasFreeShipping], [LevelAllowsQuantityDiscounts], [LevelHasNoTax], [LevelAllowsCoupons], [LevelDiscountsApplyToExtendedPrices], [LevelAllowsPO]) " +
							"	OUTPUT INSERTED.AccountLevelID " +
							"	VALUES(@Name, @LevelDiscountPercent, @LevelDiscountAmount, @LevelHasFreeShipping, @LevelAllowsQuantityDiscounts, @LevelHasNoTax, @LevelAllowsCoupons, @LevelDiscountsApplyToExtendedPrices, @LevelAllowsPO)";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("Name", SqlDbType.NVarChar, 400).Value = data.Name;
					cmd.Parameters.Add("LevelDiscountPercent", SqlDbType.Float).Value = data.LevelDiscountPercent;
					cmd.Parameters.Add("LevelDiscountAmount", SqlDbType.Money).Value = data.LevelDiscountAmount;
					cmd.Parameters.Add("LevelHasFreeShipping", SqlDbType.Bit).Value = data.LevelHasFreeShipping;
					cmd.Parameters.Add("LevelAllowsQuantityDiscounts", SqlDbType.Bit).Value = data.LevelAllowsQuantityDiscounts;
					cmd.Parameters.Add("LevelHasNoTax", SqlDbType.Bit).Value = data.LevelHasNoTax;
					cmd.Parameters.Add("LevelAllowsCoupons", SqlDbType.Bit).Value = data.LevelAllowsCoupons;
					cmd.Parameters.Add("LevelDiscountsApplyToExtendedPrices", SqlDbType.Bit).Value = data.LevelDiscountsApplyToExtendedPrices;
					cmd.Parameters.Add("LevelAllowsPO", SqlDbType.Bit).Value = data.LevelAllowsPO;

					cmd.Connection.Open();
					AccountLevelID = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}

			return AccountLevelID;
		}
		public List<AccountLevelViewModel> GetList() {
			List<AccountLevelViewModel> l = new List<AccountLevelViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT AccountLevelID FROM AccountLevel WHERE IsActive = 1 ORDER BY SortOrder, Name", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new AccountLevelViewModel(new AccountLevel(dr.GetInt32(0))));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public List<SelectListItem> GetSelectList(int SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string sql = "SELECT AccountLevelID, Name FROM AccountLevel WHERE IsActive = 1 ORDER BY SortOrder, Name";

					using (SqlCommand cmd = new SqlCommand(sql, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							itemList.Add(new SelectListItem {
								Value = dr[0].ToString(),
								Text = dr[1].ToString(),
								Selected = (dr.GetInt32(0) == SelectedID)
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return itemList;
		}
		#endregion
	}
	public class AccountLevelViewModel {
		#region Constructor
		public AccountLevelViewModel() { }
		public AccountLevelViewModel(AccountLevel data) {
			this.AccountLevelGUID = data.AccountLevelGUID;
			this.AccountLevelID = data.AccountLevelID;
			this.CreateDate = data.CreateDate;
			this.IsActive = data.IsActive;
			this.LevelAllowsCoupons = data.LevelAllowsCoupons;
			this.LevelAllowsPO = data.LevelAllowsPO;
			this.LevelAllowsQuantityDiscounts = data.LevelAllowsQuantityDiscounts;
			this.LevelDiscountAmount = data.LevelDiscountAmount;
			this.LevelDiscountPercent = data.LevelDiscountPercent;
			this.LevelDiscountsApplyToExtendedPrices = data.LevelDiscountsApplyToExtendedPrices;
			this.LevelHasFreeShipping = data.LevelHasFreeShipping;
			this.LevelHasNoTax = data.LevelHasNoTax;
			this.Name = data.Name;
			this.SortOrder = data.SortOrder;
		}
		#endregion

		#region Public Properties
		public int AccountLevelID { get; set; }
		public Guid AccountLevelGUID { get; set; }
		public string Name { get; set; }
		public double LevelDiscountPercent { get; set; }
		public decimal LevelDiscountAmount { get; set; }
		public bool LevelHasFreeShipping { get; set; }
		public bool LevelAllowsQuantityDiscounts { get; set; }
		public bool LevelHasNoTax { get; set; }
		public bool LevelAllowsCoupons { get; set; }
		public bool LevelDiscountsApplyToExtendedPrices { get; set; }
		public bool LevelAllowsPO { get; set; }
		public int SortOrder { get; set; }
		public bool IsActive { get; set; }
		public DateTime CreateDate { get; set; }
		#endregion
	}
}