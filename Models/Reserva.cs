namespace AgenciaTurismo.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public int NumeroDeDiarias { get; set; }
        public decimal ValorTotal { get; set; }

    }
}