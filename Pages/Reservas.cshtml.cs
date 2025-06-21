using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Services;
using AgenciaTurismo.Models;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AgenciaTurismo.Pages
{
    public class ReservasModel : PageModel
    {
        private readonly PacoteTuristicoService _pacoteService;

        public ReservasModel()
        {
            _pacoteService = new PacoteTuristicoService();
        }

        [BindProperty]
        public int NumeroDeDiarias { get; set; }

        [BindProperty]
        public int PrecoOriginal { get; set; }

        [BindProperty]
        public string ClienteNome { get; set; } = string.Empty;

        public decimal? TotalReserva { get; set; }

        public decimal? PrecoComDesconto { get; set; }

        public List<string> LogsEmMemoria => LoggerService.Memoria;

        [BindProperty]
        public int? PacoteSelecionadoId { get; set; }

        public List<PacoteTuristico> PacotesDisponiveis { get; set; } = new();

        public string AlertaCapacidade { get; set; } = string.Empty;

        public void OnGet()
        {
            PacotesDisponiveis = _pacoteService.GetAll();
        }

        public void OnPost()
        {
            PacotesDisponiveis = _pacoteService.GetAll();

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

            // Processar pacote turístico selecionado
            if (PacoteSelecionadoId.HasValue)
            {
                var pacoteSelecionado = _pacoteService.GetById(PacoteSelecionadoId.Value);
                if (pacoteSelecionado != null)
                {
                    // Configurar o evento de capacidade antes de adicionar a reserva
                    pacoteSelecionado.CapacityReached += (sender, e) =>
                    {
                        string mensagem = $"ALERTA: Capacidade máxima atingida para o pacote '{e.Pacote.Titulo}'! Capacidade atual: {e.CapacidadeAtual}/{e.CapacidadeMaxima}";
                        Console.WriteLine(mensagem);
                        AlertaCapacidade = $"Atenção: O pacote '{e.Pacote.Titulo}' atingiu sua capacidade máxima!";
                        log(mensagem);
                    };

                    // Criar nova reserva
                    var novaReserva = new Reserva
                    {
                        ClienteNome = ClienteNome,
                        NumeroDeDiarias = NumeroDeDiarias,
                        ValorTotal = TotalReserva.Value,
                    };

                    // Adicionar reserva ao pacote
                    pacoteSelecionado.Reservas.Add(novaReserva);

                    // Verificar capacidade (isso pode disparar o evento)
                    pacoteSelecionado.VerificarCapacidade();

                    log($"Reserva adicionada ao pacote '{pacoteSelecionado.Titulo}' para o cliente {ClienteNome}");
                }
            }
        }
    }
}
