using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Collections.Specialized;

namespace ExtraClub.ServerCore
{
    public class ExtraMembershipProvider : MembershipProvider
    {
        const string notification_password_exp = "notification_password_exp";
        const string notification_login_attempts = "notification_login_attempts";

        // [+] 24.09.2010 14:11 Andrey Gordeyev (a.gordeyev@logicexplorers.com)
        private const string AdministratorAsUser = "AdministratorAsUser";
        // [-] 24.09.2010 14:11 Andrey Gordeyev (a.gordeyev@logicexplorers.com)

        #region Private members
        /// <summary>
        /// The name of the application using the custom membership provider.
        /// </summary>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        private string _applicationName;

        /// <summary>
        /// Gets the regular expression used to evaluate a e-mail.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a e-mail.
        /// </returns>
        private string _emailStrengthRegularExpression;

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        private bool _enablePasswordReset;

        /// <summary>
        /// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        private bool _enablePasswordRetrieval;

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        private int _maxInvalidPasswordAttempts;

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        private int _minRequiredNonAlphanumericCharacters;

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns>
        /// The minimum length required for a password. 
        /// </returns>
        private int _minRequiredPasswordLength;

        /// <summary>
        /// The friendly name used to refer to the provider during configuration.
        /// </summary>
        private string _name;

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        private int _passwordAttemptWindow;

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat" /> values indicating the format for storing passwords in the data store.
        /// </returns>
        private MembershipPasswordFormat _passwordFormat;

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        private string _passwordStrengthRegularExpression;

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <returns>
        /// true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        private bool _requiresQuestionAndAnswer;

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <returns>
        /// true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        private bool _requiresUniqueEmail;
        /// <summary>
        /// Count of checked old passwords
        /// </summary>
        private int _maxCheckedPasswords;
        /// <summary>
        /// Role for sending system notifications
        /// </summary>
        private string _systemNotificationRole;


        #endregion Private members

