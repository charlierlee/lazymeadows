using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TKS.Areas.Admin.Models.CMS {
	public class Testimonial {
		#region Fields
		private int _ShortDescriptionLength = 100;
		private int _TestimonialSerial = 0;
		private string _Content = "";
		private string _ShortContent = "";
		private string _ReceivedFrom = "";
		#endregion

		#region Constructor
		public Testimonial(int TestimonialSerial) {
			_TestimonialSerial = TestimonialSerial;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT Content, ShortContent, ReceivedFrom FROM cms_Testimonial WHERE TestimonialSerial = @TestimonialSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("TestimonialSerial", SqlDbType.Int).Value = _TestimonialSerial;

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						_Content = dr[0].ToString();
						_ShortContent = dr[1].ToString();
						_ReceivedFrom = dr[2].ToString();
					} else {
						_TestimonialSerial = 0;
					}
					cmd.Connection.Close();
				}
			}
		}
		#endregion

		#region Properties
		public int TestimonialSerial { get { return _TestimonialSerial; } }
		public string Content {
			get { return _Content; }
			set {
				if (_Content != value) {
					_Content = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Testimonial SET Content = @Value WHERE TestimonialSerial = @TestimonialSerial";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@TestimonialSerial", SqlDbType.Int).Value = _TestimonialSerial;
							cmd.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string ShortContent {
			get {
				if (!string.IsNullOrEmpty(_ShortContent)) {
					return _ShortContent;
				} else {
					if (_Content.Length <= _ShortDescriptionLength) {
						return _Content;
					} else {
						int spaceFound = _Content.IndexOf(" ", _ShortDescriptionLength);
						if (spaceFound == 0) {
							return _Content;
						} else {
							return _Content.Substring(0, spaceFound) + "...";
						}
					}
				}
			}
			set {
				if (_ShortContent != value) {
					_ShortContent = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Testimonial SET ShortContent = @Value WHERE TestimonialSerial = @TestimonialSerial";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@TestimonialSerial", SqlDbType.Int).Value = _TestimonialSerial;
							cmd.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value;

							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}
		}
		public string ReceivedFrom {
			get { return _ReceivedFrom; }
			set {
				if (_ReceivedFrom != value) {
					_ReceivedFrom = value;
					using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
						string SQL = "UPDATE cms_Testimonial SET ReceivedFrom = @Value WHERE TestimonialSerial = @TestimonialSerial";
						using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.Add("@TestimonialSerial", SqlDbType.Int).Value = _TestimonialSerial;
							cmd.Parameters.Add("@Value", SqlDbType.NVarChar).Value = value;

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
		public void Delete() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("DELETE FROM cms_Testimonial WHERE TestimonialSerial = @TestimonialSerial", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@TestimonialSerial", SqlDbType.Int).Value = _TestimonialSerial;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public TestimonialViewModel GetViewModel() {
			TestimonialViewModel viewModel = new TestimonialViewModel();
			viewModel.Content = Content;
			viewModel.ReceivedFrom = ReceivedFrom;
			viewModel.ShortContent = ShortContent;
			viewModel.TestimonialSerial = TestimonialSerial;
			return viewModel;
		}
		#endregion
	}

	public class TestimonialSet {
		#region Public Methods
		public void Add(TestimonialViewModel testimonialViewModel) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO [cms_Testimonial] ( [Content], [ShortContent], [ReceivedFrom] ) VALUES (@Content, @ShortContent, @ReceivedFrom)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@Content", SqlDbType.NVarChar).Value = testimonialViewModel.Content;
					cmd.Parameters.Add("@ShortContent", SqlDbType.NVarChar).Value = testimonialViewModel.ShortContent;
					cmd.Parameters.Add("@ReceivedFrom", SqlDbType.NVarChar).Value = testimonialViewModel.ReceivedFrom;

					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public List<TestimonialViewModel> Testimonials() {
			List<TestimonialViewModel> l = new List<TestimonialViewModel>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT TestimonialSerial FROM cms_Testimonial ORDER BY TestimonialSerial DESC", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();

					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new Testimonial(dr.GetInt32(0)).GetViewModel());
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		public List<Testimonial> TestimonialsRandom() {
			List<Testimonial> l = new List<Testimonial>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT TestimonialSerial FROM cms_Testimonial ORDER BY NEWID()", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();

					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						l.Add(new Testimonial(dr.GetInt32(0)));
					}
					cmd.Connection.Close();
				}
			}
			return l;
		}
		#endregion
	}

	public class TestimonialViewModel {
		public int TestimonialSerial { get; set; }
		public string Content { get; set; }
		public string ShortContent { get; set; }
		public string ReceivedFrom { get; set; }
	}
}