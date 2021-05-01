using System.Collections.Generic;

namespace BottleCapApi.Models
{
  public class Game
  {
    public int Id { get; set; }

    public string SlackId { get; set; }

    public string ChannelName { get; set; }

    public string TeamId { get; set; }



    public List<Player> Players { get; set; } = new List<Player>();

    public List<DungeonMaster> DungeonMasters { get; set; } = new List<DungeonMaster>();



  }
}