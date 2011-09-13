namespace UCTemplate.Web.Mvc.Infrastructure.Identity
{
    #region using

    using System;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.DirectoryServices.Protocols;
    using System.Linq;
    using System.Web.Security;

    using log4net;

    using Common;

    #endregion
    
    public class ActiveDirectoryMembershipProvider : MembershipProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActiveDirectoryRoleProvider));

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

        /// <summary>
        /// Verifies that the specified user name and password exist in Active Directory.
        /// </summary>
        /// <param name="username">The name of the user to validate. </param>
        /// <param name="password">The password for the specified user. </param>
        /// <returns>true if the specified username and password are valid; otherwise, false.</returns>
        /// <exception cref="ProviderException">No domain controllers are available to handle the request.</exception>
        public override bool ValidateUser(string username, string password)
        {
            var principalContext = ActiveDirectory.Connections.FirstOrDefault(); // grab a dc connection

            if (principalContext == null)
            {
                Log.Error("No logon servers are currently available.");
                throw new ProviderException("No logon servers are currently available.");
            }

            try
            {
                return principalContext.ValidateCredentials(username, password);
            }
            catch (LdapException ex)
            {
                Log.Warn(string.Format("Problem w/{0}, disposing connection '{0}'", principalContext.ConnectedServer), ex);
                ActiveDirectory.DisposeConnection(principalContext);
                return ValidateUser(username, password);  // retry
            }
            catch (Exception ex)
            {
                Log.Error("Unable to validate credentials", ex);
                throw;
            }
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }
    }
}