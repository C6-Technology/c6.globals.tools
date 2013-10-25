using System;
using System.Diagnostics;
using System.DirectoryServices;

namespace C6.Globals.Tools
{
    public partial class LittleHelpers
    {
        /// <summary>
        ///
        /// </summary>
        public class ADTools
        {
            #region Create User in Directory

            /// <summary>
            /// Creates a User within an LDAP (AD) Directory.
            ///
            /// NOTE:   If this method is used, AuthenticationTypes.Secure is used as default!
            ///         An error will be simply bubbled trough to allow handling in the app.
            /// </summary>
            /// <param name="Username">The username to be created.</param>
            /// <param name="UserPrincipalName">The user principal name, can be empty</param>
            /// <param name="DirectoryPath">The (LDAP) directory path.</param>
            /// <param name="User">The username to login into LDAP (or null)</param>
            /// <param name="Password">The password to login into LDAP (or null)</param>
            /// <returns>true if user has been successfully created; false if not.</returns>
            public bool CreateUser(string UserID, string UserPWD, string UserDesc, string UserPrincipalName, string DirectoryPath, string AuthUser, string AuthPWD)
            {
                return CreateUser(UserID, UserPWD, UserDesc, UserPrincipalName, DirectoryPath, AuthUser, AuthPWD, AuthenticationTypes.Secure);
            }

            /// <summary>
            /// Creates a User within an LDAP (AD) Directory.
            ///
            /// NOTE:   An error will be simply bubbled trough to allow handling in the app.
            /// </summary>
            /// <param name="UserID">The UserID of the to be created user</param>
            /// <param name="UserPWD">The new password of this user</param>
            /// <param name="UserDesc">A brief description of this user (can be empty!)</param>
            /// <param name="UserPrincipalName">The user principal name (can be empty but might be auto assigned from Win2k3 on!)</param>
            /// <param name="DirectoryPath">The path to the place, the entry should be created</param>
            /// <param name="AuthUser">The user to be authenticated against (can be null)</param>
            /// <param name="AuthPWD">The password to be authenticated with (can be null)</param>
            /// <param name="AuthType">The Authentication type</param>
            /// <returns>true if user has been successfully created; false if not.</returns>
            public bool CreateUser(string UserID, string UserPWD, string UserDesc, string UserPrincipalName, string DirectoryPath, string AuthUser, string AuthPWD, AuthenticationTypes AuthType)
            {
                /*
                 * This is based on
                 * http://en.csharp-online.net/User_Management_with_Active_Directory%E2%80%94Creating_Users
                 * http://support.microsoft.com/default.aspx/kb/306273
                 *
                 */

                bool retValue = false;          // per default we believe that everything won't work out ;)

                try
                {
                    DirectoryEntry parent = new DirectoryEntry(DirectoryPath, AuthUser, AuthPWD, AuthType);

                    DirectoryEntry user = parent.Children.Add("CN=" + UserID, "user");

                    using (user)
                    {
                        //sAMAccountName is required for W2k AD, we would not use
                        //this for ADAM, however.
                        user.Properties["sAMAccountName"].Value = (UserID.IndexOf('@') != 0) ? UserID.Substring(0, UserID.IndexOf('@') - 1) : UserID;       // if @ is within the username, we take only the left part of the username for sAMAccountName

                        //userPrincipalName is not required, but recommended
                        //for ADAM. AD also contains this, so we can use it.
                        user.Properties["userPrincipalName"].Value = UserPrincipalName;

                        if (UserPWD != "")
                            user.Invoke("SetPassword", new object[] { UserPWD });

                        if (UserDesc != "")
                            user.Invoke("Put", new object[] { "Description", UserDesc });

                        user.CommitChanges();

                        retValue = true;
                    }
                }
                catch (DirectoryServicesCOMException DSError)
                {
                    Debug.Print("DSError: " + DSError.Message);
                    throw DSError;
                }
                catch (Exception err)
                {
                    Debug.Print("Common Error: " + err.Message);
                    throw err;
                }

                return retValue;
            }

            #endregion Create User in Directory

            #region Add User to Group

            #endregion Add User to Group
        }
    }
}