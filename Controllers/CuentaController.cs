//Controllers/CuentaControllers.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BancoNet.Models;
using BancoNet.Dtos;

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
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetCuentas()
        {
            var cuentas = await _context.Cuentas.Include(c => c.Cliente).ToListAsync();

            var cuentasDto = cuentas.Select(cuenta => new CuentaDto
            {
                No_Cuenta = cuenta.No_Cuenta,
                Tipo = cuenta.Tipo,
                Saldo = cuenta.Saldo,
                Beneficiarios = cuenta.Beneficiarios,
                ClienteId = cuenta.ClienteId
            }).ToList();

            return cuentasDto;
        }

        // GET: api/Cuenta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaDto>> GetCuenta(long id)
        {
            var cuenta = await _context.Cuentas.Include(c => c.Cliente)
                                               .FirstOrDefaultAsync(c => c.No_Cuenta == id);

            if (cuenta == null)
            {
                return NotFound();
            }

            var cuentaDto = new CuentaDto
            {
                No_Cuenta = cuenta.No_Cuenta,
                Tipo = cuenta.Tipo,
                Saldo = cuenta.Saldo,
                Beneficiarios = cuenta.Beneficiarios,
                ClienteId = cuenta.ClienteId
            };

            return cuentaDto;
        }

        // PUT: api/Cuenta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuenta(long id, CuentaDto cuentaDto)
        {
            if (id != cuentaDto.No_Cuenta)
            {
                return BadRequest("El ID de la cuenta no coincide con el proporcionado.");
            }

            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }

            cuenta.Tipo = cuentaDto.Tipo;
            cuenta.Saldo = cuentaDto.Saldo;
            cuenta.Beneficiarios = cuentaDto.Beneficiarios;
            cuenta.ClienteId = cuentaDto.ClienteId;

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
        public async Task<ActionResult<CuentaDto>> PostCuenta(CuentaDto cuentaDto)
        {
            if (cuentaDto == null)
            {
                return BadRequest("La cuenta no puede ser nula.");
            }

            // Verificar si el cliente ya tiene una cuenta del mismo tipo
            var cuentaExistente = await _context.Cuentas
                                                .FirstOrDefaultAsync(c => c.ClienteId == cuentaDto.ClienteId 
                                                                    && c.Tipo == cuentaDto.Tipo);
            if (cuentaExistente != null)
            {
                return Conflict("El cliente ya tiene una cuenta de este tipo."); // Retornar error 409 (Conflicto)
            }

            var cliente = await _context.Clientes.FindAsync(cuentaDto.ClienteId);
            if (cliente == null)
            {
                return BadRequest("El cliente no existe.");
            }

            var cuenta = new Cuenta
            {
                Tipo = cuentaDto.Tipo,
                Saldo = cuentaDto.Saldo,
                Beneficiarios = cuentaDto.Beneficiarios,
                ClienteId = cuentaDto.ClienteId
            };

            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();

            cuentaDto.No_Cuenta = cuenta.No_Cuenta;

            return CreatedAtAction(nameof(GetCuenta), new { id = cuenta.No_Cuenta }, cuentaDto);
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

        [HttpGet("Clientes")]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            var clientesDto = clientes.Select(cliente => new ClienteDto
            {
                Id = cliente.Id,
                Nombre = cliente.Nombre,
                Foto = cliente.Foto
            }).ToList();

            return clientesDto;
        }
    }
}