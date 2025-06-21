using AgenciaTurismo.Models;
using System.Collections.Generic;
using System.Linq;

namespace AgenciaTurismo.Services
{
    public class PacoteTuristicoService
    {
        private static readonly List<PacoteTuristico> _pacotes = new();
        private static int _nextId = 1;

        public List<PacoteTuristico> GetAll() => _pacotes;

        public PacoteTuristico? GetById(int id) => _pacotes.FirstOrDefault(p => p.Id == id);

        public void Add(PacoteTuristico pacote)
        {
            pacote.Id = _nextId++;
            _pacotes.Add(pacote);
        }
    }
}