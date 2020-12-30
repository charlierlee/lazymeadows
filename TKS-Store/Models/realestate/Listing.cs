using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;


namespace TKS.Models.realestate {
	public class Listing {

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
		public string MLS { get; set; }
		public string Class { get; set; }
		public string Type { get; set; }
		public string Township { get; set; }
		public float AskingAmt { get; set; }
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
		public string AskingPrice { get; set; }
		public string AddressNumber { get; set; }
		public string AddressDirection { get; set; }
		public string AddressStreet { get; set; }
		public string Address2 { get; set; }
		public string AddressOneLine {
			get {
				string ret = "";
				if (!string.IsNullOrEmpty(AddressNumber)) {
					ret += AddressNumber + " ";
				}
				if (!string.IsNullOrEmpty(AddressDirection)) {
					ret += AddressDirection + " ";
				}
				if (!string.IsNullOrEmpty(AddressStreet)) {
					ret += AddressStreet + " ";
				}
				if (ret.Length > 0) { ret = ret.Substring(0, ret.Length - 1); }
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
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Status { get; set; }
		public string SaleRent { get; set; }
		public string Rooms { get; set; }
		public string Bedrooms { get; set; }
		public string FullBaths { get; set; }
		public string HalfBaths { get; set; }
		public string TotalBaths { get { return FullBaths + " full, " + HalfBaths + " half"; } }
		public string GarageType { get; set; }
		public string GarageCapacity { get; set; }
		public string Fireplace { get; set; }
		public string ListingType { get; set; }
		public string Acres { get; set; }
		public string AgentID { get; set; }
		public string AgentName { get; set; }
		public string AgentPhone { get; set; }
		public string ListingOfficeID { get; set; }
		public string ListingOfficeName { get; set; }
		public string ListingOfficePhone { get; set; }
		public string County { get; set; }
		public string SchoolDistrict { get; set; }
		public string Development { get; set; }
		public string YearBuilt { get; set; }
		public string LotSizeFront { get; set; }
		public string LotSizeSide { get; set; }
		public string SqFtAboveGrade { get; set; }
		public string SqFtBelowGrade { get; set; }
		public string TotalSquareFeet { get; set; }
		public string TotalTaxes { get; set; }
		public string TaxExemption { get; set; }
		public string LandValue { get; set; }
		public string BldgValue { get; set; }
		public string TotalValue { get; set; }
		public string PossessionDate { get; set; }
		public string MaintenanceFees { get; set; }
		public string Utilities { get; set; }
		public string HomeownersFees { get; set; }
		public string FinancingTerms { get; set; }
		public string Remarks { get; set; }
		public string AFrame { get; set; }
		public string VirtualTour { get; set; }

		public float Lat { get; set; }
		public float Long { get; set; }

		public string ImageFirst {
			get {
				if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath("/img/listings"), "SCMLS" + MLS.ToString() + ".jpg"))) {
					return "SCMLS" + MLS.ToString() + ".jpg";
				} else {
					string[] temp = Images;
					if (temp.Length > 0) {
						return temp[0].Substring(temp[0].LastIndexOf("\\") + 1).ToLower();
					} else {
						return "";
					}
				}
			}
		}
		public string[] Images {
			get {
				string[] temp = Directory.GetFiles(HttpContext.Current.Server.MapPath("/img/listings"), "SCMLS" + MLS.ToString() + "*.*");
				for (int x = 0; x < temp.Length; x++) {
					string cur = temp[x];
					temp[x] = cur.Length.ToString().PadLeft(5, '0') + cur;
				}
				//sort by the length of the string first, then by the string itself
				Array.Sort(temp);
				for (int x = 0; x < temp.Length; x++) {
					string cur = temp[x];
					temp[x] = cur.Substring(cur.LastIndexOf("\\") + 1).ToLower();
				}
				return temp;
			}
		}

