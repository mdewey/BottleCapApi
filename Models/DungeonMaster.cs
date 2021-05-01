using System.Text.Json.Serialization;

namespace BottleCapApi.Models
{
  public class DungeonMaster
  {
    public int Id { get; set; }

    public string SlackId { get; set; }
    public string SlackName { get; set; }



    [JsonIgnore]
    public int GameId { get; set; }
    [JsonIgnore]
    public Game Game { get; set; }


  }
}