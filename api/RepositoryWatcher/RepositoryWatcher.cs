using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryWatcher
{
    public class ImageRepositoryWatcher
    {

        private class ResizeRule
        {

            public string OutputPath { get; set; }

            public int? Width { get; set; }

            public int? Height { get; set; }

        }

        private string filePath;
        private FileSystemWatcher watcher;
        private IList<ResizeRule> rules;

        public Action<FileSystemEventArgs> OnChanged { get; set; }

        public ImageRepositoryWatcher(string filePath)
        {
            this.rules = new List<ResizeRule>();
            this.filePath = filePath;
        }

        public void CheckPendingRequests()
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(this.filePath), 
                Path.GetFileName(this.filePath));
            foreach(var file in files)
            {
                foreach(var rule in this.rules)
                {
                    var output = Path.Combine(rule.OutputPath, Path.GetFileName(file));
                    if(!File.Exists(output)) this.Resize(file, rule);
                }
            }
        }

        public void AddResizeRule(string outputPath, int? width, int? height)
        {
            this.rules.Add(new ResizeRule
            {
                OutputPath = outputPath,
                Width = width,
                Height = height
            });
        }

        public void ProcessImage(string filePath)
        {
            foreach (var rule in this.rules)
            {
                this.Resize(filePath, rule);
            }
        }

        private void OnChangedHandler(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                foreach (var rule in this.rules)
                {
                    this.Resize(e.FullPath, rule);
                }
            }
            this.OnChanged?.Invoke(e);
        }

        private void Resize(string fullPath, ResizeRule rule)
        {
            if (rule.Width.HasValue && !rule.Height.HasValue)
            {
                var binary = BinaryHelper.FromImageToString(fullPath);
                var outputPath = Path.Combine(rule.OutputPath, Path.GetFileName(fullPath));
                ImageHelper.ResizeImageW(binary, rule.Width.Value, outputPath);
            }
            else if (rule.Height.HasValue && !rule.Width.HasValue)
            {
                var binary = BinaryHelper.FromImageToString(fullPath);
                var outputPath = Path.Combine(rule.OutputPath, Path.GetFileName(fullPath));
                ImageHelper.ResizeImageH(binary, rule.Height.Value, outputPath);
            }
            else
            {
                var binary = BinaryHelper.FromImageToString(fullPath);
                var outputPath = Path.Combine(rule.OutputPath, Path.GetFileName(fullPath));
                ImageHelper.ResizeImage(binary, rule.Width.Value, rule.Height.Value, outputPath);
            }
        }

        public void Start()
        {
            this.watcher = new FileSystemWatcher();
            this.watcher.Path = Path.GetDirectoryName(filePath);
            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                NotifyFilters.FileName | NotifyFilters.DirectoryName;
            this.watcher.Filter = Path.GetFileName(filePath);
            this.watcher.Changed += new FileSystemEventHandler(OnChangedHandler);
            this.watcher.Created += new FileSystemEventHandler(OnChangedHandler);
            this.watcher.Deleted += new FileSystemEventHandler(OnChangedHandler);
            this.watcher.Renamed += new RenamedEventHandler(OnChangedHandler);
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }

    }
}
