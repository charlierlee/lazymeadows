using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TKS {
	public class RouteConfig {
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
			routes.IgnoreRoute("{resource}.ascx/{*pathInfo}");
			routes.LowercaseUrls = true;

			routes.MapRoute(
				"Sitemap",
				"sitemap.xml",
				defaults: new { controller = "Content", action = "Sitemap" }
			);

			routes.MapRoute(
				name: "login",
				url: "login/{action}",
				defaults: new { controller = "Login", action = "Index" }
			);
			routes.MapRoute(
				name: "search",
				url: "search",
				defaults: new { controller = "RealEstate", action = "Search" }
			);
			routes.MapRoute(
				name: "searchland",
				url: "searchland",
				defaults: new { controller = "RealEstate", action = "SearchLand" }
			);
			routes.MapRoute(
				name: "searchcomm",
				url: "searchcomm",
				defaults: new { controller = "RealEstate", action = "SearchComm" }
			);
			routes.MapRoute(
				name: "detail",
				url: "detail",
				defaults: new { controller = "RealEstate", action = "Detail" }
			);
			routes.MapRoute(
				name: "landdetail",
				url: "landdetail",
				defaults: new { controller = "RealEstate", action = "LandDetail" }
			);
			routes.MapRoute(
				name: "commdetail",
				url: "commdetail",
				defaults: new { controller = "RealEstate", action = "CommDetail" }
			);
            routes.MapRoute(
                name: "detail2",
                url: "detail2",
                defaults: new { controller = "RealEstate", action = "Detail2" }
            );
            routes.MapRoute(
                name: "Sold",
                url: "sold",
                defaults: new { controller = "RealEstate", action = "Sold" }
            );
            routes.MapRoute(
				"contact-us",
				"contact-us",
				defaults: new { controller = "Contact", action = "Index" },
				namespaces: new[] { "TKS.Controllers" }
			);
			routes.MapRoute(
				"process contact",
				"contact/docontact",
				defaults: new { controller = "Contact", action = "DoContact" },
				namespaces: new[] { "TKS.Controllers" }
			);
			routes.MapRoute(
				"news",
				"support/news/{id}/{detail}",
				defaults: new { controller = "Content", action = "NewsArticle" }
			);
			routes.MapRoute(
				"event",
				"events/{id}/{detail}",
				defaults: new { controller = "Content", action = "EventArticle" }
			);
			routes.MapRoute(
				"social",
				"social/{detail}",
				defaults: new { controller = "Content", action = "Social" }
			);
			routes.MapRoute(
				name: "blog rss",
				url: "blog/rss",
				defaults: new { controller = "blog", action = "RSS" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog category",
				url: "blog/cat/{id}/{category}",
				defaults: new { controller = "blog", action = "IndexCategory" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog tag",
				url: "blog/tag/{tag}",
				defaults: new { controller = "blog", action = "IndexTag" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog month",
				url: "blog/month/{month}",
				defaults: new { controller = "blog", action = "IndexMonth" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog author",
				url: "blog/author/{id}/{author}",
				defaults: new { controller = "blog", action = "IndexAuthor" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog comment submit",
				url: "blog/commentsubmit",
				defaults: new { controller = "blog", action = "CommentSubmit" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog post",
				url: "blog/{id}/{detail}",
				defaults: new { controller = "blog", action = "details" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);
			routes.MapRoute(
				name: "blog",
				url: "blog",
				defaults: new { controller = "blog", action = "index" },
				namespaces: new[] { "TKS.Controllers.Blog" }
			);

			routes.MapRoute(
				name: "Services",
				url: "svc/{action}/{id}",
				defaults: new { controller = "Service", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "MyAccount",
				url: "myaccount/{action}/{id}",
				defaults: new { controller = "MyAccount", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Newsletter",
				url: "newsletter/{action}",
				defaults: new { controller = "Newsletter" }
			);

			routes.MapRoute(
				name: "CmsHomeRoute",
				url: "",
				defaults: new { controller = "Home", action = "Index" }
			);


			routes.MapRoute(
				name: "Real estate processor",
				url: "realestate/{action}",
				defaults: new { controller = "RealEstate" }
			);


			routes.MapRoute(
				name: "MLS",
				url: "mls",
				defaults: new { controller = "MLS", action = "Index" }
			);

			routes.MapRoute(
				name: "CmsRoute",
				url: "{*permalink}",
				defaults: new { controller = "Content", action = "Index" },
				constraints: new { permalink = new BaseRoutingConstraint() },
				namespaces: new[] { "TKS.Controllers" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				namespaces: new[] { "TKS.Controllers" }

			);
		}
	}

	public class BaseRoutingConstraint : IRouteConstraint {
		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection) {
			if (values[parameterName] != null) {
				var permalink = string.Format("~/{0}", values[parameterName].ToString());
				return true;
				//return BaseSiteMapNode.ReturnAll().Any(a => a.Url == permalink);
			}

			return false;
		}
	}
}
