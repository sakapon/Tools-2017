using System.Net;
using System.Text.RegularExpressions;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("Started.");

    dynamic data1 = await GetArticle();
    log.Info(data1.title);
    dynamic data2 = await GetArticle();
    log.Info(data2.title);

    var format = GetFormat();
    log.Info(format);
    var text = string.Format(format, data1.title, data2.title);

    var message = $"{text}\n? {data1.title} {data1.uri}\n? {data2.title} {data2.uri}";
    return req.CreateResponse(HttpStatusCode.OK, message);
}

const string Uri_Wikipedia_Random = "https://ja.wikipedia.org/wiki/%E7%89%B9%E5%88%A5:%E3%81%8A%E3%81%BE%E3%81%8B%E3%81%9B%E8%A1%A8%E7%A4%BA";

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
