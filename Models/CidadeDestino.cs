using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgenciaTurismo.Models
{
    public class CidadeDestino
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome da cidade é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome da cidade deve ter entre 3 e 100 caracteres.")]
        [Display(Name = "Nome da Cidade")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Selecione um país de destino.")]
        [Display(Name = "País de Destino")]
        public int PaisDestinoId { get; set; }
        
        public PaisDestino? Pais { get; set; }
        
        // Relacionamento com PacoteTuristico
        public ICollection<PacoteTuristico> PacotesTuristicos { get; set; }
        
        public CidadeDestino()
        {
            PacotesTuristicos = new List<PacoteTuristico>();
        }
    }
}