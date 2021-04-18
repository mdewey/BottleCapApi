using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BottleCapApi.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentLifeTracker.Models;

namespace BottleCapApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PingController : ControllerBase
  {
    [HttpGet]
    public ActionResult Ping()
    {
      return Ok(new { Ping = "Pong", When = DateTime.UtcNow });
    }

    // register game
    [HttpPost("slack")]
    public async Task<ActionResult> RegisterGame(string channel_id, string channel_name, string enterprise_id)
    {
      var blocks = new[] { new { type = "section", text = new { type = "mrkdwn", text = $"Good Morning! server time is {DateTime.UtcNow}" } } };
      return Ok(new { blocks, response_type = "in_channel" });
    }



  }
}