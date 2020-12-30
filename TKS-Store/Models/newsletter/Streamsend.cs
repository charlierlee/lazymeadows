using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using TKS.Areas.Admin.Models;

namespace TKS.Models {
	public class Streamsend {

		private static string _APIusername = Global.NewsletterStreamSendAPIUsername; // "vj1RdajmooKo";  // StreamSend API Username
		private static string _APItoken = Global.NewsletterStreamSendAPIPassword; // "4wDNFmHMadeuB9Rj"; // StreamSend API Password

		static public string EncodeTo64(string toEncode) {
			byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}


		public static string CreateNewSubscriber(string Email, string FirstName = "", string Lastname = "", string Company = "", string Street1 = "", string Street2 = "", string City= "", string State = "", string Zip = "") {

			Uri addy = new Uri("https://app.streamsend.com/audiences/1/people.xml");
			HttpWebRequest request = WebRequest.Create(addy) as HttpWebRequest;
			request.Headers.Add("Authorization: Basic " + EncodeTo64(_APIusername + ":" + _APItoken));
			request.Method = "POST";
			request.Accept = "application/xml";
			request.ContentType = "application/xml";

			StringBuilder data = new StringBuilder();
			data.AppendLine("<person>");
			data.AppendLine("<email-address>" + Email + "</email-address>");
			data.AppendLine("<first-name>" + FirstName + "</first-name>");
			data.AppendLine("<last-name>" + Lastname + "</last-name>");
			data.AppendLine("<company>" + Company + "</company>");
			data.AppendLine("<street-1>" + Street1 + "</street-1>");
			data.AppendLine("<street-2>" + Street2 + "</street-2>");
			data.AppendLine("<city>" + City + "</city>");
			data.AppendLine("<state>" + State + "</state>");
			data.AppendLine("<zip>" + Zip + "</zip>");
			data.AppendLine("</person>");


			byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

			request.ContentLength = byteData.Length;
			Stream sPost = request.GetRequestStream();
			sPost.Write(byteData, 0, byteData.Length);


			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			StreamReader srResponse = new StreamReader(response.GetResponseStream());

			return srResponse.ReadToEnd().ToString();
		}


		public static string CreateEmailMessage(string emailNameDesc, string txtHTML, string txtText) {

			//        POST /emails
			//  POST /emails.xml

			//  <email>
			//    <name>My New Email</name>
			//    <html-part><![CDATA[ ... ]]></html-part>
			//    <text-part><![CDATA[ ... ]]></text-part>
			//  </email>
			//Responses
			//  201 Created
			//  Location: /emails/{id}

			Uri addy = new Uri("https://app.streamsend.com/emails.xml");
			HttpWebRequest request = WebRequest.Create(addy) as HttpWebRequest;

			request.Proxy = null;
			request.UserAgent = ".NET/3.5";
			request.Timeout = 60000;
			CredentialCache credCache = new CredentialCache();
			credCache.Add(addy, "Basic", new NetworkCredential(_APIusername, _APItoken));
			request.Credentials = credCache;
			string combinedPass = _APIusername + ":" + _APItoken;
			request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(new ASCIIEncoding().GetBytes(combinedPass)));
			request.Method = "POST";
			request.Accept = "application/xml";
			request.ContentType = "application/xml";


			StringBuilder data = new StringBuilder();
			data.AppendLine("<email>");
			data.AppendLine("<name>" + emailNameDesc + "</name>");
			data.AppendLine("<html-part><![CDATA[" + txtHTML + "]]></html-part>");
			data.AppendLine("<text-part><![CDATA[" + txtText + "]]></text-part>");
			data.AppendLine("</email>");

			byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

			request.ContentLength = byteData.Length;
			Stream sPost = request.GetRequestStream();
			sPost.Write(byteData, 0, byteData.Length);

			HttpWebResponse response = request.GetResponse() as HttpWebResponse;
			StreamReader srResponse = new StreamReader(response.GetResponseStream());

			return srResponse.ReadToEnd().ToString();
		}

		public static int EmailExist(string Email) {
			int _returnID = 0;

			Uri addy = new Uri("https://app.streamsend.com/audiences/1/people.xml?email_address=" + Email);
			HttpWebRequest request = WebRequest.Create(addy) as HttpWebRequest;

			request.Headers.Add("Authorization: Basic " + EncodeTo64(_APIusername + ":" + _APItoken));
			request.Method = "GET";
			request.Accept = "application/xml";
			request.ContentType = "application/xml";

			HttpWebResponse response = request.GetResponse() as HttpWebResponse;

			StreamReader srResponse = new StreamReader(response.GetResponseStream());

			string xml = srResponse.ReadToEnd().ToString();

			if (String.IsNullOrEmpty(xml.Trim())) {
				_returnID = 0;
			} else {
				//Read XML Results
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xml);

				//API People Object
				XmlNodeList nodes = doc.SelectNodes("people/person");
				foreach (XmlNode node in nodes) {
					XmlNode idNameNode = node.SelectSingleNode("id");
					int.TryParse(idNameNode.InnerText, out _returnID);
				}
			}

			return _returnID;
		}
	}
}