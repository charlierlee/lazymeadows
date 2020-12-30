using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TKS.Areas.Admin.Models;
//https://ddwiki.reso.org/display/DDW17/Property+Resource
//https://bridgedataoutput.com/docs/platform/API/bridge
//https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=a57e5dfd4ce945cc52206e83bf534718&limit=200&CountyOrParish.eq=Sullivan&PropertyType.eq=Residential&MlsStatus.eq=A
//https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=a57e5dfd4ce945cc52206e83bf534718&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&PropertyType=Land
//https://api.bridgedataoutput.com/api/v2/onekey/listings?access_token=a57e5dfd4ce945cc52206e83bf534718&limit=200&sortBy=ListingId&order=asc&CountyOrParish=Sullivan&MlsStatus=A&PropertyType=Commercial%20Sale

namespace TKS.Models.realestate {
	public class MLS {
		public string success { get; set; }
		public string status { get; set; }
		public int total { get; set; }
		public List<Prop> bundle { get; set; }
	}
	public class Prop {
		public List<string> AccessibilityFeatures { get; set; }
		public List<string> Appliances { get; set; }
		public List<string> ArchitecturalStyle { get; set; } //Style
		public List<string> AssociationAmenities { get; set; } //Amenities
		public float? AssociationFee { get; set; } //AdditionalFeesAmt
		public string AssociationFeeFrequency { get; set; } //AddFeeFrequency
		public List<string> AssociationFeeIncludes { get; set; } //AdditionalFeeDes
		public bool? AssociationYN { get; set; }
		public bool? AttachedGarageYN { get; set; }
		public List<string> Basement { get; set; } //BasementDescription
		public int? BathroomsFull { get; set; } //BathsFull
		public int? BathroomsHalf { get; set; } //BathsHalf
		public int? BathroomsTotalinteger { get; set; } //BathsTotal
		public int? BedroomsTotal { get; set; } //BedsTotal
		public float? BuildingAreaTotal { get; set; }
		public string BuildingAreaUnits { get; set; }
		public List<string> BuildingFeatures { get; set; }
		public string BuildingName { get; set; }
		public string BusinessName { get; set; }
		public List<string> BusinessType { get; set; }
		public bool? CarportYN { get; set; }
		public string City { get; set; } //City
		public DateTime? CloseDate { get; set; } //CloseDate
		public List<string> CommunityFeatures { get; set; }
		public List<string> ConstructionMaterials { get; set; }  //ConstructionDescription
		public List<string> Cooling { get; set; } //AirConditioning
		public bool? CoolingYN { get; set; }
		public string CrossStreet { get; set; }
		public List<string> CurrentUse { get; set; }
		public int? DaysOnMarket { get; set; }
		public List<string> DevelopmentStatus { get; set; }
		public string DirectionFaces { get; set; }
		public string Directions { get; set; }
		public List<string> Disclosures { get; set; }
		public string DistanceToSchoolsComments { get; set; }
		public string DistanceToShoppingComments { get; set; }
		public List<string> DoorFeatures { get; set; }
		public List<string> Electric { get; set; }
		public bool? ElectricOnPropertyYN { get; set; }
		public string ElementarySchool { get; set; } //ElementarySchool
		public string ElementarySchoolDistrict { get; set; }
		public int? EntryLevel { get; set; }
		public string EntryLocation { get; set; }
		public string Exclusions { get; set; }
		public List<string> ExteriorFeatures { get; set; } //SidingDescription
		public List<string> Fencing { get; set; }
		public List<string> FireplaceFeatures { get; set; }
		public int? FireplacesTotal { get; set; } //Fireplacesnumberof
		public bool? FireplaceYN { get; set; }
		public List<string> Flooring { get; set; } //
		public float? GarageSpaces { get; set; }
		public bool? GarageYN { get; set; }
		public bool? HabitableResidenceYN { get; set; }
		public List<string> Heating { get; set; } //HeatingType
		public string HighSchool { get; set; }//HighSchool
		public string HighSchoolDistrict { get; set; }
		public List<string> HorseAmenities { get; set; }
		public bool? HorseYN { get; set; }
		public string Inclusions { get; set; } //Included
		public List<string> InteriorFeatures { get; set; }
		public bool? InternetAddressDisplayYN { get; set; }
		public bool? InternetEntireListingDisplayYN { get; set; }
		public List<string> LaundryFeatures { get; set; }
		public List<string> Levels { get; set; }
		public string ListAgentEmail { get; set; } //	ListAgentEmail
		public string ListAgentFullName { get; set; } //	ListAgentFullName
		public string ListAgentMlsId { get; set; } //	ListAgentMLSID
		public string ListAgentPreferredPhone { get; set; } //	ListAgentDirectWorkPhone
		public string ListingKey { get; set; }
		public string ListOfficeName { get; set; }
		public float? ListPrice { get; set; }  //CurrentPrice
		public float? LivingArea { get; set; } //SqFtTotal
		public string LivingAreaUnits { get; set; } //UnitCount
		public string LotDimensionsSource { get; set; }
		public List<string> LotFeatures { get; set; } //LotDescription
		public float? LotSizeAcres { get; set; }
		public float? LotSizeArea { get; set; } //LotSizeArea
		public string LotSizeDimensions { get; set; }
		public string LotSizeSource { get; set; }
		public float? LotSizeSquareFeet { get; set; } //LotSizeAreaSQFT
		public string LotSizeUnits { get; set; }
		public List<MediaItem> Media { get; set; }
		public string MiddleOrJuniorSchool { get; set; }  //Junior_MiddleHighSchool
		public string MiddleOrJuniorSchoolDistrict { get; set; }
		public bool? NewConstructionYN { get; set; }
		public string ONEKEY_AdditionalFeeDescription { get; set; }
		public List<string> ONEKEY_AdditionalFeeFrequency { get; set; }
		public bool? ONEKEY_AdditionalFeesYN { get; set; }
		public string ONEKEY_AlternateMlsNumber { get; set; } //AlternateMLSNumber
		public List<string> ONEKEY_AtticDescription { get; set; } //AtticDescription
		public string ONEKEY_BuildingDimensions { get; set; }
		public string ONEKEY_BuildingLocation { get; set; }
		public string ONEKEY_BuildingSize { get; set; }
		public string ONEKEY_BusinessAge { get; set; }
		public string ONEKEY_BusinessLocatedAt { get; set; }
		public string ONEKEY_DescriptionOfBusiness { get; set; }
		public string ONEKEY_DevelopmentName { get; set; } //ComplexName
		public List<string> ONEKEY_GarbageRemoval { get; set; } //Garbage
		public List<string> ONEKEY_Hamlet { get; set; } //Hamlet
		public string ONEKEY_HeatingNotes { get; set; }
		public List<string> ONEKEY_HotWater { get; set; } //Hotwater
		public string ONEKEY_ImprovementRemarks { get; set; }
		public List<string> ONEKEY_LivingQuartersDescription { get; set; }
		public string ONEKEY_LoadingDocks { get; set; }
		public bool? ONEKEY_LoadingDockYN { get; set; }
		public List<string> ONEKEY_LocationDescription { get; set; }
		public string ONEKEY_MinimumAge { get; set; }
		public string ONEKEY_NumberOfHeatingUnits { get; set; }
		public string ONEKEY_NumberofHeatingZones { get; set; }   //HeatingZonesNumof
		public string OriginatingSystemKey { get; set; } //MLSNumber
		public List<string> ONEKEY_Plumbing { get; set; }
		public string ONEKEY_SourceSystemModificationTimestamp { get; set; }
		public string ONEKEY_VideoTourURL { get; set; }
		public List<string> ONEKEY_Village { get; set; } //Village
		public DateTime? OnMarketDate { get; set; }
		public List<string> OtherStructures { get; set; }
		public List<string> OwnerPays { get; set; }
		public string Ownership { get; set; }
		public string ParcelNumber { get; set; }
		public List<string> ParkingFeatures { get; set; } //Parking
		public float? ParkingTotal { get; set; }
		public List<string> PatioAndPorchFeatures { get; set; }
		public string PhotosChangeTimestamp { get; set; }
		public int? PhotosCount { get; set; }  //PhotoCount
		public List<string> PoolFeatures { get; set; }
		public bool? PoolPrivateYN { get; set; }
		public List<string> PossibleUse { get; set; }
		public string PostalCode { get; set; } //PostalCode
		public List<string> PropertyCondition { get; set; }
		public string PropertySubType { get; set; }
		public string PropertyType { get; set; } //PropertyType
		public string Roof { get; set; }
		public int? RoomsTotal { get; set; } //RoomCount
		public List<string> SecurityFeatures { get; set; }
		public bool? SeniorCommunityYN { get; set; } //Adult55Community
		public List<string> Sewer { get; set; } //SewerDescription
		public List<string> SpaFeatures { get; set; }
		public bool? SpaYN { get; set; }
		public List<string> SpecialListingConditions { get; set; }
		public int? StoriesTotal { get; set; } //NumOfLevels
		public string StreetDirPrefix { get; set; } //StreetDirPrefix
		public string StreetDirSuffix { get; set; } //StreetDirSuffix
		public string StreetName { get; set; } //StreetName
		public string StreetNumber { get; set; } //StreetNumber
		public string StreetSuffix { get; set; } //StreetSuffix
		public string StructureType { get; set; }
		public string SubdivisionName { get; set; } //Subdivision_Development
		public string SyndicationRemarks { get; set; } //
		public float? TaxAnnualAmount { get; set; } //TaxAmount
		public float? TaxAssessedValue { get; set; }
		public string TaxBlock { get; set; }
		public string TaxLot { get; set; }
		public string TaxMapNumber { get; set; }
		public List<string> TenantPays { get; set; }
		public string Township { get; set; } //
		public float? TrashExpense { get; set; }
		public string UnitNumber { get; set; } //UnitNumber
		public string UnparsedAddress { get; set; } //
		public List<string> Utilities { get; set; }
		public List<string> View { get; set; }
		public string VirtualTourURLBranded { get; set; } //VirtualTourLink
		public string VirtualTourURLUnbranded { get; set; }
		public List<string> WaterfrontFeatures { get; set; }
		public bool? WaterfrontYN { get; set; } //WaterAccessYN
		public List<string> WaterSource { get; set; }
		public List<string> WindowFeatures { get; set; }
		public int? YearBuilt { get; set; } //	YearBuilt
		public string YearBuiltEffective { get; set; } //YearRenovated
		public string YearBuiltSource { get; set; }
		public string ZoningDescription { get; set; } //Zoning
	}
	public class MediaItem {
		public string Order { get; set; }
		public string MediaURL { get; set; }
		public string MediaCategory { get; set; }
		public string MediaKey { get; set; }
	}

	public class MLSManager {
		private SqlCommand cmdDeleteMedia = new SqlCommand();
		private SqlCommand cmdDeleteProp = new SqlCommand();
		private SqlCommand cmdExisting = new SqlCommand();
		private SqlCommand cmdMedia = new SqlCommand();
		private SqlCommand cmdProp = new SqlCommand();

		public MLSManager() {
			#region Delete Media command
			cmdDeleteMedia.CommandType = CommandType.Text;
			cmdDeleteMedia.CommandText = "DELETE [listings-media-onekey] WHERE ListingKey = @ListingKey";
			cmdDeleteMedia.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25);
			#endregion

			#region Delete Property command
			cmdDeleteProp.CommandType = CommandType.Text;
			cmdDeleteProp.CommandText = "DELETE [listings-residential-onekey] WHERE ListingKey = @ListingKey";
			cmdDeleteProp.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25);
			#endregion

