using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using static CounterStrikeSharp.API.Core.Listeners;

namespace Whitelist;

[MinimumApiVersion(199)]
public partial class Whitelist : BasePlugin, IPluginConfig<Config>
{
  public override string ModuleName => "Whitelist";
  public override string ModuleDescription => "Allow or block players from a list on database or file";
  public override string ModuleAuthor => "1MaaaaaacK";
  public override string ModuleVersion => "1.0.1";
  public static int ConfigVersion => 2;
  public string[] WhitelistValues = [];


  public override void Load(bool hotReload)
  {

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
