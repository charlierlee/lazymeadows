using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Indexer;

namespace TKS.Areas.Admin.Models.CMS {
	public class Faq {
		#region Fields
		private string _Answer = null;
		private int _FaqCategorySerial = 0;
		private string _FaqCategory = "";
		private Guid _FaqID = Guid.Empty;
		private int _FaqSerial = 0;
		private string _Locale = "en-US";
		private Guid _ModuleID = Guid.Empty;
		private string _Question = null;
		private int _SortOrder = 0;
		private string _URL = "";
		#endregion

		#region Constructor
		public Faq() { }
		public Faq(int FaqSerial) {
			_FaqSerial = FaqSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT f.FaqID, f.ModuleID, f.Locale, f.FAQCategorySerial, f.Question, f.Answer, f.SortOrder, p.VirtualPath, ISNULL(c.FAQCategory, '') AS FAQCategory FROM cms_FAQ f JOIN cms_Module m ON f.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID LEFT JOIN cms_FAQCategory c ON f.FAQCategorySerial = c.FAQCategorySerial WHERE FaqSerial = @FaqSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FaqSerial", SqlDbType.Int).Value = _FaqSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_FaqID = dr.GetGuid(0);
						_ModuleID = dr.GetGuid(1);
						_Locale = dr[2].ToString();
						_FaqCategorySerial = dr.IsDBNull(3) ? 0 : dr.GetInt32(3);
						_Question = dr[4].ToString();
						_Answer = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
						_URL = dr[7].ToString();
						_FaqCategory = dr[8].ToString();
					} else {
						_FaqSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		public Faq(Guid FaqID) {
			_FaqID = FaqID;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT f.FaqSerial, f.ModuleID, f.Locale, f.FAQCategorySerial, f.Question, f.Answer, f.SortOrder, p.VirtualPath, ISNULL(c.FAQCategory, '') AS FAQCategory FROM cms_FAQ f JOIN cms_Module m ON f.ModuleID = m.ModuleID JOIN cms_Page p ON m.PageID = p.PageID LEFT JOIN cms_FAQCategory c ON f.FAQCategorySerial = c.FAQCategorySerial WHERE FaqID = @FaqID", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FaqID", SqlDbType.UniqueIdentifier).Value = _FaqID;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_FaqSerial = dr.GetInt32(0);
						_ModuleID = dr.GetGuid(1);
						_Locale = dr[2].ToString();
						_FaqCategorySerial = dr.IsDBNull(3) ? 0 : dr.GetInt32(3);
						_Question = dr[4].ToString();
						_Answer = dr[5].ToString();
						_SortOrder = dr.GetInt32(6);
						_URL = dr[7].ToString();
						_FaqCategory = dr[8].ToString();
					} else {
						_FaqID = Guid.Empty;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		private void UpdateIndex() {
			// Update Lucene search index
			LuceneIndexer li = new LuceneIndexer();
			li.CreateIndexWriter();
			li.UpdateWebPage(FaqID.ToString(), URL, Question, Question + " " + Answer, "FAQ");
			li.Close();
			li.IndexWords();
		}
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_FAQ WHERE FaqSerial = @FaqSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FaqSerial", SqlDbType.Int).Value = _FaqSerial;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}

			// Remove from Lucene search index
			LuceneIndexer li = new LuceneIndexer();
			li.CreateIndexWriter();
			li.Delete(FaqID.ToString());
			li.Close();
		}
		#endregion

