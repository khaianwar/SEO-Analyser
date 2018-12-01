using SEOAnalyser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEOAnalyser
{
    public class ViewConstants
    {
        public const string FilterStopWord_Label = "Filter stop-words (‘or’, ‘and’, ‘a’, ‘the’ etc)";
        public const string CalculateWordOccurence_Label = "Calculates word occurrences";
        public const string CalculateWordOccurenceInMetaTag_Label = "Calculates word occurrences in meta tags";
        public const string CalculateExternalLink_Label = "Extract external links";

        public const string SearchInput_Placeholder = "Please enter a text in English or URL";
    }
}