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
      var (team_id, channel_id, channel_name, _, _, _) = data;

      // TODO: check DMs
      // check if game already exists
      var existingGame = _context
        .Games
        .FirstOrDefault(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame != null)
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Hold up! {existingGame.ChannelName} has already been created"));
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
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Success! {existingGame.ChannelName} has been created"));

      }

    }


    [HttpPost("update/name")]
    public async Task<ActionResult> UpdateDisplayName([FromForm] SlackRequest data)
    {
      Console.WriteLine(data);
      var (team_id, channel_id, channel_name, text, _, _) = data;
      // TODO: add DM check
      // check if game already exists
      var existingGame = _context
        .Games
        .FirstOrDefault(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.GameNotFoundMessage(channel_name));
      }
      else
      {
        existingGame.ChannelName = text;
        await _context.SaveChangesAsync();
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Success! {channel_name} has been renamed to {existingGame.ChannelName}", false));

      }
    }

    // register game
    [HttpPost("claim/dm")]
    public async Task<ActionResult> ClaimDm([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, _, user_id, user_name) = data;

      // get the game
      // check if game already exists
      var existingGame = _context
        .Games
        .Include(i => i.DungeonMasters)
        .FirstOrDefault(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.GameNotFoundMessage(channel_name));
      }
      else
      {
        if (existingGame.DungeonMasters.Count() > 0)
        {
          return Ok(this._responseFactory.CreateSimpleChannelMessage($"Muntiny is it?! {existingGame.ChannelName} has already has a DM."));
        }
        else
        {
          var dm = new DungeonMaster
          {
            SlackId = user_id,
            SlackName = user_name,
            GameId = existingGame.Id
          };

          this._context.DungeonMasters.Add(dm);
          await this._context.SaveChangesAsync();

          return Ok(this._responseFactory.CreateSimpleChannelMessage($"Claimed! {existingGame.ChannelName} now belongs to <@{dm.SlackId}|{dm.SlackName}>", false));
        }
      }
    }

    [HttpPost("give/bottlecaps")]
    public async Task<ActionResult> GiveBottleCap([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, text, user_id, user_name) = data;
      // validate
      if (!(text.First() == '<' && text.Last() == '>'))
      {
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Thats not a real player! = {text}", false));
      }

      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(g => g.Players)
        .Include(i => i.DungeonMasters)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.GameNotFoundMessage(channel_name));
      }
      else
      {
        // DM Check
        if (existingGame.DungeonMasters.Any(a => a.SlackId != user_id))
        {
          return Ok(this._responseFactory.CreateSimpleChannelMessage($"Sneaky Bastard! You are not the DM of {existingGame.ChannelName}. Too many bottle caps can crash the economy!", false));
        }
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
        }
        else
        {
          player.BottleCaps++;
        }
        await _context.SaveChangesAsync();
        return Ok(this._responseFactory.CreateSimpleChannelMessage($"Success! Bottle cap for {text}!"));
      };
    }

    [HttpPost("use/bottlecaps")]
    public async Task<ActionResult> UseBottleCap([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, text, user_id, user_name) = data;

      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(g => g.Players)
        .Include(i => i.DungeonMasters)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.GameNotFoundMessage(channel_name));
      }


      // if no text (then the user is using their own cap)
      if (String.IsNullOrWhiteSpace(text))
      {
        // update/create player
        var userId = $"<@{user_id}|{user_name}>";
        var player = existingGame.Players.FirstOrDefault(f => f.SlackId == userId);
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
            return Ok(this._responseFactory.CreateSimpleChannelMessage($"Ca-ching! Bottle cap for {userId} has been cashed in!"));
          }
        }
      }
      // if text (this is the DM giving bottlecaps)
      else
      {
        // DM Check
        if (existingGame.DungeonMasters.Any(a => a.SlackId != user_id))
        {
          return Ok(this._responseFactory.CreateSimpleChannelMessage($"! You are not the DM of {existingGame.ChannelName}", false));
        }
        // validate
        if (!(text.First() == '<' && text.Last() == '>'))
        {
          return Ok(this._responseFactory.CreateSimpleChannelMessage($"Thats not a real player! = {text}", false));
        }
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

      }




    }

    [HttpPost("get/bottlecaps")]
    public async Task<ActionResult> GetBottleCaps([FromForm] SlackRequest data)
    {
      var (team_id, channel_id, channel_name, _, _, _) = data;
      // get all players for a game
      var existingGame = await _context
        .Games
        .Include(i => i.Players)
        .FirstOrDefaultAsync(a => a.TeamId == team_id && a.SlackId == channel_id);
      if (existingGame == null)
      {
        return Ok(this._responseFactory.GameNotFoundMessage(channel_name));
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
                        text= $":spinning-coin: Bottle caps for {existingGame.ChannelName} :spinning-coin:",
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