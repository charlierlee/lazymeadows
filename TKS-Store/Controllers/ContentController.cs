using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using SitemapLib;
using TKS.Models.CMS;
using TKS.Models.realestate;

namespace TKS.Controllers.CMS {
	public class ContentController : Controller {
		// GET: Content
		public ActionResult Index(string permalink) {
			string section = "";
			string qString = "/" + permalink;
			int firstSlash = 0;
			if (!string.IsNullOrEmpty(permalink)) { firstSlash = permalink.IndexOf('/'); }
			if (firstSlash > 0) {
				section = permalink.Substring(0, firstSlash);
			} else {
				section = permalink;
			}

			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;
				ViewBag.City = new MLSListings().GetCitySelectList("");

				switch (contentPage.PageTypeName) {
					case "Gallery Page":
						List<GalleryItemModel> gallery = new GalleryPage(contentPage.PageID).GetImages();
						return View("Gallery", gallery);
					case "News Index":
						List<NewsModel> news = new NewsSet(contentPage.PageID).News();
						return View("NewsIndex", news);
					case "Event Index":
						return View("EventIndex", new EventSet().Events());
					default:
						return View();
				}
			} else {
				if (!string.IsNullOrEmpty(contentPage.RedirectURL)) { return RedirectPermanent(contentPage.RedirectURL); }
				ViewBag.City = new Listing().GetCitySelectList("");
				contentPage = new ContentPage("404");
				Response.TrySkipIisCustomErrors = true;
				Server.ClearError();
				Response.Status = "404 not found";
				Response.StatusCode = 404;
				return View();
			}
		}

		public ActionResult EventIndex(int id, string detail) {

			Event e = new Event(id);
			if (e.EventSerial == 0) { return Redirect("/events"); }
			ViewBag.Meta = e.MetaTags;
			ViewBag.City = new Listing().GetCitySelectList("");

			return View(new EventSet().Events());
		}

		public ActionResult EventArticle(int id, string detail) {

			Event e = new Event(id);
			if (e.EventSerial == 0) { return Redirect("/events"); }
			ViewBag.Meta = e.MetaTags;
			ViewBag.City = new Listing().GetCitySelectList("");

			return View("EventArticle", new EventViewModel(e));
		}

		public ActionResult NewsArticle(int id, string detail) {

			News news = new News(id);
			if (news.NewsSerial == 0) { return Redirect("/support/news"); }
			ViewBag.Meta = news.MetaTags;
			ViewBag.City = new Listing().GetCitySelectList("");

			return View("NewsArticle", new NewsModel(news));
		}

		public ContentResult Sitemap() {
			SitemapLib.Sitemap sitemap = new SitemapLib.Sitemap();
			string host = Request.Url.Scheme + "://" + Request.Url.Host;
			Random random = new Random();

			//Add regular pages
			foreach (ContentPageBasicViewModel page in ContentPages.Pages()) {
				if (page.IncludeInSitemap) {
					sitemap.AddLocation(host + page.VirtualPath, DateTime.Today, "0." + random.Next(3, 9).ToString(), ChangeFrequency.Monthly);
				}
			}

			//Add blog post pages
			TKS.Areas.Admin.Models.Blog.BlogPosts blogPosts = new TKS.Areas.Admin.Models.Blog.BlogPosts("en-US");
			foreach (TKS.Areas.Admin.Models.Blog.BlogPostViewModel blogPost in blogPosts.EveryPost()) {
				sitemap.AddLocation(host + blogPost.URL, blogPost.PublishDate, "0." + random.Next(3, 9).ToString(), ChangeFrequency.Yearly);
			}

			//Add News pages
			TKS.Areas.Admin.Models.CMS.NewsSet newsSet = new TKS.Areas.Admin.Models.CMS.NewsSet();
			foreach (TKS.Areas.Admin.Models.CMS.NewsModel news in newsSet.News()) {
				sitemap.AddLocation(host + news.URL, news.DateReleased, "0." + random.Next(3, 9).ToString(), ChangeFrequency.Never);
			}


			Response.ContentType = "text/xml";

			return Content(sitemap.GenerateSitemapXML());
		}

		public ActionResult Social(string detail) {

			//Event e = new Event(id);
			//if (e.EventSerial == 0) { return Redirect("/events"); }
			//ViewBag.Meta = e.MetaTags;

			SocialSites data = new SocialSites(detail);
			if (string.IsNullOrEmpty(data.TownFormatted)) { return Redirect("/social"); }

			ViewBag.City = new Listing().GetCitySelectList("");

			return View(data);
		}

		[ChildActionOnly]
		public ActionResult CommunityCalendar() {
			return PartialView(new EventSet().Events(4));
		}

	}
}