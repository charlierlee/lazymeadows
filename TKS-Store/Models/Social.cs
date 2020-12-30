using System.Text;
using System.Web;
using TKS.Areas.Admin.Models;

namespace TKS.Models {
	public static class Social {
		public static string BaseURL(string PageTitle) {
			string url = HttpContext.Current.Request.Url.ToString();
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 900, 600)' title='LinkedIn' class='noext'><img src='/assets/images/icons/linkedin.png' alt='LinkedIn' height='32' width='32' /></a>", "http://www.linkedin.com/shareArticle?mini=true&amp;url=" + url);
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 660, 500)' title='Twitter' class='noext'><img src='/assets/images/icons/twitter.png' alt='Twitter' height='32' width='32' /></a>", "https://twitter.com/intent/tweet?url=" + url + "&amp;text=" + HttpUtility.UrlEncode(PageTitle) + "&amp;via=" + Global.TwitterName);
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 500, 600)' title='Google+' class='noext'><img src='/assets/images/icons/googleplus.png' alt='Google+' height='32' width='32' /></a>", "https://plus.google.com/share?url=" + url);
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 660, 600)' title='facebook' class='noext'><img src='/assets/images/icons/facebook.png' alt='facebook' height='32' width='32' /></a>", "https://facebook.com/sharer.php?u=" + url);
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 800, 600)' title='StumbleUpon' class='noext'><img src='/assets/images/icons/stumbleupon.png' alt='StumbleUpon' height='32' width='32' /></a>", "http://www.stumbleupon.com/submit?url=" + url + "&amp;title=" + PageTitle);
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 660, 600)' title='Pinterest' class='noext'><img src='/assets/images/icons/pinterest.png' alt='Pinterest' height='32' width='32' /></a>", "http://pinterest.com/pin/create/button/?url=" + url);
			sb.AppendFormat("<a target='_blank' href='{0}' onclick='return windowpop(\"{0}\", 800, 600)' title='Reddit' class='noext'><img src='/assets/images/icons/reddit.png' alt='Reddit' height='32' width='32' /></a>", "http://reddit.com/submit?url=" + url + "&amp;title=" + PageTitle);
			sb.AppendFormat("<a target='_blank' href='{0}' title='Email'><img src='/assets/images/icons/email.png' alt='Email' height='32' width='32' /></a>", "mailto:" + Global.SiteEmail + "?subject=" + HttpUtility.UrlEncode(PageTitle));
			return sb.ToString();
		}
	}
}