using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = System.IO.Directory;

namespace Lucene.NET.HowTo.ExamplesFacade
{
	public class LuceneDotNetHowToExamplesFacade
	{
		public LockFactory LockFactory { get; set; }
		public DirectoryInfo IndexDirectoryInfo { get; set; }
		public IndexWriter IndexWriter { get; set; }
		public IndexSearcher IndexSearcher { get; set; }
		public IEnumerable<Action<Document, FileInfo>> IndexActions { get; set; }
		public TopDocs SearchResult { get; set; }
		public int NumIndexed { get; set; }
		public Explanation Explanation { get; private set; }

		private bool _traceOn = true;
		
		public LuceneDotNetHowToExamplesFacade(
			string indexDirectory = @"E:\Sites\BuddLeather2\dev\assets\index", 
			IEnumerable<Action<Document, FileInfo>> additionalIndexActions = null) 
		{
			IndexDirectoryInfo = new DirectoryInfo(indexDirectory);
			LockFactory = new SimpleFSLockFactory();
			var indexActions = new List<Action<Document, FileInfo>> { indexFileContents, indexFileLength, indexFilename };
			if (additionalIndexActions != null)
			{
				indexActions.AddRange(additionalIndexActions);
			}
			IndexActions = indexActions;
		}

		private void indexFilename(Document document, FileInfo fileInfo)
		{
			document.Add(
				new Field("filename", fileInfo.Name,
						  Field.Store.YES,
						  Field.Index.NOT_ANALYZED,
						  Field.TermVector.YES));

			trace("Added filename for: {0}", fileInfo.FullName);
		}

		private void indexFileLength(Document document, FileInfo fileInfo)
		{
			document.Add(
				new Field("length", fileInfo.Length.ToString(),
					Field.Store.YES,
					Field.Index.NOT_ANALYZED,
					Field.TermVector.YES));

			trace("Added length for: {0} {1}", fileInfo.FullName, fileInfo.Length);
		}

		private void indexFileContents(Document document, FileInfo fileInfo)
		{
			// open a stream reader that will read the contents of the file
			var contents = new StreamReader(fileInfo.FullName);

			// add three fields to the index for this document
			document.Add(new Field("contents", contents));

			trace("Added content for: {0}", fileInfo.FullName);
		}

		public LuceneDotNetHowToExamplesFacade deleteIndex(string indexDirectory = null)
		{
			if (indexDirectory != null) IndexDirectoryInfo =  new DirectoryInfo(indexDirectory);
			if (IndexDirectoryInfo == null) throw new Exception("Index directory not specified");
			if (IndexDirectoryInfo.Exists == false) return this;

			foreach (var fileInfo in IndexDirectoryInfo.GetFiles())
			{
				fileInfo.Delete();
			}

			IndexDirectoryInfo.Delete();

			trace("Deleted existing index at {0}", IndexDirectoryInfo.FullName);

			// must make new object as the old one knows it was deleted and will cause an error later
			IndexDirectoryInfo = new DirectoryInfo(IndexDirectoryInfo.FullName);

			return this;
		}

		public LuceneDotNetHowToExamplesFacade createIndexWriter()
		{
			IndexWriter = new IndexWriter(
				FSDirectory.Open(IndexDirectoryInfo, LockFactory),
				new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
				IndexWriter.MaxFieldLength.UNLIMITED);
			trace("Created IndexWriter");
			return this;
		}

		public LuceneDotNetHowToExamplesFacade createIndexSearcher()
		{
			IndexSearcher = new IndexSearcher(FSDirectory.Open(IndexDirectoryInfo.FullName), true);

			trace("Created IndexSearcher");
			return this;
		}

		public LuceneDotNetHowToExamplesFacade optimize()
		{
			if (IndexWriter == null) throw new Exception("IndexWriter not created");

			var start = DateTime.Now;
			NumIndexed = IndexWriter.MaxDoc();
			IndexWriter.Optimize();

			trace("Number of documents indexed: {0} {1}ms to optimize index", NumIndexed, (DateTime.Now - start).TotalMilliseconds);

			return this;
		}

