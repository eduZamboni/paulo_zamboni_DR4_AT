using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Models;
using AgenciaTurismo.Services;
using System.ComponentModel.DataAnnotations;

namespace AgenciaTurismo.Pages
{
    public class CreateCidadeDestinoModel : PageModel
    {
        private readonly CidadeDestinoService _cidadeService;
        private readonly PaisDestinoService _paisService;

        public CreateCidadeDestinoModel()
        {
            _cidadeService = new CidadeDestinoService();
            _paisService = new PaisDestinoService();
        }

        [BindProperty]
        public CidadeDestino NovaCidade { get; set; } = new();

        public List<PaisDestino> PaisesDisponiveis { get; set; } = new();
        public List<CidadeDestino> CidadesCadastradas { get; set; } = new();

        [TempData]
        public string MensagemSucesso { get; set; } = string.Empty;

        public void OnGet()
        {
            CarregarDados();
        }

        public IActionResult OnPost()
        {
            // Validações customizadas adicionais
            ValidarDadosCustomizados();

            if (!ModelState.IsValid)
            {
                CarregarDados();
                return Page();
            }

            try
            {
                // Buscar o país selecionado para associar à cidade
                var paisSelecionado = _paisService.GetById(NovaCidade.PaisDestinoId);
                if (paisSelecionado != null)
                {
                    NovaCidade.Pais = paisSelecionado;
                }

                // Adicionar a nova cidade
                _cidadeService.Add(NovaCidade);

                // Definir mensagem de sucesso
                MensagemSucesso = $"Cidade '{NovaCidade.Nome}' cadastrada com sucesso!";

                // Limpar o form e redirecionar
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro ao cadastrar cidade: {ex.Message}");
                CarregarDados();
                return Page();
            }
        }

        private void CarregarDados()
        {
            PaisesDisponiveis = _paisService.GetAll();
            CidadesCadastradas = _cidadeService.GetAll();

            // Associar dados do país às cidades já cadastradas para exibição
            foreach (var cidade in CidadesCadastradas)
            {
                var pais = _paisService.GetById(cidade.PaisDestinoId);
                if (pais != null)
                {
                    cidade.Pais = pais;
                }
            }
        }

        private void ValidarDadosCustomizados()
        {
            // Validação customizada: verificar se já existe uma cidade com o mesmo nome no mesmo país
            if (!string.IsNullOrWhiteSpace(NovaCidade.Nome) && NovaCidade.PaisDestinoId > 0)
            {
                var cidadeExistente = _cidadeService.GetAll()
                    .FirstOrDefault(c => c.Nome.Equals(NovaCidade.Nome, StringComparison.OrdinalIgnoreCase) 
                                        && c.PaisDestinoId == NovaCidade.PaisDestinoId);

                if (cidadeExistente != null)
                {
                    ModelState.AddModelError("NovaCidade.Nome", 
                        "Já existe uma cidade com este nome neste país.");
                }
            }

            // Validação customizada: verificar caracteres especiais
            if (!string.IsNullOrWhiteSpace(NovaCidade.Nome))
            {
                var caracteresInvalidos = new char[] { '@', '#', '$', '%', '&', '*', '!', '?', '<', '>', '|' };
                if (NovaCidade.Nome.IndexOfAny(caracteresInvalidos) >= 0)
                {
                    ModelState.AddModelError("NovaCidade.Nome", 
                        "O nome da cidade não pode conter caracteres especiais como @, #, $, %, &, *, !, ?, <, >, |");
                }
            }

            // Validação customizada: verificar se não é apenas espaços em branco
            if (!string.IsNullOrWhiteSpace(NovaCidade.Nome) && string.IsNullOrWhiteSpace(NovaCidade.Nome.Trim()))
            {
                ModelState.AddModelError("NovaCidade.Nome", 
                    "O nome da cidade não pode conter apenas espaços em branco.");
            }
        }
    }
}