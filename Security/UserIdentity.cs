using QuantumConcepts.Common.DataObjects;
using System;
using System.Linq;
using System.Security.Principal;

namespace QuantumConcepts.Common.Security
{
    public interface IUserIdentity : IIdentity
    {
        IUser User { get; }
        void Refresh();
    }

    public abstract class UserIdentity<D, U> : IUserIdentity
        where U : IUser
    {
        public D DataContext { get; set; }
        IUser IUserIdentity.User { get { return this.User; } }
        public U User { get; private set; }
        public string AuthenticationType { get { return null; } }
        public bool IsAuthenticated { get { return (this.User != null); } }
        public string Name { get { return this.User.FullName; } }

        protected UserIdentity() { }

        public UserIdentity(D context, EncryptionUtil encryptionUtil, string emailAddress, string password)
        {
            U user = GetUserByEmailAddress(context, emailAddress);

            if (user == null)
            {
                throw new ApplicationException("Invalid email address and/or password.");
            }
            else if (!user.IsActive())
            {
                throw new ApplicationException("The account has not been activated.");
            }
            else if (user.IsDenied())
            {
                throw new ApplicationException("Invalid email address and/or password.");
            }
            else if (user.IsDisabled())
            {
                throw new ApplicationException("The account has been disabled.");
            }

            if (!TryAuthenticate(encryptionUtil, user, password))
            {
                throw new ApplicationException("Invalid email address and/or password.");
            }

            this.DataContext = context;
            this.User = user;
        }

        public static bool TryAuthenticate(EncryptionUtil encryptionUtil, U user, string password)
        {
            try
            {
                if (user == null || !user.IsActive() || !Equals(encryptionUtil.DecryptTextViaRijndael(user.PasswordEncrypted.ToArray(), user.EncryptionVector.ToArray()), password))
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        protected abstract U GetUserByID(D context, object id);
        protected abstract U GetUserByEmailAddress(D context, string emailAddress);

        /// <summary>Reloads the current User, alleviating any issues related to cached data.</summary>
        public virtual void Refresh()
        {
            this.User = GetUserByID(this.DataContext, this.User.ID);
        }
    }
}
