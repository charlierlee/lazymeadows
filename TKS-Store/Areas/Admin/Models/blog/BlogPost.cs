using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Indexer;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using TKS.Areas.Admin.Models.CMS;

namespace TKS.Areas.Admin.Models.Blog {
	public class BlogPost {
		#region Fields
		private int _PostSerial = 0;
		private Guid _PostID = System.Guid.Empty;
		private string _Title = "";
		private string _Author = "";
		private string _Description = "";
		private string _PostContent = "";
		private Guid _AuthorID = Guid.Empty;
		private int _AuthorSerial = 0;
		private string _AuthorTwitterHandle = "";
		private string _AuthorGooglePlusURL = "";
		private bool _IsPublished = false;
		private bool _IsCommentEnabled = false;
		private string _Slug = "";
		private string _IconFilename = "";
		private string _ThumbnailFilename = "";
		private string _HeroFilename = "";
		private string _AudioFilename = "";
		private string _VideoPath = "";
		private DateTime _PublishDate = DateTime.MinValue;
		private string _Tags = "";
		private List<Guid> _AssignedCategories = new List<Guid>();
		private Readability readability;
		#endregion

		#region Constructor
		public BlogPost() { }
		public BlogPost(int PostSerial) {
			_PostSerial = PostSerial;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT bp.*, a.FirstName + ' ' + a.LastName AS Author, a.AccountID, a.TwitterHandle, a.GooglePlusURL FROM cms_BlogPost bp LEFT JOIN Account a ON bp.AuthorID = a.UserId WHERE PostSerial = @PostSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostSerial", SqlDbType.Int).Value = _PostSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_PostID = new Guid(dr["PostID"].ToString());
						_Title = dr["Title"].ToString();
						_Author = dr["Author"].ToString();
						_AuthorTwitterHandle = dr["TwitterHandle"].ToString();
						_AuthorGooglePlusURL = dr["GooglePlusURL"].ToString();
						_Description = dr["Description"].ToString();
						_PostContent = dr["PostContent"].ToString();
						//readability = new Readability(_PostContent);
						Int32.TryParse(dr["AccountID"].ToString(), out _AuthorSerial);
						Guid.TryParse(dr["AuthorID"].ToString(), out _AuthorID);
						_IsPublished = dr["IsPublished"].ToString().ToLower() == "true" ? true : false;
						_IsCommentEnabled = dr["IsCommentEnabled"].ToString().ToLower() == "true" ? true : false;
						_Slug = dr["Slug"].ToString();
						_IconFilename = dr["IconFilename"].ToString();
						_ThumbnailFilename = dr["ThumbnailFilename"].ToString();
						_HeroFilename = dr["HeroFilename"].ToString();
						_AudioFilename = dr["AudioFilename"].ToString();
						_VideoPath = dr["VideoPath"].ToString();
						_PublishDate = dr.IsDBNull(dr.GetOrdinal("PublishDate")) ? DateTime.MinValue : dr.GetDateTime(dr.GetOrdinal("PublishDate"));
					} else {
						_PostSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public BlogPost(Guid PostID) {
			_PostID = PostID;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT bp.*, a.FirstName + ' ' + a.LastName AS Author, a.AccountID FROM cms_BlogPost bp LEFT JOIN Account a ON bp.AuthorID = a.UserId WHERE PostID = @PostID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_PostSerial = Convert.ToInt32(dr["PostSerial"]);
						_Title = dr["Title"].ToString();
						_Author = dr["Author"].ToString();
						_Description = dr["Description"].ToString();
						_PostContent = dr["PostContent"].ToString();
						readability = new Readability(_PostContent);
						_AuthorSerial = Convert.ToInt32(dr["AccountID"].ToString());
						_AuthorID = new Guid(dr["AuthorID"].ToString());
						_IsPublished = dr["IsPublished"].ToString().ToLower() == "true" ? true : false;
						_IsCommentEnabled = dr["IsCommentEnabled"].ToString().ToLower() == "true" ? true : false;
						_Slug = dr["Slug"].ToString();
						_IconFilename = dr["IconFilename"].ToString();
						_ThumbnailFilename = dr["ThumbnailFilename"].ToString();
						_HeroFilename = dr["HeroFilename"].ToString();
						_AudioFilename = dr["AudioFilename"].ToString();
						_VideoPath = dr["VideoPath"].ToString();
						DateTime.TryParse(dr["PublishDate"].ToString(), out _PublishDate);
					} else {
						_PostID = System.Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private string GetComments() {
			List<BlogComment> CommentList = this.Comments;
			StringBuilder sb = new StringBuilder();
			foreach (BlogComment comment in CommentList) {
				sb.AppendLine("<div>");
				sb.AppendLine("	<p>" + comment.Author + " " + comment.CommentDate.ToShortDateString() + " @ " + comment.CommentDate.ToShortTimeString() + "</p>");
				sb.AppendLine("	<div class='ym-grid'>");
				sb.AppendLine("		<div class='ym-g75 ym-gl'>");
				sb.AppendLine("			<div class='ym-fbox-text'>");
				sb.AppendLine("				<textarea name='comment' rows='4'>" + comment.Comment + "</textarea>");
				sb.AppendLine("			</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("		<div class='ym-g25 ym-gr'>");
				sb.AppendLine("			<div class='ym-fbox' style='text-align:right'>");
				sb.AppendLine("				<div style='margin-bottom:1.5em'><button class='cmdUpdate' data-commentid='" + comment.PostCommentID.ToString() + "'>Update</button></div>");
				sb.AppendLine("				<button class='cmdDelete ym-secondary' data-commentid='" + comment.PostCommentID.ToString() + "'>Delete</button>");
				sb.AppendLine("			</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("	</div>");
				if (!comment.IsApproved) {
					sb.AppendLine("	<div class='ym-fbox'>");
					sb.AppendLine("		<button class='cmdApprove ym-primary' data-commentID='" + comment.PostCommentID.ToString() + "'>Approve</button>");
					sb.AppendLine("	</div>");
				}
				sb.AppendLine("	<hr />");
				sb.AppendLine("</div>");
				sb.Append(GetChildComments(comment, 1));
			}
			return sb.ToString();
		}
		private string GetChildComments(BlogComment comment, int level) {
			List<BlogComment> CommentList = comment.Comments;
			StringBuilder sb = new StringBuilder();
			foreach (BlogComment comm in CommentList) {
				string pendingClass = !comm.IsApproved ? " blogCommentPending" : "";
				sb.AppendLine("<div style='padding-left:" + Convert.ToString(15 * level) + "px'>");
				sb.AppendLine("	<p>" + comm.Author + " " + comm.CommentDate.ToShortDateString() + " @ " + comm.CommentDate.ToShortTimeString() + "</p>");
				sb.AppendLine("	<div class='ym-grid'>");
				sb.AppendLine("		<div class='ym-g75 ym-gl'>");
				sb.AppendLine("			<div class='ym-fbox-text'>");
				sb.AppendLine("				<textarea name='comment' rows='4'>" + comm.Comment + "</textarea>");
				sb.AppendLine("			</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("		<div class='ym-g25 ym-gr'>");
				sb.AppendLine("			<div class='ym-fbox' style='text-align:right'>");
				sb.AppendLine("				<div style='margin-bottom:1.5em'><button class='cmdUpdate' data-commentid='" + comment.PostCommentID.ToString() + "'>Update</button></div>");
				sb.AppendLine("				<button class='cmdDelete ym-secondary' data-commentid='" + comment.PostCommentID.ToString() + "'>Delete</button>");
				sb.AppendLine("			</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("	</div>");
				if (!comment.IsApproved) {
					sb.AppendLine("	<div class='ym-fbox'>");
					sb.AppendLine("		<button class='cmdApprove ym-primary' data-commentID='" + comment.PostCommentID.ToString() + "'>Approve</button>");
					sb.AppendLine("	</div>");
				}
				sb.AppendLine("	<hr />");
				sb.AppendLine("</div>");
				sb.Append(GetChildComments(comm, level + 1));
			}
			return sb.ToString();
		}

		private void UpdateIndex() {
			// Update Lucene search index
			LuceneIndexer li = new LuceneIndexer();
			li.CreateIndexWriter();
			li.UpdateWebPage(PostID.ToString(), URL, Title, PostContent, "Blog");
			li.Close();
			li.IndexWords();
		}

		private void UpdateBoolProperty(string FieldName, bool Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPost SET " + FieldName + " = @Value, UpdateDate = GETDATE() WHERE PostID = @PostID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
					cmd.Parameters.Add("Value", SqlDbType.Bit).Value = Value ? 1 : 0;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

		}
		private void UpdateDateProperty(string FieldName, DateTime Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPost SET " + FieldName + " = @Value, UpdateDate = GETDATE() WHERE PostID = @PostID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
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
		private void UpdateGuidProperty(string FieldName, Guid Value) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPost SET " + FieldName + " = @Value, UpdateDate = GETDATE() WHERE PostID = @PostID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
					if (Value == Guid.Empty) {
						cmd.Parameters.Add("Value", SqlDbType.UniqueIdentifier).Value = SqlGuid.Null;
					} else {
						cmd.Parameters.Add("Value", SqlDbType.UniqueIdentifier).Value = Value;
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
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPost SET " + FieldName + " = @Value, UpdateDate = GETDATE() WHERE PostID = @PostID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
					if (Length == Int32.MaxValue) {
						if (Value == null) {
							ret = null;
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
						}
					} else {
						if (Value == null) {
							ret = null;
							cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = SqlString.Null;
						} else {
							ret = Convert.ToString(Value).Trim();
							if (ret.Length > Length) {
								ret = ret.Substring(0, Length);
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

		#region Public Methods
		public void AddCategory(Guid CategoryID) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_BlogPost_Category_xref (PostID, CategoryID) VALUES (@PostID, @CategoryID)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
					cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier).Value = CategoryID;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("cms_DeleteBlog", cn)) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@PostID", SqlDbType.UniqueIdentifier).Value = _PostID;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			// Remove from Lucene search index
			LuceneIndexer li = new LuceneIndexer();
			li.CreateIndexWriter();
			li.Delete(_PostID.ToString());
			li.Close();
		}
		public static List<BlogAuthor> GetBlogAuthors() {
			List<BlogAuthor> l = new List<BlogAuthor>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT c.UserId, c.FirstName + ' ' + c.LastName AS Author FROM aspnet_Roles r JOIN aspnet_UsersInRoles uir ON r.RoleId = uir.RoleId JOIN Account c ON uir.UserId = c.UserId WHERE r.RoleName = 'Blog Author' ORDER BY c.LastName, c.FirstName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogAuthor() { UserID = dr[0].ToString(), AuthorName = dr[1].ToString() });
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public static string GetAuthorName(int AccountID) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT FirstName, LastName FROM [Account] WHERE AccountID = @AccountID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("AccountID", SqlDbType.Int).Value = AccountID;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						ret = dr[0].ToString() + " " + dr[1].ToString();
					}
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		public static string GetCategory(int CategorySerial) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT CategoryName FROM [cms_BlogCategory] WHERE CategorySerial = @CategorySerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("CategorySerial", SqlDbType.Int).Value = CategorySerial;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						ret = dr[0].ToString();
					}
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		public string GetFormattedComments() {
			return GetComments();
		}
		public void Save(BlogPostViewModel postViewModel) {

			//private List<Guid> _AssignedCategories = new List<Guid>();
			AudioFilename = postViewModel.AudioFilename;
			AuthorID = postViewModel.AuthorID;
			Description = postViewModel.Description;
			HeroFilename = postViewModel.HeroFilename;
			IconFilename = postViewModel.IconFilename;
			IsCommentEnabled = postViewModel.IsCommentEnabled;
			IsPublished = postViewModel.IsPublished;
			PostContent = postViewModel.PostContent;
			PublishDate = postViewModel.PublishDate;
			Slug = postViewModel.Slug;
			Tags = postViewModel.Tags;
			ThumbnailFilename = postViewModel.ThumbnailFilename;
			Title = postViewModel.Title;
			VideoPath = postViewModel.VideoPath;

			if (postViewModel.Categories != null) {
				List<Guid> cats = new List<Guid>();
				foreach (string cat in postViewModel.Categories) {
					cats.Add(new Guid(cat));
				}
				this.AssignedCategories = cats;
			}
		}
		#endregion

		#region Properties
		public int PostSerial { get { return _PostSerial; } }
		public Guid PostID { get { return _PostID; } }
		public List<Guid> AssignedCategories {
			get {
				if (_AssignedCategories.Count == 0) {
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "SELECT CategoryID FROM cms_BlogPost_Category_xref WHERE PostID = @PostID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;

							cmd.Connection.Open();
							SqlDataReader dr = cmd.ExecuteReader();
							while (dr.Read()) {
								_AssignedCategories.Add(dr.GetGuid(0));
							}
							cmd.Connection.Close();
						}
					}
				}
				return _AssignedCategories; 
			}
			set { 
				_AssignedCategories = value;
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "DELETE FROM cms_BlogPost_Category_xref WHERE PostID = @PostID";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();

						cmd.CommandText = "INSERT INTO cms_BlogPost_Category_xref (PostID, CategoryID) VALUES (@PostID, @CategoryID)";
						cmd.Parameters.Add("CategoryID", SqlDbType.UniqueIdentifier);
						foreach (Guid cat in _AssignedCategories) {
							cmd.Parameters["CategoryID"].Value = cat;
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string AudioFilename {
			get { return _AudioFilename; }
			set {
				if (!_AudioFilename.Equals(value)) {
					_AudioFilename = UpdateStringProperty("AudioFilename", 250, value);
				}
			}
		}
		public string Author {
			get { return _Author; }
		}
		public Guid AuthorID {
			get { return _AuthorID; }
			set {
				if (!_AuthorID.Equals(value)) {
					_AuthorID = value;
					UpdateGuidProperty("AuthorID", value);
				}
			}
		}
		public int CommentCount {
			get {
				int ret = 0;
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM cms_BlogPostComment WHERE PostID = @PostID AND IsDeleted = 0 AND IsApproved = 1", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
						cmd.Connection.Open();
						ret = (int)cmd.ExecuteScalar();
						cmd.Connection.Close();
					}
				}
				return ret;
			}
		}
		public int CommentToApproveCount {
			get {
				int ret = 0;
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM cms_BlogPostComment WHERE PostID = @PostID AND IsDeleted = 0 AND IsApproved = 0", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
						cmd.Connection.Open();
						ret = (int)cmd.ExecuteScalar();
						cmd.Connection.Close();
					}
				}
				return ret;
			}
		}
		public List<BlogComment> Comments {
			get {
				List<BlogComment> l = new List<BlogComment>();

				string SQL = "";
				if (HttpContext.Current.User.IsInRole("Blog Admin")) {
					//Show unapproved comments if Blog Admin
					SQL = "SELECT PostCommentID FROM cms_BlogPostComment WHERE ParentCommentID IS NULL AND IsDeleted = 0 AND PostID = @PostID ORDER BY CommentDate";
				} else {
					SQL = "SELECT PostCommentID FROM cms_BlogPostComment WHERE ParentCommentID IS NULL AND IsApproved = 1 AND IsDeleted = 0 AND PostID = @PostID ORDER BY CommentDate";
				}
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new BlogComment(dr.GetGuid(0)));
						}
						cmd.Connection.Close();
					}
				}
				return l;
			}
		}
		public bool CommentsSubscribed {
			get {
				bool ret = false;

				if (HttpContext.Current.User.Identity.IsAuthenticated) {
					MembershipUser u = Membership.GetUser();
					if (!string.IsNullOrEmpty(u.Email)) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							string SQL = "SELECT COUNT(*) FROM [cms_BlogPostNotify] " +
										"	WHERE [PostID] = @PostID " +
										"	AND [NotifyAddress] = @Email";
							using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
								cmd.Parameters.Add("Email", SqlDbType.NVarChar, 255).Value = u.Email;

								cmd.Connection.Open();
								int count = (int)cmd.ExecuteScalar();
								cmd.Connection.Close();

								ret = count > 0;
							}
						}
					}
				}


				return ret;
			}
		}
		public string Description {
			get { return _Description; } 
			set {
				if (!_Description.Equals(value)) {
					_Description = UpdateStringProperty("Description", Int32.MaxValue, value);
				}
			}
		}
		public string HeroFilename {
			get { return _HeroFilename; }
			set {
				if (!_HeroFilename.Equals(value)) {
					_HeroFilename = UpdateStringProperty("HeroFilename", 250, value);
				}
			}
		}
		public string HeroImage {
			get {
				if (!string.IsNullOrEmpty(_HeroFilename)) {
					return string.Format("<img src='{0}' alt=\"{1}\" />", _HeroFilename, "");
				} else {
					return "";
				}
			}
		}
		public string HeroObject {
			get {
				if (!string.IsNullOrEmpty(_HeroFilename)) {
					return HeroImage;
				} else if (!string.IsNullOrEmpty(_VideoPath)) {
					int hasEqual = _VideoPath.IndexOf("=");
					if (hasEqual > 0) {
						return string.Format("<p><iframe width='688' height='460' src='//www.youtube-nocookie.com/embed/{0}?rel=0'></iframe></p>", _VideoPath.Substring(hasEqual + 1));
					} else {
						return "";
					}
				} else {
					return "";
				}
			}
		}
		public string IconFilename {
			get { return _IconFilename; }
			set {
				if (!_IconFilename.Equals(value)) {
					_IconFilename = UpdateStringProperty("IconFilename", 250, value);
				}
			}
		}
		public bool IsCommentEnabled {
			get { return _IsCommentEnabled; }
			set {
				if (!_IsCommentEnabled.Equals(value)) {
					_IsCommentEnabled = value;
					UpdateBoolProperty("IsCommentEnabled", value);
				}
			}
		}
		public bool IsPublished {
			get { return _IsPublished; }
			set {
				if (!_IsPublished.Equals(value)) {
					_IsPublished = value;
					UpdateBoolProperty("IsPublished", value);
				}
			}
		}
		public string Permalink { get { return "/blog/post.aspx?guid=" + _PostID.ToString(); } }
		public string PostContent {
			get { return _PostContent; }
			set {
				if (!_PostContent.Equals(value)) {
					_PostContent = UpdateStringProperty("PostContent", Int32.MaxValue, value);
				}
			}
		}
		public string PostContentStripped {
			get { return readability.StrippedContents; }
		}
		public DateTime PublishDate {
			get { return _PublishDate; }
			set {
				if (!_PublishDate.Equals(value)) {
					_PublishDate = value;
					UpdateDateProperty("PublishDate", value);
				}
			}
		}
		public int ReadabilityCharacters { get { return readability.Characters; } }
		public int ReadabilityCharactersNoSpaces { get { return readability.CharactersNoSpaces; } }
		public int ReadabilitySentences { get { return readability.Sentences; } }
		public double ReadabilityGunningFogScore { get { return readability.GunningFogScore; } }
		public double ReadabilityFleschKincaidGradeLevel { get { return readability.FleschKincaidGradeLevel; } }
		public double ReadabilityColemanLiauIndex { get { return readability.ColemanLiauIndex; } }
		public double ReadabilitySMOGIndex { get { return readability.SMOGIndex; } }
		public double ReadabilityAutomatedReadabilityIndex { get { return readability.AutomatedReadabilityIndex; } }
		public double ReadabilityAverageGradeLevel { get { return readability.AverageGradeLevel; } }
		public string ReadabilitySentenceList {
			get {
				StringBuilder sb = new StringBuilder();
				ArrayList al = readability.SentenceList;
				for (int i = 0; i < al.Count; i++) {
					//populate a list box
					sb.AppendLine(al[i].ToString() + "<br />");
				}
				return sb.ToString();
			}
		}
		public int ReadabilitySyllables { get{ return readability.Syllables; } }
		public int ReadabilityWords { get { return readability.Words; } }
		public double ReadabilitySyllablesPerWord { get { return readability.SyllablesPerWord; } }
		public double ReadabilityWordsPerSentence { get { return readability.WordsPerSentence; } }
		public SEO seo { get { return new SEO(PostID); } }
		public string Slug {
			get { return (_Slug.Length > 0) ? _Slug : tksUtil.FormatRouteURL(_Title); }
			set {
				if (value != null) {
					string checkSlug = tksUtil.FormatRouteURL(value.Trim());
					checkSlug = checkSlug.Length > 255 ? checkSlug.Substring(0, 255) : checkSlug;
					if (!_Slug.Equals(checkSlug)) {
						_Slug = UpdateStringProperty("Slug", 255, checkSlug);
					}
				} else {
					_Slug = UpdateStringProperty("Slug", 255, null);
				}
			}
		}
		public string Tags {
			get {
				if (String.IsNullOrEmpty(_Tags)) {
					_Tags = "";

					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "SELECT Tag FROM cms_BlogPostTag WHERE PostID = @PostID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;

							cmd.Connection.Open();
							SqlDataReader dr = cmd.ExecuteReader();
							while (dr.Read()) {
								_Tags += dr[0].ToString() + ", ";
							}
							cmd.Connection.Close();
							if (_Tags.Length > 2) {
								_Tags = _Tags.Substring(0, _Tags.Length - 2);
							}
						}
					}
				}
				return _Tags; 
			}
			set {
				if (_Tags != value) {
					_Tags = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "DELETE FROM cms_BlogPostTag WHERE PostID = @PostID";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();

							if (!string.IsNullOrEmpty(_Tags)) {
								char[] delimiterChars = { ',' };
								string[] tags = Tags.Split(delimiterChars);
								cmd.CommandText = "INSERT INTO cms_BlogPostTag (PostID, Tag) VALUES (@PostID, @Tag)";
								cmd.Parameters.Add("Tag", SqlDbType.VarChar, 50);
								foreach (string tag in tags) {
									cmd.Parameters["Tag"].Value = tag.Trim().Length > 50 ? tag.Trim().Substring(0, 50) : tag.Trim();
									cmd.Connection.Open();
									cmd.ExecuteNonQuery();
									cmd.Connection.Close();
								}
							}
						}
					}
				}
			}
		}
		public string TagsURL {
			get {
				string ret = "";
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT Tag FROM cms_BlogPostTag WHERE PostID = @PostID ORDER BY Tag", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							ret += "<a href='/blog/tag/" + tksUtil.FormatRouteURL(dr[0].ToString()) + "'>" + dr[0].ToString() + "</a>, ";
						}
						cmd.Connection.Close();
					}
				}
				if (ret.Length > 2) { ret = ret.Substring(0, ret.Length - 2); }
				return ret;
			}
		}
		public string ThumbnailFilename {
			get { return _ThumbnailFilename; }
			set {
				if (!_ThumbnailFilename.Equals(value)) {
					_ThumbnailFilename = UpdateStringProperty("ThumbnailFilename", 250, value);
				}
			}
		}
		public string Title {
			get { return _Title; }
			set {
				if (!_Title.Equals(value)) {
					_Title = UpdateStringProperty("Title", 255, value);
					UpdateIndex();
				}
			}
		}
		public string URL { get { return Global.BlogDirectory + "/" + _PostSerial.ToString() + "/" + _Slug; } }
		public string VideoPath {
			get { return _VideoPath; }
			set {
				if (!_VideoPath.Equals(value)) {
					_VideoPath = UpdateStringProperty("VideoPath", 250, value);
				}
			}
		}
		public string Video {
			get {
				if (!string.IsNullOrEmpty(_VideoPath) && !string.IsNullOrEmpty(_HeroFilename)) {
					int hasEqual = _VideoPath.IndexOf("=");
					if (hasEqual > 0) {
						return string.Format("<p><iframe width='560' height='315' src='//www.youtube-nocookie.com/embed/{0}?rel=0'></iframe></p>", _VideoPath.Substring(hasEqual + 1));
					} else {
						return "";
					}
				} else {
					return "";
				}
			}
		}
		#endregion
	}
	public class BlogComment {
		#region Fields
		private Guid _PostCommentID = Guid.Empty;
		private Guid _PostID = Guid.Empty;
		private Guid _ParentCommentID = Guid.Empty;
		private DateTime _CommentDate = DateTime.MinValue;
		private string _Author = "";
		private string _Email = "";
		private string _Website = "";
		private string _Comment = "";
		private string _Country = "";
		private string _IP = "";
		private bool _IsApproved = false;
		private Guid _ModeratedBy = Guid.Empty;
		private string _Avatar = "";
		private bool _IsSpam = false;
		private bool _IsDeleted = false;
		#endregion

		#region Constructor
		public BlogComment() { }
		public BlogComment(Guid PostCommentID) {
			_PostCommentID = PostCommentID;

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT PostID, ParentCommentID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved, ModeratedBy, Avatar, IsSpam, IsDeleted FROM cms_BlogPostComment WHERE PostCommentID = @PostCommentID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostCommentID", SqlDbType.UniqueIdentifier).Value = _PostCommentID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_PostID = dr.IsDBNull(0) ? Guid.Empty : dr.GetGuid(0);
						_ParentCommentID = dr.IsDBNull(1) ? Guid.Empty : dr.GetGuid(1);
						_CommentDate = dr.GetDateTime(2);
						_Author = dr[3].ToString();
						_Email = dr[4].ToString();
						_Website = dr[5].ToString();
						_Comment = dr[6].ToString();
						_Country = dr[7].ToString();
						_IP = dr[8].ToString();
						_IsApproved = dr.GetBoolean(9);
						_ModeratedBy = dr.IsDBNull(10) ? Guid.Empty : dr.GetGuid(10);
						_Avatar = dr[11].ToString();
						_IsSpam = dr.GetBoolean(12);
						_IsDeleted = dr.GetBoolean(13);
					} else {
						_PostCommentID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public Guid PostCommentID { get { return _PostCommentID; } set { _PostCommentID = value; } }
		public Guid PostID { get { return _PostID; } set { _PostID = value; } }
		public Guid ParentCommentID { get { return _ParentCommentID; } set { _ParentCommentID = value; } }
		public DateTime CommentDate { get { return _CommentDate; } set { _CommentDate = value; } }
		public string Author { get { return _Author; } set { _Author = value; } }
		public string Email { get { return _Email; } set { _Email = value; } }
		public string Website { get { return _Website; } set { _Website = value; } }
		public string Comment { 
			get { return _Comment; } 
			set {
				if (_Comment != value) {
					_Comment = value;
					if (PostCommentID != Guid.Empty) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPostComment SET Comment = @Value WHERE PostCommentID = @PostCommentID", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("PostCommentID", SqlDbType.UniqueIdentifier).Value = _PostCommentID;
								cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public string Country { get { return _Country; } set { _Country = value; } }
		public string IP { get { return _IP; } set { _IP = value; } }
		public bool IsApproved { 
			get { return _IsApproved; } 
			set {
				if (_IsApproved != value) {
					_IsApproved = value;
					if (PostCommentID != Guid.Empty) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPostComment SET IsApproved = @Value WHERE PostCommentID = @PostCommentID", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("PostCommentID", SqlDbType.UniqueIdentifier).Value = _PostCommentID;
								cmd.Parameters.Add("Value", SqlDbType.Bit).Value = value ? 1 : 0;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public Guid ModeratedBy { get { return _ModeratedBy; } set { _ModeratedBy = value; } }
		public string Avatar { get { return _Avatar; } set { _Avatar = value; } }
		public bool IsSpam { get { return _IsSpam; } set { _IsSpam = value; } }
		public bool IsDeleted { 
			get { return _IsDeleted; } 
			set {
				if (_IsDeleted != value) {
					_IsDeleted = value;
					if (PostCommentID != Guid.Empty) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_BlogPostComment SET IsDeleted = @Value WHERE PostCommentID = @PostCommentID", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("PostCommentID", SqlDbType.UniqueIdentifier).Value = _PostCommentID;
								cmd.Parameters.Add("Value", SqlDbType.Bit).Value = value ? 1 : 0;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public List<BlogComment> Comments {
			get {
				List<BlogComment> l = new List<BlogComment>();

				string SQL = "";
				if (HttpContext.Current.User.IsInRole("Blog Admin")) {
					//Show unapproved comments if Blog Admin
					SQL = "SELECT PostCommentID FROM cms_BlogPostComment WHERE ParentCommentID = @ParentCommentID AND IsDeleted = 0 AND PostID = @PostID ORDER BY CommentDate";
				} else {
					SQL = "SELECT PostCommentID FROM cms_BlogPostComment WHERE ParentCommentID = @ParentCommentID AND IsApproved = 1 AND IsDeleted = 0 AND PostID = @PostID ORDER BY CommentDate";
				}
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
						cmd.Parameters.Add("ParentCommentID", SqlDbType.UniqueIdentifier).Value = _PostCommentID;

						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new BlogComment(dr.GetGuid(0)));
						}
						cmd.Connection.Close();
					}
				}
				return l;
			}
		}
		#endregion
	}
	public class BlogPosts {
		#region Fields
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public BlogPosts(string Locale = "en-US") { _Locale = Locale; }
		#endregion

		#region Public Methods
		public BlogPost Add(BlogPostViewModel postViewModel) {
			Guid PostID = Guid.NewGuid();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO [cms_BlogPost] ( " +
							"	[PostID], Locale, [Title], [Description], [PostContent], [AuthorID], [IsPublished], [IsCommentEnabled], [Slug], IconFilename, ThumbnailFilename, HeroFilename, AudioFilename, VideoPath, [PublishDate], [CreatedBy] " +
							") VALUES ( " +
							"	@PostID, @Locale, @Title, @Description, @PostContent, @AuthorID, @IsPublished, @IsCommentEnabled, @Slug, @IconFilename, @ThumbnailFilename, @HeroFilename, @AudioFilename, @VideoPath, @PublishDate, @CreatedBy  " +
							")";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@PostID", SqlDbType.UniqueIdentifier).Value = PostID;
					cmd.Parameters.Add("@Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 255).Value = postViewModel.Title;
					cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = postViewModel.Description + "";
					cmd.Parameters.Add("@PostContent", SqlDbType.NVarChar).Value = postViewModel.PostContent + "";
					if (postViewModel.AuthorID != Guid.Empty) {
						cmd.Parameters.Add("@AuthorID", SqlDbType.UniqueIdentifier).Value = postViewModel.AuthorID;
					} else {
						cmd.Parameters.Add("@AuthorID", SqlDbType.UniqueIdentifier).Value = SqlGuid.Null;
					}
					cmd.Parameters.Add("@IsPublished", SqlDbType.Bit).Value = postViewModel.IsPublished ? 1 : 0;
					cmd.Parameters.Add("@IsCommentEnabled", SqlDbType.Bit).Value = postViewModel.IsCommentEnabled ? 1 : 0;
					cmd.Parameters.Add("@Slug", SqlDbType.NVarChar, 255).Value = postViewModel.Slug ?? "";
					cmd.Parameters.Add("@IconFilename", SqlDbType.VarChar, 250).Value = postViewModel.IconFilename ?? "";
					cmd.Parameters.Add("@ThumbnailFilename", SqlDbType.VarChar, 250).Value = postViewModel.ThumbnailFilename ?? "";
					cmd.Parameters.Add("@HeroFilename", SqlDbType.VarChar, 250).Value = postViewModel.HeroFilename ?? "";
					cmd.Parameters.Add("@AudioFilename", SqlDbType.VarChar, 250).Value = postViewModel.AudioFilename ?? "";
					cmd.Parameters.Add("@VideoPath", SqlDbType.VarChar, 250).Value = postViewModel.VideoPath ?? "";
					if (postViewModel.PublishDate != DateTime.MinValue) {
						cmd.Parameters.Add("@PublishDate", SqlDbType.Date).Value = postViewModel.PublishDate;
					} else {
						cmd.Parameters.Add("@PublishDate", SqlDbType.Date).Value = DateTime.Now.ToShortDateString();
					}
					cmd.Parameters.Add("@CreatedBy", SqlDbType.UniqueIdentifier).Value = (Guid)Membership.GetUser().ProviderUserKey;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			BlogPost post = new BlogPost(PostID);
			post.Tags = postViewModel.Tags;

			if (postViewModel.Categories != null) {
				List<Guid> cats = new List<Guid>();
				foreach (string cat in postViewModel.Categories) {
					cats.Add(new Guid(cat));
				}
				post.AssignedCategories = cats;
			}

			// Add to Lucene search index
			LuceneIndexer li = new LuceneIndexer();
			li.CreateIndexWriter();
			li.AddWebPage(post.PostID.ToString(), post.URL, post.Title, post.PostContent, "Blog");
			li.Close();
			li.IndexWords();

			return post;
		}
		public List<BlogPost> AllPosts(int page = 1, int PostsPerPage = 12) {
			List<BlogPost> l = new List<BlogPost>();
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT * FROM (" +
								"SELECT ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC) AS Rownumber, PostSerial FROM cms_BlogPost WHERE IsDeleted = 0 AND IsPublished = 1 AND PublishDate <= GETDATE()" +
								") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogPost(dr.GetInt32(1)));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		public string AllPostsPager(int page = 1, int PostsPerPage = 12) {
			return PagerControl(page, AllPostsPageCount(PostsPerPage), "/blog/");
		}

		public List<BlogPostViewModel> EveryPost() {
			List<BlogPostViewModel> l = new List<BlogPostViewModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT PostSerial FROM cms_BlogPost WHERE IsDeleted = 0 AND PublishDate <= GETDATE() ORDER BY PublishDate DESC";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogPostViewModel(new BlogPost(dr.GetInt32(0))));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}

		public List<BlogPost> PostsInCategory(int CategorySerial, int page = 1, int PostsPerPage = 12) {
			List<BlogPost> l = new List<BlogPost>();
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT * FROM (" +
								"SELECT ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC) AS Rownumber, PostSerial FROM cms_BlogPost p JOIN cms_BlogPost_Category_xref pc ON pc.PostID = p.PostID JOIN cms_BlogCategory c ON c.CategoryID = pc.CategoryID WHERE p.IsDeleted = 0 AND p.IsPublished = 1 AND c.CategorySerial = @CategorySerial AND p.PublishDate <= GETDATE()" +
								") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@CategorySerial", SqlDbType.Int).Value = CategorySerial;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogPost(dr.GetInt32(1)));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		public string CategoryPager(int CategorySerial, string CategoryName, int page = 1, int PostsPerPage = 12) {
			return PagerControl(page, CategoryPageCount(CategorySerial, PostsPerPage), "/blog/cat/" + CategorySerial.ToString() + "/" + tksUtil.FormatRouteURL(CategoryName) + "/");
		}

		public List<BlogPost> PostsInMonth(string Month, int page = 1, int PostsPerPage = 12) {
			List<BlogPost> l = new List<BlogPost>();
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string month = Month.Substring(0, 2) + "/1/" + Month.Substring(2);
			DateTime monthyear;
			if (DateTime.TryParse(month, out monthyear)) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT * FROM (" +
									"SELECT ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC) AS Rownumber, PostSerial FROM cms_BlogPost p WHERE p.IsDeleted = 0 AND p.IsPublished = 1 AND dateadd(month, datediff(month, 0, p.PublishDate),0) = @MonthYear AND PublishDate <= GETDATE() AND p.IsDeleted = 0 AND p.IsPublished = 1" +
									") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@MonthYear", SqlDbType.VarChar).Value = month;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new BlogPost(dr.GetInt32(1)));
						}
						cmd.Connection.Close();
					}
				}
			}
			return l;
		}
		public string MonthPager(string Month, int page = 1, int PostsPerPage = 12) {
			return PagerControl(page, MonthPageCount(Month, PostsPerPage), "/blog/month/" + Month + "/");
		}

		public List<BlogPost> PostsTagged(string Tag, int page = 1, int PostsPerPage = 12) {
			List<BlogPost> l = new List<BlogPost>();
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT * FROM (" +
								"SELECT ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC) AS Rownumber, PostSerial FROM cms_BlogPostTag t JOIN cms_BlogPost p ON t.PostID = p.PostID WHERE t.Tag = @Tag AND p.IsDeleted = 0 AND p.IsPublished = 1 AND p.PublishDate <= GETDATE()" +
								") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@Tag", SqlDbType.VarChar).Value = Tag;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogPost(dr.GetInt32(1)));
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		public string TagPager(string Tag, int page = 1, int PostsPerPage = 12) {
			return PagerControl(page, TagPageCount(Tag, PostsPerPage), "/blog/tag/" + tksUtil.FormatRouteURL(Tag) + "/");
		}

		public List<BlogPost> SearchPosts(string Query) {
			List<BlogPost> l = new List<BlogPost>();
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	string SQL = "SELECT PostSerial FROM cms_BlogPost WHERE PostContent LIKE @query AND IsDeleted = 0 AND IsPublished = 1 AND PublishDate <= GETDATE() ORDER BY PublishDate DESC";
			//	using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("@query", SqlDbType.VarChar).Value = "%" + Query + "%";
			//		cmd.Connection.Open();
			//		SqlDataReader dr = cmd.ExecuteReader();
			//		while (dr.Read()) {
			//			l.Add(new BlogPost(dr.GetInt32(0)));
			//		}
			//		cmd.Connection.Close();
			//	}
			//}

			var parser = new QueryParser(
				Lucene.Net.Util.Version.LUCENE_30,
				"text",
				new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

			LuceneSearcher ls = new LuceneSearcher();
			ls.traceOff();
			ls.CreateIndexSearcher();
			ls.Search(parser.Parse(Query));
			DataTable Results = ls.ResultsTable;
			foreach (DataRow row in Results.Rows) {
				l.Add(new BlogPost(new Guid(row["id"].ToString())));
			}

			return l;
		}

		public static List<BlogTagViewModel> Tags() {
			List<BlogTagViewModel> l = new List<BlogTagViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Tag, COUNT(*) AS TagCount FROM cms_BlogPostTag GROUP BY Tag ORDER BY Tag", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogTagViewModel() { Tag = dr[0].ToString(), TagCount = dr.GetInt32(1) });
					}
					cmd.Connection.Close();
				}
			}

			return l;
		}
		#endregion

		#region Private Methods
		private int AllPostsPageCount(int PostsPerPage = 12) {
			int ret = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT COUNT(*) FROM cms_BlogPost WHERE IsDeleted = 0 AND IsPublished = 1 AND PublishDate <= GETDATE()";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					ret = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return Convert.ToInt32(Math.Ceiling((double)ret / PostsPerPage));
		}
		private int CategoryPageCount(int CategorySerial, int PostsPerPage = 12) {
			int ret = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT COUNT(*) FROM cms_BlogPost p JOIN cms_BlogPost_Category_xref pc ON pc.PostID = p.PostID JOIN cms_BlogCategory c ON c.CategoryID = pc.CategoryID WHERE p.IsDeleted = 0 AND p.IsPublished = 1 AND c.CategorySerial = @CategorySerial AND p.PublishDate <= GETDATE()";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@CategorySerial", SqlDbType.Int).Value = CategorySerial;
					cmd.Connection.Open();
					ret = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return Convert.ToInt32(Math.Ceiling((double)ret / PostsPerPage));
		}
		private int MonthPageCount(string Month, int PostsPerPage = 12) {
			int ret = 0;
			string month = Month.Substring(0, 2) + "/1/" + Month.Substring(2);

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT COUNT(*) FROM cms_BlogPost p WHERE p.IsDeleted = 0 AND p.IsPublished = 1 AND dateadd(month, datediff(month, 0, p.PublishDate),0) = @MonthYear AND p.IsDeleted = 0 AND p.IsPublished = 1 AND PublishDate <= GETDATE()";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@MonthYear", SqlDbType.VarChar).Value = month;
					cmd.Connection.Open();
					ret = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return Convert.ToInt32(Math.Ceiling((double)ret / PostsPerPage));
		}
		private int TagPageCount(string Tag, int PostsPerPage = 12) {
			int ret = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT COUNT(*) FROM cms_BlogPostTag t JOIN cms_BlogPost p ON t.PostID = p.PostID WHERE t.Tag = @Tag AND p.IsDeleted = 0 AND p.IsPublished = 1 AND p.PublishDate <= GETDATE()";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@Tag", SqlDbType.VarChar).Value = Tag;
					cmd.Connection.Open();
					ret = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return Convert.ToInt32(Math.Ceiling((double)ret / PostsPerPage));
		}
		private string PagerControl(int PageNumber, int PageCount, string BaseURL) {
			StringBuilder sb = new StringBuilder("<div class='pager'>");
			if (PageNumber > 1) {
				sb.Append("<a href='" + BaseURL + Convert.ToString(PageNumber - 1) + "' class='arrow'>&lt;</a>&nbsp;");
			}
			for (int x = 1; x <= PageCount; x++) {
				if (x == PageNumber) {
					sb.Append("<span class='current'>" + x.ToString() + "</span>&nbsp;");
				} else {
					sb.Append("<a href='" + BaseURL + x.ToString() + "'>" + x.ToString() + "</a>&nbsp;");
				}
			}
			if (PageNumber < PageCount) {
				sb.Append("<a href='" + BaseURL + Convert.ToString(PageNumber + 1) + "' class='arrow'>&gt;</a>");
			}
			sb.Append("</div>");
			return sb.ToString();
		}
		#endregion
	}

	public class BlogCommentViewModel {
		private Guid _PostCommentID = Guid.Empty;
		private Guid _PostID = Guid.Empty;
		private Guid _ParentCommentID = Guid.Empty;
		private DateTime _CommentDate = DateTime.MinValue;
		private string _Author = "";
		private string _Email = "";
		private string _Website = "";
		private string _Comment = "";
		private string _Country = "";
		private string _IP = "";
		private bool _IsApproved = false;
		private Guid _ModeratedBy = Guid.Empty;
		private string _Avatar = "";
		private bool _IsSpam = false;
		private bool _IsDeleted = false;

		public Guid PostCommentID { get { return _PostCommentID; } set { _PostCommentID = value; } }
		public Guid PostID { get { return _PostID; } set { _PostID = value; } }
		public Guid ParentCommentID { get { return _ParentCommentID; } set { _ParentCommentID = value; } }
		public DateTime CommentDate { get { return _CommentDate; } set { _CommentDate = value; } }
		public string Author { get { return _Author; } set { _Author = value; } }
		public string Email { get { return _Email; } set { _Email = value; } }
		public string Website { get { return _Website; } set { _Website = value; } }
		public string Comment { get { return _Comment; } set { _Comment = value; } }
		public string Country { get { return _Country; } set { _Country = value; } }
		public string IP { get { return _IP; } set { _IP = value; } }
		public bool IsApproved { get { return _IsApproved; } set { _IsApproved = value; } }
		public Guid ModeratedBy { get { return _ModeratedBy; } set { _ModeratedBy = value; } }
		public string Avatar { get { return _Avatar; } set { _Avatar = value; } }
		public bool IsSpam { get { return _IsSpam; } set { _IsSpam = value; } }
		public bool IsDeleted { get { return _IsDeleted; } set { _IsDeleted = value; } }
	}
	public class BlogPostViewModel {
		#region Constructor
		public BlogPostViewModel() {
			PublishDate = DateTime.Now;
		}
		public BlogPostViewModel(BlogPost data) {
			this.AssignedCategories = data.AssignedCategories;
			this.AudioFilename = data.AudioFilename;
			this.AuthorID = data.AuthorID;
			this.CommentCount = data.CommentCount;
			this.CommentToApproveCount = data.CommentToApproveCount;
			this.Description = data.Description;
			this.HeroFilename = data.HeroFilename;
			this.IconFilename = data.IconFilename;
			this.IsCommentEnabled = data.IsCommentEnabled;
			this.IsPublished = data.IsPublished;
			this.PostContent = data.PostContent;
			this.PostID = data.PostID;
			this.PostSerial = data.PostSerial;
			this.PublishDate = data.PublishDate;
			this.Slug = data.Slug;
			this.Tags = data.Tags;
			this.ThumbnailFilename = data.ThumbnailFilename;
			this.Title = data.Title;
			this.URL = data.URL;
			this.VideoPath = data.VideoPath;
		}
		#endregion

		#region Public Properties
		public List<Guid> AssignedCategories { get; set; }
		public string AudioFilename { get; set; }
		public Guid AuthorID { get; set; }
		public string[] Categories { get; set; }
		public int CommentCount { get; set; }
		public int CommentToApproveCount { get; set; }
		public string Description { get; set; }
		public string HeroFilename { get; set; }
		public string IconFilename { get; set; }
		public bool IsCommentEnabled { get; set; }
		public bool IsPublished { get; set; }
		public string PostContent { get; set; }
		public Guid PostID { get; set; }
		public int PostSerial { get; set; }
		public DateTime PublishDate { get; set; }
		public string Slug { get; set; }
		public string Tags { get; set; }
		public string ThumbnailFilename { get; set; }
		public string Title { get; set; }
		public string VideoPath { get; set; }
		public string URL { get; set; }
		#endregion
	}

	public class BlogAuthor {
		public string AuthorName { get; set; }
		public string UserID { get; set; }
	}
	public class BlogAuthors {
		public static List<SelectListItem> AuthorList(Guid SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT u.UserId, ISNULL(a.FirstName + ' ' + a.LastName, u.UserName) AS AuthorName FROM aspnet_Roles r JOIN aspnet_UsersInRoles uir ON r.RoleId = uir.RoleId JOIN aspnet_Users u ON uir.UserID = u.UserID LEFT JOIN Account a ON u.UserID = a.UserID WHERE r.RoleName = 'Blog Author' ORDER BY AuthorName", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						itemList.Add(new SelectListItem {
							Value = dr[0].ToString(),
							Text = dr[1].ToString(),
							Selected = (dr.GetGuid(0) == SelectedID)
						});
					}
					cmd.Connection.Close();
				}
			}
			return itemList;
		}
	}
}
