using System;

namespace BottleCapApi.Slack
{
  public class SlackRequest
  {
    public string channel_id { get; set; }
    public string channel_name { get; set; }

    public string team_id { get; set; }


    public override string ToString()
    {
      return $"channel id: {this.channel_id} | channel name: {this.channel_name} | team id: {this.team_id}";
    }

    internal void Deconstruct(out string team_id, out string channel_id, out string channel_name)
    {
      team_id = this.team_id;
      channel_id = this.channel_id;
      channel_name = this.channel_name;
    }
  }
}