using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Profile;
using System.Web.Routing;
using System.Web.Security;

namespace TKS {
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
		void Profile_MigrateAnonymous(object sender, ProfileMigrateEventArgs e) {
			using (System.Data.SqlClient.SqlConnection cn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("usp_MigrateOnLogin", cn)) {
					cmd.CommandType = System.Data.CommandType.StoredProcedure;
					cmd.Parameters.Add("UserID", System.Data.SqlDbType.UniqueIdentifier).Value = new Guid(Membership.GetUser().ProviderUserKey.ToString());
					cmd.Parameters.Add("AnonID", System.Data.SqlDbType.UniqueIdentifier).Value = new Guid(e.AnonymousID);
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			//Profile.shipping_ZipPostal = anonProfile.shipping_ZipPostal;
			//if (anonProfile.ShoppingCart.Count > 0) {
			//	Profile.ShoppingCart = anonProfile.ShoppingCart;
			//}

			//ProfileManager.DeleteProfile(e.AnonymousID);
			AnonymousIdentificationModule.ClearAnonymousIdentifier();
		}
	}
}
