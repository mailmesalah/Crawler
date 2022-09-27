using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfJobCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    static class AsyncDownload
    {
        public static async Task<string> DownloadPageAsync(string url)
        {
            // ... Target page.
            string page = url;

            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(page))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();
                return result;
            }
        }

    }

    class Job
    {
        public string Industry { get; set; }
        public string Career { get; set; }
        public string JobLocation { get; set; }
        public string Salary { get; set; }
        public string Experience { get; set; }
        public string JobType { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Listed { get; set; }
        public string Expires { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
    }

    public partial class MainWindow : Window
    {
        HashSet<string> links = new HashSet<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnDown1_Click(object sender, RoutedEventArgs e)
        {

            List<Job> jobs = new List<Job>();
            string url = txtUrl1.Text.Trim();

            string retData=await AsyncDownload.DownloadPageAsync(url);
            string exp = @"(?<=<a href="+"\""+@")https:\/\/buzzon.khaleejtimes.com\/classifieds\/[a-zA-Z\d:\/\.-]*";
            Match match = Regex.Match(retData, exp);
            
            while (match.Success)
            {
                
                if (!links.Contains(match.Value))
                {
                    links.Add(match.Value);
                    Console.WriteLine(match.Value);
                }
                match=match.NextMatch();
            }

            for (int i = 2; i < 4; i++)
            {
                url = txtUrl1.Text.Trim();
                retData = await AsyncDownload.DownloadPageAsync(url+"page/"+i);
                
                match = Regex.Match(retData, exp);

                if (!match.Success)
                {
                    break;
                }

                while (match.Success)
                {
                    if (!links.Contains(match.Value))
                    {
                        links.Add(match.Value);
                        Console.WriteLine(match.Value);
                    }
                    match = match.NextMatch();
                }
            }

            //All links are downloaded
            foreach (var link in links)
            {
                Job j = new Job();
                retData = await AsyncDownload.DownloadPageAsync(link);

                exp = @"(?<=Industry:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Industry = match.Value;                         
                }

                exp = @"(?<=Career:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Career = match.Value;                    
                }

                exp = @"(?<=Job Location:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.JobLocation = match.Value;                    
                }

                exp = @"(?<=Salary:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Salary = match.Value;                    
                }

                exp = @"(?<=Experience:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Experience = match.Value;                    
                }

                exp = @"(?<=Job Type:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.JobType = match.Value;                    
                }

                exp = @"(?<=Gender:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Gender = match.Value;                    
                }

                exp = @"(?<=Email:<\/span> <a href="+"\""+@"mailto:)[A-Za-z\d._%+-@]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Email = match.Value;                    
                }

                exp = @"(?<=Street:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Street = match.Value;                    
                }

                exp = @"(?<=City:<\/span>)[a-zA-Z\d\s:\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.City = match.Value;                    
                }

                exp = @"(?<=Listed:<\/span>)[a-zA-Z\d\s:,\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Listed = match.Value;                    
                }

                exp = @"(?<=Expires:<\/span>)[a-zA-Z\d\s:,\/\.-]*";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Expires = match.Value;                    
                }

                exp = @"(?<=Description<\/h3>)[a-zA-Z\d._\n\r\t<>\s\/():,-–@]*(?=<\/div>)";
                match = Regex.Match(retData, exp);
                if (match.Success)
                {
                    j.Description = match.Value;                    
                }

                j.Link = link;

                jobs.Add(j);
            }

            //Print to CSV

            using (var textWriter = new StreamWriter(@"C:\Download.csv"))
            {
                string heading = "Industry,Career,job Location,Salary,Experience,Job Type,Gender,Email,Street,City,Listed,Expires,Description,Link";

                textWriter.WriteLine(heading);
                foreach (var jb in jobs)
                {
                    string data = jb.Industry + "," + jb.Career + "," + jb.JobLocation + "," + jb.Salary + "," + jb.Experience + "," + jb.JobType + "," + jb.Gender + "," + jb.Email + "," + jb.Street + "," + jb.City + "," + jb.Listed + "," + jb.Expires + "," + jb.Description + "," + jb.Link;
                    textWriter.WriteLine(data);
                }
            }
            MessageBox.Show("Download Completed!");
        }
    }
}
