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
        private DirectorySearcher[] _directorySearchers;
        private string[] _domains = {"mctest.local"};

        public ActiveDirectory()
        {
            _directorySearchers = GetDirectorySearchers();
        }
        private DirectorySearcher[] GetDirectorySearchers()
        {
            var searchers = new List<DirectorySearcher>();

            foreach (var userDomain in _domains)
            {
                var entry = new DirectoryEntry();
                entry.Path = "LDAP://" + GetDomainDnName(userDomain); // This used to be the exchange user domain
                entry.AuthenticationType = AuthenticationTypes.Secure;

                var searcher = new DirectorySearcher(entry) { SearchRoot = entry };

                searchers.Add(searcher);
            }

            return searchers.ToArray();
        }

        private string GetDomainDnName(string userDomain)
        {
            var context = new DirectoryContext(DirectoryContextType.Domain, userDomain);
            var domain = Domain.GetDomain(context);
            var directoryEntry = domain.GetDirectoryEntry();
            return directoryEntry.Properties["DistinguishedName"].Value.ToString();
        }

        public List<string> FindUsers(string searchCriteria)
        {
            List<string> returnSearch = new List<string>();
            searchCriteria = "*" + searchCriteria + "*";
            foreach (var searcher in _directorySearchers)
            {
                searcher.Filter = "(&(objectClass=user)(|(givenName=" + searchCriteria + ")(surname=" + searchCriteria + "))";
                var results = searcher.FindAll();
                returnSearch.AddRange(from SearchResult result in results select result.GetDirectoryEntry().Name);
            }
            return returnSearch;
        }



    }
}
