using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Models;
using AgenciaTurismo.Services;

namespace AgenciaTurismo.Pages
{
    public class PacotesTuristicosModel : PageModel
    {
        private readonly PacoteTuristicoService _pacoteService;

        public PacotesTuristicosModel()
        {
            _pacoteService = new PacoteTuristicoService();
        }

        public List<PacoteTuristico> Pacotes { get; set; } = new();

        [BindProperty]
        public PacoteTuristico NovoPacote { get; set; } = new();

        public void OnGet()
        {
            Pacotes = _pacoteService.GetAll();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                Pacotes = _pacoteService.GetAll();
                return Page();
            }

            _pacoteService.Add(NovoPacote);

            NovoPacote.CapacityReached += (sender, e) => 
            {
                Console.WriteLine($"ALERTA: Capacidade máxima atingida para o pacote {e.Pacote.Titulo}!");
            };

            return RedirectToPage();
        }
    }
}