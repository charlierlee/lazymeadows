using System.Collections.Generic;
using System.Web.Mvc;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers.CMS {
	[Authorize(Roles = "Admin")]
	public class TestimonialController : Controller
    {
        // GET: Admin/Testimonial
        public ActionResult Index()
        {
			List<TestimonialViewModel> testimonialList = new TestimonialSet().Testimonials();
			return View(testimonialList);
        }
    }
}