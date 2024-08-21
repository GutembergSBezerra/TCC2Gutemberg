using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace PortalArcomix.Pages
{
    public class FornecedorExtVEModel : PageModel
    {
        private readonly OracleDbContext _context;

        public FornecedorExtVEModel(OracleDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile? UploadedFile { get; set; }

        [BindProperty]
        public string? TIPODOCUMENTO { get; set; }

        public List<Tbl_FornecedorDocumentos> UploadedFiles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return NotFound();
            }

            UploadedFiles = await _context.Tbl_FornecedorDocumentos
                                          .Where(d => d.CNPJ == cnpjClaim)
                                          .ToListAsync();

            return Page();
        }

        public IActionResult OnGetDownload(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(filePath);
            var mimeType = "application/pdf"; // Assuming all files are PDFs; adjust as needed

            return PhysicalFile(filePath, mimeType, fileName);
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return BadRequest();
            }

            if (UploadedFile != null)
            {
                var fileName = Path.GetFileName(UploadedFile.FileName);
                var baseUploadPath = Path.Combine("C:\\Users\\Gutemberg\\source\\repos\\PortalArcomix\\wwwroot\\uploads", cnpjClaim, "Documentos");

                Directory.CreateDirectory(baseUploadPath);
                var filePath = Path.Combine(baseUploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(fileStream);
                }

                var document = new Tbl_FornecedorDocumentos
                {
                    CNPJ = cnpjClaim,
                    NOMEARQUIVO = fileName,
                    TIPODOCUMENTO = TIPODOCUMENTO,
                    CAMINHOARQUIVO = filePath,
                    HORARIOUPLOAD = DateTimeOffset.Now
                };

                _context.Tbl_FornecedorDocumentos.Add(document);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var document = await _context.Tbl_FornecedorDocumentos.FindAsync(id);

            if (document == null)
            {
                return NotFound();
            }

            var filePath = document.CAMINHOARQUIVO;

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Tbl_FornecedorDocumentos.Remove(document);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}