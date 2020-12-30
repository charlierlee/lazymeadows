using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
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
						_URL = dr["VirtualPath"].ToString() + "/" + _EventSerial.ToString() + "/" + tksUtil.FormatRouteURL(_Headline);

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
						_URL = dr["VirtualPath"].ToString() + "/" + _EventSerial.ToString() + "/" + tksUtil.FormatRouteURL(_Headline);

						cmd.Connection.Close();
					} else {
						_EventID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateIndex() {
			// Update Lucene search index
			Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
			li.CreateIndexWriter();
			li.UpdateWebPage(EventSerial.ToString(), URL, Headline, FullDescription ?? "", "Events");
			li.Close();
			li.IndexWords();
		}

		private void UpdateBoolProperty(string FieldName, bool Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Event] SET " + FieldName + " = @Value WHERE EventSerial = @EventSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EventSerial", SqlDbType.Int).Value = EventSerial;
					cmd.Parameters.Add("Value", SqlDbType.Bit).Value = Value ? 1 : 0;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		private void UpdateDateProperty(string FieldName, DateTime Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Event] SET " + FieldName + " = @Value WHERE EventSerial = @EventSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EventSerial", SqlDbType.Int).Value = EventSerial;
					if (Value == DateTime.MaxValue || Value == DateTime.MinValue) {
						cmd.Parameters.Add("Value", SqlDbType.DateTime).Value = SqlDateTime.Null;
					} else {
						cmd.Parameters.Add("Value", SqlDbType.DateTime).Value = Value;
					}
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

		}
		private string UpdateStringProperty(string FieldName, int Length, string Value) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Event] SET " + FieldName + " = @Value WHERE EventSerial = @EventSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("EventSerial", SqlDbType.Int).Value = EventSerial;
					if (Length == Int32.MaxValue) {
						if (Value == null) {
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
						} else {
							ret = Value.Trim();
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
						}
					} else {
						if (Value == null) {
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = SqlString.Null;
						} else {
							if (Value.Trim().Length > Length) {
								ret = Value.Trim().Substring(0, Length);
							} else {
								ret = Value.Trim();
							}
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = ret;
						}
					}
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		#endregion

		#region Properties
		public Guid ModuleID { get { return _ModuleID; } }
		public int EventSerial { get { return _EventSerial; } }
		public Guid EventID { get { return _EventID; } }
		public string Headline {
			get { return _Headline; }
			set {
				if (_Headline != value) {
					_Headline = value;
					UpdateStringProperty("Headline", Int32.MaxValue, value);
					UpdateIndex();
				}
			}
		}
		public string ShortDescription {
			get { return _ShortDescription; }
			set {
				if (_ShortDescription != value) {
					_ShortDescription = value;
					UpdateStringProperty("ShortDescription", Int32.MaxValue, value);
				}
			}
		}
		public string FullDescription {
			get { return _FullDescription; }
			set {
				if (_FullDescription != value) {
					_FullDescription = value;
					UpdateStringProperty("FullDescription", Int32.MaxValue, value);
					UpdateIndex();
				}
			}
		}
		public string PreEventDescription {
			get { return _PreEventDescription; }
			set {
				if (_PreEventDescription != value) {
					_PreEventDescription = value;
					UpdateStringProperty("PreEventDescription", Int32.MaxValue, value);
				}
			}
		}
		public DateTime EventDate {
			get { return _EventDate; }
			set {
				if (_EventDate != value) {
					_EventDate = value;
					UpdateDateProperty("EventDate", value);
				}
			}
		}
		public string EventLink {
			get { return _EventLink; }
			set {
				if (_EventLink != value) {
					_EventLink = value;
					UpdateStringProperty("EventLink", 500, value);
				}
			}
		}
		public string IconFileName {
			get { return _IconFileName; }
			set {
				if (_IconFileName != value) {
					_IconFileName = value;
					UpdateStringProperty("IconFileName", 250, value);
				}
			}
		}
		public string IconFileName2 {
			get { return _IconFileName2; }
			set {
				if (_IconFileName2 != value) {
					_IconFileName2 = value;
					UpdateStringProperty("IconFileName2", 250, value);
				}
			}
		}
		public string EventPlayback
		{
			get { return _EventPlayback; }
			set {
				if (_EventPlayback != value) {
					_EventPlayback = value;
					UpdateStringProperty("EventPlayback", Int32.MaxValue, value);
				}
			}
		}
		public string EventLocation {
			get { return _EventLocation; }
			set {
				if (_EventLocation != value) {
					_EventLocation = value;
					UpdateStringProperty("EventLocation", Int32.MaxValue, value);
				}
			}
		}
		public string EventTimespan {
			get { return _EventTimespan; }
			set {
				if (_EventTimespan != value) {
					_EventTimespan = value;
					UpdateStringProperty("EventTimespan", Int32.MaxValue, value);
				}
			}
		}
		public string URL { get { return _URL; } }
		public SEO seo { get { return new SEO(EventID); } }
        #endregion

        #region Public Methods

        public int Copy() {
            int NewEventSerial = 0;
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "INSERT INTO [cms_Event] ([ModuleID],[Headline],[ShortDescription],[FullDescription],[PreEventDescription],[EventDate],[EventLocation],[EventTimespan],[EventLink],[IconFilename],[IconFilename2],[EventPlayback]) " +
                                "OUTPUT INSERTED.EventSerial " +
                                "SELECT ModuleID, Headline, ShortDescription, FullDescription, PreEventDescription, EventDate, EventLocation, EventTimespan, EventLink, IconFilename, IconFilename2, EventPlayback " +
                                "FROM [cms_Event] WHERE EventSerial = @EventSerial";
                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@EventSerial", SqlDbType.Int).Value = EventSerial;

                    cmd.Connection.Open();
                    NewEventSerial = (int)cmd.ExecuteScalar();
                    cmd.Connection.Close();
                }
            }

            return NewEventSerial;
        }

        public void Delete()
		{
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("DELETE cms_Event WHERE EventSerial = @EventSerial", cn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@EventSerial", SqlDbType.Int).Value = _EventSerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}

		public void DeleteIcon()
		{
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString))
			{
				using (SqlCommand cmd = new SqlCommand("UPDATE [cms_Event] SET [IconFilename] = '' WHERE EventSerial = @EventSerial", cn))
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@EventSerial", SqlDbType.Int).Value = _EventSerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}

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

		public int Add(EventViewModel data)
		{
			int EventSerial = 0;
			if (_ModuleID != Guid.Empty) {
				Guid EventID = Guid.NewGuid();
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "INSERT INTO [cms_Event] ([EventID],[ModuleID],[Headline],[ShortDescription],[FullDescription],[PreEventDescription],[EventDate],[EventLocation],[EventTimespan],[EventLink],[IconFilename],[IconFilename2],[EventPlayback]) " +
									"OUTPUT INSERTED.EventSerial " +
									"VALUES (@EventID, @ModuleID, @Headline, @ShortDescription, @FullDescription, @PreEventDescription, @EventDate, @EventLocation, @EventTimespan, @EventLink, @IconFilename, @IconFilename2, @EventPlayback)";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@EventID", SqlDbType.UniqueIdentifier).Value = EventID;
						cmd.Parameters.Add("@ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("@Headline", SqlDbType.NVarChar).Value = data.Headline + "";
						cmd.Parameters.Add("@ShortDescription", SqlDbType.NVarChar).Value = data.ShortDescription + "";
						cmd.Parameters.Add("@FullDescription", SqlDbType.NVarChar).Value = data.FullDescription + "";
						cmd.Parameters.Add("@PreEventDescription", SqlDbType.NVarChar).Value = data.PreEventDescription + "";
						cmd.Parameters.Add("@EventDate", SqlDbType.Date).Value = data.EventDate + "";
						cmd.Parameters.Add("@EventLocation", SqlDbType.NVarChar).Value = data.EventLocation + "";
						cmd.Parameters.Add("@EventTimespan", SqlDbType.NVarChar).Value = data.EventTimespan + "";
						cmd.Parameters.Add("@EventLink", SqlDbType.NVarChar, 500).Value = data.EventLink + "";
						cmd.Parameters.Add("@IconFilename", SqlDbType.VarChar, 250).Value = data.IconFileName + "";
						cmd.Parameters.Add("@IconFilename2", SqlDbType.VarChar, 250).Value = data.IconFileName2 + "";
						cmd.Parameters.Add("@EventPlayback", SqlDbType.NVarChar).Value = data.EventPlayback + "";

						cmd.Connection.Open();
						EventSerial = (int)cmd.ExecuteScalar();
						cmd.Connection.Close();
					}
				}

				// Update Lucene search index
				Event newEvent = new Event(EventID);
				EventSerial = newEvent.EventSerial;
				Indexer.LuceneIndexer li = new Indexer.LuceneIndexer();
				li.CreateIndexWriter();
				li.UpdateWebPage(EventID.ToString(), newEvent.URL, newEvent.Headline, newEvent.FullDescription, "Event");
				li.Close();
				li.IndexWords();
			}
			return EventSerial;
		}

		public List<EventViewModel> Events(int Count = 0) {
			List<EventViewModel> list = new List<EventViewModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "";
				if (_ModuleID == Guid.Empty) {
					if (Count > 0) {
						SQL = "SELECT TOP " + Count.ToString() + " EventSerial FROM cms_Event ORDER BY EventDate DESC";
					} else {
						SQL = "SELECT EventSerial FROM cms_Event ORDER BY EventDate DESC";
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