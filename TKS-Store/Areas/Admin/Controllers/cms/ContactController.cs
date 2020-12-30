using System.Collections.Generic;
using System.Web.Mvc;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class ContactController : Controller
    {
        // GET: Admin/Contact
        public ActionResult Index() {
			List<ContactViewModel> contacts = new Contacts().GetList();
			return View(contacts);
        }

		public ActionResult Delete(int ContactSerial) {
			new Contact(ContactSerial).Delete();
			return Json(true);
		}

		[HttpGet]
		public FileContentResult Export() {
			//TODO: check how names with commas are exported
			byte[] fileContent = System.Text.Encoding.ASCII.GetBytes(new Contacts().GetContacts());
			return File(fileContent, "application/text", "contacts.csv");
		}
	}
}