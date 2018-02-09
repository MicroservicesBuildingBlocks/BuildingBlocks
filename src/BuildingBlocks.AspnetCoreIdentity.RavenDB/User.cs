using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace BuildingBlocks.AspnetCoreIdentity.RavenDB
{
    public class User
    {
        private readonly List<SimplifiedClaim> _claims = new List<SimplifiedClaim>();

        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string PasswordHash { get; set; }

        public IEnumerable<SimplifiedClaim> Claims
        {
            get => _claims;
            internal set
            {
                if (value != null) _claims.AddRange(value);
            }
        }

        internal void AddClaim(SimplifiedClaim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            _claims.Add(claim);
        }

        internal void RemoveClaim(SimplifiedClaim claim)
        {
            _claims.Remove(claim);
        }
    }

    public class SimplifiedClaim : IEquatable<SimplifiedClaim>, IEquatable<Claim>
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public static implicit operator SimplifiedClaim(Claim original) =>
            new SimplifiedClaim { Type = original.Type, Value = original.Value };

        public static implicit operator Claim(SimplifiedClaim simplified) =>
            new Claim(simplified.Type, simplified.Value);

        public bool Equals(SimplifiedClaim other)
            => Type == other.Type && Value == other.Value;

        public bool Equals(Claim other)
            => Type == other.Type && Value == other.Value;
    }
}
