using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using dobo.core.Book;
using dobo.core.Extensions;
using dobo.telegram.Book;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.telegram.Command;

public class AdminUpdateDoboCommand(IConfiguration configuration, IHostApplicationLifetime lifeTime)
    : IAdminCommandHandler
{
    private readonly HttpClient httpClient = new();
    private readonly string? githubRepo = configuration.GetString(AppSettingsKey.GitHubRepo);
    private const string TargetAssetName = "dobo-linux-arm.zip";

    public string Command { get; } = "admin_update_dobo";
    public string Description { get; } = "Update the Dobo instance with the latest changes.";

    public async Task<string?> Handle(string? args, Message msg, UpdateType type)
    {
        if (string.IsNullOrEmpty(this.githubRepo))
        {
            return "GitHub repository is not configured.";
        }

        var url = $"https://api.github.com/repos/{githubRepo}/releases";

        // set User-Agent header to "dobo"
        if (httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("dobo");
        }
        var response = await httpClient.GetAsync(url);
        var body = await response.Content.ReadAsStringAsync();
        var releasesResponse = JsonSerializer.Deserialize<GitHubReleasesResponse[]>(body);

        if (releasesResponse == null || releasesResponse.Length == 0)
        {
            return "No releases found in the GitHub repository.";
        }

        var targetRelease = releasesResponse
            .OrderByDescending(release =>
                Version.TryParse(release.TagName.TrimStart('v'), out var version) ? version : new Version(0, 0, 0))
            .FirstOrDefault();

        if (targetRelease != null)
        {
            var targetAsset = targetRelease.Assets
                .FirstOrDefault(asset => asset.Name == TargetAssetName);

            if (targetAsset != null)
            {
                var browserDownloadUrl = targetAsset.BrowserDownloadUrl;
                // scarica l'asset nella directory ../{{versione}}, unzippa il file, avvia  l'eseguibile "dobo" e spegni la versione corrente
                var downloadResponse = await httpClient.GetAsync(browserDownloadUrl);
                if (!downloadResponse.IsSuccessStatusCode)
                {
                    return $"Failed to download the asset: {downloadResponse.ReasonPhrase}";
                }

                var contentStream = await downloadResponse.Content.ReadAsStreamAsync();
                var version = targetRelease.TagName.TrimStart('v');
                var versionDirectory = Path.Combine("..", version);
                if (!Directory.Exists(versionDirectory))
                {
                    Directory.CreateDirectory(versionDirectory);
                }

                var zipFilePath = Path.Combine(versionDirectory, TargetAssetName);
                await using (var fileStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write))
                {
                    await contentStream.CopyToAsync(fileStream);
                }

                // Unzip the file
                ZipFile.ExtractToDirectory(zipFilePath, versionDirectory, true);
                // Remove the zip file after extraction
                File.Delete(zipFilePath);
                // Start the new version of Dobo
                var executablePath = Path.Combine(versionDirectory, "dobo");
                if (File.Exists(executablePath))
                {
                    // Start the new version of Dobo
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = executablePath,
                        WorkingDirectory = versionDirectory,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);

                    lifeTime.StopApplication();

                    return $"Dobo has been updated to version {version} and the new instance has been started.";
                }

                return "Executable file not found in the downloaded asset.";
            }

            // Handle the case where the asset is not found
            return "Target asset not found in the latest release.";
        }

        // Handle the case where no release is found
        return "No valid releases found in the GitHub repository.";
    }
}

public record GitHubReleasesResponse
{
    [JsonPropertyName("tag_name")] public string TagName { get; set; }

    [JsonPropertyName("assets")] public GitHubReleaseAsset[] Assets { get; set; }
}

public record GitHubReleaseAsset
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; }
}