using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace TKS.Areas.Admin.Models.LazyMeadows {
	public class SoldSet {
		#region Fields
		#endregion

		#region Constructor
		public SoldSet() {
		}
        #endregion

        #region Public Methods
        public List<SoldViewModel> GetList() {
            List<SoldViewModel> p = new List<SoldViewModel>();
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "SELECT ItemSerial, Content, Thumbnail, Interior, Exterior FROM [Sold] ORDER BY SortOrder, ItemSerial DESC";

                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if(dr.HasRows) {
                        while(dr.Read()) {
                            p.Add(new SoldViewModel {
                                Serial = dr.GetInt32(0),
                                Content = dr[1].ToString(),
                                Thumbnail = dr[2].ToString(),
                                Interior = dr[3].ToString(),
                                Exterior = dr[4].ToString()
                            });
                        }
                    }
                    cmd.Connection.Close();
                }
            }
            return p;
        }
        //public void UpdateSortOrder(int SocialPageSerial, int SortOrder) {
        //    using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
        //        using(SqlCommand cmd = new SqlCommand()) {
        //            cmd.Connection = cn;
        //            cmd.CommandType = CommandType.Text;

        //            cmd.CommandText = "UPDATE SocialPage SET SortOrder = @SortOrder WHERE SocialPageSerial = @SocialPageSerial";
        //            cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = SocialPageSerial;
        //            cmd.Parameters.Add("SortOrder", SqlDbType.Int).Value = SortOrder;
        //            cmd.Connection.Open();
        //            cmd.ExecuteNonQuery();
        //            cmd.Connection.Close();
        //        }
        //    }
        //}
        public int Add(SoldViewModel data) {
            int ID = 0;
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "INSERT Sold (Content, Thumbnail, Interior, Exterior, SortOrder) " +
                    "OUTPUT Inserted.ItemSerial " +
                    "VALUES (@Content, @Thumbnail, @Interior, @Exterior, 0)";
                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add("Content", SqlDbType.VarChar).Value = data.Content ?? "";
                    cmd.Parameters.Add("Thumbnail", SqlDbType.VarChar, 250).Value = data.Thumbnail ?? "";
                    cmd.Parameters.Add("Interior", SqlDbType.VarChar, 250).Value = data.Interior ?? "";
                    cmd.Parameters.Add("Exterior", SqlDbType.VarChar, 250).Value = data.Exterior ?? "";
                    cmd.Connection.Open();
                    ID = (int)cmd.ExecuteScalar();
                    cmd.Connection.Close();
                }
            }
            return ID;
        }
        //public void EditCuratedPage(SocialSite data) {
        //    using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
        //        using(SqlCommand cmd = new SqlCommand()) {
        //            cmd.Connection = cn;
        //            cmd.CommandType = CommandType.Text;

        //            //Make room at the top
        //            cmd.CommandText = "UPDATE SocialPage SET PageType = @PageType, URL = @URL, TitleText = @TitleText WHERE SocialPageSerial = @SocialPageSerial";
        //            cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = data.SocialPageSerial;
        //            cmd.Parameters.Add("PageType", SqlDbType.VarChar, 50).Value = data.PageType;
        //            cmd.Parameters.Add("URL", SqlDbType.VarChar).Value = data.URL;
        //            cmd.Parameters.Add("TitleText", SqlDbType.VarChar, 250).Value = data.TitleText;
        //            cmd.Connection.Open();
        //            cmd.ExecuteNonQuery();
        //            cmd.Connection.Close();
        //        }
        //    }
        //}
        //public void RemoveCuratedPage(int SocialPageSerial) {
        //    using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
        //        using(SqlCommand cmd = new SqlCommand()) {
        //            cmd.Connection = cn;
        //            cmd.CommandType = CommandType.Text;

        //            //Make room at the top
        //            cmd.CommandText = "DELETE SocialPage WHERE SocialPageSerial = @SocialPageSerial";
        //            cmd.Parameters.Add("SocialPageSerial", SqlDbType.Int).Value = SocialPageSerial;
        //            cmd.Connection.Open();
        //            cmd.ExecuteNonQuery();
        //            cmd.Connection.Close();
        //        }
        //    }
        //}
        #endregion

        #region Public Properties
        #endregion
    }

	public class Sold {
        #region Fields
        private int _ItemSerial = 0;
        private int _SortOrder = 0;
        private string _Content = "";
        private string _Thumbnail = "";
        private string _Interior = "";
        private string _Exterior = "";
        #endregion

        #region Constructor
        public Sold() { }
        public Sold(int ItemSerial) {
            _ItemSerial = ItemSerial;
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "SELECT Content, Thumbnail, Interior, Exterior, SortOrder FROM [Sold] WHERE ItemSerial = @ItemSerial";
                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("ItemSerial", SqlDbType.Int).Value = _ItemSerial;

                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    if(dr.HasRows) {
                        dr.Read();
                        _Content = dr[0].ToString();
                        _Thumbnail = dr[1].ToString();
                        _Interior = dr[2].ToString();
                        _Exterior = dr[3].ToString();
                        _SortOrder = dr.GetInt32(4);
                    } else {
                        _ItemSerial = 0;
                    }
                    cmd.Connection.Close();
                }
            }
        }
        #endregion

        #region Private Methods
        private void UpdateIntProperty(string FieldName, int Value) {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                using(SqlCommand cmd = new SqlCommand("UPDATE [Sold] SET " + FieldName + " = @Value WHERE ItemSerial = @ItemSerial", cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("ItemSerial", SqlDbType.Int).Value = _ItemSerial;
                    cmd.Parameters.Add("Value", SqlDbType.Int).Value = Value;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
        }
        private string UpdateStringProperty(string FieldName, int Length, string Value) {
            string ret = "";
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                using(SqlCommand cmd = new SqlCommand("UPDATE [Sold] SET " + FieldName + " = @Value WHERE ItemSerial = @ItemSerial", cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("ItemSerial", SqlDbType.Int).Value = _ItemSerial;
                    if(Length == Int32.MaxValue) {
                        if(Value == null) {
                            cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = SqlString.Null;
                        } else {
                            ret = Value.Trim();
                            cmd.Parameters.Add("Value", SqlDbType.VarChar).Value = ret;
                        }
                    } else {
                        if(Value == null) {
                            cmd.Parameters.Add("Value", SqlDbType.VarChar, Length).Value = SqlString.Null;
                        } else {
                            if(Value.Trim().Length > Length) {
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

        #region Public Methods
        public void Delete() {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                using(SqlCommand cmd = new SqlCommand("DELETE FROM Sold WHERE ItemSerial = @ItemSerial", cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("ItemSerial", SqlDbType.Int).Value = _ItemSerial;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
        }
        #endregion

        #region Properties
        public int ItemSerial { get { return _ItemSerial; } }
        public int SortOrder {
            get { return _SortOrder; }
            set {
                if(_SortOrder != value) {
                    _SortOrder = value;
                    UpdateIntProperty("SortOrder", value);
                }
            }
        }
        public string Content {
            get { return _Content; }
            set {
                if(_Content != value) {
                    _Content = value;
                    UpdateStringProperty("Content", Int32.MaxValue, value);
                }
            }
        }
        public string Thumbnail {
            get { return _Thumbnail; }
            set {
                if(_Thumbnail != value) {
                    _Thumbnail = value;
                    UpdateStringProperty("Thumbnail", 250, value);
                }
            }
        }
        public string Interior {
            get { return _Interior; }
            set {
                if(_Interior != value) {
                    _Interior = value;
                    UpdateStringProperty("Interior", 250, value);
                }
            }
        }
        public string Exterior {
            get { return _Exterior; }
            set {
                if(_Exterior != value) {
                    _Exterior = value;
                    UpdateStringProperty("Exterior", 250, value);
                }
            }
        }
        #endregion
	}

    public class SoldViewModel {
        public int Serial { get; set; }
        public int SortOrder { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        public string Interior { get; set; }
        public string Exterior { get; set; }
    }
}