		public LuceneDotNetHowToExamplesFacade indexFile(string filename, IndexWriter writer = null, float? boost = null)
		{
			// check that an index writer has been created
			if (writer == null) writer = IndexWriter;
			if (writer == null) throw new Exception("IndexWriter not created");

			var fileInfo = new FileInfo(filename);
			if (!fileInfo.Exists) throw new Exception(string.Format("File not found: {0}", filename));

			trace("Indexing: {0}", fileInfo.FullName);

			var start = DateTime.Now;

			// create a lucene document
			var doc = new Document();

			foreach (var action in IndexActions)
			{
				action(doc, fileInfo);
			}

			// if boost specified, then assign it
			if (boost != null)
			{
				trace("Boost=={0}", boost);
				doc.Boost = boost.Value;
			}

			// tell the index writer to store what we've indexed
			writer.AddDocument(doc);

			trace("Document created and indexed in {0}ms", (DateTime.Now - start).TotalMilliseconds);

			return this;
		}

		public LuceneDotNetHowToExamplesFacade indexFiles(IEnumerable<string> filenames)
		{
			var start = DateTime.Now;
			foreach (var filename in filenames)
			{
				indexFile(filename);
			}
			trace("Indexed in {0}ms", (DateTime.Now - start).TotalMilliseconds);
			return this;
		}

		public LuceneDotNetHowToExamplesFacade closeWriter()
		{
			if (IndexWriter == null) throw new Exception("IndexWriter not created");
			//IndexWriter.Optimize();
			IndexWriter.Dispose();
			IndexWriter = null;
			trace("Closed IndexWriter");
			return this;
		}

		public LuceneDotNetHowToExamplesFacade termQuery(string fieldName, string value)
		{
			return search(new TermQuery(new Term(fieldName, value)));
		}

		public LuceneDotNetHowToExamplesFacade termRangeQuery(
			string lowerTerm,
			string upperTerm = null,
			string field = "contents",
			bool includeLower = true,
			bool includeUpper = true)

		{
			return search(new TermRangeQuery(field, lowerTerm, upperTerm, includeLower, includeUpper));
			/*
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");
			if (upperTerm == null) upperTerm = lowerTerm;

			trace("TermRangeQuery {0} {1} {2} {3} {4}", lowerTerm, upperTerm, field, includeLower, includeUpper);

			var query = new TermRangeQuery(field, lowerTerm, upperTerm, includeLower, includeUpper);

			// do the search
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
			 * */
		}

		public LuceneDotNetHowToExamplesFacade numericRangeQuery(
			string fieldName, 
			long min, long max, 
			bool minInclusive = true, bool maxInclusive = true)
		{
			return search(NumericRangeQuery.NewLongRange(fieldName, min, max, minInclusive, maxInclusive));
		}

		public LuceneDotNetHowToExamplesFacade prefixQuery(string text, string field = "contents")
		{
			return search(new PrefixQuery(new Term(field, text)));
			/*
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("prefixQuery {0} {1}", field, text);

			var query = new PrefixQuery(new Term(field, text));

			// do the search
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
			 * */
		}

		public LuceneDotNetHowToExamplesFacade booleanQuery(
			string thisText,
			string thatText,
			Occur occur,
			string field = "contents")
		{
			return search(new BooleanQuery
				{
					{new TermQuery(new Term(field, thisText)), occur},
					{new TermQuery(new Term(field, thatText)), occur}
				});
			/*
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("booleanAndQuery {0} {1} {2} {3}", field, thisText, occur, thatText);

			var query = new BooleanQuery
				{
					{new TermQuery(new Term(field, thisText)), occur},
					{new TermQuery(new Term(field, thatText)), occur}
				};

			// do the search
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
			 * */
		}
		/*
		public LuceneDotNetHowToExamplesFacade booleanOrQuery(
			string thisText,
			string andThatText,
			string field = "contents")
		{
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("booleanOrQuery {0} {1} {2}", field, thisText, andThatText);

			var query = new BooleanQuery();
			query.Add(new TermQuery(new Term(field, thisText)), Occur.SHOULD);
			query.Add(new TermQuery(new Term(field, andThatText)), Occur.SHOULD);

			// do the search
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
		}
		 * */

		public LuceneDotNetHowToExamplesFacade phraseQuery(string phrase, int slop = 1, string field = "contents")
		{
			var query = new PhraseQuery { Slop = slop };
			phrase.forEachWord(w => query.Add(new Term(field, w)));
			return search(query);
			/*
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("phraseQuery {0} {1} {2}", field, slop, phrase);

			var query = new PhraseQuery { Slop = slop };
			phrase.forEachWord(w => query.Add(new Term(field, w)));

			// do the search
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
			 * */
		}

		public LuceneDotNetHowToExamplesFacade wildcardQuery(string expression, string field = "contents")
		{
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("wildcardQuery {0} {1}", field, expression);

			var query = new WildcardQuery(new Term(field, expression));

			// do the search
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
		}

