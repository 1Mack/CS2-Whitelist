using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace Whitelist;

public partial class Whitelist
{
  [CommandHelper(minArgs: 1, usage: "[values] [values2] [values3]...")]
  public void Remove(CCSPlayerController? player, CommandInfo command)
  {
    if (!string.IsNullOrEmpty(Config.Commands.RemovePermission) && !AdminManager.PlayerHasPermissions(player, Config.Commands.RemovePermission.Split(";")))
    {
      command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer["MissingCommandPermission"]}");
      return;
    }
    string[] commands = command.ArgString.Split(" ");

    if (Config.ServerID > 0 && Config.UseDatabase == true)
    {
      commands = commands.Select(v =>
      {
        if (v.Contains('&'))
        {
          string[] toSplit = v.Split("&");
          return $"\"{toSplit[0]}\",{toSplit[1]}";
        }
        else
        {
          return $"\"{v}\",{Config.ServerID}";
        }

      }).ToArray();
    }
    else if (commands.Any(v => v.Split(",").Length > 1))
    {
      command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer["Failure"]}");
      return;
    }

    Task<bool> task = Task.Run(() => Config.UseDatabase ? SetToDatabase(commands, false) : SetToFile(commands, false));

    task.Wait();
    command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer[task.Result ? "Success" : "Failure"]}");
  }
}