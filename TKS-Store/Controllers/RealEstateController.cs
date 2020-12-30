using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Gaiocorp.Geocoding;
using TKS.Areas.Admin.Models;
using TKS.Models.realestate;

namespace TKS.Controllers.CMS {
	public class RealEstateController : Controller {
        public ActionResult Sold() {
            //string qString = "/sold";

            //ContentPage contentPage = new ContentPage(qString);
            //ViewData = contentPage.GetSections();
            //ViewBag.Meta = contentPage.MetaTags;
            ViewBag.City = new Listing().GetCitySelectList("");

            return View(new SoldSet().Properties);
        }

        public ActionResult Search(SearchModel data) {
			int page = 1;
			int PostsPerPage = 30;
			if (Request.QueryString["page"] != null) {
				Int32.TryParse(Request.QueryString["page"], out page);
			}
			if (!Request.QueryString.HasKeys()) {
				data.MinPrice = 200;
				data.MaxPrice = 400;
			}

			if (data.MaxPrice > 0 && data.MinPrice > data.MaxPrice) {
				int min = data.MinPrice;
				data.MinPrice = data.MaxPrice;
				data.MaxPrice = min;
			}
			if (data.PriceRange == 1) {
				data.MinPrice = 0;
				data.MaxPrice = 200;
			} else if (data.PriceRange == 2) {
				data.MinPrice = 200;
				data.MaxPrice = 500;
			} else if (data.PriceRange == 3) {
				data.MinPrice = 500;
				data.MaxPrice = 1000;
			} else if (data.PriceRange == 4) {
				data.MinPrice = 1000;
				data.MaxPrice = 0;
			}

			if (data.Acres2 > 0 && data.Acres > data.Acres2) {
				int min = data.Acres;
				data.Acres = data.Acres2;
				data.Acres2 = min;
			}
			if (data.AcreRange == 1) {
				data.Acres = 1;
				data.Acres2 = 5;
			} else if (data.AcreRange == 2) {
				data.Acres = 5;
				data.Acres2 = 10;
			} else if (data.AcreRange == 3) {
				data.Acres = 10;
				data.Acres2 = 20;
			} else if (data.AcreRange == 4) {
				data.Acres = 20;
				data.Acres2 = 0;
			}
			if (data.Bathrooms2 > 0 && data.Bathrooms > data.Bathrooms2) {
				int min = data.Bathrooms;
				data.Bathrooms = data.Bathrooms2;
				data.Bathrooms2 = min;
			}
			if (data.Bedrooms2 > 0 && data.Bedrooms > data.Bedrooms2) {
				int min = data.Bedrooms;
				data.Bedrooms = data.Bedrooms2;
				data.Bedrooms2 = min;
			}
			if (data.SqFt2 > 0 && data.SqFt > data.SqFt2) {
				int min = data.SqFt;
				data.SqFt = data.SqFt2;
				data.SqFt2 = min;
			}
			if (data.Years2 > 0 && data.Years > data.Years2) {
				int min = data.Years;
				data.Years = data.Years2;
				data.Years2 = min;
			}

			MLSListings list = new MLSListings();
			ViewBag.City = list.GetCitySelectList(data.City);
			ViewBag.Acres = list.GetAcreSelectList(data.Acres.ToString());
			ViewBag.Acres2 = list.GetAcreSelectList(data.Acres2.ToString());
			ViewBag.Bathrooms = list.GetBathroomSelectList(data.Bathrooms.ToString());
			ViewBag.Bathrooms2 = list.GetBathroomSelectList(data.Bathrooms2.ToString());
			ViewBag.Bedrooms = list.GetBedroomSelectList(data.Bedrooms.ToString());
			ViewBag.Bedrooms2 = list.GetBedroomSelectList(data.Bedrooms2.ToString());
			ViewBag.MinPrice = list.GetPriceSelectList(data.MinPrice.ToString());
			ViewBag.MaxPrice = list.GetPriceSelectList(data.MaxPrice.ToString());
			ViewBag.SqFt = list.GetSqFtSelectList(data.SqFt.ToString());
			ViewBag.SqFt2 = list.GetSqFtSelectList(data.SqFt2.ToString());
			ViewBag.Years = list.GetYearsSelectList(data.Years.ToString());
			ViewBag.Years2 = list.GetYearsSelectList(data.Years2.ToString());
			ViewBag.Garage = data.Garage;
			ViewBag.Pool = data.Pool;
			ViewBag.Fireplace = data.Fireplace;
			ViewBag.Barn = data.Barn;
			ViewBag.Handicap = data.Handicap;
			ViewBag.Skylights = data.Skylights;
			ViewBag.Lake = data.Lake;
			ViewBag.Meta = string.Format("<title>{0}</title>", "Your Search Results " + Global.TitleSuffixSep + Global.TitleSuffix);

			string param = Request.QueryString != null ? Request.QueryString.ToString().ToLower() : "";
			if (Request.QueryString["mapit"] != null) {
				ViewBag.Reformat = param.Replace("mapit", "listing");
				List<MLSListing> MapListings = new MLSListings().GetMapListings(data);
				return View("Search2", MapListings);
			} else {
				if (param.Contains("listing")) {
					ViewBag.Reformat = param.Replace("listing", "mapit");
				} else {
					ViewBag.Reformat = param + "&mapit=1";
				}
				MLSSearchResult res = new MLSListings().DoSearch(data, page, PostsPerPage);
				ViewBag.Start = ((page - 1) * PostsPerPage) + 1;
				ViewBag.End = (page * PostsPerPage) < res.TotalResults ? (page * PostsPerPage) : res.TotalResults;

				return View(res);
			}
		}
		public ActionResult Detail(SearchModel data) {
			if (string.IsNullOrEmpty(data.MLS)) { return Redirect("/search"); }
			MLSListing listing = new MLSListing(data.MLS);
			//Listing3 listing = new Listing3(data.MLS);
			if (string.IsNullOrEmpty(listing.MLS)) { return Redirect("/search"); }
			ViewBag.Meta = listing.GetTags();
			GlobalLM glob = new GlobalLM();
			listing.DownpaymentPercent = float.Parse(glob.DownPayment.ToString());
			listing.Percent30yr = float.Parse(glob.Fixed30Year.ToString());//  3.75F;
			listing.Percent15yr = float.Parse(glob.Fixed15Year.ToString());// 2.875F;
			listing.PercentARM = float.Parse(glob.ARM51.ToString());// 2.625F;

			string param = "";
			if (data.City != null && !string.IsNullOrEmpty(data.City[0])) {
				for (int x = 1; x <= data.City.Length; x++) {
					param += "&city=" + HttpUtility.UrlEncode(data.City[x - 1]);
				}
			}
			if (data.Acres > 0) { param += "&acres=" + data.Acres.ToString(); }
			if (data.Acres2 > 0) { param += "&acres2=" + data.Acres2.ToString(); }
			if (data.Bathrooms > 0) { param += "&bathrooms=" + data.Bathrooms.ToString(); }
			if (data.Bathrooms2 > 0) { param += "&bathrooms2=" + data.Bathrooms2.ToString(); }
			if (data.Bedrooms > 0) { param += "&bedrooms=" + data.Bedrooms.ToString(); }
			if (data.Bedrooms2 > 0) { param += "&bedrooms2=" + data.Bedrooms2.ToString(); }
			if (data.MinPrice > 0) { param += "&minprice=" + data.MinPrice.ToString(); }
			if (data.MaxPrice > 0) { param += "&maxprice=" + data.MaxPrice.ToString(); }
			if (data.SqFt > 0) { param += "&sqft=" + data.SqFt.ToString(); }
			if (data.SqFt2 > 0) { param += "&sqft2=" + data.SqFt2.ToString(); }
			if (data.Years > 0) { param += "&years=" + data.Years.ToString(); }
			if (data.Years2 > 0) { param += "&years2=" + data.Years2.ToString(); }
			if (data.Garage == 1) { param += "&garage=1"; }
			if (data.Pool == 1) { param += "&pool=1"; }
			if (data.Fireplace == 1) { param += "&fireplace=1"; }
			if (data.Barn == 1) { param += "&barn=1"; }
			if (data.Handicap == 1) { param += "&handicap=1"; }
			if (data.Skylights == 1) { param += "&skylights=1"; }
			if (data.Lake == 1) { param += "&lake=1"; }
			bool hasParams = !string.IsNullOrEmpty(param);
			int page = 1;
			if (Request.QueryString["page"] != null) {
				Int32.TryParse(Request.QueryString["page"], out page);
			}
			if (page > 0) { param += "&page=" + page.ToString(); }
			if (Request.QueryString["mapit"] != null) {
				param += "&mapit=1";
			}

			if (hasParams) {
				ViewBag.SearchAgainURL = "/search?" + param;
                ViewBag.Params = param;
            } else {
				ViewBag.SearchAgainURL = "/search?city=" + HttpUtility.UrlEncode(listing.City);
                ViewBag.Params = "&city=" + HttpUtility.UrlEncode(listing.City);
            }

            data.City = new string[] { listing.City };
			if (data.MaxPrice > 0) {
				data.MaxPrice = data.MaxPrice * 2;
			} else {
				data.MaxPrice = Convert.ToInt32((listing.AskingAmt / 1000) * 2);
			}

			MLSSearchResult searchResult = new MLSListings().DoSearch(data, 1, 5);

			ListingResult3 listingResult = new ListingResult3();
			listingResult.listing = listing;
			listingResult.searchResult = searchResult;
			return View(listingResult);
		}

		public ActionResult SearchComm(SearchModel data) {
			int page = 1;
			int PostsPerPage = 30;
			if (Request.QueryString["page"] != null) {
				Int32.TryParse(Request.QueryString["page"], out page);
			}
			if (!Request.QueryString.HasKeys()) {
				data.MinPrice = 200;
				data.MaxPrice = 400;
			}

			if (data.MaxPrice > 0 && data.MinPrice > data.MaxPrice) {
				int min = data.MinPrice;
				data.MinPrice = data.MaxPrice;
				data.MaxPrice = min;
			}
			if (data.PriceRange == 1) {
				data.MinPrice = 0;
				data.MaxPrice = 200;
			} else if (data.PriceRange == 2) {
				data.MinPrice = 200;
				data.MaxPrice = 500;
			} else if (data.PriceRange == 3) {
				data.MinPrice = 500;
				data.MaxPrice = 1000;
			} else if (data.PriceRange == 4) {
				data.MinPrice = 1000;
				data.MaxPrice = 0;
			}

			if (data.Acres2 > 0 && data.Acres > data.Acres2) {
				int min = data.Acres;
				data.Acres = data.Acres2;
				data.Acres2 = min;
			}
			if (data.Acres == 1001) {
				data.Acres = 1;
				data.Acres2 = 5;
			} else if (data.Acres == 1002) {
				data.Acres = 5;
				data.Acres2 = 10;
			} else if (data.Acres == 1003) {
				data.Acres = 10;
				data.Acres2 = 20;
			} else if (data.Acres == 1004) {
				data.Acres = 20;
				data.Acres2 = 0;
			}
			if (data.SqFt2 > 0 && data.SqFt > data.SqFt2) {
				int min = data.SqFt;
				data.SqFt = data.SqFt2;
				data.SqFt2 = min;
			}

			MLSListingsComm list = new MLSListingsComm();
			ViewBag.City = list.GetCitySelectList(data.City);
			ViewBag.Acres = list.GetAcreSelectList(data.Acres.ToString());
			ViewBag.Acres2 = list.GetAcreSelectList(data.Acres2.ToString());
			ViewBag.MinPrice = list.GetPriceSelectList(data.MinPrice.ToString());
			ViewBag.MaxPrice = list.GetPriceSelectList(data.MaxPrice.ToString());
			ViewBag.SqFt = list.GetSqFtSelectList(data.SqFt.ToString());
			ViewBag.SqFt2 = list.GetSqFtSelectList(data.SqFt2.ToString());
			ViewBag.Meta = string.Format("<title>{0}</title>", "Your Search Results " + Global.TitleSuffixSep + Global.TitleSuffix);

			string param = Request.QueryString != null ? Request.QueryString.ToString().ToLower() : "";
			if (Request.QueryString["mapit"] != null) {
				ViewBag.Reformat = param.Replace("mapit", "listing");
				List<MLSListing> MapListings = new MLSListingsComm().GetMapListings(data);
				ViewBag.TotalCount = MapListings.Count;
				return View("SearchComm2", MapListings);
			} else {
				if (param.Contains("listing")) {
					ViewBag.Reformat = param.Replace("listing", "mapit");
				} else {
					ViewBag.Reformat = param + "&mapit=1";
				}
				MLSSearchResult res = new MLSListingsComm().DoSearch(data, page, PostsPerPage);
				ViewBag.Start = ((page - 1) * PostsPerPage) + 1;
				ViewBag.End = (page * PostsPerPage) < res.TotalResults ? (page * PostsPerPage) : res.TotalResults;

				return View(res);
			}
		}
		public ActionResult CommDetail(SearchModel data) {
			if (string.IsNullOrEmpty(data.MLS)) { return Redirect("/searchcomm"); }
			MLSListing listing = new MLSListing(data.MLS);
			if (string.IsNullOrEmpty(listing.MLS)) { return Redirect("/searchcomm"); }
			ViewBag.Meta = listing.GetTags();
			GlobalLM glob = new GlobalLM();
			listing.DownpaymentPercent = float.Parse(glob.DownPayment.ToString());
			listing.Percent30yr = float.Parse(glob.Fixed30Year.ToString());//  3.75F;
			listing.Percent15yr = float.Parse(glob.Fixed15Year.ToString());// 2.875F;
			listing.PercentARM = float.Parse(glob.ARM51.ToString());// 2.625F;

			string param = "";
			if (data.City != null && !string.IsNullOrEmpty(data.City[0])) {
				for (int x = 1; x <= data.City.Length; x++) {
					param += "&city=" + HttpUtility.UrlEncode(data.City[x - 1]);
				}
			}
			if (data.Acres > 0) { param += "&acres=" + data.Acres.ToString(); }
			if (data.Acres2 > 0) { param += "&acres2=" + data.Acres2.ToString(); }
			if (data.MinPrice > 0) { param += "&minprice=" + data.MinPrice.ToString(); }
			if (data.MaxPrice > 0) { param += "&maxprice=" + data.MaxPrice.ToString(); }
			if (data.SqFt > 0) { param += "&sqft=" + data.SqFt.ToString(); }
			if (data.SqFt2 > 0) { param += "&sqft2=" + data.SqFt2.ToString(); }
			bool hasParams = !string.IsNullOrEmpty(param);
			int page = 1;
			if (Request.QueryString["page"] != null) {
				Int32.TryParse(Request.QueryString["page"], out page);
			}
			if (page > 0) { param += "&page=" + page.ToString(); }
			if (Request.QueryString["mapit"] != null) {
				param += "&mapit=1";
			}

			if (hasParams) {
				ViewBag.SearchAgainURL = "/searchcomm?" + param;
                ViewBag.Params = param;
			} else {
				ViewBag.SearchAgainURL = "/searchcomm?city=" + HttpUtility.UrlEncode(listing.City);
                ViewBag.Params = "&city=" + HttpUtility.UrlEncode(listing.City);
            }

            data.City = new string[] { listing.City };
			if (data.MaxPrice > 0) {
				data.MaxPrice = data.MaxPrice * 2;
			} else {
				data.MaxPrice = Convert.ToInt32((listing.AskingAmt / 1000) * 2);
			}

			MLSSearchResult searchResult = new MLSListingsComm().DoSearch(data, 1, 5);

			ListingResult3 listingResult = new ListingResult3();
			listingResult.listing = listing;
			listingResult.searchResult = searchResult;
			return View(listingResult);
		}

