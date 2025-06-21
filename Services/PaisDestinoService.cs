using AgenciaTurismo.Models;
using System.Collections.Generic;
using System.Linq;

namespace AgenciaTurismo.Services
{
    public class PaisDestinoService
    {
        private static readonly List<PaisDestino> _paises = new();
        private static int _nextId = 1;

        static PaisDestinoService()
        {
            _paises.Add(new PaisDestino { Id = 1, Nome = "Brasil", Continente = "América do Sul", CodigoIso = "BR" });
            _paises.Add(new PaisDestino { Id = 2, Nome = "Argentina", Continente = "América do Sul", CodigoIso = "AR" });
            _paises.Add(new PaisDestino { Id = 3, Nome = "França", Continente = "Europa", CodigoIso = "FR" });
            _paises.Add(new PaisDestino { Id = 4, Nome = "Estados Unidos", Continente = "América do Norte", CodigoIso = "US" });
            _paises.Add(new PaisDestino { Id = 5, Nome = "Japão", Continente = "Ásia", CodigoIso = "JP" });
            _nextId = 6;
        }

        public List<PaisDestino> GetAll() => _paises;

        public PaisDestino? GetById(int id) => _paises.FirstOrDefault(p => p.Id == id);

        public void Add(PaisDestino pais)
        {
            pais.Id = _nextId++;
            _paises.Add(pais);
        }
    }
}