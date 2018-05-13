using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SearchEngine.IndexingConsole
{
    public class WebPageData
    {
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Robots { get; set; }
        public List<string> Anchors { get; set; } = new List<string>();
        public string Body { get; set; }

        public void WritToFile(string path)
        {
            using (StreamWriter writeText = new StreamWriter(path))
            {
                writeText.WriteLine(Title);
                writeText.WriteLine(Keywords);
                writeText.WriteLine(Description);
                writeText.WriteLine(Robots);
                writeText.WriteLine(string.Join(',', Anchors));
                writeText.WriteLine(Body);
            }
        }
        public static WebPageData GetPageData(string path, string siteUrl)
        {
            var pageData = new WebPageData();
            var doc = new HtmlDocument();
            doc.Load(path);

            pageData.Title = doc.DocumentNode.Descendants("title").FirstOrDefault()?.InnerText;
            var metaTags = doc.DocumentNode.Descendants("meta").ToList();
            pageData.Keywords = metaTags.FirstOrDefault(x => x.Attributes["name"]?.Value == "keywords")?.Attributes["content"]?.Value;
            pageData.Description = metaTags.FirstOrDefault(x => x.Attributes["name"]?.Value == "description")?.Attributes["content"]?.Value;
            pageData.Robots = metaTags.FirstOrDefault(x => x.Attributes["name"]?.Value == "robots")?.Attributes["content"]?.Value;
            pageData.Body = doc.DocumentNode.Descendants("body").FirstOrDefault()?.InnerText;

            var anchors = doc.DocumentNode.Descendants("a");
            var baseUrl = new Uri(siteUrl);
            foreach (var a in anchors)
            {
                var absoluteUrl = new Uri(baseUrl, a.Attributes["href"]?.Value).AbsoluteUri;
                var hashtagIndex = absoluteUrl.IndexOf("#");
                if (hashtagIndex >= 0)
                {
                    absoluteUrl = absoluteUrl.Substring(0, hashtagIndex);
                }
                pageData.Anchors.Add(absoluteUrl);
            }
            return pageData;
        }
    }
}
