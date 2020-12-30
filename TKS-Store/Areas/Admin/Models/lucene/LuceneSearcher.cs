using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using TKS.Areas.Admin.Models;
using Version = Lucene.Net.Util.Version;

/// <summary>
/// Summary description for LuceneSearcher
/// </summary>
public class LuceneSearcher
{
	#region Fields
	private int _MAXRESULTS = 100;
	private string _indexDirectory = "";
	private string _spellDirectory = "";
	private string _baseURL = "";
	private DataTable _Results = new DataTable();
	private int _totalItems = 0;
	private int _startAt = 0; // First item on page (index format).
	private TimeSpan _duration;
	private int _resultsPerPage = 10000;
	private int _fromItem = 0;
	private int _toItem = 0;
	private string _query = "";
	private bool _traceOn = false;
	#endregion

	#region Constructor
	public LuceneSearcher()
	{
		_indexDirectory = HttpContext.Current.Server.MapPath("~/assets/index");
		_spellDirectory = HttpContext.Current.Server.MapPath("~/assets/spell");
		_baseURL = Global.BaseURL;

		IndexDirectoryInfo = new DirectoryInfo(_indexDirectory);

		// create the result DataTable
		_Results.Columns.Add("title", typeof(string));
		_Results.Columns.Add("sample", typeof(string));
		_Results.Columns.Add("path", typeof(string));
		_Results.Columns.Add("url", typeof(string));
		_Results.Columns.Add("score", typeof(int));
		_Results.Columns.Add("id", typeof(string));
		_Results.Columns.Add("type", typeof(string));

	}
	#endregion

	#region Public Methods
	public void CreateIndexSearcher() {
		IndexSearcher = new IndexSearcher(FSDirectory.Open(IndexDirectoryInfo.FullName), true);

		trace("Created IndexSearcher");
	}

	public void Search(Query query) {
		if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

		trace("search {0}", query.ToString());

		var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

		// do the query
		var start = DateTime.Now.TimeOfDay;
		TopDocs SearchResult = IndexSearcher.Search(query, _MAXRESULTS);

		_totalItems = SearchResult.TotalHits;
		if (_totalItems > _MAXRESULTS) { _totalItems = _MAXRESULTS; }

		// create highlighter
		IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;\">", "</span>");
		SimpleFragmenter fragmenter = new SimpleFragmenter(80);
		QueryScorer scorer = new QueryScorer(query);
		Highlighter highlighter = new Highlighter(formatter, scorer);
		highlighter.TextFragmenter = fragmenter;

		// initialize startAt
		_startAt = InitStartAt();

		// how many items we should show - less than defined at the end of the results
		int resultsCount = Math.Min(_totalItems, _resultsPerPage + _startAt);
		if (resultsCount > _MAXRESULTS) { resultsCount = _MAXRESULTS; }

