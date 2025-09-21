using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Logging;

namespace Whitelist;

public partial class Whitelist
{
    private void OnClientAuthorized(int playerSlot, SteamID steamId)
    {
        var player = Utilities.GetPlayerFromSlot(playerSlot);
        if (player is null || !player.IsValid || player.IsBot || player.UserId is null)
        {
            if (!Config.KickIfFailed)
                return;
            
            if (player != null && player.UserId != null)
                KickPlayer(player.UserId.Value, player.PlayerName, player.SteamID.ToString());
            else
                Logger.LogInformation("Can't kick player");
            
            return;
        }

        var ip = player.IpAddress?.Split(":")[0];
        var name = player.PlayerName;
        var steamId64 = steamId.SteamId64.ToString();
        var userId = player.UserId.Value;

        List<string> whitelistOptions = [
            steamId64,
            steamId.SteamId3,
            steamId.SteamId2.Replace("STEAM_0", "STEAM_1")
        ];

        Task.Run(() =>
        {
            if (Config.SteamGroup.CheckIfMemberIsInGroup)
            {
                var groups = GetSteamGroupsId(steamId64);
                if (groups.Result is not null)
                {
                    whitelistOptions.AddRange(groups.Result);
                }
                else if (Config.KickIfFailed)
                {
                    KickPlayer(userId, name, steamId64);
                    return Task.FromResult(false);
                }
            }
            
            return IsWhitelisted(ip != null ? [.. whitelistOptions, ip] : whitelistOptions);
        }).ContinueWith(task =>
        {
            if ((task.Result && Config.UseAsBlacklist) || (!task.Result && !Config.UseAsBlacklist))
                KickPlayer(userId, name, steamId64);
        });
    }
}
