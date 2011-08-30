namespace UCTemplate.Web.Mvc.Infrastructure.Membership
{
    #region using

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Diagnostics;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.Linq;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.Security;

    using log4net;

    using Common;

    #endregion

    /// <summary>
    /// Active directory roles.
    /// </summary>
    public sealed class ActiveDirectoryRoleProvider : RoleProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActiveDirectoryRoleProvider));

        private const string AD_FILTER = "(&(objectCategory=group)(|(groupType=-2147483646)(groupType=-2147483644)(groupType=-2147483640)))";
        private const string AD_FIELD = "samAccountName";

        private string _activeDirectoryConnectionString;
        private string _domain;

        // Retrieve Group Mode
        // "Additive" indicates that only the groups specified in groupsToUse will be used
        // "Subtractive" indicates that all Active Directory groups will be used except those specified in groupsToIgnore
        // "Additive" is somewhat more secure, but requires more maintenance when groups change
        private bool _isAdditiveGroupMode;

        private readonly List<string> _groupsToUse;
        private readonly List<string> _groupsToIgnore;
        private readonly List<string> _usersToIgnore;

        #region ignore lists

        // IMPORTANT - DEFAULT LIST OF ACTIVE DIRECTORY USERS TO "IGNORE"
        //             DO NOT REMOVE ANY OF THESE UNLESS YOU FULLY UNDERSTAND THE SECURITY IMPLICATIONS
        //             VERYIFY THAT ALL CRITICAL USERS ARE IGNORED DURING TESTING
        private readonly string[] _defaultUsersToIgnore = new string[]
        {
            "Administrator", "TsInternetUser", "Guest", "krbtgt", "Replicate", "SERVICE", "SMSService"
        };

        // IMPORTANT - DEFAULT LIST OF ACTIVE DIRECTORY DOMAIN GROUPS TO "IGNORE"
        //             PREVENTS ENUMERATION OF CRITICAL DOMAIN GROUP MEMBERSHIP
        //             DO NOT REMOVE ANY OF THESE UNLESS YOU FULLY UNDERSTAND THE SECURITY IMPLICATIONS
        //             VERIFY THAT ALL CRITICAL GROUPS ARE IGNORED DURING TESTING BY CALLING GetAllRoles MANUALLY
        private readonly string[] _defaultGroupsToIgnore = new string[]
            {
                "Domain Guests", "Domain Computers", "Group Policy Creator Owners", "Guests", "Users",
                "Domain Users", "Pre-Windows 2000 Compatible Access", "Exchange Domain Servers", "Schema Admins",
                "Enterprise Admins", "Domain Admins", "Cert Publishers", "Backup Operators", "Account Operators",
                "Server Operators", "Print Operators", "Replicator", "Domain Controllers", "WINS Users",
                "DnsAdmins", "DnsUpdateProxy", "DHCP Users", "DHCP Administrators", "Exchange Services",
                "Exchange Enterprise Servers", "Remote Desktop Users", "Network Configuration Operators",
                "Incoming Forest Trust Builders", "Performance Monitor Users", "Performance Log Users",
                "Windows Authorization Access Group", "Terminal Server License Servers", "Distributed COM Users",
                "Administrators", "Everybody", "RAS and IAS Servers", "MTS Trusted Impersonators",
                "MTS Impersonators", "Everyone", "LOCAL", "Authenticated Users"
            };
        #endregion

        public ActiveDirectoryRoleProvider()
        {
            _groupsToUse = new List<string>();
            _groupsToIgnore = new List<string>();
            _usersToIgnore = new List<string>();
        }

        public override string ApplicationName { get; set; }

        public override void Initialize(string name, NameValueCollection config)
        {
            Check.IsNotNull(config, "config");

            if (string.IsNullOrEmpty(name))
                name = "ActiveDirectoryRoleProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Active Directory Role Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _domain = ReadConfig(config, "domain");
            _isAdditiveGroupMode = (ReadConfig(config, "groupMode") == "Additive");
            _activeDirectoryConnectionString = ReadConfig(config, "connectionString");

            DetermineApplicationName(config);
            PopulateLists(config);
        }

        private static string ReadConfig(NameValueCollection config, string key)
        {
            if (config.AllKeys.Any(k => k == key))
                return config[key];

            throw new ProviderException("Configuration value required for key: " + key);
        }

        private void DetermineApplicationName(NameValueCollection config)
        {
            // Retrieve Application Name
            ApplicationName = config["applicationName"];
            if (string.IsNullOrEmpty(ApplicationName))
            {
                try
                {
                    var app =
                        HostingEnvironment.ApplicationVirtualPath ??
                        Process.GetCurrentProcess().MainModule.ModuleName.Split('.').FirstOrDefault();

                    ApplicationName = !string.IsNullOrWhiteSpace(app) ? app : "/";
                }
                catch
                {
                    ApplicationName = "/";
                }
            }

            if (ApplicationName.Length > 256)
                throw new ProviderException("The application name is too long.");
        }

        private void PopulateLists(NameValueCollection config)
        {
            // If Additive group mode, populate GroupsToUse with specified AD groups
            if (_isAdditiveGroupMode && !string.IsNullOrEmpty(config["groupsToUse"]))
                _groupsToUse.AddRange(
                    config["groupsToUse"].Split(',').Select(group => group.Trim())
                );

            // Populate GroupsToIgnore List<string> with AD groups that should be ignored for roles purposes
            _groupsToIgnore.AddRange(
                _defaultGroupsToIgnore.Select(group => group.Trim())
            );

            _groupsToIgnore.AddRange(
                (config["groupsToIgnore"] ?? "").Split(',').Select(group => group.Trim())
            );

            // Populate UsersToIgnore ArrayList with AD users that should be ignored for roles purposes
            string usersToIgnore = config["usersToIgnore"] ?? "";
            _usersToIgnore.AddRange(
                _defaultUsersToIgnore
                    .Select(value => value.Trim())
                    .Union(
                        usersToIgnore
                            .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(value => value.Trim())
                    )
            );
        }

        private static void RecurseGroup(PrincipalContext context, string group, List<string> groups)
        {
            var principal = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, group);

            if (principal == null)
                return;

            var res =
                principal
                    .GetGroups()
                    .ToList()
                    .Select(grp => grp.Name)
                    .ToList();

            groups.AddRange(res.Except(groups));
            foreach (var item in res)
                RecurseGroup(context, item, groups);
        }

        /// <summary>
        /// Retrieve listing of all roles to which a specified user belongs.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>String array of roles</returns>
        public override string[] GetRolesForUser(string username)
        {
            var sessionKey = "groupsForUser:" + username;

            if (HttpContext.Current != null &&
                 HttpContext.Current.Session != null &&
                 HttpContext.Current.Session[sessionKey] != null
            )
                return ((List<string>)(HttpContext.Current.Session[sessionKey])).ToArray();

            using (var context = new PrincipalContext(ContextType.Domain, _domain))
            {
                try
                {
                    // add the users groups to the result
                    var groupList =
// ReSharper disable PossibleNullReferenceException
                        UserPrincipal
                            .FindByIdentity(context, IdentityType.SamAccountName, username)
// ReSharper restore PossibleNullReferenceException
                            .GetGroups()
                            .Select(group => group.Name)
                            .ToList();

                    // add each groups sub groups into the groupList
                    foreach (var group in new List<string>(groupList))
                        RecurseGroup(context, group, groupList);

                    groupList = groupList.Except(_groupsToIgnore).ToList();

                    if (_isAdditiveGroupMode)
                        groupList = groupList.Join(_groupsToUse, r => r, g => g, (r, g) => r).ToList();

                    if (HttpContext.Current != null && HttpContext.Current.Session != null)
                        HttpContext.Current.Session[sessionKey] = groupList;

                    return groupList.ToArray();
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to query Active Directory.", ex);
                    return new string[0];
                }
            }
        }

        /// <summary>
        /// Retrieve listing of all users in a specified role.
        /// </summary>
        /// <param name="rolename">String array of users</param>
        /// <returns></returns>
        public override string[] GetUsersInRole(string rolename)
        {
            if (!RoleExists(rolename))
                throw new ProviderException(string.Format("The role '{0}' was not found.", rolename));

            using (var context = new PrincipalContext(ContextType.Domain, _domain))
            {
                try
                {
                    var p = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, rolename);

                    return p == null 
                        ? new string[0] 
                        : (from user in p.GetMembers(true) where !_usersToIgnore.Contains(user.SamAccountName) select user.SamAccountName).ToArray();
                }
                catch (Exception ex)
                {
                    Log.Error("Unable to query Active Directory.", ex);
                    return new string[0];
                }
            }
        }

        /// <summary>
        /// Determine if a specified user is in a specified role.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="rolename"></param>
        /// <returns>Boolean indicating membership</returns>
        public override bool IsUserInRole(string username, string rolename)
        {
            return GetUsersInRole(rolename).Any(user => user == username);
        }

        /// <summary>
        /// Retrieve listing of all roles.
        /// </summary>
        /// <returns>String array of roles</returns>
        public override string[] GetAllRoles()
        {
            var roles = Search(_activeDirectoryConnectionString, AD_FILTER, AD_FIELD);

            return (
                from role in roles.Except(_groupsToIgnore)
                where !_isAdditiveGroupMode || _groupsToUse.Contains(role)
                select role
            ).ToArray();
        }

        /// <summary>
        /// Determine if given role exists
        /// </summary>
        /// <param name="rolename">Role to check</param>
        /// <returns>Boolean indicating existence of role</returns>
        public override bool RoleExists(string rolename)
        {
            return GetAllRoles().Any(role => role == rolename);
        }

        /// <summary>
        /// Return sorted list of usernames like usernameToMatch in rolename
        /// </summary>
        /// <param name="rolename">Role to check</param>
        /// <param name="usernameToMatch">Partial username to check</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            if (!RoleExists(rolename))
                throw new ProviderException(String.Format("The role '{0}' was not found.", rolename));

            return (
                from user in GetUsersInRole(rolename)
                where user.ToLower().Contains(usernameToMatch.ToLower())
                select user

            ).ToArray();
        }

        #region not supoorted

        /// <summary>
        /// AddUsersToRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException("Unable to add users to roles.  For security and management purposes, " + GetType().Name + " only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// CreateRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void CreateRole(string rolename)
        {
            throw new NotSupportedException("Unable to create new role.  For security and management purposes, " + GetType().Name + " only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// DeleteRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException("Unable to delete role.  For security and management purposes, " + GetType().Name + " only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// RemoveUsersFromRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException("Unable to remove users from roles.  For security and management purposes, " + GetType().Name + " supports read operations against Active Direcory.");
        }
        #endregion

        /// <summary>
        /// Performs an extremely constrained query against Active Directory.  Requests only a single value from
        /// AD based upon the filtering parameter to minimize performance hit from large queries.
        /// </summary>
        /// <param name="connectionString">Active Directory Connection String</param>
        /// <param name="filter">LDAP format search filter</param>
        /// <param name="field">AD field to return</param>
        /// <returns>String array containing values specified by 'field' parameter</returns>
        private static IEnumerable<string> Search(string connectionString, string filter, string field)
        {
            var searcher = new DirectorySearcher
            {
                SearchRoot = new DirectoryEntry(connectionString),
                Filter = filter,
                PageSize = 500
            };
            searcher.PropertiesToLoad.Clear();
            searcher.PropertiesToLoad.Add(field);

            try
            {
                using (var results = searcher.FindAll())
                {
                    var r = new List<string>();

                    foreach (var prop in from SearchResult searchResult in results select searchResult.Properties[field])
                    {
                        r.AddRange(from object t in prop select t.ToString());
                    }

                    return r.Count > 0 ? r.ToArray() : new string[0];
                }
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to query Active Directory.", ex);
            }
        }
    }
}