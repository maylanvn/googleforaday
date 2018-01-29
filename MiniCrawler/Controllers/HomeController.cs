using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MiniCrawler.Services;
using MiniCrawler.Models;
using MiniCrawler.CrawlerServices;
using System.Data.Entity.Validation;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Concurrent;


namespace MiniCrawler.Controllers
{
    public class HomeController : Controller
    {

        int CRAWLER_DEPTH = 0;
        bool MaximumDepthReached { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ClearIndex()
        {
            SearchServices.ClearIndexSearchResults();
            return Json("All records deleted.");
        }

        public JsonResult SearchWord(string keyword)
        {
            List<WordRankVM> rankingList = SearchServices.GetWordRanking(keyword);
            return Json(rankingList, JsonRequestBehavior.AllowGet);

        }

        public JsonResult StartIndexProcess(string pageName)
        {
            try
            {
                CRAWLER_DEPTH = Int16.Parse(ConfigurationManager.AppSettings["DepthLevels"]);
                string Folder = SearchUtils.GetDirectoryForFile(pageName, -1);
                string actualPage = System.IO.Path.GetFileName(pageName);

                //create a record to serve as a groupID  for the site or group of pages to index.
                int siteIndexID = SearchServices.GetNewSiteIndex(Folder, actualPage);

                //now save the first page so that the parallel functions have links to use.
                SearchResult csr = SearchUtils.LoadPageContent(pageName, -1, siteIndexID);
                SearchUtils.GetLinksAndKeywords(csr);
                csr.PageID = SearchServices.SaveSearchResults(csr);

                //now everything is ready to run in a loop until all pages have been indexed.

                return StartCrawler(-1, siteIndexID);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;

        }

        public JsonResult StartCrawler(int parentID, int siteIndexID)
        {
            IndexResultVM finalCount;
            try
            {
                //this method runs recursively until the limit is reached.
                ConcurrentBag<SearchResult> searchResults = new ConcurrentBag<SearchResult>();
                // get the links from the saved links
                bool MaximumDepthReached = SearchServices.CanReachMaxDepth(CRAWLER_DEPTH, siteIndexID);
                if (!MaximumDepthReached)
                {
                    List<PageContentVM> pageLinksMain = SearchServices.GetLinkDataForSiteIndexID(siteIndexID);

                    //put the links into a list so that they can be run in Parallel.
                    Parallel.ForEach(pageLinksMain, (sr) =>
                {
                    string fullURL = string.Join("", sr.PageDirectory, sr.PageName);
                    SearchResult csr = SearchUtils.LoadPageContent(fullURL, sr.ParentID, siteIndexID);
                    searchResults.Add(csr);
                });

                    // now that all the links have content, do a regular loop for the parsing and saving .
                    foreach (SearchResult csr in searchResults)
                    {
                        SearchUtils.GetLinksAndKeywords(csr);
                        csr.PageID = SearchServices.SaveSearchResults(csr);
                        StartCrawler(csr.PageID, siteIndexID);
                    }
                }
            }
            catch (DbEntityValidationException)
            {
                Server.ClearError();
            }
            catch (Exception)
            {
                Server.ClearError();
            }
            finally
            {
                finalCount = SearchServices.GetIndexedPagesTotals(siteIndexID);

            }

            return Json(finalCount, JsonRequestBehavior.AllowGet);
        }
    }
}

