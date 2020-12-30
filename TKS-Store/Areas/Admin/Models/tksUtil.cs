using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace TKS.Areas.Admin.Models {
	public static class tksUtil {

		public static bool HasFile(this HttpPostedFileBase file) {
			return (file != null && file.ContentLength > 0) ? true : false;
		}

		public static bool IsValidEmail(string email) {
			try {
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			} catch {
				return false;
			}
		}

		public static string NL2BR(string value) {
			return value.Replace(Convert.ToString((char)10), "<br>");
		}
		public static string ExtractNumbers(string expr) {
			return string.Join(null, Regex.Split(expr, "[^\\d]"));
		}
		public static string FormatDateTime(string date, string time) {
			string result = string.Format("{0:d}", DateTime.Parse(date));
			if (!string.IsNullOrEmpty(time)) {
				result += " - " + string.Format("{0:t}", DateTime.Parse(time));
			}
			return result;
		}
		public static DateTime UTCtoTZ(DateTime input) {
			return TimeZoneInfo.ConvertTimeFromUtc(input, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
		}
		public static string FormatRouteURL(string Segment) {
			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			Segment = rgx.Replace(Segment, "");
			Segment = Segment.Replace("  ", " ");
			Segment = Segment.Replace(" ", "-");
			return Segment.ToLower();
		}
		public static string MakeValidFileName(string name) {
			string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string invalidReStr = string.Format(@"[{0}]+", invalidChars);
			name = Regex.Replace(name, invalidReStr, "_");
			name = name.Replace("'", "").Replace(" ", "-");
			return name;
		}
		public static String ConstructQueryString(NameValueCollection parameters) {
			List<string> items = new List<string>();

			foreach (String name in parameters)
				items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

			return String.Join("&", items.ToArray());
		}

		/// <summary>
		/// This attribute is used to represent a string value
		/// for a value in an enum.
		/// </summary>
		public class StringValueAttribute : Attribute {

			#region Public Properties

			/// <summary>
			/// Holds the stringvalue for a value in an enum.
			/// </summary>
			public string StringValue { get; protected set; }

			#endregion

			#region Constructor

			/// <summary>
			/// Constructor used to initialize a StringValue Attribute
			/// </summary>
			/// <param name="value"></param>
			public StringValueAttribute(string value) {
				this.StringValue = value;
			}

			#endregion

		}
		/// <summary>
		/// Will get the string value for a given enums value, this will
		/// only work if you assign the StringValue attribute to
		/// the items in your enum.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetStringValue(this Enum value) {
			// Get the type
			Type type = value.GetType();

			// Get fieldinfo for this type
			FieldInfo fieldInfo = type.GetField(value.ToString());

			// Get the stringvalue attributes
			StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
				typeof(StringValueAttribute), false) as StringValueAttribute[];

			// Return the first if there was a match.
			return attribs.Length > 0 ? attribs[0].StringValue : null;
		}
	}
}