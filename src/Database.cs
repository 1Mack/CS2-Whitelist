using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace WhiteList;

public partial class WhiteList
{
  private string _databaseConnectionString = string.Empty;

  private void BuildDatabaseConnectionString()
  {
    var builder = new MySqlConnectionStringBuilder
    {
      Server = Config.Database.Host,
      UserID = Config.Database.User,
      Password = Config.Database.Password,
      Database = Config.Database.Name,
      Port = (uint)Config.Database.Port,
    };

    _databaseConnectionString = builder.ConnectionString;
  }

  async private void CheckDatabaseTables()
  {
    try
    {
      using var connection = new MySqlConnection(_databaseConnectionString);
      await connection.OpenAsync();


      try
      {
        await connection.ExecuteAsync(@$"CREATE TABLE IF NOT EXISTS `{Config.Database.Prefix}` 
        (`id` INT NOT NULL AUTO_INCREMENT, `value` varchar(128) NOT NULL, `server_id` INT, PRIMARY KEY (`id`),
         CONSTRAINT UK_WHITELIST UNIQUE (value,server_id)) 
         ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci");

        await connection.CloseAsync();
      }
      catch (Exception ex)
      {
        Logger.LogError(ex.Message);
        await connection.CloseAsync();
        throw new Exception($"{Localizer["Prefix"]} Unable to create tables!");
      }
    }
    catch (Exception ex)
    {
      throw new Exception($"{Localizer["Prefix"]} Unknown mysql exception! " + ex.Message);
    }
  }
  async public Task<IEnumerable<dynamic>?> GetFromDatabase(List<string> value)
  {
    try
    {
      using var connection = new MySqlConnection(_databaseConnectionString);

      string query = @$"SELECT * FROM {Config.Database.Prefix}
       WHERE value IN ({string.Join(",", value.Select(v => $"'{v}'"))}) 
       {(Config.ServerID >= 0 ? $"AND (server_id = {Config.ServerID} OR server_id = 0)" : "")}";

      IEnumerable<dynamic> result = await connection.QueryAsync(query);
      await connection.CloseAsync();

      return result;

    }
    catch (Exception e)
    {
      Logger.LogError(e.Message);
      return null;
    }
  }
  async public Task<bool> SetToDatabase(string[] value, bool isInsert)
  {
    bool isServerIDEnabled = Config.ServerID >= 0;
    if (!isInsert && isServerIDEnabled)
    {
      for (int i = 0; i < value.Length; i++)
      {
        string[] split = value[i].Split(",");
        split[0] = "(value = " + split[0] + " AND ";
        split[1] = "server_id = " + split[1] + ")";
        value[i] = split[0] + split[1];
      }
    }

    try
    {
      using var connection = new MySqlConnection(_databaseConnectionString);
      string query =
      isInsert ?
          @$"INSERT INTO `{Config.Database.Prefix}` (value{(isServerIDEnabled ? ",server_id" : "")}) 
          VALUES {string.Join(",", string.Join(",", value.Select(v => $"({v})")))}"
      :
      !isServerIDEnabled ?
        @$"DELETE FROM `{Config.Database.Prefix}` WHERE 
        value in ({string.Join(",", value.Select(v => $"'{v}'"))}"
        :
         @$"DELETE FROM `{Config.Database.Prefix}` WHERE {string.Join(" OR ", value)}";

      await connection.ExecuteAsync(query);
      await connection.CloseAsync();
      return true;
    }
    catch (Exception e)
    {
      Logger.LogError(e.Message);
      return false;
    }
  }
}
