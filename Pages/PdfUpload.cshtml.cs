using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TCC2Gutemberg.Pages
{
    public class PdfUploadModel : PageModel
    {
        private readonly OracleDbContext _context;

        public PdfUploadModel(OracleDbContext context)
        {
            _context = context;
        }

        // List of companies to display in the dropdown
        public List<Tbl_Empresa> Empresas { get; set; } = new List<Tbl_Empresa>();

        [BindProperty]
        public IFormFile PdfFile { get; set; }

        [BindProperty]
        public int SelectedCompany { get; set; }  // To store the selected company

        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public async Task OnGetAsync()
        {
            // Load all available companies for the dropdown
            Empresas = await _context.Tbl_Empresa.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (PdfFile == null || PdfFile.Length == 0 || SelectedCompany == 0)
            {
                Message = "Por favor, selecione uma empresa e faça o upload de um arquivo PDF.";
                IsSuccess = false;
                Empresas = await _context.Tbl_Empresa.ToListAsync();  // Reload the company list
                return Page();
            }

            // Define the specific upload path
            var uploadsFolder = @"C:\Users\Gutemberg\PortalArcomix\wwwroot\uploads";
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder); // Create directory if it doesn't exist
            }

            // Define the full path for the file (consider security, prevent file name collision)
            var sanitizedFileName = Path.GetFileNameWithoutExtension(PdfFile.FileName).Replace(" ", "_") + Path.GetExtension(PdfFile.FileName);
            var filePath = Path.Combine(uploadsFolder, sanitizedFileName);

            // Ensure unique filename (in case of collision)
            int count = 1;
            while (System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(uploadsFolder, Path.GetFileNameWithoutExtension(sanitizedFileName) + $"({count++})" + Path.GetExtension(sanitizedFileName));
            }

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await PdfFile.CopyToAsync(stream);
            }

            // Save the file info and related company to the database
            var pdfEntry = new Tbl_FatoRelevante
            {
                ID_Empresa = SelectedCompany,
                NomeArquivo = sanitizedFileName,
                CaminhoArquivo = filePath,
                DataUpload = DateTime.Now
            };

            _context.Tbl_FatoRelevante.Add(pdfEntry);
            await _context.SaveChangesAsync();

            // Return success message
            Message = $"Arquivo {PdfFile.FileName} enviado com sucesso para a empresa selecionada!";
            IsSuccess = true;
            return RedirectToPage("/PdfUpload");  // Optionally redirect after successful upload
        }
    }
}