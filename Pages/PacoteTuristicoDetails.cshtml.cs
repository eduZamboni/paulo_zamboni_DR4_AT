using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Models;
using AgenciaTurismo.Services;

namespace AgenciaTurismo.Pages
{
    public class PacoteTuristicoDetailsModel : PageModel
    {
        private readonly PacoteTuristicoService _pacoteService;
        private readonly CidadeDestinoService _cidadeService;
        private readonly PaisDestinoService _paisService;

        public PacoteTuristicoDetailsModel()
        {
            _pacoteService = new PacoteTuristicoService();
            _cidadeService = new CidadeDestinoService();
            _paisService = new PaisDestinoService();
        }

        [BindProperty(SupportsGet = true)]
        public int PacoteId { get; set; }

        public PacoteTuristico? Pacote { get; set; }

        // Propriedades calculadas para exibição
        public decimal PrecoPorDia => Pacote != null && Pacote.DuracaoEmDias > 0 
            ? Pacote.PrecoPorPessoa / Pacote.DuracaoEmDias 
            : 0;

        public decimal ReceitaTotal => Pacote?.Reservas.Sum(r => r.ValorTotal) ?? 0;

        public decimal MediaPorReserva => Pacote != null && Pacote.Reservas.Any() 
            ? Pacote.Reservas.Average(r => r.ValorTotal) 
            : 0;

        public double TaxaOcupacao => Pacote != null && Pacote.CapacidadeMaxima > 0 
            ? (double)Pacote.Reservas.Count / Pacote.CapacidadeMaxima 
            : 0;

        public IActionResult OnGet(int id)
        {
            PacoteId = id;

            // Buscar o pacote pelo ID
            Pacote = _pacoteService.GetById(id);

            if (Pacote == null)
            {
                // Pacote não encontrado - a página mostrará a mensagem de erro
                return Page();
            }

            // Carregar dados relacionados (cidade e país)
            CarregarDadosRelacionados();

            return Page();
        }

        private void CarregarDadosRelacionados()
        {
            if (Pacote == null) return;

            // Carregar cidade de destino
            var cidade = _cidadeService.GetById(Pacote.CidadeDestinoId);
            if (cidade != null)
            {
                // Carregar país da cidade
                cidade.Pais = _paisService.GetById(cidade.PaisDestinoId);
                Pacote.CidadeDestino = cidade;
            }
        }

        // Método auxiliar para verificar se o pacote está disponível para reservas
        public bool PacoteDisponivel => Pacote != null && Pacote.CapacidadeRestante > 0;

        // Método auxiliar para obter o status do pacote
        public string StatusPacote
        {
            get
            {
                if (Pacote == null) return "Não encontrado";
                
                if (Pacote.CapacidadeRestante == 0)
                    return "Lotado";
                
                if (Pacote.CapacidadeRestante <= 3)
                    return "Últimas vagas";
                
                return "Disponível";
            }
        }

        // Método auxiliar para obter a classe CSS do status
        public string StatusCssClass
        {
            get
            {
                return StatusPacote switch
                {
                    "Lotado" => "danger",
                    "Últimas vagas" => "warning",
                    "Disponível" => "success",
                    _ => "secondary"
                };
            }
        }

        // Método auxiliar para verificar se as datas são válidas
        public bool TemDatasDefinidas => Pacote?.DataInicio.HasValue == true && Pacote?.DataFim.HasValue == true;

        // Método auxiliar para calcular quantos dias restam até o início (se aplicável)
        public int? DiasAteInicio
        {
            get
            {
                if (Pacote?.DataInicio.HasValue != true) return null;
                
                var diasRestantes = (Pacote.DataInicio!.Value - DateTime.Today).Days;
                return diasRestantes > 0 ? diasRestantes : null;
            }
        }

        // Método auxiliar para verificar se o pacote já começou
        public bool PacoteJaComecou => Pacote?.DataInicio.HasValue == true && Pacote.DataInicio.Value <= DateTime.Today;

        // Método auxiliar para verificar se o pacote já terminou
        public bool PacoteJaTerminou => Pacote?.DataFim.HasValue == true && Pacote.DataFim.Value < DateTime.Today;
    }
}