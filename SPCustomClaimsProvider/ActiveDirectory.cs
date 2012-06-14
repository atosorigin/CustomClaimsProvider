using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;

namespace SPCustomClaimsProvider
{

    public class ActiveDirectory
    {
        public List<string> FindUsers(string searchCriteria)
        {
            var entry = new DirectoryEntry();
            entry.Path = "GC://" + "ukstr-ad-640.peroot.com"; // This used to be the exchange user domain
            entry.AuthenticationType = AuthenticationTypes.Secure;

            var searcher = new DirectorySearcher(entry) { SearchRoot = entry };
            List<string> returnSearch = new List<string>();
            searchCriteria = "*" + searchCriteria + "*";


            searcher.Filter = "(&(objectClass=User)(|(givenName=" + searchCriteria + ")(sn=" + searchCriteria + ")))";
            var results = searcher.FindAll();
            returnSearch.AddRange(from SearchResult result in results select result.GetDirectoryEntry().Name);

            return returnSearch;
        }



    }
}
