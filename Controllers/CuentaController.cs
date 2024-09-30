using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BancoNet.Models;

namespace BancoNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Cuenta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas()
        {
            return await _context.Cuentas.Include(c => c.Cliente).ToListAsync();
        }

        // GET: api/Cuenta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(long id)
        {
            var cuenta = await _context.Cuentas.Include(c => c.Cliente)
                                               .FirstOrDefaultAsync(c => c.No_Cuenta == id);

            if (cuenta == null)
            {
                return NotFound();
            }

            return cuenta;
        }

        // PUT: api/Cuenta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuenta(long id, Cuenta cuenta)
        {
            if (id != cuenta.No_Cuenta)
            {
                return BadRequest("El ID de la cuenta no coincide con el proporcionado.");
            }

            if (cuenta == null)
            {
                return BadRequest("La cuenta no puede ser nula.");
            }

            _context.Entry(cuenta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var existingCuenta = await _context.Cuentas.FindAsync(id);
                if (existingCuenta == null)
                {
                    return NotFound("La cuenta ha sido eliminada por otro usuario.");
                }
                else
                {
                    return Conflict("La cuenta ha sido modificada por otro usuario. Por favor, actualiza y vuelve a intentar.");
                }
            }

            return NoContent();
        }

        // POST: api/Cuenta
        [HttpPost]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            if (cuenta == null)
            {
                return BadRequest("La cuenta no puede ser nula.");
            }

            var cliente = await _context.Clientes.FindAsync(cuenta.ClienteId);

            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }
            cuenta.Cliente = cliente;

            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.No_Cuenta }, cuenta);
        }

        // DELETE: api/Cuenta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuenta(long id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }

            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CuentaExists(long id)
        {
            return _context.Cuentas.Any(e => e.No_Cuenta == id);
        }
    }
}
