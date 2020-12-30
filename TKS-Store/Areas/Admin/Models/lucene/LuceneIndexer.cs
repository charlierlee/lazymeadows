/*
 * Copyright 2012 dotlucene.net
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SpellChecker.Net.Search.Spell;


namespace Indexer
{
	public class LuceneIndexer
	{
		public DirectoryInfo IndexDirectoryInfo { get; set; }
		public LockFactory LockFactory { get; set; }

		#region Fields
		private IndexWriter _writer;
		private string _docRootDirectory;
		private string _indexRootDirectory = "";
		private string _spellRootDirectory = "";
		private string _pattern;
		private bool _traceOn = false;
		#endregion

		#region Constructor
		public LuceneIndexer() {
			_indexRootDirectory = HttpContext.Current.Server.MapPath("~/assets/index");
			_spellRootDirectory = HttpContext.Current.Server.MapPath("~/assets/spell");
			IndexDirectoryInfo = new DirectoryInfo(_indexRootDirectory);
			LockFactory = new SimpleFSLockFactory();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates a new index in _indexRootDirectory. Does not Overwrite the existing index in that directory.
		/// </summary>
		public void CreateIndexWriter() {
			try {
				_writer = new IndexWriter(
					FSDirectory.Open(IndexDirectoryInfo, LockFactory), 
					new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), 
					IndexWriter.MaxFieldLength.UNLIMITED);
			} catch (LockObtainFailedException ex) {
				DirectoryInfo indexDirInfo = new DirectoryInfo(_indexRootDirectory);
				FSDirectory indexFSDir = FSDirectory.Open(indexDirInfo, new Lucene.Net.Store.SimpleFSLockFactory(indexDirInfo));
				IndexWriter.Unlock(indexFSDir);
				_writer = new IndexWriter(FSDirectory.Open(_indexRootDirectory), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);
				trace("Error: {0}", ex.Message);
			} 
			
			_writer.UseCompoundFile = true;

			trace("Created IndexWriter");
		}

		/// <summary>
		/// Add HTML files from <c>directory</c> and its subdirectories that match <c>pattern</c>.
		/// </summary>
		/// <param name="directory">Directory with the HTML files.</param>
		/// <param name="pattern">Search pattern, e.g. <c>"*.html"</c></param>
		public void AddDirectory(DirectoryInfo directory, string pattern)
		{
			_docRootDirectory = directory.FullName;
			_pattern = pattern;

			addSubDirectory(directory);
		}

		public void AddFile(string filename, float? boost = null) {
			// check that an index writer has been created
			if (_writer == null) throw new Exception("IndexWriter not created");

			var fileInfo = new FileInfo(filename);
			if (!fileInfo.Exists) throw new Exception(string.Format("File not found: {0}", filename));

			trace("Indexing: {0}", fileInfo.FullName);

			var start = DateTime.Now;

			// create a lucene document
			var doc = new Document();

			doc.Add(new Field("filename", fileInfo.Name,
						  Field.Store.YES,
						  Field.Index.NOT_ANALYZED,
						  Field.TermVector.YES));
			trace("Added filename for: {0}", fileInfo.FullName);

			doc.Add( new Field("length", fileInfo.Length.ToString(),
					Field.Store.YES,
					Field.Index.NOT_ANALYZED,
					Field.TermVector.YES));
			trace("Added length for: {0} {1}", fileInfo.FullName, fileInfo.Length);

			doc.Add( new NumericField("NumericLength", Field.Store.YES, true).SetLongValue(fileInfo.Length));
			trace("Added NumericLength for: {0} {1}", fileInfo.FullName, fileInfo.Length);

			doc.Add( new NumericField("DateIndexed", Field.Store.NO, true).SetLongValue(DateTime.Today.Ticks));
			trace("Added DateIndexed for: {0}", DateTime.Today.Ticks.ToString());

			// open a stream reader that will read the contents of the file
			var contents = new StreamReader(fileInfo.FullName);
			// add three fields to the index for this document
			doc.Add(new Field("contents", contents));
			trace("Added content for: {0}", fileInfo.FullName);

			// if boost specified, then assign it
			if (boost != null) {
				trace("Boost=={0}", boost);
				doc.Boost = boost.Value;
			}

			// tell the index writer to store what we've indexed
			_writer.AddDocument(doc);

			trace("Document created and indexed in {0}ms", (DateTime.Now - start).TotalMilliseconds);
		}
		public void AddFiles(IEnumerable<string> filenames) {
			var start = DateTime.Now;
			foreach (var filename in filenames) {
				AddFile(filename);
			}
			trace("Indexed in {0}ms", (DateTime.Now - start).TotalMilliseconds);
		}

		/// <summary>
		/// Loads, parses and indexes an HTML file.
		/// </summary>
		/// <param name="path"></param>
		public void AddHtmlDocument(string path) {
			Document doc = new Document();

			string html;
			using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default)) {
				html = sr.ReadToEnd();
			}

			int relativePathStartsAt = _docRootDirectory.EndsWith("\\") ? _docRootDirectory.Length : _docRootDirectory.Length + 1;
			string relativePath = path.Substring(relativePathStartsAt);

			doc.Add(new Field("text", ParseHtml(html), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("path", relativePath, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("title", GetTitle(html), Field.Store.YES, Field.Index.ANALYZED));

			_writer.AddDocument(doc);
		}

		/// <summary>
		/// Loads, parses and indexes a web page.
		/// </summary>
		/// <param name="url"></param>
		public void AddWebPage(string url) {
			WebClient myWebClient = new WebClient();
			byte[] myDataBuffer = myWebClient.DownloadData(url);
			string html = Encoding.ASCII.GetString(myDataBuffer);

			Document doc = new Document();
			doc.Add(new Field("text", ParseHtml(html), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("path", url, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("title", GetTitle(html), Field.Store.YES, Field.Index.ANALYZED));

			_writer.AddDocument(doc);
		}
		/// <summary>
		/// Loads, parses and indexes a web page.
		/// </summary>
		/// <param name="url"></param>
		public void AddWebPage(string id, string url, string title, string contents, string type) {
			// check that an index writer has been created
			if (_writer == null) throw new Exception("IndexWriter not created");

			Document doc = new Document();
			doc.Add(new Field("id", id, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("text", title + " " + ParseHtml(contents), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("path", url, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("type", type, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("title", title, Field.Store.YES, Field.Index.NOT_ANALYZED));

			_writer.AddDocument(doc);
		}

		public void IndexWords() {
			// open the index reader
			IndexReader indexReader = IndexReader.Open(FSDirectory.Open(_indexRootDirectory), true);

			// create the spell checker
			var spell = new SpellChecker.Net.Search.Spell.SpellChecker(FSDirectory.Open(_spellRootDirectory));

			// add all the words in the field description to the spell checker
			spell.IndexDictionary(new LuceneDictionary(indexReader, "text"));
		}

		public void Delete(string ID) {
			// check that an index writer has been created
			if (_writer == null) throw new Exception("IndexWriter not created");

			_writer.DeleteDocuments(new TermQuery(new Term("id", ID)));
		}
		public void UpdateWebPage(string ID, string url, string title, string contents, string type) {
			// check that an index writer has been created
			if (_writer == null) throw new Exception("IndexWriter not created");

			Document doc = new Document();
			doc.Add(new Field("id", ID, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("text", ParseHtml(contents), Field.Store.YES, Field.Index.ANALYZED));
			doc.Add(new Field("path", url, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("type", type, Field.Store.YES, Field.Index.NOT_ANALYZED));
			doc.Add(new Field("title", title, Field.Store.YES, Field.Index.NOT_ANALYZED));

			_writer.UpdateDocument(new Term("id", ID), doc);
		}


		/// <summary>
		/// Optimizes and save the index.
		/// </summary>
		public void Close() {
			if (_writer != null) {
				_writer.Optimize();
				_writer.Dispose();
				_writer = null;
			}
			trace("Closed writer");
		}

		public void DeleteIndex(bool DeleteDirectory) {
			Close();

			if (IndexDirectoryInfo == null) throw new Exception("Index directory not specified");
			if (IndexDirectoryInfo.Exists == false) return;

			foreach (var fileInfo in IndexDirectoryInfo.GetFiles()) {
				fileInfo.Delete();
			}

			if (DeleteDirectory) { IndexDirectoryInfo.Delete(); }

			trace("Deleted existing index at {0}", IndexDirectoryInfo.FullName);

			// must make new object as the old one knows it was deleted and will cause an error later
			IndexDirectoryInfo = new DirectoryInfo(IndexDirectoryInfo.FullName);
		}

		public void traceOn() {
			_traceOn = true;
		}
		public void traceOff() {
			_traceOn = false;
		}
		#endregion

		#region Private Methods
		private void addSubDirectory(DirectoryInfo directory) {
			foreach (FileInfo fi in directory.GetFiles(_pattern)) {
				AddHtmlDocument(fi.FullName);
			}
			foreach (DirectoryInfo di in directory.GetDirectories()) {
				addSubDirectory(di);
			}
		}

		/// <summary>
		/// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
		/// </summary>
		/// <param name="html">HTML document</param>
		/// <returns>Plain text.</returns>
		private static string ParseHtml(string html) {
			string temp = Regex.Replace(html, "<[^>]*>", "");
			return temp.Replace("&nbsp;", " ");
		}

		/// <summary>
		/// Finds a title of HTML file. Doesn't work if the title spans two or more lines.
		/// </summary>
		/// <param name="html">HTML document.</param>
		/// <returns>Title string.</returns>
		private static string GetTitle(string html) {
			Match m = Regex.Match(html, "<title>(.*)</title>");
			if (m.Groups.Count == 2)
				return m.Groups[1].Value;
			return "(unknown)";
		}

		private void trace(string format, params object[] options) {
			if (_traceOn) HttpContext.Current.Response.Write(string.Format(format, options) + "<br />");
		}
		//public void trace(TopDocs topDocs, IndexSearcher searcher) {
		//	trace("Total hits: {0}", topDocs.TotalHits);
		//	trace("MaxScore: {0}", topDocs.MaxScore);

		//	foreach (var hit in topDocs.ScoreDocs) {
		//		var filename = searcher.Doc(hit.Doc).Get("filename");
		//		var length = searcher.Doc(hit.Doc).Get("length");
		//		trace("Matched: {0} {1} DocID=={2} Score=={3}", filename, length, hit.Doc, hit.Score);
		//	}
		//}
		#endregion

	}
}
