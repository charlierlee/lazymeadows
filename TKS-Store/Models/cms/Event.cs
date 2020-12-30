using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace TKS.Models.CMS {
	public class Event
	{
		#region Fields
		private int _EventSerial = 0;
		private Guid _EventID = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Headline = "";
		private string _ShortDescription = "";
		private string _FullDescription = "";
		private string _PreEventDescription = "";
		private DateTime _EventDate;
		private string _EventLink = "";
		private string _IconFileName = "";
		private string _IconFileName2 = "";
		private string _EventPlayback = "";
		private string _EventLocation = "";
		private string _EventTimespan = "";
		private string _URL = "";
		#endregion

		#region Constructor
		public Event(int EventSerial) {
			_EventSerial = EventSerial;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT e.*, p.VirtualPath " +
							"	FROM [cms_Event] e JOIN cms_Module m ON e.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID " +
							"	WHERE e.EventSerial = @EventSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EventSerial", SqlDbType.Int).Value = _EventSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = new Guid(dr["ModuleID"].ToString());
						_EventID = new Guid(dr["EventID"].ToString());
						_EventDate = Convert.ToDateTime(dr["EventDate"]);
						_EventLink = dr["EventLink"].ToString();
						_EventLocation = dr["EventLocation"].ToString();
						_EventPlayback = dr["EventPlayback"].ToString();
						_EventTimespan = dr["EventTimespan"].ToString();
						_FullDescription = dr["FullDescription"].ToString();
						_Headline = dr["Headline"].ToString();
						_IconFileName = dr["IconFilename"].ToString();
						_IconFileName2 = dr["IconFilename2"].ToString();
						_PreEventDescription = dr["PreEventDescription"].ToString();
						_ShortDescription = dr["ShortDescription"].ToString();
						_URL = dr["VirtualPath"].ToString() + "/" + _EventSerial.ToString() + "/" + TKS.Areas.Admin.Models.tksUtil.FormatRouteURL(_Headline);

						cmd.Connection.Close();
					} else {
						_EventSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public Event(Guid EventID) {
			_EventID = EventID;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT e.*, p.VirtualPath " +
							"	FROM [cms_Event] e JOIN cms_Module m ON e.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID " +
							"	WHERE e.EventID = @EventID";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EventID", SqlDbType.UniqueIdentifier).Value = _EventID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = new Guid(dr["ModuleID"].ToString());
						_EventSerial = Convert.ToInt32(dr["EventSerial"].ToString());
						_EventDate = Convert.ToDateTime(dr["EventDate"]);
						_EventLink = dr["EventLink"].ToString();
						_EventLocation = dr["EventLocation"].ToString();
						_EventPlayback = dr["EventPlayback"].ToString();
						_EventTimespan = dr["EventTimespan"].ToString();
						_FullDescription = dr["FullDescription"].ToString();
						_Headline = dr["Headline"].ToString();
						_IconFileName = dr["IconFilename"].ToString();
						_IconFileName2 = dr["IconFilename2"].ToString();
						_PreEventDescription = dr["PreEventDescription"].ToString();
						_ShortDescription = dr["ShortDescription"].ToString();
						_URL = dr["VirtualPath"].ToString() + "/" + _EventSerial.ToString() + "/" + TKS.Areas.Admin.Models.tksUtil.FormatRouteURL(_Headline);

						cmd.Connection.Close();
					} else {
						_EventID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid ModuleID { get { return _ModuleID; } }
		public int EventSerial { get { return _EventSerial; } }
		public Guid EventID { get { return _EventID; } }
		public string Headline { get { return _Headline; } }
		public string ShortDescription { get { return _ShortDescription; } }
		public string FullDescription { get { return _FullDescription; } }
		public string PreEventDescription { get { return _PreEventDescription; } }
		public DateTime EventDate { get { return _EventDate; } }
		public string EventLink { get { return _EventLink; } }
		public string IconFileName { get { return _IconFileName; } }
		public string IconFileName2 { get { return _IconFileName2; } }
		public string EventPlayback { get { return _EventPlayback; } }
		public string EventLocation { get { return _EventLocation; } }
		public string EventTimespan { get { return _EventTimespan; } }
		public string URL { get { return _URL; } }
		public SEO seo { get { return new SEO(EventID); } }
		public string MetaTags { get { return new SEO(EventID).GetTags(URL, Headline, ShortDescription); } }
		#endregion
	}

	public class EventSet
	{
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		#endregion

		#region Constructor
		public EventSet() { }
		public EventSet(Guid ModuleID) {
			_ModuleID = ModuleID;
		}
		#endregion

		#region Public Methods
		public List<EventViewModel> Events(int Count = 0) {
			List<EventViewModel> list = new List<EventViewModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (_ModuleID == Guid.Empty) {
					if (Count > 0) {
						SQL = "SELECT TOP " + Count.ToString() + " EventSerial FROM cms_Event WHERE EventDate >= GETDATE() ORDER BY EventDate ASC";
					} else {
						SQL = "SELECT EventSerial FROM cms_Event WHERE EventDate >= GETDATE() ORDER BY EventDate ASC";
					}
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							list.Add(new EventViewModel(new Event(dr.GetInt32(0))));
						}
						cmd.Connection.Close();
					}
				} else {
					if (Count > 0) {
						SQL = "SELECT TOP " + Count.ToString() + " EventSerial FROM cms_Event WHERE ModuleID = @ModuleID ORDER BY EventDate DESC";
					} else {
						SQL = "SELECT EventSerial FROM cms_Event WHERE ModuleID = @ModuleID ORDER BY EventDate DESC";
					}
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							list.Add(new EventViewModel(new Event(dr.GetInt32(0))));
						}
						cmd.Connection.Close();
					}
				}
			}
			return list;
		}
		#endregion
	}

	public class EventViewModel
	{
		#region Constructor
		public EventViewModel() { }
		public EventViewModel(Event data) {
			this.EventID = data.EventID;
			this.EventDate = data.EventDate;
			this.EventLink = data.EventLink;
			this.EventLocation = data.EventLocation;
			this.EventPlayback = data.EventPlayback;
			this.EventSerial = data.EventSerial;
			this.EventTimespan = data.EventTimespan;
			this.FullDescription = data.FullDescription;
			this.Headline = data.Headline;
			this.IconFileName = data.IconFileName;
			this.IconFileName2 = data.IconFileName2;
			this.ModuleID = data.ModuleID;
			this.PreEventDescription = data.PreEventDescription;
			this.ShortDescription = data.ShortDescription;
			this.URL = data.URL;
		}
		#endregion

		#region Properties
		public int EventSerial { get; set; }
		public Guid EventID { get; set; }
		public Guid ModuleID { get; set; }
		public string Headline { get; set; }
		public string ShortDescription { get; set; }
		public string FullDescription { get; set; }
		public string PreEventDescription { get; set; }
		public DateTime EventDate { get; set; }
		public string EventLink { get; set; }
		public string IconFileName { get; set; }
		public string IconFileName2 { get; set; }
		public string EventPlayback { get; set; }
		public string EventLocation { get; set; }
		public string EventTimespan { get; set; }
		public string URL { get; set; }
		#endregion
	}
}