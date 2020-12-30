using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class MenuItem {
		#region Fields
		private int _MenuSerial = 0;
		private int _MenuGroup = 0;
		private int _Level = 1;
		private string _Locale = "en-US";
		private int _ParentSerial = 0;
		private string _MenuText = "";
		private string _MenuLink = "";
		private string _BaseDirectory = "";
		private int _SubmenuWidth = 0;
		private int _SortOrder = 0;
		#endregion

		#region Constructor
		public MenuItem() { }
		public MenuItem(int MenuSerial) {
			_MenuSerial = MenuSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT [MenuGroup], [Locale], [ParentSerial], [Level], [MenuText], [MenuLink], [BaseDirectory], [SubmenuWidth], [SortOrder] FROM [cms_Menu] WHERE MenuSerial = @MenuSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_MenuGroup = dr.GetInt32(0);
						_Locale = dr[1].ToString();
						_ParentSerial = dr.GetInt32(2);
						_Level = dr.GetInt32(3);
						_MenuText = dr[4].ToString();
						_MenuLink = dr[5].ToString();
						_BaseDirectory = dr[6].ToString();
						_SubmenuWidth = dr.GetInt32(7);
						_SortOrder = dr.GetInt32(8);
					} else {
						_MenuSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public MenuItem(string MenuLink, string Locale = "en-US") {
			_MenuLink = MenuLink;
			_Locale = Locale;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT [MenuSerial], [MenuGroup], [ParentSerial], [Level], [MenuText], [BaseDirectory], [SubmenuWidth], [SortOrder] FROM [cms_Menu] WHERE MenuLink = @MenuLink AND Locale = @Locale", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@MenuLink", SqlDbType.VarChar, 250).Value = _MenuLink;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_MenuSerial = dr.GetInt32(0);
						_MenuGroup = dr.GetInt32(1);
						_ParentSerial = dr.GetInt32(2);
						_Level = dr.GetInt32(3);
						_MenuText = dr[4].ToString();
						_BaseDirectory = dr[5].ToString();
						_SubmenuWidth = dr.GetInt32(6);
						_SortOrder = dr.GetInt32(7);
					} else {
						_MenuLink = "";
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_Menu WHERE MenuSerial = @MenuSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public MenuItem ParentMenuItem() {
			return new MenuItem(this.ParentSerial);
		}
		public List<MenuItem> SiblingMenuItems() {
			List<MenuItem> l = new List<MenuItem>();

			if (_MenuSerial != 0) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT m2.MenuSerial FROM cms_Menu m1 JOIN cms_Menu m2 ON m1.ParentSerial = m2.ParentSerial WHERE m1.MenuSerial = @MenuSerial ORDER BY m2.SortOrder, m2.MenuText", cn)) {
						cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new MenuItem(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		public string SiblingMenuCode() {
			if (this.ParentSerial > 0) {
				StringBuilder sb = new StringBuilder();
				List<MenuItem> leftMenu = this.SiblingMenuItems();
				if (leftMenu.Count > 0) {
					sb.AppendLine("<div class='box darkgray sidemenu'>");
					sb.AppendLine(string.Format("<h2>{0}</h2>", this.ParentMenuItem().MenuText.ToUpper()));
					sb.AppendLine("<ul>");
					foreach (MenuItem menuItem in leftMenu) {
						sb.AppendLine(string.Format("<li class='{0}'><a href='{1}'>{2}</a></li>", menuItem.LinkClass, menuItem.MenuLink, menuItem.MenuText));
					}

					sb.AppendLine("</ul>");
					sb.AppendLine("</div>");
				}

				return sb.ToString();
			} else {
				return "";
			}
		}
		public List<MenuItem> SubMenuItems() {
			List<MenuItem> l = new List<MenuItem>();

			if (_MenuSerial != 0) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT MenuSerial FROM cms_Menu WHERE ParentSerial = @ParentSerial ORDER BY SortOrder, MenuText", cn)) {
						cmd.Parameters.Add("ParentSerial", SqlDbType.Int).Value = _MenuSerial;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new MenuItem(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		#endregion

		#region Properties
		public string BaseDirectory {
			get { return _BaseDirectory; }
			set {
				if (_BaseDirectory != value) {
					_BaseDirectory = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_MenuPhoto SET BaseDirectory = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.VarChar, 250).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public int Level {
			get { return _Level; }
			set {
				if (_Level != value) {
					_Level = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET Level = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public string LinkClass {
			get {
				string ret = this.MenuLink;
				if (ret.StartsWith("/")) { ret = ret.Substring(1); }
				if (ret.EndsWith("/")) { ret = ret.Substring(0, ret.Length - 1); }
				ret = ret.Replace("/", "_");
				return ret;
			}
		}
		public string Locale { get { return _Locale; } }
		public int MenuGroup {
			get { return _MenuGroup; }
			set {
				if (_MenuGroup != value) {
					_MenuGroup = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET MenuGroup = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public string MenuLink {
			get { return _MenuLink; }
			set {
				if (_MenuLink != value) {
					_MenuLink = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET MenuLink = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.VarChar, 250).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public int MenuSerial { get { return _MenuSerial; } }
		public string MenuText {
			get { return _MenuText; }
			set {
				if (_MenuText != value) {
					_MenuText = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET MenuText = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.VarChar, 250).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public int ParentSerial {
			get { return _ParentSerial; }
			set {
				if (_ParentSerial != value) {
					_ParentSerial = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET ParentSerial = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET SortOrder = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public int SubmenuWidth {
			get { return _SubmenuWidth; }
			set {
				if (_SubmenuWidth != value) {
					_SubmenuWidth = value;
					if (_MenuSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_Menu SET SubmenuWidth = @Value WHERE MenuSerial = @MenuSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("MenuSerial", SqlDbType.Int).Value = _MenuSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		#endregion
	}
	public class Menu {
		#region Fields
		private int _MenuGroup = 0;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public Menu(int MenuGroup, string Locale = "en-US") {
			_MenuGroup = MenuGroup;
			_Locale = Locale;
		}
		#endregion

		#region Public Methods
		public int Add(MenuItem menuItem) {
			int ret = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (menuItem.ParentSerial > 0) {
					SQL = "INSERT INTO cms_Menu ([MenuGroup], [Locale], [ParentSerial], [Level], [MenuText], [MenuLink], " +
								"[BaseDirectory], [SubmenuWidth], [SortOrder]) " +
								"OUTPUT INSERTED.MenuSerial " +
								"SELECT @MenuGroup, @Locale, @ParentSerial, ISNULL(Level, 0) + 1, @MenuText, @MenuLink, " +
								"@BaseDirectory, @SubmenuWidth, @SortOrder " +
								"FROM cms_Menu " +
								"WHERE MenuSerial = @ParentSerial ";
				} else {
					SQL = "INSERT INTO cms_Menu ([MenuGroup], [Locale], [ParentSerial], [Level], [MenuText], [MenuLink], " +
								"[BaseDirectory], [SubmenuWidth], [SortOrder]) " +
								"OUTPUT INSERTED.MenuSerial " +
								"VALUES (@MenuGroup, @Locale, @ParentSerial, 1, @MenuText, @MenuLink, " +
								"@BaseDirectory, @SubmenuWidth, @SortOrder)";
				}
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("MenuGroup", SqlDbType.Int).Value = _MenuGroup;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("ParentSerial", SqlDbType.Int).Value = menuItem.ParentSerial;
					cmd.Parameters.Add("MenuText", SqlDbType.VarChar, 250).Value = menuItem.MenuText;
					cmd.Parameters.Add("MenuLink", SqlDbType.VarChar, 250).Value = menuItem.MenuLink;
					cmd.Parameters.Add("BaseDirectory", SqlDbType.VarChar, 250).Value = menuItem.BaseDirectory;
					cmd.Parameters.Add("SubmenuWidth", SqlDbType.Int).Value = menuItem.SubmenuWidth;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = menuItem.SortOrder;


					cmd.Connection.Open();
					ret = Convert.ToInt32(cmd.ExecuteScalar());
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		public List<MenuItem> MenuItems() {
			List<MenuItem> l = new List<MenuItem>();

			if (_MenuGroup != 0) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT MenuSerial FROM cms_Menu WHERE MenuGroup = @MenuGroup AND Locale = @Locale AND ParentSerial = 0 AND Level = 1 ORDER BY SortOrder, MenuText", cn)) {
						cmd.Parameters.Add("MenuGroup", SqlDbType.Int).Value = _MenuGroup;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;

						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new MenuItem(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		public string MenuCode() {
			StringBuilder sb = new StringBuilder();
			List<MenuItem> topMenu = this.MenuItems();
			if (topMenu.Count > 0) {
				sb.AppendLine("<ul class='megamenu'>");
				foreach(MenuItem menuItem in topMenu) {
					sb.AppendLine(string.Format("<li class='{0}'><a href='{1}'>{2}</a>", menuItem.BaseDirectory, menuItem.MenuLink, menuItem.MenuText));
					List<MenuItem> subMenuItems = menuItem.SubMenuItems();
					if (subMenuItems.Count > 0){
						sb.AppendLine(string.Format("<div style='width:{0}px;'>", menuItem.SubmenuWidth.ToString()));
						sb.AppendLine("<ul>");
						foreach(MenuItem subMenuItem in subMenuItems) {
							sb.AppendLine(string.Format("<li><a href='{0}'>{1}</a></li>", subMenuItem.MenuLink, subMenuItem.MenuText));
						}
						sb.AppendLine("</ul>");
						sb.AppendLine("</div>");
					}
					sb.AppendLine("</li>");
				}
											
				sb.AppendLine("</ul>");
			}

			return sb.ToString();
		}
		#endregion

		#region Properties
		public int MenuGroup { get { return _MenuGroup; } }
		public string Locale { get { return _Locale; } }
		#endregion
	}
}