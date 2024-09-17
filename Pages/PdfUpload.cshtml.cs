using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PortalArcomix.Data;
using PortalArcomix.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Net.Mail;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace TCC2Gutemberg.Pages
{
    public class PdfUploadModel : PageModel
    {
        private readonly OracleDbContext _context;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient httpClient = new HttpClient();

        public PdfUploadModel(OracleDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public List<Tbl_Empresa> Empresas { get; set; } = new List<Tbl_Empresa>();

        [BindProperty]
        public IFormFile PdfFile { get; set; }

        [BindProperty]
        public int SelectedCompany { get; set; }

        public string Message { get; set; }
        public bool IsSuccess { get; set; }

        public async Task OnGetAsync()
        {
            Empresas = await _context.Tbl_Empresa.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (PdfFile == null || PdfFile.Length == 0 || SelectedCompany == 0)
            {
                Message = "Por favor, selecione uma empresa e faça o upload de um arquivo PDF.";
                IsSuccess = false;
                Empresas = await _context.Tbl_Empresa.ToListAsync();
                return Page();
            }

            // Step 1: Save the PDF
            var uploadsFolder = System.IO.Path.Combine(@"C:\Users\Gutemberg\PortalArcomix\wwwroot\uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var sanitizedFileName = System.IO.Path.GetFileNameWithoutExtension(PdfFile.FileName).Replace(" ", "_") + System.IO.Path.GetExtension(PdfFile.FileName);
            var filePath = System.IO.Path.Combine(uploadsFolder, sanitizedFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await PdfFile.CopyToAsync(stream);
            }

            // Step 2: Extract text from the PDF using iTextSharp
            string extractedText = ExtractTextFromPdf(filePath);

            // Step 3: Trim the content to fit within OpenAI's token limits
            string trimmedText = extractedText.Length > 4000 ? extractedText.Substring(0, 4000) : extractedText;

            // Step 4: Generate summary using ChatGPT API
            string summary;
            try
            {
                summary = await GenerateSummaryFromChatGPT(trimmedText);
            }
            catch (Exception ex)
            {
                Message = "Erro ao gerar resumo via ChatGPT: " + ex.Message;
                IsSuccess = false;
                return Page();
            }

            // Step 5: Save the PDF info and summary to the database
            var pdfEntry = new Tbl_FatoRelevante
            {
                ID_Empresa = SelectedCompany,
                NomeArquivo = sanitizedFileName,
                CaminhoArquivo = filePath,
                DataUpload = DateTime.Now
            };

            _context.Tbl_FatoRelevante.Add(pdfEntry);
            await _context.SaveChangesAsync();

            // Step 6: Notify users via email
            await NotifyUsers(SelectedCompany, summary, sanitizedFileName);

            // Success message
            Message = $"Arquivo {PdfFile.FileName} enviado e processado com sucesso!";
            IsSuccess = true;
            return RedirectToPage("/PdfUpload");
        }

        // Extract text from PDF using iTextSharp
        private string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();

            using (PdfReader reader = new PdfReader(filePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }

            return text.ToString();
        }

        // Generate summary using OpenAI API (ChatGPT)
        private async Task<string> GenerateSummaryFromChatGPT(string content)
        {
            var openAiApiKey = _configuration["OpenAI:ApiKey"];
            var apiUrl = "https://api.openai.com/v1/chat/completions";

            var requestPayload = new
            {
                model = "gpt-3.5-turbo", // Change to GPT-3.5 Turbo for cost effectiveness
                messages = new[]
                {
                    new { role = "system", content = "Você é um assistente que resume textos." },
                    new { role = "user", content = "Resuma o seguinte conteúdo: " + content }
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var jsonPayload = JsonConvert.SerializeObject(requestPayload);
            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiApiKey);

            var response = await httpClient.PostAsync(apiUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseString);

            if (responseJson?.choices != null && responseJson.choices.Count > 0)
            {
                return responseJson.choices[0].message.content.ToString().Trim();
            }

            return "Falha ao gerar resumo.";
        }

        // Notify users by email
        private async Task NotifyUsers(int companyId, string summary, string fileName)
        {
            var users = await _context.Tbl_Usuario_Empresa
                                      .Include(ue => ue.Usuario)
                                      .Where(ue => ue.ID_Empresa == companyId)
                                      .Select(ue => ue.Usuario)
                                      .ToListAsync();

            foreach (var user in users)
            {
                await SendEmailNotification(user.Email, summary, user.Usuario ?? "Investidor", fileName);
            }
        }

        // Send email with summary
        private async Task SendEmailNotification(string toEmail, string summary, string username, string fileName)
        {
            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("gutemberg@hgstech.com.br"),
                    Subject = $"Resumo de Fato Relevante: {fileName}",
                    Body = $"Olá {username},\n\nAqui está o resumo do fato relevante mais recente:\n\n{summary}\n\nAtenciosamente,\nEquipe de Suporte",
                    IsBodyHtml = false
                };

                mail.To.Add(toEmail);

                SmtpClient smtpServer = new SmtpClient("smtp.hostinger.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential("gutemberg@hgstech.com.br", "HGs@8788")
                };

                await smtpServer.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}
