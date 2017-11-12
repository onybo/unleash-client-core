using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Olav.Unleash.Util;
using Serilog;

namespace Olav.Unleash.Repository
{
    public class ToggleBackupHandlerFile : IToggleBackupHandler
    {
        private readonly string _backupFile;

        public ToggleBackupHandlerFile(UnleashConfig config)
        {
            _backupFile = config.BackupFile;
        }

        public ToggleCollection Read()
        {
            Log.Information("Unleash will try to load feature toggle states from temporary backup");
            try
            {
                var json = File.ReadAllText(_backupFile);
                return JsonToggleParser.FromJson(json);
            }
            catch (FileNotFoundException)
            {
                Log.Warning(" Unleash could not find the backup-file '" + _backupFile + "'. \n" +
                        "This is expected behavior the first time unleash runs in a new environment.");
            }
            catch (UnauthorizedAccessException e)
            {
                Log.Warning(e, "Failed to read backup file:'{File}'. Do not have the required permission", _backupFile);
            }
            catch (PathTooLongException e)
            {
                Log.Warning(e, "Failed to read backup file:'{File}'. Path is more than 260 characters", _backupFile);
            }
            catch (ArgumentNullException e)
            {
                Log.Warning(e, "Failed to read backup file:'{File}'. Path is null", _backupFile);
            }
            catch (ArgumentException e)
            {
                Log.Warning(e, "Failed to read backup file:'{File}'. Invalid path", _backupFile);
            }
            catch (DirectoryNotFoundException e)
            {
                Log.Warning(e, "Failed to read backup file:'{File}'. Directory not found", _backupFile);
            }
            catch (NotSupportedException e)
            {
                Log.Warning(e, "Failed to read backup file:'{File}'. Path is in an invalid format", _backupFile);
            }
            catch (JsonException e)
            {
                Log.Warning(e, "Failed to read backup file:'{}'", _backupFile);
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
                Log.Warning(e, "Unleash was unable to backup feature toggles to file: {}", _backupFile);
            }
        }
    }
}