		#region Properties
		public string Answer {
			get { return _Answer ?? ""; }
			set {
				if (_Answer != value) {
					_Answer = value;
					if (_FaqSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQ SET Answer = @Value WHERE FaqSerial = @FaqSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("FaqSerial", SqlDbType.Int).Value = _FaqSerial;
								cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}

						UpdateIndex();
					}
				}
			}
		}
		public string FaqCategory { get { return _FaqCategory; } }
		public int FAQCategorySerial {
			get { return _FaqCategorySerial; }
			set {
				if (_FaqCategorySerial != value) {
					_FaqCategorySerial = value;
					if (_FaqSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQ SET FAQCategorySerial = @Value WHERE FaqSerial = @FaqSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("FaqSerial", SqlDbType.Int).Value = _FaqSerial;
								cmd.Parameters.Add("Value", SqlDbType.Int).Value = value > 0 ? value : SqlInt32.Null;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}
					}
				}
			}
		}
		public Guid FaqID { get { return _FaqID; } }
		public int FaqSerial { get { return _FaqSerial; } }
		public string Question {
			get { return _Question ?? ""; }
			set {
				if (_Question != value) {
					_Question = value;
					if (_FaqSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQ SET Question = @Value WHERE FaqSerial = @FaqSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("FaqSerial", SqlDbType.Int).Value = _FaqSerial;
								cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = value;

								cmd.Connection.Open();
								cmd.ExecuteNonQuery();
								cmd.Connection.Close();
							}
						}

						UpdateIndex();
					}
				}
			}
		}
		public int SortOrder {
			get { return _SortOrder; }
			set {
				if (_SortOrder != value) {
					_SortOrder = value;
					if (_FaqSerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQ SET SortOrder = @Value WHERE FaqSerial = @FaqSerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("FaqSerial", SqlDbType.Int).Value = _FaqSerial;
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
		public string URL { get { return _URL; } }
		#endregion
	}
	public class FaqSet {
		#region Fields
		private Guid _PageID = Guid.Empty;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public FaqSet(string Locale = "en-US") {
			_Locale = Locale;
		}
		public FaqSet(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		public FaqSet(Guid PageID, string ModuleName, string Locale) {
			_PageID = PageID;
			_Locale = Locale;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID FROM cms_Module WHERE PageID = @PageID AND ModuleName = @ModuleName", cn)) {
					cmd.Parameters.Add("PageID", SqlDbType.UniqueIdentifier).Value = PageID;
					cmd.Parameters.Add("ModuleName", SqlDbType.VarChar, 50).Value = ModuleName;
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Public Methods
		public void Add(Faq faq) {
			if (_ModuleID != Guid.Empty) {
				Guid FaqID = Guid.NewGuid();
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_FAQ (FaqID, ModuleID, Locale, FAQCategorySerial, Question, Answer, SortOrder) VALUES (@FaqID, @ModuleID, @Locale, @FAQCategorySerial, @Question, @Answer, @SortOrder)", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("FaqID", SqlDbType.UniqueIdentifier).Value = FaqID;
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.Parameters.Add("FAQCategorySerial", SqlDbType.Int).Value = faq.FAQCategorySerial > 0 ? faq.FAQCategorySerial : SqlInt32.Null;
						cmd.Parameters.Add("Question", SqlDbType.VarChar).Value = faq.Question;
						cmd.Parameters.Add("Answer", SqlDbType.VarChar).Value = faq.Answer;
						cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = faq.SortOrder;

						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
				}

				// Update Lucene search index
				Faq newFaq = new Faq(FaqID);
				LuceneIndexer li = new LuceneIndexer();
				li.CreateIndexWriter();
				li.UpdateWebPage(FaqID.ToString(), newFaq.URL, newFaq.Question, newFaq.Question + " " + newFaq.Answer, "FAQ");
				li.Close();
				li.IndexWords();
			}
		}
		public List<Faq> FAQs() {
			List<Faq> l = new List<Faq>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (_ModuleID != Guid.Empty) {
					using (SqlCommand cmd = new SqlCommand("SELECT f.FAQSerial FROM cms_FAQ f LEFT JOIN cms_FAQCategory fq ON f.FAQCategorySerial = fq.FAQCategorySerial WHERE f.ModuleID = @ModuleID AND f.Locale = @Locale ORDER BY fq.SortOrder, fq.FAQCategory, f.SortOrder, f.Question", cn)) {
						cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new Faq(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				} else {
					//Return all FAQs
					using (SqlCommand cmd = new SqlCommand("SELECT f.FAQSerial FROM cms_FAQ f LEFT JOIN cms_FAQCategory fq ON f.FAQCategorySerial = fq.FAQCategorySerial WHERE f.Locale = @Locale ORDER BY fq.SortOrder, fq.FAQCategory, f.SortOrder, f.Question", cn)) {
						cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							l.Add(new Faq(dr.GetInt32(0)));
						}
						cmd.Connection.Close();
					}
				}
			}

			return l;
		}
		#endregion
	}

	public class FaqCategory {
		#region Fields
		private int _FaqCategorySerial = 0;
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		private string _FAQCategory = "";
		private int _SortOrder = 0;
		#endregion

		#region Constructor
		public FaqCategory() { }
		public FaqCategory(int FaqCategorySerial) {
			_FaqCategorySerial = FaqCategorySerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT ModuleID, Locale, FAQCategory, SortOrder FROM cms_FaqCategory WHERE FaqCategorySerial = @FaqCategorySerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FaqCategorySerial", SqlDbType.Int).Value = _FaqCategorySerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_ModuleID = dr.GetGuid(0);
						_Locale = dr[1].ToString();
						_FAQCategory = dr.GetString(2);
						_SortOrder = dr.GetInt32(3);
					} else {
						_FaqCategorySerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Public Methods
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQ SET FAQCategorySerial = NULL WHERE FAQCategorySerial = @FAQCategorySerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("FAQCategorySerial", SqlDbType.Int).Value = _FaqCategorySerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					cmd.CommandText = "DELETE FROM cms_FAQCategory WHERE FAQCategorySerial=@FAQCategorySerial";
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public int FaqCategorySerial { get { return _FaqCategorySerial; } }
		public string FAQCategory {
			get { return _FAQCategory; }
			set {
				if (_FAQCategory != value) {
					_FAQCategory = value;
					if (_FaqCategorySerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQCategory SET FAQCategory = @value WHERE FAQCategorySerial = @FAQCategorySerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("FAQCategorySerial", SqlDbType.Int).Value = _FaqCategorySerial;
								cmd.Parameters.Add("Value", SqlDbType.NVarChar).Value = value;

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
					if (_FaqCategorySerial > 0) {
						using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
							using (SqlCommand cmd = new SqlCommand("UPDATE cms_FAQCategory SET SortOrder = @Value WHERE FAQCategorySerial = @FAQCategorySerial", cn)) {
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add("FAQCategorySerial", SqlDbType.Int).Value = _FaqCategorySerial;
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
	public class FaqCategories {
		#region Fields
		private Guid _ModuleID = Guid.Empty;
		private string _Locale = "en-US";
		#endregion

		#region Constructor
		public FaqCategories(Guid ModuleID, string Locale) {
			_ModuleID = ModuleID;
			_Locale = Locale;
		}
		#endregion

		#region Public Methods
		public void Add(FaqCategory faqCategory) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO cms_FAQCategory (ModuleID, Locale, FAQCategory, SortOrder) VALUES (@ModuleID, @Locale, @FAQCategory, @SortOrder)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Parameters.Add("FAQCategory", SqlDbType.NVarChar).Value = faqCategory.FAQCategory;
					cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = faqCategory.SortOrder;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public List<FaqCategory> Categories() {
			List<FaqCategory> l = new List<FaqCategory>();

			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT FAQCategorySerial FROM cms_FAQCategory WHERE ModuleID = @ModuleID AND Locale = @Locale ORDER BY SortOrder, FAQCategory", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("ModuleID", SqlDbType.UniqueIdentifier).Value = _ModuleID;
					cmd.Parameters.Add("Locale", SqlDbType.VarChar, 10).Value = _Locale;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new FaqCategory(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion
	}
}