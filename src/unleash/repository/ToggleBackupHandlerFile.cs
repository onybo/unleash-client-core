using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Olav.Unleash.Logging;
using Olav.Unleash.Util;

namespace Olav.Unleash.Repository
{
    public class ToggleBackupHandlerFile : IToggleBackupHandler
    {
        private readonly string _backupFile;
        private static readonly ILog Logger = LogProvider.For<ToggleBackupHandlerFile>();

        public ToggleBackupHandlerFile(UnleashConfig config)
        {
            _backupFile = config.BackupFile;
        }

        public ToggleCollection Read()
        {
            Logger.Info("Unleash will try to load feature toggle states from temporary backup");
            try
            {
                var json = File.ReadAllText(_backupFile);
                return JsonToggleParser.FromJson(json);
            }
            catch (FileNotFoundException)
            {
                Logger.Warn(" Unleash could not find the backup-file '" + _backupFile + "'. \n" +
                        "This is expected behavior the first time unleash runs in a new environment.");
            }
            catch (UnauthorizedAccessException e)
            {
                Logger.WarnException("Failed to read backup file:'{File}'. Do not have the required permission", e, _backupFile);
            }
            catch (PathTooLongException e)
            {
                Logger.WarnException("Failed to read backup file:'{File}'. Path is more than 260 characters", e, _backupFile);
            }
            catch (ArgumentNullException e)
            {
                Logger.WarnException("Failed to read backup file:'{File}'. Path is null", e, _backupFile);
            }
            catch (ArgumentException e)
            {
                Logger.WarnException("Failed to read backup file:'{File}'. Invalid path", e, _backupFile);
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.WarnException("Failed to read backup file:'{File}'. Directory not found", e, _backupFile);
            }
            catch (NotSupportedException e)
            {
                Logger.WarnException("Failed to read backup file:'{File}'. Path is in an invalid format", e, _backupFile);
            }
            catch (JsonException e)
            {
                Logger.WarnException("Failed to read backup file:'{}'", e, _backupFile);
            }

            return ToggleCollection.EmptyCollection;

        }

        public void Write(ToggleCollection toggleCollection)
        {
            try
            {
                using (var writer = new StreamWriter(_backupFile))
                {
                    var json = JsonConvert.SerializeObject(toggleCollection);
                    writer.Write(JsonConvert.SerializeObject(toggleCollection));
                }
            }
            catch (IOException e)
            {
                Logger.WarnException("Unleash was unable to backup feature toggles to file: {}", e, _backupFile);
            }
        }
    }
}
