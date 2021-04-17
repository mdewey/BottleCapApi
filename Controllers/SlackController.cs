using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BottleCapApi.Models;
using BottleCapApi.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentLifeTracker.Models;

namespace BottleCapApi.Controllers
{
  [Route("slack")]
  [ApiController]
  public class SlackController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public SlackController(DatabaseContext context)
    {
      _context = context;
    }


    // register game
    [HttpPost("register")]
    public async Task<ActionResult> RegisterGame(string channel_id, string channel_name, string enterprise_id)
    {

      // TODO: check DMs

      // check if game already exists
      var existingGame = _context
        .Games
        .FirstOrDefault(a => a.EnterpriseId == enterprise_id && a.SlackId == channel_id);
      if (existingGame != null)
      {
        var response = new Response
        {
          blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                          text=$"Hold up! {channel_name} has already been created"
                    }
                }
            }
        };
        return Ok(new { blocks = response.blocks });
      }
      else
      {
        // create a new game
        var game = new Game
        {
          ChannelName = channel_name,
          SlackId = channel_id,
          EnterpriseId = enterprise_id
        };

        // save it
        _context.Add(game);
        await _context.SaveChangesAsync();
        var response = new Response
        {
          blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Success! {channel_name} has been created"
                    }
                }
            }
        };
        return Ok(new { blocks = response.blocks });
      }

    }
    // give bottle cap
    // use bottle cap 

    // create GM 
    // Add player to as DM 


  }
}