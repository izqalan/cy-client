using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cyberdropDownloader
{
    class CyFile
    {
        string url, filename;
        int size; // in byte

        public CyFile(string url, string filename, int filesize)
        {
            URL = url;
            Size = size;
            FileName = filename;
        }

        public int Size { get => size; set => size = value; }
        public string URL { get => url; set => url = value; }
        public string FileName{ get => filename; set => filename = value; }
    }


}
