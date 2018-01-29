using System;
using System.Collections.Generic;
using System.Linq;
using MiniCrawler.Models;
using System.IO;
using System.Data.Entity.Validation;

namespace MiniCrawler.CrawlerServices
{
    public class SearchServices
    {
        public static CrawlerDB DB = new CrawlerDB();

        public static PageContentVM GetPageInfo(int pageID)
        {
            var pageInfo = (from pg in DB.IndexedPages
                            where pg.PageID == pageID
                            select new PageContentVM
                            {
                                PageID = pg.PageID,
                                PageURL = pg.PageURL,
                                ParentID = pg.ParentID.Value,
                                PageDirectory = pg.ParentDirectory,
                                PageName = pg.PageName

                            }).First();
            return pageInfo;

        }

        public static IndexResultVM GetIndexedPagesTotals(int IndexedSiteID)
        {
            IndexResultVM st = new IndexResultVM();
            try
            {
                var pgCount = (from px in DB.IndexedPages
                               where px.IndexedSiteID == IndexedSiteID
                               group px by px.PageURL into gr1
                               select new { myKey = gr1.Key, mycount = gr1.Count() }).ToList();
                st.PagesIndexed = pgCount.Sum(g => g.mycount);

                var kwCount = (from p in DB.IndexedPages
                               join pkw in DB.PageKeyWords
                               on p.PageID equals pkw.PageID
                               where p.IndexedSiteID == IndexedSiteID
                               group p by p.PageName into gp
                               select new { myKey = gp.Key, myKWCount = gp.Count() }).ToList();
                st.KeywordsIndexed = kwCount.Sum(c => c.myKWCount);
                return st;
            }
            catch (Exception)
            {
                return null;

            }

        }

        public static bool CanReachMaxDepth(int someLevel, int siteIndexID)
        {
            var r = from pid in DB.IndexedPages
                    where pid.IsIndexed == true
                    && pid.IndexedSiteID == siteIndexID
                    select new { pid.ParentID };
            if (r.Any())
            {
                return r.Distinct().Count() >= someLevel;
            }
            else
            {
                return false;
            }
        }

        public static List<WordRankVM> GetWordRanking(string keyWord)
        {
            try
            {
                var results = (from pg in DB.IndexedPages
                               join pgLinks in DB.PageKeyWords
                               on pg.PageID equals pgLinks.PageID

                               where pgLinks.Keyword.Contains(keyWord) || pgLinks.Keyword.StartsWith(keyWord)
                               || null == keyWord
                               group new { pg, pgLinks } by pg.PageURL into grup1
                               select new WordRankVM
                               {
                                   PageURL = grup1.FirstOrDefault().pg.PageURL,
                                   Title = grup1.FirstOrDefault().pg.Title,
                                   Rank = grup1.Sum(g => g.pgLinks.KeywordCount.Value)
                               }).ToList();


                return results;
            }
            catch (DbEntityValidationException)
            {

                return null;
            }
            catch (Exception)
            {

                return null;

            }
        }

        public static List<PageContentVM> GetLinkDataForSiteIndexID(int IndexedSiteID)
        {
            var result = (from pg in DB.IndexedPages
                          where pg.IndexedSiteID == IndexedSiteID
                          && pg.IsIndexed != true
                          select new PageContentVM
                          {
                              PageID = pg.PageID,
                              PageURL = pg.PageURL,
                              ParentID = pg.ParentID.Value,
                              PageName = pg.PageName,
                              PageDirectory = pg.ParentDirectory,
                              IndexedSiteID = pg.IndexedSiteID
                          }).ToList();

            return result;
        }


        //this resets the system by clearing all tables.
        public static void ClearIndexSearchResults()
        {

            //use stored proc.. This is too slow.
            var keywordList = DB.PageKeyWords.ToList();

            DB.PageKeyWords.RemoveRange(keywordList);
            DB.SaveChanges();


            var links = DB.IndexedPages.ToList();

            DB.IndexedPages.RemoveRange(links);
            DB.SaveChanges();

            var sites = DB.IndexedSites.ToList();
            DB.IndexedSites.RemoveRange(sites);
            DB.SaveChanges();

        }