		public string Style {
			get {
				string ret = "";
				if (AFrame.Contains("|1|")) { ret += "A-Frame, "; }
				if (AFrame.Contains("|2|")) { ret += "Bi-Level, "; }
				if (AFrame.Contains("|3|")) { ret += "Bungalow, "; }
				if (AFrame.Contains("|4|")) { ret += "Cape Cod, "; }
				if (AFrame.Contains("|5|")) { ret += "Colonial, "; }
				if (AFrame.Contains("|6|")) { ret += "Condo, "; }
				if (AFrame.Contains("|7|")) { ret += "Contemporary, "; }
				if (AFrame.Contains("|8|")) { ret += "Co-Op, "; }
				if (AFrame.Contains("|9|")) { ret += "Cottage, "; }
				if (AFrame.Contains("|10|")) { ret += "Custom, "; }
				if (AFrame.Contains("|11|")) { ret += "Double-Wide Mobile, "; }
				if (AFrame.Contains("|12|")) { ret += "Dutch Colonial, "; }
				if (AFrame.Contains("|13|")) { ret += "Farm House, "; }
				if (AFrame.Contains("|14|")) { ret += "Log Cabin, "; }
				if (AFrame.Contains("|15|")) { ret += "Mobile Home, "; }
				if (AFrame.Contains("|16|")) { ret += "Modular, "; }
				if (AFrame.Contains("|17|")) { ret += "Ranch, "; }
				if (AFrame.Contains("|18|")) { ret += "Ranch-Raised, "; }
				if (AFrame.Contains("|19|")) { ret += "Salt Box, "; }
				if (AFrame.Contains("|20|")) { ret += "Single-Wide Mobile, "; }
				if (AFrame.Contains("|21|")) { ret += "Split, "; }
				if (AFrame.Contains("|22|")) { ret += "Townhouse, "; }
				if (AFrame.Contains("|23|")) { ret += "Tri-Level, "; }
				if (AFrame.Contains("|24|")) { ret += "Tudor, "; }
				if (AFrame.Contains("|25|")) { ret += "Two Story, "; }
				if (AFrame.Contains("|26|")) { ret += "Victorian, "; }
				if (AFrame.Contains("|27|")) { ret += "Other/See Remarks, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string PresentUsage {
			get {
				string ret = "";
				if (AFrame.Contains("|28|")) { ret += "Agricultural, "; }
				if (AFrame.Contains("|29|")) { ret += "Commercial, "; }
				if (AFrame.Contains("|30|")) { ret += "Industrial, "; }
				if (AFrame.Contains("|31|")) { ret += "Mixed Use, "; }
				if (AFrame.Contains("|34|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|32|")) { ret += "Residential, "; }
				if (AFrame.Contains("|33|")) { ret += "Resort, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Condition {
			get {
				string ret = "";
				if (AFrame.Contains("|35|")) { ret += "Fair, "; }
				if (AFrame.Contains("|36|")) { ret += "Good, "; }
				if (AFrame.Contains("|37|")) { ret += "Handyman, "; }
				if (AFrame.Contains("|38|")) { ret += "Mixed, "; }
				if (AFrame.Contains("|39|")) { ret += "New, "; }
				if (AFrame.Contains("|44|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|40|")) { ret += "Poor, "; }
				if (AFrame.Contains("|41|")) { ret += "Renovated, "; }
				if (AFrame.Contains("|42|")) { ret += "To Be Built, "; }
				if (AFrame.Contains("|43|")) { ret += "Very Good, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Exterior {
			get {
				string ret = "";
				if (AFrame.Contains("|45|")) { ret += "Aluminum, "; }
				if (AFrame.Contains("|46|")) { ret += "Asbestos, "; }
				if (AFrame.Contains("|47|")) { ret += "Brick, "; }
				if (AFrame.Contains("|48|")) { ret += "Cedar, "; }
				if (AFrame.Contains("|49|")) { ret += "Cedar Shake, "; }
				if (AFrame.Contains("|50|")) { ret += "Clapboard, "; }
				if (AFrame.Contains("|51|")) { ret += "Log, "; }
				if (AFrame.Contains("|52|")) { ret += "Mixed, "; }
				if (AFrame.Contains("|58|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|53|")) { ret += "Stone, "; }
				if (AFrame.Contains("|54|")) { ret += "Stucco, "; }
				if (AFrame.Contains("|55|")) { ret += "T1-11, "; }
				if (AFrame.Contains("|56|")) { ret += "Vinyl, "; }
				if (AFrame.Contains("|57|")) { ret += "Wood, "; }
				if (AFrame.Contains("|566|")) { ret += "Aluminum Siding, "; }
				if (AFrame.Contains("|567|")) { ret += "Block, "; }
				if (AFrame.Contains("|568|")) { ret += "Brick, "; }
				if (AFrame.Contains("|569|")) { ret += "Cedar, "; }
				if (AFrame.Contains("|570|")) { ret += "Composite Wood, "; }
				if (AFrame.Contains("|571|")) { ret += "Concrete, "; }
				if (AFrame.Contains("|572|")) { ret += "Metal Siding, "; }
				if (AFrame.Contains("|577|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|573|")) { ret += "Stone, "; }
				if (AFrame.Contains("|574|")) { ret += "T1-11, "; }
				if (AFrame.Contains("|575|")) { ret += "Vinyl Siding, "; }
				if (AFrame.Contains("|576|")) { ret += "Wood Siding, "; }
				if (AFrame.Contains("|859|")) { ret += "Block, "; }
				if (AFrame.Contains("|860|")) { ret += "Concrete, "; }
				if (AFrame.Contains("|861|")) { ret += "Frame, "; }
				if (AFrame.Contains("|862|")) { ret += "Manufactured, "; }
				if (AFrame.Contains("|864|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|863|")) { ret += "Steel, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Roof {
			get {
				string ret = "";
				if (AFrame.Contains("|59|")) { ret += "Built Up, "; }
				if (AFrame.Contains("|60|")) { ret += "Flat, "; }
				if (AFrame.Contains("|66|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|61|")) { ret += "Pitched, "; }
				if (AFrame.Contains("|62|")) { ret += "Shingle, "; }
				if (AFrame.Contains("|63|")) { ret += "Slate, "; }
				if (AFrame.Contains("|64|")) { ret += "Tile, "; }
				if (AFrame.Contains("|65|")) { ret += "Tin, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Appliances {
			get {
				string ret = "";
				if (AFrame.Contains("|133|")) { ret += "Attic Fan, "; }
				if (AFrame.Contains("|134|")) { ret += "BBQ Grill, "; }
				if (AFrame.Contains("|135|")) { ret += "Ceiling Fan(s), "; }
				if (AFrame.Contains("|136|")) { ret += "Central A/C, "; }
				if (AFrame.Contains("|137|")) { ret += "Central Vacuum, "; }
				if (AFrame.Contains("|138|")) { ret += "Compactor, "; }
				if (AFrame.Contains("|139|")) { ret += "Countertop Range, "; }
				if (AFrame.Contains("|140|")) { ret += "Dishwasher, "; }
				if (AFrame.Contains("|141|")) { ret += "Disposal, "; }
				if (AFrame.Contains("|142|")) { ret += "Dryer, "; }
				if (AFrame.Contains("|143|")) { ret += "Freezer, "; }
				if (AFrame.Contains("|144|")) { ret += "Humidifier, "; }
				if (AFrame.Contains("|145|")) { ret += "Ice Maker, "; }
				if (AFrame.Contains("|146|")) { ret += "Intercom, "; }
				if (AFrame.Contains("|147|")) { ret += "Microwave, "; }
				if (AFrame.Contains("|160|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|148|")) { ret += "Refrigerator, "; }
				if (AFrame.Contains("|149|")) { ret += "Self Cleaning Oven, "; }
				if (AFrame.Contains("|150|")) { ret += "Sprinkler, "; }
				if (AFrame.Contains("|151|")) { ret += "Stove, "; }
				if (AFrame.Contains("|152|")) { ret += "Stove Top Grill, "; }
				if (AFrame.Contains("|153|")) { ret += "Timer Thermostat, "; }
				if (AFrame.Contains("|154|")) { ret += "Wall Oven, "; }
				if (AFrame.Contains("|155|")) { ret += "Washer, "; }
				if (AFrame.Contains("|156|")) { ret += "Water Filter, "; }
				if (AFrame.Contains("|157|")) { ret += "Water Softener, "; }
				if (AFrame.Contains("|159|")) { ret += "Win/Wall A/C, "; }
				if (AFrame.Contains("|158|")) { ret += "Wine Cooler, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Basement {
			get {
				string ret = "";
				if (AFrame.Contains("|118|")) { ret += "Above Grade, "; }
				if (AFrame.Contains("|119|")) { ret += "Below Grade, "; }
				if (AFrame.Contains("|120|")) { ret += "Block, "; }
				if (AFrame.Contains("|121|")) { ret += "Crawl, "; }
				if (AFrame.Contains("|122|")) { ret += "Finished, "; }
				if (AFrame.Contains("|123|")) { ret += "Full, "; }
				if (AFrame.Contains("|131|")) { ret += "None, "; }
				if (AFrame.Contains("|132|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|124|")) { ret += "Partial, "; }
				if (AFrame.Contains("|125|")) { ret += "Partially Finished, "; }
				if (AFrame.Contains("|126|")) { ret += "Piers, "; }
				if (AFrame.Contains("|127|")) { ret += "Poured, "; }
				if (AFrame.Contains("|128|")) { ret += "Slab, "; }
				if (AFrame.Contains("|129|")) { ret += "Stone, "; }
				if (AFrame.Contains("|130|")) { ret += "Walk-Out, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string CommonFacilities {
			get {
				string ret = "";
				if (AFrame.Contains("|289|")) { ret += "Clubhouse, "; }
				if (AFrame.Contains("|287|")) { ret += "Gym Facilities, "; }
				if (AFrame.Contains("|291|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|288|")) { ret += "Pool, "; }
				if (AFrame.Contains("|290|")) { ret += "Sports Courts, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Description {
			get {
				string ret = "";
				if (AFrame.Contains("|540|")) { ret += "Automotive, "; }
				if (AFrame.Contains("|541|")) { ret += "Bed & Breakfast/Inn, "; }
				if (AFrame.Contains("|542|")) { ret += "Building Only, "; }
				if (AFrame.Contains("|543|")) { ret += "Bungalow Colony, "; }
				if (AFrame.Contains("|544|")) { ret += "Business Only, "; }
				if (AFrame.Contains("|545|")) { ret += "Distribution, "; }
				if (AFrame.Contains("|546|")) { ret += "Farm, "; }
				if (AFrame.Contains("|547|")) { ret += "Food Services, "; }
				if (AFrame.Contains("|548|")) { ret += "Garage, "; }
				if (AFrame.Contains("|549|")) { ret += "Gas Station, "; }
				if (AFrame.Contains("|550|")) { ret += "Hotel/Motel, "; }
				if (AFrame.Contains("|551|")) { ret += "Industrial, "; }
				if (AFrame.Contains("|552|")) { ret += "Manufacturing, "; }
				if (AFrame.Contains("|553|")) { ret += "Mixed Use, "; }
				if (AFrame.Contains("|554|")) { ret += "Multi Dwellings, "; }
				if (AFrame.Contains("|555|")) { ret += "Multi Structures, "; }
				if (AFrame.Contains("|556|")) { ret += "Office, "; }
				if (AFrame.Contains("|557|")) { ret += "Offices w/ Apartments, "; }
				if (AFrame.Contains("|565|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|558|")) { ret += "Residence on Premises, "; }
				if (AFrame.Contains("|559|")) { ret += "Restaurant/Diner, "; }
				if (AFrame.Contains("|560|")) { ret += "Retail, "; }
				if (AFrame.Contains("|561|")) { ret += "Store Front, "; }
				if (AFrame.Contains("|562|")) { ret += "Store Front w/ Apartments, "; }
				if (AFrame.Contains("|563|")) { ret += "Transportation, "; }
				if (AFrame.Contains("|564|")) { ret += "Warehouse, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string DocumentsOnFile {
			get {
				string ret = "";
				if (AFrame.Contains("|422|")) { ret += "Codes,Covenants & Restrct, "; }
				if (AFrame.Contains("|423|")) { ret += "Deed, "; }
				if (AFrame.Contains("|424|")) { ret += "Disclosure, "; }
				if (AFrame.Contains("|425|")) { ret += "Engineering Docs, "; }
				if (AFrame.Contains("|428|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|426|")) { ret += "Survey, "; }
				if (AFrame.Contains("|427|")) { ret += "Tax Map, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Electric {
			get {
				string ret = "";
				if (AFrame.Contains("|67|")) { ret += "Circuit Breaker, "; }
				if (AFrame.Contains("|68|")) { ret += "Combo, "; }
				if (AFrame.Contains("|69|")) { ret += "Fuse Box, "; }
				if (AFrame.Contains("|70|")) { ret += "None, "; }
				if (AFrame.Contains("|71|")) { ret += "Other/See Remarks, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Equipment {
			get {
				string ret = "";
				if (AFrame.Contains("|600|")) { ret += "Agricultural Equipment, "; }
				if (AFrame.Contains("|601|")) { ret += "Central A/C, "; }
				if (AFrame.Contains("|602|")) { ret += "Chairlift, "; }
				if (AFrame.Contains("|603|")) { ret += "Counters, "; }
				if (AFrame.Contains("|604|")) { ret += "Elevators, "; }
				if (AFrame.Contains("|605|")) { ret += "Fire Alarm, "; }
				if (AFrame.Contains("|606|")) { ret += "Freight Elevator, "; }
				if (AFrame.Contains("|607|")) { ret += "Handicapped Access, "; }
				if (AFrame.Contains("|608|")) { ret += "Handicapped Facilities, "; }
				if (AFrame.Contains("|609|")) { ret += "O/H Doors 1, "; }
				if (AFrame.Contains("|610|")) { ret += "O/H Doors 2, "; }
				if (AFrame.Contains("|611|")) { ret += "O/H Doors 3+, "; }
				if (AFrame.Contains("|617|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|612|")) { ret += "Refrigeration, "; }
				if (AFrame.Contains("|613|")) { ret += "Security Lighting, "; }
				if (AFrame.Contains("|614|")) { ret += "Security System, "; }
				if (AFrame.Contains("|615|")) { ret += "Shelving, "; }
				if (AFrame.Contains("|616|")) { ret += "Sprinkler System, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string ExteriorAmenities {
			get {
				string ret = "";
				if (AFrame.Contains("|246|")) { ret += "Additional Land, "; }
				if (AFrame.Contains("|247|")) { ret += "Awnings, "; }
				if (AFrame.Contains("|248|")) { ret += "Barn, "; }
				if (AFrame.Contains("|249|")) { ret += "Community Facilities, "; }
				if (AFrame.Contains("|250|")) { ret += "Community Pool, "; }
				if (AFrame.Contains("|251|")) { ret += "Deck, "; }
				if (AFrame.Contains("|252|")) { ret += "Dock, "; }
				if (AFrame.Contains("|253|")) { ret += "Garage Opener, "; }
				if (AFrame.Contains("|254|")) { ret += "Gated Community, "; }
				if (AFrame.Contains("|255|")) { ret += "Guest Cottage, "; }
				if (AFrame.Contains("|256|")) { ret += "Handicap Access, "; }
				if (AFrame.Contains("|266|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|257|")) { ret += "Outbuilding, "; }
				if (AFrame.Contains("|258|")) { ret += "Patio, "; }
				if (AFrame.Contains("|259|")) { ret += "Pool Above Ground, "; }
				if (AFrame.Contains("|260|")) { ret += "Pool Equipment, "; }
				if (AFrame.Contains("|261|")) { ret += "Pool In Ground, "; }
				if (AFrame.Contains("|262|")) { ret += "Porch, "; }
				if (AFrame.Contains("|263|")) { ret += "Screens, "; }
				if (AFrame.Contains("|264|")) { ret += "Storm Windows, "; }
				if (AFrame.Contains("|265|")) { ret += "Swing Set, "; }
				if (AFrame.Contains("|586|")) { ret += "Alley, "; }
				if (AFrame.Contains("|587|")) { ret += "Exterior Lighting, "; }
				if (AFrame.Contains("|588|")) { ret += "Fenced, "; }
				if (AFrame.Contains("|589|")) { ret += "Fire Escape, "; }
				if (AFrame.Contains("|590|")) { ret += "Handicap Ramp, "; }
				if (AFrame.Contains("|591|")) { ret += "Landscaping, "; }
				if (AFrame.Contains("|593|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|592|")) { ret += "Underground Tank, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string FeaturesAmenities {
			get {
				string ret = "";
				if (AFrame.Contains("|871|")) { ret += "Additional Land, "; }
				if (AFrame.Contains("|872|")) { ret += "Alarm, "; }
				if (AFrame.Contains("|873|")) { ret += "Amenity Package, "; }
				if (AFrame.Contains("|874|")) { ret += "Attic, "; }
				if (AFrame.Contains("|875|")) { ret += "Awning, "; }
				if (AFrame.Contains("|876|")) { ret += "Cathedral Ceiling, "; }
				if (AFrame.Contains("|877|")) { ret += "Central Hall/Foyer, "; }
				if (AFrame.Contains("|878|")) { ret += "Community Pool, "; }
				if (AFrame.Contains("|879|")) { ret += "Deck, "; }
				if (AFrame.Contains("|880|")) { ret += "Dock, "; }
				if (AFrame.Contains("|881|")) { ret += "Elevator, "; }
				if (AFrame.Contains("|882|")) { ret += "Family Room, "; }
				if (AFrame.Contains("|883|")) { ret += "Fire Escape, "; }
				if (AFrame.Contains("|884|")) { ret += "Fireplace, "; }
				if (AFrame.Contains("|885|")) { ret += "Fireplace Equipment, "; }
				if (AFrame.Contains("|886|")) { ret += "Formal Dining Room, "; }
				if (AFrame.Contains("|887|")) { ret += "Furniture, "; }
				if (AFrame.Contains("|888|")) { ret += "Garage Opener, "; }
				if (AFrame.Contains("|889|")) { ret += "Glass, "; }
				if (AFrame.Contains("|890|")) { ret += "Great Room, "; }
				if (AFrame.Contains("|891|")) { ret += "Guest Cottage, "; }
				if (AFrame.Contains("|892|")) { ret += "Handicap Access, "; }
				if (AFrame.Contains("|893|")) { ret += "Handicap Facilities, "; }
				if (AFrame.Contains("|894|")) { ret += "Hardwood Floors, "; }
				if (AFrame.Contains("|895|")) { ret += "Hot Tub/Jacuzzi, "; }
				if (AFrame.Contains("|896|")) { ret += "Insulated Windows, "; }
				if (AFrame.Contains("|897|")) { ret += "Master Suite, "; }
				if (AFrame.Contains("|916|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|898|")) { ret += "Patio, "; }
				if (AFrame.Contains("|899|")) { ret += "Porch, "; }
				if (AFrame.Contains("|900|")) { ret += "Professional Suite, "; }
				if (AFrame.Contains("|901|")) { ret += "Screens, "; }
				if (AFrame.Contains("|902|")) { ret += "Security Lighting, "; }
				if (AFrame.Contains("|903|")) { ret += "Security System, "; }
				if (AFrame.Contains("|906|")) { ret += "Skylights, "; }
				if (AFrame.Contains("|904|")) { ret += "Sprinkler System, "; }
				if (AFrame.Contains("|905|")) { ret += "Storm Windows, "; }
				if (AFrame.Contains("|907|")) { ret += "Swing Set, "; }
				if (AFrame.Contains("|911|")) { ret += "Track Lighting, "; }
				if (AFrame.Contains("|908|")) { ret += "TV Antenna, "; }
				if (AFrame.Contains("|909|")) { ret += "TV Cable, "; }
				if (AFrame.Contains("|910|")) { ret += "TV Satellite Dish, "; }
				if (AFrame.Contains("|912|")) { ret += "Wall To Wall Carpet, "; }
				if (AFrame.Contains("|913|")) { ret += "Wet Bar, "; }
				if (AFrame.Contains("|914|")) { ret += "Window Treatments, "; }
				if (AFrame.Contains("|915|")) { ret += "Woodwork, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Floors {
			get {
				string ret = "";
				if (AFrame.Contains("|594|")) { ret += "Carpet, "; }
				if (AFrame.Contains("|595|")) { ret += "Concrete, "; }
				if (AFrame.Contains("|599|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|596|")) { ret += "Tile, "; }
				if (AFrame.Contains("|597|")) { ret += "Vinyl, "; }
				if (AFrame.Contains("|598|")) { ret += "Wood, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Foundation {
			get {
				string ret = "";
				if (AFrame.Contains("|1|")) { ret += "A-Frame, "; }
				if (AFrame.Contains("|579|")) { ret += "Block, "; }
				if (AFrame.Contains("|580|")) { ret += "Full, "; }
				if (AFrame.Contains("|585|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|581|")) { ret += "Partial, "; }
				if (AFrame.Contains("|582|")) { ret += "Pier & Post, "; }
				if (AFrame.Contains("|583|")) { ret += "Slab, "; }
				if (AFrame.Contains("|584|")) { ret += "Stone, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Fuel {
			get {
				string ret = "";
				if (AFrame.Contains("|72|")) { ret += "Electric, "; }
				if (AFrame.Contains("|76|")) { ret += "Fuel Tank Above Ground, "; }
				if (AFrame.Contains("|77|")) { ret += "Fuel Tank Below Ground, "; }
				if (AFrame.Contains("|73|")) { ret += "LP Gas, "; }
				if (AFrame.Contains("|74|")) { ret += "Oil, "; }
				if (AFrame.Contains("|79|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|75|")) { ret += "Solar, "; }
				if (AFrame.Contains("|78|")) { ret += "Wood, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Garbage {
			get {
				string ret = "";
				if (AFrame.Contains("|865|")) { ret += "Municipal, "; }
				if (AFrame.Contains("|866|")) { ret += "Private, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Heating {
			get {
				string ret = "";
				if (AFrame.Contains("|101|")) { ret += "Baseboard, "; }
				if (AFrame.Contains("|102|")) { ret += "Electric, "; }
				if (AFrame.Contains("|103|")) { ret += "Electric Thermal Storage, "; }
				if (AFrame.Contains("|104|")) { ret += "Forced Warm Air, "; }
				if (AFrame.Contains("|105|")) { ret += "Heat Pump(s), "; }
				if (AFrame.Contains("|106|")) { ret += "Hot Water, "; }
				if (AFrame.Contains("|116|")) { ret += "None, "; }
				if (AFrame.Contains("|117|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|107|")) { ret += "Pellet Stove, "; }
				if (AFrame.Contains("|108|")) { ret += "Radiant Heat, "; }
				if (AFrame.Contains("|109|")) { ret += "Radiator, "; }
				if (AFrame.Contains("|110|")) { ret += "Solar, "; }
				if (AFrame.Contains("|111|")) { ret += "Space Heater, "; }
				if (AFrame.Contains("|112|")) { ret += "Steam, "; }
				if (AFrame.Contains("|113|")) { ret += "Wood Furnace, "; }
				if (AFrame.Contains("|114|")) { ret += "Wood Stove, "; }
				if (AFrame.Contains("|115|")) { ret += "Zoned, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string InteriorAmenities {
			get {
				string ret = "";
				if (AFrame.Contains("|213|")) { ret += "Alarm, "; }
				if (AFrame.Contains("|214|")) { ret += "Amenity Package, "; }
				if (AFrame.Contains("|215|")) { ret += "Attic, "; }
				if (AFrame.Contains("|216|")) { ret += "Cathedral Ceiling, "; }
				if (AFrame.Contains("|217|")) { ret += "Central Hall/Foyer, "; }
				if (AFrame.Contains("|218|")) { ret += "Family Room, "; }
				if (AFrame.Contains("|219|")) { ret += "Fireplace, "; }
				if (AFrame.Contains("|220|")) { ret += "Fireplace Equipment, "; }
				if (AFrame.Contains("|221|")) { ret += "Formal Dining Room, "; }
				if (AFrame.Contains("|222|")) { ret += "Furniture, "; }
				if (AFrame.Contains("|223|")) { ret += "Glass, "; }
				if (AFrame.Contains("|224|")) { ret += "Great Room, "; }
				if (AFrame.Contains("|225|")) { ret += "Handicap Accessible, "; }
				if (AFrame.Contains("|226|")) { ret += "Hardwood Floors, "; }
				if (AFrame.Contains("|227|")) { ret += "Hot Tub/Jacuzzi, "; }
				if (AFrame.Contains("|228|")) { ret += "Insulated Windows, "; }
				if (AFrame.Contains("|229|")) { ret += "Master Suite, "; }
				if (AFrame.Contains("|245|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|230|")) { ret += "Pellet Stove, "; }
				if (AFrame.Contains("|231|")) { ret += "Professional Suite, "; }
				if (AFrame.Contains("|232|")) { ret += "Skylights, "; }
				if (AFrame.Contains("|233|")) { ret += "Stone, "; }
				if (AFrame.Contains("|234|")) { ret += "Sump Pump, "; }
				if (AFrame.Contains("|235|")) { ret += "Tile, "; }
				if (AFrame.Contains("|236|")) { ret += "Track Lighting, "; }
				if (AFrame.Contains("|237|")) { ret += "TV Antenna, "; }
				if (AFrame.Contains("|238|")) { ret += "TV Cable, "; }
				if (AFrame.Contains("|239|")) { ret += "TV Satellite, "; }
				if (AFrame.Contains("|240|")) { ret += "Wall/Wall Carpet, "; }
				if (AFrame.Contains("|241|")) { ret += "Wet Bar, "; }
				if (AFrame.Contains("|242|")) { ret += "Window Treatment, "; }
				if (AFrame.Contains("|243|")) { ret += "Wood Stove, "; }
				if (AFrame.Contains("|244|")) { ret += "Woodwork, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string LandCondition {
			get {
				string ret = "";
				if (AFrame.Contains("|400|")) { ret += "Below Grade, "; }
				if (AFrame.Contains("|401|")) { ret += "Cleared, "; }
				if (AFrame.Contains("|402|")) { ret += "Dry, "; }
				if (AFrame.Contains("|403|")) { ret += "Gentle Sloping, "; }
				if (AFrame.Contains("|404|")) { ret += "Level, "; }
				if (AFrame.Contains("|405|")) { ret += "Mixed, "; }
				if (AFrame.Contains("|410|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|406|")) { ret += "Pasture/Field, "; }
				if (AFrame.Contains("|407|")) { ret += "Rugged, "; }
				if (AFrame.Contains("|408|")) { ret += "Tillable, "; }
				if (AFrame.Contains("|409|")) { ret += "Wet, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string LandFeatures {
			get {
				string ret = "";
				if (AFrame.Contains("|182|")) { ret += "Circular Driveway, "; }
				if (AFrame.Contains("|183|")) { ret += "Cleared, "; }
				if (AFrame.Contains("|184|")) { ret += "Corner, "; }
				if (AFrame.Contains("|185|")) { ret += "Cul-De-Sac, "; }
				if (AFrame.Contains("|186|")) { ret += "Fenced/Enclosed, "; }
				if (AFrame.Contains("|187|")) { ret += "Fruit Trees, "; }
				if (AFrame.Contains("|188|")) { ret += "Gently Sloping, "; }
				if (AFrame.Contains("|189|")) { ret += "Hunting, "; }
				if (AFrame.Contains("|190|")) { ret += "Lake Rights, "; }
				if (AFrame.Contains("|191|")) { ret += "Lakefront, "; }
				if (AFrame.Contains("|192|")) { ret += "Level, "; }
				if (AFrame.Contains("|193|")) { ret += "Mature Trees, "; }
				if (AFrame.Contains("|194|")) { ret += "Motor Boating, "; }
				if (AFrame.Contains("|212|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|195|")) { ret += "Pasture, "; }
				if (AFrame.Contains("|196|")) { ret += "Paved Driveway, "; }
				if (AFrame.Contains("|197|")) { ret += "Pond, "; }
				if (AFrame.Contains("|198|")) { ret += "Pond Site, "; }
				if (AFrame.Contains("|199|")) { ret += "Private, "; }
				if (AFrame.Contains("|200|")) { ret += "Professional Landscaping, "; }
				if (AFrame.Contains("|202|")) { ret += "River Rights, "; }
				if (AFrame.Contains("|201|")) { ret += "Riverfront, "; }
				if (AFrame.Contains("|203|")) { ret += "Rugged, "; }
				if (AFrame.Contains("|204|")) { ret += "Secluded, "; }
				if (AFrame.Contains("|1299|")) { ret += "Signed Gas Lease, "; }
				if (AFrame.Contains("|205|")) { ret += "Stream Front, "; }
				if (AFrame.Contains("|206|")) { ret += "Sub Dividable, "; }
				if (AFrame.Contains("|207|")) { ret += "Vegetable Garden, "; }
				if (AFrame.Contains("|208|")) { ret += "View, "; }
				if (AFrame.Contains("|209|")) { ret += "Water View, "; }
				if (AFrame.Contains("|210|")) { ret += "Wetlands, "; }
				if (AFrame.Contains("|211|")) { ret += "Wooded, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string LandUseRestrictions {
			get {
				string ret = "";
				if (AFrame.Contains("|413|")) { ret += "Building Moratorium, "; }
				if (AFrame.Contains("|414|")) { ret += "DEC Approval, "; }
				if (AFrame.Contains("|415|")) { ret += "Deed Restriction/Covenant, "; }
				if (AFrame.Contains("|416|")) { ret += "Easement, "; }
				if (AFrame.Contains("|417|")) { ret += "Flood Plain, "; }
				if (AFrame.Contains("|421|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|418|")) { ret += "Perc Test Needed, "; }
				if (AFrame.Contains("|419|")) { ret += "Variance Required, "; }
				if (AFrame.Contains("|420|")) { ret += "Wetlands/Fresh, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Near {
			get {
				string ret = "";
				if (AFrame.Contains("|161|")) { ret += "Golf, "; }
				if (AFrame.Contains("|162|")) { ret += "Hotels, "; }
				if (AFrame.Contains("|163|")) { ret += "Lake/Stream, "; }
				if (AFrame.Contains("|173|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|164|")) { ret += "Parks, "; }
				if (AFrame.Contains("|165|")) { ret += "Performing Arts Center, "; }
				if (AFrame.Contains("|166|")) { ret += "Public Transportation, "; }
				if (AFrame.Contains("|167|")) { ret += "Race Track, "; }
				if (AFrame.Contains("|168|")) { ret += "Racing, "; }
				if (AFrame.Contains("|169|")) { ret += "River, "; }
				if (AFrame.Contains("|170|")) { ret += "Schools, "; }
				if (AFrame.Contains("|171|")) { ret += "Shopping, "; }
				if (AFrame.Contains("|172|")) { ret += "State Land, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Options {
			get {
				string ret = "";
				if (AFrame.Contains("|285|")) { ret += "Rent Only, "; }
				if (AFrame.Contains("|284|")) { ret += "Rent with Options, "; }
				if (AFrame.Contains("|286|")) { ret += "Sale, "; }
				if (AFrame.Contains("|309|")) { ret += "Rent Only, "; }
				if (AFrame.Contains("|308|")) { ret += "Rent with Options, "; }
				if (AFrame.Contains("|310|")) { ret += "Sale Only, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
        public string Pool {
            get {
                string ret = "";
                if(AFrame.Contains("|250|")) { ret += "Community Pool, "; }
                if(AFrame.Contains("|259|")) { ret += "Above Ground, "; }
                if(AFrame.Contains("|261|")) { ret += "In Ground, "; }
                if(ret.Length > 2) {
                    return ret.Substring(0, ret.Length - 2);
                } else {
                    return "No";
                }
            }
        }
        public string PossibleFinancing {
			get {
				string ret = "";
				if (AFrame.Contains("|276|")) { ret += "Assumable, "; }
				if (AFrame.Contains("|277|")) { ret += "Conventional, "; }
				if (AFrame.Contains("|278|")) { ret += "Creative, "; }
				if (AFrame.Contains("|279|")) { ret += "Exchange, "; }
				if (AFrame.Contains("|280|")) { ret += "FHA/VA, "; }
				if (AFrame.Contains("|283|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|281|")) { ret += "Owner First, "; }
				if (AFrame.Contains("|282|")) { ret += "Owner Second, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Road {
			get {
				string ret = "";
				if (AFrame.Contains("|174|")) { ret += "County, "; }
				if (AFrame.Contains("|175|")) { ret += "Deeded Right of Way, "; }
				if (AFrame.Contains("|181|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|176|")) { ret += "Paved, "; }
				if (AFrame.Contains("|177|")) { ret += "Private, "; }
				if (AFrame.Contains("|178|")) { ret += "State, "; }
				if (AFrame.Contains("|179|")) { ret += "Town/Village, "; }
				if (AFrame.Contains("|180|")) { ret += "Unpaved, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Sewer {
			get {
				string ret = "";
				if (AFrame.Contains("|91|")) { ret += "Cesspool, "; }
				if (AFrame.Contains("|88|")) { ret += "Community Sewer, "; }
				if (AFrame.Contains("|89|")) { ret += "Municipal Sewer, "; }
				if (AFrame.Contains("|92|")) { ret += "None, "; }
				if (AFrame.Contains("|93|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|90|")) { ret += "Septic, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string ToShow {
			get {
				string ret = "";
				if (AFrame.Contains("|267|")) { ret += "By Appointment, "; }
				if (AFrame.Contains("|268|")) { ret += "Call Broker, "; }
				if (AFrame.Contains("|269|")) { ret += "Call Owner, "; }
				if (AFrame.Contains("|270|")) { ret += "Caution Pets, "; }
				if (AFrame.Contains("|271|")) { ret += "Go Direct, "; }
				if (AFrame.Contains("|272|")) { ret += "Key At Broker, "; }
				if (AFrame.Contains("|273|")) { ret += "Lock Box, "; }
				if (AFrame.Contains("|275|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|274|")) { ret += "Send Show Notice, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Water {
			get {
				string ret = "";
				if (AFrame.Contains("|80|")) { ret += "Community, "; }
				if (AFrame.Contains("|84|")) { ret += "Drilled Well, "; }
				if (AFrame.Contains("|83|")) { ret += "Dug Well, "; }
				if (AFrame.Contains("|81|")) { ret += "Municipal, "; }
				if (AFrame.Contains("|86|")) { ret += "None, "; }
				if (AFrame.Contains("|87|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|85|")) { ret += "Shared Well, "; }
				if (AFrame.Contains("|82|")) { ret += "Spring, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string WaterHeater {
			get {
				string ret = "";
				if (AFrame.Contains("|94|")) { ret += "Electric, "; }
				if (AFrame.Contains("|95|")) { ret += "Gas, "; }
				if (AFrame.Contains("|96|")) { ret += "Oil, "; }
				if (AFrame.Contains("|100|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|97|")) { ret += "Storage, "; }
				if (AFrame.Contains("|98|")) { ret += "Tankless, "; }
				if (AFrame.Contains("|99|")) { ret += "Wood, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}

		public string ImageURL { get; set; }
		public string LinkURL { get; set; }
		public string FavoriteDescription { get; set; }

		#endregion

		#region Constructor
		public Listing() { }
		public Listing(string mls) {
			MLS = mls;
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT [MLS #],[Class],[Type],[Township],[Asking Price],[Address Number],[Address Direction]" +
						",[Address Street],[Address 2],[City],[State],[Zip],[Status],[Sale/Rent],[# Rooms],[# Bedrooms],[# Full Baths]" +
						",[# Half Baths],[Garage Type],[Garage Capacity],[Fireplace],[Listing Type],[Number of Acres],[Agent ID]" +
						",[Agent Name],[Agent Phone],[Listing Office 1 ID],[Listing Office 1 Name],[Listing Office 1 Phone],[County]" +
						",[School District],[Development],[Approx Year Built],[Approx Lot Size-Front],[Approx Lot Size-Side],[Approx SqFt Above Grade]" +
						",[Approx SqFt Below Grade],[Approx Total Square Feet],[Approx Total Taxes],[Tax Exemption Y/N],[Land Value],[Total Value]" +
						",[Possession Date],[Maintenance Fees (Year)],[Utilities (Month)],[Homeowners Fees (Year)],[Financing Terms],[Remarks]" +
						",[A-Frame],[Virtual Tour] FROM [listings-residential] WHERE [MLS #] = @MLS";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("MLS", SqlDbType.Float).Value = Convert.ToDouble(MLS);
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						if (dr.HasRows) {
							dr.Read();
							MLS = dr[0].ToString();
							Class = dr[1].ToString();
							Type = dr[2].ToString();
							Township = dr[3].ToString();
							AskingPrice = string.Format("{0:$0,000}", dr[4]);
							float temp = 0;
							float.TryParse(dr[4].ToString(), out temp);
							AskingAmt = temp;
							AddressNumber = dr[5].ToString();
							AddressDirection = dr[6].ToString();
							AddressStreet = dr[7].ToString();
							Address2 = dr[8].ToString();
							City = dr[9].ToString();
							State = dr[10].ToString();
							Zip = dr[11].ToString();
							Status = dr[12].ToString();
							SaleRent = dr[13].ToString();
							Rooms = dr[14].ToString();
							Bedrooms = dr[15].ToString();
							FullBaths = dr[16].ToString();
							HalfBaths = dr[17].ToString();
							GarageType = dr[18].ToString();
							GarageCapacity = dr[19].ToString();
							Fireplace = dr[20].ToString();
							ListingType = dr[21].ToString();
							Acres = dr[22].ToString();
							AgentID = dr[23].ToString();
							AgentName = dr[24].ToString();
							AgentPhone = dr[25].ToString();
							ListingOfficeID = dr[26].ToString();
							ListingOfficeName = dr[27].ToString();
							ListingOfficePhone = dr[28].ToString();
							County = dr[29].ToString();
							SchoolDistrict = dr[30].ToString();
							Development = dr[31].ToString();
							YearBuilt = dr[32].ToString();
							LotSizeFront = dr[33].ToString();
							LotSizeSide = dr[34].ToString();
							SqFtAboveGrade = string.Format("{0:0,0}", dr[35]);
							SqFtBelowGrade = string.Format("{0:0,0}", dr[36]);
							TotalSquareFeet = string.Format("{0:0,0}", dr[37]);
							TotalTaxes = string.Format("{0:$0,0}", dr[38]);
							TaxExemption = dr[39].ToString();
							LandValue = string.Format("{0:$0,0}", dr[40]);
							TotalValue = string.Format("{0:$0,0}", dr[41]);
							if (!string.IsNullOrEmpty(LandValue) && !string.IsNullOrEmpty(TotalValue)) {
								BldgValue = string.Format("{0:$0,0}", Convert.ToDouble(dr[41]) - Convert.ToDouble(dr[40]));
							}
							PossessionDate = dr[42].ToString();
							MaintenanceFees = string.Format("{0:$0,0}", dr[43]);
							Utilities = string.Format("{0:$0,0}", dr[44]);
							HomeownersFees = string.Format("{0:$0,0}", dr[45]);
							FinancingTerms = dr[46].ToString();
							Remarks = dr[47].ToString();
							AFrame = "|" + dr[48].ToString() + "|";
							VirtualTour = dr[49].ToString();
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
	public class ListingMap {

		#region Public Methods
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
		public string MLS { get; set; }
		public string Class { get; set; }
		public string Type { get; set; }
		public string Township { get; set; }
		public float AskingAmt { get; set; }
		public string AskingPrice { get; set; }
		public string AddressNumber { get; set; }
		public string AddressDirection { get; set; }
		public string AddressStreet { get; set; }
		public string Address2 { get; set; }
		public string AddressOneLine {
			get {
				string ret = "";
				if (!string.IsNullOrEmpty(AddressNumber)) {
					ret += AddressNumber + " ";
				}
				if (!string.IsNullOrEmpty(AddressDirection)) {
					ret += AddressDirection + " ";
				}
				if (!string.IsNullOrEmpty(AddressStreet)) {
					ret += AddressStreet + " ";
				}
				if (ret.Length > 0) { ret = ret.Substring(0, ret.Length - 1); }
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
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Status { get; set; }
		public string SaleRent { get; set; }
		public string Rooms { get; set; }
		public string Bedrooms { get; set; }
		public string FullBaths { get; set; }
		public string HalfBaths { get; set; }
		public string TotalBaths { get { return FullBaths + " full, " + HalfBaths + " half"; } }
		public string GarageType { get; set; }
		public string GarageCapacity { get; set; }
		public string Fireplace { get; set; }
		public string ListingType { get; set; }
		public string Acres { get; set; }
		public string AgentID { get; set; }
		public string AgentName { get; set; }
		public string AgentPhone { get; set; }
		public string ListingOfficeID { get; set; }
		public string ListingOfficeName { get; set; }
		public string ListingOfficePhone { get; set; }
		public string County { get; set; }
		public string SchoolDistrict { get; set; }
		public string Development { get; set; }
		public string YearBuilt { get; set; }
		public string LotSizeFront { get; set; }
		public string LotSizeSide { get; set; }
		public string SqFtAboveGrade { get; set; }
		public string SqFtBelowGrade { get; set; }
		public string TotalSquareFeet { get; set; }
		public string TotalTaxes { get; set; }
		public string TaxExemption { get; set; }
		public string LandValue { get; set; }
		public string BldgValue { get; set; }
		public string TotalValue { get; set; }
		public string PossessionDate { get; set; }
		public string MaintenanceFees { get; set; }
		public string Utilities { get; set; }
		public string HomeownersFees { get; set; }
		public string FinancingTerms { get; set; }
		public string Remarks { get; set; }
		public string AFrame { get; set; }
		public string VirtualTour { get; set; }

		public float Lat { get; set; }
		public float Long { get; set; }

		public string ImageFirst {
			get {
				if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath("/img/listings"), "SCMLS" + MLS.ToString() + ".jpg"))) {
					return "SCMLS" + MLS.ToString() + ".jpg";
				} else {
					string[] temp = Images;
					if (temp.Length > 0) {
						return temp[0].Substring(temp[0].LastIndexOf("\\") + 1).ToLower();
					} else {
						return "";
					}
				}
			}
		}
		public string[] Images {
			get {
				string[] temp = Directory.GetFiles(HttpContext.Current.Server.MapPath("/img/listings"), "SCMLS" + MLS.ToString() + "*.*");
				Array.Sort(temp);
				for (int x = 0; x < temp.Length; x++) {
					string cur = temp[x];
					temp[x] = cur.Substring(cur.LastIndexOf("\\") + 1).ToLower();
				}
				return temp;
			}
		}

		public string Style {
			get {
				string ret = "";
				if (AFrame.Contains("|1|")) { ret += "A-Frame, "; }
				if (AFrame.Contains("|2|")) { ret += "Bi-Level, "; }
				if (AFrame.Contains("|3|")) { ret += "Bungalow, "; }
				if (AFrame.Contains("|4|")) { ret += "Cape Cod, "; }
				if (AFrame.Contains("|8|")) { ret += "Co-Op, "; }
				if (AFrame.Contains("|5|")) { ret += "Colonial, "; }
				if (AFrame.Contains("|6|")) { ret += "Condo, "; }
				if (AFrame.Contains("|7|")) { ret += "Contemporary, "; }
				if (AFrame.Contains("|9|")) { ret += "Cottage, "; }
				if (AFrame.Contains("|10|")) { ret += "Custom, "; }
				if (AFrame.Contains("|11|")) { ret += "Double-Wide Mobile, "; }
				if (AFrame.Contains("|12|")) { ret += "Dutch Colonial, "; }
				if (AFrame.Contains("|13|")) { ret += "Farm House, "; }
				if (AFrame.Contains("|14|")) { ret += "Log Cabin, "; }
				if (AFrame.Contains("|15|")) { ret += "Mobile Home, "; }
				if (AFrame.Contains("|16|")) { ret += "Modular, "; }
				if (AFrame.Contains("|27|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|17|")) { ret += "Ranch, "; }
				if (AFrame.Contains("|18|")) { ret += "Ranch-Raised, "; }
				if (AFrame.Contains("|19|")) { ret += "Salt Box, "; }
				if (AFrame.Contains("|20|")) { ret += "Single-Wide Mobile, "; }
				if (AFrame.Contains("|21|")) { ret += "Split, "; }
				if (AFrame.Contains("|22|")) { ret += "Townhouse, "; }
				if (AFrame.Contains("|23|")) { ret += "Tri-Level, "; }
				if (AFrame.Contains("|24|")) { ret += "Tudor, "; }
				if (AFrame.Contains("|25|")) { ret += "Two Story, "; }
				if (AFrame.Contains("|26|")) { ret += "Victorian, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string PresentUsage {
			get {
				string ret = "";
				if (AFrame.Contains("|28|")) { ret += "Agricultural, "; }
				if (AFrame.Contains("|29|")) { ret += "Commercial, "; }
				if (AFrame.Contains("|30|")) { ret += "Industrial, "; }
				if (AFrame.Contains("|31|")) { ret += "Mixed Use, "; }
				if (AFrame.Contains("|34|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|32|")) { ret += "Residential, "; }
				if (AFrame.Contains("|33|")) { ret += "Resort, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Condition {
			get {
				string ret = "";
				if (AFrame.Contains("|35|")) { ret += "Fair, "; }
				if (AFrame.Contains("|36|")) { ret += "Good, "; }
				if (AFrame.Contains("|37|")) { ret += "Handyman, "; }
				if (AFrame.Contains("|38|")) { ret += "Mixed, "; }
				if (AFrame.Contains("|39|")) { ret += "New, "; }
				if (AFrame.Contains("|44|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|40|")) { ret += "Poor, "; }
				if (AFrame.Contains("|41|")) { ret += "Renovated, "; }
				if (AFrame.Contains("|42|")) { ret += "To Be Built, "; }
				if (AFrame.Contains("|43|")) { ret += "Very Good, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Exterior {
			get {
				string ret = "";
				if (AFrame.Contains("|45|")) { ret += "Aluminum, "; }
				if (AFrame.Contains("|46|")) { ret += "Asbestos, "; }
				if (AFrame.Contains("|47|")) { ret += "Brick, "; }
				if (AFrame.Contains("|48|")) { ret += "Cedar, "; }
				if (AFrame.Contains("|49|")) { ret += "Cedar Shake, "; }
				if (AFrame.Contains("|50|")) { ret += "Clapboard, "; }
				if (AFrame.Contains("|51|")) { ret += "Log, "; }
				if (AFrame.Contains("|52|")) { ret += "Mixed, "; }
				if (AFrame.Contains("|58|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|53|")) { ret += "Stone, "; }
				if (AFrame.Contains("|54|")) { ret += "Stucco, "; }
				if (AFrame.Contains("|55|")) { ret += "T1-11, "; }
				if (AFrame.Contains("|56|")) { ret += "Vinyl, "; }
				if (AFrame.Contains("|57|")) { ret += "Wood, "; }
				if (AFrame.Contains("|566|")) { ret += "Aluminum Siding, "; }
				if (AFrame.Contains("|567|")) { ret += "Block, "; }
				if (AFrame.Contains("|568|")) { ret += "Brick, "; }
				if (AFrame.Contains("|569|")) { ret += "Cedar, "; }
				if (AFrame.Contains("|570|")) { ret += "Composite Wood, "; }
				if (AFrame.Contains("|571|")) { ret += "Concrete, "; }
				if (AFrame.Contains("|572|")) { ret += "Metal Siding, "; }
				if (AFrame.Contains("|577|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|573|")) { ret += "Stone, "; }
				if (AFrame.Contains("|574|")) { ret += "T1-11, "; }
				if (AFrame.Contains("|575|")) { ret += "Vinyl Siding, "; }
				if (AFrame.Contains("|576|")) { ret += "Wood Siding, "; }
				if (AFrame.Contains("|859|")) { ret += "Block, "; }
				if (AFrame.Contains("|860|")) { ret += "Concrete, "; }
				if (AFrame.Contains("|861|")) { ret += "Frame, "; }
				if (AFrame.Contains("|862|")) { ret += "Manufactured, "; }
				if (AFrame.Contains("|864|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|863|")) { ret += "Steel, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Roof {
			get {
				string ret = "";
				if (AFrame.Contains("|59|")) { ret += "Built Up, "; }
				if (AFrame.Contains("|60|")) { ret += "Flat, "; }
				if (AFrame.Contains("|66|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|61|")) { ret += "Pitched, "; }
				if (AFrame.Contains("|62|")) { ret += "Shingle, "; }
				if (AFrame.Contains("|63|")) { ret += "Slate, "; }
				if (AFrame.Contains("|64|")) { ret += "Tile, "; }
				if (AFrame.Contains("|65|")) { ret += "Tin, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Electric {
			get {
				string ret = "";
				if (AFrame.Contains("|67|")) { ret += "Circuit Breaker, "; }
				if (AFrame.Contains("|68|")) { ret += "Combo, "; }
				if (AFrame.Contains("|69|")) { ret += "Fuse Box, "; }
				if (AFrame.Contains("|70|")) { ret += "None, "; }
				if (AFrame.Contains("|71|")) { ret += "Other/See Remarks, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Fuel {
			get {
				string ret = "";
				if (AFrame.Contains("|72|")) { ret += "Electric, "; }
				if (AFrame.Contains("|76|")) { ret += "Fuel Tank Above Ground, "; }
				if (AFrame.Contains("|77|")) { ret += "Fuel Tank Below Ground, "; }
				if (AFrame.Contains("|73|")) { ret += "LP Gas, "; }
				if (AFrame.Contains("|74|")) { ret += "Oil, "; }
				if (AFrame.Contains("|79|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|75|")) { ret += "Solar, "; }
				if (AFrame.Contains("|78|")) { ret += "Wood, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Water {
			get {
				string ret = "";
				if (AFrame.Contains("|80|")) { ret += "Community, "; }
				if (AFrame.Contains("|84|")) { ret += "Drilled Well, "; }
				if (AFrame.Contains("|83|")) { ret += "Dug Well, "; }
				if (AFrame.Contains("|81|")) { ret += "Municipal, "; }
				if (AFrame.Contains("|86|")) { ret += "None, "; }
				if (AFrame.Contains("|87|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|85|")) { ret += "Shared Well, "; }
				if (AFrame.Contains("|82|")) { ret += "Spring, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Sewer {
			get {
				string ret = "";
				if (AFrame.Contains("|91|")) { ret += "Cesspool, "; }
				if (AFrame.Contains("|88|")) { ret += "Community Sewer, "; }
				if (AFrame.Contains("|89|")) { ret += "Municipal Sewer, "; }
				if (AFrame.Contains("|92|")) { ret += "None, "; }
				if (AFrame.Contains("|93|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|90|")) { ret += "Septic, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string WaterHeater {
			get {
				string ret = "";
				if (AFrame.Contains("|94|")) { ret += "Electric, "; }
				if (AFrame.Contains("|95|")) { ret += "Gas, "; }
				if (AFrame.Contains("|96|")) { ret += "Oil, "; }
				if (AFrame.Contains("|100|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|97|")) { ret += "Storage, "; }
				if (AFrame.Contains("|98|")) { ret += "Tankless, "; }
				if (AFrame.Contains("|99|")) { ret += "Wood, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Heating {
			get {
				string ret = "";
				if (AFrame.Contains("|101|")) { ret += "Baseboard, "; }
				if (AFrame.Contains("|102|")) { ret += "Electric, "; }
				if (AFrame.Contains("|103|")) { ret += "Electric Thermal Storage, "; }
				if (AFrame.Contains("|104|")) { ret += "Forced Warm Air, "; }
				if (AFrame.Contains("|105|")) { ret += "Heat Pump(s), "; }
				if (AFrame.Contains("|106|")) { ret += "Hot Water, "; }
				if (AFrame.Contains("|116|")) { ret += "None, "; }
				if (AFrame.Contains("|117|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|107|")) { ret += "Pellet Stove, "; }
				if (AFrame.Contains("|108|")) { ret += "Radiant Heat, "; }
				if (AFrame.Contains("|109|")) { ret += "Radiator, "; }
				if (AFrame.Contains("|110|")) { ret += "Solar, "; }
				if (AFrame.Contains("|111|")) { ret += "Space Heater, "; }
				if (AFrame.Contains("|112|")) { ret += "Steam, "; }
				if (AFrame.Contains("|113|")) { ret += "Wood Furnace, "; }
				if (AFrame.Contains("|114|")) { ret += "Wood Stove, "; }
				if (AFrame.Contains("|115|")) { ret += "Zoned, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Basement {
			get {
				string ret = "";
				if (AFrame.Contains("|118|")) { ret += "Above Grade, "; }
				if (AFrame.Contains("|119|")) { ret += "Below Grade, "; }
				if (AFrame.Contains("|120|")) { ret += "Block, "; }
				if (AFrame.Contains("|121|")) { ret += "Crawl, "; }
				if (AFrame.Contains("|122|")) { ret += "Finished, "; }
				if (AFrame.Contains("|123|")) { ret += "Full, "; }
				if (AFrame.Contains("|131|")) { ret += "None, "; }
				if (AFrame.Contains("|132|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|124|")) { ret += "Partial, "; }
				if (AFrame.Contains("|125|")) { ret += "Partially Finished, "; }
				if (AFrame.Contains("|126|")) { ret += "Piers, "; }
				if (AFrame.Contains("|127|")) { ret += "Poured, "; }
				if (AFrame.Contains("|128|")) { ret += "Slab, "; }
				if (AFrame.Contains("|129|")) { ret += "Stone, "; }
				if (AFrame.Contains("|130|")) { ret += "Walk-Out, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Appliances {
			get {
				string ret = "";
				if (AFrame.Contains("|133|")) { ret += "Attic Fan, "; }
				if (AFrame.Contains("|134|")) { ret += "BBQ Grill, "; }
				if (AFrame.Contains("|135|")) { ret += "Ceiling Fan(s), "; }
				if (AFrame.Contains("|136|")) { ret += "Central A/C, "; }
				if (AFrame.Contains("|137|")) { ret += "Central Vacuum, "; }
				if (AFrame.Contains("|138|")) { ret += "Compactor, "; }
				if (AFrame.Contains("|139|")) { ret += "Countertop Range, "; }
				if (AFrame.Contains("|140|")) { ret += "Dishwasher, "; }
				if (AFrame.Contains("|141|")) { ret += "Disposal, "; }
				if (AFrame.Contains("|142|")) { ret += "Dryer, "; }
				if (AFrame.Contains("|143|")) { ret += "Freezer, "; }
				if (AFrame.Contains("|144|")) { ret += "Humidifier, "; }
				if (AFrame.Contains("|145|")) { ret += "Ice Maker, "; }
				if (AFrame.Contains("|146|")) { ret += "Intercom, "; }
				if (AFrame.Contains("|147|")) { ret += "Microwave, "; }
				if (AFrame.Contains("|160|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|148|")) { ret += "Refrigerator, "; }
				if (AFrame.Contains("|149|")) { ret += "Self Cleaning Oven, "; }
				if (AFrame.Contains("|150|")) { ret += "Sprinkler, "; }
				if (AFrame.Contains("|151|")) { ret += "Stove, "; }
				if (AFrame.Contains("|152|")) { ret += "Stove Top Grill, "; }
				if (AFrame.Contains("|153|")) { ret += "Timer Thermostat, "; }
				if (AFrame.Contains("|154|")) { ret += "Wall Oven, "; }
				if (AFrame.Contains("|155|")) { ret += "Washer, "; }
				if (AFrame.Contains("|156|")) { ret += "Water Filter, "; }
				if (AFrame.Contains("|157|")) { ret += "Water Softener, "; }
				if (AFrame.Contains("|159|")) { ret += "Win/Wall A/C, "; }
				if (AFrame.Contains("|158|")) { ret += "Wine Cooler, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Near {
			get {
				string ret = "";
				if (AFrame.Contains("|161|")) { ret += "Golf, "; }
				if (AFrame.Contains("|162|")) { ret += "Hotels, "; }
				if (AFrame.Contains("|163|")) { ret += "Lake/Stream, "; }
				if (AFrame.Contains("|173|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|164|")) { ret += "Parks, "; }
				if (AFrame.Contains("|165|")) { ret += "Performing Arts Center, "; }
				if (AFrame.Contains("|166|")) { ret += "Public Transportation, "; }
				if (AFrame.Contains("|167|")) { ret += "Race Track, "; }
				if (AFrame.Contains("|168|")) { ret += "Racing, "; }
				if (AFrame.Contains("|169|")) { ret += "River, "; }
				if (AFrame.Contains("|170|")) { ret += "Schools, "; }
				if (AFrame.Contains("|171|")) { ret += "Shopping, "; }
				if (AFrame.Contains("|172|")) { ret += "State Land, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Road {
			get {
				string ret = "";
				if (AFrame.Contains("|174|")) { ret += "County, "; }
				if (AFrame.Contains("|175|")) { ret += "Deeded Right of Way, "; }
				if (AFrame.Contains("|181|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|176|")) { ret += "Paved, "; }
				if (AFrame.Contains("|177|")) { ret += "Private, "; }
				if (AFrame.Contains("|178|")) { ret += "State, "; }
				if (AFrame.Contains("|179|")) { ret += "Town/Village, "; }
				if (AFrame.Contains("|180|")) { ret += "Unpaved, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string LandFeatures {
			get {
				string ret = "";
				if (AFrame.Contains("|182|")) { ret += "Circular Driveway, "; }
				if (AFrame.Contains("|183|")) { ret += "Cleared, "; }
				if (AFrame.Contains("|184|")) { ret += "Corner, "; }
				if (AFrame.Contains("|185|")) { ret += "Cul-De-Sac, "; }
				if (AFrame.Contains("|186|")) { ret += "Fenced/Enclosed, "; }
				if (AFrame.Contains("|187|")) { ret += "Fruit Trees, "; }
				if (AFrame.Contains("|188|")) { ret += "Gently Sloping, "; }
				if (AFrame.Contains("|189|")) { ret += "Hunting, "; }
				if (AFrame.Contains("|190|")) { ret += "Lake Rights, "; }
				if (AFrame.Contains("|191|")) { ret += "Lakefront, "; }
				if (AFrame.Contains("|192|")) { ret += "Level, "; }
				if (AFrame.Contains("|193|")) { ret += "Mature Trees, "; }
				if (AFrame.Contains("|194|")) { ret += "Motor Boating, "; }
				if (AFrame.Contains("|212|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|195|")) { ret += "Pasture, "; }
				if (AFrame.Contains("|196|")) { ret += "Paved Driveway, "; }
				if (AFrame.Contains("|197|")) { ret += "Pond, "; }
				if (AFrame.Contains("|198|")) { ret += "Pond Site, "; }
				if (AFrame.Contains("|199|")) { ret += "Private, "; }
				if (AFrame.Contains("|200|")) { ret += "Professional Landscaping, "; }
				if (AFrame.Contains("|202|")) { ret += "River Rights, "; }
				if (AFrame.Contains("|201|")) { ret += "Riverfront, "; }
				if (AFrame.Contains("|203|")) { ret += "Rugged, "; }
				if (AFrame.Contains("|204|")) { ret += "Secluded, "; }
				if (AFrame.Contains("|1299|")) { ret += "Signed Gas Lease, "; }
				if (AFrame.Contains("|205|")) { ret += "Stream Front, "; }
				if (AFrame.Contains("|206|")) { ret += "Sub Dividable, "; }
				if (AFrame.Contains("|207|")) { ret += "Vegetable Garden, "; }
				if (AFrame.Contains("|208|")) { ret += "View, "; }
				if (AFrame.Contains("|209|")) { ret += "Water View, "; }
				if (AFrame.Contains("|210|")) { ret += "Wetlands, "; }
				if (AFrame.Contains("|211|")) { ret += "Wooded, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string InteriorAmenities {
			get {
				string ret = "";
				if (AFrame.Contains("|213|")) { ret += "Alarm, "; }
				if (AFrame.Contains("|214|")) { ret += "Amenity Package, "; }
				if (AFrame.Contains("|215|")) { ret += "Attic, "; }
				if (AFrame.Contains("|216|")) { ret += "Cathedral Ceiling, "; }
				if (AFrame.Contains("|217|")) { ret += "Central Hall/Foyer, "; }
				if (AFrame.Contains("|218|")) { ret += "Family Room, "; }
				if (AFrame.Contains("|219|")) { ret += "Fireplace, "; }
				if (AFrame.Contains("|220|")) { ret += "Fireplace Equipment, "; }
				if (AFrame.Contains("|221|")) { ret += "Formal Dining Room, "; }
				if (AFrame.Contains("|222|")) { ret += "Furniture, "; }
				if (AFrame.Contains("|223|")) { ret += "Glass, "; }
				if (AFrame.Contains("|224|")) { ret += "Great Room, "; }
				if (AFrame.Contains("|225|")) { ret += "Handicap Accessible, "; }
				if (AFrame.Contains("|226|")) { ret += "Hardwood Floors, "; }
				if (AFrame.Contains("|227|")) { ret += "Hot Tub/Jacuzzi, "; }
				if (AFrame.Contains("|228|")) { ret += "Insulated Windows, "; }
				if (AFrame.Contains("|229|")) { ret += "Master Suite, "; }
				if (AFrame.Contains("|245|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|230|")) { ret += "Pellet Stove, "; }
				if (AFrame.Contains("|231|")) { ret += "Professional Suite, "; }
				if (AFrame.Contains("|232|")) { ret += "Skylights, "; }
				if (AFrame.Contains("|233|")) { ret += "Stone, "; }
				if (AFrame.Contains("|234|")) { ret += "Sump Pump, "; }
				if (AFrame.Contains("|235|")) { ret += "Tile, "; }
				if (AFrame.Contains("|236|")) { ret += "Track Lighting, "; }
				if (AFrame.Contains("|237|")) { ret += "TV Antenna, "; }
				if (AFrame.Contains("|238|")) { ret += "TV Cable, "; }
				if (AFrame.Contains("|239|")) { ret += "TV Satellite, "; }
				if (AFrame.Contains("|240|")) { ret += "Wall/Wall Carpet, "; }
				if (AFrame.Contains("|241|")) { ret += "Wet Bar, "; }
				if (AFrame.Contains("|242|")) { ret += "Window Treatment, "; }
				if (AFrame.Contains("|243|")) { ret += "Wood Stove, "; }
				if (AFrame.Contains("|244|")) { ret += "Woodwork, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string ExteriorAmenities {
			get {
				string ret = "";
				if (AFrame.Contains("|246|")) { ret += "Additional Land, "; }
				if (AFrame.Contains("|247|")) { ret += "Awnings, "; }
				if (AFrame.Contains("|248|")) { ret += "Barn, "; }
				if (AFrame.Contains("|249|")) { ret += "Community Facilities, "; }
				if (AFrame.Contains("|250|")) { ret += "Community Pool, "; }
				if (AFrame.Contains("|251|")) { ret += "Deck, "; }
				if (AFrame.Contains("|252|")) { ret += "Dock, "; }
				if (AFrame.Contains("|253|")) { ret += "Garage Opener, "; }
				if (AFrame.Contains("|254|")) { ret += "Gated Community, "; }
				if (AFrame.Contains("|255|")) { ret += "Guest Cottage, "; }
				if (AFrame.Contains("|256|")) { ret += "Handicap Access, "; }
				if (AFrame.Contains("|266|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|257|")) { ret += "Outbuilding, "; }
				if (AFrame.Contains("|258|")) { ret += "Patio, "; }
				if (AFrame.Contains("|259|")) { ret += "Pool Above Ground, "; }
				if (AFrame.Contains("|260|")) { ret += "Pool Equipment, "; }
				if (AFrame.Contains("|261|")) { ret += "Pool In Ground, "; }
				if (AFrame.Contains("|262|")) { ret += "Porch, "; }
				if (AFrame.Contains("|263|")) { ret += "Screens, "; }
				if (AFrame.Contains("|264|")) { ret += "Storm Windows, "; }
				if (AFrame.Contains("|265|")) { ret += "Swing Set, "; }
				if (AFrame.Contains("|586|")) { ret += "Alley, "; }
				if (AFrame.Contains("|587|")) { ret += "Exterior Lighting, "; }
				if (AFrame.Contains("|588|")) { ret += "Fenced, "; }
				if (AFrame.Contains("|589|")) { ret += "Fire Escape, "; }
				if (AFrame.Contains("|590|")) { ret += "Handicap Ramp, "; }
				if (AFrame.Contains("|591|")) { ret += "Landscaping, "; }
				if (AFrame.Contains("|593|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|592|")) { ret += "Underground Tank, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string ToShow {
			get {
				string ret = "";
				if (AFrame.Contains("|267|")) { ret += "By Appointment, "; }
				if (AFrame.Contains("|268|")) { ret += "Call Broker, "; }
				if (AFrame.Contains("|269|")) { ret += "Call Owner, "; }
				if (AFrame.Contains("|270|")) { ret += "Caution Pets, "; }
				if (AFrame.Contains("|271|")) { ret += "Go Direct, "; }
				if (AFrame.Contains("|272|")) { ret += "Key At Broker, "; }
				if (AFrame.Contains("|273|")) { ret += "Lock Box, "; }
				if (AFrame.Contains("|275|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|274|")) { ret += "Send Show Notice, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string PossibleFinancing {
			get {
				string ret = "";
				if (AFrame.Contains("|276|")) { ret += "Assumable, "; }
				if (AFrame.Contains("|277|")) { ret += "Conventional, "; }
				if (AFrame.Contains("|278|")) { ret += "Creative, "; }
				if (AFrame.Contains("|279|")) { ret += "Exchange, "; }
				if (AFrame.Contains("|280|")) { ret += "FHA/VA, "; }
				if (AFrame.Contains("|283|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|281|")) { ret += "Owner First, "; }
				if (AFrame.Contains("|282|")) { ret += "Owner Second, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Options {
			get {
				string ret = "";
				if (AFrame.Contains("|285|")) { ret += "Rent Only, "; }
				if (AFrame.Contains("|284|")) { ret += "Rent with Options, "; }
				if (AFrame.Contains("|286|")) { ret += "Sale, "; }
				if (AFrame.Contains("|309|")) { ret += "Rent Only, "; }
				if (AFrame.Contains("|308|")) { ret += "Rent with Options, "; }
				if (AFrame.Contains("|310|")) { ret += "Sale Only, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string CommonFacilities {
			get {
				string ret = "";
				if (AFrame.Contains("|289|")) { ret += "Clubhouse, "; }
				if (AFrame.Contains("|287|")) { ret += "Gym Facilities, "; }
				if (AFrame.Contains("|291|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|288|")) { ret += "Pool, "; }
				if (AFrame.Contains("|290|")) { ret += "Sports Courts, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string LandCondition {
			get {
				string ret = "";
				if (AFrame.Contains("|400|")) { ret += "Below Grade, "; }
				if (AFrame.Contains("|401|")) { ret += "Cleared, "; }
				if (AFrame.Contains("|402|")) { ret += "Dry, "; }
				if (AFrame.Contains("|403|")) { ret += "Gentle Sloping, "; }
				if (AFrame.Contains("|404|")) { ret += "Level, "; }
				if (AFrame.Contains("|405|")) { ret += "Mixed, "; }
				if (AFrame.Contains("|410|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|406|")) { ret += "Pasture/Field, "; }
				if (AFrame.Contains("|407|")) { ret += "Rugged, "; }
				if (AFrame.Contains("|408|")) { ret += "Tillable, "; }
				if (AFrame.Contains("|409|")) { ret += "Wet, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string LandUseRestrictions {
			get {
				string ret = "";
				if (AFrame.Contains("|413|")) { ret += "Building Moratorium, "; }
				if (AFrame.Contains("|414|")) { ret += "DEC Approval, "; }
				if (AFrame.Contains("|415|")) { ret += "Deed Restriction/Covenant, "; }
				if (AFrame.Contains("|416|")) { ret += "Easement, "; }
				if (AFrame.Contains("|417|")) { ret += "Flood Plain, "; }
				if (AFrame.Contains("|421|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|418|")) { ret += "Perc Test Needed, "; }
				if (AFrame.Contains("|419|")) { ret += "Variance Required, "; }
				if (AFrame.Contains("|420|")) { ret += "Wetlands/Fresh, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string DocumentsOnFile {
			get {
				string ret = "";
				if (AFrame.Contains("|422|")) { ret += "Codes,Covenants & Restrct, "; }
				if (AFrame.Contains("|423|")) { ret += "Deed, "; }
				if (AFrame.Contains("|424|")) { ret += "Disclosure, "; }
				if (AFrame.Contains("|425|")) { ret += "Engineering Docs, "; }
				if (AFrame.Contains("|428|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|426|")) { ret += "Survey, "; }
				if (AFrame.Contains("|427|")) { ret += "Tax Map, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Description {
			get {
				string ret = "";
				if (AFrame.Contains("|540|")) { ret += "Automotive, "; }
				if (AFrame.Contains("|541|")) { ret += "Bed & Breakfast/Inn, "; }
				if (AFrame.Contains("|542|")) { ret += "Building Only, "; }
				if (AFrame.Contains("|543|")) { ret += "Bungalow Colony, "; }
				if (AFrame.Contains("|544|")) { ret += "Business Only, "; }
				if (AFrame.Contains("|545|")) { ret += "Distribution, "; }
				if (AFrame.Contains("|546|")) { ret += "Farm, "; }
				if (AFrame.Contains("|547|")) { ret += "Food Services, "; }
				if (AFrame.Contains("|548|")) { ret += "Garage, "; }
				if (AFrame.Contains("|549|")) { ret += "Gas Station, "; }
				if (AFrame.Contains("|550|")) { ret += "Hotel/Motel, "; }
				if (AFrame.Contains("|551|")) { ret += "Industrial, "; }
				if (AFrame.Contains("|552|")) { ret += "Manufacturing, "; }
				if (AFrame.Contains("|553|")) { ret += "Mixed Use, "; }
				if (AFrame.Contains("|554|")) { ret += "Multi Dwellings, "; }
				if (AFrame.Contains("|555|")) { ret += "Multi Structures, "; }
				if (AFrame.Contains("|556|")) { ret += "Office, "; }
				if (AFrame.Contains("|557|")) { ret += "Offices w/ Apartments, "; }
				if (AFrame.Contains("|565|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|558|")) { ret += "Residence on Premises, "; }
				if (AFrame.Contains("|559|")) { ret += "Restaurant/Diner, "; }
				if (AFrame.Contains("|560|")) { ret += "Retail, "; }
				if (AFrame.Contains("|561|")) { ret += "Store Front, "; }
				if (AFrame.Contains("|562|")) { ret += "Store Front w/ Apartments, "; }
				if (AFrame.Contains("|563|")) { ret += "Transportation, "; }
				if (AFrame.Contains("|564|")) { ret += "Warehouse, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Foundation {
			get {
				string ret = "";
				if (AFrame.Contains("|1|")) { ret += "A-Frame, "; }
				if (AFrame.Contains("|579|")) { ret += "Block, "; }
				if (AFrame.Contains("|580|")) { ret += "Full, "; }
				if (AFrame.Contains("|585|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|581|")) { ret += "Partial, "; }
				if (AFrame.Contains("|582|")) { ret += "Pier & Post, "; }
				if (AFrame.Contains("|583|")) { ret += "Slab, "; }
				if (AFrame.Contains("|584|")) { ret += "Stone, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Floors {
			get {
				string ret = "";
				if (AFrame.Contains("|594|")) { ret += "Carpet, "; }
				if (AFrame.Contains("|595|")) { ret += "Concrete, "; }
				if (AFrame.Contains("|599|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|596|")) { ret += "Tile, "; }
				if (AFrame.Contains("|597|")) { ret += "Vinyl, "; }
				if (AFrame.Contains("|598|")) { ret += "Wood, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Equipment {
			get {
				string ret = "";
				if (AFrame.Contains("|600|")) { ret += "Agricultural Equipment, "; }
				if (AFrame.Contains("|601|")) { ret += "Central A/C, "; }
				if (AFrame.Contains("|602|")) { ret += "Chairlift, "; }
				if (AFrame.Contains("|603|")) { ret += "Counters, "; }
				if (AFrame.Contains("|604|")) { ret += "Elevators, "; }
				if (AFrame.Contains("|605|")) { ret += "Fire Alarm, "; }
				if (AFrame.Contains("|606|")) { ret += "Freight Elevator, "; }
				if (AFrame.Contains("|607|")) { ret += "Handicapped Access, "; }
				if (AFrame.Contains("|608|")) { ret += "Handicapped Facilities, "; }
				if (AFrame.Contains("|609|")) { ret += "O/H Doors 1, "; }
				if (AFrame.Contains("|610|")) { ret += "O/H Doors 2, "; }
				if (AFrame.Contains("|611|")) { ret += "O/H Doors 3+, "; }
				if (AFrame.Contains("|617|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|612|")) { ret += "Refrigeration, "; }
				if (AFrame.Contains("|613|")) { ret += "Security Lighting, "; }
				if (AFrame.Contains("|614|")) { ret += "Security System, "; }
				if (AFrame.Contains("|615|")) { ret += "Shelving, "; }
				if (AFrame.Contains("|616|")) { ret += "Sprinkler System, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string Garbage {
			get {
				string ret = "";
				if (AFrame.Contains("|865|")) { ret += "Municipal, "; }
				if (AFrame.Contains("|866|")) { ret += "Private, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		public string FeaturesAmenities {
			get {
				string ret = "";
				if (AFrame.Contains("|871|")) { ret += "Additional Land, "; }
				if (AFrame.Contains("|872|")) { ret += "Alarm, "; }
				if (AFrame.Contains("|873|")) { ret += "Amenity Package, "; }
				if (AFrame.Contains("|874|")) { ret += "Attic, "; }
				if (AFrame.Contains("|875|")) { ret += "Awning, "; }
				if (AFrame.Contains("|876|")) { ret += "Cathedral Ceiling, "; }
				if (AFrame.Contains("|877|")) { ret += "Central Hall/Foyer, "; }
				if (AFrame.Contains("|878|")) { ret += "Community Pool, "; }
				if (AFrame.Contains("|879|")) { ret += "Deck, "; }
				if (AFrame.Contains("|880|")) { ret += "Dock, "; }
				if (AFrame.Contains("|881|")) { ret += "Elevator, "; }
				if (AFrame.Contains("|882|")) { ret += "Family Room, "; }
				if (AFrame.Contains("|883|")) { ret += "Fire Escape, "; }
				if (AFrame.Contains("|884|")) { ret += "Fireplace, "; }
				if (AFrame.Contains("|885|")) { ret += "Fireplace Equipment, "; }
				if (AFrame.Contains("|886|")) { ret += "Formal Dining Room, "; }
				if (AFrame.Contains("|887|")) { ret += "Furniture, "; }
				if (AFrame.Contains("|888|")) { ret += "Garage Opener, "; }
				if (AFrame.Contains("|889|")) { ret += "Glass, "; }
				if (AFrame.Contains("|890|")) { ret += "Great Room, "; }
				if (AFrame.Contains("|891|")) { ret += "Guest Cottage, "; }
				if (AFrame.Contains("|892|")) { ret += "Handicap Access, "; }
				if (AFrame.Contains("|893|")) { ret += "Handicap Facilities, "; }
				if (AFrame.Contains("|894|")) { ret += "Hardwood Floors, "; }
				if (AFrame.Contains("|895|")) { ret += "Hot Tub/Jacuzzi, "; }
				if (AFrame.Contains("|896|")) { ret += "Insulated Windows, "; }
				if (AFrame.Contains("|897|")) { ret += "Master Suite, "; }
				if (AFrame.Contains("|916|")) { ret += "Other/See Remarks, "; }
				if (AFrame.Contains("|898|")) { ret += "Patio, "; }
				if (AFrame.Contains("|899|")) { ret += "Porch, "; }
				if (AFrame.Contains("|900|")) { ret += "Professional Suite, "; }
				if (AFrame.Contains("|901|")) { ret += "Screens, "; }
				if (AFrame.Contains("|902|")) { ret += "Security Lighting, "; }
				if (AFrame.Contains("|903|")) { ret += "Security System, "; }
				if (AFrame.Contains("|906|")) { ret += "Skylights, "; }
				if (AFrame.Contains("|904|")) { ret += "Sprinkler System, "; }
				if (AFrame.Contains("|905|")) { ret += "Storm Windows, "; }
				if (AFrame.Contains("|907|")) { ret += "Swing Set, "; }
				if (AFrame.Contains("|911|")) { ret += "Track Lighting, "; }
				if (AFrame.Contains("|908|")) { ret += "TV Antenna, "; }
				if (AFrame.Contains("|909|")) { ret += "TV Cable, "; }
				if (AFrame.Contains("|910|")) { ret += "TV Satellite Dish, "; }
				if (AFrame.Contains("|912|")) { ret += "Wall To Wall Carpet, "; }
				if (AFrame.Contains("|913|")) { ret += "Wet Bar, "; }
				if (AFrame.Contains("|914|")) { ret += "Window Treatments, "; }
				if (AFrame.Contains("|915|")) { ret += "Woodwork, "; }
				if (ret.Length > 2) {
					return ret.Substring(0, ret.Length - 2);
				} else {
					return "";
				}
			}
		}
		#endregion

		#region Constructor
		public ListingMap() { }
		public ListingMap(string mls) {
			MLS = mls;
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT l1.[MLS #],[Class],[Type],[Township],[Asking Price],[Address Number],[Address Direction]" +
						",[Address Street],[Address 2],[City],[State],[Zip],[Status],[Sale/Rent],[# Rooms],[# Bedrooms],[# Full Baths]" +
						",[# Half Baths],[Garage Type],[Garage Capacity],[Fireplace],[Listing Type],[Number of Acres],[Agent ID]" +
						",[Agent Name],[Agent Phone],[Listing Office 1 ID],[Listing Office 1 Name],[Listing Office 1 Phone],[County]" +
						",[School District],[Development],[Approx Year Built],[Approx Lot Size-Front],[Approx Lot Size-Side],[Approx SqFt Above Grade]" +
						",[Approx SqFt Below Grade],[Approx Total Square Feet],[Approx Total Taxes],[Tax Exemption Y/N],[Land Value],[Total Value]" +
						",[Possession Date],[Maintenance Fees (Year)],[Utilities (Month)],[Homeowners Fees (Year)],[Financing Terms],[Remarks]" +
						",[A-Frame],[Virtual Tour],l2.[Lat], l2.[Long] FROM [listings-residential] l1 LEFT JOIN [listings-residential-ext] l2 ON l1.[MLS #] = l2.[MLS #] WHERE l1.[MLS #] = @MLS";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("MLS", SqlDbType.Float).Value = Convert.ToDouble(MLS);
						cmd.Connection.Open();
						SqlDataReader dr = cmd.ExecuteReader();
						if (dr.HasRows) {
							dr.Read();
							MLS = dr[0].ToString();
							//Class = dr[1].ToString();
							//Type = dr[2].ToString();
							//Township = dr[3].ToString();
							AskingPrice = string.Format("{0:$0,000}", dr[4]);
							float temp = 0;
							float.TryParse(dr[4].ToString(), out temp);
							AskingAmt = temp;
							AddressNumber = dr[5].ToString();
							AddressDirection = dr[6].ToString();
							AddressStreet = dr[7].ToString();
							Address2 = dr[8].ToString();
							City = dr[9].ToString();
							State = dr[10].ToString();
							Zip = dr[11].ToString();
							//Status = dr[12].ToString();
							//SaleRent = dr[13].ToString();
							//Rooms = dr[14].ToString();
							Bedrooms = dr[15].ToString();
							FullBaths = dr[16].ToString();
							//HalfBaths = dr[17].ToString();
							//GarageType = dr[18].ToString();
							//GarageCapacity = dr[19].ToString();
							//Fireplace = dr[20].ToString();
							//ListingType = dr[21].ToString();
							Acres = dr[22].ToString();
							//AgentID = dr[23].ToString();
							//AgentName = dr[24].ToString();
							//AgentPhone = dr[25].ToString();
							//ListingOfficeID = dr[26].ToString();
							//ListingOfficeName = dr[27].ToString();
							//ListingOfficePhone = dr[28].ToString();
							//County = dr[29].ToString();
							//SchoolDistrict = dr[30].ToString();
							//Development = dr[31].ToString();
							//YearBuilt = dr[32].ToString();
							//LotSizeFront = dr[33].ToString();
							//LotSizeSide = dr[34].ToString();
							//SqFtAboveGrade = string.Format("{0:0,0}", dr[35]);
							//SqFtBelowGrade = string.Format("{0:0,0}", dr[36]);
							//TotalSquareFeet = string.Format("{0:0,0}", dr[37]);
							//TotalTaxes = string.Format("{0:$0,0}", dr[38]);
							//TaxExemption = dr[39].ToString();
							//LandValue = string.Format("{0:$0,0}", dr[40]);
							//TotalValue = string.Format("{0:$0,0}", dr[41]);
							//if (!string.IsNullOrEmpty(LandValue) && !string.IsNullOrEmpty(TotalValue)) {
							//	BldgValue = string.Format("{0:$0,0}", Convert.ToDouble(dr[41]) - Convert.ToDouble(dr[40]));
							//}
							//PossessionDate = dr[42].ToString();
							//MaintenanceFees = string.Format("{0:$0,0}", dr[43]);
							//Utilities = string.Format("{0:$0,0}", dr[44]);
							//HomeownersFees = string.Format("{0:$0,0}", dr[45]);
							//FinancingTerms = dr[46].ToString();
							//Remarks = dr[47].ToString();
							//AFrame = "|" + dr[48].ToString() + "|";
							//VirtualTour = dr[49].ToString();
							temp = 0;
							float.TryParse(dr[50].ToString(), out temp);
							Lat = temp;
							temp = 0;
							float.TryParse(dr[51].ToString(), out temp);
							Long = temp;
						} else {
							MLS = "";
						}
						cmd.Connection.Close();
					}
				}
			} catch { }
		}
		#endregion
	}

	public class Listings {
		public SearchResult DoSearch(SearchModel data, int page = 1, int PostsPerPage = 12) {
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string param = "";
			string where = "";
			SearchResult searchResult = new SearchResult();
			List<Listing> listings = new List<Listing>();
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
				//if (data.Garage == 1) { where += "AND [Garage Capacity] > 0 "; }
				//if (data.Pool == 1) { where += "AND ([A-Frame] LIKE '%|250|%' OR [A-Frame] LIKE '%|259|%' OR [A-Frame] LIKE '%|261|%') "; }
				if (data.Fireplace == 1) { where += "AND [Fireplacesnumberof] > 0 "; }
				//if (data.Barn == 1) { where += "AND ([A-Frame] LIKE '%|248|%' OR [A-Frame] LIKE '%|255|%' OR [A-Frame] LIKE '%|257|%') "; }
				//if (data.Handicap == 1) { where += "AND ([A-Frame] LIKE '%|225|%' OR [A-Frame] LIKE '%|256|%' OR [A-Frame] LIKE '%|590|%' OR [A-Frame] LIKE '%|607|%' OR [A-Frame] LIKE '%|608|%' OR [A-Frame] LIKE '%|892|%' OR [A-Frame] LIKE '%|893|%') "; }
				//if (data.Skylights == 1) { where += "AND ([A-Frame] LIKE '%|232|%' OR [A-Frame] LIKE '%|906|%') "; }
				//if (data.Lake == 1) { where += "AND ([A-Frame] LIKE '%|190|%' OR [A-Frame] LIKE '%|191|%' OR [A-Frame] LIKE '%|197|%' OR [A-Frame] LIKE '%|198|%' OR [A-Frame] LIKE '%|201|%' OR [A-Frame] LIKE '%|202|%' OR [A-Frame] LIKE '%|205|%') "; }
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
					//if (data.Garage == 1) { param += "&garage=1"; }
					//if (data.Pool == 1) { param += "&pool=1"; }
					if (data.Fireplace == 1) { param += "&fireplace=1"; }
					//if (data.Barn == 1) { param += "&barn=1"; }
					//if (data.Handicap == 1) { param += "&handicap=1"; }
					//if (data.Skylights == 1) { param += "&skylights=1"; }
					//if (data.Lake == 1) { param += "&lake=1"; }


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
						listings.Add(new Listing(dr[1].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			searchResult.Listings = listings;
			return searchResult;
		}
		//public List<Listing> GetAll(SearchModel data) {
		//	List<Listing> listings = new List<Listing>();
		//	using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {

		//		string SQL = "SELECT [MLS #] FROM [listings-residential] WHERE [State] = 'NY' ";
		//		if (data.City != null) { SQL += "AND City IN (@City) "; }
		//		if (data.Bedrooms > 0) {
		//			SQL += "AND [# Bedrooms] >= @Bedrooms ";
		//		}
		//		if (data.Bathrooms > 0) {
		//			SQL += "AND [# Full Baths] >= @Bathrooms ";
		//		}
		//		if (data.Acres > 0) {
		//			SQL += "AND [Number of Acres] >= @Acres ";
		//		}
		//		if (data.MinPrice > 0) {
		//			SQL += "AND [Asking Price] >= @MinPrice ";
		//		}
		//		if (data.MaxPrice > 0) {
		//			SQL += "AND [Asking Price] <= @MaxPrice ";
		//		}
		//		SQL += "ORDER BY [Asking Price] DESC, City";
		//		using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
		//			cmd.CommandType = CommandType.Text;
		//			//if (!string.IsNullOrEmpty(data.City)) {
		//			//	cmd.Parameters.Add("City", SqlDbType.VarChar, 255).Value = data.City;
		//			//}
		//			if (data.Bedrooms > 0) {
		//				cmd.Parameters.Add("Bedrooms", SqlDbType.Float).Value = data.Bedrooms;
		//			}
		//			if (data.Bathrooms > 0) {
		//				cmd.Parameters.Add("Bathrooms", SqlDbType.Float).Value = data.Bathrooms;
		//			}
		//			if (data.Acres > 0) {
		//				cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres;
		//			}
		//			if (data.MinPrice > 0) {
		//				cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000;
		//			}
		//			if (data.MaxPrice > 0) {
		//				cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000;
		//			}
		//			cmd.Connection.Open();
		//			SqlDataReader dr = cmd.ExecuteReader();
		//			while (dr.Read()) {
		//				listings.Add(new Listing(dr[0].ToString()));
		//			}
		//			cmd.Connection.Close();
		//		}
		//	}

		//	return listings;
		//}
		public List<Listing> GetFeatured() {
			List<Listing> listings = new List<Listing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM Favorites WHERE IsActive = 1 ORDER BY SortOrder, Town, MLS", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						if (!string.IsNullOrEmpty(dr["MLS"].ToString())) {
							Listing listing = new Listing(dr["MLS"].ToString());
							if (!string.IsNullOrEmpty(listing.MLS)) {
								listing.LinkURL = "/detail?mls=" + listing.MLS;
								listing.ImageURL = "/img/listings/" + listing.ImageFirst;
								listing.FavoriteDescription = listing.ShortDescription(150) + "...";
								listings.Add(listing);
							}
						} else {
							Listing listing = new Listing();
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
		public List<ListingMap> GetMapListings(SearchModel data) {
			string param = "";
			List<ListingMap> listings = new List<ListingMap>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT l1.[MLS #] FROM [listings-residential] l1 JOIN [listings-residential-ext] l2 ON l1.[MLS #] = l2.[MLS #] WHERE l1.Status = 'Active' ";
				string[] TownList = GetRelatedTowns(data.City);
				if (TownList != null && !string.IsNullOrEmpty(TownList[0])) {
					SQL += "AND (City = @City1 ";
					for (int x = 2; x <= TownList.Length; x++) {
						SQL += " OR City = @City" + x.ToString();
					}
					SQL += ") ";
				}
				if (data.Acres > 0) { SQL += "AND [Number of Acres] >= @Acres "; }
				if (data.Acres2 > 0) { SQL += "AND [Number of Acres] <= @Acres2 "; }
				if (data.Bathrooms > 0) { SQL += "AND [# Full Baths] >= @Bathrooms "; }
				if (data.Bathrooms2 > 0) { SQL += "AND [# Full Baths] <= @Bathrooms2 "; }
				if (data.Bedrooms > 0) { SQL += "AND [# Bedrooms] >= @Bedrooms "; }
				if (data.Bedrooms2 > 0) { SQL += "AND [# Bedrooms] <= @Bedrooms2 "; }
				if (data.MinPrice > 0) { SQL += "AND [Asking Price] >= @MinPrice "; }
				if (data.MaxPrice > 0) { SQL += "AND [Asking Price] <= @MaxPrice "; }
				if (data.SqFt > 0) { SQL += "AND [Approx Total Square Feet] >= @SqFt "; }
				if (data.SqFt2 > 0) { SQL += "AND [Approx Total Square Feet] <= @SqFt2 "; }
				if (data.Years > 0) { SQL += "AND [Approx Year Built] <= YEAR(GetDate()) - @Years "; }
				if (data.Years2 > 0) { SQL += "AND [Approx Year Built] >= YEAR(GetDate()) - @Years2 "; }
				if (data.Garage == 1) { SQL += "AND [Garage Capacity] > 0 "; }
				if (data.Pool == 1) { SQL += "AND ([A-Frame] LIKE '%|250|%' OR [A-Frame] LIKE '%|259|%' OR [A-Frame] LIKE '%|261|%') "; }
				if (data.Fireplace == 1) { SQL += "AND [Fireplace] > 0 "; }
				if (data.Barn == 1) { SQL += "AND ([A-Frame] LIKE '%|248|%' OR [A-Frame] LIKE '%|255|%' OR [A-Frame] LIKE '%|257|%') "; }
				if (data.Handicap == 1) { SQL += "AND ([A-Frame] LIKE '%|225|%' OR [A-Frame] LIKE '%|256|%' OR [A-Frame] LIKE '%|590|%' OR [A-Frame] LIKE '%|607|%' OR [A-Frame] LIKE '%|608|%' OR [A-Frame] LIKE '%|892|%' OR [A-Frame] LIKE '%|893|%') "; }
				if (data.Skylights == 1) { SQL += "AND ([A-Frame] LIKE '%|232|%' OR [A-Frame] LIKE '%|906|%') "; }
				if (data.Lake == 1) { SQL += "AND ([A-Frame] LIKE '%|190|%' OR [A-Frame] LIKE '%|191|%' OR [A-Frame] LIKE '%|197|%' OR [A-Frame] LIKE '%|198|%' OR [A-Frame] LIKE '%|201|%' OR [A-Frame] LIKE '%|202|%' OR [A-Frame] LIKE '%|205|%') "; }

				SQL += "ORDER BY [Asking Price] DESC, City";
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
					if (data.Garage == 1) { param += "&garage=1"; }
					if (data.Pool == 1) { param += "&pool=1"; }
					if (data.Fireplace == 1) { param += "&fireplace=1"; }
					if (data.Barn == 1) { param += "&barn=1"; }
					if (data.Handicap == 1) { param += "&handicap=1"; }
					if (data.Skylights == 1) { param += "&skylights=1"; }
					if (data.Lake == 1) { param += "&lake=1"; }

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new ListingMap(dr[0].ToString()));
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
            data = data.Replace("WALKUP", "Walkup, "); data = data.Replace(", ,", ", ");
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
        public string TranslateCity3(string data) {
            if(data == "BayShore") { data = "Bay Shore"; }
            if(data == "BeaconFalls") { data = "Beacon Falls"; }
            if(data == "BloomingGrove") { data = "Blooming Grove"; }
            if(data == "BoltonLanding") { data = "Bolton Landing"; }
            if(data == "CallListingAgent") { data = "Call Listing Agent"; }
            if(data == "CambriaHeights") { data = "Cambria Heights"; }
            if(data == "CampbellHall") { data = "Campbell Hall"; }
            if(data == "CentralIslip") { data = "Central Islip"; }
            if(data == "ChesterTown") { data = "Chester Town"; }
            if(data == "Clermont11") { data = "Clermont 11"; }
            if(data == "CollegePoint") { data = "College Point"; }
            if(data == "CopakeFalls") { data = "Copake Falls"; }
            if(data == "CornwallBridge") { data = "Cornwall Bridge"; }
            if(data == "CornwallonHudson") { data = "Cornwall on Hudson"; }
            if(data == "CosCob") { data = "Cos Cob"; }
            if(data == "DeerPark") { data = "Deerpark"; }
            if(data == "EastChatham") { data = "East Chatham"; }
            if(data == "EastElmhurst") { data = "East Elmhurst"; }
            if(data == "EastFishkill") { data = "East Fishkill"; }
            if(data == "EastHampton") { data = "East Hampton"; }
            if(data == "EastMeadow") { data = "East Meadow"; }
            if(data == "EastNorwalk") { data = "East Norwalk"; }
            if(data == "EastQuogue") { data = "East Quogue"; }
            if(data == "FarRockaway") { data = "Far Rockaway"; }
            if(data == "FloralPark") { data = "Floral Park"; }
            if(data == "ForestHills") { data = "Forest Hills"; }
            if(data == "FortEdward") { data = "Fort Edward"; }
            if(data == "FortMontgomery") { data = "Fort Montgomery"; }
            if(data == "FranklinSquare") { data = "Franklin Square"; }
            if(data == "FremontCenter") { data = "Fremont Center"; }
            if(data == "GlenCove") { data = "Glen Cove"; }
            if(data == "GlensFalls") { data = "Glens Falls"; }
            if(data == "GoshenTown") { data = "Goshen Town"; }
            if(data == "GreatNeck") { data = "Great Neck"; }
            if(data == "GreenwoodLake") { data = "Greenwood Lake"; }
            if(data == "HamptonBays") { data = "Hampton Bays"; }
            if(data == "HaverstrawTown") { data = "Haverstraw Town"; }
            if(data == "HighFalls") { data = "High Falls"; }
            if(data == "HighlandFalls") { data = "Highland Falls"; }
            if(data == "HighlandMills") { data = "Highland Mills"; }
            if(data == "HowardBeach") { data = "Howard Beach"; }
            if(data == "HydePark") { data = "Hyde Park"; }
            if(data == "IndianLake") { data = "Indian Lake"; }
            if(data == "IslipTerrace") { data = "Islip Terrace"; }
            if(data == "JacksonHeights") { data = "Jackson Heights"; }
            if(data == "KewGardens") { data = "Kew Gardens"; }
            if(data == "KiameshaLake") { data = "Kiamesha Lake"; }
            if(data == "KingstonCity") { data = "Kingston City"; }
            if(data == "KingstonTown") { data = "Kingston Town"; }
            if(data == "LaGrange") { data = "La Grange"; }
            if(data == "LakeKatrine") { data = "Lake Katrine"; }
            if(data == "LakeLuzerne") { data = "Lake Luzerne"; }
            if(data == "LakePlacid") { data = "Lake Placid"; }
            if(data == "LibertyTown") { data = "Liberty Town"; }
            if(data == "LittleFalls") { data = "Little Falls"; }
            if(data == "LittleNeck") { data = "Little Neck"; }
            if(data == "LochSheldrake") { data = "Loch Sheldrake"; }
            if(data == "LongBeach") { data = "Long Beach"; }
            if(data == "LyonsFalls") { data = "Lyons Falls"; }
            if(data == "MaldenonHudson") { data = "Malden on Hudson"; }
            if(data == "MiddleVillage") { data = "Middle Village"; }
            if(data == "MillerPlace") { data = "Miller Place"; }
            if(data == "MontgomeryTown") { data = "Montgomery Town"; }
            if(data == "MountHope") { data = "Mount Hope"; }
            if(data == "MountKisco") { data = "Mount Kisco"; }
            if(data == "MountPleasant") { data = "Mount Pleasant"; }
            if(data == "MountVernon") { data = "Mount Vernon"; }
            if(data == "NewBaltimore") { data = "New Baltimore"; }
            if(data == "NewBritain") { data = "New Britain"; }
            if(data == "NewburghCity") { data = "Newburgh City"; }
            if(data == "NewburghTown") { data = "Newburgh Town"; }
            if(data == "NewCanaan") { data = "New Canaan"; }
            if(data == "NewCastle") { data = "New Castle"; }
            if(data == "NewCity") { data = "New City"; }
            if(data == "NewFairfield") { data = "New Fairfield"; }
            if(data == "NewHampton") { data = "New Hampton"; }
            if(data == "NewHydePark") { data = "New Hyde Park"; }
            if(data == "NewLebanon") { data = "New Lebanon"; }
            if(data == "NewMilford") { data = "New Milford"; }
            if(data == "NewPaltz") { data = "New Paltz"; }
            if(data == "NewRochelle") { data = "New Rochelle"; }
            if(data == "NewWindsor") { data = "New Windsor "; }
            if(data == "NorthCastle") { data = "North Castle"; }
            if(data == "NorthChatham") { data = "North Chatham"; }
            if(data == "NorthSalem") { data = "North Salem"; }
            if(data == "OakHill") { data = "Oak Hill"; }
            if(data == "OaklandGardens") { data = "Oakland Gardens"; }
            if(data == "OldChatham") { data = "Old Chatham"; }
            if(data == "OldGreenwich") { data = "Old Greenwich"; }
            if(data == "OldTappan") { data = "Old Tappan"; }
            if(data == "OysterBay") { data = "Oyster Bay"; }
            if(data == "OzonePark") { data = "Ozone Park"; }
            if(data == "PineBush") { data = "Pine Bush"; }
            if(data == "PinePlains") { data = "Pine Plains"; }
            if(data == "PleasantValley") { data = "Pleasant Valley"; }
            if(data == "PortEwen") { data = "Port Ewen"; }
            if(data == "PortJervis") { data = "Port Jervis"; }
            if(data == "PoughkeepsieCity") { data = "Poughkeepsie City"; }
            if(data == "PoughkeepsieTown") { data = "Poughkeepsie Town"; }
            if(data == "PoundRidge") { data = "Pound Ridge"; }
            if(data == "PrestonHollow") { data = "Preston Hollow"; }
            if(data == "PutnamValley") { data = "Putnam Valley"; }
            if(data == "QueensVillage") { data = "Queens Village"; }
            if(data == "ReddingCenter") { data = "Redding Center"; }
            if(data == "RedHook") { data = "Red Hook"; }
            if(data == "RegoPark") { data = "Rego Park"; }
            if(data == "RichmondHill") { data = "Richmond Hill"; }
            if(data == "RoslynHeights") { data = "Roslyn Heights"; }
            if(data == "RyeCity") { data = "Rye City"; }
            if(data == "RyeTown") { data = "Rye Town"; }
            if(data == "SagHarbor") { data = "Sag Harbor"; }
            if(data == "SaintAlbans") { data = "Saint Albans"; }
            if(data == "SaintJohnsville") { data = "Saint Johnsville"; }
            if(data == "SalisburyMills") { data = "Salisbury Mills"; }
            if(data == "SandyHook") { data = "Sandy Hook"; }
            if(data == "SaratogaSprings") { data = "Saratoga Springs"; }
            if(data == "SaugertiesTown") { data = "Saugerties Town"; }
            if(data == "SchroonLake") { data = "Schroon Lake"; }
            if(data == "SouthNorwalk") { data = "South Norwalk"; }
            if(data == "SouthOzonePark") { data = "South Ozone Park"; }
            if(data == "SouthRichmondHill") { data = "South Richmond Hill"; }
            if(data == "SparrowBush") { data = "Sparrow Bush"; }
            if(data == "SpringfieldGardens") { data = "Springfield Gardens"; }
            if(data == "SpringGlen") { data = "Spring Glen"; }
            if(data == "SpringValley") { data = "Spring Valley"; }
            if(data == "StoneRidge") { data = "Stone Ridge"; }
            if(data == "StonyPoint") { data = "Stony Point"; }
            if(data == "TuxedoPark") { data = "Tuxedo Park"; }
            if(data == "UlsterPark") { data = "Ulster Park"; }
            if(data == "UnionVale") { data = "Union Vale"; }
            if(data == "ValleyCottage") { data = "Valley Cottage"; }
            if(data == "ValleyStream") { data = "Valley Stream"; }
            if(data == "Wallkill") { data = "Wallkill Town"; }
            if(data == "WarwickTown") { data = "Warwick Town"; }
            if(data == "WesthamptonBeach") { data = "Westhampton Beach"; }
            if(data == "WestHaven") { data = "West Haven"; }
            if(data == "WestHaverstraw") { data = "West Haverstraw"; }
            if(data == "WestHempstead") { data = "West Hempstead"; }
            if(data == "WestIslip") { data = "West Islip"; }
            if(data == "WestNyack") { data = "West Nyack"; }
            if(data == "WestPark") { data = "West Park"; }
            if(data == "WestRedding") { data = "West Redding"; }
            if(data == "WhitePlains") { data = "White Plains"; }
            if(data == "WoodburyTown") { data = "Woodbury Town"; }

            return data;
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
            data = data.Replace("OIL", "Oil, ");
            data = data.Replace("OILAG", "Oil Above Ground, ");
            data = data.Replace("OILBG", "Oil Below Ground, ");
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
            data = data.Replace("ELEC", "Electric, ");
            data = data.Replace("ELECHT", "Electric Stand Alone, ");
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

        public string TranslateCity(string data) {
			string ret = "";
			switch (data) {
				case "Abramsvill":
					ret = "Abramsville";
					break;
				case "Bloomingb1":
					ret = "Bloomingburg Village";
					break;
				case "Bloomingbu":
					ret = "Bloomingburg";
					break;
				case "CallicoonC":
					ret = "Callicoon Center";
					break;
				case "CochectonC":
					ret = "Cochecton Center";
					break;
				case "CooksFalls":
					ret = "Cooks Falls";
					break;
				case "Cuddebackv":
					ret = "Cuddebackville";
					break;
				case "DingmansFe":
					ret = "Dingmans Ferry";
					break;
				case "EastBranch":
					ret = "East Branch";
					break;
				case "Ellenvill1":
					ret = "Ellenville Village";
					break;
				case "Ellenville":
					ret = "Ellenville";
					break;
				case "FremontCen":
					ret = "Fremont Center";
					break;
				case "Grahamsvil":
					ret = "Grahamsville";
					break;
				case "GreenwoodL":
					ret = "Greenwood Lake";
					break;
				case "HighlandLa":
					ret = "Highland Lake";
					break;
				case "Hortonvill":
					ret = "Hortonville";
					break;
				case "Hurleyvill":
					ret = "Hurleyville";
					break;
				case "Jefferson":
					ret = "Jeffersonville";
					break;
				case "Jeffersonv":
					ret = "Jeffersonville Village";
					break;
				case "KenozaLake":
					ret = "Kenoza Lake";
					break;
				case "KiameshaLa":
					ret = "Kiamesha Lake";
					break;
				case "LakeHuntin":
					ret = "Lake Huntington";
					break;
				case "LibertyVil":
					ret = "Liberty Village";
					break;
				case "Livingston":
					ret = "Livingston Manor";
					break;
				case "LochSheldr":
					ret = "Loch Sheldrake";
					break;
				case "LordsValle":
					ret = "Lords Valley";
					break;
				case "MastenLake":
					ret = "Masten Lake";
					break;
				case "MohicanLak":
					ret = "Mohican Lake";
					break;
				case "MongaupVal":
					ret = "Mongaup Valley";
					break;
				case "Monticell1":
					ret = "Monticello Village";
					break;
				case "Mountaind1":
					ret = "Mountaindale Village";
					break;
				case "Mountainda":
					ret = "Mountaindale";
					break;
				case "Narrowsbur":
					ret = "Narrowsburg";
					break;
				case "NewWindsor":
					ret = "New Windsor";
					break;
				case "NorthBranc":
					ret = "North Branch";
					break;
				case "Phillipspo":
					ret = "Phillipsport";
					break;
				case "PortJervis":
					ret = "Port Jervis";
					break;
				case "SouthFalls":
					ret = "South Fallsburg";
					break;
				case "Sparrowbus":
					ret = "Sparrowbush";
					break;
				case "SpringGlen":
					ret = "Spring Glen";
					break;
				case "Summitvill":
					ret = "Summitville";
					break;
				case "ThompsonRi":
					ret = "Thompson Ridge";
					break;
				case "Thompsonvi":
					ret = "Thompsonville";
					break;
				case "WalkerVall":
					ret = "Walker Valley";
					break;
				case "Westbrookv":
					ret = "Westbrookville";
					break;
				case "WhiteSulph":
					ret = "White Sulphur Springs";
					break;
				case "WoodridgeV":
					ret = "Woodridge Village";
					break;
				case "WursboroHi":
					ret = "Wursboro Hills";
					break;
				case "WurtsboroV":
					ret = "Wurtsboro Village";
					break;
				case "YankeeLake":
					ret = "Yankee Lake";
					break;
				case "Youngsvill":
					ret = "Youngsville";
					break;
				case "Ulster P":
					ret = "Ulster Park";
					break;
				case "KauneongaL":
					ret = "Kauneonga Lake";
					break;
				case "Spring Val":
					ret = "Spring Valley";
					break;
				default:
					ret = data;
					break;
			}
			return ret;
		}
		public string TranslateConstruction(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "Block";
					break;
				case "1":
					ret = "Concrete";
					break;
				case "2":
					ret = "Manufactured";
					break;
				case "3":
					ret = "Mixed";
					break;
				case "4":
					ret = "Modular";
					break;
				case "5":
					ret = "Steel";
					break;
				case "6":
					ret = "Wood Frame";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateElectric(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "3 Phase";
					break;
				case "1":
					ret = "At Site";
					break;
				case "2":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateElectricComm(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "None";
					break;
				case "1":
					ret = "3 Phase";
					break;
				case "2":
					ret = "Available";
					break;
				case "3":
					ret = "On Site";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateGarageType(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "None";
					break;
				case "1":
					ret = "Attached Garage";
					break;
				case "2":
					ret = "Detached Garage";
					break;
				case "3":
					ret = "Attached Carport";
					break;
				case "4":
					ret = "Detached Carport";
					break;
				case "5":
					ret = "Under";
					break;
				case "6":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateListingType(string data) {
			string ret = "";
			switch (data) {
				case "ER":
					ret = "Exclusive Rights";
					break;
				case "EA":
					ret = "Exclusive Agency";
					break;
				case "OP":
					ret = "Open";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateSewer(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "Central Available";
					break;
				case "1":
					ret = "Central At Site";
					break;
				case "2":
					ret = "Septic System On Site";
					break;
				case "3":
					ret = "Septic System Required";
					break;
				case "4":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateSewerComm(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "None";
					break;
				case "1":
					ret = "Municipal";
					break;
				case "2":
					ret = "Community";
					break;
				case "3":
					ret = "Septic";
					break;
				case "4":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateStatus(string data) {
			string ret = "";
			switch (data) {
				case "1_0":
					ret = "Active";
					break;
				case "2_0":
					ret = "Sold";
					break;
				case "2_1":
					ret = "Sold-Non MLS";
					break;
				case "2_2":
					ret = "Sold-By Owner";
					break;
				case "2_3":
					ret = "Sold-Open Listing";
					break;
				case "3_0":
					ret = "Contract Signed";
					break;
				case "4_0":
					ret = "Expired";
					break;
				case "5_0":
					ret = "Cancelled";
					break;
				case "5_1":
					ret = ">Temp. Off Market";
					break;
				case "6_0":
					ret = "Rented";
					break;
				default:
					ret = "Active";
					break;
			}
			return ret;
		}
		public string TranslateType(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "Single Family";
					break;
				case "1":
					ret = "Condominium";
					break;
				case "2":
					ret = "2 Family";
					break;
				case "3":
					ret = "Townhouse";
					break;
				case "4":
					ret = "Seasonal";
					break;
				case "5":
					ret = "Other";
					break;
				case "6":
					ret = "Agricultural";
					break;
				case "7":
					ret = "Commercial";
					break;
				case "8":
					ret = "Forest";
					break;
				case "9":
					ret = "Mixed";
					break;
				case "10":
					ret = "Non-Conforming";
					break;
				case "11":
					ret = "Residential";
					break;
				case "12":
					ret = "Other";
					break;
				case "13":
					ret = "Commercial";
					break;
				case "14":
					ret = "Industrial";
					break;
				case "15":
					ret = "Business Only";
					break;
				case "16":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateWater(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "Central Available";
					break;
				case "1":
					ret = "Central At Site";
					break;
				case "2":
					ret = "Well Water On Site";
					break;
				case "3":
					ret = "Well Required";
					break;
				case "4":
					ret = "Seasonal";
					break;
				case "5":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
		}
		public string TranslateWaterComm(string data) {
			string ret = "";
			switch (data) {
				case "0":
					ret = "None";
					break;
				case "1":
					ret = "Municipal";
					break;
				case "2":
					ret = "Community";
					break;
				case "3":
					ret = "Well";
					break;
				case "4":
					ret = "Drilled Well";
					break;
				case "5":
					ret = "Other";
					break;
				default:
					ret = "Other";
					break;
			}
			return ret;
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

	public class SearchModel {
		public string[] City { get; set; }
		public int Bedrooms { get; set; }
		public int Bedrooms2 { get; set; }
		public int Bathrooms { get; set; }
		public int Bathrooms2 { get; set; }
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

	public class SearchResult {
		public List<Listing> Listings { get; set; }
		public string PrevPage { get; set; }
		public string NextPage { get; set; }
		public int TotalResults { get; set; }
	}
	public class ListingResult {
		public Listing listing { get; set; }
		public SearchResult searchResult { get; set; }
		public int ResultCount { get; set; }
	}
}