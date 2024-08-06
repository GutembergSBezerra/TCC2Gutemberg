using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class TempModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly OracleDbContext _context;

        public TempModel(IConfiguration configuration, OracleDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile UploadedFile { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;
        public List<Tbl_FornecedorDocumentos> UploadedFiles { get; set; }

        public IActionResult OnGet()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            FetchUploadedFiles();

            return Page();
        }

        private void FetchUploadedFiles()
        {
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;
            if (cnpjClaim != null)
            {
                UploadedFiles = _context.Tbl_FornecedorDocumentos
                    .Where(d => d.CNPJ == cnpjClaim)
                    .ToList();
            }
            else
            {
                UploadedFiles = new List<Tbl_FornecedorDocumentos>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                FetchUploadedFiles();
                return Page();
            }

            if (UploadedFile == null)
            {
                ErrorMessage = "No file selected.";
                FetchUploadedFiles();
                return Page();
            }

            if (UploadedFile.ContentType != "application/pdf")
            {
                ErrorMessage = "Only PDF files are allowed.";
                FetchUploadedFiles();
                return Page();
            }

            if (UploadedFile.Length > 5 * 1024 * 1024)
            {
                ErrorMessage = "File size must be less than 5MB.";
                FetchUploadedFiles();
                return Page();
            }

            try
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, UploadedFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(stream);
                }

                var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;
                if (cnpjClaim == null)
                {
                    ErrorMessage = "CNPJ not found.";
                    FetchUploadedFiles();
                    return Page();
                }

                var documento = new Tbl_FornecedorDocumentos
                {
                    CNPJ = cnpjClaim,
                    NOMEARQUIVO = UploadedFile.FileName,
                    CAMINHOARQUIVO = filePath,
                    TIPODOCUMENTO = null,
                    HORARIOUPLOAD = DateTimeOffset.UtcNow
                };

                _context.Tbl_FornecedorDocumentos.Add(documento);
                await _context.SaveChangesAsync();

                SuccessMessage = "File uploaded and saved successfully!";
            }
            catch (DbUpdateException ex)
            {
                ErrorMessage = $"Error uploading file: {ex.Message}";
                if (ex.InnerException != null)
                {
                    ErrorMessage += $" Inner exception: {ex.InnerException.Message}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error uploading file: {ex.Message}";
            }

            FetchUploadedFiles();
            return Page();
        }

        public IActionResult OnGetDownload(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                var bytes = System.IO.File.ReadAllBytes(filePath);
                return File(bytes, "application/pdf", Path.GetFileName(filePath));
            }

            ErrorMessage = "File not found.";
            FetchUploadedFiles();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var documento = await _context.Tbl_FornecedorDocumentos.FindAsync(id);
            if (documento == null)
            {
                ErrorMessage = "File not found.";
                FetchUploadedFiles();
                return Page();
            }

            // Delete the file from the file system
            if (System.IO.File.Exists(documento.CAMINHOARQUIVO))
            {
                System.IO.File.Delete(documento.CAMINHOARQUIVO);
            }

            // Remove the entry from the database
            _context.Tbl_FornecedorDocumentos.Remove(documento);
            await _context.SaveChangesAsync();

            SuccessMessage = "File deleted successfully!";
            FetchUploadedFiles();
            return Page();
        }
    }
}