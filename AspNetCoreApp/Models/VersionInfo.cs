namespace AspNetCoreApp.Models;

public class VersionInfo
{
    public string Version { get; set; } = "1.0.1";
    public string BuildDate { get; set; }
    public string Environment { get; set; } = "Production";
}
