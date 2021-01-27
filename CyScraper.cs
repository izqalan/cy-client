public class CyScraper
{
    private string url;
    private string dest;
    private DownloadService downloader;
    public string itemName;
    public CyScraper(String url, String path, DownloadService downloader)
    {
        this.url = url;
        this.dest = path;
        this.downloader = downloader;
    }

    private string Title(HtmlAgilityPack.HtmlDocument htmlDoc)
    {
        var title = htmlDoc.DocumentNode.SelectNodes("//div/h1[@id='title']").First().Attributes["title"].Value;
        return title;
    }

    public async void StartAsync()
    {
        try
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            string title = this.Title(htmlDoc);

            foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@class='image'][@href]"))
            {
                string url = link.Attributes["href"].Value;
                itemName = link.Attributes["title"].Value;
                // download here
                string filepath = String.Format(@"{0}\{1}\{2}", dest, title, itemName);
                Console.WriteLine(filepath);
                await downloader.DownloadFileAsync(url, filepath);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("error StartAsync");
            Console.WriteLine(e);
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}