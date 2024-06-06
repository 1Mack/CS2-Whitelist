using System.Net.Http.Json;
using System.Text.Json;
using CounterStrikeSharp.API;
using Microsoft.Extensions.Logging;

namespace Whitelist;

public partial class Whitelist
{
  public async Task<bool> IsWhitelisted(List<string> value)
  {
    if (Config.UseDatabase)
    {
      IEnumerable<dynamic>? whitelisted = await GetFromDatabase(value);

      if (whitelisted != null && whitelisted.Any())
        return true;
    }
    else
    {
      if (WhitelistValues.Any(value.Contains))
        return true;
    }
    return false;
  }
  public void KickPlayer(int userId, string name, string steamid64)
  {
    Server.NextFrame(() =>
    {
      Server.ExecuteCommand($"kickid {(ushort)userId} {Localizer["KickReason"].Value}");
      if (Config.SendMessageOnChatAfterKick) Server.PrintToChatAll(Localizer["KickMessageOnChat", name, steamid64].Value);

    });
  }
  public void CheckFile()
  {
    string path = Path.GetFullPath(
      Path.Combine(ModulePath, $"../../../configs/plugins/{ModuleName}/whitelist.txt")
    );
    Task.Run(async () =>
    {
      if (!File.Exists(path))
      {
        await File.WriteAllTextAsync(
          path,
          @$"// Use '//' to have comments
//You can insert IP, STEAMID, STEAMID64 and STEAMID3
// '//' at beginning = ignore all the line
// '//' at middle of line = just ignore after
189.84.181.96 //IP
STEAM_1:1:79461554 // STEAMID
[U:1:158923109] //STEAMID3
76561198119188837 //STEAMID64
          ");
      }
      string[] lines = await File.ReadAllLinesAsync(path, System.Text.Encoding.UTF8);

      WhitelistValues = lines.Where(line => !line.Trim().StartsWith("//"))
      .SelectMany(line => line.Split("//")
        .Take(1)
        .Select(part => part.Trim())
      )
      .ToArray();
    });
  }
  public async Task<bool> SetToFile(string[] values, bool isInsert)
  {

    try
    {
      string path = Path.GetFullPath(Path.Combine(ModulePath, $"../../../configs/plugins/{ModuleName}/whitelist.txt"));
      IEnumerable<string> result = isInsert
      ?
      WhitelistValues.Concat(values.Except(WhitelistValues))
      :
      WhitelistValues.Except(values);

      await File.WriteAllLinesAsync(path, result);

      WhitelistValues = result.ToArray();

      return true;

    }
    catch (Exception)
    {
      return false;
    }
  }
  public async Task<List<string>?> GetSteamGroupsId(string steamid)
  {

    try
    {

      using var httpClient = new HttpClient();
      JsonElement jsonData = await httpClient.GetFromJsonAsync<JsonElement>($"https://api.steampowered.com/ISteamUser/GetUserGroupList/v1/?key={Config.SteamGroup.Apikey}&steamid={steamid}");
      dynamic? response = jsonData.Deserialize<dynamic>();

      if (!jsonData.TryGetProperty("response", out var responseProperty) ||
          responseProperty.ValueKind != JsonValueKind.Object)
      {
        Logger.LogError("An error occurred: Response is null or not an object.");
        return null;
      }

      if (!responseProperty.GetProperty("success").GetBoolean())
      {
        return null;
      }
      List<string> groupsId = [];

      foreach (var group in responseProperty.GetProperty("groups").EnumerateArray())
      {

        string? groupId = group.GetProperty("gid").GetString();

        if (!string.IsNullOrEmpty(groupId))
          groupsId.Add(groupId);
      }


      return groupsId;
    }
    catch (Exception e)
    {
      Logger.LogError(e.Message);
      return null;
    }
  }
}