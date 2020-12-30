using System;
using System.Collections.Generic;
using System.Web;

namespace TKS.Areas.Admin.Models {
	public class MembershipModel {
	}

	public class CreateUserData {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserID { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string Question { get; set; }
		public string Answer { get; set; }
	}
}