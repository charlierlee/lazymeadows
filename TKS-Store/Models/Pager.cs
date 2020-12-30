using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TKS.Models {
	public class Pager {
		public string GetPager(string BaseURL, int ItemCount, int pg, int sz, string ExtraParams = "") {
			//Setup pager
			int TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(ItemCount) / sz));
			if (pg > TotalPages) { pg = TotalPages; }
			string path = BaseURL + "?pg={PG}&sz=" + sz.ToString() + ExtraParams;

			StringBuilder pager = new StringBuilder();
			if (TotalPages > 1) {
				pager.AppendLine("<div class='pagination'>");
				if (pg > 1) {
					pager.AppendLine("<a href='" + path.Replace("{PG}", "1") + "' class='page'><span class='el-icon-fast-backward icon'></span></a>&nbsp;");
				}
				if (pg > 2) {
					pager.AppendLine("<a href='" + path.Replace("{PG}", (pg - 1).ToString()) + "' class='page'><span class='el-icon-step-backward icon'></span></a>&nbsp;");
				}
				for (int x = 1; x <= TotalPages; x++) {
					if (x == pg) {
						pager.AppendLine("<span class='page active'>" + x.ToString() + "</span>&nbsp;");
					} else {
						pager.AppendLine("<a href='" + path.Replace("{PG}", x.ToString()) + "' class='page'>" + x.ToString() + "</a>&nbsp;");
					}
				}
				if (pg < TotalPages - 1) {
					pager.AppendLine("<a href='" + path.Replace("{PG}", (pg + 1).ToString()) + "' class='page'><span class='el-icon-step-forward icon'></span></a>&nbsp;");
				}
				if (pg < TotalPages) {
					pager.AppendLine("<a href='" + path.Replace("{PG}", TotalPages.ToString()) + "' class='page'><span class='el-icon-fast-forward icon'></span></a>");
				}
				pager.AppendLine("</div>");
			}

			return pager.ToString();
		}
	}
}