using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SEOAnalyser.Models
{
    public class AnalyseContentModel
    {
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Text or URL")]
        public string SearchInput { get; set; }
        public bool FilterStopWord { get; set; } = true;
        public bool CalculateWordOccurence { get; set; } = true;
        public bool CalculateWordOccurenceInMetaTag { get; set; } = true;
        public bool CalculateExternalLink { get; set; } = true;
        public bool HasNoValidationChecked { get; set; }
        public bool HasInternalError { get; set; }
        public string Content { get; set; }
        public bool IsValidModel { get; set; }

        public Dictionary<string, int> WordOccurences { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> WordOccurencesFromMetaTags { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ExternalLinks { get; set; } = new Dictionary<string, int>();
    }
}