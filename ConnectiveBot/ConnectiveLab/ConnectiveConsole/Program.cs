using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConnectiveConsole
{
    class Program
    {
        const string Uri_Wikipedia_Random = "https://ja.wikipedia.org/wiki/%E7%89%B9%E5%88%A5:%E3%81%8A%E3%81%BE%E3%81%8B%E3%81%9B%E8%A1%A8%E7%A4%BA";

        const string Html_ForTest = "ab\r\nc<h1 id=\"firstHeading\" class=\"firstHeading\" lang=\"ja\">Tiffany &amp; Co.</h1>xy\r\nz";

        static void Main(string[] args)
        {
            var t = GetArticle();
            t.Wait();
            Console.WriteLine(t.Result);
        }

        static async Task<object> GetArticle()
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(Uri_Wikipedia_Random);
                var responseBody = await response.Content.ReadAsStringAsync();
                var title = GetTitle(responseBody);

                return new { title, uri = response.RequestMessage.RequestUri.AbsoluteUri };
            }
        }

        static string GetTitle(string html)
        {
            var match = Regex.Match(html, "<h1 [^>]*id=\"firstHeading\"[^>]*>([^<]+)</h1>");
            if (!match.Success) return null;

            var title = match.Groups[1].Value;
            return WebUtility.HtmlDecode(title);
        }
    }
}
