using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
using TKS.Areas.Admin.Models.Blog;

namespace TKS.Areas.Admin.Controllers.Blog {
	[Authorize(Roles = "Admin")]
	public class BlogController : Controller
    {
        // GET: Admin/Blog
        public ActionResult Index() {
			List<BlogPostViewModel> posts = new BlogPosts().EveryPost();
			return View(posts);
		}

		#region SEO
		//AJAX: Admin/Blog/SEO/<PostSerial>
		public ActionResult SEO(int id) {
			BlogPost blogPost = new BlogPost(id);
			SEO seo = blogPost.seo;
			ViewBag.TypeOG = seo.GetOGOptions(seo.ogType);
			ViewBag.PostURL = "/admin/blog/seo";
			ViewBag.ImageBase = "/blog";
			SEOViewModel seoViewModel = new SEOViewModel(seo);
			if (string.IsNullOrEmpty(seoViewModel.MetaTitle)) { seoViewModel.MetaTitle = blogPost.Title; }
			if (string.IsNullOrEmpty(seoViewModel.MetaDescription)) { seoViewModel.MetaDescription = blogPost.Description; }

			return PartialView("_SEO", seoViewModel);
		}
		// JSON: Admin/Blog/SEO/<PostSerial>
		[HttpPost]
		public JsonResult SEO(SEOViewModel data) {
			//TODO: look into image uploads.
			//TODO: Look into sizing images
			//TODO: Show image samples
			BlogPost blogPost = new BlogPost(data.EntityID);
			SEO seo = blogPost.seo;

			seo.gDescription = data.gDescription;
			seo.gImage = data.gImage;
			seo.gName = data.gName;
			seo.MetaDescription = data.MetaDescription;
			seo.MetaKeywords = data.MetaKeywords;
			seo.MetaTitle = data.MetaTitle;
			seo.ogDescription = data.ogDescription;
			seo.ogImage = data.ogImage;
			seo.ogTitle = data.ogTitle;
			seo.ogType = data.ogType;
			seo.twitterDescription = data.twitterDescription;
			seo.twitterImage = data.twitterImage;
			seo.twitterTitle = data.twitterTitle;

			return Json(true);
		}
		#endregion

		#region Comments
		// JSON: Admin/Blog/Comments/<PostSerial>
		public ActionResult Comments(int id) {
			BlogPost blogPost = new BlogPost(id);
			return PartialView(blogPost);
		}
		// JSON: Admin/Blog/Comments/<PostSerial>
		[HttpPost]
		public JsonResult Comments(BlogPostViewModel data) {
			try {
				if (ModelState.IsValid) {
					BlogPost blogPost = new BlogPost(data.PostID);
					blogPost.IsCommentEnabled = data.IsCommentEnabled;
				}
				return Json(true);
			} catch {
				return Json(false);
			}
		}

		// JSON: Admin/Blog/CommentApprove/<CommentID>
		[HttpPost]
		public JsonResult CommentApprove(Guid commentid) {
			try {
				if (ModelState.IsValid) {
					BlogComment comment = new BlogComment(commentid);
					comment.IsApproved = true;
				}
				return Json(true);
			} catch {
				return Json(false);
			}
		}

		// JSON: Admin/Blog/CommentDelete/<CommentID>
		[HttpPost]
		public JsonResult CommentDelete(Guid commentid) {
			try {
				if (ModelState.IsValid) {
					BlogComment comment = new BlogComment(commentid);
					comment.IsDeleted = true;
				}
				return Json(true);
			} catch {
				return Json(false);
			}
		}

		// POST: Admin/Blog/CommentEdit/<CommentID>
		[HttpPost, ValidateInput(false)]
		public JsonResult CommentEdit(Guid commentid, string comment) {
			try {
				if (ModelState.IsValid) {
					BlogComment blogComment = new BlogComment(commentid);
					blogComment.Comment = comment ?? "";
				}
				return Json(true);
			} catch {
				return Json(false);
			}
		}
		#endregion

		#region Create
		// GET: Admin/Blog/Create
        public ActionResult Create()
        {
			ViewBag.AuthorID = BlogAuthors.AuthorList(Guid.Empty);
			BlogCategories blogCatgories = new BlogCategories("en-US");
			ViewBag.AllCategories = blogCatgories.Categories();

			return View(new BlogPostViewModel());
        }

        // POST: Admin/Blog/Create
		[HttpPost, ValidateInput(false)]
		public ActionResult Create(BlogPostViewModel data)
        {
			try {
				if (ModelState.IsValid) {
					BlogPosts blogPosts = new BlogPosts();
					BlogPost post = blogPosts.Add(data);
					if (post.PostSerial > 0) {
						return RedirectToAction("Edit", new { id = post.PostSerial.ToString() });
					} else {
						return View();
					}
				}
				return View();
			} catch {
				return View();
			}
        }
		#endregion

