using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using static PortalArcomix.Pages.FornecedorExtVEModel;

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
        public string COMENTARIO { get; set; }

        [BindProperty]
        public string? TIPODOCUMENTO { get; set; }

        public List<Tbl_FornecedorDocumentos> UploadedFiles { get; set; }
        public List<Tbl_FornecedorComentarios> Comentarios { get; set; } // List to hold existing comments
        public List<ComentarioViewModel> ComentariosViewModel { get; set; } // List to hold transformed comments with user info


        public bool IsSintegraUploaded { get; set; }
        public bool IsDocumentacaoSanitariaUploaded { get; set; }
        public bool IsDocumentacaoAmbientalUploaded { get; set; }
        public bool IsDocumentacaoControlePragasUploaded { get; set; }
        public bool IsDocumentacaoControleAguaUploaded { get; set; }

        public bool IsAlimentosNaoIndustrializados { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return NotFound();
            }

            // Load Fornecedor data
            var fornecedor = await _context.Tbl_Fornecedor.FirstOrDefaultAsync(f => f.CNPJ == cnpjClaim);
            if (fornecedor == null)
            {
                return NotFound();
            }

            IsAlimentosNaoIndustrializados = fornecedor.FORNECEDORALIMENTOS == "Alimentos Não Industrializados";

            // Load uploaded files
            UploadedFiles = await _context.Tbl_FornecedorDocumentos.Where(d => d.CNPJ == cnpjClaim).ToListAsync();

            // Load existing comments with user info
            var comentarios = await _context.Tbl_FornecedorComentarios
                .Where(c => c.CNPJ == cnpjClaim)
                .Join(
                    _context.Tbl_Usuario,
                    comentario => comentario.ID_USUARIO,
                    usuario => usuario.ID_Usuario,
                    (comentario, usuario) => new ComentarioViewModel
                    {
                        ID = comentario.ID,
                        CNPJ = comentario.CNPJ,
                        COMENTARIO = comentario.COMENTARIO,
                        DATACOMENTARIO = comentario.DATACOMENTARIO,
                        UsuarioNome = usuario.Usuario,
                        UsuarioTipo = usuario.TipoUsuario
                    }
                ).ToListAsync();

            ComentariosViewModel = comentarios;

            // Check if SINTEGRA is uploaded
            IsSintegraUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "SINTEGRA");

            // Check additional document types if necessary
            if (IsAlimentosNaoIndustrializados)
            {
                IsDocumentacaoSanitariaUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Sanitária");
                IsDocumentacaoAmbientalUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Ambiental ou Operacional");
                IsDocumentacaoControlePragasUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Controle de Pragas");
                IsDocumentacaoControleAguaUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Controle de Água");
            }

            return Page();
        }


        public async Task<IActionResult> OnPostUploadAsync()
        {
            // Don't bind COMENTARIO here to avoid validation issues
            ModelState.Remove("COMENTARIO");

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim == null)
            {
                return BadRequest();
            }

            if (UploadedFile != null)
            {
                // Check if the document type has already been uploaded
                bool documentAlreadyUploaded = await _context.Tbl_FornecedorDocumentos
                    .AnyAsync(d => d.CNPJ == cnpjClaim && d.TIPODOCUMENTO == TIPODOCUMENTO);

                if (documentAlreadyUploaded)
                {
                    ModelState.AddModelError(string.Empty, $"O documento '{TIPODOCUMENTO}' já foi enviado.");
                    await LoadDataAsync();
                    return Page();
                }

                // Check if the file name has already been uploaded for this CNPJ
                var fileName = Path.GetFileName(UploadedFile.FileName);
                bool fileNameAlreadyExists = await _context.Tbl_FornecedorDocumentos
                    .AnyAsync(d => d.CNPJ == cnpjClaim && d.NOMEARQUIVO == fileName);

                if (fileNameAlreadyExists)
                {
                    ModelState.AddModelError(string.Empty, $"O arquivo com o nome '{fileName}' já foi enviado.");
                    await LoadDataAsync();
                    return Page();
                }

                const long maxFileSize = 5 * 1024 * 1024; // 5MB in bytes
                if (UploadedFile.Length > maxFileSize)
                {
                    ModelState.AddModelError(string.Empty, "Tamanho Máximo 5MB");
                    await LoadDataAsync();
                    return Page();
                }

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

            // Reload data after upload
            await LoadDataAsync();

            return Page();
        }

        public IActionResult OnGetDownload(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileName = Path.GetFileName(filePath);
            var mimeType = "application/pdf"; // Adjust as needed for different file types

            return PhysicalFile(filePath, mimeType, fileName);
        }

        private async Task LoadDataAsync()
        {
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;

            if (cnpjClaim != null)
            {
                var fornecedor = await _context.Tbl_Fornecedor.FirstOrDefaultAsync(f => f.CNPJ == cnpjClaim);
                if (fornecedor == null) return;

                IsAlimentosNaoIndustrializados = fornecedor.FORNECEDORALIMENTOS == "Alimentos Não Industrializados";

                UploadedFiles = await _context.Tbl_FornecedorDocumentos.Where(d => d.CNPJ == cnpjClaim).ToListAsync();

                IsSintegraUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "SINTEGRA");

                if (IsAlimentosNaoIndustrializados)
                {
                    IsDocumentacaoSanitariaUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Sanitária");
                    IsDocumentacaoAmbientalUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Ambiental ou Operacional");
                    IsDocumentacaoControlePragasUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Controle de Pragas");
                    IsDocumentacaoControleAguaUploaded = UploadedFiles.Any(f => f.TIPODOCUMENTO == "Documentação Controle de Água");
                }

                // Load existing comments with user info
                ComentariosViewModel = await _context.Tbl_FornecedorComentarios
                    .Where(c => c.CNPJ == cnpjClaim)
                    .Join(
                        _context.Tbl_Usuario,
                        comentario => comentario.ID_USUARIO,
                        usuario => usuario.ID_Usuario,
                        (comentario, usuario) => new ComentarioViewModel
                        {
                            ID = comentario.ID,
                            CNPJ = comentario.CNPJ,
                            COMENTARIO = comentario.COMENTARIO,
                            DATACOMENTARIO = comentario.DATACOMENTARIO,
                            UsuarioNome = usuario.Usuario,
                            UsuarioTipo = usuario.TipoUsuario
                        }
                    ).ToListAsync();
            }
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

        public async Task<IActionResult> OnPostAddCommentAsync()
        {
            var cnpjClaim = User.Claims.FirstOrDefault(c => c.Type == "CNPJ")?.Value;
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ID_Usuario")?.Value;

            if (cnpjClaim == null || userIdClaim == null)
            {
                return BadRequest();
            }

            // Only save the comment if it is not null or empty
            if (!string.IsNullOrWhiteSpace(COMENTARIO))
            {
                var newComment = new Tbl_FornecedorComentarios
                {
                    CNPJ = cnpjClaim,
                    ID_USUARIO = int.Parse(userIdClaim),
                    COMENTARIO = COMENTARIO,
                    DATACOMENTARIO = DateTime.Now
                };

                _context.Tbl_FornecedorComentarios.Add(newComment);
                await _context.SaveChangesAsync();
            }

            // Reload data after adding the comment
            await LoadDataAsync(); // This replaces the previous LoadUploadedFilesAsync

            return RedirectToPage(); // Redirect to avoid resubmission
        }

        public class ComentarioViewModel
        {
            public int ID { get; set; }
            public string CNPJ { get; set; }
            public string COMENTARIO { get; set; }
            public DateTime DATACOMENTARIO { get; set; }
            public string UsuarioNome { get; set; }
            public string UsuarioTipo { get; set; }
        }

    }
}
