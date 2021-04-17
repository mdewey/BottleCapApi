using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

  }
}