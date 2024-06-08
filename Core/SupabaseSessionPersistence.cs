using System;
using System.IO;
using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace ERDT.Core
{
    internal class SupabaseSessionPersistence : IGotrueSessionPersistence<Session>
    {
        private Session _session;

        private static readonly string SessionFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "EldenRingDeathTracker",
                ".gotrue.cache"
            );

        public SupabaseSessionPersistence(Session session = null)
        {
            _session = session;
            Directory.CreateDirectory(Path.GetDirectoryName(SessionFilePath));
        }

        public void SaveSession(Session session)
        {
            var json = JsonConvert.SerializeObject(session);
            try
            {
                File.WriteAllText(SessionFilePath, json);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.ToString());
            }

        }

        public Session LoadSession()
        {
            if (_session == null)
            {
                if (File.Exists(SessionFilePath))
                {
                    var json = File.ReadAllText(SessionFilePath);
                    _session = JsonConvert.DeserializeObject<Session>(json);
                    Console.WriteLine("Loaded session");
                    return _session;
                }
                return null;
            }
            return _session;
        }

        public void DestroySession()
        {
            if (File.Exists(SessionFilePath))
            {
                // Delete the file
                File.Delete(SessionFilePath);
                Console.WriteLine("File deleted successfully.");
            }
        }
    }
}