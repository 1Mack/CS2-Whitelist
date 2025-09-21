using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;

namespace WhiteList;

public partial class WhiteList
{
  [CommandHelper(minArgs: 1, usage: "[values&serverID] [values2] [values3&serverID]...")]
  public void Add(CCSPlayerController? player, CommandInfo command)
  {
    if (!Config.Enabled)
    {
      command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer["PluginDisabled"]}");
      return;
    }
    if (!string.IsNullOrEmpty(Config.Commands.AddPermission) && !AdminManager.PlayerHasPermissions(player, Config.Commands.AddPermission.Split(";")))
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

    Task<bool> task = Task.Run(() => Config.UseDatabase ? SetToDatabase(commands, true) : SetToFile(commands, true));

    task.Wait();
    command.ReplyToCommand($"{Localizer["Prefix"]} {Localizer[task.Result ? "Success" : "Failure"]}");
  }
}