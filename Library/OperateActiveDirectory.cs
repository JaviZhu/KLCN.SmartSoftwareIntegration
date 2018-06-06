
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;

namespace Library
{
    public class OperateActiveDirectory
    {
        public OperateActiveDirectory()
        {
            GetConnection();
        }
        private static ILdapConnection _conn;
        static ILdapConnection GetConnection()
        {
            LdapConnection ldapConn = _conn as LdapConnection;

            if (ldapConn == null)
            {
                // Creating an LdapConnection instance 
                ldapConn = new LdapConnection() { SecureSocketLayer = true };

                //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                //ldapConn.Connect("CNKLSHSV2022.corp.fcs.int", 3269);
                ldapConn.Connect("KLCN-SH-SR2002.corp.fcs.int", 3269);

                //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
                ldapConn.Bind(@"corpfcsint\ycnspadmin", "fcs@sp937");
            }

            return ldapConn;
        }
        HashSet<string> SearchForGroup(string groupName)
        {
            var ldapConn = GetConnection();
            var groups = new HashSet<string>();

            var searchBase = string.Empty;
            var filter = $"(&(objectClass=group)(cn={groupName}))";
            var search = ldapConn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);
            while (search.HasMore())
            {
                var nextEntry = search.Next();
                groups.Add(nextEntry.DN);
                var childGroups = GetChildren(string.Empty, nextEntry.DN);
                foreach (var child in childGroups)
                {
                    groups.Add(child);
                }
            }

            return groups;
        }

        static HashSet<string> GetChildren(string searchBase, string groupDn, string objectClass = "group")
        {
            var ldapConn = GetConnection();
            var listNames = new HashSet<string>();

            var filter = $"(&(objectClass={objectClass})(memberOf={groupDn}))";
            var search = ldapConn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);

            while (search.HasMore())
            {
                var nextEntry = search.Next();
                listNames.Add(nextEntry.DN);
                var children = GetChildren(string.Empty, nextEntry.DN);
                foreach (var child in children)
                {
                    listNames.Add(child);
                }
            }

            return listNames;
        }

        public static IDictionary<string,string> SearchForUser(string commonName)
        {
            //commonName = "cnjazhu";
            var ldapConn = GetConnection();
            var users = new Dictionary<string, string>();

            var searchBase = string.Empty;
            string filter = "(&(&(objectCategory=person)(objectClass=user))(cn=" + commonName + "))";
            var search = ldapConn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);

            while (search.HasMore())
            {
                var nextEntry = search.Next();
                nextEntry.getAttributeSet();
                users.Add("DN", nextEntry.DN);
                users.Add("Mail", nextEntry.getAttribute("mail").StringValue);
                users.Add("DisplayName", nextEntry.getAttribute("displayname").StringValue);
                users.Add("ThumbnailPhoto", nextEntry.getAttribute("thumbnailPhoto").StringValue);
                users.Add("Title", nextEntry.getAttribute("title").StringValue);
                users.Add("Mobile", nextEntry.getAttribute("mobile").StringValue);
                users.Add("IpPhone", nextEntry.getAttribute("ipphone").StringValue);
                users.Add("TelephoneNumber", nextEntry.getAttribute("telephonenumber").StringValue);
                users.Add("Department", nextEntry.getAttribute("department").StringValue);
                users.Add("WwwHomepage", nextEntry.getAttribute("wwwhomepage").StringValue);
            }
            //while (search.HasMore())
            //{
            //    var nextEntry = search.Next();
            //    nextEntry.getAttributeSet();
            //    users.DN=nextEntry.DN;
            //    users.Mail=nextEntry.getAttribute("mail").StringValue;
            //    users.DisplayName= nextEntry.getAttribute("displayname").StringValue;
            //    users.ThumbnailPhoto= nextEntry.getAttribute("thumbnailPhoto").StringValue;
            //    users.Title=nextEntry.getAttribute("title").StringValue;
            //    users.Mobile=nextEntry.getAttribute("mobile").StringValue;
            //    users.IpPhone=nextEntry.getAttribute("ipphone").StringValue;
            //    users.TelephoneNumber=nextEntry.getAttribute("telephonenumber").StringValue;
            //    users.Department=nextEntry.getAttribute("department").StringValue;
            //    users.WwwHomepage=nextEntry.getAttribute("wwwhomepage").StringValue;
            //}
            return users;
        }
    }
}
