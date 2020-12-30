using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Indexer;
using TKS;
using TKS.Areas.Admin.Models.Blog;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Controllers
{
    public class SearchController : Controller
    {
        // GET: Admin/Search
        public ActionResult Index() {
            return View();
        }
		// POST: Admin/Search
		[HttpPost]
		public ActionResult Index(FormCollection data) {
			LuceneIndexer li = new LuceneIndexer();
			li.DeleteIndex(false);
			li.CreateIndexWriter();

			//Index blog posts
			BlogPosts blogPosts = new BlogPosts("en-US");
			foreach (BlogPostViewModel blogPost in blogPosts.EveryPost()) {
				li.AddWebPage(blogPost.PostID.ToString(), blogPost.URL, blogPost.Title, blogPost.PostContent, "Blog");
			}

			////Index Documents
			//DocumentSet docs = new DocumentSet("en-US");
			//foreach (TKS.Document doc in docs.Documents()) {
			//	if (!string.IsNullOrEmpty(doc.DocumentTitle)) {
			//		li.AddWebPage(doc.DocumentID.ToString(), doc.URL, doc.DocumentTitle, doc.Description, "Document");
			//	} else {
			//		li.AddWebPage(doc.DocumentID.ToString(), doc.URL, doc.LinkText, doc.Description, "Document");
			//	}
			//}

			////Index FAQs
			//FaqSet faqs = new FaqSet("en-US");
			//foreach (Faq faq in faqs.FAQs()) {
			//	li.AddWebPage(faq.FaqID.ToString(), faq.URL, faq.Question, faq.Question + " " + faq.Answer, "FAQ");
			//}

			////Index News
			//NewsSet newsSet = new NewsSet();
			//foreach (News news in newsSet.News()) {
			//	li.AddWebPage(news.NewsID.ToString(), news.URL, news.Headline, news.Content, "News");
			//}

			//Index Content Blocks
			ContentTextSet contentSet = new ContentTextSet("en-US");
			foreach (ContentText content in contentSet.AllContentTextBlocks()) {
				li.AddWebPage(content.ContentID.ToString(), content.URL, content.PageTitle, content.Contents, "Page");
			}

			li.Close();
			li.IndexWords();

			return View();
		}
	}
}