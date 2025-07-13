namespace CubicGUI;
using System.Net;
using System.IO;
using System.Diagnostics;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }
    
    private async Task<bool> DownloadAndExecuteAsync(string url, Button downloadButton, string targetDirectory = null,
        string executeFileName = null)
    {
        try
        {
            downloadButton.Enabled = false;
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string fileName = Path.GetFileName(new Uri(url).LocalPath);
                bool isFolder = string.IsNullOrEmpty(Path.GetExtension(fileName));
                if (isFolder)
                {
                    string folderName = string.IsNullOrEmpty(fileName)
                        ? new Uri(url).Segments.Last().TrimEnd('/')
                        : fileName;
                    string folderPath = targetDirectory != null
                        ? Path.Combine(targetDirectory, folderName)
                        : Path.Combine(Path.GetTempPath(), folderName);

                    Directory.CreateDirectory(folderPath);
                    string htmlContent = await response.Content.ReadAsStringAsync();
                    await DownloadFolderContents(client, url, folderPath, htmlContent);
                    if (!string.IsNullOrEmpty(executeFileName))
                    {
                        string executablePath = Path.Combine(folderPath, executeFileName);
                        if (File.Exists(executablePath))
                        {
                            if (MessageBox.Show($"Folder downloaded. Run {executeFileName}?", "Execute",
                                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Process.Start(new ProcessStartInfo(executablePath) { UseShellExecute = true });
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Folder downloaded, but {executeFileName} was not found.",
                                "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Folder downloaded to: {folderPath}", "Download Complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    return true;
                }
                else
                { 
                    string savePath = targetDirectory != null
                        ? Path.Combine(targetDirectory, fileName)
                        : Path.Combine(Path.GetTempPath(), fileName);
                    if (targetDirectory != null)
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }

                    await File.WriteAllBytesAsync(savePath, await response.Content.ReadAsByteArrayAsync());
                    string fileToExecute = savePath;
                    if (!string.IsNullOrEmpty(executeFileName))
                    {
                        string customExecutePath = targetDirectory != null
                            ? Path.Combine(targetDirectory, executeFileName)
                            : Path.Combine(Path.GetTempPath(), executeFileName);

                        if (File.Exists(customExecutePath))
                        {
                            fileToExecute = customExecutePath;
                        }
                        else
                        {
                            MessageBox.Show($"Downloaded file, but {executeFileName} was not found.", "File Not Found",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return true;
                        }
                    }
                    string executableName = Path.GetFileName(fileToExecute);
                    if (MessageBox.Show($"Download complete. Run {executableName}?", "Execute",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(fileToExecute) { UseShellExecute = true });
                    }

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Download failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally
        {
            downloadButton.Enabled = true;
        }
    }

    private async Task DownloadFolderContents(HttpClient client, string baseUrl, string targetFolder,
        string htmlContent)
    {
        try
        {
            var downloadedFiles = new List<string>();
            var failedFiles = new List<string>();
            var linkPatterns = new[]
            {
                @"href\s*=\s*[""']([^""']+)[""']",
                @"<a[^>]*href\s*=\s*[""']([^""']+)[""'][^>]*>([^<]+)</a>",
                @"href\s*=\s*([^\s>]+)",
                @"<link[^>]*href\s*=\s*[""']([^""']+)[""']"
            };

            var allLinks = new HashSet<string>();
            foreach (var pattern in linkPatterns)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(htmlContent, pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string link = match.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(link))
                    {
                        allLinks.Add(link);
                    }
                }
            }
            var directoryPatterns = new[]
            {
                @"<tr[^>]*>.*?<td[^>]*>.*?<a[^>]*href\s*=\s*[""']([^""']+)[""'][^>]*>([^<]+)</a>",
                @"<li[^>]*>.*?<a[^>]*href\s*=\s*[""']([^""']+)[""'][^>]*>([^<]+)</a>",
                @"<div[^>]*class\s*=\s*[""'][^""']*file[^""']*[""'][^>]*>.*?<a[^>]*href\s*=\s*[""']([^""']+)[""']"
            };

            foreach (var pattern in directoryPatterns)
            {
                var matches = System.Text.RegularExpressions.Regex.Matches(htmlContent, pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                    System.Text.RegularExpressions.RegexOptions.Singleline);

                foreach (System.Text.RegularExpressions.Match match in matches)
                {
                    string link = match.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(link))
                    {
                        allLinks.Add(link);
                    }
                }
            }
            foreach (string link in allLinks)
            {
                if (ShouldSkipLink(link, baseUrl))
                    continue;

                try
                {
                    string fileUrl = ResolveUrl(baseUrl, link);
                    string fileName = GetSafeFileName(link);
                    string filePath = Path.Combine(targetFolder, fileName);
                    if (IsLikelyFile(link) || await IsFileUrl(client, fileUrl))
                    {
                        var fileResponse = await client.GetAsync(fileUrl);
                        if (fileResponse.IsSuccessStatusCode)
                        {
                            var contentType = fileResponse.Content.Headers.ContentType?.MediaType ?? "";
                            if (!contentType.StartsWith("text/html"))
                            {
                                await File.WriteAllBytesAsync(filePath,
                                    await fileResponse.Content.ReadAsByteArrayAsync());
                                downloadedFiles.Add(fileName);
                            }
                        }
                    }
                    else if (link.EndsWith("/") && !link.Equals("../"))
                    {
                        string subDirName = link.TrimEnd('/');
                        string subDirPath = Path.Combine(targetFolder, subDirName);
                        Directory.CreateDirectory(subDirPath);

                        var subResponse = await client.GetAsync(fileUrl);
                        if (subResponse.IsSuccessStatusCode)
                        {
                            string subHtml = await subResponse.Content.ReadAsStringAsync();
                            await DownloadFolderContents(client, fileUrl, subDirPath, subHtml);
                        }
                    }
                }
                catch (Exception ex)
                {
                    failedFiles.Add($"{link}: {ex.Message}");
                    continue;
                }
            }
            if (downloadedFiles.Count > 0 || failedFiles.Count > 0)
            {
                string summary = $"Downloaded {downloadedFiles.Count} files";
                if (failedFiles.Count > 0)
                {
                    summary += $"\nFailed: {failedFiles.Count} files";
                }

                MessageBox.Show(summary, "Download Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error downloading folder contents: {ex.Message}", "Warning", MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private bool ShouldSkipLink(string link, string baseUrl)
    {
        if (string.IsNullOrEmpty(link) || link.Length < 2)
            return true;
        if (link.StartsWith("../") || link.StartsWith("./") || link.Equals("/") || link.Equals("?"))
            return true;
        if (link.StartsWith("#") || link.StartsWith("mailto:") || link.StartsWith("javascript:") ||
            link.StartsWith("tel:") || link.StartsWith("ftp:"))
            return true;
        if (link.StartsWith("http://") || link.StartsWith("https://"))
        {
            try
            {
                var linkUri = new Uri(link);
                var baseUri = new Uri(baseUrl);
                return linkUri.Host != baseUri.Host;
            }
            catch
            {
                return true;
            }
        }
        //var skipExtensions = new[]
            //{ //".css" /*".js", /*".ico", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".woff", /*".woff2" ".ttf"*/ };
        string extension = Path.GetExtension(link).ToLower();
        return false;
    }

    private string ResolveUrl(string baseUrl, string link)
    {
        try
        {
            var baseUri = new Uri(baseUrl);
            var resolvedUri = new Uri(baseUri, link);
            return resolvedUri.ToString();
        }
        catch
        {
            return baseUrl.TrimEnd('/') + "/" + link.TrimStart('/');
        }
    }

    private string GetSafeFileName(string link)
    {
        string fileName = Path.GetFileName(link);
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = link.Replace("/", "_").Replace("\\", "_");
        }
        
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }

        return fileName;
    }

    private bool IsLikelyFile(string link)
    {
        string extension = Path.GetExtension(link);
        if (!string.IsNullOrEmpty(extension) && extension.Length > 1)
            return true;
        
        var filePatterns = new[]
        {
            @"\.(exe|msi|zip|rar|7z|tar|gz|pdf|doc|docx|xls|xlsx|ppt|pptx|txt|log|xml|json|csv|js|cs|css|html|cc|go|c|c++|hlp)$",
            @"\.(mp4|avi|mkv|mov|mp3|wav|flac|ogg)$",
            @"\.(dll|so|dylib|lib|a|o)$"
        };

        foreach (var pattern in filePatterns)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(link, pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                return true;
        }

        return false;
    }

    private async Task<bool> IsFileUrl(HttpClient client, string url)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, url);
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
                return !contentType.StartsWith("text/html");
            }
        }
        catch
        {
            
        }

        return false;
    }


    private void Form1_Load(object sender, EventArgs e)
    {
        Console.WriteLine("loaded");
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        string message = "This will download SysInfo v1.0.3 from lamps-dev.dev\nDo you want to continue?";
        if (MessageBox.Show(message, "Download Software", MessageBoxButtons.YesNo, MessageBoxIcon.Question) !=
            DialogResult.Yes)
            return;

        try
        {
            string url = "https://lamps-dev.dev/files/assets/SysInfo_Setup_Ver1.0.3.exe";
            string savePath = Path.Combine(Path.GetTempPath(), "SysInfo_Setup_Ver1.0.3.exe");

            using (var client = new HttpClient())
            {
                button1.Enabled = false;
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                await File.WriteAllBytesAsync(savePath, await response.Content.ReadAsByteArrayAsync());
            }
            
            if (MessageBox.Show($"Download complete. Run installer?", "Execute", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo(savePath) { UseShellExecute = true });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Download failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            button1.Enabled = true;
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        Close();
    }

    private async void button4_Click(object sender, EventArgs e)
    {
        if (!Directory.Exists("src") || Directory.GetFiles("src").Length == 0)
        {
            Directory.CreateDirectory("src");

            if (MessageBox.Show("Folder didn't exist or was empty.\nDo you want to download the required files?",
                    "Download", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string url = "https://lamps-dev.bitbucket.io/tt";
                await DownloadAndExecuteAsync(url, button4, "src", "texttools.exe");
            }
        }
        else
        {
            string toolPath = Path.Combine("src", "texttools.exe");
            if (File.Exists(toolPath))
            {
                Process.Start(new ProcessStartInfo(toolPath) { UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("texttools.exe not found in src folder.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    private void label5_Click(object sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://lamps.lol/cubicwarn") { UseShellExecute = true });
    }
}