        //is the current page already indexed or not?
        //This is used to decide whether to index a link that came from a parent page.
        public static bool IsPageContentIndexed(string pageURL, string pageName)
        {
            Uri siteURL = new Uri(pageURL);
            string domainName = siteURL.GetLeftPart(UriPartial.Authority);
            var result = (from p in DB.IndexedPages
                          where p.PageURL.ToLower() == pageURL.ToLower()

                        && p.IsIndexed == true
                          select p).ToList();
            return result.Any();


        }

        //has the page been saved alread?
        //A link might have been inserted already. This avoids duplicates.
        public static bool IsPageAlreadySaved(string pageURL, string pageName)
        {
            try
            {
                Uri siteURL = new Uri(pageURL);
                string domainName = siteURL.GetLeftPart(UriPartial.Authority);
                var result = (from p in DB.IndexedPages
                              where p.PageName.ToLower() == pageName.ToLower()
                             && p.PageURL.StartsWith(domainName)
                             || p.PageURL == pageURL
                              select p).ToList();
                return result.Any();
            }

            catch (Exception)
            {

                return true;
            }

        }


        private static string RemoveEndingSlash(string URL)
        {
            string tempURL = URL;
            if (URL.EndsWith("/") || URL.EndsWith(@"\"))
            {
                tempURL = URL.Remove(URL.Length - 1, 1);
            }
            return tempURL;
        }
        //helper for building the URL of a page when it comes without the HTTP.
        //Get name of page, sometimes without the previous folder in front of it.
        public static string GetFileWithFolder(string singleLink)
        {
            string tempLink = RemoveEndingSlash(singleLink);



            if (singleLink.Contains("http"))
            {
                return Path.GetFileName(tempLink);
            }
            else
            {
                return singleLink;

            }
        }

        public static string GetFullURLFromName(string pageName, string parentDirectory)
        {

            if (parentDirectory.EndsWith("/"))
            {
                return string.Join("", parentDirectory, pageName);
            }
            else
            {
                return string.Join("/", parentDirectory, pageName);
            }
        }

        public static void SaveLinks(SearchResult searchResults, IndexedPages pg)
        {
            List<IndexedPages> linkPages = new List<IndexedPages>();
            try
            {

                foreach (string singleLink in searchResults.Links)
                {
                    IndexedPages cp = new IndexedPages();
                    if (singleLink.Length > 1) //to avoid root (/)
                    {
                        cp.DateCreated = DateTime.Now;
                        cp.ParentID = pg.PageID;
                        cp.PageName = GetFileWithFolder(singleLink);
                        cp.IndexedSiteID = pg.IndexedSiteID;
                        //get directory for the file, not only the filename.
                        cp.ParentDirectory = Services.SearchUtils.GetDirectoryForFile(singleLink, pg.PageID);
                        cp.PageURL = GetFullURLFromName(cp.PageName, cp.ParentDirectory);
                        cp.Title = ""; // THIS COMES ONLY FROM THE CONTENT;


                        // code to avoid duplicates.

                        if (IsValidLink(cp.PageURL) && !SearchServices.IsPageAlreadySaved(cp.PageURL, cp.PageName))
                        {

                            linkPages.Add(cp);
                        }
                    }
                }

                DB.IndexedPages.AddRange(linkPages);
                DB.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (Exception)
            {

            }

        }

        //some links are on the same page or is only the domain page..skip these.
        public static bool IsValidLink(string pageURL)
        {   // if the url is too short 
            //or is the same as the domain this will throw an error
            //and it can be skipped.

            try
            {
                Uri siteURL = new Uri(pageURL);
                string domainName = siteURL.GetLeftPart(UriPartial.Authority);

                if (pageURL.StartsWith("#"))
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;

            }

            return true;
        }


        //save each word and the # of times it occurs, word by word found on a parent page.
        public static void SaveTheKeywords(SearchResult searchResults, IndexedPages pg)
        {
            List<PageKeyWords> keywordRankingList = new List<PageKeyWords>();
            try
            {
                //save the keywords for this page.
                foreach (WordRankVM kw in searchResults.KeyWordRankingList)
                {
                    PageKeyWords pkw = new PageKeyWords();
                    pkw.PageID = pg.PageID;
                    pkw.Keyword = kw.Keyword;
                    pkw.KeywordCount = kw.Rank;
                    keywordRankingList.Add(pkw);
                }
                DB.PageKeyWords.AddRange(keywordRankingList);
                DB.SaveChanges();
            }
            catch (Exception)
            {
            }
        }


        //Update the page to Indexed so it will not be searched again.
        public static void UpdateIsIndexedFlag(int pageID)
        {//TODO: add code to handle failed page updates.
            var result = (from p in DB.IndexedPages
                          where p.PageID == pageID
                          select p).First();

            result.IsIndexed = true;
            DB.SaveChanges();
        }

        //Get the data for a page so it can be indexed.
        //used for requesting the content based on the URL.
        public static IndexedPages GetPageByName(string pageURL, string pageName)
        {

            try
            {
                Uri siteURL = new Uri(pageURL);
                string domainName = siteURL.GetLeftPart(UriPartial.Authority);
                var result = (from p in DB.IndexedPages
                              where p.PageName.ToLower() == pageName.ToLower()
                             && p.PageURL.StartsWith(domainName)
                             || p.PageURL == pageURL
                              select p).ToList();
                if (result.Any())
                {
                    return result.First();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;

            }

        }

        //get a group index for all the pages in a "run"
        //The IndexedSiteID is a grouping of all the pages that are indexed for a site.
        public static int GetNewSiteIndex(string domain, string page)
        { // insert to the SiteIndexes table

            IndexedSites ix = new IndexedSites();
            ix.Domain = domain;
            ix.InitialPage = page;
            DB.IndexedSites.Add(ix);
            DB.SaveChanges();
            return ix.IndexedSiteID;

        }


        //Main Method that calls several Save methods for the content, links and keywords of a page.
        public static int SaveSearchResults(SearchResult searchResults)
        {
            try
            {


                int pageIDAfterInsert = 0;
                //save the  page 
                IndexedPages pg = new IndexedPages();

                pg.DateCreated = DateTime.Now;
                pg.ParentID = searchResults.ParentID;
                pg.PageName = GetFileWithFolder(searchResults.PageURL);
                pg.PageURL = searchResults.PageURL;
                pg.ParentDirectory = searchResults.ParentDirectory;
                pg.IndexedSiteID = searchResults.IndexedSiteID;
                pg.Title = searchResults.Title.Length > 50 ? searchResults.Title.Substring(0, 49) : searchResults.Title;
                if (!IsPageAlreadySaved(pg.PageURL, pg.PageName))
                {
                    DB.IndexedPages.Add(pg);
                    DB.SaveChanges();
                    pageIDAfterInsert = pg.PageID;
                }
                else
                {   //the page already exists so add a few missing fields.


                    pg = GetPageByName(pg.PageURL, pg.PageName);
                    pg.DateCreated = DateTime.Now;

                    pg.Title = searchResults.Title;

                    pageIDAfterInsert = pg.PageID;
                    DB.SaveChanges();
                }



                SaveLinks(searchResults, pg); //save the links for this page.


                SaveTheKeywords(searchResults, pg); //save the keywords


                UpdateIsIndexedFlag(pg.PageID);   //update the IsIndexed flag so it is not run again.


                return pageIDAfterInsert;
            }
            catch (DbEntityValidationException)
            {
                return 0;
            }
            catch (Exception)
            {

                return 0;

            }
        }
    }
}