		public ActionResult SearchLand(SearchModel data) {

			int page = 1;
			int PostsPerPage = 30;
			if (Request.QueryString["page"] != null) {
				Int32.TryParse(Request.QueryString["page"], out page);
			}
			if (!Request.QueryString.HasKeys()) {
				data.MinPrice = 200;
				data.MaxPrice = 400;
			}

			if (data.MaxPrice > 0 && data.MinPrice > data.MaxPrice) {
				int min = data.MinPrice;
				data.MinPrice = data.MaxPrice;
				data.MaxPrice = min;
			}
			if (data.PriceRange == 1) {
				data.MinPrice = 0;
				data.MaxPrice = 50;
			} else if (data.PriceRange == 2) {
				data.MinPrice = 50;
				data.MaxPrice = 100;
			} else if (data.PriceRange == 3) {
				data.MinPrice = 100;
				data.MaxPrice = 175;
			} else if (data.PriceRange == 4) {
				data.MinPrice = 175;
				data.MaxPrice = 0;
			}

			if (data.Acres2 > 0 && data.Acres > data.Acres2) {
				int min = data.Acres;
				data.Acres = data.Acres2;
				data.Acres2 = min;
			}
			if (data.Acres == 1001) {
				data.Acres = 1;
				data.Acres2 = 5;
			} else if (data.Acres == 1002) {
				data.Acres = 5;
				data.Acres2 = 10;
			} else if (data.Acres == 1003) {
				data.Acres = 10;
				data.Acres2 = 20;
			} else if (data.Acres == 1004) {
				data.Acres = 20;
				data.Acres2 = 0;
			}

			MLSListingsLand list = new MLSListingsLand();
			ViewBag.City = list.GetCitySelectList(data.City);
			ViewBag.Acres = list.GetAcreSelectList(data.Acres.ToString());
			ViewBag.Acres2 = list.GetAcreSelectList(data.Acres2.ToString());
			ViewBag.MinPrice = list.GetPriceSelectList(data.MinPrice.ToString());
			ViewBag.MaxPrice = list.GetPriceSelectList(data.MaxPrice.ToString());
			ViewBag.Lake = data.Lake;
			ViewBag.Meta = string.Format("<title>{0}</title>", "Your Search Results " + Global.TitleSuffixSep + Global.TitleSuffix);

			string param = Request.QueryString != null ? Request.QueryString.ToString().ToLower() : "";
			if (Request.QueryString["mapit"] != null) {
				ViewBag.Reformat = param.Replace("mapit", "listing");
				List<MLSListing> MapListings = new MLSListingsLand().GetMapListings(data);
				ViewBag.TotalCount = MapListings.Count;
				return View("SearchLand2", MapListings);
			} else {
				if (param.Contains("listing")) {
					ViewBag.Reformat = param.Replace("listing", "mapit");
				} else {
					ViewBag.Reformat = param + "&mapit=1";
				}
				MLSSearchResult res = new MLSListingsLand().DoSearch(data, page, PostsPerPage);
				ViewBag.Start = ((page - 1) * PostsPerPage) + 1;
				ViewBag.End = (page * PostsPerPage) < res.TotalResults ? (page * PostsPerPage) : res.TotalResults;

				return View(res);
			}
		}
		public ActionResult LandDetail(SearchModel data) {
			if (string.IsNullOrEmpty(data.MLS)) { return Redirect("/search"); }
			MLSListing listing = new MLSListing(data.MLS);
			if (string.IsNullOrEmpty(listing.MLS)) { return Redirect("/search"); }
			ViewBag.Meta = listing.GetTags();
			GlobalLM glob = new GlobalLM();
			listing.DownpaymentPercent = float.Parse(glob.DownPayment.ToString());
			listing.Percent30yr = float.Parse(glob.Fixed30Year.ToString());//  3.75F;
			listing.Percent15yr = float.Parse(glob.Fixed15Year.ToString());// 2.875F;
			listing.PercentARM = float.Parse(glob.ARM51.ToString());// 2.625F;

			string param = "";
			if (data.City != null && !string.IsNullOrEmpty(data.City[0])) {
				for (int x = 1; x <= data.City.Length; x++) {
					param += "&city=" + HttpUtility.UrlEncode(data.City[x - 1]);
				}
			}
			if (data.Acres > 0) { param += "&acres=" + data.Acres.ToString(); }
			if (data.Acres2 > 0) { param += "&acres2=" + data.Acres2.ToString(); }
			if (data.MinPrice > 0) { param += "&minprice=" + data.MinPrice.ToString(); }
			if (data.MaxPrice > 0) { param += "&maxprice=" + data.MaxPrice.ToString(); }
			bool hasParams = !string.IsNullOrEmpty(param);
			int page = 1;
			if (Request.QueryString["page"] != null) {
				Int32.TryParse(Request.QueryString["page"], out page);
			}
			if (page > 0) { param += "&page=" + page.ToString(); }
			if (Request.QueryString["mapit"] != null) {
				param += "&mapit=1";
			}

			if (hasParams) {
				ViewBag.SearchAgainURL = "/searchland?" + param;
				ViewBag.Params = param;
			} else {
				ViewBag.SearchAgainURL = "/searchland?city=" + HttpUtility.UrlEncode(listing.City);
				ViewBag.Params = "&city=" + HttpUtility.UrlEncode(listing.City);
			}

			data.City = new string[] { listing.City };
			if (data.MaxPrice > 0) {
				data.MaxPrice = data.MaxPrice * 2;
			} else {
				data.MaxPrice = Convert.ToInt32((listing.AskingAmt / 1000) * 2);
			}

			MLSSearchResult searchResult = new MLSListingsLand().DoSearch(data, 1, 5);

			ListingResult3 listingResult = new ListingResult3();
			listingResult.listing = listing;
			listingResult.searchResult = searchResult;
			return View(listingResult);
		}

