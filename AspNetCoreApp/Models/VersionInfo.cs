namespace AspNetCoreApp.Models;

public class VersionInfo
{
    public string Version { get; set; } = "1.0.1";
    public string BuildDate { get; set; } = string.Empty;
    public string Environment { get; set; } = "Production";
    public string CommitHash { get; set; } = string.Empty;
    public string CommitDate { get; set; } = string.Empty;
}
