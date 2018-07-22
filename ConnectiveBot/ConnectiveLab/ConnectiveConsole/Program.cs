﻿using System;
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
        const string Html_ForTest = "ab\r\nc<h1 id=\"firstHeading\" class=\"firstHeading\" lang=\"ja\">Tiffany &amp; Co.</h1>xy\r\nz";

        static void Main(string[] args)
        {
            var result = GetRandomConnectedPages().GetAwaiter().GetResult();
            Console.WriteLine(result);
        }

        static async Task<string> GetRandomConnectedPages()
        {
            dynamic data1 = await GetArticle();
            dynamic data2 = await GetArticle();

            var format = GetFormat();
            var text = string.Format(format, data1.title, data2.title);

            return $"{text}\n☞ {data1.title} {data1.uri}\n☞ {data2.title} {data2.uri}";
        }

        const string Uri_Wikipedia_Random = "https://ja.wikipedia.org/wiki/%E7%89%B9%E5%88%A5:%E3%81%8A%E3%81%BE%E3%81%8B%E3%81%9B%E8%A1%A8%E7%A4%BA";

        static async Task<object> GetArticle()
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(Uri_Wikipedia_Random);
                var responseBody = await response.Content.ReadAsStringAsync();
                var title = GetTitle(responseBody);
                var uri = FormatUri(response.RequestMessage.RequestUri);

                return new { title, uri };
            }
        }

        static string GetTitle(string html)
        {
            var match = Regex.Match(html, "<h1 [^>]*id=\"firstHeading\"[^>]*>([^<]+)</h1>");
            if (!match.Success) return null;

            var title = match.Groups[1].Value;
            return WebUtility.HtmlDecode(title);
        }

        static string FormatUri(Uri uri)
        {
            var original = uri.Segments.Last();
            var escaped = Uri.EscapeDataString(Uri.UnescapeDataString(original));
            return $"https://ja.wikipedia.org/wiki/{escaped}";
        }

        static readonly string[] formats =
        {
            "{0}と{1}",
            "{0}は{1}なり",
            "{0}は{1}から",
            "{0}からの{1}",
            "{0}の上の{1}",
            "{0}の中の{1}",
        };

        static readonly Random random = new Random();

        static string GetFormat()
        {
            var i = random.Next(0, formats.Length);
            return formats[i];
        }
    }
}
