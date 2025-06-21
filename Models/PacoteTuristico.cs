using System;
using System.Collections.Generic;
using AgenciaTurismo.Models;

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
        public string Titulo { get; set; } = string.Empty;
        public int CapacidadeMaxima { get; set; }

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
    }
}