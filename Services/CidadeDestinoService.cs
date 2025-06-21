using AgenciaTurismo.Models;
using System.Collections.Generic;
using System.Linq;

namespace AgenciaTurismo.Services
{
    public class CidadeDestinoService
    {
        private static readonly List<CidadeDestino> _cidades = new();
        private static int _nextId = 1;

        public List<CidadeDestino> GetAll() => _cidades;

        public CidadeDestino? GetById(int id) => _cidades.FirstOrDefault(c => c.Id == id);

        public void Add(CidadeDestino cidade)
        {
            cidade.Id = _nextId++;
            _cidades.Add(cidade);
        }

        public void Update(CidadeDestino cidade)
        {
            var cidadeExistente = GetById(cidade.Id);
            if (cidadeExistente != null)
            {
                cidadeExistente.Nome = cidade.Nome;
                cidadeExistente.PaisDestinoId = cidade.PaisDestinoId;
            }
        }

        public void Delete(int id)
        {
            var cidade = GetById(id);
            if (cidade != null)
            {
                _cidades.Remove(cidade);
            }
        }
    }
}