﻿using System;
namespace Domain.Interfaces
{
    /// <summary>
    /// An interface to define an Article.
    /// </summary>
	public interface IArticle
	{
        /// <summary>
        /// The unique ID and Azure Storage Table entity key. Typically a base64 encoded version of the Github path, for example blogs/Test.md will result in a key of YmxvZ3MvdGVzdC5tZA==
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The title of the article
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The author of the article
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// The description of the article
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// An absolute Url to an image to be used in the article itself
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// An absolute Url to a thumbnail image to be used in when listing the article amogst others
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// The date the article was published
        /// </summary>
        public DateTime Published { get; set; }

        /// <summary>
        /// A comma delimited list of tags or categories for the article
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// The status of the article, Typically either "draft" or "published"
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// A web friendly path for the article based on Title which will be used to form the web url. If the Title is "Coding for Dummies", the path will be "coding-for-dummies"
        /// </summary>
        public string WebPath { get; set; }

        /// <summary>
        /// The absolute uri to the related html blob in Azure Storage. Does not require key or sas token.
        /// </summary>
        public string HtmlBlobPath { get; set; }

        /// <summary>
        /// The absolute uri to the related markdown file on GitHub.
        /// </summary>
        public string GitHubUrl { get; set; }


        /// <summary>
        /// The file name of the related html blob in Azure Storage.
        /// </summary>
        public string HtmlBlobFileName { get; set; }       
    }
}