		public ActionResult ImportSullivanComm() {
			Server.ScriptTimeout = 300;

			using (StreamReader sr = new StreamReader(Server.MapPath("/dl/listings-commercial_industrial.txt"))) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-commercial]", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
					using (SqlCommand cmd = new SqlCommand("usp_InsertSullivanComm", cn)) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@MLS", SqlDbType.Float);
						cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Township", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AskingPrice", SqlDbType.Float);
						cmd.Parameters.Add("@AddressNumber", SqlDbType.Float);
						cmd.Parameters.Add("@AddressDirection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AddressStreet", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@City", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@State", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Zip", SqlDbType.Float);
						cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SaleRent", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Construction", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Stories", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@GarageBay", SqlDbType.Int);
						cmd.Parameters.Add("@Electric", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Water", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Sewer", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@NumberOfAcres", SqlDbType.Float);
						cmd.Parameters.Add("@NumberOfUnits", SqlDbType.Int);
						cmd.Parameters.Add("@AgentID", SqlDbType.Float);
						cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AgentPhone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1ID", SqlDbType.Float);
						cmd.Parameters.Add("@ListingOffice1Name", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1Phone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Referral", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@County", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Development", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxYearBuilt", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxTotalSquareFeet", SqlDbType.Float);
						cmd.Parameters.Add("@WarehouseSqFtHeated", SqlDbType.Float);
						cmd.Parameters.Add("@WarehouseSqFtUnheated", SqlDbType.Float);
						cmd.Parameters.Add("@Buildings", SqlDbType.Int);
						cmd.Parameters.Add("@Building1SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@Building2SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@Building3SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@Building4SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@TaxMapSection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapBlock", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapLot", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxTotalTaxes", SqlDbType.Float);
						cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
						cmd.Parameters.Add("@LandValue", SqlDbType.Float);
						cmd.Parameters.Add("@TotalValue", SqlDbType.Float);
						cmd.Parameters.Add("@Zoning", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@BrokerComments", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Golf", SqlDbType.NVarChar);
						cmd.Parameters.Add("@VirtualTour", SqlDbType.NVarChar, 255);

						string line;
						sr.ReadLine(); //Burn the header line
						while ((line = sr.ReadLine()) != null) {
							string[] fields = line.Split('\t');

							cmd.Parameters["@MLS"].Value = GetFloat(fields[0]);
							cmd.Parameters["@Class"].Value = fields[1];
							cmd.Parameters["@Type"].Value = fields[2];
							cmd.Parameters["@Township"].Value = fields[3];
							SqlDouble temp = GetFloat(fields[4]);
							cmd.Parameters["@AskingPrice"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[5]);
							cmd.Parameters["@AddressNumber"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AddressDirection"].Value = fields[6];
							cmd.Parameters["@AddressStreet"].Value = fields[7];
							cmd.Parameters["@Address2"].Value = fields[8];
							cmd.Parameters["@City"].Value = fields[9];
							cmd.Parameters["@State"].Value = fields[10];
							cmd.Parameters["@Zip"].Value = GetFloat(fields[11]);
							cmd.Parameters["@Status"].Value = fields[12];
							cmd.Parameters["@SaleRent"].Value = fields[13];
							cmd.Parameters["@Construction"].Value = fields[14];
							cmd.Parameters["@Stories"].Value = fields[15];
							cmd.Parameters["@GarageBay"].Value = GetInt(fields[16]);
							cmd.Parameters["@Electric"].Value = fields[17];
							cmd.Parameters["@Water"].Value = fields[18];
							cmd.Parameters["@Sewer"].Value = fields[19];
							cmd.Parameters["@ListingType"].Value = fields[20];
							temp = GetFloat(fields[21]);
							cmd.Parameters["@NumberOfAcres"].Value = temp > -1 ? temp : SqlDouble.Null;
							SqlInt32 temp2 = GetInt(fields[22]);
							cmd.Parameters["@NumberOfUnits"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							cmd.Parameters["@AgentID"].Value = GetFloat(fields[23]);
							cmd.Parameters["@AgentName"].Value = fields[24];
							cmd.Parameters["@AgentPhone"].Value = fields[25];
							cmd.Parameters["@ListingOffice1ID"].Value = GetFloat(fields[26]);
							cmd.Parameters["@ListingOffice1Name"].Value = fields[27];
							cmd.Parameters["@ListingOffice1Phone"].Value = fields[28];
							cmd.Parameters["@Referral"].Value = fields[33];
							cmd.Parameters["@SchoolDistrict"].Value = fields[38];
							cmd.Parameters["@County"].Value = fields[40];
							cmd.Parameters["@Development"].Value = fields[41];
							temp = GetFloat(fields[42]);
							cmd.Parameters["@ApproxYearBuilt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[43]);
							cmd.Parameters["@ApproxTotalSquareFeet"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@WarehouseSqFtHeated"].Value = GetFloat(fields[44]);
							cmd.Parameters["@WarehouseSqFtUnheated"].Value = GetFloat(fields[45]);
							temp2 = GetInt(fields[46]);
							cmd.Parameters["@Buildings"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							temp = GetFloat(fields[47]);
							cmd.Parameters["@Building1SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[48]);
							cmd.Parameters["@Building2SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[49]);
							cmd.Parameters["@Building3SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[50]);
							cmd.Parameters["@Building4SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@TaxMapSection"].Value = fields[51];
							cmd.Parameters["@TaxMapBlock"].Value = fields[52];
							cmd.Parameters["@TaxMapLot"].Value = fields[53];
							cmd.Parameters["@ApproxTotalTaxes"].Value = GetFloat(fields[54]);
							cmd.Parameters["@TaxYear"].Value = GetInt(fields[55]);
							cmd.Parameters["@LandValue"].Value = GetFloat(fields[56]);
							cmd.Parameters["@TotalValue"].Value = GetFloat(fields[57]);
							cmd.Parameters["@Zoning"].Value = fields[58];
							cmd.Parameters["@BrokerComments"].Value = fields[59];
							cmd.Parameters["@Remarks"].Value = fields[60];
							cmd.Parameters["@Golf"].Value = fields[89];
							cmd.Parameters["@VirtualTour"].Value = fields[90];
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}

			BingGeocoder.UpdateGeoCodingComm();

			return View();

		}
		public ActionResult ImportSullivanLand() {
			Server.ScriptTimeout = 300;

			using (StreamReader sr = new StreamReader(Server.MapPath("/dl/listings-land.txt"))) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-land]", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
					using (SqlCommand cmd = new SqlCommand("usp_InsertSullivanLand", cn)) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@MLS", SqlDbType.Float);
						cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Township", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AskingPrice", SqlDbType.Float);
						cmd.Parameters.Add("@AddressNumber", SqlDbType.Float);
						cmd.Parameters.Add("@AddressDirection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AddressStreet", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@City", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@State", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Zip", SqlDbType.Float);
						cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SaleRent", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Acreage", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Electric", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Water", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Sewer", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@NumberofAcres", SqlDbType.Float);
						cmd.Parameters.Add("@PricePerAcre", SqlDbType.Float);
						cmd.Parameters.Add("@AgentID", SqlDbType.Float);
						cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AgentPhone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1ID", SqlDbType.Float);
						cmd.Parameters.Add("@ListingOffice1Name", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1Phone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Referral", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@County", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Development", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapSection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapBlock", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapLot", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxLotSizeFront", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxLotSizeSide", SqlDbType.Float);
						cmd.Parameters.Add("@LandValue", SqlDbType.Float);
						cmd.Parameters.Add("@Parcels", SqlDbType.Int);
						cmd.Parameters.Add("@ApproxTotalTaxes", SqlDbType.Float);
						cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
						cmd.Parameters.Add("@Subdividable", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxExemption", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@BrokerComments", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Golf", SqlDbType.NVarChar);

						string line;
						sr.ReadLine(); //Burn the header line
						while ((line = sr.ReadLine()) != null) {
							string[] fields = line.Split('\t');

							cmd.Parameters["@MLS"].Value = GetFloat(fields[0]);
							cmd.Parameters["@Class"].Value = fields[1];
							cmd.Parameters["@Type"].Value = fields[2];
							cmd.Parameters["@Township"].Value = fields[3];
							SqlDouble temp = GetFloat(fields[4]);
							cmd.Parameters["@AskingPrice"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[5]);
							cmd.Parameters["@AddressNumber"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AddressDirection"].Value = fields[6];
							cmd.Parameters["@AddressStreet"].Value = fields[7];
							cmd.Parameters["@Address2"].Value = fields[8];
							cmd.Parameters["@City"].Value = fields[9];
							cmd.Parameters["@State"].Value = fields[10];
							cmd.Parameters["@Zip"].Value = GetFloat(fields[11]);
							cmd.Parameters["@Status"].Value = fields[12];
							cmd.Parameters["@SaleRent"].Value = fields[13];
							cmd.Parameters["@Acreage"].Value = fields[14];
							cmd.Parameters["@Electric"].Value = fields[15];
							cmd.Parameters["@Water"].Value = fields[16];
							cmd.Parameters["@Sewer"].Value = fields[17];
							cmd.Parameters["@ListingType"].Value = fields[18];
							cmd.Parameters["@NumberofAcres"].Value = GetFloat(fields[19]);
							temp = GetFloat(fields[20]);
							cmd.Parameters["@PricePerAcre"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AgentID"].Value = GetFloat(fields[21]);
							cmd.Parameters["@AgentName"].Value = fields[22];
							cmd.Parameters["@AgentPhone"].Value = fields[23];
							cmd.Parameters["@ListingOffice1ID"].Value = GetFloat(fields[24]);
							cmd.Parameters["@ListingOffice1Name"].Value = fields[25];
							cmd.Parameters["@ListingOffice1Phone"].Value = fields[26];
							cmd.Parameters["@Referral"].Value = fields[31];
							cmd.Parameters["@SchoolDistrict"].Value = fields[36];
							cmd.Parameters["@County"].Value = fields[38];
							cmd.Parameters["@Development"].Value = fields[39];
							cmd.Parameters["@TaxMapSection"].Value = fields[40];
							cmd.Parameters["@TaxMapBlock"].Value = fields[41];
							cmd.Parameters["@TaxMapLot"].Value = fields[42];
							temp = GetFloat(fields[43]);
							cmd.Parameters["@ApproxLotSizeFront"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[44]);
							cmd.Parameters["@ApproxLotSizeSide"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[45]);
							cmd.Parameters["@LandValue"].Value = temp > -1 ? temp : SqlDouble.Null;
							SqlInt32 temp2 = GetInt(fields[46]);
							cmd.Parameters["@Parcels"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							temp = GetFloat(fields[47]);
							cmd.Parameters["@ApproxTotalTaxes"].Value = temp > -1 ? temp : SqlInt32.Null;
							temp2 = GetInt(fields[48]);
							cmd.Parameters["@TaxYear"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							cmd.Parameters["@Subdividable"].Value = fields[49];
							cmd.Parameters["@TaxExemption"].Value = fields[50];
							cmd.Parameters["@BrokerComments"].Value = fields[55];
							cmd.Parameters["@Remarks"].Value = fields[56];
							cmd.Parameters["@Golf"].Value = fields[81];
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}

			BingGeocoder.UpdateGeoCodingLand();
			//BingGeolocation? bl = BingGeocoder.ResolveAddress("25 S Lewcy lane", "White Lake", "NY", "12786", "US");
			//if (bl != null) {
			//	string temp = "Lat: " + bl.Value.Lat.ToString() + " Lon:" + bl.Value.Lon;
			//}

			//SendConfirm();
			return View();

		}
		public ActionResult ImportSullivanRes() {
			Server.ScriptTimeout = 300;
			//FtpWebRequest myFtpWebRequest = (FtpWebRequest)WebRequest.Create("ftp://gsmlsftp3.gsmls.com/Agent/Rev4_ResData.txt");

			//myFtpWebRequest.Credentials = new NetworkCredential("TechKnowSysCorp", "18Washington!");
			//myFtpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
			//myFtpWebRequest.UseBinary = false;

			//FtpWebResponse myFtpWebResponse = (FtpWebResponse)myFtpWebRequest.GetResponse();

			//StreamWriter myStreamWriter = new StreamWriter(Server.MapPath("/gsmls/Rev4_ResData.txt"));
			//myStreamWriter.Write(new StreamReader(myFtpWebResponse.GetResponseStream()).ReadToEnd());
			//myStreamWriter.Close();
			////litResponse.Text = myFtpWebResponse.StatusDescription
			//myFtpWebResponse.Close();

			using (StreamReader sr = new StreamReader(Server.MapPath("/dl/listings-residential.txt"))) {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-residential]", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
					using (SqlCommand cmd = new SqlCommand("usp_InsertSullivanRes", cn)) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@MLS", SqlDbType.Float);
						cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Township", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AskingPrice", SqlDbType.Float);
						cmd.Parameters.Add("@AddressNumber", SqlDbType.Float);
						cmd.Parameters.Add("@AddressDirection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AddressStreet", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@City", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@State", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Zip", SqlDbType.Float);
						cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SaleRent", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Rooms", SqlDbType.Float);
						cmd.Parameters.Add("@Bedrooms", SqlDbType.Float);
						cmd.Parameters.Add("@FullBaths", SqlDbType.Float);
						cmd.Parameters.Add("@HalfBaths", SqlDbType.Float);
						cmd.Parameters.Add("@GarageType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@GarageCapacity", SqlDbType.Float);
						cmd.Parameters.Add("@Fireplace", SqlDbType.Float);
						cmd.Parameters.Add("@ListingType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@NumberofAcres", SqlDbType.Float);
						cmd.Parameters.Add("@AgentID", SqlDbType.Float);
						cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AgentPhone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1ID", SqlDbType.Float);
						cmd.Parameters.Add("@ListingOffice1Name", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1Phone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@County", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Development", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxYearBuilt", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxLotSizeFront", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxLotSizeSide", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxSqFtAboveGrade", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxSqFtBelowGrade", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxTotalSquareFeet", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxTotalTaxes", SqlDbType.Float);
						cmd.Parameters.Add("@TaxExemption", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@LandValue", SqlDbType.Float);
						cmd.Parameters.Add("@TotalValue", SqlDbType.Float);
						cmd.Parameters.Add("@PossessionDate", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@MaintenanceFees", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Utilities", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@HomeownersFees", SqlDbType.Float);
						cmd.Parameters.Add("@FinancingTerms", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar);
						cmd.Parameters.Add("@AFrame", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@VirtualTour", SqlDbType.NVarChar, 255);

						string line;
						sr.ReadLine(); //Burn the header line
						while ((line = sr.ReadLine()) != null) {
							string[] fields = line.Split('\t');

							cmd.Parameters["@MLS"].Value = GetFloat(fields[0]);
							cmd.Parameters["@Class"].Value = fields[1];
							cmd.Parameters["@Type"].Value = fields[2];
							cmd.Parameters["@Township"].Value = fields[3];
							SqlDouble temp = GetFloat(fields[4]);
							cmd.Parameters["@AskingPrice"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[5]);
							cmd.Parameters["@AddressNumber"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AddressDirection"].Value = fields[6];
							cmd.Parameters["@AddressStreet"].Value = fields[7];
							cmd.Parameters["@Address2"].Value = fields[8];
							cmd.Parameters["@City"].Value = fields[9];
							cmd.Parameters["@State"].Value = fields[10];
							cmd.Parameters["@Zip"].Value = GetFloat(fields[11]);
							cmd.Parameters["@Status"].Value = fields[12];
							cmd.Parameters["@SaleRent"].Value = fields[13];
							cmd.Parameters["@Rooms"].Value = GetFloat(fields[14]);
							cmd.Parameters["@Bedrooms"].Value = GetFloat(fields[15]);
							cmd.Parameters["@FullBaths"].Value = GetFloat(fields[16]);
							cmd.Parameters["@HalfBaths"].Value = GetFloat(fields[17]);
							cmd.Parameters["@GarageType"].Value = fields[18];
							cmd.Parameters["@GarageCapacity"].Value = GetFloat(fields[19]);
							cmd.Parameters["@Fireplace"].Value = GetFloat(fields[20]);
							cmd.Parameters["@ListingType"].Value = fields[21];
							cmd.Parameters["@NumberofAcres"].Value = GetFloat(fields[23]);
							cmd.Parameters["@AgentID"].Value = GetFloat(fields[24]);
							cmd.Parameters["@AgentName"].Value = fields[25];
							cmd.Parameters["@AgentPhone"].Value = fields[26];
							cmd.Parameters["@ListingOffice1ID"].Value = GetFloat(fields[27]);
							cmd.Parameters["@ListingOffice1Name"].Value = fields[28];
							cmd.Parameters["@ListingOffice1Phone"].Value = fields[29];
							cmd.Parameters["@County"].Value = fields[38];
							cmd.Parameters["@SchoolDistrict"].Value = fields[39];
							cmd.Parameters["@Development"].Value = fields[42];
							cmd.Parameters["@ApproxYearBuilt"].Value = GetFloat(fields[43]);
							cmd.Parameters["@ApproxLotSizeFront"].Value = GetFloat(fields[44]);
							cmd.Parameters["@ApproxLotSizeSide"].Value = GetFloat(fields[45]);
							cmd.Parameters["@ApproxSqFtAboveGrade"].Value = GetFloat(fields[49]);
							cmd.Parameters["@ApproxSqFtBelowGrade"].Value = GetFloat(fields[50]);
							cmd.Parameters["@ApproxTotalSquareFeet"].Value = GetFloat(fields[51]);
							cmd.Parameters["@ApproxTotalTaxes"].Value = GetFloat(fields[91]);
							cmd.Parameters["@TaxExemption"].Value = fields[92];
							cmd.Parameters["@LandValue"].Value = GetFloat(fields[93]);
							cmd.Parameters["@TotalValue"].Value = GetFloat(fields[94]);
							cmd.Parameters["@PossessionDate"].Value = fields[95];
							cmd.Parameters["@MaintenanceFees"].Value = fields[96];
							cmd.Parameters["@Utilities"].Value = fields[97];
							cmd.Parameters["@HomeownersFees"].Value = GetFloat(fields[98]);
							cmd.Parameters["@FinancingTerms"].Value = fields[99];
							cmd.Parameters["@Remarks"].Value = fields[100];
							cmd.Parameters["@AFrame"].Value = fields[114];
							cmd.Parameters["@VirtualTour"].Value = fields[115];
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}
				}
			}

			BingGeocoder.UpdateGeoCoding();
			//BingGeolocation? bl = BingGeocoder.ResolveAddress("25 S Lewcy lane", "White Lake", "NY", "12786", "US");
			//if (bl != null) {
			//	string temp = "Lat: " + bl.Value.Lat.ToString() + " Lon:" + bl.Value.Lon;
			//}

			//SendConfirm();
			return View();

		}

		public ActionResult ImportSullivanComm2() {
			Server.ScriptTimeout = 300;

			//using (StreamReader sr = new StreamReader(@"C:\Projects\Clients\KKPR\CatskillFarms\lazy-meadow\Materials\commercial.txt")) {
			using (StreamReader sr = new StreamReader(@"E:\Sites\LazyMeadows\downloads\commercial.txt")) {
				Listings listings = new Listings();

				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-commercial]", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
					using (SqlCommand cmd = new SqlCommand("usp_InsertSullivanComm", cn)) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@MLS", SqlDbType.Float);
						cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Township", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AskingPrice", SqlDbType.Float);
						cmd.Parameters.Add("@AddressNumber", SqlDbType.Float);
						cmd.Parameters.Add("@AddressDirection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AddressStreet", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@City", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@State", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Zip", SqlDbType.Float);
						cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SaleRent", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Construction", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Stories", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@GarageBay", SqlDbType.Int);
						cmd.Parameters.Add("@Electric", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Water", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Sewer", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@NumberOfAcres", SqlDbType.Float);
						cmd.Parameters.Add("@NumberOfUnits", SqlDbType.Int);
						cmd.Parameters.Add("@AgentID", SqlDbType.Float);
						cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AgentPhone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1ID", SqlDbType.Float);
						cmd.Parameters.Add("@ListingOffice1Name", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1Phone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Referral", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@County", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Development", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxYearBuilt", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxTotalSquareFeet", SqlDbType.Float);
						cmd.Parameters.Add("@WarehouseSqFtHeated", SqlDbType.Float);
						cmd.Parameters.Add("@WarehouseSqFtUnheated", SqlDbType.Float);
						cmd.Parameters.Add("@Buildings", SqlDbType.Int);
						cmd.Parameters.Add("@Building1SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@Building2SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@Building3SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@Building4SqFt", SqlDbType.Float);
						cmd.Parameters.Add("@TaxMapSection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapBlock", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapLot", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxTotalTaxes", SqlDbType.Float);
						cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
						cmd.Parameters.Add("@LandValue", SqlDbType.Float);
						cmd.Parameters.Add("@TotalValue", SqlDbType.Float);
						cmd.Parameters.Add("@Zoning", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@BrokerComments", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Golf", SqlDbType.NVarChar);
						cmd.Parameters.Add("@VirtualTour", SqlDbType.NVarChar, 255);

						string line;
						sr.ReadLine(); //Burn the header line
						while ((line = sr.ReadLine()) != null) {
							string[] fields = line.Split('\t');

							cmd.Parameters["@MLS"].Value = GetFloat(fields[159]);
							cmd.Parameters["@Class"].Value = "COMMERCIAL/INDUSTRIAL";
							cmd.Parameters["@Type"].Value = listings.TranslateType(fields[256]);
							cmd.Parameters["@Township"].Value = "";
							SqlDouble temp = GetFloat(fields[14]);
							cmd.Parameters["@AskingPrice"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[5]);
							cmd.Parameters["@AddressNumber"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AddressDirection"].Value = fields[4];
							cmd.Parameters["@AddressStreet"].Value = fields[7];
							cmd.Parameters["@Address2"].Value = fields[3];
							cmd.Parameters["@City"].Value = listings.TranslateCity(fields[27]);
							cmd.Parameters["@State"].Value = fields[239];
							cmd.Parameters["@Zip"].Value = GetFloat(fields[270]);
							cmd.Parameters["@Status"].Value = listings.TranslateStatus(fields[240]);
							cmd.Parameters["@SaleRent"].Value = fields[212];
							cmd.Parameters["@Construction"].Value = listings.TranslateConstruction(fields[32]);
							cmd.Parameters["@Stories"].Value = fields[1];
							cmd.Parameters["@GarageBay"].Value = GetInt(fields[53]);
							cmd.Parameters["@Electric"].Value = listings.TranslateElectricComm(fields[42]);
							cmd.Parameters["@Water"].Value = listings.TranslateWaterComm(fields[267]);
							cmd.Parameters["@Sewer"].Value = listings.TranslateSewerComm(fields[267]);
							cmd.Parameters["@ListingType"].Value = listings.TranslateListingType(fields[141]);
							temp = GetFloat(fields[161]);
							cmd.Parameters["@NumberOfAcres"].Value = temp > -1 ? temp : SqlDouble.Null;
							SqlInt32 temp2 = GetInt(fields[162]);
							cmd.Parameters["@NumberOfUnits"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							cmd.Parameters["@AgentID"].Value = GetFloat(fields[8]);
							cmd.Parameters["@AgentName"].Value = fields[74] + " " + fields[75];
							cmd.Parameters["@AgentPhone"].Value = fields[80];
							cmd.Parameters["@ListingOffice1ID"].Value = GetFloat(fields[140]);
							cmd.Parameters["@ListingOffice1Name"].Value = fields[150];
							cmd.Parameters["@ListingOffice1Phone"].Value = fields[151];
							cmd.Parameters["@Referral"].Value = fields[176];
							cmd.Parameters["@SchoolDistrict"].Value = fields[213];
							cmd.Parameters["@County"].Value = fields[34];
							cmd.Parameters["@Development"].Value = fields[36];
							temp = GetFloat(fields[13]);
							cmd.Parameters["@ApproxYearBuilt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[11]);
							cmd.Parameters["@ApproxTotalSquareFeet"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@WarehouseSqFtHeated"].Value = GetFloat(fields[265]);
							cmd.Parameters["@WarehouseSqFtUnheated"].Value = GetFloat(fields[266]);
							temp2 = GetInt(fields[0]);
							cmd.Parameters["@Buildings"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							temp = GetFloat(fields[21]);
							cmd.Parameters["@Building1SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[22]);
							cmd.Parameters["@Building2SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[23]);
							cmd.Parameters["@Building3SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[24]);
							cmd.Parameters["@Building4SqFt"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@TaxMapSection"].Value = fields[249];
							cmd.Parameters["@TaxMapBlock"].Value = fields[247];
							cmd.Parameters["@TaxMapLot"].Value = fields[248];
							cmd.Parameters["@ApproxTotalTaxes"].Value = GetFloat(fields[12]);
							cmd.Parameters["@TaxYear"].Value = GetInt(fields[251]);
							cmd.Parameters["@LandValue"].Value = GetFloat(fields[137]);
							cmd.Parameters["@TotalValue"].Value = GetFloat(fields[253]);
							cmd.Parameters["@Zoning"].Value = fields[271];
							cmd.Parameters["@BrokerComments"].Value = fields[20];
							cmd.Parameters["@Remarks"].Value = fields[177];
							cmd.Parameters["@Golf"].Value = "|" + fields[35].Replace(",", "|") + "|" + fields[45].Replace(",", "|") + "|" + fields[46].Replace(",", "|") + "|" + fields[51].Replace(",", "|") + "|" +
								fields[50].Replace(",", "|") + "|" + fields[43].Replace(",", "|") + "|" + fields[31].Replace(",", "|") + "|" + fields[160].Replace(",", "|") + "|" + fields[178].Replace(",", "|") + "|" + fields[179].Replace(",", "|") + "|" +
								fields[39].Replace(",", "|") + "|" + fields[52].Replace(",", "|") + "|" + fields[164].Replace(",", "|") + "|" + fields[173].Replace(",", "|") + "|" +
								fields[252].Replace(",", "|") + "|" + fields[66].Replace(",", "|") + "|" + fields[17].Replace(",", "|") + "|";
							cmd.Parameters["@VirtualTour"].Value = fields[260];
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}

                    using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-PriceHistory] ([MLS #], [Asking Price]) SELECT r.[MLS #], r.[Asking Price] FROM[listings-commercial] r LEFT JOIN[listings-PriceHistory] ph ON r.[MLS #] = ph.[MLS #] AND r.[Asking Price] = ph.[Asking Price] WHERE[Status] = 'Active' AND ph.Serial IS NULL", cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }

			BingGeocoder.UpdateGeoCoding();

			return View();

		}
		public ActionResult ImportSullivanLand2() {
			Server.ScriptTimeout = 300;

			//using (StreamReader sr = new StreamReader(Server.MapPath("/dl/residential.txt"))) {
			using (StreamReader sr = new StreamReader(@"E:\Sites\LazyMeadows\downloads\land.txt")) {
            //using (StreamReader sr = new StreamReader(@"C:\Projects\Clients\KKPR\CatskillFarms\lazy-meadow\lazymeadowsrealty.com\downloads\land.txt")) {
                Listings listings = new Listings();

				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-land]", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
					using (SqlCommand cmd = new SqlCommand("usp_InsertSullivanLand", cn)) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@MLS", SqlDbType.Float);
						cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Township", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AskingPrice", SqlDbType.Float);
						cmd.Parameters.Add("@AddressNumber", SqlDbType.Float);
						cmd.Parameters.Add("@AddressDirection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AddressStreet", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@City", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@State", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Zip", SqlDbType.Float);
						cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SaleRent", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Acreage", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Electric", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Water", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Sewer", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@NumberofAcres", SqlDbType.Float);
						cmd.Parameters.Add("@PricePerAcre", SqlDbType.Float);
						cmd.Parameters.Add("@AgentID", SqlDbType.Float);
						cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AgentPhone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1ID", SqlDbType.Float);
						cmd.Parameters.Add("@ListingOffice1Name", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1Phone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Referral", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@County", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Development", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapSection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapBlock", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxMapLot", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxLotSizeFront", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxLotSizeSide", SqlDbType.Float);
						cmd.Parameters.Add("@LandValue", SqlDbType.Float);
						cmd.Parameters.Add("@Parcels", SqlDbType.Int);
						cmd.Parameters.Add("@ApproxTotalTaxes", SqlDbType.Float);
						cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
						cmd.Parameters.Add("@Subdividable", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@TaxExemption", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@BrokerComments", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar);
						cmd.Parameters.Add("@Golf", SqlDbType.NVarChar);

						string line;
						sr.ReadLine(); //Burn the header line
						while ((line = sr.ReadLine()) != null) {
							string[] fields = line.Split('\t');

							cmd.Parameters["@MLS"].Value = GetFloat(fields[144]);
							cmd.Parameters["@Class"].Value ="LAND";
							cmd.Parameters["@Type"].Value = listings.TranslateType(fields[242]);
							cmd.Parameters["@Township"].Value = "";
							SqlDouble temp = GetFloat(fields[12]);
							cmd.Parameters["@AskingPrice"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[5]);
							cmd.Parameters["@AddressNumber"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AddressDirection"].Value = fields[4];
							cmd.Parameters["@AddressStreet"].Value = fields[7];
							cmd.Parameters["@Address2"].Value = fields[3];
							cmd.Parameters["@City"].Value = listings.TranslateCity(fields[20]);
							cmd.Parameters["@State"].Value = fields[224];
							cmd.Parameters["@Zip"].Value = GetFloat(fields[253]);
							cmd.Parameters["@Status"].Value = listings.TranslateStatus(fields[225]);
							cmd.Parameters["@SaleRent"].Value = fields[196];
							cmd.Parameters["@Acreage"].Value = fields[1];
							cmd.Parameters["@Electric"].Value = listings.TranslateElectric(fields[32]);
							cmd.Parameters["@Water"].Value = listings.TranslateWater(fields[251]);
							cmd.Parameters["@Sewer"].Value = listings.TranslateSewer(fields[203]);
							cmd.Parameters["@ListingType"].Value = listings.TranslateListingType(fields[126]);
							cmd.Parameters["@NumberofAcres"].Value = GetFloat(fields[146]);
							temp = GetFloat(fields[159]);
							cmd.Parameters["@PricePerAcre"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AgentID"].Value = GetFloat(fields[8]);
							cmd.Parameters["@AgentName"].Value = fields[56] + " " + fields[57];
							cmd.Parameters["@AgentPhone"].Value = fields[62];
							cmd.Parameters["@ListingOffice1ID"].Value = GetFloat(fields[125]);
							cmd.Parameters["@ListingOffice1Name"].Value = fields[135];
							cmd.Parameters["@ListingOffice1Phone"].Value = fields[136];
							cmd.Parameters["@Referral"].Value = fields[161];
							cmd.Parameters["@SchoolDistrict"].Value = fields[197];
							cmd.Parameters["@County"].Value = fields[25];
							cmd.Parameters["@Development"].Value = fields[26];
							cmd.Parameters["@TaxMapSection"].Value = fields[235];
							cmd.Parameters["@TaxMapBlock"].Value = fields[233];
							cmd.Parameters["@TaxMapLot"].Value = fields[234];
							temp = GetFloat(fields[9]);
							cmd.Parameters["@ApproxLotSizeFront"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[10]);
							cmd.Parameters["@ApproxLotSizeSide"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[122]);
							cmd.Parameters["@LandValue"].Value = temp > -1 ? temp : SqlDouble.Null;
							SqlInt32 temp2 = GetInt(fields[0]);
							cmd.Parameters["@Parcels"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							temp = GetFloat(fields[11]);
							cmd.Parameters["@ApproxTotalTaxes"].Value = temp > -1 ? temp : SqlInt32.Null;
							temp2 = GetInt(fields[237]);
							cmd.Parameters["@TaxYear"].Value = temp2 > -1 ? temp2 : SqlInt32.Null;
							cmd.Parameters["@Subdividable"].Value = fields[229];
							cmd.Parameters["@TaxExemption"].Value = fields[232];
							cmd.Parameters["@BrokerComments"].Value = fields[17];
							cmd.Parameters["@Remarks"].Value = fields[162];
							cmd.Parameters["@Golf"].Value = "|" + fields[119].Replace(",", "|") + "|" + fields[203].Replace(",", "|") + "|" + fields[120].Replace(",", "|") + "|" + fields[121].Replace(",", "|") + "|" +
								fields[29].Replace(",", "|") + "|" + fields[252].Replace(",", "|") + "|" + fields[204].Replace(",", "|") + "|" + fields[145].Replace(",", "|") + "|" + fields[163].Replace(",", "|") + "|" + fields[60].Replace(",", "|") + "|" +
								fields[157].Replace(",", "|") + "|" + fields[238].Replace(",", "|") + "|" + fields[148].Replace(",", "|") + "|";
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}

                    using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-PriceHistory] ([MLS #], [Asking Price]) SELECT r.[MLS #], r.[Asking Price] FROM[listings-land] r LEFT JOIN[listings-PriceHistory] ph ON r.[MLS #] = ph.[MLS #] AND r.[Asking Price] = ph.[Asking Price] WHERE[Status] = 'Active' AND ph.Serial IS NULL", cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }

			BingGeocoder.UpdateGeoCodingLand();
			return View();

		}
		public ActionResult ImportSullivanRes2() {
			Server.ScriptTimeout = 300;

            //using (StreamReader sr = new StreamReader(Server.MapPath("/dl/residential.txt"))) {

            //using(StreamReader sr = new StreamReader(@"C:\Projects\Clients\KKPR\CatskillFarms\lazy-meadow\lazymeadowsrealty.com\downloads\residential.txt")) {
            using (StreamReader sr = new StreamReader(@"E:\Sites\LazyMeadows\downloads\residential.txt")) {
                Listings listings = new Listings();

				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-residential]", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Connection.Open();
						cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}
					string SQL = "INSERT INTO [listings-residential]" +
							"	([MLS #]" +
							"	,[Class]" +
							"	,[Type]" +
							"	,[Township]" +
							"	,[Asking Price]" +
							"	,[Address Number]" +
							"	,[Address Direction]" +
							"	,[Address Street]" +
							"	,[Address 2]" +
							"	,[City]" +
							"	,[State]" +
							"	,[Zip]" +
							"	,[Status]" +
							"	,[Sale/Rent]" +
							"	,[# Rooms]" +
							"	,[# Bedrooms]" +
							"	,[# Full Baths]" +
							"	,[# Half Baths]" +
							"	,[Garage Type]" +
							"	,[Garage Capacity]" +
							"	,[Fireplace]" +
							"	,[Listing Type]" +
							"	,[Number of Acres]" +
							"	,[Agent ID]" +
							"	,[Agent Name]" +
							"	,[Agent Phone]" +
							"	,[Listing Office 1 ID]" +
							"	,[Listing Office 1 Name]" +
							"	,[Listing Office 1 Phone]" +
							"	,[County]" +
							"	,[School District]" +
							"	,[Development]" +
							"	,[Approx Year Built]" +
							"	,[Approx Lot Size-Front]" +
							"	,[Approx Lot Size-Side]" +
							"	,[Approx SqFt Above Grade]" +
							"	,[Approx SqFt Below Grade]" +
							"	,[Approx Total Square Feet]" +
							"	,[Approx Total Taxes]" +
							"	,[Tax Exemption Y/N]" +
							"	,[Land Value]" +
							"	,[Total Value]" +
							"	,[Possession Date]" +
							"	,[Maintenance Fees (Year)]" +
							"	,[Utilities (Month)]" +
							"	,[Homeowners Fees (Year)]" +
							"	,[Financing Terms]" +
							"	,[Remarks]" +
							"	,[A-Frame]" +
							"	,[Virtual Tour])" +
							"	VALUES" +
							"	(@MLS," +
							"	@Class," +
							"	@Type," +
							"	@Township," +
							"	@AskingPrice," +
							"	@AddressNumber," +
							"	@AddressDirection," +
							"	@AddressStreet," +
							"	@Address2," +
							"	@City," +
							"	@State," +
							"	@Zip," +
							"	@Status," +
							"	@SaleRent," +
							"	@Rooms," +
							"	@Bedrooms," +
							"	@FullBaths," +
							"	@HalfBaths," +
							"	@GarageType," +
							"	@GarageCapacity," +
							"	@Fireplace," +
							"	@ListingType," +
							"	@NumberofAcres," +
							"	@AgentID," +
							"	@AgentName," +
							"	@AgentPhone," +
							"	@ListingOffice1ID," +
							"	@ListingOffice1Name," +
							"	@ListingOffice1Phone," +
							"	@County," +
							"	@SchoolDistrict," +
							"	@Development," +
							"	@ApproxYearBuilt," +
							"	@ApproxLotSizeFront," +
							"	@ApproxLotSizeSide," +
							"	@ApproxSqFtAboveGrade," +
							"	@ApproxSqFtBelowGrade," +
							"	@ApproxTotalSquareFeet," +
							"	@ApproxTotalTaxes," +
							"	@TaxExemption," +
							"	@LandValue," +
							"	@TotalValue," +
							"	@PossessionDate," +
							"	@MaintenanceFees," +
							"	@Utilities," +
							"	@HomeownersFees," +
							"	@FinancingTerms," +
							"	@Remarks," +
							"	@AFrame," +
							"	@VirtualTour)";
					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@MLS", SqlDbType.Float);
						cmd.Parameters.Add("@Class", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Township", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AskingPrice", SqlDbType.Float);
						cmd.Parameters.Add("@AddressNumber", SqlDbType.Float);
						cmd.Parameters.Add("@AddressDirection", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AddressStreet", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@City", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@State", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Zip", SqlDbType.Float);
						cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SaleRent", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Rooms", SqlDbType.Float);
						cmd.Parameters.Add("@Bedrooms", SqlDbType.Float);
						cmd.Parameters.Add("@FullBaths", SqlDbType.Float);
						cmd.Parameters.Add("@HalfBaths", SqlDbType.Float);
						cmd.Parameters.Add("@GarageType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@GarageCapacity", SqlDbType.Float);
						cmd.Parameters.Add("@Fireplace", SqlDbType.Float);
						cmd.Parameters.Add("@ListingType", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@NumberofAcres", SqlDbType.Float);
						cmd.Parameters.Add("@AgentID", SqlDbType.Float);
						cmd.Parameters.Add("@AgentName", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@AgentPhone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1ID", SqlDbType.Float);
						cmd.Parameters.Add("@ListingOffice1Name", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ListingOffice1Phone", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@County", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Development", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@ApproxYearBuilt", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxLotSizeFront", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxLotSizeSide", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxSqFtAboveGrade", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxSqFtBelowGrade", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxTotalSquareFeet", SqlDbType.Float);
						cmd.Parameters.Add("@ApproxTotalTaxes", SqlDbType.Float);
						cmd.Parameters.Add("@TaxExemption", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@LandValue", SqlDbType.Float);
						cmd.Parameters.Add("@TotalValue", SqlDbType.Float);
						cmd.Parameters.Add("@PossessionDate", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@MaintenanceFees", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Utilities", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@HomeownersFees", SqlDbType.Float);
						cmd.Parameters.Add("@FinancingTerms", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar);
						cmd.Parameters.Add("@AFrame", SqlDbType.NVarChar, 255);
						cmd.Parameters.Add("@VirtualTour", SqlDbType.NVarChar, 255);

						string line;
						sr.ReadLine(); //Burn the header line
						while ((line = sr.ReadLine()) != null) {
							string[] fields = line.Split('\t');
							cmd.Parameters["@MLS"].Value = GetFloat(fields[189]);
							cmd.Parameters["@Class"].Value = "Residential";
							cmd.Parameters["@Type"].Value = listings.TranslateType(fields[287]);
							cmd.Parameters["@Township"].Value = ""; // fields[];
							SqlDouble temp = GetFloat(fields[21]);
							cmd.Parameters["@AskingPrice"].Value = temp > -1 ? temp : SqlDouble.Null;
							temp = GetFloat(fields[9]);
							cmd.Parameters["@AddressNumber"].Value = temp > -1 ? temp : SqlDouble.Null;
							cmd.Parameters["@AddressDirection"].Value = fields[8];
							cmd.Parameters["@AddressStreet"].Value = fields[11];
							cmd.Parameters["@Address2"].Value = fields[7];
							cmd.Parameters["@City"].Value = listings.TranslateCity(fields[40]);
							cmd.Parameters["@State"].Value = fields[269];
							cmd.Parameters["@Zip"].Value = GetFloat(fields[301]);
							cmd.Parameters["@Status"].Value = listings.TranslateStatus(fields[270]);
							cmd.Parameters["@SaleRent"].Value = fields[242];
							cmd.Parameters["@Rooms"].Value = GetFloat(fields[3]);
							cmd.Parameters["@Bedrooms"].Value = GetFloat(fields[0]);
							cmd.Parameters["@FullBaths"].Value = GetFloat(fields[1]);
							cmd.Parameters["@HalfBaths"].Value = GetFloat(fields[2]);
							cmd.Parameters["@GarageType"].Value = listings.TranslateGarageType(fields[68]);
							cmd.Parameters["@GarageCapacity"].Value = GetFloat(fields[67]);
							cmd.Parameters["@Fireplace"].Value = GetFloat(fields[64]);
							cmd.Parameters["@ListingType"].Value = ""; // fields[];
							cmd.Parameters["@NumberofAcres"].Value = GetFloat(fields[191]);
							cmd.Parameters["@AgentID"].Value = GetFloat(fields[12]);
							cmd.Parameters["@AgentName"].Value = fields[98] + " " + fields[99];
							cmd.Parameters["@AgentPhone"].Value = fields[104];
							cmd.Parameters["@ListingOffice1ID"].Value = GetFloat(fields[165]);
							cmd.Parameters["@ListingOffice1Name"].Value = fields[177];
							cmd.Parameters["@ListingOffice1Phone"].Value = fields[178];
							cmd.Parameters["@County"].Value = fields[47];
							cmd.Parameters["@SchoolDistrict"].Value = fields[243];
							cmd.Parameters["@Development"].Value = fields[50];
							cmd.Parameters["@ApproxYearBuilt"].Value = GetFloat(fields[20]);
							cmd.Parameters["@ApproxLotSizeFront"].Value = GetFloat(fields[14]);
							cmd.Parameters["@ApproxLotSizeSide"].Value = GetFloat(fields[15]);
							cmd.Parameters["@ApproxSqFtAboveGrade"].Value = GetFloat(fields[16]);
							cmd.Parameters["@ApproxSqFtBelowGrade"].Value = GetFloat(fields[17]);
							cmd.Parameters["@ApproxTotalSquareFeet"].Value = GetFloat(fields[18]);
							cmd.Parameters["@ApproxTotalTaxes"].Value = GetFloat(fields[19]);
							cmd.Parameters["@TaxExemption"].Value = fields[277];
							cmd.Parameters["@LandValue"].Value = GetFloat(fields[162]);
							cmd.Parameters["@TotalValue"].Value = GetFloat(fields[284]);
							cmd.Parameters["@PossessionDate"].Value = fields[201];
							cmd.Parameters["@MaintenanceFees"].Value = fields[186];
							cmd.Parameters["@Utilities"].Value = fields[289];
							cmd.Parameters["@HomeownersFees"].Value = GetFloat(fields[86]);
							cmd.Parameters["@FinancingTerms"].Value = fields[63];
							cmd.Parameters["@Remarks"].Value = fields[207];
							cmd.Parameters["@AFrame"].Value = "|" + fields[273].Replace(",", "|") + "|" + fields[203].Replace(",", "|") + "|" + fields[45].Replace(",", "|") + "|" + fields[59].Replace(",", "|") + "|" +
								fields[209].Replace(",", "|") + "|" + fields[13].Replace(",", "|") + "|" + fields[24].Replace(",", "|") + "|" + fields[43].Replace(",", "|") + "|" + fields[57].Replace(",", "|") + "|" + fields[60].Replace(",", "|") + "|" +
								fields[66].Replace(",", "|") + "|" + fields[91].Replace(",", "|") + "|" + fields[85].Replace(",", "|") + "|" + fields[161].Replace(",", "|") + "|" + fields[190].Replace(",", "|") + "|" + fields[193].Replace(",", "|") + "|" +
								fields[202].Replace(",", "|") + "|" + fields[208].Replace(",", "|") + "|" + fields[249].Replace(",", "|") + "|" + fields[283].Replace(",", "|") + "|" + fields[299].Replace(",", "|") + "|" + fields[300].Replace(",", "|") + "|";
							cmd.Parameters["@VirtualTour"].Value = fields[294];
							cmd.Connection.Open();
							cmd.ExecuteNonQuery();
							cmd.Connection.Close();
						}
					}

                    using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-PriceHistory] ([MLS #], [Asking Price]) SELECT r.[MLS #], r.[Asking Price] FROM[listings-residential] r LEFT JOIN[listings-PriceHistory] ph ON r.[MLS #] = ph.[MLS #] AND r.[Asking Price] = ph.[Asking Price] WHERE[Status] = 'Active' AND ph.Serial IS NULL", cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
            }

			BingGeocoder.UpdateGeoCoding();
			//BingGeolocation? bl = BingGeocoder.ResolveAddress("25 S Lewcy lane", "White Lake", "NY", "12786", "US");
			//if (bl != null) {
			//	string temp = "Lat: " + bl.Value.Lat.ToString() + " Lon:" + bl.Value.Lon;
			//}

			//SendConfirm();
			return View();

		}

        public ActionResult ImportSullivanComm3() {
            Server.ScriptTimeout = 300;

            //using (StreamReader sr = new StreamReader(Server.MapPath("/dl/residential.txt"))) {

            //using(StreamReader sr = new StreamReader(@"C:\Projects\Clients\KKPR\CatskillFarms\lazy-meadow\lazymeadowsrealty.com\downloads\commercial.txt")) {
            using(StreamReader sr = new StreamReader(@"E:\Sites\LazyMeadows\downloads\commercial.txt")) {
                Listings3 listings = new Listings3();

                using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                    using(SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-commercial-3]", cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                    string SQL = "INSERT INTO [dbo].[listings-commercial-3] " +
                    "           ([Access24Hour] " +
                    "           ,[AddFeeFrequency] " +
                    "           ,[AdditionalFeeDes] " +
                    "           ,[AdditionalFeesAmt] " +
                    "           ,[Adult55Community] " +
                    "           ,[AirConditioning] " +
                    "           ,[AlternateMLSNumber] " +
                    "           ,[Amenities] " +
                    "           ,[AtticDescription] " +
                    "           ,[BasementDescription] " +
                    "           ,[BathsFull] " +
                    "           ,[BathsHalf] " +
                    "           ,[BathsTotal] " +
                    "           ,[BedsTotal] " +
                    "           ,[City] " +
                    "           ,[CloseDate] " +
                    "           ,[ComplexName] " +
                    "           ,[ConstructionDescription] " +
                    "           ,[ContractDate] " +
                    "           ,[CountyOrParish] " +
                    "           ,[CurrentPrice] " +
                    "           ,[ElementarySchool] " +
                    "           ,[Fireplacesnumberof] " +
                    "           ,[FloorNumber] " +
                    "           ,[Garbage] " +
                    "           ,[GasAvailableYN] " +
                    "           ,[Hamlet] " +
                    "           ,[HeatingFuel] " +
                    "           ,[HeatingType] " +
                    "           ,[HeatingZonesNumof] " +
                    "           ,[HighSchool] " +
                    "           ,[HoaFeeIncludes] " +
                    "           ,[Hotwater] " +
                    "           ,[Included] " +
                    "           ,[Junior_MiddleHighSchool] " +
                    "           ,[LastLeasePrice] " +
                    "           ,[LeasePrice] " +
                    "           ,[LeaseTerm] " +
                    "           ,[ListAgentDirectWorkPhone] " +
                    "           ,[ListAgentEmail] " +
                    "           ,[ListAgentFullName] " +
                    "           ,[ListAgentMLSID] " +
                    "           ,[LotDescription] " +
                    "           ,[LotSizeArea] " +
                    "           ,[LotSizeAreaSQFT] " +
                    "           ,[MarketingRemarks] " +
                    "           ,[Matrix_Unique_ID] " +
                    "           ,[MLSNumber] " +
                    "           ,[MonthlyHOAFee] " +
                    "           ,[NumOfLevels] " +
                    "           ,[OpenHouseUpcoming] " +
                    "           ,[OriginalLeasePrice] " +
                    "           ,[Parking] " +
                    "           ,[PhotoCount] " +
                    "           ,[PostalCode] " +
                    "           ,[PostalCodePlus4] " +
                    "           ,[PropertySubType] " +
                    "           ,[PropertyType] " +
                    "           ,[PublicTransportation] " +
                    "           ,[REO_BankOwned] " +
                    "           ,[RoomCount] " +
                    "           ,[SchoolDistrict] " +
                    "           ,[SewerDescription] " +
                    "           ,[SidingDescription] " +
                    "           ,[SprinklerSystemYN] " +
                    "           ,[SqFtSource] " +
                    "           ,[SqFtTotal] " +
                    "           ,[StateOrProvince] " +
                    "           ,[Status] " +
                    "           ,[StreetDirPrefix] " +
                    "           ,[StreetDirSuffix] " +
                    "           ,[StreetName] " +
                    "           ,[StreetNumber] " +
                    "           ,[StreetNumberModifier] " +
                    "           ,[StreetSuffix] " +
                    "           ,[StreetType] " +
                    "           ,[Style] " +
                    "           ,[TaxAmount] " +
                    "           ,[TaxSource] " +
                    "           ,[TaxYear] " +
                    "           ,[TotalRoomsFinished] " +
                    "           ,[TransactionType] " +
                    "           ,[TypeOfDwelling] " +
                    "           ,[TypeOfUnit] " +
                    "           ,[UnitCount] " +
                    "           ,[UnitNumber] " +
                    "           ,[Village] " +
                    "           ,[VirtualTourLink] " +
                    "           ,[WaterAccessYN] " +
                    "           ,[WaterDescription] " +
                    "           ,[YearBuilt] " +
                    "           ,[YearBuiltException] " +
                    "           ,[YearRenovated] " +
                    "           ,[Zoning]) " +
                    "     VALUES " +
                    "           (@Access24Hour " +
                    "           ,@AddFeeFrequency " +
                    "           ,@AdditionalFeeDes " +
                    "           ,@AdditionalFeesAmt " +
                    "           ,@Adult55Community " +
                    "           ,@AirConditioning " +
                    "           ,@AlternateMLSNumber " +
                    "           ,@Amenities " +
                    "           ,@AtticDescription " +
                    "           ,@BasementDescription " +
                    "           ,@BathsFull " +
                    "           ,@BathsHalf " +
                    "           ,@BathsTotal " +
                    "           ,@BedsTotal " +
                    "           ,@City " +
                    "           ,@CloseDate " +
                    "           ,@ComplexName " +
                    "           ,@ConstructionDescription " +
                    "           ,@ContractDate " +
                    "           ,@CountyOrParish " +
                    "           ,@CurrentPrice " +
                    "           ,@ElementarySchool " +
                    "           ,@Fireplacesnumberof " +
                    "           ,@FloorNumber " +
                    "           ,@Garbage " +
                    "           ,@GasAvailableYN " +
                    "           ,@Hamlet " +
                    "           ,@HeatingFuel " +
                    "           ,@HeatingType " +
                    "           ,@HeatingZonesNumof " +
                    "           ,@HighSchool " +
                    "           ,@HoaFeeIncludes " +
                    "           ,@Hotwater " +
                    "           ,@Included " +
                    "           ,@Junior_MiddleHighSchool " +
                    "           ,@LastLeasePrice " +
                    "           ,@LeasePrice " +
                    "           ,@LeaseTerm " +
                    "           ,@ListAgentDirectWorkPhone " +
                    "           ,@ListAgentEmail " +
                    "           ,@ListAgentFullName " +
                    "           ,@ListAgentMLSID " +
                    "           ,@LotDescription " +
                    "           ,@LotSizeArea " +
                    "           ,@LotSizeAreaSQFT " +
                    "           ,@MarketingRemarks " +
                    "           ,@Matrix_Unique_ID " +
                    "           ,@MLSNumber " +
                    "           ,@MonthlyHOAFee " +
                    "           ,@NumOfLevels " +
                    "           ,@OpenHouseUpcoming " +
                    "           ,@OriginalLeasePrice " +
                    "           ,@Parking " +
                    "           ,@PhotoCount " +
                    "           ,@PostalCode " +
                    "           ,@PostalCodePlus4 " +
                    "           ,@PropertySubType " +
                    "           ,@PropertyType " +
                    "           ,@PublicTransportation " +
                    "           ,@REO_BankOwned " +
                    "           ,@RoomCount " +
                    "           ,@SchoolDistrict " +
                    "           ,@SewerDescription " +
                    "           ,@SidingDescription " +
                    "           ,@SprinklerSystemYN " +
                    "           ,@SqFtSource " +
                    "           ,@SqFtTotal " +
                    "           ,@StateOrProvince " +
                    "           ,@Status " +
                    "           ,@StreetDirPrefix " +
                    "           ,@StreetDirSuffix " +
                    "           ,@StreetName " +
                    "           ,@StreetNumber " +
                    "           ,@StreetNumberModifier " +
                    "           ,@StreetSuffix " +
                    "           ,@StreetType " +
                    "           ,@Style " +
                    "           ,@TaxAmount " +
                    "           ,@TaxSource " +
                    "           ,@TaxYear " +
                    "           ,@TotalRoomsFinished " +
                    "           ,@TransactionType " +
                    "           ,@TypeOfDwelling " +
                    "           ,@TypeOfUnit " +
                    "           ,@UnitCount " +
                    "           ,@UnitNumber " +
                    "           ,@Village " +
                    "           ,@VirtualTourLink " +
                    "           ,@WaterAccessYN " +
                    "           ,@WaterDescription " +
                    "           ,@YearBuilt " +
                    "           ,@YearBuiltException " +
                    "           ,@YearRenovated " +
                    "           ,@Zoning)";
                    using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@Access24Hour", SqlDbType.Bit);
                        cmd.Parameters.Add("@AddFeeFrequency", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@AdditionalFeeDes", SqlDbType.NVarChar, 40);
                        cmd.Parameters.Add("@AdditionalFeesAmt", SqlDbType.Float);
                        cmd.Parameters.Add("@Adult55Community", SqlDbType.Bit);
                        cmd.Parameters.Add("@AirConditioning", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@AlternateMLSNumber", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@Amenities", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@AtticDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@BasementDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@BathsFull", SqlDbType.Int);
                        cmd.Parameters.Add("@BathsHalf", SqlDbType.Float);
                        cmd.Parameters.Add("@BathsTotal", SqlDbType.Float);
                        cmd.Parameters.Add("@BedsTotal", SqlDbType.Int);
                        cmd.Parameters.Add("@City", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@CloseDate", SqlDbType.DateTime);
                        cmd.Parameters.Add("@ComplexName", SqlDbType.NVarChar, 30);
                        cmd.Parameters.Add("@ConstructionDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@ContractDate", SqlDbType.DateTime);
                        cmd.Parameters.Add("@CountyOrParish", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@CurrentPrice", SqlDbType.Float);
                        cmd.Parameters.Add("@ElementarySchool", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Fireplacesnumberof", SqlDbType.Int);
                        cmd.Parameters.Add("@FloorNumber", SqlDbType.Int);
                        cmd.Parameters.Add("@Garbage", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@GasAvailableYN", SqlDbType.Bit);
                        cmd.Parameters.Add("@Hamlet", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@HeatingFuel", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@HeatingType", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@HeatingZonesNumof", SqlDbType.Int);
                        cmd.Parameters.Add("@HighSchool", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@HoaFeeIncludes", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Hotwater", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Included", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Junior_MiddleHighSchool", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@LastLeasePrice", SqlDbType.Float);
                        cmd.Parameters.Add("@LeasePrice", SqlDbType.Float);
                        cmd.Parameters.Add("@LeaseTerm", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@ListAgentDirectWorkPhone", SqlDbType.NVarChar, 50);
                        cmd.Parameters.Add("@ListAgentEmail", SqlDbType.NVarChar, 80);
                        cmd.Parameters.Add("@ListAgentFullName", SqlDbType.NVarChar, 150);
                        cmd.Parameters.Add("@ListAgentMLSID", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@LotDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@LotSizeArea", SqlDbType.Float);
                        cmd.Parameters.Add("@LotSizeAreaSQFT", SqlDbType.Int);
                        cmd.Parameters.Add("@MarketingRemarks", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Matrix_Unique_ID", SqlDbType.NVarChar, 19);
                        cmd.Parameters.Add("@MLSNumber", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@MonthlyHOAFee", SqlDbType.Int);
                        cmd.Parameters.Add("@NumOfLevels", SqlDbType.Float);
                        cmd.Parameters.Add("@OpenHouseUpcoming", SqlDbType.NVarChar, 255);
                        cmd.Parameters.Add("@OriginalLeasePrice", SqlDbType.Float);
                        cmd.Parameters.Add("@Parking", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@PhotoCount", SqlDbType.Int);
                        cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 10);
                        cmd.Parameters.Add("@PostalCodePlus4", SqlDbType.NVarChar, 4);
                        cmd.Parameters.Add("@PropertySubType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@PropertyType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@PublicTransportation", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@REO_BankOwned", SqlDbType.Bit);
                        cmd.Parameters.Add("@RoomCount", SqlDbType.Int);
                        cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@SewerDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@SidingDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@SprinklerSystemYN", SqlDbType.Bit);
                        cmd.Parameters.Add("@SqFtSource", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@SqFtTotal", SqlDbType.Int);
                        cmd.Parameters.Add("@StateOrProvince", SqlDbType.NVarChar, 2);
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetDirPrefix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetDirSuffix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetName", SqlDbType.NVarChar, 50);
                        cmd.Parameters.Add("@StreetNumber", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@StreetNumberModifier", SqlDbType.NVarChar, 8);
                        cmd.Parameters.Add("@StreetSuffix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@Style", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@TaxAmount", SqlDbType.Int);
                        cmd.Parameters.Add("@TaxSource", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
                        cmd.Parameters.Add("@TotalRoomsFinished", SqlDbType.Int);
                        cmd.Parameters.Add("@TransactionType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@TypeOfDwelling", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@TypeOfUnit", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@UnitCount", SqlDbType.Float);
                        cmd.Parameters.Add("@UnitNumber", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@Village", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@VirtualTourLink", SqlDbType.NVarChar, 200);
                        cmd.Parameters.Add("@WaterAccessYN", SqlDbType.Bit);
                        cmd.Parameters.Add("@WaterDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@YearBuilt", SqlDbType.Float);
                        cmd.Parameters.Add("@YearBuiltException", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@YearRenovated", SqlDbType.Int);
                        cmd.Parameters.Add("@Zoning", SqlDbType.NVarChar, 255);

                        string line;
                        sr.ReadLine(); //Burn the header line
                        while((line = sr.ReadLine()) != null) {
                            string[] fields = line.Split('\t');
                            cmd.Parameters["@Access24Hour"].Value = GetBit(fields[0]);
                            cmd.Parameters["@AddFeeFrequency"].Value = listings.TranslateAddFeeFrequency(fields[1]);
                            cmd.Parameters["@AdditionalFeeDes"].Value = fields[2];
                            cmd.Parameters["@AdditionalFeesAmt"].Value = GetFloat(fields[3]);
                            cmd.Parameters["@Adult55Community"].Value = GetBit(fields[4]);
                            cmd.Parameters["@AirConditioning"].Value = listings.TranslateAirCond(fields[5]);
                            cmd.Parameters["@AlternateMLSNumber"].Value = fields[6];
                            cmd.Parameters["@Amenities"].Value = listings.TranslateAmenities(fields[7]);
                            cmd.Parameters["@AtticDescription"].Value = listings.TranslateAttic(fields[8]);
                            cmd.Parameters["@BasementDescription"].Value = listings.TranslateBasement(fields[9]);
                            cmd.Parameters["@BathsFull"].Value = GetInt(fields[10]);
                            cmd.Parameters["@BathsHalf"].Value = GetFloat(fields[11]);
                            cmd.Parameters["@BathsTotal"].Value = GetFloat(fields[12]);
                            cmd.Parameters["@BedsTotal"].Value = GetInt(fields[13]);
                            cmd.Parameters["@City"].Value = listings.TranslateCity3(fields[14], fields[78]);
                            DateTime temp = GetDatetime(fields[15]);
                            cmd.Parameters["@CloseDate"].Value = temp != DateTime.MinValue ? temp : SqlDateTime.Null;
                            cmd.Parameters["@ComplexName"].Value = fields[26];
                            cmd.Parameters["@ConstructionDescription"].Value = listings.TranslateConstructionDesc(fields[27]);
                            DateTime temp2 = GetDatetime(fields[28]);
                            cmd.Parameters["@ContractDate"].Value = temp2 != DateTime.MinValue ? temp2 : SqlDateTime.Null; ;
                            cmd.Parameters["@CountyOrParish"].Value = fields[29];
                            cmd.Parameters["@CurrentPrice"].Value = GetFloat(fields[30]);
                            cmd.Parameters["@ElementarySchool"].Value = fields[31];
                            cmd.Parameters["@Fireplacesnumberof"].Value = GetInt(fields[32]);
                            cmd.Parameters["@FloorNumber"].Value = GetInt(fields[33]);
                            cmd.Parameters["@Garbage"].Value = listings.TranslateGarbage(fields[34]);
                            cmd.Parameters["@GasAvailableYN"].Value = GetBit(fields[35]);
                            cmd.Parameters["@Hamlet"].Value = fields[36];
                            cmd.Parameters["@HeatingFuel"].Value = listings.TranslateHeatingFuel(fields[37]);
                            cmd.Parameters["@HeatingType"].Value = listings.TranslateHeatingType(fields[38]);
                            cmd.Parameters["@HeatingZonesNumof"].Value = GetInt(fields[39]);
                            cmd.Parameters["@HighSchool"].Value = fields[40];
                            cmd.Parameters["@HoaFeeIncludes"].Value = listings.TranslateHOAFee(fields[41]);
                            cmd.Parameters["@Hotwater"].Value = listings.TranslateHotWater(fields[42]);
                            cmd.Parameters["@Included"].Value = listings.TranslateIncluded(fields[45]);
                            cmd.Parameters["@Junior_MiddleHighSchool"].Value = fields[46];
                            cmd.Parameters["@LastLeasePrice"].Value = GetFloat(fields[49]);
                            cmd.Parameters["@LeasePrice"].Value = GetFloat(fields[50]);
                            cmd.Parameters["@LeaseTerm"].Value = listings.TranslateLeaseTerm(fields[51]);
                            cmd.Parameters["@ListAgentDirectWorkPhone"].Value = fields[52];
                            cmd.Parameters["@ListAgentEmail"].Value = fields[53];
                            cmd.Parameters["@ListAgentFullName"].Value = fields[57];
                            cmd.Parameters["@ListAgentMLSID"].Value = fields[55];
                            cmd.Parameters["@LotDescription"].Value = listings.TranslateLotDesc(fields[59]);
                            cmd.Parameters["@LotSizeArea"].Value = GetFloat(fields[60]);
                            cmd.Parameters["@LotSizeAreaSQFT"].Value = GetInt(fields[61]);
                            string t = fields[64];
                            if(t.StartsWith("\"")) { t = t.Substring(1); }
                            if(t.EndsWith("\"")) { t = t.Substring(0, t.Length - 1); }
                            cmd.Parameters["@MarketingRemarks"].Value = t;
                            cmd.Parameters["@Matrix_Unique_ID"].Value = fields[66];
                            cmd.Parameters["@MLSNumber"].Value = fields[67];
                            cmd.Parameters["@MonthlyHOAFee"].Value = GetInt(fields[68]);
                            cmd.Parameters["@NumOfLevels"].Value = GetFloat(fields[69]);
                            cmd.Parameters["@OpenHouseUpcoming"].Value = fields[71];
                            cmd.Parameters["@OriginalLeasePrice"].Value = GetFloat(fields[72]);
                            cmd.Parameters["@Parking"].Value = listings.TranslateParking(fields[75]);
                            cmd.Parameters["@PhotoCount"].Value = GetInt(fields[76]);
                            cmd.Parameters["@PostalCode"].Value = fields[79];
                            cmd.Parameters["@PostalCodePlus4"].Value = fields[80];
                            cmd.Parameters["@PropertySubType"].Value = listings.TranslatePropertySubType(fields[81]);
                            cmd.Parameters["@PropertyType"].Value = listings.TranslatePropertyType(fields[82]);
                            cmd.Parameters["@PublicTransportation"].Value = fields[83];
                            cmd.Parameters["@REO_BankOwned"].Value = GetBit(fields[84]);
                            cmd.Parameters["@RoomCount"].Value = GetInt(fields[85]);
                            cmd.Parameters["@SchoolDistrict"].Value = fields[86];
                            cmd.Parameters["@SewerDescription"].Value = listings.TranslateSewerDesc(fields[89]);
                            cmd.Parameters["@SidingDescription"].Value = listings.TranslateSiding(fields[90]);
                            cmd.Parameters["@SprinklerSystemYN"].Value = GetBit(fields[93]);
                            cmd.Parameters["@SqFtSource"].Value = fields[94];
                            cmd.Parameters["@SqFtTotal"].Value = GetInt(fields[95]);
                            cmd.Parameters["@StateOrProvince"].Value = fields[96];
                            cmd.Parameters["@Status"].Value = fields[97];
                            cmd.Parameters["@StreetDirPrefix"].Value = fields[99];
                            cmd.Parameters["@StreetDirSuffix"].Value = fields[100];
                            cmd.Parameters["@StreetName"].Value = fields[101];
                            cmd.Parameters["@StreetNumber"].Value = fields[102];
                            cmd.Parameters["@StreetNumberModifier"].Value = fields[103];
                            cmd.Parameters["@StreetSuffix"].Value = fields[104];
                            cmd.Parameters["@StreetType"].Value = listings.TranslateStreetType(fields[105]);
                            cmd.Parameters["@Style"].Value = listings.TranslateStyle(fields[106]);
                            cmd.Parameters["@TaxAmount"].Value = GetInt(fields[107]);
                            cmd.Parameters["@TaxSource"].Value = fields[108];
                            cmd.Parameters["@TaxYear"].Value = GetInt(fields[109]);
                            cmd.Parameters["@TotalRoomsFinished"].Value = GetInt(fields[110]);
                            cmd.Parameters["@TransactionType"].Value = listings.TranslateTransactionType(fields[111]);
                            cmd.Parameters["@TypeOfDwelling"].Value = fields[112];
                            cmd.Parameters["@TypeOfUnit"].Value = fields[113];
                            cmd.Parameters["@UnitCount"].Value = GetFloat(fields[114]);
                            cmd.Parameters["@UnitNumber"].Value = fields[115];
                            cmd.Parameters["@Village"].Value = fields[116];
                            cmd.Parameters["@VirtualTourLink"].Value = fields[117];
                            cmd.Parameters["@WaterAccessYN"].Value = GetBit(fields[118]);
                            cmd.Parameters["@WaterDescription"].Value = listings.TranslateWaterDesc(fields[119]);
                            cmd.Parameters["@YearBuilt"].Value = GetFloat(fields[120]);
                            cmd.Parameters["@YearBuiltException"].Value = fields[121];
                            cmd.Parameters["@YearRenovated"].Value = GetInt(fields[122]);
                            cmd.Parameters["@Zoning"].Value = fields[123];

                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }
                    }

                    //using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-PriceHistory] ([MLS #], [Asking Price]) SELECT r.[MLS #], r.[Asking Price] FROM[listings-residential] r LEFT JOIN[listings-PriceHistory] ph ON r.[MLS #] = ph.[MLS #] AND r.[Asking Price] = ph.[Asking Price] WHERE[Status] = 'Active' AND ph.Serial IS NULL", cn)) {
                    //    cmd.CommandType = CommandType.Text;
                    //    cmd.Connection.Open();
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Connection.Close();
                    //}
                }
            }

            BingGeocoder.UpdateGeoCodingComm3();

            return View();

        }
        public ActionResult ImportSullivanLand3() {
            Server.ScriptTimeout = 300;

            //using (StreamReader sr = new StreamReader(Server.MapPath("/dl/residential.txt"))) {

            //using(StreamReader sr = new StreamReader(@"C:\Projects\Clients\KKPR\CatskillFarms\lazy-meadow\lazymeadowsrealty.com\downloads\land.txt")) {
            using(StreamReader sr = new StreamReader(@"E:\Sites\LazyMeadows\downloads\land.txt")) {
                Listings3 listings = new Listings3();

                using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                    using(SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-land-3]", cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                    string SQL = "INSERT INTO [dbo].[listings-land-3] " +
                    "           ([AddFeeFrequency] " +
                    "           ,[AdditionalFeeDes] " +
                    "           ,[AdditionalFeesAmt] " +
                    "           ,[Adult55Community] " +
                    "           ,[AlternateMLSNumber] " +
                    "           ,[AtticDescription] " +
                    "           ,[AvailableUtilities] " +
                    "           ,[BathsFull] " +
                    "           ,[BathsHalf] " +
                    "           ,[BathsTotal] " +
                    "           ,[BedsTotal] " +
                    "           ,[City] " +
                    "           ,[CloseDate] " +
                    "           ,[ContractDate] " +
                    "           ,[CountyOrParish] " +
                    "           ,[CurrentPrice] " +
                    "           ,[ElementarySchool] " +
                    "           ,[Fireplacesnumberof] " +
                    "           ,[Garbage] " +
                    "           ,[GasAvailableYN] " +
                    "           ,[Hamlet] " +
                    "           ,[HighSchool] " +
                    "           ,[Hotwater] " +
                    "           ,[Junior_MiddleHighSchool] " +
                    "           ,[LeasePrice] " +
                    "           ,[ListAgentDirectWorkPhone] " +
                    "           ,[ListAgentEmail] " +
                    "           ,[ListAgentFullName] " +
                    "           ,[ListAgentMLSID] " +
                    "           ,[LotDescription] " +
                    "           ,[LotSizeArea] " +
                    "           ,[LotSizeAreaSQFT] " +
                    "           ,[MarketingRemarks] " +
                    "           ,[Matrix_Unique_ID] " +
                    "           ,[MLSNumber] " +
                    "           ,[OpenHouseUpcoming] " +
                    "           ,[PhotoCount] " +
                    "           ,[PostalCode] " +
                    "           ,[PostalCodePlus4] " +
                    "           ,[PropertyType] " +
                    "           ,[PUD] " +
                    "           ,[REO_BankOwned] " +
                    "           ,[RoadFrontDescription] " +
                    "           ,[RoomCount] " +
                    "           ,[SchoolDistrict] " +
                    "           ,[SewerDescription] " +
                    "           ,[SoilType] " +
                    "           ,[SqFtSource] " +
                    "           ,[SqFtTotal] " +
                    "           ,[StateOrProvince] " +
                    "           ,[Status] " +
                    "           ,[StreetDirPrefix] " +
                    "           ,[StreetDirSuffix] " +
                    "           ,[StreetName] " +
                    "           ,[StreetNumber] " +
                    "           ,[StreetNumberModifier] " +
                    "           ,[StreetSuffix] " +
                    "           ,[StreetType] " +
                    "           ,[Style] " +
                    "           ,[Subdivision_Development] " +
                    "           ,[TaxAmount] " +
                    "           ,[TaxSource] " +
                    "           ,[TaxYear] " +
                    "           ,[Topography] " +
                    "           ,[TotalRoomsFinished] " +
                    "           ,[TypeofDwelling] " +
                    "           ,[TypeofUnit] " +
                    "           ,[UnitCount] " +
                    "           ,[UnitNumber] " +
                    "           ,[UtilitiesOn_AbuttingSite] " +
                    "           ,[Village] " +
                    "           ,[VirtualTourLink] " +
                    "           ,[WaterAccessYN] " +
                    "           ,[WaterDescription] " +
                    "           ,[YearBuilt] " +
                    "           ,[YearBuiltException] " +
                    "           ,[YearRenovated] " +
                    "           ,[Zoning]) " +
                    "     VALUES " +
                    "           (@AddFeeFrequency " +
                    "           ,@AdditionalFeeDes " +
                    "           ,@AdditionalFeesAmt " +
                    "           ,@Adult55Community " +
                    "           ,@AlternateMLSNumber " +
                    "           ,@AtticDescription " +
                    "           ,@AvailableUtilities " +
                    "           ,@BathsFull " +
                    "           ,@BathsHalf " +
                    "           ,@BathsTotal " +
                    "           ,@BedsTotal " +
                    "           ,@City " +
                    "           ,@CloseDate " +
                    "           ,@ContractDate " +
                    "           ,@CountyOrParish " +
                    "           ,@CurrentPrice " +
                    "           ,@ElementarySchool " +
                    "           ,@Fireplacesnumberof " +
                    "           ,@Garbage " +
                    "           ,@GasAvailableYN " +
                    "           ,@Hamlet " +
                    "           ,@HighSchool " +
                    "           ,@Hotwater " +
                    "           ,@Junior_MiddleHighSchool " +
                    "           ,@LeasePrice " +
                    "           ,@ListAgentDirectWorkPhone " +
                    "           ,@ListAgentEmail " +
                    "           ,@ListAgentFullName " +
                    "           ,@ListAgentMLSID " +
                    "           ,@LotDescription " +
                    "           ,@LotSizeArea " +
                    "           ,@LotSizeAreaSQFT " +
                    "           ,@MarketingRemarks " +
                    "           ,@Matrix_Unique_ID " +
                    "           ,@MLSNumber " +
                    "           ,@OpenHouseUpcoming " +
                    "           ,@PhotoCount " +
                    "           ,@PostalCode " +
                    "           ,@PostalCodePlus4 " +
                    "           ,@PropertyType " +
                    "           ,@PUD " +
                    "           ,@REO_BankOwned " +
                    "           ,@RoadFrontDescription " +
                    "           ,@RoomCount " +
                    "           ,@SchoolDistrict " +
                    "           ,@SewerDescription " +
                    "           ,@SoilType " +
                    "           ,@SqFtSource " +
                    "           ,@SqFtTotal " +
                    "           ,@StateOrProvince " +
                    "           ,@Status " +
                    "           ,@StreetDirPrefix " +
                    "           ,@StreetDirSuffix " +
                    "           ,@StreetName " +
                    "           ,@StreetNumber " +
                    "           ,@StreetNumberModifier " +
                    "           ,@StreetSuffix " +
                    "           ,@StreetType " +
                    "           ,@Style " +
                    "           ,@Subdivision_Development " +
                    "           ,@TaxAmount " +
                    "           ,@TaxSource " +
                    "           ,@TaxYear " +
                    "           ,@Topography " +
                    "           ,@TotalRoomsFinished " +
                    "           ,@TypeofDwelling " +
                    "           ,@TypeofUnit " +
                    "           ,@UnitCount " +
                    "           ,@UnitNumber " +
                    "           ,@UtilitiesOn_AbuttingSite " +
                    "           ,@Village " +
                    "           ,@VirtualTourLink " +
                    "           ,@WaterAccessYN " +
                    "           ,@WaterDescription " +
                    "           ,@YearBuilt " +
                    "           ,@YearBuiltException " +
                    "           ,@YearRenovated " +
                    "           ,@Zoning) ";
                    using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@AddFeeFrequency", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@AdditionalFeeDes", SqlDbType.NVarChar, 40);
                        cmd.Parameters.Add("@AdditionalFeesAmt", SqlDbType.Float);
                        cmd.Parameters.Add("@Adult55Community", SqlDbType.Bit);
                        cmd.Parameters.Add("@AlternateMLSNumber", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@AtticDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@AvailableUtilities", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@BathsFull", SqlDbType.Int);
                        cmd.Parameters.Add("@BathsHalf", SqlDbType.Float);
                        cmd.Parameters.Add("@BathsTotal", SqlDbType.Float);
                        cmd.Parameters.Add("@BedsTotal", SqlDbType.Int);
                        cmd.Parameters.Add("@City", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@CloseDate", SqlDbType.DateTime);
                        cmd.Parameters.Add("@ContractDate", SqlDbType.DateTime);
                        cmd.Parameters.Add("@CountyOrParish", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@CurrentPrice", SqlDbType.Float);
                        cmd.Parameters.Add("@ElementarySchool", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Fireplacesnumberof", SqlDbType.Int);
                        cmd.Parameters.Add("@Garbage", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@GasAvailableYN", SqlDbType.Bit);
                        cmd.Parameters.Add("@Hamlet", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@HighSchool", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@Hotwater", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Junior_MiddleHighSchool", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@LeasePrice", SqlDbType.Float);
                        cmd.Parameters.Add("@ListAgentDirectWorkPhone", SqlDbType.NVarChar, 50);
                        cmd.Parameters.Add("@ListAgentEmail", SqlDbType.NVarChar, 80);
                        cmd.Parameters.Add("@ListAgentFullName", SqlDbType.NVarChar, 150);
                        cmd.Parameters.Add("@ListAgentMLSID", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@LotDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@LotSizeArea", SqlDbType.Float);
                        cmd.Parameters.Add("@LotSizeAreaSQFT", SqlDbType.Int);
                        cmd.Parameters.Add("@MarketingRemarks", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Matrix_Unique_ID", SqlDbType.NVarChar, 19);
                        cmd.Parameters.Add("@MLSNumber", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@OpenHouseUpcoming", SqlDbType.NVarChar, 255);
                        cmd.Parameters.Add("@PhotoCount", SqlDbType.Int);
                        cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 10);
                        cmd.Parameters.Add("@PostalCodePlus4", SqlDbType.NVarChar, 4);
                        cmd.Parameters.Add("@PropertyType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@PUD", SqlDbType.Bit);
                        cmd.Parameters.Add("@REO_BankOwned", SqlDbType.Bit);
                        cmd.Parameters.Add("@RoadFrontDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@RoomCount", SqlDbType.Int);
                        cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@SewerDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@SoilType", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@SqFtSource", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@SqFtTotal", SqlDbType.Int);
                        cmd.Parameters.Add("@StateOrProvince", SqlDbType.NVarChar, 2);
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetDirPrefix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetDirSuffix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetName", SqlDbType.NVarChar, 50);
                        cmd.Parameters.Add("@StreetNumber", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@StreetNumberModifier", SqlDbType.NVarChar, 8);
                        cmd.Parameters.Add("@StreetSuffix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@Style", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Subdivision_Development", SqlDbType.NVarChar, 30);
                        cmd.Parameters.Add("@TaxAmount", SqlDbType.Int);
                        cmd.Parameters.Add("@TaxSource", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
                        cmd.Parameters.Add("@Topography", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@TotalRoomsFinished", SqlDbType.Int);
                        cmd.Parameters.Add("@TypeofDwelling", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@TypeofUnit", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@UnitCount", SqlDbType.Float);
                        cmd.Parameters.Add("@UnitNumber", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@UtilitiesOn_AbuttingSite", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Village", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@VirtualTourLink", SqlDbType.NVarChar, 200);
                        cmd.Parameters.Add("@WaterAccessYN", SqlDbType.Bit);
                        cmd.Parameters.Add("@WaterDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@YearBuilt", SqlDbType.Float);
                        cmd.Parameters.Add("@YearBuiltException", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@YearRenovated", SqlDbType.Int);
                        cmd.Parameters.Add("@Zoning", SqlDbType.NVarChar, 255);

                        string line;
                        sr.ReadLine(); //Burn the header line
                        while((line = sr.ReadLine()) != null) {
                            string[] fields = line.Split('\t');
                            cmd.Parameters["@AddFeeFrequency"].Value = listings.TranslateAddFeeFrequency(fields[0]);
                            cmd.Parameters["@AdditionalFeeDes"].Value = fields[1];
                            cmd.Parameters["@AdditionalFeesAmt"].Value = GetFloat(fields[2]);
                            cmd.Parameters["@Adult55Community"].Value = GetBit(fields[3]);
                            cmd.Parameters["@AlternateMLSNumber"].Value = fields[4];
                            cmd.Parameters["@AtticDescription"].Value = listings.TranslateAttic(fields[5]);
                            cmd.Parameters["@AvailableUtilities"].Value = listings.TranslateAvailableUtilities(fields[6]);
                            cmd.Parameters["@BathsFull"].Value = GetInt(fields[7]);
                            cmd.Parameters["@BathsHalf"].Value = GetFloat(fields[8]);
                            cmd.Parameters["@BathsTotal"].Value = GetFloat(fields[9]);
                            cmd.Parameters["@BedsTotal"].Value = GetInt(fields[10]);
                            cmd.Parameters["@City"].Value = listings.TranslateCity3(fields[11], fields[61]);
                            DateTime temp = GetDatetime(fields[12]);
                            cmd.Parameters["@CloseDate"].Value = temp != DateTime.MinValue ? temp : SqlDateTime.Null;
                            DateTime temp2 = GetDatetime(fields[23]);
                            cmd.Parameters["@ContractDate"].Value = temp2 != DateTime.MinValue ? temp2 : SqlDateTime.Null; ;
                            cmd.Parameters["@CountyOrParish"].Value = fields[24];
                            cmd.Parameters["@CurrentPrice"].Value = GetFloat(fields[25]);
                            cmd.Parameters["@ElementarySchool"].Value = fields[26];
                            cmd.Parameters["@Fireplacesnumberof"].Value = GetInt(fields[27]);
                            cmd.Parameters["@Garbage"].Value = listings.TranslateGarbage(fields[28]);
                            cmd.Parameters["@GasAvailableYN"].Value = GetBit(fields[29]);
                            cmd.Parameters["@Hamlet"].Value = fields[30];
                            cmd.Parameters["@HighSchool"].Value = fields[31];
                            cmd.Parameters["@Hotwater"].Value = listings.TranslateHotWater(fields[32]);
                            cmd.Parameters["@Junior_MiddleHighSchool"].Value = fields[35];
                            cmd.Parameters["@LeasePrice"].Value = GetFloat(fields[38]);
                            cmd.Parameters["@ListAgentDirectWorkPhone"].Value = fields[39];
                            cmd.Parameters["@ListAgentEmail"].Value = fields[40];
                            cmd.Parameters["@ListAgentFullName"].Value = fields[44]; //fields[41];
                            cmd.Parameters["@ListAgentMLSID"].Value = fields[42];
                            cmd.Parameters["@LotDescription"].Value = listings.TranslateLotDesc(fields[46]);
                            cmd.Parameters["@LotSizeArea"].Value = GetFloat(fields[47]);
                            cmd.Parameters["@LotSizeAreaSQFT"].Value = GetInt(fields[48]);
                            string t = fields[51];
                            if(t.StartsWith("\"")) { t = t.Substring(1); }
                            if(t.EndsWith("\"")) { t = t.Substring(0, t.Length - 1); }
                            cmd.Parameters["@MarketingRemarks"].Value = t;
                            cmd.Parameters["@Matrix_Unique_ID"].Value = fields[53];
                            cmd.Parameters["@MLSNumber"].Value = fields[54];
                            cmd.Parameters["@OpenHouseUpcoming"].Value = fields[56];
                            cmd.Parameters["@PhotoCount"].Value = GetInt(fields[59]);
                            cmd.Parameters["@PostalCode"].Value = fields[62];
                            cmd.Parameters["@PostalCodePlus4"].Value = fields[63];
                            cmd.Parameters["@PropertyType"].Value = listings.TranslatePropertyType(fields[64]);
                            cmd.Parameters["@PUD"].Value = GetBit(fields[65]);
                            cmd.Parameters["@REO_BankOwned"].Value = GetBit(fields[66]);
                            cmd.Parameters["@RoadFrontDescription"].Value = listings.TranslateRoadFrontDescription(fields[67]);
                            cmd.Parameters["@RoomCount"].Value = GetInt(fields[68]);
                            cmd.Parameters["@SchoolDistrict"].Value = fields[69];
                            cmd.Parameters["@SewerDescription"].Value = listings.TranslateSewerDesc(fields[72]);
                            cmd.Parameters["@SoilType"].Value = listings.TranslateSoilType(fields[73]);
                            cmd.Parameters["@SqFtSource"].Value = fields[76];
                            cmd.Parameters["@SqFtTotal"].Value = GetInt(fields[77]);
                            cmd.Parameters["@StateOrProvince"].Value = fields[78];
                            cmd.Parameters["@Status"].Value = fields[79];
                            cmd.Parameters["@StreetDirPrefix"].Value = fields[81];
                            cmd.Parameters["@StreetDirSuffix"].Value = fields[82];
                            cmd.Parameters["@StreetName"].Value = fields[83];
                            cmd.Parameters["@StreetNumber"].Value = fields[84];
                            cmd.Parameters["@StreetNumberModifier"].Value = fields[85];
                            cmd.Parameters["@StreetSuffix"].Value = fields[86];
                            cmd.Parameters["@StreetType"].Value = listings.TranslateStreetType(fields[87]);
                            cmd.Parameters["@Style"].Value = listings.TranslateStyle(fields[88]);
                            cmd.Parameters["@Subdivision_Development"].Value = fields[89];
                            cmd.Parameters["@TaxAmount"].Value = GetInt(fields[90]);
                            cmd.Parameters["@TaxSource"].Value = fields[91];
                            cmd.Parameters["@TaxYear"].Value = GetInt(fields[92]);
                            cmd.Parameters["@Topography"].Value = listings.TranslateTopography(fields[93]);
                            cmd.Parameters["@TotalRoomsFinished"].Value = GetInt(fields[94]);
                            cmd.Parameters["@TypeofDwelling"].Value = fields[95];
                            cmd.Parameters["@TypeofUnit"].Value = fields[96];
                            cmd.Parameters["@UnitCount"].Value = GetFloat(fields[97]);
                            cmd.Parameters["@UnitNumber"].Value = fields[98];
                            cmd.Parameters["@UtilitiesOn_AbuttingSite"].Value = listings.TranslateUtilitiesOn_AbuttingSite(fields[99]);
                            cmd.Parameters["@Village"].Value = fields[100];
                            cmd.Parameters["@VirtualTourLink"].Value = fields[101];
                            cmd.Parameters["@WaterAccessYN"].Value = GetBit(fields[102]);
                            cmd.Parameters["@WaterDescription"].Value = listings.TranslateWaterDesc(fields[103]);
                            cmd.Parameters["@YearBuilt"].Value = GetFloat(fields[104]);
                            cmd.Parameters["@YearBuiltException"].Value = fields[105];
                            cmd.Parameters["@YearRenovated"].Value = GetInt(fields[106]);
                            cmd.Parameters["@Zoning"].Value = fields[107];

                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }
                    }

                    //using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-PriceHistory] ([MLS #], [Asking Price]) SELECT r.[MLS #], r.[Asking Price] FROM[listings-residential] r LEFT JOIN[listings-PriceHistory] ph ON r.[MLS #] = ph.[MLS #] AND r.[Asking Price] = ph.[Asking Price] WHERE[Status] = 'Active' AND ph.Serial IS NULL", cn)) {
                    //    cmd.CommandType = CommandType.Text;
                    //    cmd.Connection.Open();
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Connection.Close();
                    //}
                }
            }

            BingGeocoder.UpdateGeoCodingLand3();

            return View();
        }
        public ActionResult ImportSullivanRes3() {
            Server.ScriptTimeout = 300;

            //using (StreamReader sr = new StreamReader(Server.MapPath("/dl/residential.txt"))) {

            //using(StreamReader sr = new StreamReader(@"C:\Projects\Clients\KKPR\CatskillFarms\lazy-meadow\lazymeadowsrealty.com\downloads\residential.txt")) {
            using(StreamReader sr = new StreamReader(@"E:\Sites\LazyMeadows\downloads\residential.txt")) {
                Listings3 listings = new Listings3();

                using(SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
                    using(SqlCommand cmd = new SqlCommand("TRUNCATE TABLE [listings-residential-3]", cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                    string SQL = "INSERT INTO [dbo].[listings-residential-3] " +
                    "           ([AddFeeFrequency] " +
                    "           ,[AdditionalFeeDes] " +
                    "           ,[AdditionalFeesAmt] " +
                    "           ,[Adult55Community] " +
                    "           ,[AirConditioning] " +
                    "           ,[AlternateMLSNumber] " +
                    "           ,[Amenities] " +
                    "           ,[AtticDescription] " +
                    "           ,[BasementDescription] " +
                    "           ,[BathsFull] " +
                    "           ,[BathsHalf] " +
                    "           ,[BathsTotal] " +
                    "           ,[BedsTotal] " +
                    "           ,[City] " +
                    "           ,[CloseDate] " +
                    "           ,[ComplexName] " +
                    "           ,[ConstructionDescription] " +
                    "           ,[ContractDate] " +
                    "           ,[CountyOrParish] " +
                    "           ,[CurrentPrice] " +
                    "           ,[ElementarySchool] " +
                    "           ,[Fireplacesnumberof] " +
                    "           ,[Garbage] " +
                    "           ,[Hamlet] " +
                    "           ,[HeatingFuel] " +
                    "           ,[HeatingType] " +
                    "           ,[HeatingZonesNumof] " +
                    "           ,[HighSchool] " +
                    "           ,[HoaFeeIncludes] " +
                    "           ,[Hotwater] " +
                    "           ,[Included] " +
                    "           ,[Junior_MiddleHighSchool] " +
                    "           ,[LeasePrice] " +
                    "           ,[ListAgentDirectWorkPhone] " +
                    "           ,[ListAgentEmail] " +
                    "           ,[ListAgentFullName] " +
                    "           ,[ListAgentMLSID] " +
                    "           ,[LotDescription] " +
                    "           ,[LotSizeArea] " +
                    "           ,[LotSizeAreaSQFT] " +
                    "           ,[MarketingRemarks] " +
                    "           ,[Matrix_Unique_ID] " +
                    "           ,[MLSNumber] " +
                    "           ,[Model] " +
                    "           ,[MonthlyHOAFee] " +
                    "           ,[NumOfLevels] " +
                    "           ,[OpenHouseUpcoming] " +
                    "           ,[Parking] " +
                    "           ,[PhotoCount] " +
                    "           ,[PostalCode] " +
                    "           ,[PostalCodePlus4] " +
                    "           ,[PropertyType] " +
                    "           ,[PUD] " +
                    "           ,[REO_BankOwned] " +
                    "           ,[RoomCount] " +
                    "           ,[SchoolDistrict] " +
                    "           ,[SewerDescription] " +
                    "           ,[SidingDescription] " +
                    "           ,[SqFtSource] " +
                    "           ,[SqFtTotal] " +
                    "           ,[StateOrProvince] " +
                    "           ,[Status] " +
                    "           ,[StreetDirPrefix] " +
                    "           ,[StreetDirSuffix] " +
                    "           ,[StreetName] " +
                    "           ,[StreetNumber] " +
                    "           ,[StreetNumberModifier] " +
                    "           ,[StreetSuffix] " +
                    "           ,[StreetType] " +
                    "           ,[Style] " +
                    "           ,[Subdivision_Development] " +
                    "           ,[TaxAmount] " +
                    "           ,[TaxSource] " +
                    "           ,[TaxYear] " +
                    "           ,[TotalRoomsFinished] " +
                    "           ,[UnitCount] " +
                    "           ,[UnitNumber] " +
                    "           ,[Village] " +
                    "           ,[VirtualTourLink] " +
                    "           ,[WaterAccessYN] " +
                    "           ,[WaterDescription] " +
                    "           ,[YearBuilt] " +
                    "           ,[YearBuiltException] " +
                    "           ,[YearRenovated] " +
                    "           ,[Zoning]) " +
                    "     VALUES " +
                    "           (@AddFeeFrequency " +
                    "           ,@AdditionalFeeDes " +
                    "           ,@AdditionalFeesAmt " +
                    "           ,@Adult55Community " +
                    "           ,@AirConditioning " +
                    "           ,@AlternateMLSNumber " +
                    "           ,@Amenities " +
                    "           ,@AtticDescription " +
                    "           ,@BasementDescription " +
                    "           ,@BathsFull " +
                    "           ,@BathsHalf " +
                    "           ,@BathsTotal " +
                    "           ,@BedsTotal " +
                    "           ,@City " +
                    "           ,@CloseDate " +
                    "           ,@ComplexName " +
                    "           ,@ConstructionDescription " +
                    "           ,@ContractDate " +
                    "           ,@CountyOrParish " +
                    "           ,@CurrentPrice " +
                    "           ,@ElementarySchool " +
                    "           ,@Fireplacesnumberof " +
                    "           ,@Garbage " +
                    "           ,@Hamlet " +
                    "           ,@HeatingFuel " +
                    "           ,@HeatingType " +
                    "           ,@HeatingZonesNumof " +
                    "           ,@HighSchool " +
                    "           ,@HoaFeeIncludes " +
                    "           ,@Hotwater " +
                    "           ,@Included " +
                    "           ,@Junior_MiddleHighSchool " +
                    "           ,@LeasePrice " +
                    "           ,@ListAgentDirectWorkPhone " +
                    "           ,@ListAgentEmail " +
                    "           ,@ListAgentFullName " +
                    "           ,@ListAgentMLSID " +
                    "           ,@LotDescription " +
                    "           ,@LotSizeArea " +
                    "           ,@LotSizeAreaSQFT " +
                    "           ,@MarketingRemarks " +
                    "           ,@Matrix_Unique_ID " +
                    "           ,@MLSNumber " +
                    "           ,@Model " +
                    "           ,@MonthlyHOAFee " +
                    "           ,@NumOfLevels " +
                    "           ,@OpenHouseUpcoming " +
                    "           ,@Parking " +
                    "           ,@PhotoCount " +
                    "           ,@PostalCode " +
                    "           ,@PostalCodePlus4 " +
                    "           ,@PropertyType " +
                    "           ,@PUD " +
                    "           ,@REO_BankOwned " +
                    "           ,@RoomCount " +
                    "           ,@SchoolDistrict " +
                    "           ,@SewerDescription " +
                    "           ,@SidingDescription " +
                    "           ,@SqFtSource " +
                    "           ,@SqFtTotal " +
                    "           ,@StateOrProvince " +
                    "           ,@Status " +
                    "           ,@StreetDirPrefix " +
                    "           ,@StreetDirSuffix " +
                    "           ,@StreetName " +
                    "           ,@StreetNumber " +
                    "           ,@StreetNumberModifier " +
                    "           ,@StreetSuffix " +
                    "           ,@StreetType " +
                    "           ,@Style " +
                    "           ,@Subdivision_Development " +
                    "           ,@TaxAmount " +
                    "           ,@TaxSource " +
                    "           ,@TaxYear " +
                    "           ,@TotalRoomsFinished " +
                    "           ,@UnitCount " +
                    "           ,@UnitNumber " +
                    "           ,@Village " +
                    "           ,@VirtualTourLink " +
                    "           ,@WaterAccessYN " +
                    "           ,@WaterDescription " +
                    "           ,@YearBuilt " +
                    "           ,@YearBuiltException " +
                    "           ,@YearRenovated " +
                    "           ,@Zoning)";
                    using(SqlCommand cmd = new SqlCommand(SQL, cn)) {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@AddFeeFrequency", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@AdditionalFeeDes", SqlDbType.NVarChar, 40);
                        cmd.Parameters.Add("@AdditionalFeesAmt", SqlDbType.Float);
                        cmd.Parameters.Add("@Adult55Community", SqlDbType.Bit);
                        cmd.Parameters.Add("@AirConditioning", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@AlternateMLSNumber", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@Amenities", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@AtticDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@BasementDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@BathsFull", SqlDbType.Int);
                        cmd.Parameters.Add("@BathsHalf", SqlDbType.Float);
                        cmd.Parameters.Add("@BathsTotal", SqlDbType.Float);
                        cmd.Parameters.Add("@BedsTotal", SqlDbType.Int);
                        cmd.Parameters.Add("@City", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@CloseDate", SqlDbType.DateTime);
                        cmd.Parameters.Add("@ComplexName", SqlDbType.NVarChar, 30);
                        cmd.Parameters.Add("@ConstructionDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@ContractDate", SqlDbType.DateTime);
                        cmd.Parameters.Add("@CountyOrParish", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@CurrentPrice", SqlDbType.Float);
                        cmd.Parameters.Add("@ElementarySchool", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Fireplacesnumberof", SqlDbType.Int);
                        cmd.Parameters.Add("@Garbage", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Hamlet", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@HeatingFuel", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@HeatingType", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@HeatingZonesNumof", SqlDbType.Int);
                        cmd.Parameters.Add("@HighSchool", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@HoaFeeIncludes", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Hotwater", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Included", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Junior_MiddleHighSchool", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@LeasePrice", SqlDbType.Float);
                        cmd.Parameters.Add("@ListAgentDirectWorkPhone", SqlDbType.NVarChar, 50);
                        cmd.Parameters.Add("@ListAgentEmail", SqlDbType.NVarChar, 80);
                        cmd.Parameters.Add("@ListAgentFullName", SqlDbType.NVarChar, 150);
                        cmd.Parameters.Add("@ListAgentMLSID", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@LotDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@LotSizeArea", SqlDbType.Float);
                        cmd.Parameters.Add("@LotSizeAreaSQFT", SqlDbType.Int);
                        cmd.Parameters.Add("@MarketingRemarks", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Matrix_Unique_ID", SqlDbType.NVarChar, 19);
                        cmd.Parameters.Add("@MLSNumber", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@Model", SqlDbType.NVarChar, 20);
                        cmd.Parameters.Add("@MonthlyHOAFee", SqlDbType.Int);
                        cmd.Parameters.Add("@NumOfLevels", SqlDbType.Float);
                        cmd.Parameters.Add("@OpenHouseUpcoming", SqlDbType.NVarChar, 255);
                        cmd.Parameters.Add("@Parking", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@PhotoCount", SqlDbType.Int);
                        cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 10);
                        cmd.Parameters.Add("@PostalCodePlus4", SqlDbType.NVarChar, 4);
                        cmd.Parameters.Add("@PropertyType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@PUD", SqlDbType.Bit);
                        cmd.Parameters.Add("@REO_BankOwned", SqlDbType.Bit);
                        cmd.Parameters.Add("@RoomCount", SqlDbType.Int);
                        cmd.Parameters.Add("@SchoolDistrict", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@SewerDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@SidingDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@SqFtSource", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@SqFtTotal", SqlDbType.Int);
                        cmd.Parameters.Add("@StateOrProvince", SqlDbType.NVarChar, 2);
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetDirPrefix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetDirSuffix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetName", SqlDbType.NVarChar, 50);
                        cmd.Parameters.Add("@StreetNumber", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@StreetNumberModifier", SqlDbType.NVarChar, 8);
                        cmd.Parameters.Add("@StreetSuffix", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@StreetType", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@Style", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@Subdivision_Development", SqlDbType.NVarChar, 30);
                        cmd.Parameters.Add("@TaxAmount", SqlDbType.Int);
                        cmd.Parameters.Add("@TaxSource", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@TaxYear", SqlDbType.Int);
                        cmd.Parameters.Add("@TotalRoomsFinished", SqlDbType.Int);
                        cmd.Parameters.Add("@UnitCount", SqlDbType.Float);
                        cmd.Parameters.Add("@UnitNumber", SqlDbType.NVarChar, 25);
                        cmd.Parameters.Add("@Village", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@VirtualTourLink", SqlDbType.NVarChar, 200);
                        cmd.Parameters.Add("@WaterAccessYN", SqlDbType.Bit);
                        cmd.Parameters.Add("@WaterDescription", SqlDbType.NVarChar, 1000);
                        cmd.Parameters.Add("@YearBuilt", SqlDbType.Float);
                        cmd.Parameters.Add("@YearBuiltException", SqlDbType.NVarChar, 75);
                        cmd.Parameters.Add("@YearRenovated", SqlDbType.Int);
                        cmd.Parameters.Add("@Zoning", SqlDbType.NVarChar, 255);

                        string line;
                        sr.ReadLine(); //Burn the header line
                        while((line = sr.ReadLine()) != null) {
                            string[] fields = line.Split('\t');
                            cmd.Parameters["@AddFeeFrequency"].Value = listings.TranslateAddFeeFrequency(fields[0]);
                            cmd.Parameters["@AdditionalFeeDes"].Value = fields[1];
                            cmd.Parameters["@AdditionalFeesAmt"].Value = GetFloat(fields[2]);
                            cmd.Parameters["@Adult55Community"].Value = GetBit(fields[3]);
                            cmd.Parameters["@AirConditioning"].Value = listings.TranslateAirCond(fields[4]);
                            cmd.Parameters["@AlternateMLSNumber"].Value = fields[5];
                            cmd.Parameters["@Amenities"].Value = listings.TranslateAmenities(fields[6]);
                            cmd.Parameters["@AtticDescription"].Value = listings.TranslateAttic(fields[7]);
                            cmd.Parameters["@BasementDescription"].Value = listings.TranslateBasement(fields[8]);
                            cmd.Parameters["@BathsFull"].Value = GetInt(fields[9]);
                            cmd.Parameters["@BathsHalf"].Value = GetFloat(fields[10]);
                            cmd.Parameters["@BathsTotal"].Value = GetFloat(fields[11]);
                            cmd.Parameters["@BedsTotal"].Value = GetInt(fields[12]);
                            cmd.Parameters["@City"].Value = listings.TranslateCity3(fields[13], fields[73]);
                            DateTime temp = GetDatetime(fields[21]);
                            cmd.Parameters["@CloseDate"].Value = temp != DateTime.MinValue ? temp : SqlDateTime.Null;
                            cmd.Parameters["@ComplexName"].Value = fields[25];
                            cmd.Parameters["@ConstructionDescription"].Value = listings.TranslateConstructionDesc(fields[26]);
                            DateTime temp2 = GetDatetime(fields[27]);
                            cmd.Parameters["@ContractDate"].Value = temp2 != DateTime.MinValue ? temp2 : SqlDateTime.Null; ;
                            cmd.Parameters["@CountyOrParish"].Value = fields[28];
                            cmd.Parameters["@CurrentPrice"].Value = GetFloat(fields[29]);
                            cmd.Parameters["@ElementarySchool"].Value = fields[30];
                            cmd.Parameters["@Fireplacesnumberof"].Value = GetInt(fields[31]);
                            cmd.Parameters["@Garbage"].Value = listings.TranslateGarbage(fields[32]);
                            cmd.Parameters["@Hamlet"].Value = fields[33];
                            cmd.Parameters["@HeatingFuel"].Value = listings.TranslateHeatingFuel(fields[34]);
                            cmd.Parameters["@HeatingType"].Value = listings.TranslateHeatingType(fields[35]);
                            cmd.Parameters["@HeatingZonesNumof"].Value = GetInt(fields[36]);
                            cmd.Parameters["@HighSchool"].Value = fields[37];
                            cmd.Parameters["@HoaFeeIncludes"].Value = listings.TranslateHOAFee(fields[38]);
                            cmd.Parameters["@Hotwater"].Value = listings.TranslateHotWater(fields[39]);
                            cmd.Parameters["@Included"].Value = listings.TranslateIncluded(fields[42]);
                            cmd.Parameters["@Junior_MiddleHighSchool"].Value = fields[43];
                            cmd.Parameters["@LeasePrice"].Value = GetFloat(fields[46]);
                            cmd.Parameters["@ListAgentDirectWorkPhone"].Value = fields[47];
                            cmd.Parameters["@ListAgentEmail"].Value = fields[48];
                            cmd.Parameters["@ListAgentFullName"].Value = fields[52]; //fields[49];
                            cmd.Parameters["@ListAgentMLSID"].Value = fields[50];
                            cmd.Parameters["@LotDescription"].Value = listings.TranslateLotDesc(fields[54]);
                            cmd.Parameters["@LotSizeArea"].Value = GetFloat(fields[55]);
                            cmd.Parameters["@LotSizeAreaSQFT"].Value = GetInt(fields[56]);
                            string t = fields[59];
                            if(t.StartsWith("\"")) { t = t.Substring(1); }
                            if(t.EndsWith("\"")) { t = t.Substring(0, t.Length - 1); }
                            cmd.Parameters["@MarketingRemarks"].Value = t;
                            cmd.Parameters["@Matrix_Unique_ID"].Value = fields[61];
                            cmd.Parameters["@MLSNumber"].Value = fields[62];
                            cmd.Parameters["@Model"].Value = fields[63];
                            cmd.Parameters["@MonthlyHOAFee"].Value = GetInt(fields[64]);
                            cmd.Parameters["@NumOfLevels"].Value = GetFloat(fields[65]);
                            cmd.Parameters["@OpenHouseUpcoming"].Value = fields[67];
                            cmd.Parameters["@Parking"].Value = listings.TranslateParking(fields[70]);
                            cmd.Parameters["@PhotoCount"].Value = GetInt(fields[71]);
                            cmd.Parameters["@PostalCode"].Value = fields[74];
                            cmd.Parameters["@PostalCodePlus4"].Value = fields[75];
                            cmd.Parameters["@PropertyType"].Value = listings.TranslatePropertyType(fields[76]);
                            cmd.Parameters["@PUD"].Value = GetBit(fields[77]);
                            cmd.Parameters["@REO_BankOwned"].Value = GetBit(fields[78]);
                            cmd.Parameters["@RoomCount"].Value = GetInt(fields[79]);
                            cmd.Parameters["@SchoolDistrict"].Value = fields[80];
                            cmd.Parameters["@SewerDescription"].Value = listings.TranslateSewerDesc(fields[83]);
                            cmd.Parameters["@SidingDescription"].Value = listings.TranslateSiding(fields[84]);
                            cmd.Parameters["@SqFtSource"].Value = fields[87];
                            cmd.Parameters["@SqFtTotal"].Value = GetInt(fields[88]);
                            cmd.Parameters["@StateOrProvince"].Value = fields[89];
                            cmd.Parameters["@Status"].Value = fields[90];
                            cmd.Parameters["@StreetDirPrefix"].Value = fields[92];
                            cmd.Parameters["@StreetDirSuffix"].Value = fields[93];
                            cmd.Parameters["@StreetName"].Value = fields[94];
                            cmd.Parameters["@StreetNumber"].Value = fields[95];
                            cmd.Parameters["@StreetNumberModifier"].Value = fields[96];
                            cmd.Parameters["@StreetSuffix"].Value = fields[97];
                            cmd.Parameters["@StreetType"].Value = listings.TranslateStreetType(fields[98]);
                            cmd.Parameters["@Style"].Value = listings.TranslateStyle(fields[99]);
                            cmd.Parameters["@Subdivision_Development"].Value = fields[100];
                            cmd.Parameters["@TaxAmount"].Value = GetInt(fields[101]);
                            cmd.Parameters["@TaxSource"].Value = fields[102];
                            cmd.Parameters["@TaxYear"].Value = GetInt(fields[103]);
                            cmd.Parameters["@TotalRoomsFinished"].Value = GetInt(fields[104]);
                            cmd.Parameters["@UnitCount"].Value = GetFloat(fields[105]);
                            cmd.Parameters["@UnitNumber"].Value = fields[106];
                            cmd.Parameters["@Village"].Value = fields[107];
                            cmd.Parameters["@VirtualTourLink"].Value = fields[108];
                            cmd.Parameters["@WaterAccessYN"].Value = GetBit(fields[109]);
                            cmd.Parameters["@WaterDescription"].Value = listings.TranslateWaterDesc(fields[110]);
                            cmd.Parameters["@YearBuilt"].Value = GetFloat(fields[111]);
                            cmd.Parameters["@YearBuiltException"].Value = fields[112];
                            cmd.Parameters["@YearRenovated"].Value = GetInt(fields[113]);
                            cmd.Parameters["@Zoning"].Value = fields[114];

                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();
                            cmd.Connection.Close();
                        }
                    }

                    //using(SqlCommand cmd = new SqlCommand("INSERT INTO [listings-PriceHistory] ([MLS #], [Asking Price]) SELECT r.[MLS #], r.[Asking Price] FROM[listings-residential] r LEFT JOIN[listings-PriceHistory] ph ON r.[MLS #] = ph.[MLS #] AND r.[Asking Price] = ph.[Asking Price] WHERE[Status] = 'Active' AND ph.Serial IS NULL", cn)) {
                    //    cmd.CommandType = CommandType.Text;
                    //    cmd.Connection.Open();
                    //    cmd.ExecuteNonQuery();
                    //    cmd.Connection.Close();
                    //}
                }
            }

            BingGeocoder.UpdateGeoCoding3();
            //BingGeolocation? bl = BingGeocoder.ResolveAddress("25 S Lewcy lane", "White Lake", "NY", "12786", "US");
            //if (bl != null) {
            //	string temp = "Lat: " + bl.Value.Lat.ToString() + " Lon:" + bl.Value.Lon;
            //}

            //SendConfirm();
            return View();

        }

        private int GetBit(string val) {
            int ret = 0;

            if(int.TryParse(val, out ret)) {
                return int.Parse(ret.ToString());
            } else {
                return 0;
            }
        }
        private DateTime GetDatetime(string val) {
            DateTime ret = DateTime.MinValue;

            if(DateTime.TryParse(val, out ret)) {
                return DateTime.Parse(ret.ToString());
            } else {
                return DateTime.MinValue;
            }
        }
        private SqlInt32 GetInt(string val) {
			int ret = 0;

			if (int.TryParse(val, out ret)) {
				return SqlInt32.Parse(ret.ToString());
			} else {
				return -1;
			}
		}
		private SqlDouble GetFloat(string val) {
			float ret = 0;

			if (float.TryParse(val, out ret)) {
				return SqlDouble.Parse(ret.ToString());
			} else {
				return -1;
			}
		}
	}
}