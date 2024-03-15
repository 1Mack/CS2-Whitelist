using CounterStrikeSharp.API;

namespace Whitelist;

public partial class Whitelist
{
  public async Task<bool> IsWhitelisted(string[] value)
  {
    if (Config.UseDatabase)
    {
      IEnumerable<dynamic>? whitelisted = await GetWhitelistFromDatabase(value);

      if (whitelisted != null && whitelisted.Any())
        return true;
    }
    else
    {
      if (WhitelistValues.Any(wv => value.Contains(wv)))
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
  public void CheckWhitelistFile()
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

      WhitelistValues = lines.Where(line => !line.StartsWith("//"))
      .SelectMany(line => line.Split("//")
        .Take(1)
        .Select(part => part.Trim())
      )
      .ToArray();
    });
  }
  public async Task<bool> SetWhitelistToFile(string[] values, bool isInsert)
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
    catch (System.Exception)
    {
      return false;
    }
  }
}