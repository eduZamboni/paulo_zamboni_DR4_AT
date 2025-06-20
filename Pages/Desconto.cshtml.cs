using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Services;

namespace AgenciaTurismo.Pages
{
    public class DescontoModel : PageModel
    {
        [BindProperty]
        public decimal PrecoOriginal { get; set; }

        public decimal? PrecoComDesconto { get; set; }

        public List<string> LogsEmMemoria => LoggerService.Memoria;

        public void OnPost()
        {
            CalculateDelegate desconto10 = preco => preco * 0.9m;
            PrecoComDesconto = DescontoService.AplicarDesconto(PrecoOriginal, desconto10);

            //Multicast Delegate para Registro de Logs
            Action<string> log = LoggerService.LogToConsole;
            log += LoggerService.LogToFile;
            log += LoggerService.LogToMemory;

            log("Reserva criada, aguardando pagamento de R$ " + PrecoComDesconto);

        }
    }
}
