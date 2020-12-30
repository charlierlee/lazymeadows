using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;


namespace TKS.Models.realestate {
	public class Listing3 {

        #region Constructor
        public Listing3() { }
        public Listing3(string mls) {
            MLS = mls;
            try {
                using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                    string SQL = "SELECT [AddFeeFrequency], [AdditionalFeeDes], [AdditionalFeesAmt], " +
                        "[Adult55Community], [AirConditioning], [AlternateMLSNumber], " +
                        "[Amenities], [AtticDescription], [BasementDescription], " +
                        "[BathsFull], [BathsHalf], [BathsTotal], " +
                        "[BedsTotal], [City], [CloseDate], " +
                        "[ComplexName], [ConstructionDescription], [ContractDate], " +
                        "[CountyOrParish], [CurrentPrice], [ElementarySchool], " +
                        "[Fireplacesnumberof], [Garbage], [Hamlet], " +
                        "[HeatingFuel], [HeatingType], [HeatingZonesNumof], " +
                        "[HighSchool], [HoaFeeIncludes], [Hotwater], " +
                        "[Included], [Junior_MiddleHighSchool], [LeasePrice], " +
                        "[ListAgentDirectWorkPhone], [ListAgentEmail], [ListAgentFullName], " +
                        "[ListAgentMLSID], [LotDescription], [LotSizeArea], " +
                        "[LotSizeAreaSQFT], [MarketingRemarks], [Matrix_Unique_ID], " +
                        "l1.[MLSNumber], [Model], [MonthlyHOAFee], " +
                        "[NumOfLevels], [OpenHouseUpcoming], [Parking], " +
                        "[PhotoCount], [PostalCode], [PostalCodePlus4], " +
                        "[PropertyType], [PUD], [REO_BankOwned], " +
                        "[RoomCount], [SchoolDistrict], [SewerDescription], " +
                        "[SidingDescription], [SqFtSource], [SqFtTotal], " +
                        "[StateOrProvince], [Status], [StreetDirPrefix], " +
                        "[StreetDirSuffix], [StreetName], [StreetNumber], " +
                        "[StreetNumberModifier], [StreetSuffix], [StreetType], " +
                        "[Style], [Subdivision_Development], [TaxAmount], " +
                        "[TaxSource], [TaxYear], [TotalRoomsFinished], " +
                        "[UnitCount], [UnitNumber], [Village], " +
                        "[VirtualTourLink], [WaterAccessYN], [WaterDescription], " +
                        "[YearBuilt], [YearBuiltException], [YearRenovated], " +
                        "[Zoning], ISNULL(l2.[Lat], 0), ISNULL(l2.[Long], 0)  " +
                        "FROM [dbo].[listings-residential-3] l1 LEFT JOIN [listings-geo] l2 ON l1.[MLSNumber] = l2.[MLSNumber] " +
                        "WHERE l1.[MLSNumber] = @MLS";

                    using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("MLS", SqlDbType.VarChar, 20).Value = MLS;
                        cmd.Connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        if(dr.HasRows) {
                            dr.Read();
                            if(dr[2].ToString() != "-1") { AdditionalFees = string.Format("{0:$#,###}", dr[2]) + " "; }
                            AdditionalFees += (" " + dr[0].ToString() + " " + dr[1].ToString()).Trim();
                            Adult55Community = dr.GetBoolean(3);
                            AirConditioning = dr[4].ToString();
                            FeaturesAmenities = dr[6].ToString();
                            Attic = dr[7].ToString();
                            Basement = dr[8].ToString();
                            FullBaths = dr[9].ToString();
                            HalfBaths = dr[10].ToString();
                            if(dr[12].ToString() != "-1") { Bedrooms = dr[12].ToString(); }
                            City = dr[13].ToString();
                            Development = (dr[15].ToString() + " " + dr[70].ToString()).Trim();
                            Construction = dr[16].ToString();
                            County = dr[18].ToString();
                            float aa = 0;
                            float.TryParse(dr[19].ToString(), out aa);
                            AskingAmt = aa;
                            AskingPrice = string.Format("{0:$#,###}", dr[19]);
                            if(dr[21].ToString() != "-1" && dr[21].ToString() != "0") {
                                Fireplace = dr[21].ToString();
                            }
                            Garbage = dr[22].ToString();
                            Fuel = dr[24].ToString();
                            Heating = dr[25].ToString();
                            if(dr[26].ToString() != "-1") { HeatingZones = dr[26].ToString(); }
                            HOAFeeIncludes = dr[28].ToString();
                            WaterHeater = dr[29].ToString();
                            Included = dr[30].ToString();
                            AgentPhone = dr[33].ToString();
                            AgentName = dr[35].ToString();
                            AgentID = dr[36].ToString();
                            LandFeatures = dr[37].ToString();
                            if(dr[38].ToString() != "-1") { Acres = dr[38].ToString(); }
                            if(dr[39].ToString() != "-1") { LotSquareFeet = dr[39].ToString(); }
                            Remarks = dr[40].ToString();
                            MatrixUnique = dr[41].ToString();
                            MLS = dr[42].ToString();
                            Model = dr[43].ToString();
                            if(dr.GetInt32(44) > 0) { HomeownersFees = string.Format("{0:$#,###}", dr[44]); }
                            if(dr[45].ToString() != "-1") { Levels = dr[45].ToString(); }
                            Parking = dr[47].ToString();
                            PhotoCount = dr.GetInt32(48);
                            Zip = dr[49].ToString();
                            if(dr[50].ToString().Length > 0) { Zip += "-" + dr[50].ToString(); }
                            Type = dr[51].ToString();
                            PUD = dr.GetBoolean(52);
                            REO = dr.GetBoolean(53);
                            if(dr.GetInt32(54) > -1) { Rooms = dr[54].ToString(); }
                            SchoolDistrict = dr[55].ToString();
                            Sewer = dr[56].ToString();
                            Exterior = dr[57].ToString();
                            if(dr[59].ToString() != "-1") { TotalSquareFeet = string.Format("{0:#,###}", dr[59]); }
                            State = dr[60].ToString();
                            Status = dr[61].ToString();
                            if(dr[65].ToString() != "") {
                                AddressStreet = dr[65].ToString() + " ";
                            }
                            if(dr[66].ToString() != "") {
                                AddressStreet = (AddressStreet ?? "").Trim() +  dr[66].ToString() + " ";
                            }
                            if(dr[62].ToString() != "") {
                                AddressStreet += dr[62].ToString() + " ";
                            }
                            if(dr[64].ToString() != "") {
                                AddressStreet += dr[64].ToString() + " ";
                            }
                            if(dr[63].ToString() != "") {
                                AddressStreet += dr[63].ToString() + " ";
                            }
                            if(dr[67].ToString() != "") {
                                AddressStreet += dr[67].ToString();
                            }
                            AddressStreet = (AddressStreet ?? "").Trim();
                            Road = dr[68].ToString();
                            Style = dr[69].ToString();
                            if(dr[71].ToString() != "-1") { TotalTaxes = string.Format("{0:$#,###}", dr[71]); }
                            if(dr[73].ToString() != "-1") { TaxYear = dr[73].ToString(); }
                            RoomsFinished = dr[74].ToString();
                            VirtualTour = dr[78].ToString();
                            WaterAccess = dr.GetBoolean(79);
                            Water = dr[80].ToString();
                            if(dr[81].ToString() != "-1") { YearBuilt = dr[81].ToString(); }
                            Lat = (float)dr.GetDouble(85);
                            Long = (float)dr.GetDouble(86);
                        } else {
                            MLS = "";
                        }
                        cmd.Connection.Close();

                        //PaymentCalculator calculator = new PaymentCalculator();
                        //calculator.PurchasePrice = Convert.ToDecimal(AskingAmt);
                        //calculator.InterestRate = Convert.ToDecimal(6.0);
                        //calculator.LoanTermYears = 5;
                        //for (int i = 0; i <= 10000; i += 1000) {
                        //	calculator.DownPayment = i;
                        //}
                        //Console.WriteLine("Purchase Price: {0:C}", calculator.PurchasePrice);
                        //Console.WriteLine("Down Payment: {0:C}", calculator.DownPayment);
                        //Console.WriteLine("Loan Amount: {0:C}", calculator.LoanAmount);
                        //Console.WriteLine("Annual Interest Rate: {0}%", calculator.InterestRate);
                        //Console.WriteLine("Term: {0} years ({1} months)", calculator.LoanTermYears, calculator.LoanTermMonths);
                        //Console.WriteLine("Monthly Payment: {0:C}", calculator.CalculatePayment());
                    }
                }
            } catch { }
        }
        #endregion

        #region Public Methods
        public string GetTags() {
			StringBuilder sb = new StringBuilder();
			string URL = Global.BaseURL + "/detail?mls=" + MLS;

			//Meta tags
			sb.AppendLine(string.Format("<title>{0}</title>", this.AddressOneLine + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta name='description' content=\"{0}\" />", this.ShortDescription(150)));

			//OG / Facebook tags
			sb.AppendLine(string.Format("<meta property='og:title' content=\"{0}\" />", this.AddressOneLine + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta property='og:description' content=\"{0}\" />", this.ShortDescription(150)));
			sb.AppendLine("<meta property='og:type' content='website' />");
			sb.AppendLine(string.Format("<meta property='og:url' content=\"{0}\" />", URL));
			//sb.AppendLine(string.Format("<meta property='og:image' content=\"{0}\" />", Global.BaseURL + this.ogImage));
			sb.AppendLine(string.Format("<meta property='fb:admins' content=\"{0}\" />", Global.FacebookAdmin));

			//Google+ tags
			//if (!string.IsNullOrEmpty(this.gAuthor)) { sb.AppendLine(string.Format("<link rel='author' href=\"{0}\" />", this.gAuthor)); }
			//if (!string.IsNullOrEmpty(this.gPublisher)) { sb.AppendLine(string.Format("<link rel='publisher' href=\"{0}\" />", this.gPublisher)); }
			sb.AppendLine(string.Format("<meta itemprop='name' content=\"{0}\" />", this.AddressOneLine + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta itemprop='description' content=\"{0}\" />", this.ShortDescription(150)));
			//if (!string.IsNullOrEmpty(this.gImage)) {
			//	sb.AppendLine(string.Format("<meta itemprop='image' content=\"{0}\" />", Global.BaseURL + this.gImage));
			//} else if (!string.IsNullOrEmpty(this.ogImage)) {
			//	sb.AppendLine(string.Format("<meta itemprop='image' content=\"{0}\" />", Global.BaseURL + this.gImage));
			//}

			//Twitter tags
			sb.AppendLine("<meta name='twitter:card' content='summary'>");
			sb.AppendLine(string.Format("<meta name='twitter:title' content=\"{0}\" />", this.AddressOneLine + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta name='twitter:description' content=\"{0}\" />", this.ShortDescription(150)));
			//if (!string.IsNullOrEmpty(this.twitterSite)) {
			//	sb.AppendLine(string.Format("<meta name='twitter:site' content=\"{0}\" />", this.twitterSite));
			//}
			//if (!string.IsNullOrEmpty(this.twitterCreator)) {
			//	sb.AppendLine(string.Format("<meta name='twitter:creator' content=\"{0}\" />", this.twitterCreator));
			//}
			//if (!string.IsNullOrEmpty(this.twitterImage)) {
			//	sb.AppendLine(string.Format("<meta name='twitter:image' content=\"{0}\" />", Global.BaseURL + this.twitterImage));
			//} else if (!string.IsNullOrEmpty(this.ogImage)) {
			//	sb.AppendLine(string.Format("<meta name='twitter:image' content=\"{0}\" />", Global.BaseURL + this.ogImage));
			//} else if (!string.IsNullOrEmpty(this.gImage)) {
			//	sb.AppendLine(string.Format("<meta name='twitter:image' content=\"{0}\" />", Global.BaseURL + this.gImage));
			//}

			//Other tags
			if (!string.IsNullOrEmpty(Global.RSSFeedURL) && Global.RSSFeedURL != Global.BaseURL) {
				sb.AppendLine("<link rel='alternate' type='application/rss+xml' title='" + Global.FeedTitle + "'  href='" + Global.RSSFeedURL + "' />");
			}

			if (!string.IsNullOrEmpty(Global.SiteName) || !string.IsNullOrEmpty(Global.SiteNameAlternate)) {
				if (!string.IsNullOrEmpty(Global.SiteName) && !string.IsNullOrEmpty(Global.SiteNameAlternate)) {
					sb.AppendLine("<script type='application/ld+json'>" +
									"{  \"@context\" : \"http://schema.org\", " +
									   "\"@type\" : \"WebSite\", " +
									   "\"name\" : \"" + Global.SiteName + "\", " +
									   "\"alternateName\" : \"" + Global.SiteNameAlternate + "\", " +
									   "\"url\" : \"" + URL + "\" }</script>");
				}
			}

			return sb.ToString();
		}
		public string ShortDescription(int words) {
			return TruncateAtWord(Remarks, words);
		}
		private string TruncateAtWord(string value, int length) {
			if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
				return value;

			return value.Substring(0, value.IndexOf(" ", length));
		}
        #endregion

        #region Public Properties
        public string AdditionalFees { get; set; }
        public bool Adult55Community { get; set; }
        public string AirConditioning { get; set; }
        public string FeaturesAmenities { get; set; }
        public string Attic { get; set; }
        public string Basement { get; set; }
        public string FullBaths { get; set; }
        public string HalfBaths { get; set; }
        public string TotalBaths {
            get {
                if(FullBaths != "-1" && HalfBaths != "-1") {
                    return FullBaths + " full, " + HalfBaths + " half";
                } else if(FullBaths != "-1") {
                    return FullBaths + " full";
                } else if(HalfBaths != "-1") {
                    return HalfBaths + " half";
                } else {
                    return "";
                }
            }
        }
        public string Bedrooms { get; set; }
        public string City { get; set; }
        public string Development { get; set; }
        public string Construction { get; set; }
        public string County { get; set; }
        public float AskingAmt { get; set; }
        public string AskingPrice { get; set; }
        public string Fireplace { get; set; }
        public string Garbage { get; set; }
        public string Fuel { get; set; }
        public string Heating { get; set; }
        public string HeatingZones { get; set; }
        public string HOAFeeIncludes { get; set; }
        public string WaterHeater { get; set; }
        public string Included { get; set; }
        public string AgentPhone { get; set; }
        public string AgentID { get; set; }
        public string AgentName { get; set; }
        public string LandFeatures { get; set; }
        public string Acres { get; set; }
        public string LotSquareFeet { get; set; }
        public string Remarks { get; set; }
        public string MatrixUnique { get; set; }
        public string MLS { get; set; }
        public string Model { get; set; }
        public string HomeownersFees { get; set; }
        public string Levels { get; set; }
        public string Parking { get; set; }
        public int PhotoCount { get; set; }
        public string Zip { get; set; }
        public string Type { get; set; }
        public bool PUD { get; set; }
        public bool REO { get; set; }
        public string Rooms { get; set; }
        public string RoomsFinished { get; set; }
        public string SchoolDistrict { get; set; }
        public string Sewer { get; set; }
        public string Exterior { get; set; }
        public string TotalSquareFeet { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string AddressStreet { get; set; }
        public string Road { get; set; }
        public string Style { get; set; }
        public string TotalTaxes { get; set; }
        public string TaxYear { get; set; }
        public string VirtualTour { get; set; }
        public bool WaterAccess { get; set; }
        public string Water { get; set; }
        public string YearBuilt { get; set; }


        public string Class { get; set; }
		public string Township { get; set; }
		public float DownpaymentPercent { get; set; }
		public float Percent30yr { get; set; }
		public float Percent15yr { get; set; }
		public float PercentARM { get; set; }
		public string Downpayment {
			get {
				if (AskingAmt > 0 && DownpaymentPercent > 0) {
					return string.Format("{0:$#,###}", AskingAmt * DownpaymentPercent / 100);
				} else {
					return "";
				}
			}
		}
		public string Payment30yr {
			get {
				if (AskingAmt > 0 && Percent30yr > 0) {
					PaymentCalculator calculator = new PaymentCalculator();
					if (DownpaymentPercent > 0) {
						calculator.DownPayment = Convert.ToDecimal(AskingAmt) * Convert.ToDecimal(DownpaymentPercent) / 100;
					}
					calculator.PurchasePrice = Convert.ToDecimal(AskingAmt);
					calculator.InterestRate = Convert.ToDecimal(Percent30yr);
					calculator.LoanTermYears = 30;
					double temp = calculator.CalculatePayment();
					Payment30yrTotal = string.Format("{0:c}", temp * 360);
					return string.Format("{0:c}", temp);
				} else {
					return "";
				}
			}
		}
		public string Payment30yrTotal { get; set; }
		public string Payment15yr {
			get {
				if (AskingAmt > 0 && Percent15yr > 0) {
					PaymentCalculator calculator = new PaymentCalculator();
					if (DownpaymentPercent > 0) {
						calculator.DownPayment = Convert.ToDecimal(AskingAmt) * Convert.ToDecimal(DownpaymentPercent) / 100;
					}
					calculator.PurchasePrice = Convert.ToDecimal(AskingAmt);
					calculator.InterestRate = Convert.ToDecimal(Percent15yr);
					calculator.LoanTermYears = 15;
					double temp = calculator.CalculatePayment();
					Payment15yrTotal = string.Format("{0:c}", temp * 180);
					return string.Format("{0:c}", temp);
				} else {
					return "";
				}
			}
		}
		public string Payment15yrTotal { get; set; }
		public string PaymentARM {
			get {
				if (AskingAmt > 0 && PercentARM > 0) {
					PaymentCalculator calculator = new PaymentCalculator();
					if (DownpaymentPercent > 0) {
						calculator.DownPayment = Convert.ToDecimal(AskingAmt) * Convert.ToDecimal(DownpaymentPercent) / 100;
					}
					calculator.PurchasePrice = Convert.ToDecimal(AskingAmt);
					calculator.InterestRate = Convert.ToDecimal(PercentARM);
					calculator.LoanTermYears = 30;
					double temp = calculator.CalculatePayment();
					PaymentARMTotal = string.Format("{0:c}", temp * 360);
					return string.Format("{0:c}", temp);
				} else {
					return "";
				}
			}
		}
		public string PaymentARMTotal { get; set; }
		public string AddressOneLine {
			get {
				string ret = AddressStreet + "";
				if (!string.IsNullOrEmpty(City)) {
					if (ret.Length > 0) {
						ret += ", " + City + " ";
					} else {
						ret += City + " ";
					}
				}
				if (!string.IsNullOrEmpty(State)) {
					ret += State + " ";
				}
				if (!string.IsNullOrEmpty(Zip)) {
					ret += Zip + " ";
				}
				if (ret.Length > 0) { ret = ret.Substring(0, ret.Length - 1); }
				return ret;
			}
		}

		public float Lat { get; set; }
		public float Long { get; set; }

		public string ImageFirst {
			get {
                if(PhotoCount > 0) {
                    return "http://www.lazymeadowsrealty.com/img/listings/XLargePhoto" + MatrixUnique.ToString() + "-1.jpeg";
                } else {
                    return "";
                }
            }
        }
		public string[] Images {
			get {
                string[] imgs = new string[PhotoCount];
                for (int x = 1; x <= PhotoCount; x++) {
                    imgs[x - 1] = "http://www.lazymeadowsrealty.com/img/listings/XLargePhoto" + MatrixUnique.ToString() + "-" + x.ToString() + ".jpeg";
                }
                return imgs;
			}
		}

		public string ImageURL { get; set; }
		public string LinkURL { get; set; }
		public string FavoriteDescription { get; set; }

		#endregion

		public List<SelectListItem> GetAcreSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			for (int x = 1; x <= 10; x++) {
				itemList.Add(new SelectListItem {
					Value = x.ToString(),
					Text = x.ToString(),
					Selected = (x.ToString() == SelectedID)
				});
			}
			return itemList;
		}
		public List<SelectListItem> GetCitySelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT DISTINCT City FROM [listings-residential-3] WHERE City <> '' ORDER BY City";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							itemList.Add(new SelectListItem {
								Value = dr[0].ToString(),
								Text = dr[0].ToString(),
								Selected = (dr[0].ToString() == SelectedID)
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return itemList;
		}
		public List<SelectListItem> GetCitySelectList(string[] SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT DISTINCT City FROM [listings-residential-3] WHERE City <> '' ORDER BY City";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = System.Data.CommandType.Text;
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						while (dr.Read()) {
							bool selected = SelectedID != null && SelectedID.Contains(dr[0].ToString(), StringComparer.OrdinalIgnoreCase);
							itemList.Add(new SelectListItem {
								Value = dr[0].ToString(),
								Text = dr[0].ToString(),
								Selected = selected
							});
						}
						cmd.Connection.Close();
					}
				}
			} catch (Exception) {
			}
			return itemList;
		}
		public List<SelectListItem> GetBathroomSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			for (int x = 1; x <= 5; x++) {
				itemList.Add(new SelectListItem {
					Value = x.ToString(),
					Text = x.ToString(),
					Selected = (x.ToString() == SelectedID)
				});
			}
			return itemList;
		}
		public List<SelectListItem> GetBedroomSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			for (int x = 1; x <= 5; x++) {
				itemList.Add(new SelectListItem {
					Value = x.ToString(),
					Text = x.ToString(),
					Selected = (x.ToString() == SelectedID)
				});
			}
			return itemList;
		}
		public List<SelectListItem> GetPriceSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			for (int x = 1; x <= 20; x++) {
				itemList.Add(new SelectListItem {
					Value = (x * 100).ToString(),
					Text = string.Format("{0:$0,0}", (x * 100000)),
					Selected = ((x * 100).ToString() == SelectedID)
				});
			}
			return itemList;
		}
		public List<SelectListItem> GetSqFtSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			for (int x = 1; x <= 20; x++) {
				itemList.Add(new SelectListItem {
					Value = (x * 500).ToString(),
					Text = string.Format("{0:0,0}", (x * 500)),
					Selected = ((x * 500).ToString() == SelectedID)
				});
			}
			return itemList;
		}
		public List<SelectListItem> GetYearsSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			itemList.Add(new SelectListItem { Value = "0", Text = "New", Selected = ("0" == SelectedID) });
			itemList.Add(new SelectListItem { Value = "1", Text = "1 Year", Selected = ("1" == SelectedID) });
			for (int x = 2; x <= 5; x++) {
				itemList.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() + " Years", Selected = (x.ToString() == SelectedID) });
			}
			for (int x = 10; x <= 100; x = x + 5) {
				itemList.Add(new SelectListItem { Value = x.ToString(), Text = x.ToString() + " Years", Selected = (x.ToString() == SelectedID) });
			}

			return itemList;
		}
	}

    public class Listings3 {
		public SearchResult3 DoSearch(SearchModel data, int page = 1, int PostsPerPage = 12) {
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string param = "";
			string where = "";
			SearchResult3 searchResult = new SearchResult3();
			List<Listing3> listings = new List<Listing3>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string[] TownList = GetRelatedTowns(data.City);
                if(data.City != null) {
                    foreach(string city in data.City) {
                        param += "&city=" + HttpUtility.UrlEncode(city);
                    }
                }
                if(TownList != null && !string.IsNullOrEmpty(TownList[0])) {
					where += "AND (City = @City1 ";
					for (int x = 2; x <= TownList.Length; x++) {
						where += " OR City = @City" + x.ToString();
					}
					where += ") ";
				}
				if (data.Acres > 0) { where += "AND [LotSizeArea] >= @Acres "; }
				if (data.Acres2 > 0) { where += "AND [LotSizeArea] <= @Acres2 "; }
				if (data.Bathrooms > 0) { where += "AND [BathsFull] >= @Bathrooms "; }
				if (data.Bathrooms2 > 0) { where += "AND [BathsFull] <= @Bathrooms2 "; }
				if (data.Bedrooms > 0) { where += "AND [BedsTotal] >= @Bedrooms "; }
				if (data.Bedrooms2 > 0) { where += "AND [BedsTotal] <= @Bedrooms2 "; }
				if (data.MinPrice > 0) { where += "AND [CurrentPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { where += "AND [CurrentPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { where += "AND [SqFtTotal] >= @SqFt "; }
				if (data.SqFt2 > 0) { where += "AND [SqFtTotal] <= @SqFt2 "; }
				if (data.Years > 0) { where += "AND [YearBuilt] <= YEAR(GetDate()) - @Years "; }
				if (data.Years2 > 0) { where += "AND [YearBuilt] >= YEAR(GetDate()) - @Years2 "; }
				if (data.Pool == 1) { where += "AND Amenities LIKE '%pool%' "; }
				if (data.Fireplace == 1) { where += "AND [Fireplacesnumberof] > 0 "; }
                if (data.Barn == 1) { where += "AND Amenities LIKE '%barn%' "; }
                if (data.Handicap == 1) { where += "AND Amenities LIKE '%ada%' "; }
                if (data.Skylights == 1) { where += "AND (Amenities LIKE '%skylight%' OR Amenities LIKE '%sky light%') "; }
                if (data.Lake == 1) { where += "AND (Amenities LIKE '%lake%' OR WaterAccessYN = 1) "; }
                string SQL = "SELECT COUNT(*) FROM [listings-residential-3] WHERE Status = 'A' " + where;

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (TownList != null && !string.IsNullOrEmpty(TownList[0])) {
						for (int x = 1; x <= TownList.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = TownList[x - 1];
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.Bathrooms > 0) { cmd.Parameters.Add("Bathrooms", SqlDbType.Float).Value = data.Bathrooms; param += "&bathrooms=" + data.Bathrooms.ToString(); }
					if (data.Bathrooms2 > 0) { cmd.Parameters.Add("Bathrooms2", SqlDbType.Float).Value = data.Bathrooms2; param += "&bathrooms2=" + data.Bathrooms2.ToString(); }
					if (data.Bedrooms > 0) { cmd.Parameters.Add("Bedrooms", SqlDbType.Float).Value = data.Bedrooms; param += "&bedrooms=" + data.Bedrooms.ToString(); }
					if (data.Bedrooms2 > 0) { cmd.Parameters.Add("Bedrooms2", SqlDbType.Float).Value = data.Bedrooms2; param += "&bedrooms2=" + data.Bedrooms2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.SqFt > 0) { cmd.Parameters.Add("SqFt", SqlDbType.Float).Value = data.SqFt; param += "&sqft=" + data.SqFt.ToString(); }
					if (data.SqFt2 > 0) { cmd.Parameters.Add("SqFt2", SqlDbType.Float).Value = data.SqFt2; param += "&sqft2=" + data.SqFt2.ToString(); }
					if (data.Years > 0) { cmd.Parameters.Add("Years", SqlDbType.Float).Value = data.Years; param += "&years=" + data.Years.ToString(); }
					if (data.Years2 > 0) { cmd.Parameters.Add("Years2", SqlDbType.Float).Value = data.Years2; param += "&years2=" + data.Years2.ToString(); }
					if (data.Pool == 1) { param += "&pool=1"; }
					if (data.Fireplace == 1) { param += "&fireplace=1"; }
					if (data.Barn == 1) { param += "&barn=1"; }
					if (data.Handicap == 1) { param += "&handicap=1"; }
					if (data.Skylights == 1) { param += "&skylights=1"; }
					if (data.Lake == 1) { param += "&lake=1"; }

					cmd.Connection.Open();
					searchResult.TotalResults = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();

					int maxPages = (int)Math.Ceiling(Convert.ToDouble(searchResult.TotalResults) / PostsPerPage);// (searchResult.TotalResults / PostsPerPage) + 1;
					if (page > maxPages) {
						start = ((maxPages - 1) * PostsPerPage) + 1;
						end = (maxPages * PostsPerPage);
						searchResult.NextPage = "";
					} else if (page == maxPages) {
						searchResult.NextPage = "";
					} else {
						searchResult.NextPage = "/search?" + param + "&page=" + (page + 1).ToString();
					}
					if (page == 1) {
						searchResult.PrevPage = "";
					} else {
						searchResult.PrevPage = "/search?" + param + "&page=" + (page - 1).ToString();
					}

					SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY [CurrentPrice] DESC, City) AS Rownumber, [MLSNumber] FROM [listings-residential-3] WHERE Status = 'A' " + where;
					SQL += ") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();

					cmd.CommandText = SQL;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new Listing3(dr[1].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			searchResult.Listings = listings;
			return searchResult;
		}
		public List<Listing3> GetFeatured() {
			List<Listing3> listings = new List<Listing3>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM Favorites WHERE IsActive = 1 ORDER BY SortOrder, Town, MLS", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						if (!string.IsNullOrEmpty(dr["MLS"].ToString())) {
							Listing3 listing = new Listing3(dr["MLS"].ToString());
							if (!string.IsNullOrEmpty(listing.MLS)) {
								listing.LinkURL = "/detail?mls=" + listing.MLS;
								listing.ImageURL = listing.ImageFirst;
								listing.FavoriteDescription = listing.ShortDescription(150) + "...";
								listings.Add(listing);
							}
						} else {
							Listing3 listing = new Listing3();
							listing.City = dr["Town"].ToString();
							listing.AskingPrice = dr["Price"].ToString();
							listing.FavoriteDescription = dr["Description"].ToString();
							listing.LinkURL = dr["Link"].ToString();
							listing.ImageURL = "/img/favorites/" + dr["FavoriteID"].ToString() + "/" + dr["Photo"].ToString();
							listings.Add(listing);
						}
					}
					cmd.Connection.Close();
				}
			}
			//string[] favs = new TKS.Areas.Admin.Models.GlobalLM().Favorites.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			//foreach (string fav in favs) {
			//Listing listing = new Listing(fav.Trim());
			//if (!string.IsNullOrEmpty(listing.MLS)) {
			//    listings.Add(listing);
			//}
			//}

			return listings;
		}
		public List<Listing3> GetMapListings(SearchModel data) {
			string param = "";
			List<Listing3> listings = new List<Listing3>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT l1.[MLSNumber] FROM [listings-residential-3] l1 JOIN [listings-geo] l2 ON l1.[MLSNumber] = l2.[MLSNumber] WHERE l1.Status = 'A' ";
				string[] TownList = GetRelatedTowns(data.City);
				if (TownList != null && !string.IsNullOrEmpty(TownList[0])) {
					SQL += "AND (City = @City1 ";
					for (int x = 2; x <= TownList.Length; x++) {
						SQL += " OR City = @City" + x.ToString();
					}
					SQL += ") ";
				}

                if(data.Acres > 0) { SQL += "AND [LotSizeArea] >= @Acres "; }
				if (data.Acres2 > 0) { SQL += "AND [LotSizeArea] <= @Acres2 "; }
				if (data.Bathrooms > 0) { SQL += "AND [BathsFull] >= @Bathrooms "; }
				if (data.Bathrooms2 > 0) { SQL += "AND [BathsFull] <= @Bathrooms2 "; }
				if (data.Bedrooms > 0) { SQL += "AND [BedsTotal] >= @Bedrooms "; }
				if (data.Bedrooms2 > 0) { SQL += "AND [BedsTotal] <= @Bedrooms2 "; }
				if (data.MinPrice > 0) { SQL += "AND [CurrentPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { SQL += "AND [CurrentPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { SQL += "AND [SqFtTotal] >= @SqFt "; }
				if (data.SqFt2 > 0) { SQL += "AND [SqFtTotal] <= @SqFt2 "; }
				if (data.Years > 0) { SQL += "AND [YearBuilt] <= YEAR(GetDate()) - @Years "; }
				if (data.Years2 > 0) { SQL += "AND [YearBuilt] >= YEAR(GetDate()) - @Years2 "; }
				//if (data.Garage == 1) { SQL += "AND [Garage Capacity] > 0 "; }
				//if (data.Pool == 1) { SQL += "AND ([A-Frame] LIKE '%|250|%' OR [A-Frame] LIKE '%|259|%' OR [A-Frame] LIKE '%|261|%') "; }
				if (data.Fireplace == 1) { SQL += "AND [Fireplacesnumberof] > 0 "; }
				//if (data.Barn == 1) { SQL += "AND ([A-Frame] LIKE '%|248|%' OR [A-Frame] LIKE '%|255|%' OR [A-Frame] LIKE '%|257|%') "; }
				//if (data.Handicap == 1) { SQL += "AND ([A-Frame] LIKE '%|225|%' OR [A-Frame] LIKE '%|256|%' OR [A-Frame] LIKE '%|590|%' OR [A-Frame] LIKE '%|607|%' OR [A-Frame] LIKE '%|608|%' OR [A-Frame] LIKE '%|892|%' OR [A-Frame] LIKE '%|893|%') "; }
				//if (data.Skylights == 1) { SQL += "AND ([A-Frame] LIKE '%|232|%' OR [A-Frame] LIKE '%|906|%') "; }
				//if (data.Lake == 1) { SQL += "AND ([A-Frame] LIKE '%|190|%' OR [A-Frame] LIKE '%|191|%' OR [A-Frame] LIKE '%|197|%' OR [A-Frame] LIKE '%|198|%' OR [A-Frame] LIKE '%|201|%' OR [A-Frame] LIKE '%|202|%' OR [A-Frame] LIKE '%|205|%') "; }

				SQL += "ORDER BY [CurrentPrice] DESC, City";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (TownList != null && !string.IsNullOrEmpty(TownList[0])) {
						for (int x = 1; x <= TownList.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = TownList[x - 1];
							param += "&city=" + HttpUtility.UrlEncode(TownList[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.Bathrooms > 0) { cmd.Parameters.Add("Bathrooms", SqlDbType.Float).Value = data.Bathrooms; param += "&bathrooms=" + data.Bathrooms.ToString(); }
					if (data.Bathrooms2 > 0) { cmd.Parameters.Add("Bathrooms2", SqlDbType.Float).Value = data.Bathrooms2; param += "&bathrooms2=" + data.Bathrooms2.ToString(); }
					if (data.Bedrooms > 0) { cmd.Parameters.Add("Bedrooms", SqlDbType.Float).Value = data.Bedrooms; param += "&bedrooms=" + data.Bedrooms.ToString(); }
					if (data.Bedrooms2 > 0) { cmd.Parameters.Add("Bedrooms2", SqlDbType.Float).Value = data.Bedrooms2; param += "&bedrooms2=" + data.Bedrooms2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.SqFt > 0) { cmd.Parameters.Add("SqFt", SqlDbType.Float).Value = data.SqFt; param += "&sqft=" + data.SqFt.ToString(); }
					if (data.SqFt2 > 0) { cmd.Parameters.Add("SqFt2", SqlDbType.Float).Value = data.SqFt2; param += "&sqft2=" + data.SqFt2.ToString(); }
					if (data.Years > 0) { cmd.Parameters.Add("Years", SqlDbType.Float).Value = data.Years; param += "&years=" + data.Years.ToString(); }
					if (data.Years2 > 0) { cmd.Parameters.Add("Years2", SqlDbType.Float).Value = data.Years2; param += "&years2=" + data.Years2.ToString(); }
					//if (data.Garage == 1) { param += "&garage=1"; }
					//if (data.Pool == 1) { param += "&pool=1"; }
					if (data.Fireplace == 1) { param += "&fireplace=1"; }
					//if (data.Barn == 1) { param += "&barn=1"; }
					//if (data.Handicap == 1) { param += "&handicap=1"; }
					//if (data.Skylights == 1) { param += "&skylights=1"; }
					//if (data.Lake == 1) { param += "&lake=1"; }

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new Listing3(dr[0].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			return listings;
		}

        public string TranslateAddFeeFrequency(string data) {
            string ret = "";
            switch(data) {
                case "MONT":
                    ret = "Monthly";
                    break;
                case "QTER":
                    ret = "Quarterly";
                    break;
                case "YEAR":
                    ret = "Yearly";
                    break;
                default:
                    break;
            }
            return ret;
        }
        public string TranslateAirCond(string data) {
            if (data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("AIRPUR", "Air Purification System, ");
            data = data.Replace("CENT", "Central, ");
            data = data.Replace("DCTWRK", "Ductwork, ");
            data = data.Replace("DLESS", "Ductless, ");
            data = data.Replace("ESUNTS", "Energy Star Unit(s), ");
            data = data.Replace("GEOTHR", "Geothermal, ");
            data = data.Replace("HPS", "High Pressure System, ");
            data = data.Replace("INDIVIDUAL", "Individual, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("SEER12", "SEER Rating 12 +, ");
            data = data.Replace("UNITS", "Wall Units, ");
            data = data.Replace("WIND", "Window Units, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateAmenities(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("3THRMW", "Triple Thermo Windows, ");
            data = data.Replace("ACCP", "Access Apartments, ");
            data = data.Replace("ADDL", "Additional Land, ");
            data = data.Replace("BALC", "Balcony, ");
            data = data.Replace("BARN", "Barn(s), ");
            data = data.Replace("BBALL", "Basketball Court, ");
            data = data.Replace("BEAC", "Beach, ");
            data = data.Replace("BIKERM", "Bicycle Room, ");
            data = data.Replace("BLDGPOOL", "Building Pool, ");
            data = data.Replace("BROWNSTONE", "Brownstone, ");
            data = data.Replace("BuildingLink", "BuildingLink, ");
            data = data.Replace("BUSCNTR", "Business Center, ");
            data = data.Replace("CHLDPLYRM", "Children Playroom, ");
            data = data.Replace("CITYVIEW", "City View, ");
            data = data.Replace("CLOS", "Close to Railroad, ");
            data = data.Replace("CLPK", "Close to Park, ");
            data = data.Replace("CLSP", "Close to Shops, ");
            data = data.Replace("CLUB", "Clubhouse, ");
            data = data.Replace("COMMOUTDRSPC", "Common Outdoor Space, ");
            data = data.Replace("COURTYARD", "Courtyard, ");
            data = data.Replace("CTBS", "Close to Bus, ");
            data = data.Replace("CTSC", "Close to School, ");
            data = data.Replace("CULDSAC", "Cul-De-Sac, ");
            data = data.Replace("DECK", "Deck, ");
            data = data.Replace("DIPLOMATSOK", "Diplomats OK, ");
            data = data.Replace("DOCK", "Dock/Mooring, ");
            data = data.Replace("DOORMAN", "Doorman, ");
            data = data.Replace("EATK", "Eat in Kitchen, ");
            data = data.Replace("ELEV", "Elevator, ");
            data = data.Replace("ESDOOR", "Energy Star Doors, ");
            data = data.Replace("ESSKLIT", "Energy Star Skylight(s), ");
            data = data.Replace("ESWNDS", "Energy Star Windows, ");
            data = data.Replace("EXRC", "Exercise Room, ");
            data = data.Replace("FDRM", "Formal Dining Room, ");
            data = data.Replace("FENC", "Fencing, ");
            data = data.Replace("FENCED", "Fenced, ");
            data = data.Replace("FENY", "Fenced Yard, ");
            data = data.Replace("FFBD", "1st Floor Bedrm, ");
            data = data.Replace("FFMB", "1st Fl Master Bedroom, ");
            data = data.Replace("FOYER", "Foyer, ");
            data = data.Replace("GARDEN", "Garden, ");
            data = data.Replace("GATE", "Gated Community, ");
            data = data.Replace("GOLF", "Golf Course, ");
            data = data.Replace("GRANIT", "Granite Countertops, ");
            data = data.Replace("GRNHSE", "Greenhouse, ");
            data = data.Replace("GUEST", "Guest Quarters, ");
            data = data.Replace("HANA", "ADA Access, ");
            data = data.Replace("HAND", "ADA Inside, ");
            data = data.Replace("HEALTHCLUB", "Health Club, ");
            data = data.Replace("HISPEEDINTERNET", "High Speed Internet, ");
            data = data.Replace("HORP", "Horse Property, ");
            data = data.Replace("HP", "Heated Parking, ");
            data = data.Replace("HWDFLRS", "Hardwood Floors As Seen, ");
            data = data.Replace("KITCLB", "Kitchen In Clubhouse, ");
            data = data.Replace("LAA", "Legal Accessory Apartment, ");
            data = data.Replace("LAKE", "Lake Association, ");
            data = data.Replace("LAKEFRONT", "Lake Front, ");
            data = data.Replace("LAKERIGHTS", "Lake Rights, ");
            data = data.Replace("LIGHT", "Light, ");
            data = data.Replace("LIVEINSUPER", "Live In Super, ");
            data = data.Replace("LIVEWORK", "Live Work, ");
            data = data.Replace("LNDRYCMN", "Laundry Room/Common, ");
            data = data.Replace("LNDRYPRVT", "Laundry Room/Private, ");
            data = data.Replace("LNDRYSRVC", "Laundry Services, ");
            data = data.Replace("LOFT", "Loft, ");
            data = data.Replace("LOUNGE", "Lounge, ");
            data = data.Replace("LPSP", "Lake/Pond/Stream, ");
            data = data.Replace("LVIEW", "Lake Views, ");
            data = data.Replace("MAIDSERVICE", "Maid Service, ");
            data = data.Replace("MARBLEBTH", "Marble Bath, ");
            data = data.Replace("MBA", "Motor Boats Allowed, ");
            data = data.Replace("MRBLCT", "Marble Countertops, ");
            data = data.Replace("MSTB", "Master Bath, ");
            data = data.Replace("MULTILEVEL", "Multi Level, ");
            data = data.Replace("MVIEW", "Mountain Views, ");
            data = data.Replace("NPT", "Near Public Transportation, ");
            data = data.Replace("NURSERY", "Nursery, ");
            data = data.Replace("OPENKIT", "Open kitchen, ");
            data = data.Replace("OPENVIEW", "Open View, ");
            data = data.Replace("ORIGDETAILS", "Original Details, ");
            data = data.Replace("OUTBLD", "Out Building, ");
            data = data.Replace("OUTDOORSPACE", "Outdoor Space, ");
            data = data.Replace("PARKVIEW", "Park View, ");
            data = data.Replace("PATO", "Patio, ");
            data = data.Replace("PIEDATERRE", "Pied a Terre, ");
            data = data.Replace("PNTRY", "Pantry, ");
            data = data.Replace("POOL", "Community Pool, ");
            data = data.Replace("POOLAG", "Above Ground Pool, ");
            data = data.Replace("POOLIN", "Pool Indoor, ");
            data = data.Replace("POOLING", "In Ground Pool, ");
            data = data.Replace("POOLOUT", "Pool Outdoor, ");
            data = data.Replace("PORCH", "Porch, ");
            data = data.Replace("PRIV", "Privacy, ");
            data = data.Replace("PVTLNDRY", "Private Laundry, ");
            data = data.Replace("PWDR", "Powder Room, ");
            data = data.Replace("RECEIVINGRM", "Receiving Room, ");
            data = data.Replace("RIDR", "Riding Ring, ");
            data = data.Replace("RIVE", "River, ");
            data = data.Replace("ROOFDECK", "Roof Deck, ");
            data = data.Replace("ROOMFORRNT", "Room For Rent, ");
            data = data.Replace("RVIEW", "River Views, ");
            data = data.Replace("SAUN", "Sauna/Steam Room, ");
            data = data.Replace("SECSYS", "Security System, ");
            data = data.Replace("SKYL", "Sky Light, ");
            data = data.Replace("SKYLINGVIEW", "Skyline View, ");
            data = data.Replace("SPA", "Community Spa, ");
            data = data.Replace("SPONSORUNIT", "Sponsor Unit, ");
            data = data.Replace("SPRF", "Sprinkler Fire Sys, ");
            data = data.Replace("SPRK", "Sprinkler Lawn Sys, ");
            data = data.Replace("SROOM", "Steam Room, ");
            data = data.Replace("STAB", "Stable/Paddock, ");
            data = data.Replace("STOR", "Storage, ");
            data = data.Replace("STSTEELAPP", "Stainless Steel Appliances, ");
            data = data.Replace("SUBWAY", "Subway, ");
            data = data.Replace("SVIEW", "Scenic View, ");
            data = data.Replace("TENN", "Tennis, ");
            data = data.Replace("TENNISC", "Community Tennis Courts, ");
            data = data.Replace("TERRACE", "Terrace, ");
            data = data.Replace("TOTLOT", "Tot Lot, ");
            data = data.Replace("TRASH", "Trash Collection, ");
            data = data.Replace("VACRENTAL", "Vacation Rental, ");
            data = data.Replace("VALET", "Valet, ");
            data = data.Replace("VAUL", "Cathedral/Vaulted/High Ceiling, ");
            data = data.Replace("VIEW", "View, ");
            data = data.Replace("VIRTUALDRMN", "Virtual Doorman, ");
            data = data.Replace("WALK", "Walk In Closet, ");
            data = data.Replace("WALO", "Walk Out Basement, ");
            data = data.Replace("WDSTOV", "Wood Burning Stove, ");
            data = data.Replace("WETB", "Wetbar, ");
            data = data.Replace("WIFI", "WiFi, ");
            data = data.Replace("WKSP", "Workshop, ");
            data = data.Replace("WTRA", "Water Access, ");
            data = data.Replace("WTRF", "Waterfront, ");
            data = data.Replace("WTRV", "Water View, ");
            data = data.Replace("WWCRPT", "Wall To Wall Carpet, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateAttic(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COMMON", "Common, ");
            data = data.Replace("CRAWL", "Crawl, ");
            data = data.Replace("DORMER", "Dormer, ");
            data = data.Replace("FINISH", "Finished, ");
            data = data.Replace("FULL", "Full, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("PARTFIN", "Partially Finished, ");
            data = data.Replace("PARTIAL", "Partial, ");
            data = data.Replace("SCUTTLE", "Scuttle, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("STAIRS", "Pull Stairs, ");
            data = data.Replace("STOR", "Storage, ");
            data = data.Replace("UNFNSHD", "Unfinished, ");
            data = data.Replace("WALKUP", "Walkup, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateAvailableUtilities(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("CABLE", "Cable, ");
            data = data.Replace("ELEC", "Electric, ");
            data = data.Replace("GAS", "Gas, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("PHONE", "Phone, ");
            data = data.Replace("SEPTIC", "Septic, ");
            data = data.Replace("SEWER", "Sewer, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("WATER", "Water, ");
            data = data.Replace("WELL", "Well, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }

        public string TranslateBasement(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("BILCODRS", "Bilco Door(s), ");
            data = data.Replace("COMMON", "Common, ");
            data = data.Replace("CRAWL", "Crawl, ");
            data = data.Replace("FINISH", "Finished, ");
            data = data.Replace("FULL", "Full, ");
            data = data.Replace("LND", "Laundry, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("PARTFIN", "Partially Finished, ");
            data = data.Replace("PARTIAL", "Partial, ");
            data = data.Replace("SLAB", "Slab, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("STO", "Storage, ");
            data = data.Replace("UNFNSHD", "Unfinished, ");
            data = data.Replace("UTIL", "Utilities, ");
            data = data.Replace("WALKOUT", "Walk Out, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateCity3(string City, string PostOffice) {
            string toUse = "Unspecified";
            if(City.ToLower() == PostOffice.ToLower()) {
                toUse = City;
                if (toUse.ToUpper() == City) {
                    return City;
                }
            } else {
                if(PostOffice.ToLower() == "calllistingagent" || PostOffice.Trim().Length == 0) {
                    toUse = City;
                    if(toUse.ToUpper() == City) {
                        return City;
                    }
                } else {
                    toUse = PostOffice;
                    if(toUse.ToUpper() == PostOffice) {
                        return PostOffice;
                    }
                }
            }
            if(toUse.ToLower() == "calllistingagent") {
                return "Unspecified";
            }
            //split on capital letters
            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return r.Replace(toUse, " ");

            //if(data == "BayShore") { data = "Bay Shore"; }
            //if(data == "BeaconFalls") { data = "Beacon Falls"; }
            //if(data == "BloomingGrove") { data = "Blooming Grove"; }
            //if(data == "BoltonLanding") { data = "Bolton Landing"; }
            //if(data == "CallListingAgent") { data = "Call Listing Agent"; }
            //if(data == "CambriaHeights") { data = "Cambria Heights"; }
            //if(data == "CampbellHall") { data = "Campbell Hall"; }
            //if(data == "CentralIslip") { data = "Central Islip"; }
            //if(data == "ChesterTown") { data = "Chester Town"; }
            //if(data == "Clermont11") { data = "Clermont 11"; }
            //if(data == "CollegePoint") { data = "College Point"; }
            //if(data == "CopakeFalls") { data = "Copake Falls"; }
            //if(data == "CornwallBridge") { data = "Cornwall Bridge"; }
            //if(data == "CornwallonHudson") { data = "Cornwall on Hudson"; }
            //if(data == "CosCob") { data = "Cos Cob"; }
            //if(data == "DeerPark") { data = "Deerpark"; }
            //if(data == "EastChatham") { data = "East Chatham"; }
            //if(data == "EastElmhurst") { data = "East Elmhurst"; }
            //if(data == "EastFishkill") { data = "East Fishkill"; }
            //if(data == "EastHampton") { data = "East Hampton"; }
            //if(data == "EastMeadow") { data = "East Meadow"; }
            //if(data == "EastNorwalk") { data = "East Norwalk"; }
            //if(data == "EastQuogue") { data = "East Quogue"; }
            //if(data == "FarRockaway") { data = "Far Rockaway"; }
            //if(data == "FloralPark") { data = "Floral Park"; }
            //if(data == "ForestHills") { data = "Forest Hills"; }
            //if(data == "FortEdward") { data = "Fort Edward"; }
            //if(data == "FortMontgomery") { data = "Fort Montgomery"; }
            //if(data == "FranklinSquare") { data = "Franklin Square"; }
            //if(data == "FremontCenter") { data = "Fremont Center"; }
            //if(data == "GlenCove") { data = "Glen Cove"; }
            //if(data == "GlensFalls") { data = "Glens Falls"; }
            //if(data == "GoshenTown") { data = "Goshen Town"; }
            //if(data == "GreatNeck") { data = "Great Neck"; }
            //if(data == "GreenwoodLake") { data = "Greenwood Lake"; }
            //if(data == "HamptonBays") { data = "Hampton Bays"; }
            //if(data == "HaverstrawTown") { data = "Haverstraw Town"; }
            //if(data == "HighFalls") { data = "High Falls"; }
            //if(data == "HighlandFalls") { data = "Highland Falls"; }
            //if(data == "HighlandMills") { data = "Highland Mills"; }
            //if(data == "HowardBeach") { data = "Howard Beach"; }
            //if(data == "HydePark") { data = "Hyde Park"; }
            //if(data == "IndianLake") { data = "Indian Lake"; }
            //if(data == "IslipTerrace") { data = "Islip Terrace"; }
            //if(data == "JacksonHeights") { data = "Jackson Heights"; }
            //if(data == "KewGardens") { data = "Kew Gardens"; }
            //if(data == "KiameshaLake") { data = "Kiamesha Lake"; }
            //if(data == "KingstonCity") { data = "Kingston City"; }
            //if(data == "KingstonTown") { data = "Kingston Town"; }
            //if(data == "LaGrange") { data = "La Grange"; }
            //if(data == "LakeKatrine") { data = "Lake Katrine"; }
            //if(data == "LakeLuzerne") { data = "Lake Luzerne"; }
            //if(data == "LakePlacid") { data = "Lake Placid"; }
            //if(data == "LibertyTown") { data = "Liberty Town"; }
            //if(data == "LittleFalls") { data = "Little Falls"; }
            //if(data == "LittleNeck") { data = "Little Neck"; }
            //if(data == "LochSheldrake") { data = "Loch Sheldrake"; }
            //if(data == "LongBeach") { data = "Long Beach"; }
            //if(data == "LyonsFalls") { data = "Lyons Falls"; }
            //if(data == "MaldenonHudson") { data = "Malden on Hudson"; }
            //if(data == "MiddleVillage") { data = "Middle Village"; }
            //if(data == "MillerPlace") { data = "Miller Place"; }
            //if(data == "MontgomeryTown") { data = "Montgomery Town"; }
            //if(data == "MountHope") { data = "Mount Hope"; }
            //if(data == "MountKisco") { data = "Mount Kisco"; }
            //if(data == "MountPleasant") { data = "Mount Pleasant"; }
            //if(data == "MountVernon") { data = "Mount Vernon"; }
            //if(data == "NewBaltimore") { data = "New Baltimore"; }
            //if(data == "NewBritain") { data = "New Britain"; }
            //if(data == "NewburghCity") { data = "Newburgh City"; }
            //if(data == "NewburghTown") { data = "Newburgh Town"; }
            //if(data == "NewCanaan") { data = "New Canaan"; }
            //if(data == "NewCastle") { data = "New Castle"; }
            //if(data == "NewCity") { data = "New City"; }
            //if(data == "NewFairfield") { data = "New Fairfield"; }
            //if(data == "NewHampton") { data = "New Hampton"; }
            //if(data == "NewHydePark") { data = "New Hyde Park"; }
            //if(data == "NewLebanon") { data = "New Lebanon"; }
            //if(data == "NewMilford") { data = "New Milford"; }
            //if(data == "NewPaltz") { data = "New Paltz"; }
            //if(data == "NewRochelle") { data = "New Rochelle"; }
            //if(data == "NewWindsor") { data = "New Windsor "; }
            //if(data == "NorthCastle") { data = "North Castle"; }
            //if(data == "NorthChatham") { data = "North Chatham"; }
            //if(data == "NorthSalem") { data = "North Salem"; }
            //if(data == "OakHill") { data = "Oak Hill"; }
            //if(data == "OaklandGardens") { data = "Oakland Gardens"; }
            //if(data == "OldChatham") { data = "Old Chatham"; }
            //if(data == "OldGreenwich") { data = "Old Greenwich"; }
            //if(data == "OldTappan") { data = "Old Tappan"; }
            //if(data == "OysterBay") { data = "Oyster Bay"; }
            //if(data == "OzonePark") { data = "Ozone Park"; }
            //if(data == "PineBush") { data = "Pine Bush"; }
            //if(data == "PinePlains") { data = "Pine Plains"; }
            //if(data == "PleasantValley") { data = "Pleasant Valley"; }
            //if(data == "PortEwen") { data = "Port Ewen"; }
            //if(data == "PortJervis") { data = "Port Jervis"; }
            //if(data == "PoughkeepsieCity") { data = "Poughkeepsie City"; }
            //if(data == "PoughkeepsieTown") { data = "Poughkeepsie Town"; }
            //if(data == "PoundRidge") { data = "Pound Ridge"; }
            //if(data == "PrestonHollow") { data = "Preston Hollow"; }
            //if(data == "PutnamValley") { data = "Putnam Valley"; }
            //if(data == "QueensVillage") { data = "Queens Village"; }
            //if(data == "ReddingCenter") { data = "Redding Center"; }
            //if(data == "RedHook") { data = "Red Hook"; }
            //if(data == "RegoPark") { data = "Rego Park"; }
            //if(data == "RichmondHill") { data = "Richmond Hill"; }
            //if(data == "RoslynHeights") { data = "Roslyn Heights"; }
            //if(data == "RyeCity") { data = "Rye City"; }
            //if(data == "RyeTown") { data = "Rye Town"; }
            //if(data == "SagHarbor") { data = "Sag Harbor"; }
            //if(data == "SaintAlbans") { data = "Saint Albans"; }
            //if(data == "SaintJohnsville") { data = "Saint Johnsville"; }
            //if(data == "SalisburyMills") { data = "Salisbury Mills"; }
            //if(data == "SandyHook") { data = "Sandy Hook"; }
            //if(data == "SaratogaSprings") { data = "Saratoga Springs"; }
            //if(data == "SaugertiesTown") { data = "Saugerties Town"; }
            //if(data == "SchroonLake") { data = "Schroon Lake"; }
            //if(data == "SouthNorwalk") { data = "South Norwalk"; }
            //if(data == "SouthOzonePark") { data = "South Ozone Park"; }
            //if(data == "SouthRichmondHill") { data = "South Richmond Hill"; }
            //if(data == "SparrowBush") { data = "Sparrow Bush"; }
            //if(data == "SpringfieldGardens") { data = "Springfield Gardens"; }
            //if(data == "SpringGlen") { data = "Spring Glen"; }
            //if(data == "SpringValley") { data = "Spring Valley"; }
            //if(data == "StoneRidge") { data = "Stone Ridge"; }
            //if(data == "StonyPoint") { data = "Stony Point"; }
            //if(data == "TuxedoPark") { data = "Tuxedo Park"; }
            //if(data == "UlsterPark") { data = "Ulster Park"; }
            //if(data == "UnionVale") { data = "Union Vale"; }
            //if(data == "ValleyCottage") { data = "Valley Cottage"; }
            //if(data == "ValleyStream") { data = "Valley Stream"; }
            //if(data == "Wallkill") { data = "Wallkill Town"; }
            //if(data == "WarwickTown") { data = "Warwick Town"; }
            //if(data == "WesthamptonBeach") { data = "Westhampton Beach"; }
            //if(data == "WestHaven") { data = "West Haven"; }
            //if(data == "WestHaverstraw") { data = "West Haverstraw"; }
            //if(data == "WestHempstead") { data = "West Hempstead"; }
            //if(data == "WestIslip") { data = "West Islip"; }
            //if(data == "WestNyack") { data = "West Nyack"; }
            //if(data == "WestPark") { data = "West Park"; }
            //if(data == "WestRedding") { data = "West Redding"; }
            //if(data == "WhitePlains") { data = "White Plains"; }
            //if(data == "WoodburyTown") { data = "Woodbury Town"; }

            //return data;
        }
        public string TranslateConstructionDesc(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("ADFRTCH", "Advanced Framing Technique, ");
            data = data.Replace("BATTINS", "Batt Insulation, ");
            data = data.Replace("BLOC", "Block, ");
            data = data.Replace("BLWNINL", "Blown-In Insulation, ");
            data = data.Replace("BRIK", "Brick, ");
            data = data.Replace("CLULOSI", "Cellulose Insulation, ");
            data = data.Replace("ENRGYST", "Energy Star, ");
            data = data.Replace("FBRGLSI", "Fiberglass Insulation, ");
            data = data.Replace("FRAM", "Frame, ");
            data = data.Replace("INSLCRT", "Insulated Concrete Forms, ");
            data = data.Replace("LEEDGLD", "LEED-Gold, ");
            data = data.Replace("LEEDPLT", "LEED-Platinum, ");
            data = data.Replace("LEEDSIL", "LEED-Silver, ");
            data = data.Replace("LOG", "Log, ");
            data = data.Replace("MNUFCRD", "Manufactured, ");
            data = data.Replace("MODULAR", "Modular, ");
            data = data.Replace("NAHBGB", "NAHB Green-Bronze, ");
            data = data.Replace("NAHBGG", "NAHB Green-Gold, ");
            data = data.Replace("NAHBGS", "NAHB Green-Silver, ");
            data = data.Replace("OTHR", "Other/See Remarks, ");
            data = data.Replace("PSTNBM", "Post and Beam, ");
            data = data.Replace("RCYLD", "Recycled/Renewable materials/Resources used, ");
            data = data.Replace("SRMK", "Other/See Remarks, ");
            data = data.Replace("STON", "Stone, ");
            data = data.Replace("STRCTIP", "Structurally Insulated Panels, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateGarbage(string data) {
            if(data == "OTHR") { data = "Other/See Remarks"; }
            if(data == "PRIV") { data = "Private"; }
            if(data == "PUBL") { data = "Public"; }

            return data;
        }
        public string TranslateHeatingFuel(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COAL", "Coal, ");
            data = data.Replace("ELEC", "Electric, ");
            data = data.Replace("GAS", "Natural Gas, ");
            data = data.Replace("KERO", "Kerosene, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("OILAG", "Oil Above Ground, ");
            data = data.Replace("OILBG", "Oil Below Ground, ");
            data = data.Replace("OIL", "Oil, ");
            data = data.Replace("OTHE", "Other/See Remarks, ");
            data = data.Replace("PROPAN", "Propane, ");
            data = data.Replace("SOLR", "Solar, ");
            data = data.Replace("WOOD", "Wood, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateHeatingType(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("BASE", "Base Board, ");
            data = data.Replace("DUCTS", "Ducts, ");
            data = data.Replace("ELE", "Electric, ");
            data = data.Replace("ESUNTS", "Energy Star Unit(s), ");
            data = data.Replace("FA", "Forced Air, ");
            data = data.Replace("GEOT", "Geothermal, ");
            data = data.Replace("GHA", "Gravity Hot Air, ");
            data = data.Replace("HA", "Hot Air, ");
            data = data.Replace("HETP", "Heat Pump, ");
            data = data.Replace("HOTW", "Hot Water, ");
            data = data.Replace("HPUMPA", "Heat Pump Air, ");
            data = data.Replace("HTRCSY", "Heat Recovery System, ");
            data = data.Replace("HYDF", "Hydro Air, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("OTHR", "See Remarks, ");
            data = data.Replace("PSVSLR", "Passive Solar, ");
            data = data.Replace("RADI", "Radiant, ");
            data = data.Replace("RADTOR", "Radiator, ");
            data = data.Replace("SOLA", "Solar Thermal, ");
            data = data.Replace("STEM", "Steam, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateHOAFee(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COMMON", "Common Area Costs, ");
            data = data.Replace("EXMAINT", "Exterior Maintenance, ");
            data = data.Replace("OTH", "Other/See Remarks, ");
            data = data.Replace("SEW", "Sewer, ");
            data = data.Replace("SNOWREM", "Snow Removal, ");
            data = data.Replace("TRASH", "Trash Collection, ");
            data = data.Replace("WTRSWR", "Water/Sewer, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateHotWater(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COAL", "Coal, ");
            data = data.Replace("ELECHT", "Electric Stand Alone, ");
            data = data.Replace("ELEC", "Electric, ");
            data = data.Replace("FOHWHT", "Fuel Oil Stand Alone, ");
            data = data.Replace("GASSTN", "Gas Stand Alone, ");
            data = data.Replace("GTHRML", "Geothermal, ");
            data = data.Replace("INDTNK", "Indirect Tank, ");
            data = data.Replace("KERO", "Kerosene, ");
            data = data.Replace("NATG", "Natural Gas, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("OIL", "Oil, ");
            data = data.Replace("ONDMND", "On Demand, ");
            data = data.Replace("PROPAN", "Propane, ");
            data = data.Replace("REM", "See Remarks, ");
            data = data.Replace("SLRTHR", "Solar Thermal, ");
            data = data.Replace("SOL", "Solar, ");
            data = data.Replace("TNKLSC", "Tank Less Coil, ");
            data = data.Replace("WOOD", "Wood, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateIncluded(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("ACUN", "A/C Units, ");
            data = data.Replace("AFAN", "Attic Fan, ");
            data = data.Replace("AIRF", "Air Filter System, ");
            data = data.Replace("ALRM", "Alarm System, ");
            data = data.Replace("AWNI", "Awning, ");
            data = data.Replace("BISH", "B/I Shelves, ");
            data = data.Replace("BIVE", "B/I Audio/Visual Equip, ");
            data = data.Replace("BRDW", "Bread Warmer, ");
            data = data.Replace("BSKT", "Basketball Hoop, ");
            data = data.Replace("CEIL", "Ceiling Fan, ");
            data = data.Replace("CENF", "Central Vacuum, ");
            data = data.Replace("CHAN", "Chandelier(s), ");
            data = data.Replace("CKTOP", "Cook Top, ");
            data = data.Replace("COMP", "Compactor, ");
            data = data.Replace("CONO", "Convection Oven, ");
            data = data.Replace("CRFT", "Craft/Table/Bench, ");
            data = data.Replace("CURT", "Curtains/Drapes, ");
            data = data.Replace("DEHI", "Dehumidifier, ");
            data = data.Replace("DISP", "Disposal, ");
            data = data.Replace("DOOH", "Door Hardware, ");
            data = data.Replace("DRYR", "Dryer, ");
            data = data.Replace("DSHW", "Dishwasher, ");
            data = data.Replace("ENTN", "Entertainment Cabinets, ");
            data = data.Replace("ESAPPL", "Energy Star Appliance(s), ");
            data = data.Replace("FLAT", "Flat Screen TV Bracket, ");
            data = data.Replace("FREE", "Freezer, ");
            data = data.Replace("FRNG", "Front Gate, ");
            data = data.Replace("FRPE", "Fireplace Equip, ");
            data = data.Replace("GARR", "Garage Remote, ");
            data = data.Replace("GAST", "Gas Tank, ");
            data = data.Replace("GDOP", "Garage Door Opener, ");
            data = data.Replace("GENE", "Generator, ");
            data = data.Replace("GREE", "Greenhouse, ");
            data = data.Replace("GRIL", "Gas Grill, ");
            data = data.Replace("HOTT", "Hot Tub, ");
            data = data.Replace("HUMI", "Humidifier, ");
            data = data.Replace("INTR", "Intercom, ");
            data = data.Replace("LAWM", "Lawn Maint Equip, ");
            data = data.Replace("LTRF", "Light Fixtures, ");
            data = data.Replace("LWFLWF", "Low Flow fixtures, ");
            data = data.Replace("MAIL", "Mailbox, ");
            data = data.Replace("MICR", "Microwave, ");
            data = data.Replace("NANN", "Nanny Cam/Comp Serv, ");
            data = data.Replace("PAYS", "Playset, ");
            data = data.Replace("PELT", "Pellet Stove, ");
            data = data.Replace("POLE", "Pool Equipt/Cover, ");
            data = data.Replace("RANG", "Oven/Range, ");
            data = data.Replace("REFR", "Refrigerator, ");
            data = data.Replace("SCRE", "Screens, ");
            data = data.Replace("SDIS", "Second Dishwasher, ");
            data = data.Replace("SECD", "Second Dryer, ");
            data = data.Replace("SECF", "Second Freezer, ");
            data = data.Replace("SECS", "Second Stove, ");
            data = data.Replace("SHAD", "Shades/Blinds, ");
            data = data.Replace("SHED", "Shed, ");
            data = data.Replace("SPNLSL", "Solar Panels Leased, ");
            data = data.Replace("SPNLSO", "Solar Panels Owned, ");
            data = data.Replace("SPOU", "Speakers Outdoor, ");
            data = data.Replace("SPRK", "Speakers Indoor, ");
            data = data.Replace("SREF", "Second  Refrigerator, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("STAN", "Stained Glass Window, ");
            data = data.Replace("STOR", "Storm Windows, ");
            data = data.Replace("SWAH", "Second Washer, ");
            data = data.Replace("TVDISH", "TV Dish, ");
            data = data.Replace("VISC", "Video Cameras, ");
            data = data.Replace("WAOVEN", "Wall Oven, ");
            data = data.Replace("WASH", "Washer, ");
            data = data.Replace("WCOWN", "Water Conditioner Owned, ");
            data = data.Replace("WCRENT", "Water Conditioner Rented, ");
            data = data.Replace("WHET", "Whole House Ent. Syst, ");
            data = data.Replace("WHIR", "Whirlpool Tub, ");
            data = data.Replace("WNEC", "Wine Cooler, ");
            data = data.Replace("WOOB", "Woodburning Stove, ");
            data = data.Replace("WWCA", "Wall to Wall Carpet, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateLeaseTerm(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("12MP", "Over 12 Months, ");
            data = data.Replace("12M", "12 Months, ");
            data = data.Replace("16M", "1-6 Months, ");
            data = data.Replace("612M", "6-12 Month, ");
            data = data.Replace("MONTH", "Monthly, ");
            data = data.Replace("SUBLS", "Sublease, ");
            data = data.Replace("WEEK", "Weekly, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateLotDesc(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("BRDSSTLND", "Borders State Land, ");
            data = data.Replace("CORNER", "Corner Lot, ");
            data = data.Replace("EASMNT", "Easement, ");
            data = data.Replace("HDSTRCT", "Historic District, ");
            data = data.Replace("HILLY", "Sloping, ");
            data = data.Replace("LEVEL", "Level, ");
            data = data.Replace("PSUBDIV", "Possible Sub Division, ");
            data = data.Replace("PTWOOD", "Partly Wooded, ");
            data = data.Replace("RESTRCT", "Restrictions, ");
            data = data.Replace("SBWALL", "Stone/Brick Wall, ");
            data = data.Replace("WOODED", "Wooded, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateParking(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("ASS", "Assigned, ");
            data = data.Replace("ATT", "Attached, ");
            data = data.Replace("COMM", "Common, ");
            data = data.Replace("COV", "Covered, ");
            data = data.Replace("CP", "Carport, ");
            data = data.Replace("DET", "Detached, ");
            data = data.Replace("DRIV", "Driveway, ");
            data = data.Replace("FCAT", "4+ Car Attached, ");
            data = data.Replace("FCDE", "4+ Car Detached, ");
            data = data.Replace("GP", "Garage Parking, ");
            data = data.Replace("LP", "Lot Parking, ");
            data = data.Replace("NOG", "No Garage, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("NOP", "No Parking, ");
            data = data.Replace("OCAT", "1 Car Attached, ");
            data = data.Replace("OCDT", "1 Car Detached, ");
            data = data.Replace("OSIP", "Off-Site Parking, ");
            data = data.Replace("OSP", "Off-Street Parking, ");
            data = data.Replace("OTH", "Other/See Remarks, ");
            data = data.Replace("PCAT", "3 Car Attached, ");
            data = data.Replace("PCDE", "3 Car Detached, ");
            data = data.Replace("PP", "Private Parking, ");
            data = data.Replace("PUB", "Public Parking, ");
            data = data.Replace("STO", "Storage, ");
            data = data.Replace("STP", "Street Parking, ");
            data = data.Replace("TAN", "Tandem, ");
            data = data.Replace("TCAT", "2 Car Attached, ");
            data = data.Replace("TCDE", "2 Car Detached, ");
            data = data.Replace("UHG", "Under Home/Ground, ");
            data = data.Replace("UN", "Unassigned, ");
            data = data.Replace("WAIT", "Waitlist, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslatePropertySubType(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("1FD", "1 Family Dwelling, ");
            data = data.Replace("1L", "1 Level, ");
            data = data.Replace("24FD", "2-4 Family Dwelling, ");
            data = data.Replace("4MFD", "5 or More Family Dwelling, ");
            data = data.Replace("AA", "Agricultural- Active, ");
            data = data.Replace("AG", "Agricultural, ");
            data = data.Replace("AN", "Agricultural- Nonactive, ");
            data = data.Replace("AP", "Apartment, ");
            data = data.Replace("AT", "Attached, ");
            data = data.Replace("BC", "Bungalow Colony, ");
            data = data.Replace("COM", "Commercial, ");
            data = data.Replace("CON", "Condo, ");
            data = data.Replace("COP", "Co-op, ");
            data = data.Replace("DET", "Detached, ");
            data = data.Replace("DUP", "Duplex, ");
            data = data.Replace("EQ", "Equestrian, ");
            data = data.Replace("FA", "Farm Agricultural, ");
            data = data.Replace("FC", "Farm Crops, ");
            data = data.Replace("FCAH", "Farm Con/Ag/Horse 7-10-10 Rule, ");
            data = data.Replace("HC", "Health Care, ");
            data = data.Replace("HM", "Hotel &amp; Motel, ");
            data = data.Replace("IND", "Industrial, ");
            data = data.Replace("LA", "Legal Accessory, ");
            data = data.Replace("LL", "Lot/Land, ");
            data = data.Replace("LND", "Land, ");
            data = data.Replace("MF", "Multi-Family, ");
            data = data.Replace("MLS", "Multiple Lots/Subdivision, ");
            data = data.Replace("MU", "Mixed Use, ");
            data = data.Replace("NULL", "NULL, ");
            data = data.Replace("O", "Office, ");
            data = data.Replace("OTH", "Other/See Remarks, ");
            data = data.Replace("RD", "Row Dwelling, ");
            data = data.Replace("REC", "Recreational , ");
            data = data.Replace("RET", "Retail, ");
            data = data.Replace("RI", "Residential Income, ");
            data = data.Replace("SC", "Shopping Center, ");
            data = data.Replace("SE", "Sport &amp; Entertainment, ");
            data = data.Replace("SEA", "Seasonal, ");
            data = data.Replace("SF", "Single Family, ");
            data = data.Replace("SH", "Senior Housing, ");
            data = data.Replace("SP", "Special Purpose, ");
            data = data.Replace("TRI", "Triplex, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslatePropertyType(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COM", "Commercial, ");
            data = data.Replace("CON", "Condominium, ");
            data = data.Replace("COP", "Co-Operative, ");
            data = data.Replace("LND", "Land, ");
            data = data.Replace("MF", "Multi-Family 2-4, ");
            data = data.Replace("MF5", "Multi-Family 5+, ");
            data = data.Replace("RES", "Single Family, ");
            data = data.Replace("RNT", "Rental, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateRoadFrontDescription(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COUNTY", "County, ");
            data = data.Replace("INTERST", "Interstate, ");
            data = data.Replace("MUNCIP", "Municipal, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("PRIVATE", "Private, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("STATE", "State, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateSewerDesc(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("CESS", "Cesspool, ");
            data = data.Replace("COMNTY", "Community, ");
            data = data.Replace("MUNCIP", "Municipal, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("S500", "Sewer Within 500 Feet, ");
            data = data.Replace("S500P", "Sewer Over 500 Feet, ");
            data = data.Replace("SAPP", "Septic Approved, ");
            data = data.Replace("SEPT", "Septic, ");
            data = data.Replace("SEWE", "Sewer, ");
            data = data.Replace("SPTCAG", "Septic Above Ground, ");
            data = data.Replace("SRMK", "Others/See Remarks, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateSiding(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("ALUM", "Aluminum, ");
            data = data.Replace("ASBES", "Asbestos, ");
            data = data.Replace("BLOCK", "Block, ");
            data = data.Replace("BRICK", "Brick, ");
            data = data.Replace("CEDAR", "Cedar, ");
            data = data.Replace("CLAPBD", "Clap Board, ");
            data = data.Replace("CONC", "Concrete Fiber Board, ");
            data = data.Replace("HP", "Hardiplank, ");
            data = data.Replace("LOG", "Log, ");
            data = data.Replace("MASO", "Masonry, ");
            data = data.Replace("OTH", "Other/See Remarks, ");
            data = data.Replace("SHAKE", "Cedar Shake, ");
            data = data.Replace("SHIN", "Shingle, ");
            data = data.Replace("STONE", "Stone, ");
            data = data.Replace("STUC", "Stucco, ");
            data = data.Replace("T111", "T111, ");
            data = data.Replace("VNYL", "Vinyl, ");
            data = data.Replace("WOOD", "Wood, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateSoilType(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("CLAY", "Clay, ");
            data = data.Replace("GRAVEL", "Gravel, ");
            data = data.Replace("LOAM", "Loam, ");
            data = data.Replace("MUCK", "Muck, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("UNKNWN", "Unknown, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateStreetType(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("MAINT", "Maintenance Agreement, ");
            data = data.Replace("PUBLIC", "Public, ");
            data = data.Replace("PRIVATE", "Private, ");
            data = data.Replace("TOBEDED", "To Be Dedicated, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateStyle(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("170", "Other/See Remarks, ");
            data = data.Replace("180", "Raised Ranch, ");
            data = data.Replace("190", "Ranch, ");
            data = data.Replace("200", "Salt Box, ");
            data = data.Replace("230", "Split Level, ");
            data = data.Replace("240", "Town House, ");
            data = data.Replace("250", "Tudor, ");
            data = data.Replace("260", "Victorian, ");
            data = data.Replace("2STORY", "Two Story, ");
            data = data.Replace("40", "Carriage House, ");
            data = data.Replace("4SQUARE", "Foursquare, ");
            data = data.Replace("50", "Colonial, ");
            data = data.Replace("60", "Contemporary, ");
            data = data.Replace("70", "Converted Barn, ");
            data = data.Replace("80", "Cottage, ");
            data = data.Replace("90", "Farm House, ");
            data = data.Replace("APARTMENT", "Apartment, ");
            data = data.Replace("ARTSNCRAFT", "Arts&Crafts, ");
            data = data.Replace("ATTACHED", "Attached, ");
            data = data.Replace("BILEV", "Bilevel, ");
            data = data.Replace("BUNGALOW", "Bungalow, ");
            data = data.Replace("CAPECOD", "Capecod, ");
            data = data.Replace("CHALET", "Chalet, ");
            data = data.Replace("DETACHED", "Detached, ");
            data = data.Replace("ESTATE", "Estate, ");
            data = data.Replace("FLATS", "Flats, ");
            data = data.Replace("GARDENAPT", "Garden Apartment, ");
            data = data.Replace("HIGHRISE", "High Rise, ");
            data = data.Replace("LOG", "Log, ");
            data = data.Replace("MEDIT", "Mediterranean, ");
            data = data.Replace("MIDRISE", "Mid Rise, ");
            data = data.Replace("MINIESTATE", "Mini Estate, ");
            data = data.Replace("MOBILE", "Mobile Home With Property, ");
            data = data.Replace("SEASONAL", "Seasonal, ");
            data = data.Replace("SPLANCH", "Splanch, ");
            data = data.Replace("TRILEVEL", "Trilevel, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateTopography(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("FILLNEED", "Fill Needed, ");
            data = data.Replace("HILLY", "Hilly, ");
            data = data.Replace("LEVEL", "Level, ");
            data = data.Replace("ROLLING", "Rolling, ");
            data = data.Replace("SCENIC", "Scenic View, ");
            data = data.Replace("STEEP", "Steep, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateTransactionType(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("LSE", "Lease, ");
            data = data.Replace("SLE", "Sale, ");
            data = data.Replace("SL", "Sale Or Lease, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }
        public string TranslateUtilitiesOn_AbuttingSite(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("CABLE", "Cable, ");
            data = data.Replace("ELEC", "Electric, ");
            data = data.Replace("GAS", "Gas, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("PHONE", "Phone, ");
            data = data.Replace("SEPTC", "Septic, ");
            data = data.Replace("SEWER", "Sewer, ");
            data = data.Replace("SRMK", "See Remarks, ");
            data = data.Replace("WATER", "Water, ");
            data = data.Replace("WELL", "Well, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }

        public string TranslateWaterDesc(string data) {
            if(data.StartsWith("\"")) { data = data.Substring(1); }
            if(data.EndsWith("\"")) { data = data.Substring(0, data.Length - 1); }
            data = data.Replace("COMNTY", "Community, ");
            data = data.Replace("DRILL", "Drilled Well, ");
            data = data.Replace("DUG", "Dug Well, ");
            data = data.Replace("LESS500", "Municipal Over 500 Feet, ");
            data = data.Replace("MUNI", "Municipal, ");
            data = data.Replace("NONE", "None, ");
            data = data.Replace("OTHR", "Other/See Remarks, ");
            data = data.Replace("OVER500", "Municipal Within 500 Feet, ");
            data = data.Replace("PRIV", "Private, ");
            data = data.Replace("SEASONAL", "Seasonal, ");
            data = data.Replace("SHAR", "Shared, ");
            data = data.Replace("SPRING", "Spring, ");
            data = data.Replace(", ,", ", ");
            if(data.EndsWith(", ")) { data = data.Substring(0, data.Length - 2); }

            return data;
        }

        private string[] GetRelatedTowns(string[] towns) {
			List<string> result = new List<string>();
			if (towns != null && towns.Length > 0 && !string.IsNullOrEmpty(towns[0])) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("SELECT RelatedTown FROM TownRelations WHERE Town = @Town", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("Town", SqlDbType.VarChar, 100);
						foreach (string town in towns) {
							if (!result.Contains(town)) {
								result.Add(town);
							}
							cmd.Parameters["Town"].Value = town;
							cmd.Connection.Open();
							SqlDataReader dr = cmd.ExecuteReader();
							while (dr.Read()) {
								if (!result.Contains(dr[0].ToString())) {
									result.Add(dr[0].ToString());
								}
							}
							cmd.Connection.Close();
						}
					}
				}
				return result.ToArray();
			} else {
				return null;
			}
		}
	}

	public class SearchResult3 {
		public List<Listing3> Listings { get; set; }
		public string PrevPage { get; set; }
		public string NextPage { get; set; }
		public int TotalResults { get; set; }
	}
	public class ListingResult3 {
		//public Listing3 listing { get; set; }
		public MLSListing listing { get; set; }
		public MLSSearchResult searchResult { get; set; }
		//public SearchResult3 searchResult { get; set; }
		public int ResultCount { get; set; }
	}
}