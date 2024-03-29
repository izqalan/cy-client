﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace CyberdropDownloader.Core.DataModels
{
    public class Album
    {
        private readonly string _title;
        private readonly double _size;
        private readonly Queue<AlbumFile> _files;

        public Album(string title, string size, Queue<AlbumFile> files)
        {
            _title = title;
            _size = ConvertAlbumSizeToBytes(size);
            _files = files;
        }

        public string Title => _title;
        public double Size => _size;
        public Queue<AlbumFile> Files => _files;

        private double ConvertAlbumSizeToBytes(string albumSize)
        {
            // KB, MB, GB, TB
            string sizeAbbreviation = albumSize.Substring(albumSize.Length - 2);

            albumSize = albumSize.Replace(sizeAbbreviation, string.Empty).Trim();

            decimal albumSizeDecimal = Convert.ToDecimal(albumSize, CultureInfo.InvariantCulture);

            switch(sizeAbbreviation)
            {
                case "KB":
                    albumSizeDecimal *= 1000;
                    break;

                case "MB":
                    albumSizeDecimal *= 1000000;
                    break;

                case "GB":
                    albumSizeDecimal *= 1000000000;
                    break;

                case "TB":
                    albumSizeDecimal *= 1000000000000;
                    break;

                default:
                    throw new Exception("Unknown filesize");
            }

            decimal roundedValue = decimal.Round(albumSizeDecimal, 0);

            return Convert.ToDouble(roundedValue, CultureInfo.InvariantCulture);
        }
    }
}
