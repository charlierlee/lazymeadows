using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;


namespace TKS.Models.realestate {
    public class ListingComm {

        #region Constructor
        public ListingComm() { }
        public ListingComm(string mls) {
            MLS = mls;
            //try {
            using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                string SQL = "SELECT [Access24Hour],[AddFeeFrequency],[AdditionalFeeDes]," +
                    "[AdditionalFeesAmt],[Adult55Community],[AirConditioning]," +
                    "[AlternateMLSNumber],[Amenities],[AtticDescription]," +
                    "[BasementDescription],[BathsFull],[BathsHalf]," +
                    "[BathsTotal],[BedsTotal],[City]," +
                    "[CloseDate],[ComplexName],[ConstructionDescription]," +
                    "[ContractDate],[CountyOrParish],[CurrentPrice]," +
                    "[ElementarySchool],[Fireplacesnumberof],[FloorNumber]," +
                    "[Garbage],[GasAvailableYN],[Hamlet]," +
                    "[HeatingFuel],[HeatingType],[HeatingZonesNumof]," +
                    "[HighSchool],[HoaFeeIncludes],[Hotwater]," +
                    "[Included],[Junior_MiddleHighSchool],[LastLeasePrice]," +
                    "[LeasePrice],[LeaseTerm],[ListAgentDirectWorkPhone]," +
                    "[ListAgentEmail],[ListAgentFullName],[ListAgentMLSID]," +
                    "[LotDescription],[LotSizeArea],[LotSizeAreaSQFT]," +
                    "[MarketingRemarks],[Matrix_Unique_ID],l1.[MLSNumber]," +
                    "[MonthlyHOAFee],[NumOfLevels],[OpenHouseUpcoming]," +
                    "[OriginalLeasePrice],[Parking],[PhotoCount]," +
                    "[PostalCode],[PostalCodePlus4],[PropertySubType]," +
                    "[PropertyType],[PublicTransportation],[REO_BankOwned]," +
                    "[RoomCount],[SchoolDistrict],[SewerDescription]," +
                    "[SidingDescription],[SprinklerSystemYN],[SqFtSource]," +
                    "[SqFtTotal],[StateOrProvince],[Status]," +
                    "[StreetDirPrefix],[StreetDirSuffix],[StreetName]," +
                    "[StreetNumber],[StreetNumberModifier],[StreetSuffix]," +
                    "[StreetType],[Style],[TaxAmount]," +
                    "[TaxSource],[TaxYear],[TotalRoomsFinished]," +
                    "[TransactionType],[TypeofDwelling],[TypeofUnit]," +
                    "[UnitCount],[UnitNumber],[Village]," +
                    "[VirtualTourLink],[WaterAccessYN],[WaterDescription]," +
                    "[YearBuilt],[YearBuiltException],[YearRenovated],[Zoning], " +
                    "ISNULL(l2.[Lat], 0), ISNULL(l2.[Long], 0)  " +
                    "FROM [dbo].[listings-commercial-3] l1 LEFT JOIN [listings-geo] l2 ON l1.[MLSNumber] = l2.[MLSNumber] " +
                    "WHERE l1.[MLSNumber] = @MLS";

                using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("MLS", SqlDbType.Float).Value = Convert.ToDouble(MLS);
                    cmd.Connection.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if(dr.HasRows) {
                        dr.Read();
                        Access24Hour = dr.GetBoolean(0);
                        if(dr[3].ToString() != "-1") { AdditionalFees = string.Format("{0:$#,###}", dr[3]) + " "; }
                        AdditionalFees += (" " + dr[1].ToString() + " " + dr[2].ToString()).Trim();
                        Adult55Community = dr.GetBoolean(4);
                        AirConditioning = dr[5].ToString();
                        FeaturesAmenities = dr[7].ToString();
                        Attic = dr[8].ToString();
                        Basement = dr[9].ToString();
                        FullBaths = dr[10].ToString();
                        HalfBaths = dr[11].ToString();
                        if(dr[13].ToString() != "-1") { Bedrooms = dr[13].ToString(); }
                        City = dr[14].ToString();
                        ComplexName = dr[16].ToString();
                        Construction = dr[17].ToString();
                        County = dr[19].ToString();
                        float aa = 0;
                        float.TryParse(dr[20].ToString(), out aa);
                        AskingAmt = aa;
                        AskingPrice = string.Format("{0:$#,###}", dr[20]);
                        if(dr[22].ToString() != "-1" && dr[22].ToString() != "0") {
                            Fireplace = dr[22].ToString();
                        }
                        if(dr[23].ToString() != "-1") { FloorNumber = dr.GetInt32(23); }
                        Garbage = dr[24].ToString();
                        GasAvailableYN = dr.GetBoolean(25);
                        Fuel = dr[27].ToString();
                        Heating = dr[28].ToString();
                        if(dr[29].ToString() != "-1") { HeatingZones = dr[29].ToString(); }
                        HOAFeeIncludes = dr[31].ToString();
                        WaterHeater = dr[32].ToString();
                        Included = dr[33].ToString();
                        AgentPhone = dr[38].ToString();
                        AgentName = dr[40].ToString();
                        AgentID = dr[41].ToString();
                        if(dr[35].ToString() != "-1") { LastLeasePrice = string.Format("{0:$#,###}", dr[35]); }
                        if(dr[36].ToString() != "-1") { LeasePrice = string.Format("{0:$#,###}", dr[36]); }
                        if(dr[51].ToString() != "-1") { OriginalLeasePrice = string.Format("{0:$#,###}", dr[51]); }
                        LeaseTerm = dr[37].ToString();
                        LotDescription = dr[42].ToString();
                        if(dr[43].ToString() != "-1") { Acres = dr[43].ToString(); }
                        if(dr[44].ToString() != "-1") { LotSquareFeet = dr[44].ToString(); }
                        Remarks = dr[45].ToString();
                        MatrixUnique = dr[46].ToString();
                        MLS = dr[47].ToString();
                        if(dr[48].ToString() != "-1") { HomeownersFees = string.Format("{0:$#,###}", dr[48]); }
                        if(dr[49].ToString() != "-1") { Levels = dr[49].ToString(); }
                        Parking = dr[52].ToString();
                        PhotoCount = dr.GetInt32(53);
                        Zip = dr[54].ToString();
                        if(dr[55].ToString().Length > 0) { Zip += "-" + dr[55].ToString(); }
                        SubType = dr[56].ToString();
                        Type = dr[57].ToString();
                        REO = dr.GetBoolean(59);
                        if(dr.GetInt32(60) > -1) { Rooms = dr[60].ToString(); }
                        SchoolDistrict = dr[61].ToString();
                        Sewer = dr[62].ToString();
                        Exterior = dr[63].ToString();
                        SprinklerSystemYN = dr.GetBoolean(64);
                        if(dr[66].ToString() != "-1") { TotalSquareFeet = string.Format("{0:#,###}", dr[66]); }
                        State = dr[67].ToString();
                        Status = dr[68].ToString();
                        if(dr[72].ToString() != "") {
                            AddressStreet = dr[72].ToString() + " ";
                        }
                        if(dr[73].ToString() != "") {
                            AddressStreet = AddressStreet.Trim() + dr[73].ToString() + " ";
                        }
                        if(dr[69].ToString() != "") {
                            AddressStreet += dr[69].ToString() + " ";
                        }
                        if(dr[71].ToString() != "") {
                            AddressStreet += dr[71].ToString() + " ";
                        }
                        if(dr[70].ToString() != "") {
                            AddressStreet += dr[70].ToString() + " ";
                        }
                        if(dr[74].ToString() != "") {
                            AddressStreet += dr[74].ToString();
                        }
                        AddressStreet = (AddressStreet ?? "").Trim();
                        Road = dr[75].ToString();
                        Style = dr[76].ToString();
                        if(dr[77].ToString() != "-1") { TotalTaxes = string.Format("{0:$#,###}", dr[77]); }
                        if(dr[79].ToString() != "-1") { TaxYear = dr[79].ToString(); }
                        if(dr[80].ToString() != "-1") { RoomsFinished = dr[80].ToString(); }
                        TransactionType = dr[81].ToString();
                        UnitNumber = dr[85].ToString();
                        VirtualTour = dr[87].ToString();
                        WaterAccess = dr.GetBoolean(88);
                        Water = dr[89].ToString();
                        if(dr[90].ToString() != "-1") { YearBuilt = dr[90].ToString(); }
                        Zoning = dr[93].ToString();
                        Lat = (float)dr.GetDouble(94);
                        Long = (float)dr.GetDouble(95);

                    } else {
                        MLS = null;
                    }
                    cmd.Connection.Close();
                }
            }
            //} catch { }
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
            if(!string.IsNullOrEmpty(Global.RSSFeedURL) && Global.RSSFeedURL != Global.BaseURL) {
                sb.AppendLine("<link rel='alternate' type='application/rss+xml' title='" + Global.FeedTitle + "'  href='" + Global.RSSFeedURL + "' />");
            }

