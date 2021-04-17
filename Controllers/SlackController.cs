using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BottleCapApi.Models;
using BottleCapApi.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    // see bottle caps
    [HttpPost("get/bottlecaps")]
    public async Task<ActionResult> GetBottleCaps(string channel_id, string channel_name, string enterprise_id)
    {
      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(i => i.Players)
        .FirstOrDefaultAsync(a => a.EnterpriseId == enterprise_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        var response = new Response
        {
          blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                          text=$"Welp! {channel_name} is not a game! Create a game first!"
                    }
                }
            }
        };
        return Ok(new { blocks = response.blocks });
      }
      else
      {
        var players = new List<Player>{
            new Player{
                BottleCaps= new Random().Next(0,10),
                SlackId = "Tim  1"
            },
            new Player{
                BottleCaps= new Random().Next(0,10),
                SlackId = "Tim  2"
            },
            new Player{
                BottleCaps= new Random().Next(0,10),
                SlackId = "Tim  3"
            }
        };
        var response = new
        {
          blocks = new List<Object>{
                new {
                    type = "header",
                    text= new {
                        type= "plain_text",
                        text= $":bottle-cap: Bottle caps for {channel_name} :bottle-cap:",
                        emoji= true
                    }
                },
                new {
                    type = "divider",
                },
                new {
                    type = "section",
                    text = new {
                        type= "mrkdwn",
                        text= MarkdownFactory.CreateTable(players)
                    }
                }
            }
        };
        return Ok(new
        {
          existingGame,
          blocks = response.blocks
        });
      }

    }




    // create GM 

    // Add player to as DM 


  }
}