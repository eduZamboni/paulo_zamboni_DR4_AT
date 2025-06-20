using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Services;

namespace AgenciaTurismo.Pages
{
    public class ReservasModel : PageModel
    {
        [BindProperty]
        public int NumeroDeDiarias { get; set; }

        [BindProperty]
        public int PrecoOriginal { get; set; }

        public decimal? TotalReserva { get; set; }

        public decimal? PrecoComDesconto { get; set; }

        public List<string> LogsEmMemoria => LoggerService.Memoria;

        public void OnPost()
        {
            // EX3: Uso de Func com Expressão Lambda
            Func<int, int, decimal> calcularTotal = (dias, preco) => dias * preco;
            TotalReserva = ReservaService.CalcularValorTotal(NumeroDeDiarias, PrecoOriginal, calcularTotal);

            //Ex1: Delegate para Cálculo de Descontos
            CalculateDelegate desconto10 = preco => preco * 0.9m;
            PrecoComDesconto = DescontoService.AplicarDesconto(TotalReserva.Value, desconto10);

            // Ex2: Multicast Delegate para Registro de Logs
            Action<string> log = LoggerService.LogToConsole;
            log += LoggerService.LogToFile;
            log += LoggerService.LogToMemory;

            log($"Reserva criada: {NumeroDeDiarias} diárias de R$ {PrecoOriginal} = R$ {TotalReserva} + 10% de desconto = R$ {PrecoComDesconto}");
        }
    }
}
