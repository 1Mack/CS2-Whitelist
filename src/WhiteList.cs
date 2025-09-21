using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;

namespace WhiteList;

public partial class WhiteList : BasePlugin, IPluginConfig<Config>
{
  public override string ModuleName => "WhiteList";
  public override string ModuleDescription => "Allow or block players from a list on database or file";
  public override string ModuleAuthor => "1MaaaaaacK";
  public override string ModuleVersion => "1.0.3";
  public static int ConfigVersion => 3;
  public string[] WhiteListValues = [];


  public override void Load(bool hotReload)
  {
    if (Config.UsePrivateFeature)
    {
      if (hotReload)
      {
        Config.Enabled = Convar_isPluginEnabled.Value;
        Config.UseAsBlacklist = Convar_useAsBlacklist.Value;
      }
      Convar_isPluginEnabled.ValueChanged += (_, value) =>
      {
        Config.Enabled = value;
      };
      if (!Config.Enabled)
      {
        Logger.LogWarning("This plugin is disabled");
        return;
      }
      Convar_useAsBlacklist.ValueChanged += (_, value) =>
      {
        Config.UseAsBlacklist = value;
      };
    }

    RegisterListener<OnClientAuthorized>(OnClientAuthorized);


    AddCommand($"css_{Config.Commands.Add}", "Set Admin", Add);
    AddCommand($"css_{Config.Commands.Remove}", "Remove Admin", Remove);

    CheckVersion();


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

  public FakeConVar<bool> Convar_isPluginEnabled = new("plugin_whitelist_enabled", "Enable WhiteList", true);
  public FakeConVar<bool> Convar_useAsBlacklist = new("plugin_whitelist_useasblacklist", "Use as blacklist", false);

}
