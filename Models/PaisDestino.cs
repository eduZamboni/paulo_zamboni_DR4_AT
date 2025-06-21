using System;
using System.Collections.Generic;

namespace AgenciaTurismo.Models
{
    public class PaisDestino
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Continente { get; set; } = string.Empty;
        public string CodigoIso { get; set; } = string.Empty;
        
        // Relacionamento com CidadeDestino
        public ICollection<CidadeDestino> Cidades { get; set; }
        
        public PaisDestino()
        {
            Cidades = new List<CidadeDestino>();
        }
    }
}