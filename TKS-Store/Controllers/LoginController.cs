using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TKS.Models;

namespace TKS.Controllers {
    public class LoginController : Controller
    {
		[ChildActionOnly]
		public ActionResult CreateUser() {
			CreateUserViewModel createUser = new CreateUserViewModel();
			if (TempData["CreateStatusMessage"] != null) {
				createUser.ErrorMessage = TempData["CreateStatusMessage"].ToString();
			}
			if (TempData["CreateUserName"] != null) {
				createUser.NewUserName = TempData["CreateUserName"].ToString();
			}
			if (TempData["CreateEmail"] != null) {
				createUser.Email = TempData["CreateEmail"].ToString();
			}
			if (TempData["ReturnURL"] != null) {
				createUser.ReturnURL = TempData["ReturnURL"].ToString();
			}
			return PartialView("_CreateAccount", createUser);
		}

		[HttpPost]
		public ActionResult CreateAccount(CreateUserViewModel data) {
			//Users users = new Users();
			//if (users.Add(data)) {
			//	Membership.ValidateUser(data.NewUserName, data.NewPassword);
			//	FormsAuthentication.SetAuthCookie(data.NewUserName, false);
			//	if (string.IsNullOrEmpty(data.ReturnURL))
			//		return Redirect(FormsAuthentication.DefaultUrl);
			//	else
			//		return Redirect(data.ReturnURL);
			//} else {
			//	TempData["CreateStatusMessage"] = data.ErrorMessage;
			//	TempData["CreateUserName"] = data.NewUserName;
			//	TempData["CreateEmail"] = data.Email;
			//	TempData["ReturnURL"] = data.ReturnURL;
			//	return View("Index");
			//}

			MembershipCreateStatus status = new MembershipCreateStatus();
			Membership.CreateUser(data.NewUserName, data.NewPassword, data.Email, "", "", true, out status);
			if (status == MembershipCreateStatus.Success) {
				Membership.ValidateUser(data.NewUserName, data.NewPassword);
				FormsAuthentication.SetAuthCookie(data.NewUserName, false);
				if (string.IsNullOrEmpty(data.ReturnURL))
					return Redirect(FormsAuthentication.DefaultUrl);
				else
					return Redirect(data.ReturnURL);
			} else {
				ViewBag.StatusMessage = "Error creating user account!";
			}

			return View("Index");
		}

		[HttpGet]
		public ActionResult Index() {
			string section = "login";
			//Language language;

			string qString = "/" + section;

			//language = new Language("en-US");
			//ContentPage contentPage = new ContentPage(qString);
			//if (contentPage.PageID != Guid.Empty) {
			//	ViewData = contentPage.GetSections();

			//	MenuItem sideMenu = new MenuItem(Request.Url.AbsolutePath);
			//	ViewBag.SideMenu = sideMenu.SiblingMenuCode();

			//	ViewBag.Highlight = "$('." + section + "').addClass('active');";
			//	ViewBag.Title = contentPage.MetaTitle;
			//}

			if (Request.QueryString["returnurl"] != null) {
				TempData["ReturnURL"] = Request.QueryString["returnurl"].ToString();
			}
			return View();
		}
		[HttpPost]
		public ActionResult Index(LoginViewModel data) {
			//Guid AnonID = new TKS.Models.User().UserKey;

			if (Membership.ValidateUser(data.UserName, data.Password)) {
				FormsAuthentication.SetAuthCookie(data.UserName, data.RememberMe);
				//TKS.Models.User user = new TKS.Models.User();
				//user.MigrateAnonymous(AnonID);
				return Redirect(FormsAuthentication.GetRedirectUrl(data.UserName, data.RememberMe));
			} else {
				if (Request.QueryString["returnurl"] != null) {
					TempData["ReturnURL"] = Request.QueryString["returnurl"].ToString();
				}
				data.ErrorMessage = "Your login attempt was not successful. Please try again.";
				return View(data);
			}
		}

		[HttpPost]
		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return RedirectToAction("Index");
			//FormsAuthentication.RedirectToLoginPage();
		}

		[ChildActionOnly]
		public ActionResult Retrieve() {
			RetrievePasswordViewModel retrievePassword = new RetrievePasswordViewModel();
			if (TempData["RetrieveUserName"] != null) {
				retrievePassword.RetrieveUserName = TempData["RetrieveUserName"].ToString();
			}
			if (TempData["RecoverSuccess"] != null) {
				retrievePassword.SuccessMessage = TempData["RecoverSuccess"].ToString();
			}
			if (TempData["RecoverFail"] != null) {
				retrievePassword.ErrorMessage = TempData["RecoverFail"].ToString();
			}
			return PartialView("_RetrievePassword", retrievePassword);
		}

		[HttpPost]
		public ActionResult RetrievePassword(RetrievePasswordViewModel data) {
			//Users users = new Users();
			//if (users.RecoverPassword(data)) {
			//	TempData["RecoverSuccess"] = "<p>Your username and password have been sent to you by email.</p><p>When you have received your password, <a href='/login'>return to this page to login</a>.</p>";
			//	TempData["RecoverFail"] = "";
			//} else {
			//	TempData["RetrieveUserName"] = data.RetrieveUserName;
			//	TempData["RecoverSuccess"] = "";
			//	TempData["RecoverFail"] = "<p>We were unable to access your information. An alert containing the information you entered has been sent to the help desk.</p><p>If you need immediate assistance, email us at <a href='mailto:" + Global.SiteEmail + "'>" + Global.SiteEmail + "</a></p>";
			//}
			return RedirectToAction("Index");
		}

		[ChildActionOnly]
		public ActionResult ShowLogin() {
			TKS.Models.User createUser = new TKS.Models.User();
			return PartialView("_LoginPartial", createUser);
		}
		[ChildActionOnly]
		public ActionResult ShowLogin2() {
			TKS.Models.User createUser = new TKS.Models.User();
			return PartialView("_LoginPartial2", createUser);
		}
	}
}