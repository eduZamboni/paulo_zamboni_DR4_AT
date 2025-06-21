using System;
using System.Collections.Generic;
using AgenciaTurismo.Models;
using System.ComponentModel.DataAnnotations;

namespace AgenciaTurismo.Models
{
    // Delegate Para a Capacidade Atingida
    public delegate void CapacityReachedEventHandler(object sender, CapacityReachedEventArgs e);

    public class CapacityReachedEventArgs : EventArgs
    {
        public PacoteTuristico Pacote { get; set; }
        public int CapacidadeAtual { get; set; }
        public int CapacidadeMaxima { get; set; }

        public CapacityReachedEventArgs(PacoteTuristico pacote, int capacidadeAtual, int capacidadeMaxima)
        {
            Pacote = pacote;
            CapacidadeAtual = capacidadeAtual;
            CapacidadeMaxima = capacidadeMaxima;
        }
    }

    public class PacoteTuristico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O título do pacote é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "O título deve ter entre 5 e 200 caracteres.")]
        [Display(Name = "Título do Pacote")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição do pacote é obrigatória.")]
        [StringLength(1000, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 1000 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A capacidade máxima é obrigatória.")]
        [Range(1, 500, ErrorMessage = "A capacidade deve ser entre 1 e 500 pessoas.")]
        [Display(Name = "Capacidade Máxima")]
        public int CapacidadeMaxima { get; set; }

        [Required(ErrorMessage = "O preço por pessoa é obrigatório.")]
        [Range(0.01, 50000.00, ErrorMessage = "O preço deve ser entre R$ 0,01 e R$ 50.000,00.")]
        [Display(Name = "Preço por Pessoa")]
        [DataType(DataType.Currency)]
        public decimal PrecoPorPessoa { get; set; }

        [Required(ErrorMessage = "A duração em dias é obrigatória.")]
        [Range(1, 365, ErrorMessage = "A duração deve ser entre 1 e 365 dias.")]
        [Display(Name = "Duração (dias)")]
        public int DuracaoEmDias { get; set; }

        [Required(ErrorMessage = "Selecione uma cidade de destino.")]
        [Display(Name = "Cidade de Destino")]
        public int CidadeDestinoId { get; set; }

        // Navegação para cidade de destino
        public CidadeDestino? CidadeDestino { get; set; }

        [Display(Name = "Data de Início")]
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }

        [Display(Name = "Data de Fim")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }

        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres.")]
        [Display(Name = "Observações")]
        public string? Observacoes { get; set; }

        public List<Reserva> Reservas { get; set; } = new();

        // Event Alert para Capacidade Atingida
        public event CapacityReachedEventHandler? CapacityReached;

        public bool VerificarCapacidade()
        {
            // Dispara o evento quando a capacidade é atingida ou excedida
            if (Reservas.Count >= CapacidadeMaxima)
            {
                OnCapacityReached();
                return false; // Capacidade excedida
            }
            return true; // Ainda há capacidade
        }

        public int CapacidadeRestante => Math.Max(0, CapacidadeMaxima - Reservas.Count);

        protected virtual void OnCapacityReached()
        {
            CapacityReached?.Invoke(this, new CapacityReachedEventArgs(this, Reservas.Count, CapacidadeMaxima));
        }

        // Propriedade calculada para exibir o nome completo do destino
        public string DestinoCompleto => CidadeDestino != null ? 
            $"{CidadeDestino.Nome}, {CidadeDestino.Pais?.Nome}" : 
            "Destino não definido";

        // Validação customizada de datas
        public bool ValidarDatas()
        {
            if (DataInicio.HasValue && DataFim.HasValue)
            {
                return DataFim > DataInicio;
            }
            return true; // Se não tem datas, não há problema
        }
    }
}