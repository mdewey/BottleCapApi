namespace BottleCapApi.Slack
{
  public class ResponseFactory
  {
    public object CreateSimpleChannelMessage(string text, bool inChannel = true)
    {
      var blocks = new[] { new { type = "section", text = new { type = "mrkdwn", text } } };

      return new { blocks, response_type = inChannel ? "in_channel" : "ephemeral" };
    }
  }
}