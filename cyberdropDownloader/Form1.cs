using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace cyberdropDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void downloadBtn_Click(object sender, EventArgs e)
        {
            var scraper = new CyScraper(urlBox.Text);
            scraper.Start();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }

    public class CyScraper
    {
        private string url;
        public CyScraper(String url)
        {
            this.url = url;
        }

        private string title(HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            var title = htmlDoc.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
            return title;
        }

        public void Start()
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            string title = this.title(htmlDoc);
            Console.WriteLine(title);
            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
            {
                string url = link.Attributes["href"].Value;
                
            }
        }
    }


}
