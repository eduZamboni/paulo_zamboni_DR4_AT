using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;

namespace AgenciaTurismo.Pages
{
    public class ViewNotesModel : PageModel
    {
        private readonly string _pastaArquivos;

        public ViewNotesModel(IWebHostEnvironment environment)
        {
            _pastaArquivos = Path.Combine(environment.WebRootPath, "files");
            
            if (!Directory.Exists(_pastaArquivos))
            {
                Directory.CreateDirectory(_pastaArquivos);
            }
        }

        [BindProperty]
        public string NomeArquivo { get; set; } = string.Empty;

        [BindProperty]
        public string ConteudoNota { get; set; } = string.Empty;

        public Dictionary<string, DateTime> ArquivosDisponiveis { get; set; } = new();
        public string ArquivoSelecionado { get; set; } = string.Empty;
        public string ConteudoArquivo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string TipoMensagem { get; set; } = "info";

        public void OnGet()
        {
            CarregarArquivos();
        }

        public IActionResult OnPostCriarNota()
        {
            try
            {
                // Validar nome do arquivo
                if (string.IsNullOrWhiteSpace(NomeArquivo))
                {
                    Mensagem = "O nome da nota é obrigatório.";
                    TipoMensagem = "danger";
                    CarregarArquivos();
                    return Page();
                }

                // Validar conteúdo
                if (string.IsNullOrWhiteSpace(ConteudoNota))
                {
                    Mensagem = "O conteúdo da nota é obrigatório.";
                    TipoMensagem = "danger";
                    CarregarArquivos();
                    return Page();
                }

                // Remover caracteres inválidos do nome do arquivo
                var nomeArquivoLimpo = RemoverCaracteresInvalidos(NomeArquivo.Trim());
                
                if (string.IsNullOrWhiteSpace(nomeArquivoLimpo))
                {
                    Mensagem = "Nome do arquivo inválido. Use apenas letras, números, espaços e hífens.";
                    TipoMensagem = "danger";
                    CarregarArquivos();
                    return Page();
                }

                // Definir caminho do arquivo
                var nomeComExtensao = $"{nomeArquivoLimpo}.txt";
                var caminhoCompleto = Path.Combine(_pastaArquivos, nomeComExtensao);

                if (System.IO.File.Exists(caminhoCompleto))
                {
                    Mensagem = $"Já existe uma nota com o nome '{nomeArquivoLimpo}'. Escolha outro nome.";
                    TipoMensagem = "warning";
                    CarregarArquivos();
                    return Page();
                }

                // Salvar arquivo
                System.IO.File.WriteAllText(caminhoCompleto, ConteudoNota, Encoding.UTF8);

                // Limpar formulário
                NomeArquivo = string.Empty;
                ConteudoNota = string.Empty;

                // Mensagem de sucesso
                Mensagem = $"Nota '{nomeArquivoLimpo}' salva com sucesso!";
                TipoMensagem = "success";

                CarregarArquivos();
                return Page();
            }
            catch (Exception ex)
            {
                Mensagem = $"Erro ao salvar a nota: {ex.Message}";
                TipoMensagem = "danger";
                CarregarArquivos();
                return Page();
            }
        }

        public IActionResult OnPostLerArquivo(string nomeArquivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    Mensagem = "Nome do arquivo não informado.";
                    TipoMensagem = "danger";
                    CarregarArquivos();
                    return Page();
                }

                var caminhoCompleto = Path.Combine(_pastaArquivos, nomeArquivo);

                if (!System.IO.File.Exists(caminhoCompleto))
                {
                    Mensagem = $"Arquivo '{nomeArquivo}' não encontrado.";
                    TipoMensagem = "danger";
                    CarregarArquivos();
                    return Page();
                }

                // Ler conteúdo do arquivo
                ConteudoArquivo = System.IO.File.ReadAllText(caminhoCompleto, Encoding.UTF8);
                ArquivoSelecionado = nomeArquivo;

                CarregarArquivos();
                return Page();
            }
            catch (Exception ex)
            {
                Mensagem = $"Erro ao ler o arquivo: {ex.Message}";
                TipoMensagem = "danger";
                CarregarArquivos();
                return Page();
            }
        }

        private void CarregarArquivos()
        {
            ArquivosDisponiveis.Clear();

            try
            {
                if (Directory.Exists(_pastaArquivos))
                {
                    var arquivos = Directory.GetFiles(_pastaArquivos, "*.txt");
                    
                    foreach (var arquivo in arquivos)
                    {
                        var nomeArquivo = Path.GetFileName(arquivo);
                        var dataModificacao = System.IO.File.GetLastWriteTime(arquivo);
                        ArquivosDisponiveis.Add(nomeArquivo, dataModificacao);
                    }

                    // Ordenar por data de modificação (mais recente primeiro)
                    ArquivosDisponiveis = ArquivosDisponiveis
                        .OrderByDescending(x => x.Value)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
            }
            catch (Exception ex)
            {
                Mensagem = $"Erro ao carregar arquivos: {ex.Message}";
                TipoMensagem = "danger";
            }
        }

        private string RemoverCaracteresInvalidos(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return string.Empty;

            // Caracteres inválidos para nomes de arquivo
            var caracteresInvalidos = Path.GetInvalidFileNameChars();
            
            // Construir string limpa
            var resultado = new StringBuilder();
            
            foreach (char c in nome)
            {
                if (!caracteresInvalidos.Contains(c))
                {
                    resultado.Append(c);
                }
                else
                {
                    resultado.Append('_');
                }
            }

            return resultado.ToString().Trim();
        }

        // Propriedade auxiliar para verificar se há arquivos
        public bool TemArquivos => ArquivosDisponiveis.Any();

        // Propriedade auxiliar para contar total de arquivos
        public int TotalArquivos => ArquivosDisponiveis.Count;

        // Método auxiliar para formatar tamanho do arquivo
        public string ObterTamanhoArquivo(string nomeArquivo)
        {
            try
            {
                var caminhoCompleto = Path.Combine(_pastaArquivos, nomeArquivo);
                if (System.IO.File.Exists(caminhoCompleto))
                {
                    var info = new FileInfo(caminhoCompleto);
                    var tamanho = info.Length;

                    if (tamanho < 1024)
                        return $"{tamanho} bytes";
                    else if (tamanho < 1024 * 1024)
                        return $"{tamanho / 1024:F1} KB";
                    else
                        return $"{tamanho / (1024 * 1024):F1} MB";
                }
            }
            catch
            {
                // Ignorar erros
            }

            return "Tamanho desconhecido";
        }
    }
}