using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using TKS.Models;
using TKS.Areas.Admin.Models.CMS;
using TKS.Areas.Admin.Models;
using TKS.Models.Blog;

namespace TKS.Controllers.Blog {
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index() {
			string section = "blog";
			string qString = "/blog";

			Language language = new Language("en-US");
			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;

				MenuItem sideMenu = new MenuItem(Request.Url.AbsolutePath);
				ViewBag.SideMenu = sideMenu.SiblingMenuCode();

				ViewBag.Highlight = "$('." + section + "').addClass('active');";
			}

			ViewBag.BlogHeaderExt = "Blog";

			//Paging defaults
			int pg = 1;
			int sz = 20;

			//Retrieve / Store paging parameters
			if (Request.QueryString["pg"] != null) {
				if (!Int32.TryParse(Request.QueryString["pg"].ToString(), out pg)) { pg = 1; }
			}
			if (Request.QueryString["sz"] != null) {
				if (!Int32.TryParse(Request.QueryString["sz"].ToString(), out sz)) { sz = 20; }
			}

			BlogPosts posts = new BlogPosts();
			List<BlogPost> blogs = posts.AllPosts(pg, sz);
			ViewBag.Pager = posts.AllPostsPager(pg, sz);
			
			return View(blogs);
        }

		// GET: Blog/author/{id}/{author}
		public ActionResult IndexAuthor(Guid id, string author) {
			string section = "blog";
			string qString = "/blog";

			Language language = new Language("en-US");
			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;

				MenuItem sideMenu = new MenuItem(Request.Url.AbsolutePath);
				ViewBag.SideMenu = sideMenu.SiblingMenuCode();

				ViewBag.Highlight = "$('." + section + "').addClass('active');";
			}

			ViewBag.BlogHeaderExt = "Blogs by: " + BlogPost.GetAuthorName(id);

			//Paging defaults
			int pg = 1;
			int sz = 20;

			//Retrieve / Store paging parameters
			if (Request.QueryString["pg"] != null) {
				if (!Int32.TryParse(Request.QueryString["pg"].ToString(), out pg)) { pg = 1; }
			}
			if (Request.QueryString["sz"] != null) {
				if (!Int32.TryParse(Request.QueryString["sz"].ToString(), out sz)) { sz = 20; }
			}

			BlogPosts posts = new BlogPosts();
			List<BlogPost> blogs = posts.PostsByAuthor(id, pg, sz);
			ViewBag.Pager = posts.ByAuthorPager(id, author, pg, sz);

			return View("Index", blogs);
		}

		// GET: Blog/cat/{id}/{category}
		public ActionResult IndexCategory(int id, string category) {
			string section = "blog";
			string qString = "/blog";

			Language language = new Language("en-US");
			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;

				MenuItem sideMenu = new MenuItem(Request.Url.AbsolutePath);
				ViewBag.SideMenu = sideMenu.SiblingMenuCode();

				ViewBag.Highlight = "$('." + section + "').addClass('active');";
			}

			ViewBag.BlogHeaderExt = "Blog Category: " + BlogPost.GetCategory(id);

			//Paging defaults
			int pg = 1;
			int sz = 20;

			//Retrieve / Store paging parameters
			if (Request.QueryString["pg"] != null) {
				if (!Int32.TryParse(Request.QueryString["pg"].ToString(), out pg)) { pg = 1; }
			}
			if (Request.QueryString["sz"] != null) {
				if (!Int32.TryParse(Request.QueryString["sz"].ToString(), out sz)) { sz = 20; }
			}

			BlogPosts posts = new BlogPosts();
			List<BlogPost> blogs = posts.PostsInCategory(id, pg, sz);
			ViewBag.Pager = posts.CategoryPager(id, category, pg, sz);

			return View("Index", blogs);
		}

		// GET: Blog/month/{month}
		public ActionResult IndexMonth(string month) {
			string section = "blog";
			string qString = "/blog";

			Language language = new Language("en-US");
			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;

				MenuItem sideMenu = new MenuItem(Request.Url.AbsolutePath);
				ViewBag.SideMenu = sideMenu.SiblingMenuCode();

				ViewBag.Highlight = "$('." + section + "').addClass('active');";
			}

			DateTime currentMonth;
			if (DateTime.TryParse(month.Substring(0, 2) + "/1/" + month.Substring(2), out currentMonth)) {
				ViewBag.BlogHeaderExt = "Blogs From: " + currentMonth.ToString("MMMM yyyy");
			} else {
				ViewBag.BlogHeaderExt = "Blogs From: " + month.Substring(0, 2) + "-" + month.Substring(2);
			}

			//Paging defaults
			int pg = 1;
			int sz = 20;

			//Retrieve / Store paging parameters
			if (Request.QueryString["pg"] != null) {
				if (!Int32.TryParse(Request.QueryString["pg"].ToString(), out pg)) { pg = 1; }
			}
			if (Request.QueryString["sz"] != null) {
				if (!Int32.TryParse(Request.QueryString["sz"].ToString(), out sz)) { sz = 20; }
			}

			BlogPosts posts = new BlogPosts();
			List<BlogPost> blogs = posts.PostsInMonth(month, pg, sz);
			ViewBag.Pager = posts.MonthPager(month, pg, sz);

			return View("Index", blogs);
		}

		// GET: Blog/tag/{tag}
		public ActionResult IndexTag(string tag) {
			string section = "blog";
			string qString = "/blog";

			Language language = new Language("en-US");
			ContentPage contentPage = new ContentPage(qString);
			if (contentPage.PageID != Guid.Empty) {
				ViewData = contentPage.GetSections();
				ViewBag.Meta = contentPage.MetaTags;

				MenuItem sideMenu = new MenuItem(Request.Url.AbsolutePath);
				ViewBag.SideMenu = sideMenu.SiblingMenuCode();

				ViewBag.Highlight = "$('." + section + "').addClass('active');";
			}

			ViewBag.BlogHeaderExt = "Blogs Tagged: " + tag;

			//Paging defaults
			int pg = 1;
			int sz = 20;

			//Retrieve / Store paging parameters
			if (Request.QueryString["pg"] != null) {
				if (!Int32.TryParse(Request.QueryString["pg"].ToString(), out pg)) { pg = 1; }
			}
			if (Request.QueryString["sz"] != null) {
				if (!Int32.TryParse(Request.QueryString["sz"].ToString(), out sz)) { sz = 20; }
			}

			BlogPosts posts = new BlogPosts();
			List<BlogPost> blogs = posts.PostsTagged(tag, pg, sz);
			ViewBag.Pager = posts.TagPager(tag, pg, sz);

			return View("Index", blogs);
		}

		[HttpPost, ValidateInput(false)]
		public JsonResult CommentSubmit(BlogCommentViewModel commentViewModel) {
			BlogPost blogPost = new BlogPost(commentViewModel.PostID);
			TKS.Models.User user = new TKS.Models.User();
			commentViewModel.Author = user.UserName;
			commentViewModel.CommentDate = DateTime.Now;
			commentViewModel.Email = user.Email;
			commentViewModel.IP = Request.UserHostAddress;
			blogPost.AddComment(commentViewModel);

			return Json(true);
		}

        // GET: Blog/Details/5
        public ActionResult Details(int id)
        {
			BlogPost blogPost = new BlogPost(id);
			if (blogPost.PostSerial == 0) { return Redirect("/blog"); }

			if (blogPost.URL != Request.Url.AbsolutePath) {
				return RedirectPermanent(blogPost.URL);
			}

			BlogPostViewModel blogPostViewModel = new BlogPostViewModel(blogPost);
			ViewBag.Meta = blogPost.MetaTags;

			//EmailNotification.Checked = blogPost.CommentsSubscribed;

			if (User.IsInRole("Blog Admin")) {
				blogPostViewModel.EditLink = "<div class='blog-edit'><a href='/admin/blog/edit/" + blogPost.PostSerial.ToString() + "'>Edit</a></div>";
			}

			return View(blogPostViewModel);
        }

		public ContentResult RSS() {
			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;

			writer.WriteStartDocument();
			writer.WriteStartElement("rss");
			writer.WriteAttributeString("version", "2.0");
			writer.WriteAttributeString("xmlns:atom", "http://www.w3.org/2005/Atom");
			writer.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
			writer.WriteStartElement("channel");
			writer.WriteElementString("title", Global.FeedTitle);
			writer.WriteElementString("link", Request.Url.AbsoluteUri);
			writer.WriteElementString("description", Global.FeedDescription);
			writer.WriteElementString("copyright", "(c) " + DateTime.Now.Year.ToString() + ", " + Global.TitleSuffix + ". All rights reserved.");
			writer.WriteElementString("language", "en-US");
			writer.WriteStartElement("atom:link");
			writer.WriteAttributeString("href", Request.Url.AbsoluteUri);
			writer.WriteAttributeString("rel", "self");
			writer.WriteAttributeString("type", "application/rss+xml");
			writer.WriteEndElement();

			List<BlogPost> blogs = new BlogPosts("en-US").AllPosts();
			foreach (BlogPost blog in blogs) {
				writer.WriteStartElement("item");
				writer.WriteElementString("title", blog.Title);
				writer.WriteElementString("description", blog.Description);
				writer.WriteElementString("link", Request.Url.Scheme + "://" + Request.Url.Host + blog.URL); 
				writer.WriteElementString("pubDate", blog.PublishDate.ToString("R"));
				writer.WriteElementString("guid", Request.Url.Scheme + "://" + Request.Url.Host + "/blog/post.aspx?guid=" + blog.PostID);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndDocument();
			writer.Flush();
			//writer.Close();

			stream.Position = 0;
			StreamReader sr = new StreamReader(stream);

			return Content(sr.ReadToEnd(), "application/rss+xml", Encoding.UTF8);
		}

		private string FormatBlogs(List<BlogPost> blogs) {
			int col = 1;
			StringBuilder sb = new StringBuilder("<div class='blog-posts'>");
			foreach (BlogPost post in blogs) {
				if (col == 1) {
					sb.AppendLine("<div class='ym-grid linearize-level-2 ym-equalize'>");
					sb.AppendLine("<div class='ym-g33 ym-gl'><div class='ym-gbox'><div class='item equalize'>");
				} else if (col == 2) {
					sb.AppendLine("<div class='ym-g33 ym-gl'><div class='ym-gbox'><div class='item equalize'>");
				} else if (col == 3) {
					sb.AppendLine("<div class='ym-g33 ym-gr'><div class='ym-gbox'><div class='item equalize'>");
				}
				if (!string.IsNullOrEmpty(post.ThumbnailFilename)) {
					sb.AppendLine("<div class='blog-thumb'><a href='" + post.URL + "'><img src='" + post.ThumbnailFilename + "' alt='' style='width:100%' /></a></div>");
				}
				sb.AppendLine("<div class='blog-title'><a href='" + post.URL + "'>" + post.Title + "</a></div>");
				sb.AppendLine("<div class='excerpt'>" + post.Description + "</div>");
				sb.AppendLine("<div class='read-more'><a href='" + post.URL + "'>Read More</a></div>");
				if (col == 1) {
					sb.AppendLine("</div></div></div>");
					col = 2;
				} else if (col == 2) {
					sb.AppendLine("</div></div></div>");
					col = 3;
				} else if (col == 3) {
					sb.AppendLine("</div></div></div>");
					sb.AppendLine("</div>");
					col = 1;
				}
			}
			if (col == 1) {
				sb.AppendLine("</div>");
			} else if (col == 2) {
				sb.AppendLine("<div class='ym-g33 ym-gl'></div><div class='ym-g33 ym-gr'></div>");
				sb.AppendLine("</div>");
				sb.AppendLine("</div>");
			} else if (col == 3) {
				sb.AppendLine("<div class='ym-g33 ym-gr'></div>");
				sb.AppendLine("</div>");
				sb.AppendLine("</div>");
			}
			return sb.ToString();
		}
    }
}
