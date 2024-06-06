using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

namespace Whitelist;

[MinimumApiVersion(199)]
public partial class Whitelist : BasePlugin, IPluginConfig<Config>
{
  public override string ModuleName => "Whitelist";
  public override string ModuleDescription => "Allow or block players from a list on database or file";
  public override string ModuleAuthor => "1MaaaaaacK";
  public override string ModuleVersion => "1.0.2";
  public static int ConfigVersion => 3;
  public string[] WhitelistValues = [];


  public override void Load(bool hotReload)
  {
    if (!Config.Enabled)
    {
      Logger.LogWarning("This plugin is disabled");
      return;
    }

    RegisterListener<OnClientAuthorized>(OnClientAuthorized);


    AddCommand($"css_{Config.Commands.Add}", "Set Admin", Add);
    AddCommand($"css_{Config.Commands.Remove}", "Remove Admin", Remove);


    if (Config.UseDatabase)
    {
      BuildDatabaseConnectionString();
      CheckDatabaseTables();
    }
    else
    {
      CheckFile();
    }
  }
}