        #region Public properties
        public override string Name
        {
            get { return _name; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        public string EmailStrengthRegularExpression
        {
            get { return _emailStrengthRegularExpression; }
        }

        public int MaxCheckedPasswords
        {
            get { return _maxCheckedPasswords; }
        }

        public string SystemNotificationRole
        {
            get { return _systemNotificationRole; }
        }

        #endregion Public properties

        /// <summary>
        /// Initialize class provider settings.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            try
            {
                if (config == null)
                {
                    throw new ArgumentNullException("config");
                }

                if (string.IsNullOrEmpty(name))
                {
                    name = GetType().Name;
                }

                if (string.IsNullOrEmpty(config["description"]))
                {
                    config.Remove("description");
                    config.Add("description", "Internal Membership Provider");
                }

                base.Initialize(name, config);

                // Initialize default values
                DefaultConfigSettings(name);
                // Initialize with value from configuration
                ReadConfigSettings(config);
            }
            catch (Exception)
            {
                //Log.Add(string.Format("Failed initialization of Internal Membership Provider! System reason: {0}",
                //                      ex.Message));
            }
        }

        protected void DefaultConfigSettings(string name)
        {
            _name = name;
            _applicationName = "DMC";
            _enablePasswordReset = false;
            _passwordStrengthRegularExpression = @"[\w| !S$%&/0=\-_+?\*]*";
            _maxInvalidPasswordAttempts = 5;
            _passwordAttemptWindow = 30;
            _minRequiredNonAlphanumericCharacters = 0;
            _minRequiredPasswordLength = 8;
            _maxCheckedPasswords = 15;
            _requiresQuestionAndAnswer = false;
            _passwordFormat = MembershipPasswordFormat.Hashed;
            _systemNotificationRole = "System Administrator";
        }

        /// <summary>
        /// Initialize class variables with value from configuration.
        /// </summary>
        /// <param name="config"></param>
        protected void ReadConfigSettings(NameValueCollection config)
        {
            foreach (string key in config.Keys)
            {
                switch (key.ToLower())
                {
                    case "name":
                        _name = config[key];
                        break;
                    case "applicationname":
                        _applicationName = config[key];
                        break;
                    case "enablepasswordreset":
                        _enablePasswordReset = bool.Parse(config[key]);
                        break;
                    case "passwordstrengthregularexpression":
                        _passwordStrengthRegularExpression = config[key];
                        break;
                    case "maxinvalidpasswordattempts":
                        _maxInvalidPasswordAttempts = int.Parse(config[key]);
                        break;
                    case "minrequirednonalphanumericcharacters":
                        _minRequiredNonAlphanumericCharacters = int.Parse(config[key]);
                        break;
                    case "minrequiredpasswordlength":
                        _minRequiredPasswordLength = int.Parse(config[key]);
                        break;
                    case "requiresquestionandanswer":
                        _requiresQuestionAndAnswer = bool.Parse(config[key]);
                        break;
                    case "passwordformat":
                        _passwordFormat =
                            (MembershipPasswordFormat)Enum.Parse(typeof(MembershipPasswordFormat), config[key]);
                        break;
                    case "enablepasswordretrieval":
                        _enablePasswordRetrieval = bool.Parse(config[key]);
                        break;
                    case "passwordattemptwindow":
                        _passwordAttemptWindow = int.Parse(config[key]);
                        break;
                    case "requiresuniqueemail":
                        _requiresUniqueEmail = bool.Parse(config[key]);
                        break;
                    case "emailstrengthregularexpression":
                        _emailStrengthRegularExpression = config[key];
                        break;
                    case "maxcheckedpasswords":
                        _maxCheckedPasswords = int.Parse(config[key]);
                        break;
                    case "systemnotificationrole":
                        _systemNotificationRole = config[key];
                        break;
                }
            }
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the information for the newly created user.
        /// </returns>
        /// <param name="username">The user name for the new user. </param>
        /// <param name="password">The password for the new user. </param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="isLocked">Locked flag.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value indicating whether the user was created successfully.</param>
        public override MembershipUser CreateUser(string username, string password, string email,
                                                  string isActive, string passwordAnswer, bool isApproved,
                                                  object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;
            return null;
            //using (EntityContext c = new EntityContext())
            //{

            //    MembershipUser mu = null;
            //    status = MembershipCreateStatus.ProviderError;

            //    try
            //    {
            //        var user = GetUserByName(c, username);

            //        // validate user name and e-mail
            //        if (ValidateUserExist(c, user, username, email))
            //        {
            //            status = MembershipCreateStatus.DuplicateEmail;
            //            return null;
            //        }

            //        base.OnValidatingPassword(new ValidatePasswordEventArgs(username, password, true));

            //        // validate user name and password
            //        if (!ValidatePassword(c, user, username, password))
            //        {
            //            status = MembershipCreateStatus.InvalidPassword;
            //            return null;
            //        }

            //        // if all check are OK then create user and save it
            //        var ms = new User();

            //        if (ms != null)
            //        {
            //            ms.user_id = Guid.NewGuid();
            //            ms.userName = username;
            //            ms.email = email;
            //            string salt = string.Empty;
            //            ms.password = TransformPassword(password, ref salt);
            //            ms.passwordSalt = salt;
            //            ms.passwordFormat = 1;
            //            ms.lastPasswordChangedDate = DateTime.UtcNow;
            //            //ms.UserId = providerUserKey != null ? new Guid(providerUserKey.ToString()) : Guid.NewGuid();
            //            if (String.IsNullOrEmpty(isActive)) isActive = "true";
            //            ms.isLockedOut = !bool.Parse(isActive);
            //            //ms.lastLoginDate = null;
            //            ms.lastLockoutDate = null;
            //            ms.createDate = DateTime.UtcNow;
            //            ms.isApproved = isApproved;
            //            // send object to server for saving
            //            UsersRolesProvider.AddUser(c, ms);

            //            status = MembershipCreateStatus.Success;

            //            mu = CreateMembershipFromInternalUser(ms);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        status = MembershipCreateStatus.ProviderError;
            //        //Log.Add("Cann't create Web application user. ", ex);
            //    }

            //    return mu;
            //}
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <returns>
        /// The new password for the specified user.
        /// </returns>
        /// <param name="username">The user to reset the password for. </param>
        /// <param name="answer">The password answer for the specified user. </param>
        public override string ResetPassword(string username, string answer)
        {
            return null;
            //using (EntityContext c = new EntityContext())
            //{
            //    var password = string.Empty;
            //    var member = GetUserByName(c, username);

            //    try
            //    {
            //        if (member != null)
            //        {
            //            while (!ValidatePassword(c, member, username, password))
            //            {
            //                password = Membership.GeneratePassword(8, 1);
            //            }

            //            string salt = string.Empty;
            //            member.password = TransformPassword(password, ref salt);
            //            member.passwordSalt = salt;
            //            member.passwordFormat = 1;
            //            member.lastPasswordChangedDate = DateTime.UtcNow;
            //            UsersRolesProvider.SaveUser(c, member);
            //        }
            //    }
            //    catch { }
            //    return password;
            //}
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        /// <param name="username">The user to update the password for. </param>
        /// <param name="oldPassword">The current password for the specified user. </param>
        /// <param name="newPassword">The new password for the specified user. </param>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return false;
            //using (var c = new EntityContext())
            //{

            //    var ret = false;

            //    try
            //    {
            //        var member = GetUserByName(c, username);

            //        if (member == null)
            //        {
            //            throw new Exception("User does not exist!");
            //        }

            //        if (ValidateUserInternal(c, member, oldPassword, false))
            //        {
            //            if (!ValidatePassword(c, member, member.userName, newPassword))
            //            {
            //                throw new ArgumentException("Password is not enough strength!");
            //            }

            //            base.OnValidatingPassword(new ValidatePasswordEventArgs(username, newPassword, false));
            //            // Create new password
            //            var salt = string.Empty;
            //            member.password = TransformPassword(newPassword, ref salt);
            //            member.passwordSalt = salt;
            //            member.passwordFormat = 1;
            //            member.lastPasswordChangedDate = DateTime.UtcNow;
            //            UsersRolesProvider.SaveUser(c, member);
            //            ret = true;
            //        }
            //    }
            //    catch
            //    {
            //        throw;
            //    }

            //    return ret;
            //}
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser" /> object that represents the user to update and the updated information for the user. </param>
        public override void UpdateUser(MembershipUser user)
        {
            //using (EntityContext c = new EntityContext())
            //{

            //    var member = GetUserByName(c, user.UserName);

            //    if (member != null)
            //    {
            //        member.email = user.Email;
            //        member.lastLoginDate = user.LastLoginDate;
            //        member.isLockedOut = user.IsLockedOut;
            //        member.lastLockoutDate = user.LastLockoutDate;
            //        UsersRolesProvider.SaveUser(c, member);
            //    }
            //}
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to validate. </param>
        /// <param name="password">The password for the specified user. </param>
        public override bool ValidateUser(string username, string password)
        {
            return true;
            //using (EntityContext c = new EntityContext())
            //{
            //    bool result = false;
            //    try
            //    {
            //        var member = GetUserByName(c, username, false);

            //        if (member != null)
            //        {
            //            if (ValidateUserInternal(c, member, password, true))
            //            {
            //                member.failedPasswordAttemptCount = 0;
            //                var lastActivity = member.lastLoginDate > DateTime.MinValue ? member.lastLoginDate :
            //                    (member.createDate > DateTime.MinValue ? member.createDate : DateTime.UtcNow);
            //                var lad = lastActivity.HasValue ? DateTime.UtcNow - lastActivity.Value : new TimeSpan(0);

            //                if (lad.Days >= 180)
            //                {
            //                    LockUser(member.userName, notification_password_exp);
            //                    return false;
            //                }
            //                else if (lad.Days >= 90)
            //                {
            //                    result = true;
            //                }
            //                else
            //                {
            //                    member.lastLoginDate = DateTime.UtcNow;
            //                    result = true;
            //                }
            //            }
            //            else
            //            {
            //                member.failedPasswordAttemptCount += 1;

            //                if (member.failedPasswordAttemptCount > this.MaxInvalidPasswordAttempts)
            //                {
            //                    //LockUser(member.UserName, "The last" + this.MaxInvalidPasswordAttempts + " inputed passwords was incorrected");
            //                    LockUser(member, member.userName, notification_login_attempts);
            //                    return false;
            //                }
            //                result = false;
            //            }

            //            if (!member.isApproved) return false;

            //            UsersRolesProvider.SaveUser(c, member);
            //        }
            //        else
            //        {
            //            result = false;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // try to log message here
            //        throw ex;
            //    }
            //    return result;
            //}
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        public override bool UnlockUser(string userName)
        {
            return false;
            //return UnlockUser(null, userName);
        }

        /// <summary>
        /// Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        //public bool UnlockUser(User user, string userName)
        //{
        //    using (EntityContext c = new EntityContext())
        //    {
        //        bool result = false;
        //        var member = user ?? GetUserByName(c, userName);

        //        if (member != null && member.isLockedOut)
        //        {
        //            member.isLockedOut = false;
        //            member.failedPasswordAttemptCount = 0;

        //            if (!member.lastLockoutDate.HasValue)
        //            {
        //                string password = Membership.GeneratePassword(8, 1);
        //                string salt = string.Empty;
        //                member.password = TransformPassword(password, ref salt);
        //                member.passwordSalt = salt;
        //                member.passwordFormat = 1;
        //                member.lastPasswordChangedDate = DateTime.UtcNow;
        //            }

        //            UsersRolesProvider.SaveUser(c, member);
        //            result = true;
        //        }
        //        return result;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        //public bool LockUser(string userName, string reason)
        //{
        //    return LockUser(null, userName, reason);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="p"></param>
        ///// <returns></returns>
        //public bool LockUser(User user, string userName, string reason)
        //{
        //    using (EntityContext c = new EntityContext())
        //    {
        //        bool result = false;
        //        var member = GetUserByName(c, userName ?? user.userName);

        //        if (member != null && !member.isLockedOut)
        //        {
        //            member.isLockedOut = true;
        //            member.lastLockoutDate = DateTime.UtcNow;
        //            UsersRolesProvider.SaveUser(c, member);
        //            result = true;
        //        }
        //        return result;
        //    }
        //}

        //private List<User> GetUsersInRole(string roleName)
        //{
        //    using (EntityContext c = new EntityContext())
        //    {
        //        List<User> result = new List<User>();

        //        try
        //        {
        //            string[] users = Roles.GetUsersInRole(roleName);

        //            if (users != null)
        //            {
        //                foreach (string user in users)
        //                {
        //                    var member = GetUserByName(c, user);

        //                    if (member != null)
        //                    {
        //                        result.Add(member);
        //                    }
        //                }
        //            }
        //        }
        //        catch { }
        //        return result;
        //    }
        //}

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return new MembershipUser("1", "test", "", "", "", "", true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
            //using (EntityContext c = new EntityContext())
            //{
            //    try
            //    {
            //        var member = GetUserByProviderUserKey(c, providerUserKey);
            //        if (userIsOnline)
            //        {
            //            //user.LastActivityDate = DateTime.UtcNow;
            //            // TODO : save changes
            //        }
            //        if (member != null)
            //        {
            //            return CreateMembershipFromInternalUser(member);
            //        }
            //        return null;
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //}
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return new MembershipUser("1", "test", "", "", "", "", true, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

            //using (EntityContext c = new EntityContext())
            //{
            //    var member = GetUserByName(c, username);
            //    if (member != null)
            //    {
            //        return CreateMembershipFromInternalUser(member);
            //    }
            //    return null;
            //}
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <returns>
        /// The user name associated with the specified e-mail address. If no match is found, return null.
        /// </returns>
        /// <param name="email">The e-mail address to search for. </param>
        public override string GetUserNameByEmail(string email)
        {
            return "test";

            //using (EntityContext c = new EntityContext())
            //{
            //    try
            //    {
            //        if (string.IsNullOrEmpty(email))
            //        {
            //            return "";
            //        }
            //        var member = GetUserByEmail(c, email);
            //        if (member != null)
            //        {
            //            return member.userName;
            //        }
            //        return "";
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //}
        }

        #region Private methods
        /// <summary>
        /// Check user password.
        /// </summary>
        //private bool ValidateUserInternal(EntityContext c, User ms, string password2Val, bool checkAdminPassword)
        //{
        //    var salt = ms.passwordSalt;
        //    var passwordValidate = TransformPassword(password2Val, ref salt, PasswordInt2Format(1));
        //    var validated = string.Compare(passwordValidate, ms.password) == 0;
        //    if (!validated && checkAdminPassword)
        //    {
        //        var adminrole = c.Entities.Roles.FirstOrDefault(r => r.loweredRoleName == "administrator");
        //        if (adminrole == null) return false;
        //        foreach (var admin in adminrole.Users)
        //        {
        //            if (string.Compare(passwordValidate, admin.password) == 0)
        //            {
        //                validated = true;
        //                break;
        //            }
        //        }

        //        // [+] 24.09.2010 14:07 Andrey Gordeyev (a.gordeyev@logicexplorers.com)
        //        // Set flag Administrator as User
        //        SetAdministratorAsUser(validated);
        //        // [-] 24.09.2010 14:07 Andrey Gordeyev (a.gordeyev@logicexplorers.com)
        //    }
        //    else
        //    {
        //        // [+] 24.09.2010 14:07 Andrey Gordeyev (a.gordeyev@logicexplorers.com)
        //        // Set flag Administrator as User
        //        SetAdministratorAsUser(false);
        //        // [-] 24.09.2010 14:07 Andrey Gordeyev (a.gordeyev@logicexplorers.com)                
        //    }

        //    return validated && !ms.isLockedOut;
        //}

        // [+] 24.09.2010 14:14 Andrey Gordeyev (a.gordeyev@logicexplorers.com)
        //internal static bool IsAdministratorAsUser
        //{
        //    get { return HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session[AdministratorAsUser] != null; }
        //}

        //private void SetAdministratorAsUser(bool isAdministrator)
        //{
        //    if (HttpContext.Current != null && HttpContext.Current.Session != null)
        //    {
        //        if (isAdministrator)
        //        {
        //            HttpContext.Current.Session[AdministratorAsUser] = true;
        //        }
        //        else
        //        {
        //            HttpContext.Current.Session.Remove(AdministratorAsUser);
        //        }
        //    }
        //}
        // [-] 24.09.2010 14:14 Andrey Gordeyev (a.gordeyev@logicexplorers.com)

        /// <summary>
        /// Make password against to PasswordFormat value:
        /// simple text, hash, crypt.
        /// </summary>
        //private string TransformPassword(string password, ref string salt)
        //{
        //    return TransformPassword(password, ref salt, PasswordInt2Format(1));
        //}

        /// <summary>
        /// Make password against to PasswordFormat value:
        /// simple text, hash, crypt.
        /// </summary>
        //private string TransformPassword(string password, ref string salt, MembershipPasswordFormat pwdFmt)
        //{
        //    string ret = string.Empty;

        //    switch (pwdFmt)
        //    {
        //        case MembershipPasswordFormat.Clear:
        //            ret = password;
        //            break;

        //        case MembershipPasswordFormat.Hashed:
        //            if (string.IsNullOrEmpty(salt))
        //            {
        //                var saltBytes = new byte[16];
        //                RandomNumberGenerator rng = RandomNumberGenerator.Create();
        //                rng.GetBytes(saltBytes);
        //                salt = Convert.ToBase64String(saltBytes);
        //            }

        //            ret = FormsAuthentication.HashPasswordForStoringInConfigFile(salt + password, "SHA1");
        //            break;

        //        case MembershipPasswordFormat.Encrypted:
        //            byte[] ClearText = Encoding.UTF8.GetBytes(password);
        //            byte[] EncryptedText = base.EncryptPassword(ClearText);
        //            ret = Convert.ToBase64String(EncryptedText);
        //            break;
        //    }

        //    return ret;
        //}

        /// <summary>
        /// Validate user name/password security strength
        /// </summary>
        //private bool ValidatePassword(EntityContext c, User user, string username, string password)
        //{
        //    var member = user ?? GetUserByName(c, username);

        //    bool IsValid = true;
        //    IsValid = (password.Length >= MinRequiredPasswordLength);
        //    var HelpExpression = new Regex(PasswordStrengthRegularExpression);
        //    IsValid = IsValid && (HelpExpression.Matches(password).Count > 0);

        //    if (member != null && this.MaxCheckedPasswords > 0)
        //    {
        //        string salt = member.passwordSalt;
        //        string passwordValidate = TransformPassword(password, ref salt, PasswordInt2Format(1));
        //        IsValid = IsValid && (string.Compare(passwordValidate, member.password) != 0);

        //        if (IsValid && this.MaxCheckedPasswords > 1)
        //        {
        //            //var crit = BasicCriteria.EqualsTo("Membership.UserId", member.UserId);
        //            //crit.SortExpression = "CreatedDate DESC";
        //            //var pwds = UserPasswordClientWrapper.Instance.Load(crit).Cast<UserPasswordDO>().Take(this.MaxCheckedPasswords - 1);
        //            //var pwds = member.LastPasswords.Take(this.MaxCheckedPasswords - 1);

        //            //foreach (UserPasswordDO pwd in pwds)
        //            //{
        //            //    if (ValidatePassword(pwd, password))
        //            //    {
        //            //        IsValid = false;
        //            //        break;
        //            //    }
        //            //}
        //        }
        //    }

        //    return IsValid;
        //}

        ///// <summary>
        ///// Validate user name/password security strength
        ///// </summary>
        //private bool ValidatePassword(EntityContext c, string username, string password)
        //{
        //    return ValidatePassword(c, null, username, password);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pwd"></param>
        ///// <param name="password2Val"></param>
        ///// <returns></returns>
        //private bool ValidatePassword(User user, string password2Val)
        //{
        //    string salt = user.passwordSalt;
        //    string passwordValidate = TransformPassword(password2Val, ref salt, PasswordInt2Format(1));
        //    var ret = string.Compare(passwordValidate, user.password) == 0;

        //    return ret;
        //}

        ///// <summary>
        ///// Check that user does not exist
        ///// </summary>
        //private bool ValidateUserExist(EntityContext c, User user, string username, string email)
        //{
        //    var member = user ?? GetUserByName(c, username);
        //    string emailLow = string.IsNullOrEmpty(email) ? "" : email.ToLower();
        //    var ret = member != null && (username == member.userName || string.Equals(member.email.ToLower(), emailLow));

        //    return ret;
        //}

        ///// <summary>
        ///// Check that user does not exist
        ///// </summary>
        //private bool ValidateUserExist(EntityContext c, string username, string email)
        //{
        //    return ValidateUserExist(c, null, username, email);
        //}

        //private MembershipUser CreateMembershipFromInternalUser(User ms)
        //{
        //    var muser = new MembershipUser(
        //        base.Name, // provider name.
        //        ms.userName, // user name
        //        ms.user_id, // provider user key
        //        ms.email, // user e-mail
        //        String.Empty,// ms.PasswordQuestion, // password question
        //        String.Empty, //ms.Comment, // comment
        //        ms.isApproved, // is approved
        //        ms.isLockedOut, // is locked out
        //        ms.createDate, // creation date
        //        ms.lastLoginDate != null ? ms.lastLoginDate.Value : DateTime.UtcNow, // last login date
        //        ms.lastLoginDate != null ? ms.lastLoginDate.Value : DateTime.UtcNow, // last active date
        //        ms.lastPasswordChangedDate != null ? ms.lastPasswordChangedDate.Value : DateTime.UtcNow, // last password change
        //        ms.lastLockoutDate != null ? ms.lastLockoutDate.Value : DateTime.UtcNow // last lock out date
        //        );
        //    return muser;
        //}

        //private User GetUserByName(EntityContext c, string userName)
        //{
        //    return GetUserByName(c, userName, true);
        //}

        //private User GetUserByName(EntityContext c, string userName, bool loadPassword)
        //{
        //    return UsersRolesProvider.GetUsers(c).FirstOrDefault(o => o.userName == userName);
        //}

        //private User GetUserByProviderUserKey(EntityContext c, object providerUserKey)
        //{
        //    Guid g = new Guid(providerUserKey.ToString());
        //    return UsersRolesProvider.GetUserById(c, g);
        //}

        //private User GetUserByEmail(EntityContext c, string email)
        //{
        //    return UsersRolesProvider.GetUsers(c).FirstOrDefault(o => o.email == email);
        //}
        //#endregion Private methods

        #region Unsupported
        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        /// <param name="username">The user to change the password question and answer for. </param>
        /// <param name="password">The password for the specified user. </param>
        /// <param name="newPasswordQuestion">The new password question for the specified user. </param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user. </param>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        /// <param name="username">The user to retrieve the password for. </param>
        /// <param name="answer">The password answer for the user. </param>
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes a user from the membership data source. 
        /// </summary>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.
        /// </returns>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.
        /// </returns>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.
        /// </returns>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert membership password format to integer.
        /// </summary>
        /// <param name="mpf"></param>
        /// <returns></returns>
        public int PasswordFormat2Int(MembershipPasswordFormat mpf)
        {
            if (mpf == MembershipPasswordFormat.Clear)
            {
                return 0;
            }
            else if (mpf == MembershipPasswordFormat.Encrypted)
            {
                return 1;
            }
            else if (mpf == MembershipPasswordFormat.Hashed)
            {
                return 3;
            }

            return -1;
        }

        /// <summary>
        /// Convert membership password format fron int to enum.
        /// </summary>
        public MembershipPasswordFormat PasswordInt2Format(int mpf)
        {
            if (mpf == 0)
            {
                return MembershipPasswordFormat.Clear;
            }
            else if (mpf == 1)
            {
                return MembershipPasswordFormat.Encrypted;
            }
            else if (mpf == 3)
            {
                return MembershipPasswordFormat.Hashed;
            }

            return PasswordFormat;
        }

        #endregion Unsupported
        #endregion
    }
}
