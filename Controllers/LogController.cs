using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BottleCapApi.Models;


namespace BottleCapApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LogController : ControllerBase
  {
    private readonly DatabaseContext _context;

    public LogController(DatabaseContext context)
    {
      _context = context;
    }

    // GET: api/Log
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
    {
      return await _context.Logs.OrderByDescending(o => o.When).ToListAsync();
    }


    [HttpDelete("all")]
    public async Task<ActionResult> ClearAll()
    {
      var all = _context.Logs;
      _context.RemoveRange(all);
      await _context.SaveChangesAsync();
      return Ok();
    }

    [HttpDelete("week")]
    public async Task<ActionResult> DeleteOlderThanAWeek()
    {
      var compareDate = DateTime.Now.AddDays(-5);
      var old = _context.Logs.Where(w => w.When < compareDate);
      _context.RemoveRange(old);
      await _context.SaveChangesAsync();
      return Ok();
    }

  }
}
