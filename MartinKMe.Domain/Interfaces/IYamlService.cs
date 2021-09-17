using MartinKMe.Domain.Models;
using System;

namespace MartinKMe.Domain.Interfaces
{
    public interface IYamlService
    {
        public Article YamlToArticle(string yaml, Uri blobUri, string githubPath);
    }
}
