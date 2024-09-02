using System;
using System.Collections.Generic;
using System.IO;
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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagePath;
        private readonly string _defaultImageName = "Default.png";

        public ClientesController(AppDbContext context)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clientes>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clientes>> GetClientes(long id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientes(long id, [FromForm] Clientes clientes, IFormFile? foto)
        {
            if (id != clientes.Id)
            {
                return BadRequest("El ID del cliente no coincide.");
            }

            var existingClient = await _context.Clientes.FindAsync(id);
            if (existingClient == null)
            {
                return NotFound();
            }

            existingClient.Nombre = clientes.Nombre;
            existingClient.Telefono = clientes.Telefono;
            existingClient.Nacimiento = clientes.Nacimiento;

            if (foto != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var filePath = Path.Combine(_imagePath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingClient.Foto) && existingClient.Foto != _defaultImageName)
                {
                    var oldFilePath = Path.Combine(_imagePath, existingClient.Foto);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existingClient.Foto = newFileName;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientesExists(id))
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

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Clientes>> PostClientes([FromForm] Clientes cliente, IFormFile? foto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (foto != null)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(foto.FileName);
                var filePath = Path.Combine(_imagePath, newFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await foto.CopyToAsync(stream);
                }

                cliente.Foto = newFileName;
            }
            else
            {
                var defaultImagePath = Path.Combine(_imagePath, _defaultImageName);
                if (!System.IO.File.Exists(defaultImagePath))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "La imagen predeterminada no existe." });
                }

                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(defaultImagePath);
                var newFilePath = Path.Combine(_imagePath, newFileName);
                System.IO.File.Copy(defaultImagePath, newFilePath);

                cliente.Foto = newFileName;
            }

            _context.Clientes.Add(cliente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al agregar el cliente: " + ex.Message });
            }

            return CreatedAtAction(nameof(GetClientes), new { id = cliente.Id }, cliente);
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientes(long id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var filePath = Path.Combine(_imagePath, cliente.Foto);
            if (System.IO.File.Exists(filePath) && cliente.Foto != _defaultImageName)
            {
                System.IO.File.Delete(filePath);
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientesExists(long id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
