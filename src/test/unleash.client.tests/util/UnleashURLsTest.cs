using System;
using Olav.Unleash.Util;
using Xunit;

namespace Olav.Unleash.Client.Tests.Util
{
    public class UnleashURLsTest
    {
        [Fact]//(expected = IllegalArgumentException.class)
        public void should_throw()
        {
            Assert.Throws<UriFormatException>(() => new UnleashURLs(new Uri("unleash.com")));
        }
    }
}