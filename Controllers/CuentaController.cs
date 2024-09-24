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
                return BadRequest();
            }

            _context.Entry(cuenta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentaExists(id))
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

        // POST: api/Cuenta
        [HttpPost]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            // Buscar al cliente relacionado por ClienteId
            var cliente = await _context.Clientes.FindAsync(cuenta.ClienteId);

            // Verificar si el cliente existe
            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }

            // Relacionar la cuenta con el cliente
            cuenta.Cliente = cliente;

            // Agregar la cuenta a la base de datos
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCuenta", new { id = cuenta.No_Cuenta }, cuenta);
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

