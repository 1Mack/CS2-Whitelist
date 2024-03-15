using CounterStrikeSharp.API.Core;
using static CounterStrikeSharp.API.Core.Listeners;

namespace Whitelist;

public partial class Whitelist : BasePlugin, IPluginConfig<Config>
{
  public override string ModuleName => "Whitelist";
  public override string ModuleDescription => "Only allow players ";
  public override string ModuleAuthor => "1MaaaaaacK";
  public override string ModuleVersion => "1.0.0";
  public static int ConfigVersion => 1;
  public string DatabaseConnectionString = string.Empty;
  public string[] WhitelistValues = Array.Empty<string>();


  public override void Load(bool hotReload)
  {

    RegisterListener<OnClientAuthorized>(OnClientAuthorized);


    AddCommand($"css_{Config.Commands.Add}", "Set Admin", Add);
    AddCommand($"css_{Config.Commands.Remove}", "Remove Admin", Remove);


    if (Config.UseDatabase)
    {
      BuildDatabaseConnectionString();
      TestDatabaseConnection();
    }
    else
    {
      CheckWhitelistFile();
    }
  }
}
