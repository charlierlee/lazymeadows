using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TKS.Models {
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
		private Guid _UserKey = Guid.Empty;
		private int _AccountLevelID = 1;
		private MembershipUser _memUser;
		private string _UserName = "Visitor";
		#endregion

		#region Constructor
		public User() {
			if (HttpContext.Current.User.Identity.IsAuthenticated) {
				_IsAuthenticated = true;
				_UserName = HttpContext.Current.User.Identity.Name;
				_memUser = Membership.GetUser(UserName);
				Initialize();
			} else {
				Guid.TryParse(HttpContext.Current.Request.AnonymousID, out _UserKey);
			}
		}

		private void Initialize() {
			_UserKey = (Guid)_memUser.ProviderUserKey;
			Comment = _memUser.Comment;
			_CreationDate = _memUser.CreationDate;
			Email = _memUser.Email;
			_IsApproved = _memUser.IsApproved;
			_IsLockedOut = _memUser.IsLockedOut;
			_IsOnline = _memUser.IsOnline;
			_LastActivityDate = _memUser.LastActivityDate;
			_LastLockoutDate = _memUser.LastLockoutDate;
			_LastPasswordChangedDate = _memUser.LastPasswordChangedDate;
			_UserName = _memUser.UserName;
			_LastLoginDate = _memUser.LastLoginDate;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT FirstName, LastName, Phone, AccountLevelID FROM Account WHERE UserId = @UserId", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = _UserKey;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						FirstName = dr[0].ToString();
						LastName = dr[1].ToString();
						Phone = dr[2].ToString();
						_AccountLevelID = dr.GetInt32(3);
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void Create(string UserName, string Password, string Email) {
			MembershipUser newUser = Membership.CreateUser(UserName, Password, Email);
			_memUser = Membership.GetUser(UserName);
			_UserKey = (Guid)_memUser.ProviderUserKey;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO [Account] ([UserId] ,[Email], [LastIPAddress]) VALUES (@UserId, @Email, @LastIPAddress)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserId", SqlDbType.UniqueIdentifier).Value = _UserKey;
					cmd.Parameters.Add("Email", SqlDbType.NVarChar, 100).Value = Email;
					cmd.Parameters.Add("LastIPAddress", SqlDbType.VarChar, 40).Value = HttpContext.Current.Request.UserHostAddress;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void MigrateAnonymous(Guid AnonymousKey) {
			using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("usp_MigrateOnLogin", cn)) {
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.Parameters.Add("UserID", System.Data.SqlDbType.UniqueIdentifier).Value = UserKey;
					cmd.Parameters.Add("AnonID", System.Data.SqlDbType.UniqueIdentifier).Value = AnonymousKey;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			//ProfileManager.DeleteProfile(e.AnonymousID);
			AnonymousIdentificationModule.ClearAnonymousIdentifier();
		}
		public void Update() {
			try {
				_memUser.Email = Email;
				_memUser.Comment = Comment;
				_memUser.IsApproved = IsApproved;
				Membership.UpdateUser(_memUser);
			} catch (Exception) {
				throw;
			}
		}
		public List<Address> GetAddresses() {
			return new Addresses(_UserKey).GetList();
		}
		public List<SelectListItem> GetAddressSelectList() {
			return new Addresses(_UserKey).GetSelectList();
		}
		#endregion

		#region Public Properties
		public Guid UserKey { get { return _UserKey; } }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Phone { get; set; }
		public string Comment { get; set; }
		public DateTime CreationDate { get { return _CreationDate; } }
		public string Email { get; set; }
		public bool IsAuthenticated { get { return _IsAuthenticated; } }
		public bool IsApproved { get { return _IsApproved; } }
		public bool IsLockedOut { get { return _IsLockedOut; } }
		public bool IsOnline { get { return _IsOnline; } }
		public DateTime LastActivityDate { get { return _LastActivityDate; } }
		public DateTime LastLockoutDate { get { return _LastLockoutDate; } }
		public DateTime LastLoginDate { get { return _LastLoginDate; } }
		public DateTime LastPasswordChangedDate { get { return _LastPasswordChangedDate; } }
		public string UserName { get { return _UserName; } }
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
		private Guid _UserKey = Guid.Empty;
		private bool _IsApproved = true;
		private bool _IsAuthenticated = false;
		private bool _IsLockedOut = false;
		private bool _IsOnline = false;
		private DateTime _LastActivityDate = DateTime.MinValue;
		private DateTime _LastLockoutDate = DateTime.MinValue;
		private DateTime _LastLoginDate = DateTime.MinValue;
		private DateTime _LastPasswordChangedDate = DateTime.MinValue;
		#endregion

		#region Constructor
		public UserViewModel() { }
		public UserViewModel(User user) {
			_CreationDate = user.CreationDate;
			Comment = user.Comment;
			Email = user.Email;
			FirstName = user.FirstName;
			_UserKey = user.UserKey;
			_IsApproved = user.IsApproved;
			_IsAuthenticated = user.IsAuthenticated;
			_IsLockedOut = user.IsLockedOut;
			_IsOnline = user.IsOnline;
			_LastActivityDate = user.LastActivityDate;
			_LastLockoutDate = user.LastLockoutDate;
			_LastLoginDate = user.LastLoginDate;
			LastName = user.LastName;
			_LastPasswordChangedDate = user.LastPasswordChangedDate;
			//Password = user.LastPasswordChangedDate;
			Phone = user.Phone;
			UserName = user.UserName;
		}
		#endregion

		#region Public Properties
		public string Comment { get; set; }
		[DisplayName("Created")]
		[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
		public DateTime CreationDate { get { return _CreationDate; } set { _CreationDate = value; } }
		[Required(ErrorMessage = "Email address is required")]
		[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
							ErrorMessage = "Email is not valid")]
		public string Email { get; set; }
		[DisplayName("First Name")]
		public string FirstName { get; set; }
		[DisplayName("Last Name")]
		public string LastName { get; set; }
		[DisplayName("Active User")]
		public bool IsApproved { get { return _IsApproved; } set { _IsApproved = value; } }
		public bool IsAuthenticated { get { return _IsAuthenticated; } }
		public bool IsLockedOut { get { return _IsLockedOut; } set { _IsLockedOut = value; } }
		public bool IsOnline { get { return _IsOnline; } }
		[DisplayName("Last Activity")]
		[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
		public DateTime LastActivityDate { get { return _LastActivityDate; } set { _LastActivityDate = value; } }
		public DateTime LastLockoutDate { get { return _LastLockoutDate; } }
		[DisplayName("Last Login")]
		[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
		public DateTime LastLoginDate { get { return _LastLoginDate; } set { _LastLoginDate = value; } }
		public DateTime LastPasswordChangedDate { get { return _LastPasswordChangedDate; } }
		[DisplayName("Password")]
		public string Password { get; set; }
		public string Phone { get; set; }
		public Guid UserKey { get { return _UserKey; } set { _UserKey = value; } }
		[DisplayName("User Name")]
		[Required(ErrorMessage = "User Name is required")]
		public string UserName { get; set; }
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
}