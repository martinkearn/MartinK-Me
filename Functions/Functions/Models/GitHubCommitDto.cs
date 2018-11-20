using System;
using System.Collections.Generic;
using System.Text;

namespace Functions.Models
{
    public class GitHubCommitDto
    {
        public string sha { get; set; }
        public string node_id { get; set; }

        public string url { get; set; }
        public string html_url { get; set; }
        public string comments_url { get; set; }

        public List<File> files { get; set; }
    }

    public class File
    {
        public string sha { get; set; }
        public string filename { get; set; }
        public string status { get; set; }
        public int additions { get; set; }
        public int deletions { get; set; }
        public int changes { get; set; }
        public string blob_url { get; set; }
        public string raw_url { get; set; }
        public string contents_url { get; set; }
        public string patch { get; set; }
        public string previous_filename { get; set; }
    }
}
