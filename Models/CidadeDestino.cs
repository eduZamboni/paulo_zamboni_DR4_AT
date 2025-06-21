using System;

namespace AgenciaTurismo.Models
{
    public class CidadeDestino
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int PaisDestinoId { get; set; }
        public PaisDestino Pais { get; set; } = null!;
        
        // Relacionamento com PacoteTuristico
        public ICollection<PacoteTuristico> PacotesTuristicos { get; set; }
        
        public CidadeDestino()
        {
            PacotesTuristicos = new List<PacoteTuristico>();
        }
    }
}