using HtmlAgilityPack;

namespace MiniCrawler.Services
{
    static class ExtensorMethodHAP
    {
        public static HtmlNodeCollection NullToEmptyCollection(this HtmlNodeCollection self)
        {
            if (self == null)
                return new HtmlNodeCollection(null);
            else
                return self;
        }

    }
}