			#region Existing command
			cmdExisting.CommandType = CommandType.Text;
			cmdExisting.CommandText = "SELECT PhotosChangeTimestamp, ONEKEY_SourceSystemModificationTimestamp " +
				"FROM [listings-residential-onekey] WHERE ListingKey = @ListingKey";
			cmdExisting.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25);
			#endregion

			#region Media command
			cmdMedia.CommandType = CommandType.Text;
			cmdMedia.CommandText = "INSERT INTO [listings-media-onekey] ( " +
				"MediaKey, ListingKey, [Order], MediaURL " +
				") VALUES ( " +
				"@MediaKey, @ListingKey, @Order, @MediaURL" +
				")";
			cmdMedia.Parameters.Add("@MediaKey", SqlDbType.NVarChar, 255);
			cmdMedia.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25);
			cmdMedia.Parameters.Add("@Order", SqlDbType.Int);
			cmdMedia.Parameters.Add("@MediaURL", SqlDbType.NVarChar);
			#endregion

			#region Property
			cmdProp.CommandType = CommandType.Text;
			cmdProp.CommandText = "INSERT INTO [dbo].[listings-residential-onekey] (" +
						   "[AccessibilityFeatures]," +
						   "[Appliances]," +
						   "[ArchitecturalStyle]," +
						   "[AssociationAmenities]," +
						   "[AssociationFee]," +
						   "[AssociationFeeFrequency]," +
						   "[AssociationFeeIncludes]," +
						   "[AssociationYN]," +
						   "[AttachedGarageYN]," +
						   "[Basement]," +
						   "[BathroomsFull]," +
						   "[BathroomsHalf]," +
						   "[BathroomsTotalinteger]," +
						   "[BedroomsTotal]," +
						   "[BuildingAreaTotal]," +
						   "[BuildingAreaUnits]," +
						   "[BuildingFeatures]," +
						   "[BuildingName]," +
						   "[BusinessName]," +
						   "[BusinessType]," +
						   "[CarportYN]," +
						   "[City]," +
						   "[CloseDate]," +
						   "[CommunityFeatures]," +
						   "[ConstructionMaterials]," +
						   "[Cooling]," +
						   "[CoolingYN]," +
						   "[CrossStreet]," +
						   "[CurrentUse]," +
						   "[DaysOnMarket]," +
						   "[DevelopmentStatus]," +
						   "[DirectionFaces]," +
						   "[Directions]," +
						   "[Disclosures]," +
						   "[DistanceToSchoolsComments]," +
						   "[DistanceToShoppingComments]," +
						   "[DoorFeatures]," +
						   "[Electric]," +
						   "[ElectricOnPropertyYN]," +
						   "[ElementarySchool]," +
						   "[ElementarySchoolDistrict]," +
						   "[EntryLevel]," +
						   "[EntryLocation]," +
						   "[Exclusions]," +
						   "[ExteriorFeatures]," +
						   "[Fencing]," +
						   "[FireplaceFeatures]," +
						   "[FireplacesTotal]," +
						   "[FireplaceYN]," +
						   "[Flooring]," +
						   "[GarageSpaces]," +
						   "[GarageYN]," +
						   "[HabitableResidenceYN]," +
						   "[Heating]," +
						   "[HighSchool]," +
						   "[HighSchoolDistrict]," +
						   "[HorseAmenities]," +
						   "[HorseYN]," +
						   "[Inclusions]," +
						   "[InteriorFeatures]," +
						   "[InternetAddressDisplayYN]," +
						   "[InternetEntireListingDisplayYN]," +
						   "[LaundryFeatures]," +
						   "[Levels]," +
						   "[ListAgentEmail]," +
						   "[ListAgentFullName]," +
						   "[ListAgentMlsId]," +
						   "[ListAgentPreferredPhone]," +
						   "[ListingKey]," +
						   "[ListOfficeName]," +
						   "[ListPrice]," +
						   "[LivingArea]," +
						   "[LivingAreaUnits]," +
						   "[LotDimensionsSource]," +
						   "[LotFeatures]," +
						   "[LotSizeAcres]," +
						   "[LotSizeArea]," +
						   "[LotSizeDimensions]," +
						   "[LotSizeSource]," +
						   "[LotSizeSquareFeet]," +
						   "[LotSizeUnits]," +
						   "[MiddleOrJuniorSchool]," +
						   "[MiddleOrJuniorSchoolDistrict]," +
						   "[NewConstructionYN]," +
						   "[ONEKEY_AdditionalFeeDescription]," +
						   "[ONEKEY_AdditionalFeeFrequency]," +
						   "[ONEKEY_AdditionalFeesYN]," +
						   "[ONEKEY_AlternateMlsNumber]," +
						   "[ONEKEY_AtticDescription]," +
						   "[ONEKEY_BuildingDimensions]," +
						   "[ONEKEY_BuildingLocation]," +
						   "[ONEKEY_BuildingSize]," +
						   "[ONEKEY_BusinessAge]," +
						   "[ONEKEY_BusinessLocatedAt]," +
						   "[ONEKEY_DescriptionOfBusiness]," +
						   "[ONEKEY_DevelopmentName]," +
						   "[ONEKEY_GarbageRemoval]," +
						   "[ONEKEY_Hamlet]," +
						   "[ONEKEY_HeatingNotes]," +
						   "[ONEKEY_HotWater]," +
						   "[ONEKEY_ImprovementRemarks]," +
						   "[ONEKEY_LivingQuartersDescription]," +
						   "[ONEKEY_LoadingDocks]," +
						   "[ONEKEY_LoadingDockYN]," +
						   "[ONEKEY_LocationDescription]," +
						   "[ONEKEY_MinimumAge]," +
						   "[ONEKEY_NumberOfHeatingUnits]," +
						   "[ONEKEY_NumberofHeatingZones]," +
						   "[ONEKEY_Plumbing]," +
						   "[ONEKEY_SourceSystemModificationTimestamp]," +
						   "[ONEKEY_VideoTourURL]," +
						   "[ONEKEY_Village]," +
						   "[OnMarketDate]," +
						   "[OriginatingSystemKey]," +
						   "[OtherStructures]," +
						   "[OwnerPays]," +
						   "[Ownership]," +
						   "[ParcelNumber]," +
						   "[ParkingFeatures]," +
						   "[ParkingTotal]," +
						   "[PatioAndPorchFeatures]," +
						   "[PhotosChangeTimestamp]," +
						   "[PhotosCount]," +
						   "[PoolFeatures]," +
						   "[PoolPrivateYN]," +
						   "[PossibleUse]," +
						   "[PostalCode]," +
						   "[PropertyCondition]," +
						   "[PropertySubType]," +
						   "[PropertyType]," +
						   "[Roof]," +
						   "[RoomsTotal]," +
						   "[SecurityFeatures]," +
						   "[SeniorCommunityYN]," +
						   "[Sewer]," +
						   "[SpaFeatures]," +
						   "[SpaYN]," +
						   "[SpecialListingConditions]," +
						   "[StoriesTotal]," +
						   "[StreetDirPrefix]," +
						   "[StreetDirSuffix]," +
						   "[StreetName]," +
						   "[StreetNumber]," +
						   "[StreetSuffix]," +
						   "[StructureType]," +
						   "[SubdivisionName]," +
						   "[SyndicationRemarks]," +
						   "[TaxAnnualAmount]," +
						   "[TaxAssessedValue]," +
						   "[TaxBlock]," +
						   "[TaxLot]," +
						   "[TaxMapNumber]," +
						   "[TenantPays]," +
						   "[Township]," +
						   "[TrashExpense]," +
						   "[UnitNumber]," +
						   "[UnparsedAddress]," +
						   "[Utilities]," +
						   "[View]," +
						   "[VirtualTourURLBranded]," +
						   "[VirtualTourURLUnbranded]," +
						   "[WaterfrontFeatures]," +
						   "[WaterfrontYN]," +
						   "[WaterSource]," +
						   "[WindowFeatures]," +
						   "[YearBuilt]," +
						   "[YearBuiltEffective]," +
						   "[YearBuiltSource]," +
						   "[ZoningDescription] " +
					 ") VALUES ( " +
						   "@AccessibilityFeatures, " +
						   "@Appliances, " +
						   "@ArchitecturalStyle, " +
						   "@AssociationAmenities, " +
						   "@AssociationFee," +
						   "@AssociationFeeFrequency," +
						   "@AssociationFeeIncludes, " +
						   "@AssociationYN," +
						   "@AttachedGarageYN," +
						   "@Basement, " +
						   "@BathroomsFull," +
						   "@BathroomsHalf," +
						   "@BathroomsTotalinteger," +
						   "@BedroomsTotal," +
						   "@BuildingAreaTotal," +
						   "@BuildingAreaUnits," +
						   "@BuildingFeatures, " +
						   "@BuildingName, " +
						   "@BusinessName, " +
						   "@BusinessType, " +
						   "@CarportYN," +
						   "@City," +
						   "@CloseDate," +
						   "@CommunityFeatures, " +
						   "@ConstructionMaterials, " +
						   "@Cooling, " +
						   "@CoolingYN," +
						   "@CrossStreet," +
						   "@CurrentUse," +
						   "@DaysOnMarket," +
						   "@DevelopmentStatus," +
						   "@DirectionFaces," +
						   "@Directions, " +
						   "@Disclosures, " +
						   "@DistanceToSchoolsComments," +
						   "@DistanceToShoppingComments," +
						   "@DoorFeatures, " +
						   "@Electric, " +
						   "@ElectricOnPropertyYN," +
						   "@ElementarySchool, " +
						   "@ElementarySchoolDistrict," +
						   "@EntryLevel," +
						   "@EntryLocation, " +
						   "@Exclusions, " +
						   "@ExteriorFeatures, " +
						   "@Fencing, " +
						   "@FireplaceFeatures, " +
						   "@FireplacesTotal," +
						   "@FireplaceYN," +
						   "@Flooring, " +
						   "@GarageSpaces," +
						   "@GarageYN," +
						   "@HabitableResidenceYN," +
						   "@Heating, " +
						   "@HighSchool," +
						   "@HighSchoolDistrict," +
						   "@HorseAmenities, " +
						   "@HorseYN," +
						   "@Inclusions," +
						   "@InteriorFeatures, " +
						   "@InternetAddressDisplayYN," +
						   "@InternetEntireListingDisplayYN," +
						   "@LaundryFeatures, " +
						   "@Levels, " +
						   "@ListAgentEmail," +
						   "@ListAgentFullName," +
						   "@ListAgentMlsId," +
						   "@ListAgentPreferredPhone," +
						   "@ListingKey," +
						   "@ListOfficeName," +
						   "@ListPrice," +
						   "@LivingArea," +
						   "@LivingAreaUnits," +
						   "@LotDimensionsSource," +
						   "@LotFeatures, " +
						   "@LotSizeAcres," +
						   "@LotSizeArea," +
						   "@LotSizeDimensions," +
						   "@LotSizeSource," +
						   "@LotSizeSquareFeet," +
						   "@LotSizeUnits," +
						   "@MiddleOrJuniorSchool," +
						   "@MiddleOrJuniorSchoolDistrict," +
						   "@NewConstructionYN," +
						   "@ONEKEY_AdditionalFeeDescription, " +
						   "@ONEKEY_AdditionalFeeFrequency, " +
						   "@ONEKEY_AdditionalFeesYN," +
						   "@ONEKEY_AlternateMlsNumber, " +
						   "@ONEKEY_AtticDescription, " +
						   "@ONEKEY_BuildingDimensions, " +
						   "@ONEKEY_BuildingLocation, " +
						   "@ONEKEY_BuildingSize, " +
						   "@ONEKEY_BusinessAge, " +
						   "@ONEKEY_BusinessLocatedAt, " +
						   "@ONEKEY_DescriptionOfBusiness, " +
						   "@ONEKEY_DevelopmentName, " +
						   "@ONEKEY_GarbageRemoval, " +
						   "@ONEKEY_Hamlet, " +
						   "@ONEKEY_HeatingNotes, " +
						   "@ONEKEY_HotWater, " +
						   "@ONEKEY_ImprovementRemarks, " +
						   "@ONEKEY_LivingQuartersDescription, " +
						   "@ONEKEY_LoadingDocks, " +
						   "@ONEKEY_LoadingDockYN, " +
						   "@ONEKEY_LocationDescription, " +
						   "@ONEKEY_MinimumAge, " +
						   "@ONEKEY_NumberOfHeatingUnits, " +
						   "@ONEKEY_NumberofHeatingZones, " +
						   "@ONEKEY_Plumbing, " +
						   "@ONEKEY_SourceSystemModificationTimestamp, " +
						   "@ONEKEY_VideoTourURL," +
						   "@ONEKEY_Village, " +
						   "@OnMarketDate," +
						   "@OriginatingSystemKey, " +
						   "@OtherStructures, " +
						   "@OwnerPays, " +
						   "@Ownership, " +
						   "@ParcelNumber," +
						   "@ParkingFeatures, " +
						   "@ParkingTotal," +
						   "@PatioAndPorchFeatures, " +
						   "@PhotosChangeTimestamp," +
						   "@PhotosCount," +
						   "@PoolFeatures, " +
						   "@PoolPrivateYN," +
						   "@PossibleUse," +
						   "@PostalCode," +
						   "@PropertyCondition, " +
						   "@PropertySubType," +
						   "@PropertyType," +
						   "@Roof, " +
						   "@RoomsTotal," +
						   "@SecurityFeatures, " +
						   "@SeniorCommunityYN," +
						   "@Sewer, " +
						   "@SpaFeatures, " +
						   "@SpaYN," +
						   "@SpecialListingConditions, " +
						   "@StoriesTotal," +
						   "@StreetDirPrefix," +
						   "@StreetDirSuffix," +
						   "@StreetName," +
						   "@StreetNumber," +
						   "@StreetSuffix," +
						   "@StructureType, " +
						   "@SubdivisionName," +
						   "@SyndicationRemarks," +
						   "@TaxAnnualAmount," +
						   "@TaxAssessedValue," +
						   "@TaxBlock," +
						   "@TaxLot," +
						   "@TaxMapNumber," +
						   "@TenantPays, " +
						   "@Township," +
						   "@TrashExpense," +
						   "@UnitNumber," +
						   "@UnparsedAddress," +
						   "@Utilities, " +
						   "@View, " +
						   "@VirtualTourURLBranded," +
						   "@VirtualTourURLUnbranded," +
						   "@WaterfrontFeatures, " +
						   "@WaterfrontYN," +
						   "@WaterSource, " +
						   "@WindowFeatures, " +
						   "@YearBuilt," +
						   "@YearBuiltEffective," +
						   "@YearBuiltSource," +
						   "@ZoningDescription)";
			cmdProp.Parameters.Add("@AccessibilityFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Appliances", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ArchitecturalStyle", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@AssociationAmenities", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@AssociationFee", SqlDbType.Float);
			cmdProp.Parameters.Add("@AssociationFeeFrequency", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@AssociationFeeIncludes", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@AssociationYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@AttachedGarageYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@Basement", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@BathroomsFull", SqlDbType.Int);
			cmdProp.Parameters.Add("@BathroomsHalf", SqlDbType.Int);
			cmdProp.Parameters.Add("@BathroomsTotalinteger", SqlDbType.Int);
			cmdProp.Parameters.Add("@BedroomsTotal", SqlDbType.Int);
			cmdProp.Parameters.Add("@BuildingAreaTotal", SqlDbType.Float);
			cmdProp.Parameters.Add("@BuildingAreaUnits", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@BuildingFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@BuildingName", SqlDbType.NVarChar, 255);
			cmdProp.Parameters.Add("@BusinessName", SqlDbType.NVarChar, 255);
			cmdProp.Parameters.Add("@BusinessType", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@CarportYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@City", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@CloseDate", SqlDbType.DateTime);
			cmdProp.Parameters.Add("@CommunityFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ConstructionMaterials", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Cooling", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@CoolingYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@CrossStreet", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@CurrentUse", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@DaysOnMarket", SqlDbType.Int);
			cmdProp.Parameters.Add("@DevelopmentStatus", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@DirectionFaces", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@Directions", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Disclosures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@DistanceToSchoolsComments", SqlDbType.NVarChar, 255);
			cmdProp.Parameters.Add("@DistanceToShoppingComments", SqlDbType.NVarChar, 255);
			cmdProp.Parameters.Add("@DoorFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Electric", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ElectricOnPropertyYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@ElementarySchool", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ElementarySchoolDistrict", SqlDbType.NVarChar, 100);
			cmdProp.Parameters.Add("@EntryLevel", SqlDbType.Int);
			cmdProp.Parameters.Add("@EntryLocation", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Exclusions", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ExteriorFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Fencing", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@FireplaceFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@FireplacesTotal", SqlDbType.Int);
			cmdProp.Parameters.Add("@FireplaceYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@Flooring", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@GarageSpaces", SqlDbType.Float);
			cmdProp.Parameters.Add("@GarageYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@HabitableResidenceYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@Heating", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@HighSchool", SqlDbType.NVarChar, 150);
			cmdProp.Parameters.Add("@HighSchoolDistrict", SqlDbType.NVarChar, 150);
			cmdProp.Parameters.Add("@HorseAmenities", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@HorseYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@Inclusions", SqlDbType.NVarChar, 2000);
			cmdProp.Parameters.Add("@InteriorFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@InternetAddressDisplayYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@InternetEntireListingDisplayYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@LaundryFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Levels", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ListAgentEmail", SqlDbType.NVarChar, 100);
			cmdProp.Parameters.Add("@ListAgentFullName", SqlDbType.NVarChar, 150);
			cmdProp.Parameters.Add("@ListAgentMlsId", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@ListAgentPreferredPhone", SqlDbType.NVarChar, 16);
			cmdProp.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@ListOfficeName", SqlDbType.NVarChar, 255);
			cmdProp.Parameters.Add("@ListPrice", SqlDbType.Float);
			cmdProp.Parameters.Add("@LivingArea", SqlDbType.Float);
			cmdProp.Parameters.Add("@LivingAreaUnits", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@LotDimensionsSource", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@LotFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@LotSizeAcres", SqlDbType.Float);
			cmdProp.Parameters.Add("@LotSizeArea", SqlDbType.Float);
			cmdProp.Parameters.Add("@LotSizeDimensions", SqlDbType.NVarChar, 150);
			cmdProp.Parameters.Add("@LotSizeSource", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@LotSizeSquareFeet", SqlDbType.Float);
			cmdProp.Parameters.Add("@LotSizeUnits", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@MiddleOrJuniorSchool", SqlDbType.NVarChar, 150);
			cmdProp.Parameters.Add("@MiddleOrJuniorSchoolDistrict", SqlDbType.NVarChar, 150);
			cmdProp.Parameters.Add("@NewConstructionYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@ONEKEY_AdditionalFeeDescription", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_AdditionalFeeFrequency", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_AdditionalFeesYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@ONEKEY_AlternateMlsNumber", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_AtticDescription", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_BuildingDimensions", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_BuildingLocation", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_BuildingSize", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_BusinessAge", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_BusinessLocatedAt", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_DescriptionOfBusiness", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_DevelopmentName", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_GarbageRemoval", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_Hamlet", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_HeatingNotes", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_HotWater", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_ImprovementRemarks", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_LivingQuartersDescription", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_LoadingDocks", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_LoadingDockYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@ONEKEY_LocationDescription", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_MinimumAge", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_NumberOfHeatingUnits", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_NumberofHeatingZones", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_Plumbing", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_SourceSystemModificationTimestamp", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ONEKEY_VideoTourURL", SqlDbType.NVarChar);
			cmdProp.Parameters.Add("@ONEKEY_Village", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@OnMarketDate", SqlDbType.DateTime);
			cmdProp.Parameters.Add("@OriginatingSystemKey", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@OtherStructures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@OwnerPays", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Ownership", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ParcelNumber", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@ParkingFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@ParkingTotal", SqlDbType.Float);
			cmdProp.Parameters.Add("@PatioAndPorchFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@PhotosChangeTimestamp", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@PhotosCount", SqlDbType.Int);
			cmdProp.Parameters.Add("@PoolFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@PoolPrivateYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@PossibleUse", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 10);
			cmdProp.Parameters.Add("@PropertyCondition", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@PropertySubType", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@PropertyType", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@Roof", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@RoomsTotal", SqlDbType.Int);
			cmdProp.Parameters.Add("@SecurityFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@SeniorCommunityYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@Sewer", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@SpaFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@SpaYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@SpecialListingConditions", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@StoriesTotal", SqlDbType.Int);
			cmdProp.Parameters.Add("@StreetDirPrefix", SqlDbType.NVarChar, 15);
			cmdProp.Parameters.Add("@StreetDirSuffix", SqlDbType.NVarChar, 15);
			cmdProp.Parameters.Add("@StreetName", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@StreetNumber", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@StreetSuffix", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@StructureType", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@SubdivisionName", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@SyndicationRemarks", SqlDbType.NVarChar, 4000);
			cmdProp.Parameters.Add("@TaxAnnualAmount", SqlDbType.Float);
			cmdProp.Parameters.Add("@TaxAssessedValue", SqlDbType.Float);
			cmdProp.Parameters.Add("@TaxBlock", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@TaxLot", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@TaxMapNumber", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@TenantPays", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@Township", SqlDbType.NVarChar, 50);
			cmdProp.Parameters.Add("@TrashExpense", SqlDbType.Float);
			cmdProp.Parameters.Add("@UnitNumber", SqlDbType.NVarChar, 25);
			cmdProp.Parameters.Add("@UnparsedAddress", SqlDbType.NVarChar, 255);
			cmdProp.Parameters.Add("@Utilities", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@View", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@VirtualTourURLBranded", SqlDbType.NVarChar);
			cmdProp.Parameters.Add("@VirtualTourURLUnbranded", SqlDbType.NVarChar);
			cmdProp.Parameters.Add("@WaterfrontFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@WaterfrontYN", SqlDbType.Bit);
			cmdProp.Parameters.Add("@WaterSource", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@WindowFeatures", SqlDbType.NVarChar, 1024);
			cmdProp.Parameters.Add("@YearBuilt", SqlDbType.Int);
			cmdProp.Parameters.Add("@YearBuiltEffective", SqlDbType.NVarChar, 11);
			cmdProp.Parameters.Add("@YearBuiltSource", SqlDbType.NVarChar, 60);
			cmdProp.Parameters.Add("@ZoningDescription", SqlDbType.NVarChar, 255);
			#endregion
		}

		public void PreUpdate() {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "UPDATE [listings-residential-onekey] " +
					"SET LastSeen = NULL";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
		}
		public int PostUpdate() {
			int deleted = 0;
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				//Delete orphaned records
				string SQL = "DELETE [listings-residential-onekey] WHERE LastSeen IS NULL";

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					deleted = cmd.ExecuteNonQuery();
					cmd.Connection.Close();

					//Delete duplicate MLS numbers
					cmd.CommandText = "WITH cte AS (" +
									"		SELECT [Key], [OriginatingSystemKey], ROW_NUMBER() " +
									"		OVER (" +
									"			PARTITION BY [OriginatingSystemKey]" +
									"			ORDER BY [OriginatingSystemKey]" +
									"		) row_num" +
									"		FROM [listings-residential-onekey]" +
									"	)" +
									"	DELETE a FROM [listings-residential-onekey] a JOIN (SELECT * FROM cte WHERE row_num > 1) b ON a.[Key] = b.[Key]";
					cmd.Connection.Open();
					deleted += cmd.ExecuteNonQuery();
					cmd.Connection.Close();
				}
			}
			return deleted;
		}

		public void SaveProp(Prop prop) {
			if (prop.PostalCode.StartsWith("12")) {
				cmdMedia.Parameters["@ListingKey"].Value = prop.ListingKey;
				int recordsFound = 0;

				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					using (SqlCommand cmd = new SqlCommand("UPDATE [listings-residential-onekey] SET LastSeen = GETDATE() WHERE ListingKey = @ListingKey", cn)) {
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25).Value = prop.ListingKey;
						cmd.Connection.Open();
						recordsFound = cmd.ExecuteNonQuery();
						cmd.Connection.Close();
					}

					string PhotosChangeTimestamp = "";
					string SourceSystemModificationTimestamp = "";

					if (recordsFound > 0) {
						//Record is already in the database. Get timestamps
						cmdExisting.Connection = cn;
						cmdExisting.Parameters["@ListingKey"].Value = prop.ListingKey;
						cmdExisting.Connection.Open();
						SqlDataReader dr = cmdExisting.ExecuteReader();
						if (dr.HasRows) {
							dr.Read();
							PhotosChangeTimestamp = dr[0].ToString();
							SourceSystemModificationTimestamp = dr[1].ToString();
						}
						cmdExisting.Connection.Close();
					}

					if (SourceSystemModificationTimestamp != prop.ONEKEY_SourceSystemModificationTimestamp) {
						//Property record has been changed. Delete and re-insert
						if (!string.IsNullOrEmpty(SourceSystemModificationTimestamp)) {
							//There is an existing record
							cmdDeleteProp.Connection = cn;
							cmdDeleteProp.Parameters["@ListingKey"].Value = prop.ListingKey;
							cmdDeleteProp.Connection.Open();
							cmdDeleteProp.ExecuteNonQuery();
							cmdDeleteProp.Connection.Close();
						}

						cmdProp.Connection = cn;
						cmdProp.Parameters["@AccessibilityFeatures"].Value = string.Join(", ", prop.AccessibilityFeatures);
						cmdProp.Parameters["@Appliances"].Value = string.Join(", ", prop.Appliances);
						cmdProp.Parameters["@ArchitecturalStyle"].Value = string.Join(", ", prop.ArchitecturalStyle);
						cmdProp.Parameters["@AssociationAmenities"].Value = string.Join(", ", prop.AssociationAmenities);
						if (prop.AssociationFee != null) {
							cmdProp.Parameters["@AssociationFee"].Value = prop.AssociationFee;
						} else {
							cmdProp.Parameters["@AssociationFee"].Value = 0;
						}
						cmdProp.Parameters["@AssociationFeeFrequency"].Value = prop.AssociationFeeFrequency + "";
						cmdProp.Parameters["@AssociationFeeIncludes"].Value = string.Join(", ", prop.AssociationFeeIncludes);
						if (prop.AssociationYN != null) {
							cmdProp.Parameters["@AssociationYN"].Value = prop.AssociationYN;
						} else {
							cmdProp.Parameters["@AssociationYN"].Value = 0;
						}
						if (prop.AttachedGarageYN != null) {
							cmdProp.Parameters["@AttachedGarageYN"].Value = prop.AttachedGarageYN;
						} else {
							cmdProp.Parameters["@AttachedGarageYN"].Value = 0;
						}
						cmdProp.Parameters["@Basement"].Value = string.Join(", ", prop.Basement);
						cmdProp.Parameters["@BathroomsFull"].Value = prop.BathroomsFull != null ? (int)prop.BathroomsFull : SqlInt32.Null;
						cmdProp.Parameters["@BathroomsHalf"].Value = prop.BathroomsHalf != null ? (int)prop.BathroomsHalf : SqlInt32.Null;
						cmdProp.Parameters["@BathroomsTotalinteger"].Value = prop.BathroomsTotalinteger != null ? (int)prop.BathroomsTotalinteger : SqlInt32.Null;
						cmdProp.Parameters["@BedroomsTotal"].Value = prop.BedroomsTotal != null ? (int)prop.BedroomsTotal : SqlInt32.Null;
						if (prop.BuildingAreaTotal != null) {
							cmdProp.Parameters["@BuildingAreaTotal"].Value = prop.BuildingAreaTotal;
						} else {
							cmdProp.Parameters["@BuildingAreaTotal"].Value = 0;
						}
						cmdProp.Parameters["@BuildingAreaUnits"].Value = prop.BuildingAreaUnits + "";
						cmdProp.Parameters["@BuildingFeatures"].Value = string.Join(", ", prop.BuildingFeatures);
						cmdProp.Parameters["@BuildingName"].Value = prop.BuildingName + "";
						cmdProp.Parameters["@BusinessName"].Value = prop.BusinessName + "";
						cmdProp.Parameters["@BusinessType"].Value = string.Join(", ", prop.BusinessType);
						if (prop.CarportYN != null) {
							cmdProp.Parameters["@CarportYN"].Value = prop.CarportYN;
						} else {
							cmdProp.Parameters["@CarportYN"].Value = 0;
						}
						cmdProp.Parameters["@City"].Value = prop.City + "";
						cmdProp.Parameters["@CloseDate"].Value = !string.IsNullOrEmpty(prop.CloseDate.ToString()) ? (DateTime)prop.CloseDate : SqlDateTime.Null;
						cmdProp.Parameters["@CommunityFeatures"].Value = string.Join(", ", prop.CommunityFeatures);
						cmdProp.Parameters["@ConstructionMaterials"].Value = string.Join(", ", prop.ConstructionMaterials);
						cmdProp.Parameters["@Cooling"].Value = string.Join(", ", prop.Cooling);
						if (prop.CoolingYN != null) {
							cmdProp.Parameters["@CoolingYN"].Value = prop.CoolingYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@CoolingYN"].Value = 0;
						}
						cmdProp.Parameters["@CrossStreet"].Value = prop.CrossStreet + "";
						cmdProp.Parameters["@CurrentUse"].Value = string.Join(", ", prop.CurrentUse);
						cmdProp.Parameters["@DaysOnMarket"].Value = prop.DaysOnMarket != null ? (int)prop.DaysOnMarket : SqlInt32.Null;
						cmdProp.Parameters["@DevelopmentStatus"].Value = string.Join(", ", prop.DevelopmentStatus);
						cmdProp.Parameters["@DirectionFaces"].Value = prop.DirectionFaces + "";
						cmdProp.Parameters["@Directions"].Value = prop.Directions + "";
						cmdProp.Parameters["@Disclosures"].Value = string.Join(", ", prop.Disclosures);
						cmdProp.Parameters["@DistanceToSchoolsComments"].Value = prop.DistanceToSchoolsComments + "";
						cmdProp.Parameters["@DistanceToShoppingComments"].Value = prop.DistanceToShoppingComments + "";
						cmdProp.Parameters["@DoorFeatures"].Value = string.Join(", ", prop.DoorFeatures);
						cmdProp.Parameters["@Electric"].Value = string.Join(", ", prop.Electric);
						if (prop.ElectricOnPropertyYN != null) {
							cmdProp.Parameters["@ElectricOnPropertyYN"].Value = prop.ElectricOnPropertyYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@ElectricOnPropertyYN"].Value = 0;
						}
						cmdProp.Parameters["@ElementarySchool"].Value = prop.ElementarySchool + "";
						cmdProp.Parameters["@ElementarySchoolDistrict"].Value = prop.ElementarySchoolDistrict + "";
						cmdProp.Parameters["@EntryLevel"].Value = prop.EntryLevel != null ? (int)prop.EntryLevel : SqlInt32.Null;
						cmdProp.Parameters["@EntryLocation"].Value = prop.EntryLocation + "";
						cmdProp.Parameters["@Exclusions"].Value = prop.Exclusions + "";
						cmdProp.Parameters["@ExteriorFeatures"].Value = string.Join(", ", prop.ExteriorFeatures);
						cmdProp.Parameters["@Fencing"].Value = string.Join(", ", prop.Fencing);
						cmdProp.Parameters["@FireplaceFeatures"].Value = string.Join(", ", prop.FireplaceFeatures);
						cmdProp.Parameters["@FireplacesTotal"].Value = prop.FireplacesTotal != null ? (int)prop.FireplacesTotal : SqlInt32.Null;
						if (prop.FireplaceYN != null) {
							cmdProp.Parameters["@FireplaceYN"].Value = prop.FireplaceYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@FireplaceYN"].Value = 0;
						}
						cmdProp.Parameters["@Flooring"].Value = string.Join(", ", prop.Flooring);
						if (prop.GarageSpaces != null) {
							cmdProp.Parameters["@GarageSpaces"].Value = prop.GarageSpaces;
						} else {
							cmdProp.Parameters["@GarageSpaces"].Value = 0;
						}
						if (prop.GarageYN != null) {
							cmdProp.Parameters["@GarageYN"].Value = prop.GarageYN;
						} else {
							cmdProp.Parameters["@GarageYN"].Value = 0;
						}
						if (prop.HabitableResidenceYN != null) {
							cmdProp.Parameters["@HabitableResidenceYN"].Value = prop.HabitableResidenceYN;
						} else {
							cmdProp.Parameters["@HabitableResidenceYN"].Value = 0;
						}
						cmdProp.Parameters["@Heating"].Value = string.Join(", ", prop.Heating);
						cmdProp.Parameters["@HighSchool"].Value = prop.HighSchool + "";
						cmdProp.Parameters["@HighSchoolDistrict"].Value = prop.HighSchoolDistrict + "";
						cmdProp.Parameters["@HorseAmenities"].Value = string.Join(", ", prop.HorseAmenities);
						if (prop.HorseYN != null) {
							cmdProp.Parameters["@HorseYN"].Value = prop.HorseYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@HorseYN"].Value = 0;
						}
						cmdProp.Parameters["@Inclusions"].Value = prop.Inclusions + "";
						cmdProp.Parameters["@InteriorFeatures"].Value = string.Join(", ", prop.InteriorFeatures);
						if (prop.InternetAddressDisplayYN != null) {
							cmdProp.Parameters["@InternetAddressDisplayYN"].Value = prop.InternetAddressDisplayYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@InternetAddressDisplayYN"].Value = 0;
						}
						if (prop.InternetEntireListingDisplayYN != null) {
							cmdProp.Parameters["@InternetEntireListingDisplayYN"].Value = prop.InternetEntireListingDisplayYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@InternetEntireListingDisplayYN"].Value = 0;
						}
						cmdProp.Parameters["@LaundryFeatures"].Value = string.Join(", ", prop.LaundryFeatures);
						cmdProp.Parameters["@Levels"].Value = string.Join(", ", prop.Levels);
						cmdProp.Parameters["@ListAgentEmail"].Value = prop.ListAgentEmail + "";
						cmdProp.Parameters["@ListAgentFullName"].Value = prop.ListAgentFullName + "";
						cmdProp.Parameters["@ListAgentMlsId"].Value = prop.ListAgentMlsId + "";
						cmdProp.Parameters["@ListAgentPreferredPhone"].Value = prop.ListAgentPreferredPhone + "";
						cmdProp.Parameters["@ListingKey"].Value = prop.ListingKey + "";
						cmdProp.Parameters["@ListOfficeName"].Value = prop.ListOfficeName + "";
						if (prop.ListPrice != null) {
							cmdProp.Parameters["@ListPrice"].Value = prop.ListPrice;
						} else {
							cmdProp.Parameters["@ListPrice"].Value = 0;
						}
						if (prop.LivingArea != null) {
							cmdProp.Parameters["@LivingArea"].Value = prop.LivingArea;
						} else {
							cmdProp.Parameters["@LivingArea"].Value = 0;
						}
						cmdProp.Parameters["@LivingAreaUnits"].Value = prop.LivingAreaUnits + "";
						cmdProp.Parameters["@LotDimensionsSource"].Value = prop.LotDimensionsSource + "";
						cmdProp.Parameters["@LotFeatures"].Value = string.Join(", ", prop.LotFeatures);
						if (prop.LotSizeAcres != null) {
							cmdProp.Parameters["@LotSizeAcres"].Value = prop.LotSizeAcres;
						} else {
							cmdProp.Parameters["@LotSizeAcres"].Value = 0;
						}
						if (prop.LotSizeArea != null) {
							cmdProp.Parameters["@LotSizeArea"].Value = prop.LotSizeArea;
						} else {
							cmdProp.Parameters["@LotSizeArea"].Value = 0;
						}
						cmdProp.Parameters["@LotSizeDimensions"].Value = prop.LotSizeDimensions + "";
						cmdProp.Parameters["@LotSizeSource"].Value = prop.LotSizeSource + "";
						if (prop.LotSizeSquareFeet != null) {
							cmdProp.Parameters["@LotSizeSquareFeet"].Value = prop.LotSizeSquareFeet;
						} else {
							cmdProp.Parameters["@LotSizeSquareFeet"].Value = 0;
						}
						cmdProp.Parameters["@LotSizeUnits"].Value = prop.LotSizeUnits + "";
						cmdProp.Parameters["@MiddleOrJuniorSchool"].Value = prop.MiddleOrJuniorSchool + "";
						cmdProp.Parameters["@MiddleOrJuniorSchoolDistrict"].Value = prop.MiddleOrJuniorSchoolDistrict + "";
						if (prop.ONEKEY_AdditionalFeesYN != null) {
							cmdProp.Parameters["@NewConstructionYN"].Value = prop.NewConstructionYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@NewConstructionYN"].Value = 0;
						}
						cmdProp.Parameters["@ONEKEY_AdditionalFeeDescription"].Value = prop.ONEKEY_AdditionalFeeDescription + "";
						cmdProp.Parameters["@ONEKEY_AdditionalFeeFrequency"].Value = string.Join(", ", prop.ONEKEY_AdditionalFeeFrequency);
						if (prop.ONEKEY_AdditionalFeesYN != null) {
							cmdProp.Parameters["@ONEKEY_AdditionalFeesYN"].Value = prop.ONEKEY_AdditionalFeesYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@ONEKEY_AdditionalFeesYN"].Value = 0;
						}
						cmdProp.Parameters["@ONEKEY_AlternateMlsNumber"].Value = prop.ONEKEY_AlternateMlsNumber + "";
						cmdProp.Parameters["@ONEKEY_AtticDescription"].Value = string.Join(", ", prop.ONEKEY_AtticDescription);
						cmdProp.Parameters["@ONEKEY_BuildingDimensions"].Value = prop.ONEKEY_BuildingDimensions + "";
						cmdProp.Parameters["@ONEKEY_BuildingLocation"].Value = prop.ONEKEY_BuildingLocation + "";
						cmdProp.Parameters["@ONEKEY_BuildingSize"].Value = prop.ONEKEY_BuildingSize + "";
						cmdProp.Parameters["@ONEKEY_BusinessAge"].Value = prop.ONEKEY_BusinessAge + "";
						cmdProp.Parameters["@ONEKEY_BusinessLocatedAt"].Value = prop.ONEKEY_BusinessLocatedAt + "";
						cmdProp.Parameters["@ONEKEY_DescriptionOfBusiness"].Value = prop.ONEKEY_DescriptionOfBusiness + "";
						cmdProp.Parameters["@ONEKEY_DevelopmentName"].Value = prop.ONEKEY_DevelopmentName + "";
						cmdProp.Parameters["@ONEKEY_GarbageRemoval"].Value = string.Join(", ", prop.ONEKEY_GarbageRemoval);
						cmdProp.Parameters["@ONEKEY_Hamlet"].Value = string.Join(", ", prop.ONEKEY_Hamlet);
						cmdProp.Parameters["@ONEKEY_HeatingNotes"].Value = prop.ONEKEY_HeatingNotes + "";
						cmdProp.Parameters["@ONEKEY_HotWater"].Value = string.Join(", ", prop.ONEKEY_HotWater);
						cmdProp.Parameters["@ONEKEY_ImprovementRemarks"].Value = prop.ONEKEY_ImprovementRemarks + "";
						cmdProp.Parameters["@ONEKEY_LivingQuartersDescription"].Value = string.Join(", ", prop.ONEKEY_LivingQuartersDescription);
						cmdProp.Parameters["@ONEKEY_LoadingDocks"].Value = prop.ONEKEY_LoadingDocks + "";
						if (prop.ONEKEY_LoadingDockYN != null) {
							cmdProp.Parameters["@ONEKEY_LoadingDockYN"].Value = prop.ONEKEY_LoadingDockYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@ONEKEY_LoadingDockYN"].Value = 0;
						}
						cmdProp.Parameters["@ONEKEY_LocationDescription"].Value = string.Join(", ", prop.ONEKEY_LocationDescription);
						cmdProp.Parameters["@ONEKEY_MinimumAge"].Value = prop.ONEKEY_MinimumAge + "";
						cmdProp.Parameters["@ONEKEY_NumberOfHeatingUnits"].Value = prop.ONEKEY_NumberOfHeatingUnits + "";
						cmdProp.Parameters["@ONEKEY_NumberofHeatingZones"].Value = prop.ONEKEY_NumberofHeatingZones + "";
						cmdProp.Parameters["@ONEKEY_Plumbing"].Value = string.Join(", ", prop.ONEKEY_Plumbing); ;
						cmdProp.Parameters["@ONEKEY_SourceSystemModificationTimestamp"].Value = prop.ONEKEY_SourceSystemModificationTimestamp + "";
						cmdProp.Parameters["@ONEKEY_VideoTourURL"].Value = prop.ONEKEY_VideoTourURL + "";
						cmdProp.Parameters["@ONEKEY_Village"].Value = string.Join(", ", prop.ONEKEY_Village);
						cmdProp.Parameters["@OnMarketDate"].Value = !string.IsNullOrEmpty(prop.OnMarketDate.ToString()) ? (DateTime)prop.OnMarketDate : SqlDateTime.Null;
						cmdProp.Parameters["@OriginatingSystemKey"].Value = prop.OriginatingSystemKey + "";
						cmdProp.Parameters["@OtherStructures"].Value = string.Join(", ", prop.OtherStructures);
						cmdProp.Parameters["@OwnerPays"].Value = string.Join(", ", prop.OwnerPays);
						cmdProp.Parameters["@Ownership"].Value = prop.Ownership + "";
						cmdProp.Parameters["@ParcelNumber"].Value = prop.ParcelNumber + "";
						cmdProp.Parameters["@ParkingFeatures"].Value = string.Join(", ", prop.ParkingFeatures);
						if (prop.ParkingTotal != null) {
							cmdProp.Parameters["@ParkingTotal"].Value = prop.ParkingTotal;
						} else {
							cmdProp.Parameters["@ParkingTotal"].Value = 0;
						}
						cmdProp.Parameters["@PatioAndPorchFeatures"].Value = string.Join(", ", prop.PatioAndPorchFeatures);
						cmdProp.Parameters["@PhotosChangeTimestamp"].Value = prop.PhotosChangeTimestamp + "";
						cmdProp.Parameters["@PhotosCount"].Value = prop.PhotosCount != null ? (int)prop.PhotosCount : SqlInt32.Null;
						cmdProp.Parameters["@PoolFeatures"].Value = string.Join(", ", prop.PoolFeatures);
						if (prop.PoolPrivateYN != null) {
							cmdProp.Parameters["@PoolPrivateYN"].Value = prop.PoolPrivateYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@PoolPrivateYN"].Value = 0;
						}
						cmdProp.Parameters["@PossibleUse"].Value = string.Join(", ", prop.PossibleUse);
						cmdProp.Parameters["@PostalCode"].Value = prop.PostalCode + "";
						cmdProp.Parameters["@PropertyCondition"].Value = string.Join(", ", prop.PropertyCondition);
						cmdProp.Parameters["@PropertySubType"].Value = prop.PropertySubType + "";
						cmdProp.Parameters["@PropertyType"].Value = prop.PropertyType + "";
						cmdProp.Parameters["@Roof"].Value = prop.Roof + "";
						cmdProp.Parameters["@RoomsTotal"].Value = prop.RoomsTotal != null ? (int)prop.RoomsTotal : SqlInt32.Null;
						cmdProp.Parameters["@SecurityFeatures"].Value = string.Join(", ", prop.SecurityFeatures);
						if (prop.SeniorCommunityYN != null) {
							cmdProp.Parameters["@SeniorCommunityYN"].Value = prop.SeniorCommunityYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@SeniorCommunityYN"].Value = 0;
						}
						cmdProp.Parameters["@Sewer"].Value = string.Join(", ", prop.Sewer);
						cmdProp.Parameters["@SpaFeatures"].Value = string.Join(", ", prop.SpaFeatures);
						if (prop.SpaYN != null) {
							cmdProp.Parameters["@SpaYN"].Value = prop.SpaYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@SpaYN"].Value = 0;
						}
						cmdProp.Parameters["@SpecialListingConditions"].Value = string.Join(", ", prop.SpecialListingConditions);
						cmdProp.Parameters["@StoriesTotal"].Value = prop.StoriesTotal != null ? (int)prop.StoriesTotal : SqlInt32.Null;
						cmdProp.Parameters["@StreetDirPrefix"].Value = prop.StreetDirPrefix + "";
						cmdProp.Parameters["@StreetDirSuffix"].Value = prop.StreetDirSuffix + "";
						cmdProp.Parameters["@StreetName"].Value = prop.StreetName + "";
						cmdProp.Parameters["@StreetNumber"].Value = prop.StreetNumber + "";
						cmdProp.Parameters["@StreetSuffix"].Value = prop.StreetSuffix + "";
						cmdProp.Parameters["@StructureType"].Value = prop.StructureType + "";
						cmdProp.Parameters["@SubdivisionName"].Value = prop.SubdivisionName + "";
						cmdProp.Parameters["@SyndicationRemarks"].Value = prop.SyndicationRemarks + "";
						if (prop.TaxAnnualAmount != null) {
							cmdProp.Parameters["@TaxAnnualAmount"].Value = prop.TaxAnnualAmount;
						} else {
							cmdProp.Parameters["@TaxAnnualAmount"].Value = 0;
						}
						if (prop.TaxAssessedValue != null) {
							cmdProp.Parameters["@TaxAssessedValue"].Value = prop.TaxAssessedValue;
						} else {
							cmdProp.Parameters["@TaxAssessedValue"].Value = 0;
						}
						cmdProp.Parameters["@TaxBlock"].Value = prop.TaxBlock + "";
						cmdProp.Parameters["@TaxLot"].Value = prop.TaxLot + "";
						cmdProp.Parameters["@TaxMapNumber"].Value = prop.TaxMapNumber + "";
						cmdProp.Parameters["@TenantPays"].Value = string.Join(", ", prop.TenantPays);
						cmdProp.Parameters["@Township"].Value = prop.Township + "";
						if (prop.TrashExpense != null) {
							cmdProp.Parameters["@TrashExpense"].Value = prop.TrashExpense;
						} else {
							cmdProp.Parameters["@TrashExpense"].Value = 0;
						}
						cmdProp.Parameters["@UnitNumber"].Value = prop.UnitNumber + "";
						cmdProp.Parameters["@UnparsedAddress"].Value = prop.UnparsedAddress + "";
						cmdProp.Parameters["@Utilities"].Value = string.Join(", ", prop.Utilities);
						cmdProp.Parameters["@View"].Value = string.Join(", ", prop.View);
						cmdProp.Parameters["@VirtualTourURLBranded"].Value = prop.VirtualTourURLBranded + "";
						cmdProp.Parameters["@VirtualTourURLUnbranded"].Value = prop.VirtualTourURLUnbranded + "";
						cmdProp.Parameters["@WaterfrontFeatures"].Value = string.Join(", ", prop.WaterfrontFeatures);
						if (prop.WaterfrontYN != null) {
							cmdProp.Parameters["@WaterfrontYN"].Value = prop.WaterfrontYN == true ? 1 : 0;
						} else {
							cmdProp.Parameters["@WaterfrontYN"].Value = 0;
						}
						cmdProp.Parameters["@WaterSource"].Value = string.Join(", ", prop.WaterSource);
						cmdProp.Parameters["@WindowFeatures"].Value = string.Join(", ", prop.WindowFeatures);
						cmdProp.Parameters["@YearBuilt"].Value = prop.YearBuilt != null ? (int)prop.YearBuilt : SqlInt32.Null;
						cmdProp.Parameters["@YearBuiltEffective"].Value = prop.YearBuiltEffective + "";
						cmdProp.Parameters["@YearBuiltSource"].Value = prop.YearBuiltSource + "";
						cmdProp.Parameters["@ZoningDescription"].Value = prop.ZoningDescription + "";

						cmdProp.Connection.Open();
						cmdProp.ExecuteNonQuery();
						cmdProp.Connection.Close();
					}

					if (PhotosChangeTimestamp != prop.PhotosChangeTimestamp) {
						//Media has been changed. Delete and re-insert
						//if (!string.IsNullOrEmpty(PhotosChangeTimestamp)) {
							//There are existing records
							cmdDeleteMedia.Connection = cn;
							cmdDeleteMedia.Parameters["@ListingKey"].Value = prop.ListingKey;
							cmdDeleteMedia.Connection.Open();
							cmdDeleteMedia.ExecuteNonQuery();
							cmdDeleteMedia.Connection.Close();
						//}

						if (prop.Media != null) {
							cmdMedia.Connection = cn;
							foreach (MediaItem m in prop.Media) {
								if (m.MediaCategory.ToLower() == "photo" &&
									!string.IsNullOrEmpty(m.MediaKey) &&
									!string.IsNullOrEmpty(m.MediaURL)) {

									int order = 1;
									if (!Int32.TryParse(m.Order, out order)) { order = 1; }
									cmdMedia.Parameters["@MediaKey"].Value = m.MediaKey;
									cmdMedia.Parameters["@Order"].Value = order;
									cmdMedia.Parameters["@MediaURL"].Value = m.MediaURL;
									cmdMedia.Connection.Open();
									cmdMedia.ExecuteNonQuery();
									cmdMedia.Connection.Close();
								}
							}
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
		}
	}

	public class MLSListing {
		public MLSListing(string MLSNumber) {
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				string SQL = "SELECT " +
						   "[AccessibilityFeatures]," +
						   "[Appliances]," +
						   "[ArchitecturalStyle]," +
						   "[AssociationAmenities]," +
						   "[AssociationFee]," +
						   "[AssociationFeeFrequency]," +
						   "[AssociationFeeIncludes]," +
						   "[AssociationYN]," +
						   "[AttachedGarageYN]," +
						   "[Basement]," +
						   "[BathroomsFull]," +
						   "[BathroomsHalf]," +
						   "[BathroomsTotalinteger]," +
						   "[BedroomsTotal]," +
						   "[BuildingAreaTotal]," +
						   "[BuildingAreaUnits]," +
						   "[BuildingFeatures]," +
						   "[BuildingName]," +
						   "[BusinessName]," +
						   "[BusinessType]," +
						   "[CarportYN]," +
						   "[City]," +
						   "[CloseDate]," +
						   "[CommunityFeatures]," +
						   "[ConstructionMaterials]," +
						   "[Cooling]," +
						   "[CoolingYN]," +
						   "[CrossStreet]," +
						   "[CurrentUse]," +
						   "[DaysOnMarket]," +
						   "[DevelopmentStatus]," +
						   "[DirectionFaces]," +
						   "[Directions]," +
						   "[Disclosures]," +
						   "[DistanceToSchoolsComments]," +
						   "[DistanceToShoppingComments]," +
						   "[DoorFeatures]," +
						   "[Electric]," +
						   "[ElectricOnPropertyYN]," +
						   "[ElementarySchool]," +
						   "[ElementarySchoolDistrict]," +
						   "[EntryLevel]," +
						   "[EntryLocation]," +
						   "[Exclusions]," +
						   "[ExteriorFeatures]," +
						   "[Fencing]," +
						   "[FireplaceFeatures]," +
						   "[FireplacesTotal]," +
						   "[FireplaceYN]," +
						   "[Flooring]," +
						   "[GarageSpaces]," +
						   "[GarageYN]," +
						   "[HabitableResidenceYN]," +
						   "[Heating]," +
						   "[HighSchool]," +
						   "[HighSchoolDistrict]," +
						   "[HorseAmenities]," +
						   "[HorseYN]," +
						   "[Inclusions]," +
						   "[InteriorFeatures]," +
						   "[InternetAddressDisplayYN]," +
						   "[InternetEntireListingDisplayYN]," +
						   "[LaundryFeatures]," +
						   "[Levels]," +
						   "[ListAgentEmail]," +
						   "[ListAgentFullName]," +
						   "[ListAgentMlsId]," +
						   "[ListAgentPreferredPhone]," +
						   "[ListingKey]," +
						   "[ListOfficeName]," +
						   "[ListPrice]," +
						   "[LivingArea]," +
						   "[LivingAreaUnits]," +
						   "[LotDimensionsSource]," +
						   "[LotFeatures]," +
						   "[LotSizeAcres]," +
						   "[LotSizeArea]," +
						   "[LotSizeDimensions]," +
						   "[LotSizeSource]," +
						   "[LotSizeSquareFeet]," +
						   "[LotSizeUnits]," +
						   "[MiddleOrJuniorSchool]," +
						   "[MiddleOrJuniorSchoolDistrict]," +
						   "[NewConstructionYN]," +
						   "[ONEKEY_AdditionalFeeDescription]," +
						   "[ONEKEY_AdditionalFeeFrequency]," +
						   "[ONEKEY_AdditionalFeesYN]," +
						   "[ONEKEY_AlternateMlsNumber]," +
						   "[ONEKEY_AtticDescription]," +
						   "[ONEKEY_BuildingDimensions]," +
						   "[ONEKEY_BuildingLocation]," +
						   "[ONEKEY_BuildingSize]," +
						   "[ONEKEY_BusinessAge]," +
						   "[ONEKEY_BusinessLocatedAt]," +
						   "[ONEKEY_DescriptionOfBusiness]," +
						   "[ONEKEY_DevelopmentName]," +
						   "[ONEKEY_GarbageRemoval]," +
						   "[ONEKEY_Hamlet]," +
						   "[ONEKEY_HeatingNotes]," +
						   "[ONEKEY_HotWater]," +
						   "[ONEKEY_ImprovementRemarks]," +
						   "[ONEKEY_LivingQuartersDescription]," +
						   "[ONEKEY_LoadingDocks]," +
						   "[ONEKEY_LoadingDockYN]," +
						   "[ONEKEY_LocationDescription]," +
						   "[ONEKEY_MinimumAge]," +
						   "[ONEKEY_NumberOfHeatingUnits]," +
						   "[ONEKEY_NumberofHeatingZones]," +
						   "[ONEKEY_Plumbing]," +
						   "[ONEKEY_SourceSystemModificationTimestamp]," +
						   "[ONEKEY_VideoTourURL]," +
						   "[ONEKEY_Village]," +
						   "[OnMarketDate]," +
						   "[OriginatingSystemKey]," +
						   "[OtherStructures]," +
						   "[OwnerPays]," +
						   "[Ownership]," +
						   "[ParcelNumber]," +
						   "[ParkingFeatures]," +
						   "[ParkingTotal]," +
						   "[PatioAndPorchFeatures]," +
						   "[PhotosChangeTimestamp]," +
						   "[PhotosCount]," +
						   "[PoolFeatures]," +
						   "[PoolPrivateYN]," +
						   "[PossibleUse]," +
						   "[PostalCode]," +
						   "[PropertyCondition]," +
						   "[PropertySubType]," +
						   "[PropertyType]," +
						   "[Roof]," +
						   "[RoomsTotal]," +
						   "[SecurityFeatures]," +
						   "[SeniorCommunityYN]," +
						   "[Sewer]," +
						   "[SpaFeatures]," +
						   "[SpaYN]," +
						   "[SpecialListingConditions]," +
						   "[StoriesTotal]," +
						   "[StreetDirPrefix]," +
						   "[StreetDirSuffix]," +
						   "[StreetName]," +
						   "[StreetNumber]," +
						   "[StreetSuffix]," +
						   "[StructureType]," +
						   "[SubdivisionName]," +
						   "[SyndicationRemarks]," +
						   "[TaxAnnualAmount]," +
						   "[TaxAssessedValue]," +
						   "[TaxBlock]," +
						   "[TaxLot]," +
						   "[TaxMapNumber]," +
						   "[TenantPays]," +
						   "[Township]," +
						   "[TrashExpense]," +
						   "[UnitNumber]," +
						   "[UnparsedAddress]," +
						   "[Utilities]," +
						   "[View]," +
						   "[VirtualTourURLBranded]," +
						   "[VirtualTourURLUnbranded]," +
						   "[WaterfrontFeatures]," +
						   "[WaterfrontYN]," +
						   "[WaterSource]," +
						   "[WindowFeatures]," +
						   "[YearBuilt]," +
						   "[YearBuiltEffective]," +
						   "[YearBuiltSource]," +
						   "[ZoningDescription], ISNULL(l2.[Lat], 0), ISNULL(l2.[Long], 0), " +
						   "z.Town " +
					 "FROM [dbo].[listings-residential-onekey] l1 LEFT JOIN [listings-geo] l2 ON l1.[OriginatingSystemKey] = l2.[MLSNumber] " +
					 "LEFT JOIN ZIP_Town z ON l1.PostalCode = z.ZIPCode " +
					 "WHERE OriginatingSystemKey = @MLSNumber";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add("@MLSNumber", SqlDbType.NVarChar, 1024).Value = MLSNumber;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow);
					if (dr.HasRows) {
						dr.Read();
						AccessibilityFeatures = dr[0].ToString(); // 1024);
						Appliances = dr[1].ToString(); // 1024);
						ArchitecturalStyle = dr[2].ToString(); // 1024);
						AssociationAmenities = dr[3].ToString(); // 1024);
						AssociationFee = dr[4].ToString();
						AssociationFeeFrequency = dr[5].ToString(); // 25);
						AssociationFeeIncludes = dr[6].ToString(); // 1024);
						AssociationYN = dr.GetBoolean(7);
						AttachedGarageYN = dr.GetBoolean(8);
						Basement = dr[9].ToString(); // 1024);
						BathroomsFull = !dr.IsDBNull(10) ? dr.GetInt32(10) : -1;
						BathroomsHalf = !dr.IsDBNull(11) ? dr.GetInt32(11) : -1;
						BathroomsTotalinteger = !dr.IsDBNull(12) ? dr.GetInt32(12) : -1;
						BedroomsTotal = !dr.IsDBNull(13) ? dr.GetInt32(13) : -1;
						BuildingAreaTotal = dr[14].ToString();
						BuildingAreaUnits = dr[15].ToString(); // 25);
						BuildingFeatures = dr[16].ToString(); // 1024);
						BuildingName = dr[17].ToString(); // 255);
						BusinessName = dr[18].ToString(); // 255);
						BusinessType = dr[19].ToString(); // 1024);
						CarportYN = dr.GetBoolean(20);
						//City = dr[21].ToString(); // 50);
						CloseDate = !dr.IsDBNull(22) ? dr.GetDateTime(22) : DateTime.MinValue;
						CommunityFeatures = dr[23].ToString(); // 1024);
						ConstructionMaterials = dr[24].ToString(); // 1024);
						Cooling = dr[25].ToString(); // 1024);
						CoolingYN = dr.GetBoolean(26);
						CrossStreet = dr[27].ToString(); // 50);
						CurrentUse = dr[28].ToString(); // 1024);
						DaysOnMarket = !dr.IsDBNull(29) ? dr.GetInt32(29) : -1;
						DevelopmentStatus = dr[30].ToString(); // 1024);
						DirectionFaces = dr[31].ToString(); // 25);
						Directions = dr[32].ToString(); // 1024);
						Disclosures = dr[33].ToString(); // 1024);
						DistanceToSchoolsComments = dr[34].ToString(); // 255);
						DistanceToShoppingComments = dr[35].ToString(); // 255);
						DoorFeatures = dr[36].ToString(); // 1024);
						Electric = dr[37].ToString(); // 1024);
						ElectricOnPropertyYN = dr.GetBoolean(38);
						ElementarySchool = dr[39].ToString(); // 1024);
						ElementarySchoolDistrict = dr[40].ToString(); // 100);
						EntryLevel = !dr.IsDBNull(41) ? dr.GetInt32(41) : -1;
						EntryLocation = dr[42].ToString(); // 1024);
						Exclusions = dr[43].ToString(); // 1024);
						ExteriorFeatures = dr[44].ToString(); // 1024);
						Fencing = dr[45].ToString(); // 1024);
						FireplaceFeatures = dr[46].ToString(); // 1024);
						FireplacesTotal = !dr.IsDBNull(47) ? dr.GetInt32(47) : -1;
						FireplaceYN = dr.GetBoolean(48);
						Flooring = dr[49].ToString(); // 1024);
						GarageSpaces = dr[50].ToString();
						GarageYN = dr.GetBoolean(51);
						HabitableResidenceYN = dr.GetBoolean(52);
						Heating = dr[53].ToString(); // 1024);
						HighSchool = dr[54].ToString(); // 150);
						HighSchoolDistrict = dr[55].ToString(); // 150);
						HorseAmenities = dr[56].ToString(); // 1024);
						HorseYN = dr.GetBoolean(57);
						Inclusions = dr[58].ToString(); // 2000);
						InteriorFeatures = dr[59].ToString(); // 1024);
						InternetAddressDisplayYN = dr.GetBoolean(60);
						InternetEntireListingDisplayYN = dr.GetBoolean(61);
						LaundryFeatures = dr[62].ToString(); // 1024);
						Levels = dr[63].ToString(); // 1024);
						ListAgentEmail = dr[64].ToString(); // 100);
						ListAgentFullName = dr[65].ToString(); // 150);
						ListAgentMlsId = dr[66].ToString(); // 25);
						ListAgentPreferredPhone = dr[67].ToString(); // 16);
						ListingKey = dr[68].ToString(); // 25);
						ListOfficeName = dr[69].ToString(); // 255);
						ListPrice = dr[70].ToString();
						LivingArea = dr[71].ToString();
						LivingAreaUnits = dr[73].ToString(); // 25);
						LotDimensionsSource = dr[73].ToString(); // 50);
						LotFeatures = dr[74].ToString(); // 1024);
						LotSizeAcres = dr[75].ToString();
						LotSizeArea = dr[76].ToString();
						LotSizeDimensions = dr[77].ToString(); // 150);
						LotSizeSource = dr[78].ToString(); // 50);
						LotSizeSquareFeet = dr[79].ToString();
						LotSizeUnits = dr[80].ToString(); // 25);
						MiddleOrJuniorSchool = dr[81].ToString(); // 150);
						MiddleOrJuniorSchoolDistrict = dr[82].ToString(); // 150);
						NewConstructionYN = dr.GetBoolean(83);
						ONEKEY_AdditionalFeeDescription = dr[84].ToString(); // 1024);
						ONEKEY_AdditionalFeeFrequency = dr[85].ToString(); // 1024);
						ONEKEY_AdditionalFeesYN = dr.GetBoolean(86);
						ONEKEY_AlternateMlsNumber = dr[87].ToString(); // 1024);
						ONEKEY_AtticDescription = dr[88].ToString(); // 1024);
						ONEKEY_BuildingDimensions = dr[89].ToString(); // 1024);
						ONEKEY_BuildingLocation = dr[90].ToString(); // 1024);
						ONEKEY_BuildingSize = dr[91].ToString(); // 1024);
						ONEKEY_BusinessAge = dr[92].ToString(); // 1024);
						ONEKEY_BusinessLocatedAt = dr[93].ToString(); // 1024);
						ONEKEY_DescriptionOfBusiness = dr[94].ToString(); // 1024);
						ONEKEY_DevelopmentName = dr[95].ToString(); // 1024);
						ONEKEY_GarbageRemoval = dr[96].ToString(); // 1024);
						ONEKEY_Hamlet = dr[97].ToString(); // 1024);
						ONEKEY_HeatingNotes = dr[98].ToString(); // 1024);
						ONEKEY_HotWater = dr[99].ToString(); // 1024);
						ONEKEY_ImprovementRemarks = dr[100].ToString(); // 1024);
						ONEKEY_LivingQuartersDescription = dr[101].ToString(); // 1024);
						ONEKEY_LoadingDocks = dr[102].ToString(); // 1024);
						ONEKEY_LoadingDockYN = dr.GetBoolean(103);
						ONEKEY_LocationDescription = dr[104].ToString(); // 1024);
						ONEKEY_MinimumAge = dr[105].ToString(); // 1024);
						ONEKEY_NumberOfHeatingUnits = dr[106].ToString(); // 1024);
						ONEKEY_NumberofHeatingZones = dr[107].ToString(); // 1024);
						ONEKEY_Plumbing = dr[108].ToString(); // 1024);
						ONEKEY_SourceSystemModificationTimestamp = dr[109].ToString(); // 1024);
						ONEKEY_VideoTourURL = dr[110].ToString();
						ONEKEY_Village = dr[111].ToString(); // 1024);
						OnMarketDate = !dr.IsDBNull(112) ? dr.GetDateTime(112) : DateTime.MinValue;
						OriginatingSystemKey = dr[113].ToString(); // 1024);
						OtherStructures = dr[114].ToString(); // 1024);
						OwnerPays = dr[115].ToString(); // 1024);
						Ownership = dr[116].ToString(); // 1024);
						ParcelNumber = dr[117].ToString(); // 50);
						ParkingFeatures = dr[118].ToString(); // 1024);
						ParkingTotal = dr[119].ToString();
						PatioAndPorchFeatures = dr[120].ToString(); // 1024);
						PhotosChangeTimestamp = dr[121].ToString(); // 50);
						PhotosCount = !dr.IsDBNull(122) ? dr.GetInt32(122) : -1;
						PoolFeatures = dr[123].ToString(); // 1024);
						PoolPrivateYN = dr.GetBoolean(124);
						PossibleUse = dr[125].ToString(); // 1024);
						PostalCode = dr[126].ToString(); // 10);
						PropertyCondition = dr[127].ToString(); // 1024);
						PropertySubType = dr[128].ToString(); // 50);
						PropertyType = dr[129].ToString(); // 50);
						Roof = dr[130].ToString(); // 1024);
						RoomsTotal = !dr.IsDBNull(131) ? dr.GetInt32(131) : -1;
						SecurityFeatures = dr[132].ToString(); // 1024);
						SeniorCommunityYN = dr.GetBoolean(133);
						Sewer = dr[134].ToString(); // 1024);
						SpaFeatures = dr[135].ToString(); // 1024);
						SpaYN = dr.GetBoolean(136);
						SpecialListingConditions = dr[137].ToString(); // 1024);
						StoriesTotal = !dr.IsDBNull(138) ? dr.GetInt32(138) : -1;
						StreetDirPrefix = dr[139].ToString(); // 15);
						StreetDirSuffix = dr[140].ToString(); // 15);
						StreetName = dr[141].ToString(); // 50);
						StreetNumber = dr[142].ToString(); // 25);
						StreetSuffix = dr[143].ToString(); // 25);
						StructureType = dr[144].ToString(); // 1024);
						SubdivisionName = dr[145].ToString(); // 50);
						SyndicationRemarks = dr[146].ToString(); // 4000);
						TaxAnnualAmount = dr[147].ToString();
						TaxAssessedValue = dr[148].ToString();
						TaxBlock = dr[149].ToString(); // 25);
						TaxLot = dr[150].ToString(); // 25);
						TaxMapNumber = dr[151].ToString(); // 50);
						TenantPays = dr[152].ToString(); // 1024);
						Township = dr[153].ToString(); // 50);
						TrashExpense = dr[154].ToString();
						UnitNumber = dr[155].ToString(); // 25);
						UnparsedAddress = dr[156].ToString(); // 255);
						Utilities = dr[157].ToString(); // 1024);
						View = dr[158].ToString(); // 1024);
						VirtualTourURLBranded = dr[159].ToString();
						VirtualTourURLUnbranded = dr[160].ToString();
						WaterfrontFeatures = dr[161].ToString(); // 1024);
						WaterfrontYN = dr.GetBoolean(162);
						WaterSource = dr[163].ToString(); // 1024);
						WindowFeatures = dr[164].ToString(); // 1024);
						YearBuilt = !dr.IsDBNull(165) ? dr.GetInt32(165) : -1;
						YearBuiltEffective = dr[166].ToString(); // 11);
						YearBuiltSource = dr[167].ToString(); // 60);
						ZoningDescription = dr[168].ToString(); // 255);
						Lat = (float)dr.GetDouble(169);
						Long = (float)dr.GetDouble(170);
						if (!dr.IsDBNull(171)) {
							City = dr[171].ToString();
						} else {
							City = dr[21].ToString();
						}
					}
					cmd.Connection.Close();

					Images = new List<string>();
					if (!string.IsNullOrEmpty(ListingKey)) {
						cmd.CommandText = "SELECT MediaURL FROM [listings-media-onekey] WHERE ListingKey = @ListingKey ORDER BY [Order]";
						cmd.Parameters.Clear();
						cmd.Parameters.Add("@ListingKey", SqlDbType.NVarChar, 25).Value = ListingKey;
						cmd.Connection.Open();
						dr = cmd.ExecuteReader();
						while (dr.Read()) {
							Images.Add(dr[0].ToString());
						}
						cmd.Connection.Close();
					}
				}
			}
		}

		#region Public Methods
		public string GetTags() {
			StringBuilder sb = new StringBuilder();
			string URL = Global.BaseURL + "/detail?mls=" + MLS;

			//Meta tags
			sb.AppendLine(string.Format("<title>{0}</title>", this.UnparsedAddress + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta name='description' content=\"{0}\" />", this.ShortDescription(150)));

			//OG / Facebook tags
			sb.AppendLine(string.Format("<meta property='og:title' content=\"{0}\" />", this.UnparsedAddress + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta property='og:description' content=\"{0}\" />", this.ShortDescription(150)));
			sb.AppendLine("<meta property='og:type' content='website' />");
			sb.AppendLine(string.Format("<meta property='og:url' content=\"{0}\" />", URL));
			//sb.AppendLine(string.Format("<meta property='og:image' content=\"{0}\" />", Global.BaseURL + this.ogImage));
			sb.AppendLine(string.Format("<meta property='fb:admins' content=\"{0}\" />", Global.FacebookAdmin));

			//Google+ tags
			//if (!string.IsNullOrEmpty(this.gAuthor)) { sb.AppendLine(string.Format("<link rel='author' href=\"{0}\" />", this.gAuthor)); }
			//if (!string.IsNullOrEmpty(this.gPublisher)) { sb.AppendLine(string.Format("<link rel='publisher' href=\"{0}\" />", this.gPublisher)); }
			sb.AppendLine(string.Format("<meta itemprop='name' content=\"{0}\" />", this.UnparsedAddress + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
			sb.AppendLine(string.Format("<meta itemprop='description' content=\"{0}\" />", this.ShortDescription(150)));
			//if (!string.IsNullOrEmpty(this.gImage)) {
			//	sb.AppendLine(string.Format("<meta itemprop='image' content=\"{0}\" />", Global.BaseURL + this.gImage));
			//} else if (!string.IsNullOrEmpty(this.ogImage)) {
			//	sb.AppendLine(string.Format("<meta itemprop='image' content=\"{0}\" />", Global.BaseURL + this.gImage));
			//}

			//Twitter tags
			sb.AppendLine("<meta name='twitter:card' content='summary'>");
			sb.AppendLine(string.Format("<meta name='twitter:title' content=\"{0}\" />", this.UnparsedAddress + " For Sale " + Global.TitleSuffixSep + Global.TitleSuffix));
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
			return TruncateAtWord(SyndicationRemarks, words);
		}
		private string TruncateAtWord(string value, int length) {
			if (value == null || value.Length < length || value.IndexOf(" ", length) == -1)
				return value;

			return value.Substring(0, value.IndexOf(" ", length));
		}
		#endregion

		public float DownpaymentPercent { get; set; } = 0;
		public float Percent30yr { get; set; } = 0;
		public float Percent15yr { get; set; } = 0;
		public float PercentARM { get; set; } = 0;
		public string MLS { get { return OriginatingSystemKey; } }
		public float AskingAmt {
			get {
				float f = 0;
				if (float.TryParse(ListPrice, out f)) {
					return f;
				} else {
					return 0;
				}
			}
		}
		public string TotalBaths {
			get {
				if (BathroomsFull > -1 && BathroomsHalf > -1) {
					return BathroomsFull.ToString() + " full, " + BathroomsHalf.ToString() + " half";
				} else if (BathroomsFull > -1) {
					return BathroomsFull.ToString() + " full";
				} else if (BathroomsHalf > -1) {
					return BathroomsHalf.ToString() + " half";
				} else {
					return "";
				}
			}
		}
		public string Acres {
			get {
				if (!string.IsNullOrEmpty(LotSizeSquareFeet)) {
					float f = 0;
					if (float.TryParse(LotSizeSquareFeet, out f)) {
						return (f / 43560.0).ToString("#,###.00");
					} else {
						return "";
					}
				} else if (!string.IsNullOrEmpty(LotSizeAcres) && LotSizeAcres != "0") {
					return LotSizeAcres;
				} else {
					return "";
				}
			}
		}
		public string Downpayment {
			get {
				float f = 0;
				if (float.TryParse(ListPrice, out f) && DownpaymentPercent > 0) {
					return string.Format("{0:$#,###}", f * DownpaymentPercent / 100);
				} else {
					return "";
				}
			}
		}
		public string Payment30yr {
			get {
				float f = 0;
				if (float.TryParse(ListPrice, out f) && Percent30yr > 0) {
					PaymentCalculator calculator = new PaymentCalculator();
					if (DownpaymentPercent > 0) {
						calculator.DownPayment = Convert.ToDecimal(f) * Convert.ToDecimal(DownpaymentPercent) / 100;
					}
					calculator.PurchasePrice = Convert.ToDecimal(f);
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
				float f = 0;
				if (float.TryParse(ListPrice, out f) && Percent15yr > 0) {
					PaymentCalculator calculator = new PaymentCalculator();
					if (DownpaymentPercent > 0) {
						calculator.DownPayment = Convert.ToDecimal(f) * Convert.ToDecimal(DownpaymentPercent) / 100;
					}
					calculator.PurchasePrice = Convert.ToDecimal(f);
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
				float f = 0;
				if (float.TryParse(ListPrice, out f) && PercentARM > 0) {
					PaymentCalculator calculator = new PaymentCalculator();
					if (DownpaymentPercent > 0) {
						calculator.DownPayment = Convert.ToDecimal(f) * Convert.ToDecimal(DownpaymentPercent) / 100;
					}
					calculator.PurchasePrice = Convert.ToDecimal(f);
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
		public List<string> Images { get; set; }
		public string AddressStreet {
			get {
				return StreetNumber + " " + StreetDirPrefix + " " + StreetName + " " + StreetSuffix + " " + StreetDirSuffix;
			}
		}

		public float Lat { get; set; }
		public float Long { get; set; }

		public string AccessibilityFeatures { get; set; }
		public string Appliances { get; set; }
		public string ArchitecturalStyle { get; set; } //Style
		public string AssociationAmenities { get; set; } //Amenities
		public string AssociationFee { get; set; } //AdditionalFeesAmt
		public string AssociationFeeFrequency { get; set; } //AddFeeFrequency
		public string AssociationFeeIncludes { get; set; } //AdditionalFeeDes
		public bool AssociationYN { get; set; }
		public bool AttachedGarageYN { get; set; }
		public string Basement { get; set; } //BasementDescription
		public int BathroomsFull { get; set; } //BathsFull
		public int BathroomsHalf { get; set; } //BathsHalf
		public int BathroomsTotalinteger { get; set; } //BathsTotal
		public int BedroomsTotal { get; set; } //BedsTotal
		public string BuildingAreaTotal { get; set; }
		public string BuildingAreaUnits { get; set; }
		public string BuildingFeatures { get; set; }
		public string BuildingName { get; set; }
		public string BusinessName { get; set; }
		public string BusinessType { get; set; }
		public bool CarportYN { get; set; }
		public string City { get; set; } //City
		public DateTime? CloseDate { get; set; } //CloseDate
		public string CommunityFeatures { get; set; }
		public string ConstructionMaterials { get; set; }  //ConstructionDescription
		public string Cooling { get; set; } //AirConditioning
		public bool CoolingYN { get; set; }
		public string CrossStreet { get; set; }
		public string CurrentUse { get; set; }
		public int? DaysOnMarket { get; set; }
		public string DevelopmentStatus { get; set; }
		public string DirectionFaces { get; set; }
		public string Directions { get; set; }
		public string Disclosures { get; set; }
		public string DistanceToSchoolsComments { get; set; }
		public string DistanceToShoppingComments { get; set; }
		public string DoorFeatures { get; set; }
		public string Electric { get; set; }
		public bool ElectricOnPropertyYN { get; set; }
		public string ElementarySchool { get; set; } //ElementarySchool
		public string ElementarySchoolDistrict { get; set; }
		public int? EntryLevel { get; set; }
		public string EntryLocation { get; set; }
		public string Exclusions { get; set; }
		public string ExteriorFeatures { get; set; } //SidingDescription
		public string Fencing { get; set; }
		public string FireplaceFeatures { get; set; }
		public int? FireplacesTotal { get; set; } //Fireplacesnumberof
		public bool FireplaceYN { get; set; }
		public string Flooring { get; set; } //
		public string GarageSpaces { get; set; }
		public bool GarageYN { get; set; }
		public bool HabitableResidenceYN { get; set; }
		public string Heating { get; set; } //HeatingType
		public string HighSchool { get; set; }//HighSchool
		public string HighSchoolDistrict { get; set; }
		public string HorseAmenities { get; set; }
		public bool HorseYN { get; set; }
		public string Inclusions { get; set; } //Included
		public string InteriorFeatures { get; set; }
		public bool InternetAddressDisplayYN { get; set; }
		public bool InternetEntireListingDisplayYN { get; set; }
		public string LaundryFeatures { get; set; }
		public string Levels { get; set; }
		public string ListAgentEmail { get; set; } //	ListAgentEmail
		public string ListAgentFullName { get; set; } //	ListAgentFullName
		public string ListAgentMlsId { get; set; } //	ListAgentMLSID
		public string ListAgentPreferredPhone { get; set; } //	ListAgentDirectWorkPhone
		public string ListingKey { get; set; }
		public string ListOfficeName { get; set; }
		public string ListPrice { get; set; }  //CurrentPrice
		public string LivingArea { get; set; } //SqFtTotal
		public string LivingAreaUnits { get; set; } //UnitCount
		public string LotDimensionsSource { get; set; }
		public string LotFeatures { get; set; } //LotDescription
		public string LotSizeAcres { get; set; }
		public string LotSizeArea { get; set; } //LotSizeArea
		public string LotSizeDimensions { get; set; }
		public string LotSizeSource { get; set; }
		public string LotSizeSquareFeet { get; set; } //LotSizeAreaSQFT
		public string LotSizeUnits { get; set; }
		public List<MediaItem> Media { get; set; }
		public string MiddleOrJuniorSchool { get; set; }  //Junior_MiddleHighSchool
		public string MiddleOrJuniorSchoolDistrict { get; set; }
		public bool NewConstructionYN { get; set; }
		public string ONEKEY_AdditionalFeeDescription { get; set; }
		public string ONEKEY_AdditionalFeeFrequency { get; set; }
		public bool ONEKEY_AdditionalFeesYN { get; set; }
		public string ONEKEY_AlternateMlsNumber { get; set; } //AlternateMLSNumber
		public string ONEKEY_AtticDescription { get; set; } //AtticDescription
		public string ONEKEY_BuildingDimensions { get; set; }
		public string ONEKEY_BuildingLocation { get; set; }
		public string ONEKEY_BuildingSize { get; set; }
		public string ONEKEY_BusinessAge { get; set; }
		public string ONEKEY_BusinessLocatedAt { get; set; }
		public string ONEKEY_DescriptionOfBusiness { get; set; }
		public string ONEKEY_DevelopmentName { get; set; } //ComplexName
		public string ONEKEY_GarbageRemoval { get; set; } //Garbage
		public string ONEKEY_Hamlet { get; set; } //Hamlet
		public string ONEKEY_HeatingNotes { get; set; }
		public string ONEKEY_HotWater { get; set; } //Hotwater
		public string ONEKEY_ImprovementRemarks { get; set; }
		public string ONEKEY_LivingQuartersDescription { get; set; }
		public string ONEKEY_LoadingDocks { get; set; }
		public bool ONEKEY_LoadingDockYN { get; set; }
		public string ONEKEY_LocationDescription { get; set; }
		public string ONEKEY_MinimumAge { get; set; }
		public string ONEKEY_NumberOfHeatingUnits { get; set; }
		public string ONEKEY_NumberofHeatingZones { get; set; }   //HeatingZonesNumof
		public string OriginatingSystemKey { get; set; } //MLSNumber
		public string ONEKEY_Plumbing { get; set; }
		public string ONEKEY_SourceSystemModificationTimestamp { get; set; }
		public string ONEKEY_VideoTourURL { get; set; }
		public string ONEKEY_Village { get; set; } //Village
		public DateTime? OnMarketDate { get; set; }
		public string OtherStructures { get; set; }
		public string OwnerPays { get; set; }
		public string Ownership { get; set; }
		public string ParcelNumber { get; set; }
		public string ParkingFeatures { get; set; } //Parking
		public string ParkingTotal { get; set; }
		public string PatioAndPorchFeatures { get; set; }
		public string PhotosChangeTimestamp { get; set; }
		public int? PhotosCount { get; set; }  //PhotoCount
		public string PoolFeatures { get; set; }
		public bool PoolPrivateYN { get; set; }
		public string PossibleUse { get; set; }
		public string PostalCode { get; set; } //PostalCode
		public string PropertyCondition { get; set; }
		public string PropertySubType { get; set; }
		public string PropertyType { get; set; } //PropertyType
		public string Roof { get; set; }
		public int? RoomsTotal { get; set; } //RoomCount
		public string SecurityFeatures { get; set; }
		public bool SeniorCommunityYN { get; set; } //Adult55Community
		public string Sewer { get; set; } //SewerDescription
		public string SpaFeatures { get; set; }
		public bool SpaYN { get; set; }
		public string SpecialListingConditions { get; set; }
		public int? StoriesTotal { get; set; } //NumOfLevels
		public string StreetDirPrefix { get; set; } //StreetDirPrefix
		public string StreetDirSuffix { get; set; } //StreetDirSuffix
		public string StreetName { get; set; } //StreetName
		public string StreetNumber { get; set; } //StreetNumber
		public string StreetSuffix { get; set; } //StreetSuffix
		public string StructureType { get; set; }
		public string SubdivisionName { get; set; } //Subdivision_Development
		public string SyndicationRemarks { get; set; } //
		public string TaxAnnualAmount { get; set; } //TaxAmount
		public string TaxAssessedValue { get; set; }
		public string TaxBlock { get; set; }
		public string TaxLot { get; set; }
		public string TaxMapNumber { get; set; }
		public string TenantPays { get; set; }
		public string Township { get; set; } //
		public string TrashExpense { get; set; }
		public string UnitNumber { get; set; } //UnitNumber
		public string UnparsedAddress { get; set; } //
		public string Utilities { get; set; }
		public string View { get; set; }
		public string VirtualTourURLBranded { get; set; } //VirtualTourLink
		public string VirtualTourURLUnbranded { get; set; }
		public string WaterfrontFeatures { get; set; }
		public bool WaterfrontYN { get; set; } //WaterAccessYN
		public string WaterSource { get; set; }
		public string WindowFeatures { get; set; }
		public int? YearBuilt { get; set; } //	YearBuilt
		public string YearBuiltEffective { get; set; } //YearRenovated
		public string YearBuiltSource { get; set; }
		public string ZoningDescription { get; set; } //Zoning
	}

	public class MLSListings {
		public MLSSearchResult DoSearch(SearchModel data, int page = 1, int PostsPerPage = 12) {
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string param = "";
			string where = "";
			MLSSearchResult searchResult = new MLSSearchResult();
			List<MLSListing> listings = new List<MLSListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.City != null && data.City.Length == 1 && string.IsNullOrEmpty(data.City[0])) { data.City = null; }
				if (data.City != null) {
					foreach (string city in data.City) {
						param += "&city=" + HttpUtility.UrlEncode(city);
					}
				}
				if (data.City != null) {
					where += "AND (Town = @City1 ";
					for (int x = 2; x <= data.City.Length; x++) {
						where += " OR Town = @City" + x.ToString();
					}
					where += ") ";
				}
				if (data.Acres > 0) { where += "AND [LotSizeSquareFeet] >= @Acres "; }
				if (data.Acres2 > 0) { where += "AND [LotSizeSquareFeet] <= @Acres2 "; }
				if (data.Bathrooms > 0) { where += "AND [BathroomsFull] >= @Bathrooms "; }
				if (data.Bathrooms2 > 0) { where += "AND [BathroomsFull] <= @Bathrooms2 "; }
				if (data.Bedrooms > 0) { where += "AND [BedroomsTotal] >= @Bedrooms "; }
				if (data.Bedrooms2 > 0) { where += "AND [BedroomsTotal] <= @Bedrooms2 "; }
				if (data.MinPrice > 0) { where += "AND [ListPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { where += "AND [ListPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { where += "AND [LivingArea] >= @SqFt "; }
				if (data.SqFt2 > 0) { where += "AND [LivingArea] <= @SqFt2 "; }
				if (data.Years > 0) { where += "AND [YearBuilt] <= YEAR(GetDate()) - @Years "; }
				if (data.Years2 > 0) { where += "AND [YearBuilt] >= YEAR(GetDate()) - @Years2 "; }
				if (data.Pool == 1) { where += "AND LEN(PoolFeatures) > 0 "; }
				if (data.Fireplace == 1) { where += "AND ISNULL ([Fireplacestotal], 0) > 0 "; }
				if (data.Barn == 1) { where += "AND LEN(HorseAmenities) > 0 "; }
				if (data.Handicap == 1) { where += "AND LEN(AccessibilityFeatures) > 0 "; }
				if (data.Skylights == 1) { where += "AND WindowFeatures LIKE '%sky%' "; }
				if (data.Lake == 1) { where += "AND LEN(WaterfrontFeatures) > 0 "; }
				string SQL = "";
				if (data.City != null) {
					SQL = "SELECT COUNT(*) FROM [listings-residential-onekey] l JOIN ZIP_Town z ON l.PostalCode = z.ZIPCode WHERE PropertyType = 'Residential' " + where;
				} else {
					SQL = "SELECT COUNT(*) FROM [listings-residential-onekey] WHERE PropertyType = 'Residential' " + where;
				}

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (data.City != null) {
						for (int x = 1; x <= data.City.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = GetRelatedTown(data.City[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres * 43560; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2 * 43560; param += "&acres2=" + data.Acres2.ToString(); }
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

					int maxPages = (int)Math.Ceiling(Convert.ToDouble(searchResult.TotalResults) / PostsPerPage);
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

					if (data.City != null) {
						SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER " +
						"(ORDER BY [ListPrice] DESC, City) AS Rownumber, [OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l JOIN ZIP_Town z ON l.PostalCode = z.ZIPCode " +
						"WHERE PropertyType = 'Residential' " + where +
						") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					} else {
						SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER " +
						"(ORDER BY [ListPrice] DESC, City) AS Rownumber, [OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] WHERE PropertyType = 'Residential' " + where +
						") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					}
					cmd.CommandText = SQL;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new MLSListing(dr[1].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			searchResult.Listings = listings;
			return searchResult;
		}
		public List<MLSListing> GetMapListings(SearchModel data) {
			string param = "";
			List<MLSListing> listings = new List<MLSListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.City != null && data.City.Length == 1 && string.IsNullOrEmpty(data.City[0])) { data.City = null; }
				string SQL = "";
				if (data.City != null) {
					SQL = "SELECT l1.[OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l1 JOIN [listings-geo] l2 ON l1.OriginatingSystemKey = l2.[MLSNumber] " +
						"JOIN ZIP_Town z ON l1.PostalCode = z.ZIPCode " +
						"WHERE l1.PropertyType = 'Residential' ";
					SQL += "AND (Town = @City1 ";
					for (int x = 2; x <= data.City.Length; x++) {
						SQL += " OR Town = @City" + x.ToString();
					}
					SQL += ") ";
				} else {
					SQL = "SELECT l1.[OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l1 JOIN [listings-geo] l2 ON l1.OriginatingSystemKey = l2.[MLSNumber] " +
						"WHERE l1.PropertyType = 'Residential' ";
				}

				if (data.Acres > 0) { SQL += "AND [LotSizeSquareFeet] >= @Acres "; }
				if (data.Acres2 > 0) { SQL += "AND [LotSizeSquareFeet] <= @Acres2 "; }
				if (data.Bathrooms > 0) { SQL += "AND [BathroomsFull] >= @Bathrooms "; }
				if (data.Bathrooms2 > 0) { SQL += "AND [BathroomsFull] <= @Bathrooms2 "; }
				if (data.Bedrooms > 0) { SQL += "AND [BedroomsTotal] >= @Bedrooms "; }
				if (data.Bedrooms2 > 0) { SQL += "AND [BedroomsTotal] <= @Bedrooms2 "; }
				if (data.MinPrice > 0) { SQL += "AND [ListPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { SQL += "AND [ListPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { SQL += "AND [LivingArea] >= @SqFt "; }
				if (data.SqFt2 > 0) { SQL += "AND [LivingArea] <= @SqFt2 "; }
				if (data.Years > 0) { SQL += "AND [YearBuilt] <= YEAR(GetDate()) - @Years "; }
				if (data.Years2 > 0) { SQL += "AND [YearBuilt] >= YEAR(GetDate()) - @Years2 "; }
				if (data.Pool == 1) { SQL += "AND LEN(PoolFeatures) > 0 "; }
				if (data.Fireplace == 1) { SQL += "AND ISNULL ([Fireplacestotal], 0) > 0 "; }
				if (data.Barn == 1) { SQL += "AND LEN(HorseAmenities) > 0 "; }
				if (data.Handicap == 1) { SQL += "AND LEN(AccessibilityFeatures) > 0 "; }
				if (data.Skylights == 1) { SQL += "AND WindowFeatures LIKE '%sky%' "; }
				if (data.Lake == 1) { SQL += "AND LEN(WaterfrontFeatures) > 0 "; }

				SQL += "ORDER BY [ListPrice] DESC, City";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (data.City != null) {
						for (int x = 1; x <= data.City.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = GetRelatedTown(data.City[x - 1]);
							param += "&city=" + HttpUtility.UrlEncode(data.City[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres * 43560; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2 * 43560; param += "&acres2=" + data.Acres2.ToString(); }
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
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new MLSListing(dr[0].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			return listings;
		}

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
					string SQL = "SELECT DISTINCT b.Town " +
								"FROM [listings-residential-onekey] a JOIN ZIP_Town b ON a.PostalCode = b.ZIPCode " +
								"WHERE City <> '' AND PropertyType = 'Residential'";
					//string SQL = "SELECT DISTINCT b.Town AS City " +
					//				"FROM [listings-residential-onekey] a RIGHT JOIN TownRelations b ON a.City = b.RelatedTown " +
					//				"WHERE City<> '' AND PropertyType = 'Residential' " +
					//				"UNION " +
					//				"SELECT DISTINCT City FROM [listings-residential-onekey] WHERE City<> '' AND PropertyType = 'Residential' " +
					//				"ORDER BY City";

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
					string SQL = "SELECT DISTINCT b.Town " +
								"FROM [listings-residential-onekey] a JOIN ZIP_Town b ON a.PostalCode = b.ZIPCode " +
								"WHERE City <> '' AND PropertyType = 'Residential'";
					//string SQL = "SELECT DISTINCT b.Town AS City " +
					//				"FROM [listings-residential-onekey] a RIGHT JOIN TownRelations b ON a.City = b.RelatedTown " +
					//				"WHERE City<> '' AND PropertyType = 'Residential' " +
					//				"UNION " +
					//				"SELECT DISTINCT City FROM [listings-residential-onekey] WHERE City<> '' AND PropertyType = 'Residential' " +
					//				"ORDER BY City";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
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
			} catch (Exception e) {
				Console.Write(e.Message);
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

		private string GetRelatedTown(string town) {
			string ret = town;
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	using (SqlCommand cmd = new SqlCommand("SELECT RelatedTown FROM TownRelations WHERE Town = @Town", cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("Town", SqlDbType.VarChar, 100).Value = town;
			//		cmd.Connection.Open();
			//		object dr = cmd.ExecuteScalar();
			//		if (dr != null) { ret = (string)dr; }
			//		cmd.Connection.Close();
			//	}
			//}
			return ret;
		}

		public List<MLSFavListing> GetFeatured() {
			List<MLSFavListing> listings = new List<MLSFavListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				using (SqlCommand cmd = new SqlCommand("SELECT * FROM Favorites WHERE IsActive = 1 ORDER BY SortOrder, Town, MLS", cn)) {
					cmd.CommandType = CommandType.Text;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						if (!string.IsNullOrEmpty(dr["MLS"].ToString())) {
							MLSListing listing = new MLSListing(dr["MLS"].ToString());
							MLSFavListing fav = new MLSFavListing();
							if (!string.IsNullOrEmpty(listing.MLS)) {
								fav.LinkURL = "/detail?mls=" + listing.MLS;
								if (listing.Images.Count > 0) {
									fav.ImageURL = listing.Images[0];
								}
								fav.FavoriteDescription = listing.ShortDescription(150) + "...";
								listings.Add(fav);
							}
						} else {
							MLSFavListing fav = new MLSFavListing();
							fav.City = dr["Town"].ToString();
							fav.AskingPrice = dr["Price"].ToString();
							fav.FavoriteDescription = dr["Description"].ToString();
							fav.LinkURL = dr["Link"].ToString();
							fav.ImageURL = "/img/favorites/" + dr["FavoriteID"].ToString() + "/" + dr["Photo"].ToString();
							listings.Add(fav);
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

	}
	public class MLSListingsComm {
		public MLSSearchResult DoSearch(SearchModel data, int page = 1, int PostsPerPage = 12) {
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string param = "";
			string where = "";
			MLSSearchResult searchResult = new MLSSearchResult();
			List<MLSListing> listings = new List<MLSListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.City != null && data.City.Length == 1 && string.IsNullOrEmpty(data.City[0])) { data.City = null; }
				if (data.City != null) {
					foreach (string city in data.City) {
						param += "&city=" + HttpUtility.UrlEncode(city);
					}
				}
				if (data.City != null) {
					where += "AND (Town = @City1 ";
					for (int x = 2; x <= data.City.Length; x++) {
						where += " OR Town = @City" + x.ToString();
					}
					where += ") ";
				}
				if (data.Acres > 0) { where += "AND [LotSizeSquareFeet] >= @Acres "; }
				if (data.Acres2 > 0) { where += "AND [LotSizeSquareFeet] <= @Acres2 "; }
				if (data.MinPrice > 0) { where += "AND [ListPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { where += "AND [ListPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { where += "AND [BuildingAreaTotal] >= @SqFt "; }
				if (data.SqFt2 > 0) { where += "AND [BuildingAreaTotal] <= @SqFt2 "; }
				string SQL = "";
				if (data.City != null) {
					SQL = "SELECT COUNT(*) FROM [listings-residential-onekey] l JOIN ZIP_Town z ON l.PostalCode = z.ZIPCode WHERE PropertyType = 'Commercial Sale' " + where;
				} else {
					SQL = "SELECT COUNT(*) FROM [listings-residential-onekey] WHERE PropertyType = 'Commercial Sale' " + where;
				}

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (data.City != null) {
						for (int x = 1; x <= data.City.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = GetRelatedTown(data.City[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres * 43560; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2 * 43560; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.SqFt > 0) { cmd.Parameters.Add("SqFt", SqlDbType.Float).Value = data.SqFt; param += "&sqft=" + data.SqFt.ToString(); }
					if (data.SqFt2 > 0) { cmd.Parameters.Add("SqFt2", SqlDbType.Float).Value = data.SqFt2; param += "&sqft2=" + data.SqFt2.ToString(); }

					cmd.Connection.Open();
					searchResult.TotalResults = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();

					int maxPages = (int)Math.Ceiling(Convert.ToDouble(searchResult.TotalResults) / PostsPerPage);
					if (page > maxPages) {
						start = ((maxPages - 1) * PostsPerPage) + 1;
						end = (maxPages * PostsPerPage);
						searchResult.NextPage = "";
					} else if (page == maxPages) {
						searchResult.NextPage = "";
					} else {
						searchResult.NextPage = "/searchcomm?" + param + "&page=" + (page + 1).ToString();
					}
					if (page == 1) {
						searchResult.PrevPage = "";
					} else {
						searchResult.PrevPage = "/searchcomm?" + param + "&page=" + (page - 1).ToString();
					}

					if (data.City != null) {
						SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER " +
						"(ORDER BY [ListPrice] DESC, City) AS Rownumber, [OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l JOIN ZIP_Town z ON l.PostalCode = z.ZIPCode " +
						"WHERE PropertyType = 'Commercial Sale' " + where +
						") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					} else {
						SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER " +
						"(ORDER BY [ListPrice] DESC, City) AS Rownumber, [OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] WHERE PropertyType = 'Commercial Sale' " + where +
						") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					}

					cmd.CommandText = SQL;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new MLSListing(dr[1].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			searchResult.Listings = listings;
			return searchResult;
		}
		public List<MLSListing> GetMapListings(SearchModel data) {
			string param = "";
			List<MLSListing> listings = new List<MLSListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.City != null && data.City.Length == 1 && string.IsNullOrEmpty(data.City[0])) { data.City = null; }
				string SQL = "";
				if (data.City != null) {
					SQL = "SELECT l1.[OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l1 JOIN [listings-geo] l2 ON l1.OriginatingSystemKey = l2.[MLSNumber] " +
						"JOIN ZIP_Town z ON l1.PostalCode = z.ZIPCode " +
						"WHERE l1.PropertyType = 'Commercial Sale' ";
					SQL += "AND (Town = @City1 ";
					for (int x = 2; x <= data.City.Length; x++) {
						SQL += " OR Town = @City" + x.ToString();
					}
					SQL += ") ";
				} else {
					SQL = "SELECT l1.[OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l1 JOIN [listings-geo] l2 ON l1.OriginatingSystemKey = l2.[MLSNumber] " +
						"WHERE l1.PropertyType = 'Commercial Sale' ";
				}

				if (data.Acres > 0) { SQL += "AND [LotSizeSquareFeet] >= @Acres "; }
				if (data.Acres2 > 0) { SQL += "AND [LotSizeSquareFeet] <= @Acres2 "; }
				if (data.MinPrice > 0) { SQL += "AND [ListPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { SQL += "AND [ListPrice] <= @MaxPrice "; }
				if (data.SqFt > 0) { SQL += "AND [LivingArea] >= @SqFt "; }
				if (data.SqFt2 > 0) { SQL += "AND [LivingArea] <= @SqFt2 "; }

				SQL += "ORDER BY [ListPrice] DESC, City";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (data.City != null) {
						for (int x = 1; x <= data.City.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = GetRelatedTown(data.City[x - 1]);
							param += "&city=" + HttpUtility.UrlEncode(data.City[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres * 43560; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2 * 43560; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.SqFt > 0) { cmd.Parameters.Add("SqFt", SqlDbType.Float).Value = data.SqFt; param += "&sqft=" + data.SqFt.ToString(); }
					if (data.SqFt2 > 0) { cmd.Parameters.Add("SqFt2", SqlDbType.Float).Value = data.SqFt2; param += "&sqft2=" + data.SqFt2.ToString(); }

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new MLSListing(dr[0].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			return listings;
		}

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
					string SQL = "SELECT DISTINCT b.Town " +
								"FROM [listings-residential-onekey] a JOIN ZIP_Town b ON a.PostalCode = b.ZIPCode " +
								"WHERE City <> '' AND PropertyType = 'Commercial Sale'";
					//string SQL = "SELECT DISTINCT b.Town AS City " +
					//				"FROM [listings-residential-onekey] a RIGHT JOIN TownRelations b ON a.City = b.RelatedTown " +
					//				"WHERE City<> '' AND PropertyType = 'Commercial Sale' " +
					//				"UNION " +
					//				"SELECT DISTINCT City FROM [listings-residential-onekey] WHERE City<> '' AND PropertyType = 'Commercial Sale' " +
					//				"ORDER BY City";

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
					string SQL = "SELECT DISTINCT b.Town " +
								"FROM [listings-residential-onekey] a JOIN ZIP_Town b ON a.PostalCode = b.ZIPCode " +
								"WHERE City <> '' AND PropertyType = 'Commercial Sale'";
					//string SQL = "SELECT DISTINCT b.Town AS City " +
					//				"FROM [listings-residential-onekey] a RIGHT JOIN TownRelations b ON a.City = b.RelatedTown " +
					//				"WHERE City<> '' AND PropertyType = 'Commercial Sale' " +
					//				"UNION " +
					//				"SELECT DISTINCT City FROM [listings-residential-onekey] WHERE City<> '' AND PropertyType = 'Commercial Sale' " +
					//				"ORDER BY City";

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

		private string GetRelatedTown(string town) {
			string ret = town;
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	using (SqlCommand cmd = new SqlCommand("SELECT RelatedTown FROM TownRelations WHERE Town = @Town", cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("Town", SqlDbType.VarChar, 100).Value = town;
			//		cmd.Connection.Open();
			//		object dr = cmd.ExecuteScalar();
			//		if (dr != null) { ret = (string)dr; }
			//		cmd.Connection.Close();
			//	}
			//}
			return ret;
		}
	}
	public class MLSListingsLand {
		public MLSSearchResult DoSearch(SearchModel data, int page = 1, int PostsPerPage = 12) {
			int start = ((page - 1) * PostsPerPage) + 1;
			int end = (page * PostsPerPage);
			string param = "";
			string where = "";
			MLSSearchResult searchResult = new MLSSearchResult();
			List<MLSListing> listings = new List<MLSListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.City != null && data.City.Length == 1 && string.IsNullOrEmpty(data.City[0])) { data.City = null; }
				if (data.City != null) {
					foreach (string city in data.City) {
						param += "&city=" + HttpUtility.UrlEncode(city);
					}

					where += "AND (Town = @City1 ";
					for (int x = 2; x <= data.City.Length; x++) {
						where += " OR Town = @City" + x.ToString();
					}
					where += ") ";
				}
				if (data.Acres > 0) { where += "AND [LotSizeSquareFeet] >= @Acres "; }
				if (data.Acres2 > 0) { where += "AND [LotSizeSquareFeet] <= @Acres2 "; }
				if (data.MinPrice > 0) { where += "AND [ListPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { where += "AND [ListPrice] <= @MaxPrice "; }
				if (data.Lake == 1) { where += "AND LEN(WaterfrontFeatures) > 0 "; }
				string SQL = "";
				if (data.City != null) {
					SQL = "SELECT COUNT(*) FROM [listings-residential-onekey] l JOIN ZIP_Town z ON l.PostalCode = z.ZIPCode WHERE PropertyType = 'Land' " + where;
				} else {
					SQL = "SELECT COUNT(*) FROM [listings-residential-onekey] WHERE PropertyType = 'Land' " + where;
				}

				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (data.City != null) {
						for (int x = 1; x <= data.City.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = GetRelatedTown(data.City[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres * 43560; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2 * 43560; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.Lake == 1) { param += "&lake=1"; }

					cmd.Connection.Open();
					searchResult.TotalResults = (int)cmd.ExecuteScalar();
					cmd.Connection.Close();

					int maxPages = (int)Math.Ceiling(Convert.ToDouble(searchResult.TotalResults) / PostsPerPage);
					if (page > maxPages) {
						start = ((maxPages - 1) * PostsPerPage) + 1;
						end = (maxPages * PostsPerPage);
						searchResult.NextPage = "";
					} else if (page == maxPages) {
						searchResult.NextPage = "";
					} else {
						searchResult.NextPage = "/searchland?" + param + "&page=" + (page + 1).ToString();
					}
					if (page == 1) {
						searchResult.PrevPage = "";
					} else {
						searchResult.PrevPage = "/searchland?" + param + "&page=" + (page - 1).ToString();
					}

					if (data.City != null) {
						SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER " +
						"(ORDER BY [ListPrice] DESC, City) AS Rownumber, [OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l JOIN ZIP_Town z ON l.PostalCode = z.ZIPCode " +
						"WHERE PropertyType = 'Land' " + where +
						") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					} else {
						SQL = "SELECT * FROM (SELECT ROW_NUMBER() OVER " +
						"(ORDER BY [ListPrice] DESC, City) AS Rownumber, [OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] WHERE PropertyType = 'Land' " + where +
						") a WHERE Rownumber BETWEEN " + start.ToString() + " and " + end.ToString();
					}

					cmd.CommandText = SQL;
					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new MLSListing(dr[1].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			searchResult.Listings = listings;
			return searchResult;
		}
		public List<MLSListing> GetMapListings(SearchModel data) {
			string param = "";
			List<MLSListing> listings = new List<MLSListing>();
			using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
				if (data.City != null && data.City.Length == 1 && string.IsNullOrEmpty(data.City[0])) { data.City = null; }
				string SQL = "";
				if (data.City != null) {
					SQL = "SELECT l1.[OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l1 JOIN [listings-geo] l2 ON l1.OriginatingSystemKey = l2.[MLSNumber] " +
						"JOIN ZIP_Town z ON l1.PostalCode = z.ZIPCode " +
						"WHERE l1.PropertyType = 'Land' ";
					SQL += "AND (Town = @City1 ";
					for (int x = 2; x <= data.City.Length; x++) {
						SQL += " OR Town = @City" + x.ToString();
					}
					SQL += ") ";
				} else {
					SQL = "SELECT l1.[OriginatingSystemKey] " +
						"FROM [listings-residential-onekey] l1 JOIN [listings-geo] l2 ON l1.OriginatingSystemKey = l2.[MLSNumber] " +
						"WHERE l1.PropertyType = 'Land' ";
				}

				if (data.Acres > 0) { SQL += "AND [LotSizeSquareFeet] >= @Acres "; }
				if (data.Acres2 > 0) { SQL += "AND [LotSizeSquareFeet] <= @Acres2 "; }
				if (data.MinPrice > 0) { SQL += "AND [ListPrice] >= @MinPrice "; }
				if (data.MaxPrice > 0) { SQL += "AND [ListPrice] <= @MaxPrice "; }
				if (data.Lake == 1) { SQL += "AND LEN(WaterfrontFeatures) > 0 "; }

				SQL += "ORDER BY [ListPrice] DESC, City";
				using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
					cmd.CommandType = CommandType.Text;
					if (data.City != null) {
						for (int x = 1; x <= data.City.Length; x++) {
							cmd.Parameters.Add("City" + x.ToString(), SqlDbType.VarChar, 255).Value = GetRelatedTown(data.City[x - 1]);
							param += "&city=" + HttpUtility.UrlEncode(data.City[x - 1]);
						}
					}
					if (data.Acres > 0) { cmd.Parameters.Add("Acres", SqlDbType.Float).Value = data.Acres * 43560; param += "&acres=" + data.Acres.ToString(); }
					if (data.Acres2 > 0) { cmd.Parameters.Add("Acres2", SqlDbType.Float).Value = data.Acres2 * 43560; param += "&acres2=" + data.Acres2.ToString(); }
					if (data.MinPrice > 0) { cmd.Parameters.Add("MinPrice", SqlDbType.Float).Value = data.MinPrice * 1000; param += "&minprice=" + data.MinPrice.ToString(); }
					if (data.MaxPrice > 0) { cmd.Parameters.Add("MaxPrice", SqlDbType.Float).Value = data.MaxPrice * 1000; param += "&maxprice=" + data.MaxPrice.ToString(); }
					if (data.Lake == 1) { param += "&lake=1"; }

					cmd.Connection.Open();
					SqlDataReader dr = cmd.ExecuteReader();
					while (dr.Read()) {
						listings.Add(new MLSListing(dr[0].ToString()));
					}
					cmd.Connection.Close();
				}
			}

			return listings;
		}

		public List<SelectListItem> GetAcreSelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			int x = 2;
			while (x <= 20) {
				itemList.Add(new SelectListItem {
					Value = x.ToString(),
					Text = x.ToString(),
					Selected = (x.ToString() == SelectedID)
				});
				x = x + 2;
			}
			return itemList;
		}
		public List<SelectListItem> GetCitySelectList(string SelectedID) {
			List<SelectListItem> itemList = new List<SelectListItem>();
			try {
				using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
					string SQL = "SELECT DISTINCT b.Town " +
								"FROM [listings-residential-onekey] a JOIN ZIP_Town b ON a.PostalCode = b.ZIPCode " +
								"WHERE City <> '' AND PropertyType = 'Land'";
					//string SQL = "SELECT DISTINCT b.Town AS City " +
					//				"FROM [listings-residential-onekey] a RIGHT JOIN TownRelations b ON a.City = b.RelatedTown " +
					//				"WHERE City<> '' AND PropertyType = 'Land' " +
					//				"UNION " +
					//				"SELECT DISTINCT City FROM [listings-residential-onekey] WHERE City<> '' AND PropertyType = 'Land' " +
					//				"ORDER BY City";

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
					string SQL = "SELECT DISTINCT b.Town " +
								"FROM [listings-residential-onekey] a JOIN ZIP_Town b ON a.PostalCode = b.ZIPCode " +
								"WHERE City <> '' AND PropertyType = 'Land'";
					//string SQL = "SELECT DISTINCT b.Town AS City " +
					//				"FROM [listings-residential-onekey] a RIGHT JOIN TownRelations b ON a.City = b.RelatedTown " +
					//				"WHERE City<> '' AND PropertyType = 'Commercial Sale' " +
					//				"UNION " +
					//				"SELECT DISTINCT City FROM [listings-residential-onekey] WHERE City<> '' AND PropertyType = 'Land' " +
					//				"ORDER BY City";

					using (SqlCommand cmd = new SqlCommand(SQL, cn)) {
						cmd.CommandType = CommandType.Text;
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
			itemList.Add(new SelectListItem { Value = "50", Text = "$50,000", Selected = "50" == SelectedID });
			itemList.Add(new SelectListItem { Value = "100", Text = "$100,000", Selected = "100" == SelectedID });
			itemList.Add(new SelectListItem { Value = "150", Text = "$150,000", Selected = "150" == SelectedID });
			itemList.Add(new SelectListItem { Value = "200", Text = "$200,000", Selected = "200" == SelectedID });
			itemList.Add(new SelectListItem { Value = "250", Text = "$250,000", Selected = "250" == SelectedID });
			itemList.Add(new SelectListItem { Value = "300", Text = "$300,000", Selected = "300" == SelectedID });
			return itemList;
		}

		private string GetRelatedTown(string town) {
			string ret = town;
			//using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString)) {
			//	using (SqlCommand cmd = new SqlCommand("SELECT RelatedTown FROM TownRelations WHERE Town = @Town", cn)) {
			//		cmd.CommandType = CommandType.Text;
			//		cmd.Parameters.Add("Town", SqlDbType.VarChar, 100).Value = town;
			//		cmd.Connection.Open();
			//		object dr = cmd.ExecuteScalar();
			//		if (dr != null) { ret = (string)dr; }
			//		cmd.Connection.Close();
			//	}
			//}
			return ret;
		}
	}

	public class MLSSearchResult {
		public List<MLSListing> Listings { get; set; }
		public string PrevPage { get; set; }
		public string NextPage { get; set; }
		public int TotalResults { get; set; }
	}

	public class MLSFavListing {
		public string City { get; set; }
		public string AskingPrice { get; set; }
		public string FavoriteDescription { get; set; }
		public string LinkURL { get; set; }
		public string ImageURL { get; set; }

	}
}