using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using Indexer;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using TKS.Areas.Admin.Models;

namespace TKS.Models.Blog {
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
						//_AuthorTwitterHandle = dr["TwitterHandle"].ToString();
						//_AuthorGooglePlusURL = dr["GooglePlusURL"].ToString();
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
		protected string GetComments() {
			List<BlogComment> CommentList = this.Comments;
			StringBuilder sb = new StringBuilder();
			foreach (BlogComment comment in CommentList) {
				string pendingClass = !comment.IsApproved ? " blogCommentPending" : "";
				sb.AppendLine("<div class='blogCommentItem" + pendingClass + "'>");
				sb.AppendLine("	<div class='ym-grid' id='" + comment.PostCommentID + "'>");
				sb.AppendLine("		<div class='ym-g50 ym-gl'>");
				sb.AppendLine("			<div class='blogCommentAuthor'>" + comment.Author + "</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("		<div class='ym-g50 ym-gr'>");
				sb.AppendLine("			<div class='blogCommentDate'>" + comment.CommentDate.ToShortDateString() + " @ " + comment.CommentDate.ToShortTimeString() + "</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("	</div>");
				sb.AppendLine("	<div class='blogCommentBody'>");
				sb.AppendLine("		" + TKS.Areas.Admin.Models.tksUtil.NL2BR(comment.Comment));
				sb.AppendLine("	</div>");
				if (HttpContext.Current.User.Identity.IsAuthenticated)
					sb.AppendLine("	<a href='javascript:;' class='rLink' data-id='" + comment.PostCommentID + "'>Reply</a>");
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
				sb.AppendLine("<div class='blogCommentItem" + pendingClass + "' style='padding-left:" + Convert.ToString(15 * level) + "px'>");
				sb.AppendLine("	<div class='ym-grid' id='" + comm.PostCommentID + "'>");
				sb.AppendLine("		<div class='ym-g50 ym-gl'>");
				sb.AppendLine("			<div class='blogCommentAuthor'>" + comm.Author + "</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("		<div class='ym-g50 ym-gr'>");
				sb.AppendLine("			<div class='blogCommentDate'>" + comm.CommentDate.ToShortDateString() + " @ " + comm.CommentDate.ToShortTimeString() + "</div>");
				sb.AppendLine("		</div>");
				sb.AppendLine("	</div>");
				sb.AppendLine("	<div class='blogCommentBody'>");
				sb.AppendLine("		" + tksUtil.NL2BR(comm.Comment));
				sb.AppendLine("	</div>");
				if (HttpContext.Current.User.Identity.IsAuthenticated)
					sb.AppendLine("	<a href='javascript:;' class='rLink' id='rLink" + comm.PostCommentID + "' onclick='replyComment(\"" + comm.PostCommentID + "\")'>Reply</a>");
				sb.AppendLine("</div>");
				sb.Append(GetChildComments(comm, level + 1));
			}
			return sb.ToString();
		}
		#endregion

		#region Public Methods
		public void AddComment(BlogCommentViewModel comment) {
			Guid PostCommentID = Guid.NewGuid();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "INSERT INTO [cms_BlogPostComment] ( " +
							"	[PostCommentID], [PostID], [ParentCommentID], [Author], [Email], [Website], [Comment], [Country], [Ip], [IsApproved], [Avatar] " +
							"	) VALUES ( " +
							"	@PostCommentID, @PostID, @ParentCommentID, @Author, @Email, @Website, @Comment, @Country, @Ip, @IsApproved, @Avatar " +
							"	)";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("PostCommentID", SqlDbType.UniqueIdentifier).Value = PostCommentID;
					cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
					cmd.Parameters.Add("ParentCommentID", SqlDbType.UniqueIdentifier).Value = comment.ParentCommentID == Guid.Empty ? SqlGuid.Null : comment.ParentCommentID;
					cmd.Parameters.Add("Author", SqlDbType.NVarChar, 255).Value = comment.Author;
					cmd.Parameters.Add("Email", SqlDbType.NVarChar, 255).Value = comment.Email;
					cmd.Parameters.Add("Website", SqlDbType.NVarChar, 255).Value = comment.Website ?? SqlString.Null;
					cmd.Parameters.Add("Comment", SqlDbType.NVarChar).Value = Regex.Replace(comment.Comment, @"<(.|\n)*?>", string.Empty);
					cmd.Parameters.Add("Country", SqlDbType.NVarChar, 255).Value = comment.Country ?? SqlString.Null;
					cmd.Parameters.Add("Ip", SqlDbType.NVarChar, 255).Value = comment.IP;
					cmd.Parameters.Add("IsApproved", SqlDbType.Bit).Value = comment.IsApproved ? 1 : 0;
					cmd.Parameters.Add("Avatar", SqlDbType.NVarChar, 255).Value = comment.Avatar ?? SqlString.Null;


					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					//---------------- Sending Email
					if (!string.IsNullOrEmpty(Global.BlogCommentNotificationAddress)) {
						string strBody = "<html><body>===========================================<br />Blog Comment Submitted<br />===========================================<br />" +
										  "<a href='" + HttpContext.Current.Request.Url.AbsoluteUri + "'>" + HttpContext.Current.Request.Url.AbsoluteUri + "</a><br />" +
										  "Author: " + comment.Author + " - " + comment.Email + "<br />" +
										  "Comment:<br />" + tksUtil.NL2BR(comment.Comment) + "<br /></body></html>";

						MailMessage mm = new MailMessage();
						mm.Subject = "Blog Comment Submitted";
						mm.To.Add(Global.BlogCommentNotificationAddress);
						mm.From = new MailAddress("web@uofmusic.com", "UofMusic.com");
						mm.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

						AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(strBody, @"<(.|\n)*?>", string.Empty), System.Text.Encoding.GetEncoding("utf-8"), "text/plain");
						AlternateView htmlView = AlternateView.CreateAlternateViewFromString(strBody, System.Text.Encoding.GetEncoding("utf-8"), "text/html");
						mm.AlternateViews.Add(plainView);
						mm.AlternateViews.Add(htmlView);
						SmtpClient smtp = new SmtpClient();
						try {
							smtp.Send(mm);
						} catch { }
					}
				}
			}
		}
		public void CommentsSubscribe() {
			if (HttpContext.Current.User.Identity.IsAuthenticated) {
				MembershipUser u = Membership.GetUser();
				if (!string.IsNullOrEmpty(u.Email)) {
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "INSERT INTO [cms_BlogPostNotify] ( " +
									"	[PostID], [NotifyAddress] " +
									"	) VALUES ( " +
									"	@PostID, @Email " +
									"	)";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
							cmd.Parameters.Add("Email", SqlDbType.NVarChar, 255).Value = u.Email;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public void CommentsUnsubscribe() {
			if (HttpContext.Current.User.Identity.IsAuthenticated) {
				MembershipUser u = Membership.GetUser();
				if (!string.IsNullOrEmpty(u.Email)) {
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "DELETE FROM [cms_BlogPostNotify] " +
									"	WHERE [PostID] = @PostID " +
									"	AND [NotifyAddress] = @Email";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = _PostID;
							cmd.Parameters.Add("Email", SqlDbType.NVarChar, 255).Value = u.Email;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public static string GetAuthorName(Guid UserID) {
			string ret = "";
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT FirstName, LastName FROM [Account] WHERE UserID = @UserID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("UserID", SqlDbType.UniqueIdentifier).Value = UserID;
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
		public string PostNext() {
			int Serial = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "WITH CTE AS ( " +
							"	  SELECT RN = ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC), PostSerial " +
							"	  FROM cms_BlogPost " +
							"	) " +
							"	SELECT ISNULL([Next Row].PostSerial, 0) AS NextSerial " +
							"	FROM CTE [Current Row] LEFT JOIN CTE [Next Row] ON [Next Row].RN = [Current Row].RN + 1 " +
							"	WHERE [Current Row].PostSerial = @PostSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@PostSerial", SqlDbType.Int).Value = this.PostSerial;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
						dr.Read();
						Serial = dr.GetInt32(0);
					}
					cmd.Connection.Close();
				}
			}

			if (Serial > 0) {
				BlogPost nextPost = new BlogPost(Serial);
				if (nextPost.PostID != Guid.Empty) {
					return "<a href='" + nextPost.URL + "'>" + nextPost.Title + "&nbsp;&gt;</a>";
				} else {
					return "";
				}
			} else {
				return "";
			}
		}
		public string PostPrev() {
			int Serial = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "WITH CTE AS ( " +
							"	  SELECT RN = ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC), PostSerial " +
							"	  FROM cms_BlogPost " +
							"	) " +
							"	SELECT ISNULL([Previous Row].PostSerial, 0) AS PreviousSerial " +
							"	FROM CTE [Current Row] LEFT JOIN CTE [Previous Row] ON [Previous Row].RN = [Current Row].RN - 1 " +
							"	WHERE [Current Row].PostSerial = @PostSerial";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@PostSerial", SqlDbType.Int).Value = this.PostSerial;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
						dr.Read();
						Serial = dr.GetInt32(0);
					}
					cmd.Connection.Close();
				}
			}

			if (Serial > 0) {
				BlogPost prevPost = new BlogPost(Serial);
				if (prevPost.PostID != Guid.Empty) {
					return "<a href='" + prevPost.URL + "'>&lt;&nbsp;" + prevPost.Title + "</a>";
				} else {
					return "";
				}
			} else {
				return "";
			}
		}
		#endregion

