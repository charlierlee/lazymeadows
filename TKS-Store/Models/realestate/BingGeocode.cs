using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web;

namespace Gaiocorp.Geocoding {

	/// Resolve addresses into latitude/longitude coordinates using Bing MAP API web services
	public static class BingGeocoder {

		private static string _BingMapsKey = ConfigurationManager.AppSettings["BingMapsKey"];

		public static void UpdateGeoCoding() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT l1.[MLS #], [Address Number], [Address Direction], [Address Street], City, [State], Zip FROM [listings-residential] l1 LEFT JOIN [listings-residential-ext] l2 ON l1.[MLS #] = l2.[MLS #] WHERE l2.[MLS #] IS NULL", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						BingGeolocation? bl = BingGeocoder.ResolveAddress(dr[1].ToString() + " " + dr[2].ToString() + " " + dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), "US");
						if (bl != null) {
							float MLS = 0;
							float lat = 0;
							float lon = 0;
							float.TryParse(dr[0].ToString(), out MLS);
							float.TryParse(bl.Value.Lat.ToString(), out lat);
							float.TryParse(bl.Value.Lon.ToString(), out lon);
							if (lat != 0 && lon != 0) {
								UpdateLatLong(MLS, lat, lon);
							}
						}
					}
					cmd.Connection.Close();
				}
			}
		}
		public static void UpdateGeoCodingComm() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT l1.[MLS #], [Address Number], [Address Direction], [Address Street], City, [State], Zip FROM [listings-commercial] l1 LEFT JOIN [listings-residential-ext] l2 ON l1.[MLS #] = l2.[MLS #] WHERE l2.[MLS #] IS NULL", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						BingGeolocation? bl = BingGeocoder.ResolveAddress(dr[1].ToString() + " " + dr[2].ToString() + " " + dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), "US");
						if (bl != null) {
							float MLS = 0;
							float lat = 0;
							float lon = 0;
							float.TryParse(dr[0].ToString(), out MLS);
							float.TryParse(bl.Value.Lat.ToString(), out lat);
							float.TryParse(bl.Value.Lon.ToString(), out lon);
							if (lat != 0 && lon != 0) {
								UpdateLatLong(MLS, lat, lon);
							}
						}
					}
					cmd.Connection.Close();
				}
			}
		}
		public static void UpdateGeoCodingLand() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT l1.[MLS #], [Address Number], [Address Direction], [Address Street], City, [State], Zip FROM [listings-land] l1 LEFT JOIN [listings-residential-ext] l2 ON l1.[MLS #] = l2.[MLS #] WHERE l2.[MLS #] IS NULL", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						BingGeolocation? bl = BingGeocoder.ResolveAddress(dr[1].ToString() + " " + dr[2].ToString() + " " + dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), "US");
						if (bl != null) {
							float MLS = 0;
							float lat = 0;
							float lon = 0;
							float.TryParse(dr[0].ToString(), out MLS);
							float.TryParse(bl.Value.Lat.ToString(), out lat);
							float.TryParse(bl.Value.Lon.ToString(), out lon);
							if (lat != 0 && lon != 0) {
								UpdateLatLong(MLS, lat, lon);
							}
						}
					}
					cmd.Connection.Close();
				}
			}
		}

        public static void UpdateGeoCoding3() {
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT DISTINCT l1.[OriginatingSystemKey], [UnparsedAddress] " +
						"FROM [listings-residential-onekey] l1 LEFT JOIN [listings-geo] l2 ON l1.[OriginatingSystemKey] = l2.[MLSNumber] " +
						"WHERE l2.[MLSNumber] IS NULL";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							BingGeolocation? bl = BingGeocoder.ResolveAddress(dr[1].ToString());
							if (bl != null) {
								float lat = 0;
								float lon = 0;
								float.TryParse(bl.Value.Lat.ToString(), out lat);
								float.TryParse(bl.Value.Lon.ToString(), out lon);
								if (lat != 0 && lon != 0) {
									UpdateLatLong3(dr[0].ToString(), lat, lon);
								}
							}
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception e) {
				Console.Write(e.Message);
			}
        }
        public static void UpdateGeoCodingComm3() {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "SELECT l1.[OriginatingSystemKey], [UnparsedAddress] " +
					"FROM [listings-residential-onekey] l1 LEFT JOIN [listings-geo] l2 ON l1.[OriginatingSystemKey] = l2.[MLSNumber] " +
					"WHERE l2.[MLSNumber] IS NULL";
                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while(dr.Read()) {
                        BingGeolocation? bl = BingGeocoder.ResolveAddress(dr[1].ToString());
                        if(bl != null) {
                            float lat = 0;
                            float lon = 0;
                            float.TryParse(bl.Value.Lat.ToString(), out lat);
                            float.TryParse(bl.Value.Lon.ToString(), out lon);
                            if(lat != 0 && lon != 0) {
                                UpdateLatLong3(dr[0].ToString(), lat, lon);
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
            }
        }
        public static void UpdateGeoCodingLand3() {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "SELECT l1.[MLSNumber], [StreetNumber], [StreetNumberModifier], " +
                    "StreetDirPrefix, [StreetName], StreetSuffix, StreetDirSuffix, City, " +
                    "[StateOrProvince], PostalCode FROM [listings-land-3] l1 LEFT JOIN [listings-geo] l2 ON l1.[MLSNumber] = l2.[MLSNumber] WHERE l2.[MLSNumber] IS NULL";
                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while(dr.Read()) {
                        BingGeolocation? bl = BingGeocoder.ResolveAddress(dr[1].ToString() + " " + dr[2].ToString() + " " + dr[3].ToString() + " " + dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString(), dr[7].ToString(), dr[8].ToString(), dr[9].ToString(), "US");
                        if(bl != null) {
                            float lat = 0;
                            float lon = 0;
                            float.TryParse(bl.Value.Lat.ToString(), out lat);
                            float.TryParse(bl.Value.Lon.ToString(), out lon);
                            if(lat != 0 && lon != 0) {
                                UpdateLatLong3(dr[0].ToString(), lat, lon);
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
            }
        }

        private static void UpdateLatLong(float MLS, float Lat, float Long) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("INSERT INTO [listings-residential-ext] ([MLS #], Lat, Long) VALUES (@MLS, @Lat, @Long)", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("MLS", SqlDbType.Float).Value = MLS;
					cmd.Parameters.Add("Lat", SqlDbType.Float).Value = Lat;
					cmd.Parameters.Add("Long", SqlDbType.Float).Value = Long;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
        private static void UpdateLatLong3(string MLS, float Lat, float Long) {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-geo] ([MLSNumber], Lat, Long) VALUES (@MLS, @Lat, @Long)", cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("MLS", SqlDbType.VarChar, 20).Value = MLS;
                    cmd.Parameters.Add("Lat", SqlDbType.Float).Value = Lat;
                    cmd.Parameters.Add("Long", SqlDbType.Float).Value = Long;
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
        }

        /// Bing Maps Geocoder
        /// Url request to
        ///http://dev.virtualearth.net/REST/v1/Locations?countryRegion=countryRegion&adminDistrict=adminDistrict&locality=locality&postalCode=postalCode&addressLine=addressLine&key=BingMapsKey;
        public static BingGeolocation? ResolveAddress(string query) {
			if (string.IsNullOrEmpty(_BingMapsKey)) {
				_BingMapsKey = System.Configuration.ConfigurationManager.AppSettings["BingMapsKey"];
			}

			string url = "http://dev.virtualearth.net/REST/v1/Locations?o=xml&q={0}&key=" + _BingMapsKey;
			url = String.Format(url, query);
			XmlNode coords = null;

			try {
				string xmlString = GetUrl(url);
				XmlDocument xd = new XmlDocument();
				xd.LoadXml(xmlString);
				XmlNamespaceManager xnm = new XmlNamespaceManager(xd.NameTable);
				coords = xd.GetElementsByTagName("Point")[0];
			} catch { }

			BingGeolocation? gl = null;
			if (coords != null && coords.HasChildNodes) {
				string Lat = coords.ChildNodes[0].InnerText;
				string Lon = coords.ChildNodes[1].InnerText;
				gl = new BingGeolocation(Convert.ToDecimal(Lat, System.Globalization.CultureInfo.InvariantCulture), Convert.ToDecimal(Lon, System.Globalization.CultureInfo.InvariantCulture));
			}

			return gl;
		}

		/// <summary>
		/// Returns the Lat
		/// </summary>
		/// <param name="address"></param>
		/// <param name="city"></param>
		/// <param name="state"></param>
		/// <param name="postcode"></param>
		/// <param name="country"></param>
		/// <returns></returns>
		public static BingGeolocation? ResolveAddress(string address, string city, string state, string postcode, string country) {
			return ResolveAddress("&addressLine=" + HttpUtility.UrlEncode(address) + "&locality=" + HttpUtility.UrlEncode(city) + "&adminDistrict=" + state + "&postalCode=" + postcode + "&countryRegion=" + country);
		}

		///<summary>
		/// Retrieve a Url via WebClient
		///
		public static string GetUrl(string url) {
			string result = string.Empty;
			System.Net.WebClient Client = new WebClient();
			using (Stream strm = Client.OpenRead(url)) {
				StreamReader sr = new StreamReader(strm);
				result = sr.ReadToEnd();
			}
			return result;
		}
	}

	public struct BingGeolocation {
		public decimal Lat;
		public decimal Lon;
		public BingGeolocation(decimal lat, decimal lon) {
			Lat = lat;
			Lon = lon;
		}

		public override string ToString() {
			return "Latitude: " + Lat.ToString() + " Longitude: " + Lon.ToString();
		}

		public string ToQueryString() {
			return "+to:" + Lat + "%2B" + Lon;
		}
	}
}

