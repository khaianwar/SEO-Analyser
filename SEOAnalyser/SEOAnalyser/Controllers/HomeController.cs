using SEOAnalyser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SEOAnalyser.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            AnalyseContentModel model = new AnalyseContentModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AnalyseContentModel model)
        {
            model.HasNoValidationChecked = !(model.FilterStopWord ||
                model.CalculateWordOccurence || model.CalculateWordOccurenceInMetaTag ||
                model.CalculateExternalLink);

            if (model.HasNoValidationChecked)
            {
                ModelState.AddModelError(nameof(model.HasNoValidationChecked), "Please select at least one validation");
            }

            model.IsValidModel = ModelState.IsValid;

            if (model.IsValidModel)
            {
                try
                {
                    PopulateContent(model);
                    RemoveStopWords(model);
                    PopulateWordOccurences(model);
                    PopulateWordOccurencesInMetaTag(model);
                    PopulateExternalLinks(model);
                }
                catch (AggregateException ex)
                {
                    model.IsValidModel = false;
                    if (ex.InnerException != null && ex.InnerException.InnerException != null)
                    {
                        ModelState.AddModelError(nameof(model.HasInternalError), ex.InnerException.InnerException.Message);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.HasInternalError), ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    model.IsValidModel = false;
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError(nameof(model.HasInternalError), ex.InnerException);
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(model.HasInternalError), ex.Message);
                    }
                }
            }

            return View(model);
        }

        private void PopulateContent(AnalyseContentModel model)
        {
            if (IsValidURL(model.SearchInput))
            {
                model.Content = GetHtmlResultFromURL(model.SearchInput);
            }
            else
            {
                model.Content = model.SearchInput;
            }
        }

        private string GetHtmlResultFromURL(string url)
        {
            using (var client = new HttpClient())
            {
                Uri uri = new Uri(url);

                HttpResponseMessage response = client.GetAsync(uri).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                throw new ArgumentException("Invalid input URL");
            }
        }

        private void RemoveStopWords(AnalyseContentModel model)
        {
            if (!model.FilterStopWord)
            {
                return;
            }

            IList<string> stopWords = new List<string>()
                {
                    "a","about", "above", "after", "again", "against",
                    "all", "am", "an", "and", "any", "are", "as", "at",
                    "be", "because", "been", "before", "being", "below", "between", "both", "but", "by",
                    "could",
                    "did", "do", "does", "doing", "down", "during",
                    "each",
                    "few", "for", "from", "further",
                    "had", "has", "have", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's",
                    "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "it", "it's", "its", "itself", "let's",
                    "me", "more", "most", "my", "myself",
                    "nor",
                    "of", "on", "once", "only", "or", "other", "ought", "our", "ours", "ourselves", "out", "over", "own",
                    "same", "she", "she'd", "she'll", "she's", "should", "so", "some", "such",
                    "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too",
                    "under", "until", "up",
                    "very",
                    "was", "we", "we'd", "we'll", "we're", "we've", "were", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "would",
                    "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves"
                };

            string[] words = model.Content.Split(' ');
            model.Content = string.Join(" ",
                words.Where(o => !string.IsNullOrEmpty(o) && !stopWords.Contains(o)));
        }

        private void PopulateWordOccurences(AnalyseContentModel model)
        {
            if (!model.CalculateWordOccurence)
            {
                return;
            }

            string plainTextContent = HtmlToPlainText(model.Content);
            model.WordOccurences = GetWordByCount(plainTextContent);
        }

        private void PopulateWordOccurencesInMetaTag(AnalyseContentModel model)
        {
            if (!model.CalculateWordOccurenceInMetaTag)
            {
                return;
            }

            Regex metaTag = new Regex(@"<meta.*?>");
            Dictionary<string, string> metaInformation = new Dictionary<string, string>();

            string metaData = string.Empty;
            foreach (Match m in metaTag.Matches(model.Content))
            {
                metaData += " " + m.Value;
            }

            model.WordOccurencesFromMetaTags = GetWordByCount(metaData);
        }

        private void PopulateExternalLinks(AnalyseContentModel model)
        {
            if (!model.CalculateExternalLink)
            {
                return;
            }

            string[] words = model.Content.Split(' ');
            Regex metaTag = new Regex(@"(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?");

            string urlData = string.Empty;
            foreach (Match m in metaTag.Matches(model.Content))
            {
                urlData += " " + m.Value;
            }

            model.ExternalLinks = GetWordByCount(urlData);
        }

        private bool IsValidURL(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }

        private Dictionary<string, int> GetWordByCount(string contents)
        {
            string[] words = contents.Split(' ');
            return words.Where(o => !string.IsNullOrEmpty(o)).GroupBy(o => o)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        private string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
            const string stripFormatting = @"<[^>]*(>|$)";
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            text = tagWhiteSpaceRegex.Replace(text, "><");
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            text = stripFormattingRegex.Replace(text, string.Empty);
            text = System.Net.WebUtility.HtmlDecode(text);

            return text;
        }
    }
}