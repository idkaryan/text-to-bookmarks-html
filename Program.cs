using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;

namespace TextToBookmarksHtml {
    class Program {
        static void Main(string[] args) {

            var txtFile = File.ReadAllLines("./yt.txt");
            var bookmarksMap = new Dictionary<string, List<Link>>();

            var doc = new HtmlDocument();
            var parentHtmlNode = HtmlNode.CreateNode("<html></html>");
            doc.DocumentNode.AppendChild(parentHtmlNode);

            // CreateDTForHeader("Googe", ref parentHtmlNode);
            //parentHtmlNode.AppendChild(CreateDT("http://google.com", "Google"));

            var lastHeading = "";
            foreach (var line in txtFile) {
                var sharpIndex = line.IndexOf("#");
                var httpIndex = line.IndexOf("http");
                if (sharpIndex != -1) {
                    // HEADING FOUND
                    lastHeading = line.Substring(sharpIndex + 2);
                    bookmarksMap.Add(line.Substring(sharpIndex + 2), new List<Link>());
                } else if (httpIndex != -1) {
                    // LINK FOUND FOR HEADING
                    bookmarksMap[lastHeading].Add(new Link() {
                        Href = line.Substring(httpIndex), Title = line.Substring(0, httpIndex - 1)
                    });
                }
            }

            foreach (KeyValuePair<string, List<Link>> item in bookmarksMap) {
                var header = item.Key;
                var linkList = item.Value;
                CreateDTForHeader(header, ref parentHtmlNode, linkList);
            }

            doc.Save("./bookmarks.html");

        }

        static HtmlNode CreateDT_A(string href, string title) {
            var dtNode = HtmlNode.CreateNode("<DT>");
            var aNode = HtmlNode.CreateNode("<A></A>");
            var textNode = HtmlTextNode.CreateNode(title);
            aNode.SetAttributeValue("HREF", href);
            aNode.AppendChild(textNode);
            dtNode.AppendChild(aNode);
            return dtNode;
        }

        static void CreateDTForHeader(string title, ref HtmlNode parentNode, List<Link> items) {
            var titleDT = HtmlNode.CreateNode("<DT>");
            var titleH3 = HtmlNode.CreateNode("<H3>");
            var titleText = HtmlTextNode.CreateNode(title);
            titleDT.AppendChild(titleH3);
            titleH3.AppendChild(titleText);

            var dlNode = HtmlNode.CreateNode("<DL>");

            foreach (var item in items) {
                var itemNode = CreateDT_A(item.Href, item.Title);
                dlNode.AppendChild(itemNode);
            }

            parentNode.AppendChild(titleDT);
            parentNode.AppendChild(dlNode);
        }
    }
}