            if(!string.IsNullOrEmpty(Global.SiteName) || !string.IsNullOrEmpty(Global.SiteNameAlternate)) {
                if(!string.IsNullOrEmpty(Global.SiteName) && !string.IsNullOrEmpty(Global.SiteNameAlternate)) {
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
            if(value == null || value.Length < length || value.IndexOf(" ", length) == -1)
                return value;

            return value.Substring(0, value.IndexOf(" ", length));
        }
        #endregion

        #region Public Properties
        public bool Access24Hour { get; set; }
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
        public string ComplexName { get; set; }
        public string Construction { get; set; }
        public string County { get; set; }
        public float AskingAmt { get; set; }
        public string AskingPrice { get; set; }
        public string Fireplace { get; set; }
        public int FloorNumber { get; set; }
        public string Garbage { get; set; }
        public bool GasAvailableYN { get; set; }
        public string Fuel { get; set; }
        public string Heating { get; set; }
        public string HeatingZones { get; set; }
        public string HOAFeeIncludes { get; set; }
        public string WaterHeater { get; set; }
        public string Included { get; set; }
        public string AgentPhone { get; set; }
        public string AgentID { get; set; }
        public string AgentName { get; set; }
        public string LastLeasePrice { get; set; }
        public string LeasePrice { get; set; }
        public string OriginalLeasePrice { get; set; }
        public string LeaseTerm { get; set; }
        public string LotDescription { get; set; }
        public string Acres { get; set; }
        public string LotSquareFeet { get; set; }
        public string Remarks { get; set; }
        public string MatrixUnique { get; set; }
        public string MLS { get; set; }
        public string HomeownersFees { get; set; }
        public string Levels { get; set; }
        public string Parking { get; set; }
        public int PhotoCount { get; set; }
        public string Zip { get; set; }
        public string SubType { get; set; }
        public string Type { get; set; }
        public bool REO { get; set; }
        public string Rooms { get; set; }
        public string RoomsFinished { get; set; }
        public string SchoolDistrict { get; set; }
        public string Sewer { get; set; }
        public bool SprinklerSystemYN { get; set; }
        public string Exterior { get; set; }
        public string TotalSquareFeet { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string AddressStreet { get; set; }
        public string Road { get; set; }
        public string Style { get; set; }
        public string TotalTaxes { get; set; }
        public string TaxYear { get; set; }
        public string TransactionType { get; set; }
        public string UnitNumber { get; set; }
        public string VirtualTour { get; set; }
        public bool WaterAccess { get; set; }
        public string Water { get; set; }
        public string YearBuilt { get; set; }

		public float DownpaymentPercent { get; set; }
		public float Percent30yr { get; set; }
		public float Percent15yr { get; set; }
		public float PercentARM { get; set; }
		public string Downpayment {
			get {
				if (AskingAmt > 0 && DownpaymentPercent > 0) {
					return string.Format("{0:$0,000}", AskingAmt * DownpaymentPercent / 100);
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
                if(!string.IsNullOrEmpty(City)) {
                    if(ret.Length > 0) {
                        ret += ", " + City + " ";
                    } else {
                        ret += City + " ";
                    }
                }
                if(!string.IsNullOrEmpty(State)) {
                    ret += State + " ";
                }
                if(!string.IsNullOrEmpty(Zip)) {
                    ret += Zip + " ";
                }
                if(ret.Length > 0) { ret = ret.Substring(0, ret.Length - 1); }
                return ret;
            }
        }
        public string ListingType { get; set; }
		public string NumberOfUnits { get; set; }
		public string Zoning { get; set; }

		public float Lat { get; set; }
		public float Long { get; set; }

		public string ImageFirst{
			get{
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
                for(int x = 1; x <= PhotoCount; x++) {
                    imgs[x - 1] = "http://www.lazymeadowsrealty.com/img/listings/XLargePhoto" + MatrixUnique.ToString() + "-" + x.ToString() + ".jpeg";
                }
                return imgs;
            }
        }
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
					string SQL = "SELECT DISTINCT City FROM [listings-commercial-3] ORDER BY City";

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
					string SQL = "SELECT DISTINCT City FROM [listings-commercial-3] ORDER BY City";

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

	public class ListingsComm {
		public SearchResultComm DoSearch(SearchModelComm data, int page = 1, int PostsPerPage = 12) {
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string param = "";
			string where = "";
			SearchResultComm searchResult = new SearchResultComm();
			List<ListingComm> listings = new List<ListingComm>();
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
				if (data.MinPrice > 0) { where += "AND [CurrentPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { where += "AND [CurrentPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { where += "AND [SqFtTotal] >= @SqFt "; }
				if (data.SqFt2 > 0) { where += "AND [SqFtTotal] <= @SqFt2 "; }

                string SQL = "SELECT COUNT(*) FROM [listings-commercial-3] WHERE Status = 'A' " + where;

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (TownList != null && !string.IsNullOrEmpty(TownList[0])) {
						for (int x = 1; x <= TownList.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = TownList[x-1];
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.SqFt > 0) { cmd.Parameters.Add("SqFt", SqlDbType.Float).Value = data.SqFt; param += "&sqft=" + data.SqFt.ToString(); }
					if (data.SqFt2 > 0) { cmd.Parameters.Add("SqFt2", SqlDbType.Float).Value = data.SqFt2; param += "&sqft2=" + data.SqFt2.ToString(); }


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
					} else  {
						searchResult.NextPage = "/searchcomm?" + param + "&page=" + (page + 1).ToString();
					}
					if (page == 1) {
						searchResult.PrevPage = "";
					} else {
						searchResult.PrevPage = "/searchcomm?" + param + "&page=" + (page - 1).ToString();
					}

					SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY [CurrentPrice] DESC, City) AS Rownumber, [MLSNumber] FROM [listings-commercial-3] WHERE Status = 'A' " + where;
					SQL += ") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();

					cmd.CommandText = SQL;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new ListingComm(dr[1].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			searchResult.Listings = listings;
			return searchResult;
		}

        public List<ListingComm> GetMapListings(SearchModelComm data) {
			string param = "";
			List<ListingComm> listings = new List<ListingComm>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT l1.[MLSNumber] FROM [listings-commercial-3] l1 JOIN [listings-geo] l2 ON l1.[MLSNumber] = l2.[MLSNumber] WHERE l1.Status = 'A' ";
				string[] TownList = GetRelatedTowns(data.City);
				if (TownList != null && !string.IsNullOrEmpty(TownList[0])) {
					SQL += "AND (City = @City1 ";
					for (int x = 2; x <= TownList.Length; x++) {
						SQL += " OR City = @City" + x.ToString();
					}
					SQL += ") ";
				}
				if (data.Acres > 0) { SQL += "AND [LotSizeArea] >= @Acres "; }
				if (data.Acres2 > 0) { SQL += "AND [LotSizeArea] <= @Acres2 "; }
				if (data.MinPrice > 0) { SQL += "AND [CurrentPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { SQL += "AND [CurrentPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { SQL += "AND [SqFtTotal] >= @SqFt "; }
				if (data.SqFt2 > 0) { SQL += "AND [SqFtTotal] <= @SqFt2 "; }

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
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.SqFt > 0) { cmd.Parameters.Add("SqFt", SqlDbType.Float).Value = data.SqFt; param += "&sqft=" + data.SqFt.ToString(); }
					if (data.SqFt2 > 0) { cmd.Parameters.Add("SqFt2", SqlDbType.Float).Value = data.SqFt2; param += "&sqft2=" + data.SqFt2.ToString(); }

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new ListingComm(dr[0].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			return listings;
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

	public class SearchModelComm {
		public string[] City { get; set; }
		public int AcreRange { get; set; }
		public int Acres { get; set; }
		public int Acres2 { get; set; }
		public int SqFt { get; set; }
		public int SqFt2 { get; set; }
		public int PriceRange { get; set; }
		public int MinPrice { get; set; }
		public int MaxPrice { get; set; }
		public int Years { get; set; }
		public int Years2 { get; set; }
		public string MLS { get; set; }
		public int Garage { get; set; }
		public int Pool { get; set; }
		public int Fireplace { get; set; }
		public int Barn { get; set; }
		public int Handicap { get; set; }
		public int Skylights { get; set; }
		public int Lake { get; set; }
	}

	public class SearchResultComm {
		public List<ListingComm> Listings { get; set; }
		public string PrevPage { get; set; }
		public string NextPage { get; set; }
		public int TotalResults { get; set; }
	}
	public class ListingResultComm {
		public ListingComm listing { get; set; }
		public SearchResultComm searchResult { get; set; }
		public int ResultCount { get; set; }
	}
}