		// POST: Admin/Blog/Delete/<PostSerial>
		[HttpPost]
		public JsonResult Delete(int id) {
			try {
				BlogPost blogPost = new BlogPost(id);
				blogPost.Delete();

				return Json(true);
			} catch {
				return Json(false);
			}
		}

		#region Edit
		// GET: Admin/Blog/Edit/<PostSerial>
        public ActionResult Edit(int id)
        {
			BlogPost post = new BlogPost(id);
			ViewBag.AuthorID = BlogAuthors.AuthorList(post.AuthorID);
			BlogCategories blogCatgories = new BlogCategories("en-US");
			ViewBag.AllCategories = blogCatgories.Categories();

			return View(new BlogPostViewModel(post));
		}

		// POST: Admin/Blog/Edit/<PostSerial>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(int id, BlogPostViewModel data) {
			try {
				if (ModelState.IsValid) {
					BlogPost post = new BlogPost(id);
					post.Title = data.Title;
					post.AuthorID = data.AuthorID;
					post.Slug = data.Slug;
					post.PublishDate = data.PublishDate;
					post.Description = data.Description;
					post.ThumbnailFilename = data.ThumbnailFilename;
					post.HeroFilename = data.HeroFilename;
					post.PostContent = data.PostContent;
					post.IsPublished = data.IsPublished;
					post.IsCommentEnabled = data.IsCommentEnabled;
					post.Tags = data.Tags;
					List<Guid> cats = new List<Guid>();
					foreach (string cat in data.Categories) {
						cats.Add(new Guid(cat));
					}
					post.AssignedCategories = cats;
				}
				return RedirectToAction("Index");
			} catch {
				return View(data);
			}
		}
		#endregion

		#region Extras
		// JSON: Admin/Blog/Extras/<PostSerial>
		public ActionResult Extras(int id) {
			BlogPost blogPost = new BlogPost(id);
			return PartialView(new BlogPostViewModel(blogPost));
		}

		// JSON: Admin/Blog/Extras/<PostSerial>
		[HttpPost, ValidateInput(false)]
		public JsonResult Extras(BlogPostViewModel data) {
			try {
				if (ModelState.IsValid) {
					BlogPost blogPost = new BlogPost(data.PostID);
					blogPost.AudioFilename = data.AudioFilename;
					blogPost.VideoPath = data.VideoPath;
				}
				return Json(true);
			} catch {
				return Json(false);
			}
		}
		#endregion
	}

	[Authorize(Roles = "Admin")]
	public class BlogCategoryController : Controller {
		// GET: Admin/Blog
		public ActionResult Index() {
			List<BlogCategoryViewModel> categories = new BlogCategories().Categories();
			return View(categories);
		}

		#region Create
		// POST: Admin/BlogCategory/Create
		[HttpPost, ValidateInput(false)]
		public ActionResult Create(BlogCategoryViewModel data) {
			try {
				if (ModelState.IsValid) {
					Guid cat = new BlogCategories().Add(data);
				}
				return RedirectToAction("Index");
			} catch {
				return RedirectToAction("Index");
			}
		}
		#endregion

		// POST: Admin/BlogCategory/Delete/<CategoryID>
		[HttpPost]
		public JsonResult Delete(Guid id) {
			BlogCategory cat = new BlogCategory(id);
			cat.Delete();

			return Json(true);
		}

		#region Edit
		// GET: Admin/BlogCategory/Edit/<CategoryID>
		public ActionResult Edit(Guid id) {
			BlogCategory cat = new BlogCategory(id);
			return View(new BlogCategoryViewModel(cat));
		}

		// POST: Admin/BlogCategory/Edit/<CategoryID>
		[HttpPost, ValidateInput(false)]
		public ActionResult Edit(Guid id, BlogCategoryViewModel data) {
			try {
				if (ModelState.IsValid) {
					BlogCategory cat = new BlogCategory(id);
					cat.CategoryName = data.CategoryName;
					cat.Description = data.Description;
				}
				return RedirectToAction("Index");
			} catch {
				return RedirectToAction("Index");
			}
		}
		#endregion
	}

	[Authorize(Roles = "Admin")]
	public class BlogTagController : Controller {
		// GET: Admin/Blog
		public ActionResult Index() {
			List<BlogTagViewModel> tags = new BlogTags().Tags();
			return View(tags);
		}

		// POST: Admin/BlogTag/Delete
		[HttpPost]
		public JsonResult Delete(BlogTagViewModel data) {
			new BlogTags().Delete(data.Tag);

			return Json(true);
		}
	}
}
