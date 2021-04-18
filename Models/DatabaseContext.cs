using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;


namespace BottleCapApi.Models
{
  public partial class DatabaseContext : DbContext
  {

    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }

    public DbSet<Log> Logs { get; set; }



    private string ConvertPostConnectionToConnectionString(string connection)
    {
      var _connection = connection.Replace("postgres://", String.Empty);
      var output = Regex.Split(_connection, ":|@|/");
      return $"server={output[2]};database={output[4]};User Id={output[0]}; password={output[1]}; port={output[3]};SSL Mode=Prefer;Trust Server Certificate=true;";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        var envConn = Environment.GetEnvironmentVariable("DATABASE_URL");
        var conn = "server=localhost;database=BottleCapDatabase";
        if (envConn != null)
        {
          conn = ConvertPostConnectionToConnectionString(envConn);
        }
        optionsBuilder.UseNpgsql(conn);
      }
    }

  }
}