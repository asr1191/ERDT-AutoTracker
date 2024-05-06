using System;
using System.IO;

namespace ERDT
{
    internal class SavefileWatcher
    {
        private readonly FileSystemWatcher _watcher;
        public EventHandler SavefileChanged;

        public SavefileWatcher(string filepath)
        {
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(filepath), Path.GetFileName(filepath));
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += onWatcherChanged;
        }

        private void onWatcherChanged(object sender, FileSystemEventArgs e)
        {
            SavefileChanged?.Invoke(sender, e);
        }

        public void stopWatching()
        {
            _watcher.EnableRaisingEvents = false;
        }
    }
}
