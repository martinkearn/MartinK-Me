using System;

namespace MartinKMe.Functions.Models
{
    public class YamlToMarkdownActivityInput
    {
        public string FileContents { get; set; }

        public Uri BlobPath { get; set; }

        public Uri GithubPath { get; set; }
    }
}
