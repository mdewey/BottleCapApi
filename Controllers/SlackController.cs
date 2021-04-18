using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BottleCapApi.Models;
using BottleCapApi.Slack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BottleCapApi.Controllers
{
  [Route("slack")]
  [ApiController]
  public class SlackController : ControllerBase
  {
    private readonly DatabaseContext _context;


    private readonly ResponseFactory _responseFactory;

    public SlackController(DatabaseContext context, ResponseFactory responseFactory)
    {
      _context = context;
      this._responseFactory = responseFactory;
    }

    // register game
    [HttpPost("register")]
    public async Task<ActionResult> RegisterGame([FromForm] SlackRequest data)
    {
      Console.WriteLine(data);
      var (team_id, channel_id, channel_name) = data;

      // TODO: check DMs
      // check if game already exists
      var existingGame = _context
        .Games
        .FirstOrDefault(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame != null)
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Hold up! {channel_name} has already been created"));
      }
      else
      {
        // create a new game
        var game = new Game
        {
          ChannelName = channel_name,
          SlackId = channel_id,
          TeamId = team_id
        };

        // save it
        _context.Add(game);
        await _context.SaveChangesAsync();
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Success! {channel_name} has been created"));

      }

    }

    [HttpPost("give/bottlecaps")]
    public async Task<ActionResult> GiveBottleCap(string channel_id, string channel_name, string text, string team_id)
    {
      // validate
      if (!(text.First() == '<' && text.Last() == '>'))
      {
        var response = new Response
        {
          blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Thats not a real player!"
                    }
                }
            }
        };
        return Ok(new
        {
          blocks = response.blocks
        });
      }

      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(g => g.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
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
        // START HERE:
        // get username
        var userName = text.Trim();
        // update/create player
        var player = existingGame.Players.FirstOrDefault(f => f.SlackId == userName);
        if (player == null)
        {
          var newPlayer = new Player
          {
            SlackId = userName,
            GameId = existingGame.Id,
            BottleCaps = 1,
          };
          _context.Players.Add(newPlayer);
          await _context.SaveChangesAsync();
        }
        else
        {
          player.BottleCaps++;
          await _context.SaveChangesAsync();
        }
        // save player

        var response = new Response
        {
          blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Success! Bottle cap for {text}!"
                    }
                }
            }
        };
        return Ok(new
        {
          blocks = response.blocks
        });
      };
    }

    [HttpPost("use/bottlecaps")]
    public async Task<ActionResult> UseBottleCap(string channel_id, string channel_name, string text, string team_id)
    {
      // validate
      if (!(text.First() == '<' && text.Last() == '>'))
      {
        var response = new Response
        {
          blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Thats not a real player!"
                    }
                }
            }
        };
        return Ok(new
        {
          blocks = response.blocks
        });
      }

      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(g => g.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
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
        // START HERE:
        // get username
        var userName = text.Trim();
        // update/create player
        var player = existingGame.Players.FirstOrDefault(f => f.SlackId == userName);
        if (player == null)
        {

          var response = new Response
          {
            blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Whomp! {text} does not have any bottle caps!"
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
        else
        {
          if (player.BottleCaps <= 0)
          {
            var resp = new Response
            {
              blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Whomp! {text} does not have any bottle caps!"
                    }
                }
            }
            };
            return Ok(new
            {
              existingGame,
              blocks = resp.blocks
            });
          }
          else
          {
            player.BottleCaps--;
            await _context.SaveChangesAsync();

            var response = new Response
            {
              blocks = new List<Block>{
                new Block{
                    type ="section",
                    text = new Text{
                        type="mrkdwn",
                        text=$"Ca-ching! Bottle cap for {text} has been cashed in!"
                    }
                }
            }
            };
            return Ok(new
            {
              blocks = response.blocks
            });
          }

        }
      };
    }


    [HttpPost("get/bottlecaps")]
    public async Task<ActionResult> GetBottleCaps(string channel_id, string channel_name, string team_id)
    {
      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(i => i.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
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
                        text= MarkdownFactory.CreateTable(existingGame.Players)
                    }
                }
            }
        };
        return Ok(new
        {
          blocks = response.blocks
        });
      }
    }


    // COMMAND: create GM 

    // COMMAND: Add player to as DM 


  }
}