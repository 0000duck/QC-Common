using QuantumConcepts.Common.DataObjects;
using System.Linq;
using System.Security.Principal;

namespace QuantumConcepts.Common.Security
{
    /// <summary>Defines basic requirements to implement an <see cref="IPrincipal"/>.</summary>
    public interface IUserPrincipal : IPrincipal
    {
        /// <summary>The <see cref="IUserIdentity"/> for this instance.</summary>
        IUserIdentity UserIdentity { get; }
    }

    /// <summary>This class handles authentication for an <see cref="IUser"/> against a <see cref="DataContext"/>.</summary>
    /// <typeparam name="D">The type of the data context/client.</typeparam>
    /// <typeparam name="U">The type of the <see cref="IUser"/>.</typeparam>
    public abstract class UserPrincipal<D, U> : IUserPrincipal
        where U : IUser
    {
        IUserIdentity IUserPrincipal.UserIdentity { get { return this.UserIdentity; } }

        /// <summary>The <see cref="IUserIdentity"/> for this instance.</summary>
        public UserIdentity<D, U> UserIdentity { get; private set; }

        /// <summary>The <see cref="IIdentity"/> for this instance.</summary>
        public IIdentity Identity { get { return (this.UserIdentity as IIdentity); } }

        /// <summary>Creates a new <see cref="UserPrincipal"/> from the provided <see cref="UserIdentity"/>.</summary>
        /// <param name="userIdentity">The identity to use to create the <see cref="UserPrincipal"/>.</param>
        protected UserPrincipal(UserIdentity<D, U> userIdentity)
        {
            this.UserIdentity = userIdentity;
        }

        /// <summary>Creates a new <see cref="UserPrincipal"/> from the provided username and password.</summary>
        /// <param name="context">The <see cref="DataContext"/> to use to query and authenticate the user.</param>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password to authenticate.</param>
        public UserPrincipal(D context, string username, string password)
        {
            this.UserIdentity = CreateUserIdentity(context, username, password);
        }

        /// <summary>Creates a new <see cref="UserPrincipal"/> from the provided <see cref="IUser"/>.</summary>
        /// <remarks>This constructor does not require a password; the password is extracted from the provided user. Therefore, this constructor bypasses authentication.</remarks>
        /// <param name="context">The <see cref="DataContext"/> to use to query and authenticate the user.</param>
        /// <param name="user">The <see cref="IUser"/> from which to create the <see cref="UserPrincipal"/>.</param>
        public UserPrincipal(D context, EncryptionUtil encryptionUtil, U user)
        {
            string username = user.EmailAddress;
            string password = encryptionUtil.DecryptTextViaRijndael(user.PasswordEncrypted.ToArray(), user.EncryptionVector.ToArray());

            this.UserIdentity = CreateUserIdentity(context, username, password);
        }

        /// <summary>Indicates whether or not the <see cref="UserIdentity"/> has full access.</summary>
        public abstract bool HasFullAccess();

        /// <summary>Indicates whether or not the <see cref="UserIdentity"/> is a member of the provided role.</summary>
        public abstract bool IsInRole(string role);

        /// <summary>Allows derrived classes to query and authenticate the <see cref="UserIdentity"/>.</summary>
        /// <param name="context">The <see cref="DataContext"/> to use.</param>
        /// <param name="username">The username to authenticate.</param>
        /// <param name="password">The password to authenticate.</param>
        public abstract UserIdentity<D, U> CreateUserIdentity(D context, string username, string password);
    }
}