		#region Properties
		public int PostSerial { get { return _PostSerial; } }
		public Guid PostID { get { return _PostID; } }
		public string AdminLink {
			get {
				if (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("BlogAdmin")) {
					return "<p><a href='/admin/blog/edit/" + PostSerial.ToString() + "'>Edit</a></p>";
				} else {
					return "";
				}
			}
		}
		public string AssignedCategories {
			get {
				string ret = "";
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT c.CategoryID, c.CategoryName, c.CategorySerial FROM cms_BlogPost_Category_xref x JOIN cms_BlogCategory c ON x.CategoryID = c.CategoryID WHERE x.PostID = @PostID", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("PostID", SqlDbType.UniqueIdentifier).Value = PostID;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							ret += "<a href='/blog/cat/" + dr[2].ToString() + "/" + tksUtil.FormatRouteURL(dr[1].ToString()) + "' rel='nofollow'>" + dr[1].ToString() + "</a>, ";
						}
						cmd.Connection.Close();
					}
				}
				if (ret.Length > 2) { ret = ret.Substring(0, ret.Length - 2); }
				return ret;
			}
		}
		public string AudioFilename { get { return _AudioFilename; } }
		public string AudioObject {
			get {
				if (!string.IsNullOrEmpty(_AudioFilename)) {
					string encoding = "audio/mpeg";
					string extension = Path.GetExtension(_AudioFilename).ToLower();
					if (extension == "m4a") {
						encoding = "audio/mp4";
					} else if (extension == "oga") {
						encoding = "audio/ogg";
					}
					return string.Format("<audio controls><source src='{0}' type='{1}' /> Your browser does not support the audio element.</audio>", _AudioFilename, encoding);
				} else {
					return "";
				}
			}
		}
		public string Author { get { return _Author; } }
		public Guid AuthorID { get { return _AuthorID; } }
		public string AuthorURL {
			get {
				string ret = "";
				if (!string.IsNullOrEmpty(_Author)) {
					if (_AuthorID != Guid.Empty) {
						ret = "<a href='/blog/author/" + _AuthorID.ToString() + "/" + tksUtil.FormatRouteURL(_Author) + "' rel='nofollow'>" + _Author + "</a>";
					} else {
						ret = _Author;
					}
				}
				return ret;
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
			get {
				if (!string.IsNullOrEmpty(_Description)) {
					return _Description;
				} else {
					if (!string.IsNullOrEmpty(PostContent)) {
						string content = Regex.Replace(PostContent, @"<(.|\n)*?>", string.Empty);
						if (!string.IsNullOrEmpty(content)) {
							if (content.Length > 200) {
								int nextSpace = content.IndexOf(" ", 200);
								if (nextSpace > 0) {
									return content.Substring(0, nextSpace);
								} else {
									return content;
								}
							} else {
								return content;
							}
						}
					}
					return "";
				}
			} 
		}
		public string HeroFilename { get { return _HeroFilename; } }
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
		public string IconFilename { get { return _IconFilename; } }
		public bool IsCommentEnabled { get { return _IsCommentEnabled; } }
		public bool IsPublished { get { return _IsPublished; } }
		public string MetaTags { 
			get {
				string desc = "";
				if (Description.Length > 0) {
					desc = Description;
				} else {
					desc = Regex.Replace(PostContent, @"<(.|\n)*?>", string.Empty);
					if (desc.Length > 200) {
						desc = desc.Substring(0, 200);
					}
				}

				return new SEO(_PostID).GetTags(URL, Title, desc); 
			} 
		}
		public string Permalink { get { return "/blog/post.aspx?guid=" + _PostID.ToString(); } }
		public string PostContent { get { return _PostContent; } }
		public DateTime PublishDate { get { return _PublishDate; } }
		public string Slug { get { return (_Slug.Length > 0) ? _Slug : tksUtil.FormatRouteURL(_Title); } }
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
		public string ThumbnailFilename { get { return _ThumbnailFilename; } }
		public string Title { get { return _Title; } }
		public string URL { get { return Global.BlogDirectory + "/" + _PostSerial.ToString() + "/" + Slug; } }
		public string VideoPath { get { return _VideoPath; } }
		public string VideoObject {
			get {
				if (!string.IsNullOrEmpty(_VideoPath)) {
					string path = _VideoPath;
					path = path.Replace("https:", "");
					path = path.Replace("http:", "");
					int hasEqual = path.IndexOf("=");
					if (hasEqual > 0) {
						return string.Format("<p><iframe width='560' height='315' src='//www.youtube-nocookie.com/embed/{0}?rel=0'></iframe></p>", path.Substring(hasEqual + 1));
					} else {
						return string.Format("<p><iframe width='560' height='315' src='{0}?rel=0'></iframe></p>", path);
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
		public static List<BlogArchiveViewModel> ArchiveMonths() {
			List<BlogArchiveViewModel> l = new List<BlogArchiveViewModel>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT DATEADD(month, DATEDIFF(month, 0, PublishDate),0) AS ArchiveMonth, COUNT(*) AS ArchiveMonthCount FROM cms_BlogPost WHERE IsDeleted = 0 AND IsPublished = 1 AND PublishDate <= GETDATE() GROUP BY DATEADD(month, DATEDIFF(month, 0, PublishDate),0) ORDER BY DATEADD(month, DATEDIFF(month, 0, PublishDate),0) DESC", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogArchiveViewModel() { ArchiveMonth = dr.GetDateTime(0), PostCount = dr.GetInt32(1) });
					}
					cmd.Connection.Close();
				}
			}

			return l;
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
			return new Pager().GetPager("/blog/", AllPostsCount(), page, PostsPerPage);
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

		public List<BlogPost> PostsByAuthor(Guid AuthorID, int page = 1, int PostsPerPage = 12) {
			List<BlogPost> l = new List<BlogPost>();
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT * FROM (" +
								"SELECT ROW_NUMBER() OVER (ORDER BY PublishDate DESC, PostSerial DESC) AS Rownumber, PostSerial FROM cms_BlogPost p WHERE p.IsDeleted = 0 AND p.IsPublished = 1 AND p.AuthorID = @AuthorID AND p.PublishDate <= GETDATE()" +
								") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@AuthorID", SqlDbType.UniqueIdentifier).Value = AuthorID;
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
		public string ByAuthorPager(Guid AuthorID, string AuthorName, int page = 1, int PostsPerPage = 12) {
			return new Pager().GetPager("/blog/author/" + AuthorID.ToString() + "/" + tksUtil.FormatRouteURL(AuthorName) + "/", AuthorPostCount(AuthorID), page, PostsPerPage);
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
			return new Pager().GetPager("/blog/cat/" + CategorySerial.ToString() + "/" + tksUtil.FormatRouteURL(CategoryName) + "/", CategoryPostCount(CategorySerial), page, PostsPerPage);
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
			return new Pager().GetPager("/blog/month/" + Month + "/", MonthPostCount(Month), page, PostsPerPage);
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
					cmd.Parameters.Add("@Tag", SqlDbType.VarChar).Value = Tag.Replace('-', ' ');
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
			return new Pager().GetPager("/blog/tag/" + tksUtil.FormatRouteURL(Tag) + "/", TagPostCount(Tag), page, PostsPerPage);
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

		public static List<BlogPost> RecentPosts(int Count = 3) {
			List<BlogPost> l = new List<BlogPost>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT TOP " + Count.ToString() + " PostSerial FROM cms_BlogPost WHERE IsDeleted = 0 AND IsPublished = 1 AND PublishDate <= GETDATE() ORDER BY PublishDate DESC", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new BlogPost(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
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
		private int AllPostsCount() {
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
			return ret;
		}
		private int AuthorPostCount(Guid AuthorID) {
			int ret = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT COUNT(*) FROM cms_BlogPost p WHERE p.IsDeleted = 0 AND p.IsPublished = 1 AND p.AuthorID = @AuthorID AND p.PublishDate <= GETDATE()";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@AuthorID", SqlDbType.UniqueIdentifier).Value = AuthorID;
					cmd.Connection.Open();
					ret = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();
				}
			}
			return ret;
		}
		private int CategoryPostCount(int CategorySerial) {
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
			return ret;
		}
		private int MonthPostCount(string Month) {
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
			return ret;
		}
		private int TagPostCount(string Tag) {
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
			return ret;
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

	public class BlogArchiveViewModel {
		public DateTime ArchiveMonth {get;set;}
		public int PostCount{get;set;}
	}
	public class BlogCommentViewModel {
		public Guid PostCommentID { get; set; }
		public Guid PostID { get; set; }
		public Guid ParentCommentID { get; set; }
		public DateTime CommentDate { get; set; }
		public string Author { get; set; }
		public string Email { get; set; }
		public string Website { get; set; }
		public string Comment { get; set; }
		public string Country { get; set; }
		public string IP { get; set; }
		public bool IsApproved { get; set; }
		public Guid ModeratedBy { get; set; }
		public string Avatar { get; set; }
		public bool IsSpam { get; set; }
		public bool IsDeleted { get; set; }
	}
	public class BlogPostViewModel {
		#region Constructor
		public BlogPostViewModel() { }
		public BlogPostViewModel(BlogPost data) {
			this.AssignedCategories = data.AssignedCategories;
			this.AudioObject = data.AudioObject;
			this.AuthorURL = data.AuthorURL;
			this.Comments = data.GetFormattedComments();
			this.Description = data.Description;
			this.HeroObject = data.HeroObject;
			this.IconFilename = data.IconFilename;
			this.IsCommentEnabled = data.IsCommentEnabled;
			this.IsPublished = data.IsPublished;
			this.PostContent = data.PostContent;
			this.PostID = data.PostID;
			this.PostNext = data.PostNext();
			this.PostPrev = data.PostPrev();
			this.PostSerial = data.PostSerial;
			this.PublishDate = data.PublishDate;
			this.Slug = data.Slug;
			this.Tags = data.TagsURL;
			this.ThumbnailFilename = data.ThumbnailFilename;
			this.Title = data.Title;
			this.URL = data.URL;
			this.VideoObject = data.VideoObject;
		}
		#endregion

		#region Public Properties
		public bool IsCommentEnabled { get; set; }
		public bool IsPublished { get; set; }
		public DateTime PublishDate { get; set; }//
		public Guid PostID { get; set; }
		public int PostSerial { get; set; }
		public string AssignedCategories { get; set; }//
		public string AudioObject { get; set; }//
		public string AuthorURL { get; set; }//
		public string Comments { get; set; }//
		public string Description { get; set; }
		public string EditLink { get; set; }//
		public string HeroObject { get; set; }//
		public string IconFilename { get; set; }
		public string PostContent { get; set; }//
		public string PostNext { get; set; }//
		public string PostPrev { get; set; }//
		public string Slug { get; set; }
		public string Tags { get; set; }
		public string ThumbnailFilename { get; set; }
		public string Title { get; set; }//
		public string URL { get; set; }
		public string VideoObject { get; set; }//
		#endregion
	}

	public class BlogAuthor {
		public string AuthorName { get; set; }
		public string UserID { get; set; }
	}
	public class BlogAuthors {
		public static List<BlogAuthor> AuthorList() {
			List<BlogAuthor> l = new List<BlogAuthor>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT u.UserId, ISNULL(a.FirstName + ' ' + a.LastName, u.UserName) AS AuthorName FROM aspnet_Roles r JOIN aspnet_UsersInRoles uir ON r.RoleId = uir.RoleId JOIN aspnet_Users u ON uir.UserID = u.UserID LEFT JOIN Account a ON u.UserID = a.UserID WHERE r.RoleName = 'Blog Author' ORDER BY AuthorName", cn)) {
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
	}
}
