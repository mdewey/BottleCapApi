using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BottleCapApi.Models;
using StudentLifeTracker.Models;

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

  }
}
