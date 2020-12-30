using System.Web;
using System.Web.Optimization;

namespace TKS {
	public class BundleConfig {
			//// Set EnableOptimizations to false for debugging. For more information,
			//// visit http://go.microsoft.com/fwlink/?LinkId=301862
			//BundleTable.EnableOptimizations = true;

		public static void RegisterBundles(BundleCollection bundles) {
			var bundleScript = new ScriptBundle("~/js/std");
			bundleScript.Include("~/js/libs/jquery.megamenu.js");
			//bundleScript.Include("~/js/libs/jquery.caroufredsel-6.2.1.js");
			bundleScript.Include("~/js/libs/jquery.slicknav.js");
			bundleScript.Include("~/js/libs/jquery.fancybox.js");
			bundleScript.Include("~/js/libs/jquery.syncheight.js");
			bundleScript.Include("~/js/libs/jquery.rwdimagemaps.js");
			bundleScript.Include("~/js/plugins.js");
			bundleScript.Include("~/js/main.js");
			bundleScript.Include("~/js/libs/yaml-focusfix.js");

			//bundleScript.Include("~/scripts/backtotop.jquery.js");
			//bundleScript.Include("~/scripts/jquery.mousewheel-3.0.6.pack.js");
			//bundleScript.Include("~/scripts/jquery.fancybox-media.js");
			bundles.Add(bundleScript);

			var bundleAdminScript = new ScriptBundle("~/js/admin");
			bundleAdminScript.Include("~/areas/admin/js/libs/jquery-{version}.js");
			bundleAdminScript.Include("~/areas/admin/js/libs/jquery-ui.js");
			bundleAdminScript.Include("~/areas/admin/js/libs/jquery.validate.js");
			bundleAdminScript.Include("~/areas/admin/js/libs/additional-methods.js");
			bundleAdminScript.Include("~/areas/admin/js/libs/jquery.fancybox.js");
			bundleAdminScript.Include("~/areas/admin/js/libs/jquery.uploadfile.js");
			bundleAdminScript.Include("~/areas/admin/js/plugins.js");
			bundleAdminScript.Include("~/areas/admin/js/script.js");
			bundleAdminScript.Include("~/areas/admin/js/yaml-focusfix.js");
			bundles.Add(bundleAdminScript);

			bundles.Add(new ScriptBundle("~/js/jquery").Include(
						"~/js/libs/jquery-{version}.js"));

			bundles.Add(new ScriptBundle("~/js/jqueryui").Include(
						"~/js/libs/jquery-ui.js"));

			bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
						"~/js/libs/jquery.validate.js",
						//"~/js/libs/jquery.unobtrusive*",
						"~/js/libs/additional-methods.js"
						));

			bundles.Add(new ScriptBundle("~/js/modernizr").Include(
						"~/js/libs/modernizr-*"));

			var bundle = new Bundle("~/css/std");
			bundle.Include("~/css/normalize.css");
			bundle.Include("~/css/base.css");
			bundle.Include("~/css/elusive-icons.css");
			bundle.Include("~/css/font-awesome.css");
			bundle.Include("~/css/typography.css");
			bundle.Include("~/css/layout.css");
			bundle.Include("~/css/hlist.css");
			bundle.Include("~/css/smoothness/jquery-ui.css");
			bundle.Include("~/css/gray-theme.css");
			bundle.Include("~/css/jquery.megamenu.css");
			bundle.Include("~/css/fancybox/jquery.fancybox.css");
			bundle.Include("~/css/media.css");
			bundle.Include("~/css/print.css");
			bundle.Include("~/css/changes.css");
			bundles.Add(bundle);


			bundle = new Bundle("~/css/ie");
			bundle.Include("~/css/iehacks.css");
			bundles.Add(bundle);

			bundles.Add(new StyleBundle("~/admin/css").Include(
					"~/areas/admin/css/normalize.css",
					"~/areas/admin/css/base.css",
					"~/areas/admin/css/elusive-icons.css",
					"~/areas/admin/css/typography.css",
					"~/areas/admin/css/layout.css",

					"~/areas/admin/css/hlist.css",
					"~/areas/admin/css/gray-theme.css",

					"~/areas/admin/css/fancybox/jquery.fancybox.css",
					"~/areas/admin/css/uploadfile.css",
					"~/areas/admin/css/smoothness/jquery-ui.css",

					"~/areas/admin/css/media.css",
					"~/areas/admin/css/print.css",
					"~/areas/admin/css/changes.css"
			));
			bundles.Add(new StyleBundle("~/admin/css/ie").Include(
					  "~/css-admin/iehacks.css"));

			bundles.FileSetOrderList.Clear();
		}
	}
}