		for (int i = _startAt; i < resultsCount; i++) {
			// get the document from index
			Document doc = IndexSearcher.Doc(SearchResult.ScoreDocs[i].Doc);

			TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("text")));
			String sample = highlighter.GetBestFragments(stream, doc.Get("text"), 2, "...");

			String path = doc.Get("path");

			// create a new row with the result data
			DataRow row = _Results.NewRow();
			row["title"] = doc.Get("title");
			row["path"] = path;
			row["url"] = _baseURL + path;
			row["sample"] = sample;
			row["score"] = Convert.ToInt16(SearchResult.ScoreDocs[i].Score * 100);
			row["id"] = doc.Get("id");
			row["type"] = doc.Get("type");

			_Results.Rows.Add(row);
		}
		IndexSearcher.Dispose();

		var end = DateTime.Now.TimeOfDay;

		trace("Search completed in {0}ms", end.TotalMilliseconds - start.TotalMilliseconds);
		trace(SearchResult, IndexSearcher);
	}

	public void Search(string Query) {
		if (IndexSearcher == null) throw new Exception("IndexSearcher not created");

		_query = Query;
		DateTime start = DateTime.Now;

		var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

		var fieldName = "text";
		var minimumSimilarity = 0.5f;
		var prefixLength = 3;
		var query = new BooleanQuery();

		var segments = _query.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string segment in segments) {
			Term term = new Term(fieldName, segment);
			FuzzyQuery fuzzyQuery = new FuzzyQuery(term, minimumSimilarity, prefixLength);
			query.Add(fuzzyQuery, Occur.SHOULD);
		}
	
		// search
		TopDocs hits = IndexSearcher.Search(query, 200);

		_totalItems = hits.TotalHits;
		if (_totalItems > _MAXRESULTS) { _totalItems = _MAXRESULTS; }

		// create highlighter
		IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;\">", "</span>");
		SimpleFragmenter fragmenter = new SimpleFragmenter(80);
		QueryScorer scorer = new QueryScorer(query);
		Highlighter highlighter = new Highlighter(formatter, scorer);
		highlighter.TextFragmenter = fragmenter;

		// initialize startAt
		_startAt = InitStartAt();

		// how many items we should show - less than defined at the end of the results
		int resultsCount = Math.Min(_totalItems, _resultsPerPage + _startAt);
		if (resultsCount > _MAXRESULTS) { resultsCount = _MAXRESULTS; }

		for (int i = _startAt; i < resultsCount; i++) {
			// get the document from index
			Document doc = IndexSearcher.Doc(hits.ScoreDocs[i].Doc);

			TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("text")));
			String sample = highlighter.GetBestFragments(stream, doc.Get("text"), 2, "...");

			String path = doc.Get("path");

			// create a new row with the result data
			DataRow row = _Results.NewRow();
			row["title"] = doc.Get("title");
			row["path"] = path;
			row["url"] = _baseURL + path;
			row["sample"] = sample;
			row["score"] = Convert.ToInt16(hits.ScoreDocs[i].Score * 100);

			_Results.Rows.Add(row);
		}
		IndexSearcher.Dispose();

		// result information
		_duration = DateTime.Now - start;
		_fromItem = _startAt + 1;
		_toItem = Math.Min(_startAt + this.ResultsPerPage, _totalItems);
	}
	#endregion

	#region Private Methods
	/// <summary>
	/// Initializes startAt value. Checks for bad values.
	/// </summary>
	/// <returns></returns>
	private int InitStartAt() {
		try {
			int sa = 0;
			if (HttpContext.Current.Request.Params["start"] != null) {
				Int32.TryParse(HttpContext.Current.Request.Params["start"].ToString(), out sa);
			}
			// too small starting item, return first page
			if (sa < 0) {
				return 0;
			}

			// too big starting item, return last page
			if (sa >= _totalItems - 1) {
				return this.LastPageStartsAt;
			}

			return sa;
		} catch {
			return 0;
		}
	}
	#endregion

	#region Public Properties

	public IndexSearcher IndexSearcher { get; set; }
	public DirectoryInfo IndexDirectoryInfo { get; set; }

	/// <summary>
	/// How many pages are there in the results.
	/// </summary>
	public int PageCount {
		get {
			return (_totalItems - 1) / _resultsPerPage; // floor
		}
	}

	public DataTable ResultsTable {
		get { return _Results; }
	}
	public int ResultsPerPage {
		get { return _resultsPerPage; }
		set { _resultsPerPage = value; }
	}
	public string SimilarSuggestions {
		get {
			string ret = "";

			// create the spell checker
			var spell = new SpellChecker.Net.Search.Spell.SpellChecker(FSDirectory.Open(_spellDirectory));

			// get 2 similar words
			string[] similarWords = spell.SuggestSimilar(_query, 2);

			// show the similar words
			if (similarWords.Length > 0) {
				ret = "Did you mean ";
				for (int wordIndex = 0; wordIndex < similarWords.Length; wordIndex++) {
					ret += "<a href='search.aspx?q=" + HttpContext.Current.Server.UrlEncode(similarWords[wordIndex]) + "'>" + similarWords[wordIndex] + "</a>";
					if (wordIndex + 2 < similarWords.Length) { ret += ", ";}
					if (wordIndex + 1 < similarWords.Length) { ret += " or ";}
				}
			}
			return ret;
		}
	}
	public int StartAt {
		get { return _startAt; }
	}

	/// <summary>
	/// Prepares the string with search summary information.
	/// </summary>
	public string Summary {
		get {
			if (this.TotalItems > 0) {
				return "Results <b>" + _fromItem + " - " + _toItem + "</b> of <b>" + this.TotalItems + "</b> for <b>" + _query + "</b>. (" + _duration.TotalSeconds + " seconds)";
			} else {
				return "No results found";
			}
		}
	}

	public int TotalItems {
		get { return _totalItems; }
	}
	#endregion

	#region Private Properties

	/// <summary>
	/// First item of the last page
	/// </summary>
	private int LastPageStartsAt {
		get {
			return PageCount * _resultsPerPage;
		}
	}
	#endregion

	public void traceOn() {
		_traceOn = true;
	}
	public void traceOff() {
		_traceOn = false;
	}
	public void trace(string format, params object[] options) {
		if (_traceOn) HttpContext.Current.Response.Write(string.Format(format, options) + "<br />");
	}
	public void trace(TopDocs topDocs, IndexSearcher searcher) {
		//trace("Total hits: {0}", topDocs.TotalHits);
		//trace("MaxScore: {0}", topDocs.MaxScore);

		//foreach (var hit in topDocs.ScoreDocs) {
		//	var filename = searcher.Doc(hit.Doc).Get("title");
		//	var length = searcher.Doc(hit.Doc).Get("path");
		//	trace("Matched: {0} {1} DocID=={2} Score=={3}", filename, length, hit.Doc, hit.Score);
		//}
	}

}