using System.Text.Json.Serialization;

namespace BottleCapApi.Models
{
  public class Player
  {
    public int Id { get; set; }

    public string SlackId { get; set; }

    public string Name { get; set; }

    public int BottleCaps { get; set; } = 0;

    [JsonIgnore]
    public int GameId { get; set; }
    [JsonIgnore]
    public Game Game { get; set; }

  }
}