namespace UCTemplate.Web.Mvc.Infrastructure.Identity
{
    #region using

    using System;
    using System.Collections.Specialized;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Web.Security;

    using log4net;

    using Common;

    #endregion

    public sealed class ActiveDirectoryRoleProvider : RoleProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActiveDirectoryRoleProvider));

        /// <summary>
        /// List of users to ignore for security reeasons.
        /// </summary>
        private readonly string[] _ignoreUsers = new[]
                                                     {
                                                         "Administrator", "TsInternetUser", "Guest", "krbtgt",
                                                         "Replicate", "SERVICE", "SMSService"
                                                     };

        /// <summary>
        /// List of groups to ignore for security reeasons.
        /// </summary>
        private readonly string[] _ignoreGroups = new[]
                                                      {
                                                          "Domain Guests", "Domain Computers",
                                                          "Group Policy Creator Owners", "Guests", "Users",
                                                          "Domain Users", "Pre-Windows 2000 Compatible Access",
                                                          "Exchange Domain Servers", "Schema Admins",
                                                          "Enterprise Admins", "Domain Admins", "Cert Publishers",
                                                          "Backup Operators", "Account Operators",
                                                          "Server Operators", "Print Operators", "Replicator",
                                                          "Domain Controllers", "WINS Users",
                                                          "DnsAdmins", "DnsUpdateProxy", "DHCP Users",
                                                          "DHCP Administrators", "Exchange Services",
                                                          "Exchange Enterprise Servers", "Remote Desktop Users",
                                                          "Network Configuration Operators",
                                                          "Incoming Forest Trust Builders", "Performance Monitor Users",
                                                          "Performance Log Users",
                                                          "Windows Authorization Access Group",
                                                          "Terminal Server License Servers", "Distributed COM Users",
                                                          "Administrators", "Everybody", "RAS and IAS Servers",
                                                          "MTS Trusted Impersonators",
                                                          "MTS Impersonators", "Everyone", "LOCAL",
                                                          "Authenticated Users"
                                                      };

        public override void Initialize(string name, NameValueCollection config)
        {
            Check.IsNotNull(config, "config");

            if (string.IsNullOrEmpty(name))
                name = "ActiveDirectoryMembershipProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Active Directory Membership Provider");
            }

            base.Initialize(name, config);
        }


        public override bool IsUserInRole(string username, string roleName)
        {
            Check.IsNotNullOrEmpty(username, "username");
            Check.IsNotNullOrEmpty(roleName, "roleName");

            return !_ignoreUsers.Contains(username) && GetUsersInRole(roleName).Any(x => x == username);
        }

        /// <summary>
        /// Returns a list of roles for the specified user.
        /// </summary>
        /// <param name="username">User's samAccountName.</param>
        /// <returns>A list of roles for the specified user.</returns>
        public override string[] GetRolesForUser(string username)
        {
            Check.IsNotNullOrEmpty(username, "username");

            if(_ignoreUsers.Contains(username))
                return new string[0];

            var principalContext = ActiveDirectory.Connections.FirstOrDefault();

            if (principalContext == null)
            {
                Log.Error("No logon servers are currently available.");
                return new string[0];
            }

            try
            {
                var user = UserPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, username);

                if (user != null)
                {

                    var roles = user.GetGroups().Select(x => (GroupPrincipal) x).ToList();

                    return
                        roles.Union(
                                roles.SelectMany(g => g.GetMembers(true).Where(x => x is GroupPrincipal))
                            )
                            .Select(x => x.SamAccountName)
                            .ToArray();
                }
            }
            catch(Exception ex)
            {
                Log.Error(string.Format("Cannot retrieve roles for user, '{0}'", username), ex);
            }
            
            return new string[0];
        }


        public override bool RoleExists(string roleName)
        {
            Check.IsNotNullOrEmpty(roleName, "roleName");

            return GetAllRoles().Any(x => x == roleName);
        }

        

        public override string[] GetUsersInRole(string roleName)
        {
            Check.IsNotNullOrEmpty(roleName, "roleName");

            var principalContext = ActiveDirectory.Connections.FirstOrDefault();

            if (principalContext == null)
            {
                Log.Error("No logon servers are currently available.");
                return new string[0];
            }

            try
            {
                var role = GroupPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, roleName);

                if(role != null)
                {
                    return role.GetMembers(true)
                        .Where(x => !_ignoreUsers.Contains(x.SamAccountName))
                        .Select(x => x.SamAccountName)
                        .ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Cannot retrieve users in role, '{0}'", roleName), ex);
            }

            return new string[0];
        }

        public override string[] GetAllRoles()
        {
            var principalContext = ActiveDirectory.Connections.FirstOrDefault();

            if (principalContext == null)
            {
                Log.Error("No logon servers are currently available.");
                return new string[0];
            }

            try
            {
                return new PrincipalSearcher(new GroupPrincipal(principalContext))
                    .FindAll()
                    .Where(x => !_ignoreGroups.Contains(x.SamAccountName))
                    .Select(x => x.SamAccountName)
                    .ToArray();
            }
            catch (Exception ex)
            {
                Log.Error("Unable to retrieve all roles.", ex);
            }

            return new string[0];
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The username fragment to search for.</param>
        /// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            Check.IsNotNullOrEmpty(roleName, "roleName");
            Check.IsNotNullOrEmpty(usernameToMatch, "usernameToMatch");

            if (_ignoreGroups.Contains(roleName))
                return new string[0];

            var principalContext = ActiveDirectory.Connections.FirstOrDefault();

            if (principalContext == null)
            {
                Log.Error("No logon servers are currently available.");
                return new string[0];
            }

            try
            {
                var role = GroupPrincipal.FindByIdentity(principalContext, IdentityType.SamAccountName, roleName);

                if(role != null)
                {
                    return role.GetMembers(true)
                        .Where(x => x is UserPrincipal)
                        .Select(x => x.SamAccountName)
                        .Where(x => x.ContainsInsensitive(usernameToMatch) && !_ignoreUsers.Contains(usernameToMatch))
                        .ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Cannot match users in role, '{0}'", roleName), ex);
            }

            return new string[0];
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
    }
}