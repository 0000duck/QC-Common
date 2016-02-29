using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace QuantumConcepts.Common.DataObjects {
    /// <summary>A multi-purpose interface which defines the basic properties of a user.</summary>
    public interface IUser : IDataObject {
        /// <summary>The user's full name.</summary>
        string FullName { get; }

        /// <summary>The user's email address.</summary>
        string EmailAddress { get; }

        /// <summary>The user's encrypted password.</summary>
        byte[] PasswordEncrypted { get; }

        /// <summary>The encryption vector used to encrypt the user's password.</summary>
        byte[] EncryptionVector { get; }

        /// <summary>Indicates whether or not the user is an Administrator.</summary>
        bool IsAdministrator();

        /// <summary>Determines whether or not the user is active.</summary>
        bool IsActive();

        /// <summary>Determines whether or not the user is banned.</summary>
        bool IsDenied();

        /// <summary>Determines whether or not the user is disabled.</summary>
        bool IsDisabled();
    }
}
