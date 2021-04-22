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
      var (team_id, channel_id, channel_name, _) = data;

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
    public async Task<ActionResult> GiveBottleCap([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, text) = data;
      // validate
      if (!(text.First() == '<' && text.Last() == '>'))
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Thats not a real player! = {text}", false));
      }

      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(g => g.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Welp! {channel_name} is not a game! Create a game first!", false));
      }
      else
      {
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
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Success! Bottle cap for {text}!"));
      };
    }

    [HttpPost("use/bottlecaps")]
    public async Task<ActionResult> UseBottleCap([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, text) = data;
      // validate
      if (!(text.First() == '<' && text.Last() == '>'))
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Thats not a real player! = {text}", false));
      }

      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(g => g.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Welp! {channel_name} is not a game! Create a game first!", false));
      }
      else
      {
        // get username
        var userName = text.Trim();
        // update/create player
        var player = existingGame.Players.FirstOrDefault(f => f.SlackId == userName);
        if (player == null)
        {
          return Ok(this._responseFactory.CreateSimpleChannelMessage($"Whomp! {text} does not have any bottle caps!"));
        }
        else
        {
          if (player.BottleCaps <= 0)
          {
            return Ok(this._responseFactory.CreateSimpleChannelMessage($"Whomp! {text} has run out of bottle caps!"));
          }
          else
          {
            player.BottleCaps--;
            await _context.SaveChangesAsync();
            return Ok(this._responseFactory.CreateSimpleChannelMessage($"Ca-ching! Bottle cap for {text} has been cashed in!"));
          }
        }
      };
    }

    [HttpPost("get/bottlecaps")]
    public async Task<ActionResult> GetBottleCaps([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, _) = data;
      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(i => i.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Welp! {channel_name} is not a game! Create a game first!", false));
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
                        text= $":d20-yeah: Bottle caps for {channel_name} :d20-yeah:",
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
          blocks = response.blocks,
          response_type = "in_channel"
        });
      }
    }

  }
}