using System.Collections.Generic;
using Olav.Unleash.Strategy;
using Xunit;

namespace Olav.Unleash.Client.Core.Tests.Strategy
{
    public class ApplicationHostnameStrategyTest
    {
        [Fact]
        public void should_be_disabled_if_no_HostNames_in_parms()
        {
            var strategy = new ApplicationHostnameStrategy();
            var parms = new Dictionary<string, string>();
            parms.Add("hostNames", null);

            Assert.False(strategy.IsEnabled(parms));
        }

        [Fact]
        public void should_be_disabled_if_hostname_not_in_list()
        {
            var strategy = new ApplicationHostnameStrategy();
            var parms = new Dictionary<string, string>();
            parms.Add("hostNames", "MegaHost,MiniHost, happyHost");

            Assert.False(strategy.IsEnabled(parms));
        }

        // [Fact]
        // public void should_be_enabled_for_hostName()
        // {
        //     string hostName = "my-super-host";
        //     //System.setProperty("hostname", hostName);

        //     var strategy = new ApplicationHostnameStrategy();

        //     var parms = new Dictionary<string, string>();
        //     parms.Add("hostNames", "MegaHost," + hostName + ",MiniHost, happyHost");
        //     Assert.True(strategy.IsEnabled(parms));
        // }

        // [Fact]
        // public void should_handle_weird_casing()
        // {
        //     string hostName = "my-super-host";
        //     //System.setProperty("hostname", hostName);

        //     var strategy = new ApplicationHostnameStrategy();

        //     var parms = new Dictionary<string, string>();

        //     parms.Add("hostNames", "MegaHost," + hostName.ToUpper() + ",MiniHost, happyHost");
        //     Assert.True(strategy.IsEnabled(parms));
        // }

        // [Fact]
        // public void so_close_but_no_cigar()
        // {
        //     string hostName = "my-super-host";
        //     //System.setProperty("hostname", hostName);

        //     var strategy = new ApplicationHostnameStrategy();

        //     var parms = new Dictionary<string, string>();

        //     parms.Add("hostNames", "MegaHost, MiniHost, SuperhostOne");
        //     Assert.False(strategy.IsEnabled(parms));
        // }

        // [Fact]
        // public void should_be_enabled_for_InetAddress()
        // {
        //     string hostName = System.Net.Dns.GetHostName();
        //     //System.setProperty("hostname", hostName);

        //     var strategy = new ApplicationHostnameStrategy();

        //     var parms = new Dictionary<string, string>();
        //     parms.Add("hostNames", "MegaHost," + hostName + ",MiniHost, happyHost");
        //     Assert.True(strategy.IsEnabled(parms));
        // }

        // [Fact]
        // public void should_be_enabled_for_dashed_host()
        // {
        //     string hostName = "super-wiEred-host";
        //     //System.setProperty("hostname", hostName);

        //     var strategy = new ApplicationHostnameStrategy();

        //     var parms = new Dictionary<string, string>();
        //     parms.Add("hostNames", "MegaHost," + hostName + ",MiniHost, happyHost");
        //     Assert.True(strategy.IsEnabled(parms));
        // }

        [Fact]
        public void null_test()
        {
            var strategy = new ApplicationHostnameStrategy();
            Assert.False(strategy.IsEnabled(new Dictionary<string, string>()));
        }
    }
}