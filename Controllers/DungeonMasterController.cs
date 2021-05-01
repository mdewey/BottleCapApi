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
    public class DungeonMasterController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public DungeonMasterController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/DungeonMaster
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DungeonMaster>>> GetDungeonMasters()
        {
            return await _context.DungeonMasters.ToListAsync();
        }

        // GET: api/DungeonMaster/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DungeonMaster>> GetDungeonMaster(int id)
        {
            var dungeonMaster = await _context.DungeonMasters.FindAsync(id);

            if (dungeonMaster == null)
            {
                return NotFound();
            }

            return dungeonMaster;
        }

        // PUT: api/DungeonMaster/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDungeonMaster(int id, DungeonMaster dungeonMaster)
        {
            if (id != dungeonMaster.Id)
            {
                return BadRequest();
            }

            _context.Entry(dungeonMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DungeonMasterExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DungeonMaster
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DungeonMaster>> PostDungeonMaster(DungeonMaster dungeonMaster)
        {
            _context.DungeonMasters.Add(dungeonMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDungeonMaster", new { id = dungeonMaster.Id }, dungeonMaster);
        }

        // DELETE: api/DungeonMaster/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDungeonMaster(int id)
        {
            var dungeonMaster = await _context.DungeonMasters.FindAsync(id);
            if (dungeonMaster == null)
            {
                return NotFound();
            }

            _context.DungeonMasters.Remove(dungeonMaster);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DungeonMasterExists(int id)
        {
            return _context.DungeonMasters.Any(e => e.Id == id);
        }
    }
}
