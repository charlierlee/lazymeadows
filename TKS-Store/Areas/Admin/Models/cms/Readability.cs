using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace TKS.Areas.Admin.Models.CMS {
	public class Readability {

		#region Fields
		private int _Characters = 0;
		private int _CharactersNoSpaces = 0;
		private int _ComplexWordCount = 0; //3 or greater syllables
		private string _Contents = "";
		private ArrayList _SentanceList;
		private int _Sentences = 0;
		private string _StrippedContents = "";
		private int _Syllables = 0;
		private int _Words = 0;
		#endregion

		#region Constructor
		public Readability(string Contents) {
			_Contents = Contents;
		}
		#endregion

		#region Private Methods
		private ArrayList ReasonableParser(string sTextToParse) {
            ArrayList al = new ArrayList();

			sTextToParse = sTextToParse.Replace(Environment.NewLine, " ");

            // split the string using sentence terminations
            char[] arrSplitChars = { '.', '?', '!' };

            //do the split
			string[] splitSentences = sTextToParse.Split(arrSplitChars, StringSplitOptions.RemoveEmptyEntries);

            // loop the array of splitSentences
            for (int i = 0; i < splitSentences.Length; i++) {
                // find the position of each sentence in the
                // original paragraph and get its termination ('.', '?', '!')
				int pos = sTextToParse.IndexOf(splitSentences[i].ToString());
				char[] arrChars = sTextToParse.Trim().ToCharArray();
                char c = arrChars[pos + splitSentences[i].Length];

                // since this approach looks only for the first instance
                // of the string, it does not handle duplicate sentences
                // with different terminations.  You could expand this
                // approach to search for later instances of the same
                // string to get the proper termination but the previous
                // method of using the regular expression to split the
                // string is reliable and less bothersome.

                // add the sentences termination to the end of the sentence
                al.Add(splitSentences[i].ToString().Trim() + c.ToString());
            }

			//// Update the show of statistics
			//lblCharCount.Text = "Character Count: " +
			//	GenerateCharacterCount(sTemp).ToString();

			//lblSentenceCount.Text = "Sentence Count: " +
			//	GenerateSentenceCount(splitSentences).ToString();

			//lblWordCount.Text = "Word Count: " +
			//	GenerateWordCount(al).ToString();

            return al;
        }
		private int SyllableCount(string word) {
			word = word.ToLower().Trim();
			int count = System.Text.RegularExpressions.Regex.Matches(word, "[aeiouy]+").Count;
			if ((word.EndsWith("e") || (word.EndsWith("es") || word.EndsWith("ed"))) && !word.EndsWith("le")) {
				if (count > 1) {
					count--;
				}
			}
			if (count >= 3) { _ComplexWordCount++; }
			return count;
		}
		#endregion

		#region Properties
		public int Characters {
			get {
				if (_Characters == 0) {
					// clean up the string by removing newlines and by trimming both ends
					string sTemp = StrippedContents;
					sTemp = sTemp.Replace(Environment.NewLine, string.Empty).Trim();

					// split the string into sentences using a regular expression
					string[] splitSentences = Regex.Split(sTemp, @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])");

					// loop through the sentences to get character counts
					for (int cnt = 0; cnt < splitSentences.Length; cnt++) {
						// get the current sentence
						string sSentence = splitSentences[cnt].ToString();

						// trim it
						sSentence = sSentence.Trim();

						// convert it to a character array
						char[] sentence = sSentence.ToCharArray();

						// test each character and add it to the return value if it passes
						for (int i = 0; i < sentence.Length; i++) {
							// make sure it is a letter, number, punctuation or whitespace before adding it to the tally
							if (char.IsLetterOrDigit(sentence[i]) ||
								char.IsPunctuation(sentence[i]) ||
								char.IsWhiteSpace(sentence[i]))
								_Characters++;
						}
					}

					//bool lastWasSpace = false;

					//foreach (char c in StrippedContents) {
					//	if (char.IsWhiteSpace(c)) {
					//		// A.
					//		// Only count sequential spaces one time.
					//		if (lastWasSpace == false) {
					//			_Characters++;
					//		}
					//		lastWasSpace = true;
					//	} else {
					//		// B.
					//		// Count other characters every time.
					//		_Characters++;
					//		lastWasSpace = false;
					//	}
					//}
				}
				return _Characters;
			}
		}
		public int CharactersNoSpaces {
			get {
				if (_CharactersNoSpaces == 0) {
					// clean up the string by removing newlines and by trimming both ends
					string sTemp = StrippedContents;
					sTemp = sTemp.Replace(Environment.NewLine, string.Empty).Trim();

					// split the string into sentences using a regular expression
					string[] splitSentences = Regex.Split(sTemp, @"(?<=['""A-Za-z0-9][\.\!\?])\s+(?=[A-Z])");

					// loop through the sentences to get character counts
					for (int cnt = 0; cnt < splitSentences.Length; cnt++) {
						// get the current sentence
						string sSentence = splitSentences[cnt].ToString();

						// trim it
						sSentence = sSentence.Trim();

						// convert it to a character array
						char[] sentence = sSentence.ToCharArray();

						// test each character and add it to the return value if it passes
						for (int i = 0; i < sentence.Length; i++) {
							// make sure it is a letter, number, punctuation or whitespace before adding it to the tally
							if (char.IsLetterOrDigit(sentence[i]) ||
								char.IsPunctuation(sentence[i]))
								_CharactersNoSpaces++;
						}
					}
					//foreach (char c in StrippedContents) {
					//	if (!char.IsWhiteSpace(c)) {
					//		_CharactersNoSpaces++;
					//	}
					//}
				}
				return _CharactersNoSpaces;
			}
		}
		public string Contents {
			get { return _Contents; }
		}
		public double GunningFogScore {
			get {
				return (0.4 * ((Words / Convert.ToDouble(Sentences)) + (100 * (_ComplexWordCount / Convert.ToDouble(Words))))); 
			}
		}
		public double FleschKincaidGradeLevel {
			get {
				return ((.39 * (Words / Sentences)) + (11.8 * (Syllables / Words)) - 15.59);
			}
		}
		public double ColemanLiauIndex {
			get {
				return (206.876 - (1.015 * (Words / Sentences)) - (84.6 * (Syllables / Words)));
			}
		}
		public double SMOGIndex {
			get {
				return (206.876 - (1.015 * (Words / Sentences)) - (84.6 * (Syllables / Words)));
			}
		}
		public double AutomatedReadabilityIndex {
			get {
				return ((4.71 * (CharactersNoSpaces / Convert.ToDouble(Words))) + (.5 * (Words / Convert.ToDouble(Sentences))) - 21.43);
			}
		}
		public double AverageGradeLevel {
			get {
				return (206.876 - (1.015 * (Words / Sentences)) - (84.6 * (Syllables / Words)));
			}
		}
		public ArrayList SentenceList {
			get {
				if (_Sentences == 0) {
					_SentanceList = ReasonableParser(StrippedContents);
					_Sentences = _SentanceList.Count;
				}
				return _SentanceList;
			}
		}
		public int Sentences {
			get {
				if (_Sentences == 0) {
					_SentanceList = ReasonableParser(StrippedContents);
					_Sentences = _SentanceList.Count;
				}
				return _Sentences;
			}
		}
		public string StrippedContents {
			get {
				if (_StrippedContents.Length == 0 && _Contents.Length > 0) {
					HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
					doc.LoadHtml(_Contents);
					if (doc == null) return null;

					StringBuilder sb = new StringBuilder();
					foreach (var node in doc.DocumentNode.ChildNodes) {
						sb.Append(node.InnerText.Replace(Environment.NewLine, " ").Trim() + " ");
					}
					_StrippedContents = sb.ToString();
				}
				return _StrippedContents;
			}
		}
		public int Syllables {
			get {
				if (_Syllables == 0) {
					MatchCollection collection = Regex.Matches(StrippedContents, @"[\S]+");
					foreach (Match word in collection) {
						//HttpContext.Current.Response.Write(string.Format("{0} - {1}<br />", word.ToString(), SyllableCount(word.ToString())));
						_Syllables += SyllableCount(word.ToString());
					}
				}
				return _Syllables;
			}
		}
		public double SyllablesPerWord {
			get {
				if (Words > 0) {
					return Syllables / Convert.ToDouble(Words);
				} else {
					return 0;
				}
			}
		}
		public int Words {
			get {
				if (_Words == 0) {
					MatchCollection collection = Regex.Matches(StrippedContents, @"[\S]+");
					_Words = collection.Count;
				}
				return _Words;
			}
		}
		public double WordsPerSentence {
			get {
				if (Sentences > 0) {
					return Words / Convert.ToDouble(Sentences);
				} else {
					return 0;
				}
			}
		}
	}
	#endregion
}