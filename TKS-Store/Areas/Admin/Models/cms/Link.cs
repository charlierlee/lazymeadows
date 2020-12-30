using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class Link
	{
		#region Fields

		private int _LinkSerial = 0;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		private string _LinkLabel = "";
		private string _LinkDescription = "";
		private string _LinkURL = "";
		private int _LinkCategorySerial = 0;

		#endregion

		#region Constructor

		public Link(int LinkSerial)
		{
			_LinkSerial = LinkSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM cms_Link WHERE LinkSerial = @LinkSerial", cn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("LinkSerial", SqlDbType.Int).Value = _LinkSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows)
					{
						dr.Read();
						_LinkCategorySerial = Convert.ToInt32(dr["LinkCategorySerial"]);
						_Locale = dr["Locale"].ToString();
						_LinkLabel = dr["LinkLabel"].ToString();
						_LinkURL = dr["LinkURL"].ToString();
						_LinkDescription = dr["LinkDescription"].ToString();
					}
					else
					{
						_LinkSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}

		#endregion

		#region Properties

		public int LinkSerial { get { return _LinkSerial; } }
		public string Locale
		{
			get { return _Locale; }
			set
			{
				if (_Locale != value)
				{
					_Locale = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
					{
						using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Link] SET [Locale] = @Locale WHERE LinkSerial = @LinkSerial", cn))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@LinkSerial", SqlDbType.Int).Value = _LinkSerial;
							cmd.Parameters.Add("@Locale", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string LinkLabel
		{
			get { return _LinkLabel; }
			set
			{
				if (_LinkLabel != value)
				{
					_LinkLabel = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
					{
						using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Link] SET [LinkLabel] = @LinkLabel WHERE LinkSerial = @LinkSerial", cn))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@LinkSerial", SqlDbType.Int).Value = _LinkSerial;
							cmd.Parameters.Add("@LinkLabel", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string LinkDescription
		{
			get { return _LinkDescription; }
			set
			{
				if (_LinkDescription != value)
				{
					_LinkDescription = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
					{
						using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Link] SET [LinkDescription] = @LinkDescription WHERE LinkSerial = @LinkSerial", cn))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@LinkSerial", SqlDbType.Int).Value = _LinkSerial;
							cmd.Parameters.Add("@LinkDescription", SqlDbType.VarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string LinkURL
		{
			get { return _LinkURL; }
			set
			{
				if (_LinkURL != value)
				{
					_LinkURL = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
					{
						using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Link] SET [LinkURL] = @LinkURL WHERE LinkSerial = @LinkSerial", cn))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@LinkSerial", SqlDbType.Int).Value = _LinkSerial;
							cmd.Parameters.Add("@LinkURL", SqlDbType.VarChar, 250).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public int LinkCategorySerial
		{
			get { return _LinkCategorySerial; }
			set
			{
				if (_LinkCategorySerial != value)
				{
					_LinkCategorySerial = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
					{
						using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Link] SET [LinkCategorySerial] = @LinkCategorySerial WHERE LinkSerial = @LinkSerial", cn))
						{
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@LinkSerial", SqlDbType.Int).Value = _LinkSerial;
							cmd.Parameters.Add("@LinkCategorySerial", SqlDbType.Int).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}

		#endregion

		#region Public Methods

		public void Delete()
		{
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_Link WHERE LinkSerial = @LinkSerial", cn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@LinkSerial", SqlDbType.Int).Value = _LinkSerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}

		#endregion
	}

	public class LinkSet
	{
		#region Fields

		private Guid _PageID = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";

		#endregion

		#region Constructor

		public LinkSet(Guid ModuleID, string Locale)
		{
			_ModuleID = ModuleID;
			_Locale = Locale;
		}

		public LinkSet(Guid PageID, string ModuleName, string Locale)
		{
			_PageID = PageID;
			_Locale = Locale;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID FROM cms_Module WHERE PageID = @PageID AND ModuleName = @ModuleName", cn))
				{
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("ModuleName", SqlDbType.VarChar, 50).Value = ModuleName;
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows)
					{
						dr.Read();
						_ModuleID = dr.GetGuid(0);
					}
					cmd.Connection.Close();
				}
			}
		}

		#endregion

		#region Public Methods

		public void Add(LinkViewModel linkViewModel)
		{
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("INSERT INTO [cms_Link] ([ModuleID], [Locale], [LinkLabel], [LinkDescription], [LinkURL], [LinkCategorySerial]) VALUES (@ModuleID, @Locale, @LinkLabel, @LinkDescription, @LinkURL, @LinkCategorySerial)", cn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("@Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("@LinkLabel", SqlDbType.VarChar, 250).Value = linkViewModel.LinkLabel;
					cmd.Parameters.Add("@LinkDescription", SqlDbType.VarChar).Value = linkViewModel.LinkDescription;
					cmd.Parameters.Add("@LinkURL", SqlDbType.VarChar, 250).Value = linkViewModel.LinkURL;
					cmd.Parameters.Add("@LinkCategorySerial", SqlDbType.Int).Value = linkViewModel.LinkCategorySerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}

		public List<LinkViewModel> Links()
		{
			List<LinkViewModel> list = new List<LinkViewModel>();

			if (_ModuleID != Guid.Empty)
			{
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand("SELECT d.*, dc.DisplayName AS CategoryName, dc.LinkCategorySerial  FROM cms_Link d JOIN cms_LinkCategory dc ON d.LinkCategorySerial = dc.LinkCategorySerial WHERE d.ModuleID = @ModuleID AND Locale = @Locale ORDER BY dc.SortOrder, dc.DisplayName, d.LinkLabel", cn))
					{
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read())
						{
							LinkViewModel linkViewModel = new LinkViewModel();
							linkViewModel.LinkSerial = Convert.ToInt32(dr["LinkSerial"]);
							linkViewModel.LinkCategorySerial = Convert.ToInt32(dr["LinkCategorySerial"]);
							linkViewModel.Locale = dr["Locale"].ToString();
							linkViewModel.LinkLabel = dr["LinkLabel"].ToString();
							linkViewModel.LinkURL = dr["LinkURL"].ToString();
							linkViewModel.LinkDescription = dr["LinkDescription"].ToString();
							linkViewModel.CategoryName = dr["CategoryName"].ToString();
							list.Add(linkViewModel);
						}
						cmd.Connection.Close();
					}
				}
			}

			return list;
		}

		#endregion
	}

	public class LinkViewModel
	{
		public int LinkSerial { get; set; }
		public Guid ModuleID { get; set; }
		public string Locale { get; set; }
		public string LinkLabel { get; set; }
		public string LinkDescription { get; set; }
		public string LinkURL { get; set; }
		public int LinkCategorySerial { get; set; }
		public string CategoryName { get; set; }
	}

	public class LinkCategories
	{
		#region Public Methods

		public List<LinkCategoryViewModel> Categories()
		{
			List<LinkCategoryViewModel> list = new List<LinkCategoryViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM cms_LinkCategory ORDER BY DisplayName", cn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read())
					{
						LinkCategoryViewModel linkViewModel = new LinkCategoryViewModel();
						linkViewModel.LinkCategorySerial = Convert.ToInt32(dr["LinkCategorySerial"]);
						linkViewModel.CategoryName = dr["CategoryName"].ToString();
						linkViewModel.DisplayName = dr["DisplayName"].ToString();
						linkViewModel.SortOrder = Convert.ToInt32(dr["SortOrder"]);
						list.Add(linkViewModel);
					}
					cmd.Connection.Close();
				}
			}

			return list;
		}

		#endregion
	}

	public class LinkCategoryViewModel
	{
		public int LinkCategorySerial { get; set; }
		public string CategoryName { get; set; }
		public string DisplayName { get; set; }
		public int SortOrder { get; set; }
	}
}