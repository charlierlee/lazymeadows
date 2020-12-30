using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TKS.Areas.Admin.Models;

namespace TKS.Areas.Admin.Controllers.store {
	public class UsersController : Controller {
		#region Reports
		// GET: Admin/user/ByRole/<roleid>
		[Authorize(Roles = "Admin")]
		public ActionResult ByRole(string id = "") {
			List<UserViewModel> users = new List<UserViewModel>();
			Guid roleID = Guid.Empty;
			Guid.TryParse(id, out roleID);

			if (roleID == Guid.Empty) {
				users = Users.AllUsers();
				ViewBag.CurrentRoleID = "";
			} else {
				Role role = new Role(roleID);
				users = role.GetUsers();
				ViewBag.CurrentRoleID = roleID.ToString();
			}

			ViewBag.selectedrole = new TKS.Areas.Admin.Models.Roles().GetSelectList(roleID);

			return View(users);
		}

		// GET: Admin/user/ByActive/1
		[Authorize(Roles = "Admin")]
		public ActionResult ByActive(int id = 1) {
			List<UserViewModel> users = new List<UserViewModel>();

			users = Users.IsActive(id);
			ViewBag.CurrentActive = id;

			return View(users);
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult Locked() {
			List<UserViewModel> users = Users.LockedUsers();
			return View(users);
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult Online() {
			List<UserViewModel> users = Users.OnlineUsers();
			return View(users);
		}
		#endregion

		#region Index
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult Index() {
			//TODO: Implement search on Account List view
			List<UserViewModel> users = Users.AllUsers();

			return View(users);
		}
		#endregion

		#region Create
		// GET: Membership
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult CreateUser() {
			ViewBag.AccountLevelID = new AccountLevels().GetSelectList(0);
			return View(new UserViewModel());
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult CreateUser(UserViewModel data) {
			Users users = new Users();
			if (users.Add(data)) {
				return RedirectToAction("Index");
			} else {
				ViewBag.StatusMessage = "Error creating user account!";
				return View(data);
			}
		}
		#endregion

		#region General
		// GET: Admin/user/Edit/<username>
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(string id) {
			ViewBag.UserName = id;
			return View();
		}

		// GET: Admin/user/General/<username>
		[Authorize(Roles = "Admin")]
		public ActionResult General(string id) {
			User user = new User(id);
			UserViewModel userViewModel = new UserViewModel(user);
			ViewBag.AccountLevelID = new AccountLevels().GetSelectList(user.AccountLevelID);

			return PartialView(userViewModel);
		}
		// POST: Admin/user/General/<username>
		[HttpPost, ValidateInput(false)]
		[Authorize(Roles = "Admin")]
		public ActionResult General(string id, UserViewModel data) {
			try {
				User user = new User(id);
				user.AccountLevelID = data.AccountLevelID;
				user.Email = data.Email;
				user.FirstName = data.FirstName;
				user.LastName = data.LastName;
				user.Phone = data.Phone;
				user.UpdateRoles(data.Roles);

				return RedirectToAction("index", "users");
			} catch {
				ViewBag.StatusMessage = "There was a problem saving your changes";
				return View(data);
			}
		}

		// POST: Admin/users/Unlock/<username>
		[HttpPost, ValidateInput(false)]
		[Authorize(Roles = "Admin")]
		public ActionResult Unlock(string username) {
			try {
				User user = new User(username);
				user.Unlock();
				return RedirectToAction("edit", "users", new { id = username });
			} catch {
				return View();
			}
		}

		// POST: Admin/users/Delete/<username>
		[HttpPost, ValidateInput(false)]
		[Authorize(Roles = "Admin")]
		public ActionResult Delete(string username) {
			try {
				User user = new User(username);
				user.Delete();
				return RedirectToAction("index", "users");
			} catch {
				return View();
			}
		}
		#endregion

		[HttpGet]
		public ActionResult Login() {
			return View();
		}
		[HttpPost]
		public ActionResult Login(LoginViewModel data) {
			if (System.Web.Security.Membership.ValidateUser(data.UserName, data.Password)) {
				System.Web.Security.FormsAuthentication.SetAuthCookie(data.UserName, data.RememberMe);
				return Redirect(System.Web.Security.FormsAuthentication.GetRedirectUrl(data.UserName, data.RememberMe));
				//return RedirectToAction("Index", "Article");
			} else {
				ViewBag.ErrorMessage = "Your login attempt was not successful. Please try again.";
				return View();
			}
		}

		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("/");
		}

		#region Roles
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult Roles() {
			List<Role> roles = new TKS.Areas.Admin.Models.Roles().AllRoles();
			ViewBag.ErrorMessage = TempData["ErrorMessage"];
			return View(roles);
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult RoleAdd(string role) {
			TKS.Areas.Admin.Models.Roles roles = new TKS.Areas.Admin.Models.Roles();
			if (!roles.Add(role)) {
				TempData["ErrorMessage"] = "New Role not created. Error Message: " + roles.ErrorMsg;
			}
			return RedirectToAction("roles", "users");
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult RoleDelete(string RoleName) {
			Role role = new Role(RoleName);
			if (!role.Delete()) {
				TempData["ErrorMessage"] = "Role not deleted. Error Message: " + role.ErrorMsg;
			}
			return RedirectToAction("roles", "users");
		}
		#endregion

		#region Account Levels
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult CreateAccountLevel() {
			AccountLevelViewModel data = new AccountLevelViewModel();
			data.LevelAllowsCoupons = true;
			data.LevelAllowsQuantityDiscounts = true;
			return View(data);
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult CreateAccountLevel(AccountLevelViewModel data) {
			AccountLevels a = new AccountLevels();
			a.Add(data);
			return RedirectToAction("AccountLevels");
		}

		// POST: Admin/users/DeleteAccountLevel/<AccountLevelID>
		[HttpPost, ValidateInput(false)]
		[Authorize(Roles = "Admin")]
		public ActionResult DeleteAccountLevel(int id) {
			AccountLevel accountLevel = new AccountLevel(id);
			accountLevel.Delete();
			return Json(true);
		}

		// GET: Admin/user/EditAccountLevel/<AccountLevelID>
		[Authorize(Roles = "Admin")]
		public ActionResult EditAccountLevel(int id) {
			return View(new AccountLevelViewModel(new AccountLevel(id)));
		}
		// POST: Admin/user/EditAccountLevel/<AccountLevelID>
		[HttpPost, ValidateInput(false)]
		[Authorize(Roles = "Admin")]
		public ActionResult EditAccountLevel(int id, AccountLevelViewModel data) {
			try {
				AccountLevel accountLevel = new AccountLevel(id);
				accountLevel.LevelAllowsCoupons = data.LevelAllowsCoupons;
				accountLevel.LevelAllowsPO = data.LevelAllowsPO;
				accountLevel.LevelAllowsQuantityDiscounts = data.LevelAllowsQuantityDiscounts;
				accountLevel.LevelDiscountAmount = data.LevelDiscountAmount;
				accountLevel.LevelDiscountPercent = data.LevelDiscountPercent;
				accountLevel.LevelDiscountsApplyToExtendedPrices = data.LevelDiscountsApplyToExtendedPrices;
				accountLevel.LevelHasFreeShipping = data.LevelHasFreeShipping;
				accountLevel.LevelHasNoTax = data.LevelHasNoTax;
				accountLevel.Name = data.Name;

				return RedirectToAction("AccountLevels");
			} catch {
				ViewBag.StatusMessage = "There was a problem saving your changes";
				return View(data);
			}
		}


		// POST: Admin/Size/Reorder
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult AccountLevelReorder(string order) {
			order = "&" + order;
			int pos = 1;
			string[] arOrder = order.Split(new string[] { "&rowid[]=" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string SizeID in arOrder) {
				AccountLevel level = new AccountLevel(Convert.ToInt32(SizeID));
				level.SortOrder = pos; ;
				pos++;
			}

			return Json("success");
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult AccountLevels() {
			List<AccountLevelViewModel> accountLevels = new AccountLevels().GetList();
			return View(accountLevels);
		}
		#endregion

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public FileContentResult DownloadUserEmails() {
			//TODO: check how names with commas are exported
			byte[] fileContent = System.Text.Encoding.ASCII.GetBytes(new Users().GetEmailAddresses());
			return File(fileContent, "application/text", "EmailAddresses.csv");
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public ActionResult DailyLogins(string id) {
			DateTime theDate = DateTime.MinValue;
			if (!DateTime.TryParse(id, out theDate)) {
				theDate = DateTime.Now;
			}
			List<UserViewModel> users = new Users().DailyLogins(theDate);
			ViewBag.TheDate = theDate.ToShortDateString();

			return View(users);
		}

	}
}