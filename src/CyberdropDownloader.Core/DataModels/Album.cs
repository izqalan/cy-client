using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CyberdropDownloader.Core.DataModels
{
    public class Album
    {
        private string _title;
        private double _size;
        private Queue<AlbumFile> _files;

        public Album(string title, string size, Queue<AlbumFile> files)
        {
            _title = title;
            _size = ConvertAlbumSizeToBytes(size);
            _files = files;
        }

        public string Title { get => _title; }
        public double Size { get => _size; }
        public Queue<AlbumFile> Files { get => _files; }

        private double ConvertAlbumSizeToBytes(string albumSize)
        {
            decimal byteValue = 0;

            if (albumSize.Contains("KB"))
                byteValue = 1024;
            else if (albumSize.Contains("MB"))
                byteValue = 1048576;
            else if (albumSize.Contains("GB"))
                byteValue = 1073741824;
            else if (albumSize.Contains("TB"))
                byteValue = 1099511627776;

            decimal regexValue = Convert.ToDecimal(Regex.Replace(albumSize, @"[a-zA-Z]+", string.Empty));

            double roundedValue = Convert.ToDouble(decimal.Round(regexValue * byteValue, 0));

            return roundedValue;
        }
    }
}
