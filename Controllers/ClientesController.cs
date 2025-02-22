using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BancoNet.Models;
using OfficeOpenXml;

namespace BancoNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _imagePath;
        private readonly string _trashPath;
        private readonly string _defaultImageName = "Default.png";

        public ClientesController(AppDbContext context)
        {
            _context = context;
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
            _trashPath = Path.Combine(_imagePath, "Trash");

            if (!Directory.Exists(_trashPath))
            {
                Directory.CreateDirectory(_trashPath);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clientes>>> GetClientes()
        {
            return await _context.Clientes.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Clientes>> GetClientes(long id)
        {
            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

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
                    MoveImageToTrash(existingClient.Foto);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientes(long id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(cliente.Foto) && cliente.Foto != _defaultImageName)
            {
                MoveImageToTrash(cliente.Foto);
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientesExists(long id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private void MoveImageToTrash(string fileName)
        {
            var oldFilePath = Path.Combine(_imagePath, fileName);
            var trashFilePath = Path.Combine(_trashPath, fileName);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Move(oldFilePath, trashFilePath);
            }
        }
        [HttpGet("exportar-clientes")]
        public async Task<IActionResult> ExportarClientes()
        {
            var clientes = await _context.Clientes.AsNoTracking().ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Clientes");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Nombre";
                worksheet.Cells[1, 3].Value = "Teléfono";
                worksheet.Cells[1, 4].Value = "Fecha de Nacimiento";
                worksheet.Cells[1, 5].Value = "Edad";

                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 10;

                worksheet.Cells[1, 1, 1, 5].Style.Font.Bold = true;

                for (int i = 0; i < clientes.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = clientes[i].Id;
                    worksheet.Cells[i + 2, 2].Value = clientes[i].Nombre ?? "N/A";
                    worksheet.Cells[i + 2, 3].Value = clientes[i].Telefono?.ToString() ?? "N/A";
                    worksheet.Cells[i + 2, 4].Value = clientes[i].Nacimiento; // Asignar como DateTime

                    // Formatear la celda para mostrar solo la fecha
                    worksheet.Cells[i + 2, 4].Style.Numberformat.Format = "dd-MM-yyyy"; // Cambia el formato aquí
                    
                    var edad = DateTime.Now.Year - clientes[i].Nacimiento.Year;
                    worksheet.Cells[i + 2, 5].Value = edad;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Clientes.xlsx";
                var mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(stream, mimeType, fileName);
            }
        }
    }
}
