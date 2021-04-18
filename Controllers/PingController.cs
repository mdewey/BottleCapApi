using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BottleCapApi.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BottleCapApi.Models;

namespace BottleCapApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PingController : ControllerBase
  {

    private readonly ResponseFactory _responseFactory;

    public PingController(ResponseFactory responseFactory)
    {
      this._responseFactory = responseFactory;
    }

    [HttpGet]
    public ActionResult Ping()
    {
      return Ok(new { Ping = "Pong", When = DateTime.UtcNow });
    }

    // register game
    [HttpPost("slack")]
    public async Task<ActionResult> RegisterGame(string channel_id, string channel_name, string enterprise_id)
    {
      return Ok(this._responseFactory.CreateSimpleChannelMessage($"Good Morning! server time is {DateTime.UtcNow}"));
    }



  }
}