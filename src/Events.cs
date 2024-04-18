using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Logging;

namespace Whitelist;

public partial class Whitelist
{
  private void OnClientAuthorized(int playerSlot, SteamID steamId)
  {
    CCSPlayerController? player = Utilities.GetPlayerFromSlot(playerSlot);

    if (player == null || !player.IsValid || player.IsBot || player.UserId == null)
    {
      if (Config.KickIfFailed)
      {
        if (player != null && player.UserId != null)
        {
          KickPlayer(player.UserId.Value, player.PlayerName, player.SteamID.ToString());
        }
        else
        {
          Logger.LogInformation("Can't kick player");
        }
      }
      return;
    }

    string? ip = player.IpAddress?.Split(":")[0];
    string name = player.PlayerName;
    string steamid64 = steamId.SteamId64.ToString();
    int userId = player.UserId.Value;

    List<string> whitelistOptions = [
      steamid64,
      steamId.SteamId3.ToString(),
      steamId.SteamId2.ToString().Replace("STEAM_0", "STEAM_1")
    ];

    if (Config.SteamGroup.CheckIfMemberIsInGroup)
    {
      Task<List<string>?> task = Task.Run(() => GetSteamGroupsId(steamid64));

      task.Wait();

      if (task.Result == null)
      {
        if (Config.KickIfFailed)
        {
          KickPlayer(userId, name, steamid64);
          return;
        }
      }
      else
      {
        foreach (var item in task.Result)
        {
          whitelistOptions.Add(item);
        }
      }
    }

    Task<bool> task2 = Task.Run(() => IsWhitelisted(ip != null ? [.. whitelistOptions, ip] : whitelistOptions));

    task2.Wait();

    if ((task2.Result && Config.UseAsBlacklist) || (!task2.Result && !Config.UseAsBlacklist))
    {
      KickPlayer(userId, name, steamid64);
    }
  }
}
