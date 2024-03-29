using System.Collections.Generic;

namespace BottleCapApi.Slack
{
  public class Text
  {
    public string type { get; set; }
    public string text { get; set; }
    public bool emoji { get; set; } = true;
  }

  public class Block
  {
    public string type { get; set; }
    public Text text { get; set; }
    public List<Text> fields { get; set; }


  }

  public class Response
  {
    public List<Block> blocks { get; set; }
  }


}