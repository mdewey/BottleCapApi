namespace BottleCapApi.Models
{
  public class Player
  {
    public int Id { get; set; }

    public string SlackName { get; set; }

    public string Name { get; set; }

    public int BottleCaps { get; set; } = 0;

    public int GameId { get; set; }

    public Game Game { get; set; }

  }
}