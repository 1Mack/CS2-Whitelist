using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Whitelist;

public partial class Whitelist
{
  [CommandHelper(minArgs: 1, usage: "[values] [values2] [values3]...")]
  public void Add(CCSPlayerController? player, CommandInfo command)
  {
    if (!string.IsNullOrEmpty(Config.Commands.AddPermission) && !AdminManager.PlayerHasPermissions(player, Config.Commands.AddPermission.Split(";")))
    {
      command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer["MissingCommandPermission"]}");
      return;
    }
    string[] commands = command.ArgString.Split(" ");
    Task.Run(async () =>
    {
      bool result = Config.UseDatabase ? await SetWhitelistToDatabase(commands, true) : await SetWhitelistToFile(commands, true);

      Server.NextFrame(() =>
      {
        if (result)
          Server.PrintToChatAll($"{Localizer["Prefix"]} {Localizer["Success"]}");
        else
          Server.PrintToChatAll($"{Localizer["Prefix"]} {Localizer["Failure"]}");
      });
    });

  }
}