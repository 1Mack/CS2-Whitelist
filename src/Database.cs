using Dapper;
using MySqlConnector;

namespace Whitelist;

public partial class Whitelist
{
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

    DatabaseConnectionString = builder.ConnectionString;
  }

  private void TestDatabaseConnection()
  {
    try
    {
      using var connection = new MySqlConnection(DatabaseConnectionString);
      connection.Open();

      if (connection.State != System.Data.ConnectionState.Open)
      {
        throw new Exception($"{Localizer["Prefix"]} Unable connect to database!");
      }
    }
    catch (Exception ex)
    {
      throw new Exception($"{Localizer["Prefix"]} Unknown mysql exception! " + ex.Message);
    }
    CheckDatabaseTables();
  }

  async private void CheckDatabaseTables()
  {
    try
    {
      using var connection = new MySqlConnection(DatabaseConnectionString);
      await connection.OpenAsync();


      try
      {
        await connection.ExecuteAsync(@$"CREATE TABLE IF NOT EXISTS `{Config.Database.Prefix}` 
        (`id` INT NOT NULL AUTO_INCREMENT, `value` varchar(128) UNIQUE, PRIMARY KEY (`id`)) 
         ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci");

        await connection.CloseAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        await connection.CloseAsync();
        throw new Exception($"{Localizer["Prefix"]} Unable to create tables!");
      }
    }
    catch (Exception ex)
    {
      throw new Exception($"{Localizer["Prefix"]} Unknown mysql exception! " + ex.Message);
    }
  }
  async public Task<IEnumerable<dynamic>?> GetWhitelistFromDatabase(string[] value)
  {
    try
    {
      using var connection = new MySqlConnection(DatabaseConnectionString);

      string query = $"SELECT * FROM {Config.Database.Prefix} WHERE value IN ({string.Join(",", value.Select(v => $"'{v}'"))})";

      IEnumerable<dynamic> result = await connection.QueryAsync(query, new { value });
      await connection.CloseAsync();

      return result;

    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      return null;
    }
  }
  async public Task<bool> SetWhitelistToDatabase(string[] value, bool isInsert)
  {
    try
    {
      using var connection = new MySqlConnection(DatabaseConnectionString);

      string query = isInsert ?
       $"INSERT INTO `{Config.Database.Prefix}` (value) VALUES {string.Join(",", value.Select(v => $"('{v}')"))}"
      :
      $"DELETE FROM `{Config.Database.Prefix}` WHERE value in ({string.Join(",", value.Select(v => $"'{v}'"))})"
      ;

      await connection.ExecuteAsync(query, new { value });
      await connection.CloseAsync();
      return true;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      return false;
    }
  }
}