		public LuceneDotNetHowToExamplesFacade fuzzyQuery(string expression, string field = "contents")
		{
			return search(new FuzzyQuery(new Term(field, expression)));
		}

		public LuceneDotNetHowToExamplesFacade search(Query query)
		{
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("search {0}", query.ToString());

			// do the query
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
		}

		public LuceneDotNetHowToExamplesFacade search(Query query, Sort sort)
		{
			if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

			trace("search {0}", query.ToString());

			// do the query
			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(query, null, 100, sort);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
		}

		public LuceneDotNetHowToExamplesFacade search(string query, string field = "contents")
		{
			return search(
				new QueryParser(
					Lucene.Net.Util.Version.LUCENE_30,
					field,
					new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30)
					).Parse(query));
		}

		public LuceneDotNetHowToExamplesFacade search(string query, Sort sort, string field = "contents")
		{
			return search(
				new QueryParser(
					Lucene.Net.Util.Version.LUCENE_30,
					field,
					new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30)
					).Parse(query), sort);
		}

		public LuceneDotNetHowToExamplesFacade numericEquivalenceQuery(string fieldName, long value) {
			return numericRangeQuery(fieldName, value, value);
		}

		//var luceneQuery = parser.Parse(query);

				/*
			if (IndexSearcher == null) throw new Exception("IndexParser not created.");

			trace("Searching for: {0}", query);

			var parser = new QueryParser(
				Lucene.Net.Util.Version.LUCENE_30, 
				field,
				new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

			var luceneQuery = parser.Parse(query);

			var start = DateTime.Now.TimeOfDay;
			SearchResult = IndexSearcher.Search(luceneQuery, 100);
			var end = DateTime.Now.TimeOfDay;

			trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
			trace(SearchResult, IndexSearcher);

			return this;
				 * */
		//}

		public LuceneDotNetHowToExamplesFacade traceOn()
		{
			_traceOn = true;
			return this;
		}

		public LuceneDotNetHowToExamplesFacade traceOff()
		{
			_traceOn = false;
			return this;
		}

		public void trace(string format, params object[] options) {
			//if (_traceOn) Trace.WriteLine(string.Format(format, options));
			if (_traceOn) HttpContext.Current.Response.Write(string.Format(format, options) + "<br />");
		}

		public void trace(TopDocs topDocs, IndexSearcher searcher)
		{
			trace("Total hits: {0}", topDocs.TotalHits);
			trace("MaxScore: {0}", topDocs.MaxScore);

			foreach (var hit in topDocs.ScoreDocs)
			{
				var filename = searcher.Doc(hit.Doc).Get("filename");
				var length = searcher.Doc(hit.Doc).Get("length");
				trace("Matched: {0} {1} DocID=={2} Score=={3}", filename, length, hit.Doc, hit.Score);
			}
		}

		public LuceneDotNetHowToExamplesFacade buildLexicographicalExampleIndex(int maxDocs=100)
		{
			this.traceOff()
				.deleteIndex()
				.createIndexWriter()
				.createLexicographicalIndexDataSet(maxDocs)
				.indexDirectory(@"c:\Lucene\data\Lexi")
				.closeWriter()
				.traceOn();
			return this;
		}

		public LuceneDotNetHowToExamplesFacade buildLexicographicalExampleIndex(
			Func<string, bool> matcher,
			Func<float?> booster,
			int maxDocs = 100)
		{
			this.traceOff()
				.deleteIndex()
				.createIndexWriter()
				.createLexicographicalIndexDataSet(maxDocs)
				.indexDirectory(@"c:\Lucene\data\Lexi", matcher, booster)
				.closeWriter()
				.traceOn();
			return this;
		}

		private LuceneDotNetHowToExamplesFacade createLexicographicalIndexDataSet(int maxDocs = 100)
		{
			var di = new DirectoryInfo(@"c:\Lucene\data\lexi");
			if (di.Exists)
			{
				foreach (var file in di.EnumerateFiles())
				{
					file.Delete();
				}
			}
			else
			{
				di.Create();
			}

			Enumerable.Range(0, maxDocs).forEach(
				i =>
					{
						using (var writer = File.CreateText(
							string.Format(@"c:\Lucene\Data\Lexi\{0}.txt", i)))
						{
							writer.Write("File {0} {1}", i, numberToWords(i));
						}
					});

			return this;
		}

		private static string numberToWords(int number)
		{
			if (number == 0)
				return "zero";

			if (number < 0)
				return "minus " + numberToWords(Math.Abs(number));

			string words = "";

			if ((number / 1000000) > 0)
			{
				words += numberToWords(number / 1000000) + " million ";
				number %= 1000000;
			}

			if ((number / 1000) > 0)
			{
				words += numberToWords(number / 1000) + " thousand ";
				number %= 1000;
			}

			if ((number / 100) > 0)
			{
				words += numberToWords(number / 100) + " hundred ";
				number %= 100;
			}

			if (number > 0)
			{
				if (words != "")
					words += "and ";

				var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
				var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

				if (number < 20)
					words += unitsMap[number];
				else
				{
					words += tensMap[number / 10];
					if ((number % 10) > 0)
						words += "-" + unitsMap[number % 10];
				}
			}

			return words;
		}

		private LuceneDotNetHowToExamplesFacade indexDirectory(string directoryName)
		{
			var di = new DirectoryInfo(directoryName);
			if (!di.Exists) return this;

			foreach (var file in di.EnumerateFiles())
			{
				indexFile(file.FullName);
			}

			return this;
		}

		private LuceneDotNetHowToExamplesFacade indexDirectory(string directoryName, Func<string, bool> matcher, Func<float?> booster)
		{
			var di = new DirectoryInfo(directoryName);
			if (!di.Exists) return this;

			foreach (var file in di.EnumerateFiles())
			{
				indexFile(file.FullName, IndexWriter, matcher(file.FullName) ? booster() : null);
			}

			return this;
		}

		public LuceneDotNetHowToExamplesFacade dumpDocumentProperties(Document doc, IndexSearcher searcherToUse = null)
		{
			var searcher = searcherToUse;
			if (searcher == null) searcher = IndexSearcher;
			if (searcher == null) throw new Exception("No IndexSearcher specified");

			foreach (var field in doc.GetFields())
			{
				trace("{0}=={1}", field.Name, doc.Get(field.Name));
			}
			return this;
		}

		public LuceneDotNetHowToExamplesFacade dumpSearchResultDocumentProperties(int searchResultIndex = 0, IndexSearcher searcherToUse = null)
		{
			var searcher = searcherToUse;
			if (searcher == null) searcher = IndexSearcher;
			if (searcher == null) throw new Exception("No IndexSearcher specified");
			if (SearchResult == null || searchResultIndex >= SearchResult.TotalHits)
				throw new Exception("No search results of specified document index > # of search results");

			return dumpDocumentProperties(searcher.Doc(SearchResult.ScoreDocs[searchResultIndex].Doc), searcher);
		}

		public LuceneDotNetHowToExamplesFacade searchWithTimeout(string query, TimeSpan timeSpan, string field = "contents")
		{
			if (IndexSearcher == null) throw new Exception("IndexParser not created.");

			trace("Searching for: {0}", query);

			var parser = new QueryParser(
				Lucene.Net.Util.Version.LUCENE_30,
				field,
				new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));

			var luceneQuery = parser.Parse(query);

			var topScoreCollector = TopScoreDocCollector.Create(100, false);
			var collector = new TimeLimitingCollector(topScoreCollector, (long)timeSpan.TotalMilliseconds);

			try
			{
				var start = DateTime.Now.TimeOfDay;
				IndexSearcher.Search(luceneQuery, collector);
				var end = DateTime.Now.TimeOfDay;

				SearchResult = topScoreCollector.TopDocs();

				trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
				trace(SearchResult, IndexSearcher);
			}
			catch (Exception)
			{
				trace("Search took too much time");
			}

			return this;
		}

		public LuceneDotNetHowToExamplesFacade explain(FuzzyQuery query, int docID)
		{
			if (IndexSearcher == null) throw new Exception("IndexParser not created.");

			trace("Explaining DocumentID=={0} Query={1}", docID, query);

			this.Explanation = IndexSearcher.Explain(query, docID);

			trace(this.Explanation);

			return this;
		}

		private void trace(Explanation explanation)
		{
			trace(explanation.ToString());
		}

		public LuceneDotNetHowToExamplesFacade setIndexSearcherFieldSortScoring(bool doTrackScores, bool doMaxScore)
		{
			if (IndexSearcher == null) throw new Exception("IndexParser not created.");

			IndexSearcher.SetDefaultFieldSortScoring(doTrackScores, doMaxScore);

			return this;
		}
	}
}
