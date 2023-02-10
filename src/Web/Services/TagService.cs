namespace Web.Services
{
    public class TagService
    {
        public string TagToCategory(string tag)
        {
            var category = tag.Replace('-', ' ');
            tag = tag.ToLowerInvariant();
            return category; 
        }

        public string CategoryToTag(string category)
        {
            var tag = category.Replace(' ', '-');
            tag = tag.ToLowerInvariant();
            return tag; 
        }
    }
}