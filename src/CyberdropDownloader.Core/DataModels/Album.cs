using System.Collections.Generic;

namespace CyberdropDownloader.Core.DataModels
{
    public class Album
    {
        private string _title;
        private string _size;
        private Queue<AlbumFile> _files;

        public Album(string title, string size, Queue<AlbumFile> files)
        {
            _title = title;
            _size = size;
            _files = files;
        }

        public string Title { get => _title; }
        public string Size { get => _size; }
        public Queue<AlbumFile> Files { get => _files; }
    }
}
