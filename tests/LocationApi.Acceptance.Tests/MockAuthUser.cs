using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace LocationApi.Acceptance.Tests
{
    public class MockAuthUser
    {
        public List<Claim> Claims { get; }

        public MockAuthUser(IEnumerable<Claim> claims) => Claims = claims.ToList();
    }
}