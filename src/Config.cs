using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace Whitelist;

public partial class Whitelist
{
  public required Config Config { get; set; }

  public void OnConfigParsed(Config config)
  {
    if (config.Version != ConfigVersion) throw new Exception($"You have a wrong config version. Delete it and restart the server to get the right version ({ConfigVersion})!");

    if (config.UseDatabase && (config.Database.Host.Length < 1 || config.Database.Name.Length < 1 || config.Database.User.Length < 1))
    {
      throw new Exception($"You need to setup Database credentials in config!");
    }

    Config = config;
  }

}
public class Config : BasePluginConfig
{
  [JsonPropertyName("UseDatabase")]
  public bool UseDatabase { get; set; } = true;
  [JsonPropertyName("KickIfFailed")]
  public bool KickIfFailed { get; set; } = false;
  [JsonPropertyName("UseAsBlacklist")]
  public bool UseAsBlacklist { get; set; } = false;
  [JsonPropertyName("SendMessageOnChatAfterKick")]
  public bool SendMessageOnChatAfterKick { get; set; } = true;
  [JsonPropertyName("Database")]
  public Database Database { get; set; } = new();
  [JsonPropertyName("Commands")]
  public Commands Commands { get; set; } = new();
}
public class Database
{
  [JsonPropertyName("Host")]
  public string Host { get; set; } = "";
  [JsonPropertyName("Port")]
  public int Port { get; set; } = 3306;
  [JsonPropertyName("User")]
  public string User { get; set; } = "";
  [JsonPropertyName("Password")]
  public string Password { get; set; } = "";
  [JsonPropertyName("Name")]
  public string Name { get; set; } = "";
  [JsonPropertyName("Prefix")]
  public string Prefix { get; set; } = "whitelist";
}
public class Commands
{
  [JsonPropertyName("Add")]
  public string Add { get; set; } = "wladd";
  [JsonPropertyName("AddPermission")]
  public string AddPermission { get; set; } = "@css/root";
  [JsonPropertyName("Remove")]
  public string Remove { get; set; } = "wlremove";
  [JsonPropertyName("RemovePermission")]
  public string RemovePermission { get; set; } = "@css/root";

}