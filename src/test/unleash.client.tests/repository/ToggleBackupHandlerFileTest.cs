using System;
using System.IO;
using System.Linq;
using Olav.Unleash.Repository;
using Olav.Unleash.Util;
using Xunit;

namespace Olav.Unleash.Client.Tests.Repository
{


    public class ToggleBackupHandlerFileTest
    {

        [Fact]
        public void test_read()
        {
            var config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://http://unleash.org")
                    .BackupFile("resources/unleash-repo-v0.json")
                    .Build();
            var toggleBackupHandlerFile = new ToggleBackupHandlerFile(config);
            var toggleCollection = toggleBackupHandlerFile.Read();
            Assert.NotNull(toggleCollection.GetToggle("presentFeature"));
        }

        [Fact]
        public void test_read_file_with_invalid_data()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://unleash.org")
                    .BackupFile("resources/unleash-repo-without-feature-field.json")
                    .Build();

            var fileGivingNullFeature = new ToggleBackupHandlerFile(config);
            Assert.NotNull(fileGivingNullFeature.Read());
        }

        [Fact]
        public void test_read_without_file()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://unleash.org")
                    .BackupFile("/does/not/exist.json")
                    .Build();

            ToggleBackupHandlerFile toggleBackupHandlerFile = new ToggleBackupHandlerFile(config);
            ToggleCollection toggleCollection = toggleBackupHandlerFile.Read();

            Assert.Null(toggleCollection.GetToggle("presentFeature"));
        }

        [Fact]
        public void test_write_strategies()
        {
            var backupFile = Path.GetTempPath() + Path.PathSeparator + "unleash-repo-test-write.json";
            var config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://unleash.org")
                    .BackupFile(backupFile)
                    .Build();

            var staticData = "{\"features\": [{\"name\": \"writableFeature\",\"enabled\": true,\"strategy\": \"default\"}]}";

            var toggleCollection = JsonToggleParser.FromJson(staticData);

            var toggleBackupHandlerFile = new ToggleBackupHandlerFile(config);
            toggleBackupHandlerFile.Write(toggleCollection);
            toggleBackupHandlerFile = new ToggleBackupHandlerFile(config);
            toggleCollection = toggleBackupHandlerFile.Read();
            Assert.NotNull(toggleCollection.GetToggle("writableFeature"));
        }

        [Fact]
        public void test_read_old_format_with_strategies()
        {
            UnleashConfig config = UnleashConfig.TheBuilder()
                    .AppName("test")
                    .UnleashAPI("http://unleash.org")
                    .BackupFile("resources/unleash-repo-v0.json")
                    .Build();

            var toggleBackupHandlerFile = new ToggleBackupHandlerFile(config);
            var toggleCollection = toggleBackupHandlerFile.Read();
            Assert.NotNull(toggleCollection.GetToggle("featureCustomStrategy"));
            Assert.Single(toggleCollection.GetToggle("featureCustomStrategy").Strategies);
            Assert.Equal("customValue", toggleCollection.GetToggle("featureCustomStrategy").Strategies.First().Parameters["customParameter"]);
        }
    }
}