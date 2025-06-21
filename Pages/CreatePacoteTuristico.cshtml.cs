using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Models;
using AgenciaTurismo.Services;
using System.ComponentModel.DataAnnotations;

namespace AgenciaTurismo.Pages
{
    public class CreatePacoteTuristicoModel : PageModel
    {
        private readonly PacoteTuristicoService _pacoteService;
        private readonly CidadeDestinoService _cidadeService;
        private readonly PaisDestinoService _paisService;

        public CreatePacoteTuristicoModel()
        {
            _pacoteService = new PacoteTuristicoService();
            _cidadeService = new CidadeDestinoService();
            _paisService = new PaisDestinoService();
        }

        [BindProperty]
        public PacoteTuristico NovoPacote { get; set; } = new();

        public List<CidadeDestino> CidadesDisponiveis { get; set; } = new();
        public List<PacoteTuristico> PacotesCadastrados { get; set; } = new();

        [TempData]
        public string MensagemSucesso { get; set; } = string.Empty;

        public void OnGet()
        {
            CarregarDados();
        }

        public IActionResult OnPost()
        {
            // Validações customizadas
            ValidarDadosCustomizados();

            if (!ModelState.IsValid)
            {
                CarregarDados();
                return Page();
            }

            try
            {
                // Buscar a cidade selecionada para associar ao pacote
                var cidadeSelecionada = _cidadeService.GetById(NovoPacote.CidadeDestinoId);
                if (cidadeSelecionada != null)
                {
                    // Garantir que a cidade tem o país carregado
                    cidadeSelecionada.Pais = _paisService.GetById(cidadeSelecionada.PaisDestinoId);
                    NovoPacote.CidadeDestino = cidadeSelecionada;
                }

                // Adicionar o novo pacote
                _pacoteService.Add(NovoPacote);

                // Configurar evento de capacidade
                NovoPacote.CapacityReached += (sender, e) => 
                {
                    Console.WriteLine($"ALERTA: Capacidade máxima atingida para o pacote {e.Pacote.Titulo}!");
                };

                MensagemSucesso = $"Pacote '{NovoPacote.Titulo}' cadastrado com sucesso!";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao cadastrar pacote: {ex.Message}");
                CarregarDados();
                return Page();
            }
        }

        private void CarregarDados()
        {
            // Carregar cidades com seus países
            CidadesDisponiveis = _cidadeService.GetAll();
            foreach (var cidade in CidadesDisponiveis)
            {
                cidade.Pais = _paisService.GetById(cidade.PaisDestinoId);
            }

            // Carregar pacotes cadastrados
            PacotesCadastrados = _pacoteService.GetAll();
            foreach (var pacote in PacotesCadastrados)
            {
                var cidade = _cidadeService.GetById(pacote.CidadeDestinoId);
                if (cidade != null)
                {
                    cidade.Pais = _paisService.GetById(cidade.PaisDestinoId);
                    pacote.CidadeDestino = cidade;
                }
            }
        }

        private void ValidarDadosCustomizados()
        {
            // Validação customizada: verificar se já existe um pacote com o mesmo título
            if (!string.IsNullOrWhiteSpace(NovoPacote.Titulo))
            {
                var pacoteExistente = _pacoteService.GetAll()
                    .FirstOrDefault(p => p.Titulo.Equals(NovoPacote.Titulo, StringComparison.OrdinalIgnoreCase));

                if (pacoteExistente != null)
                {
                    ModelState.AddModelError("NovoPacote.Titulo", 
                        "Já existe um pacote com este título.");
                }
            }

            // Validação customizada: verificar consistência de datas
            if (NovoPacote.DataInicio.HasValue && NovoPacote.DataFim.HasValue)
            {
                if (NovoPacote.DataFim <= NovoPacote.DataInicio)
                {
                    ModelState.AddModelError("NovoPacote.DataFim", 
                        "A data de fim deve ser posterior à data de início.");
                }

                // Verificar se as datas não são no passado
                if (NovoPacote.DataInicio < DateTime.Today)
                {
                    ModelState.AddModelError("NovoPacote.DataInicio", 
                        "A data de início não pode ser no passado.");
                }
            }
        }
    }
}