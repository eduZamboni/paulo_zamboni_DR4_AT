namespace AgenciaTurismo.Services
{
    public static class ReservaService
    {
        public static decimal CalcularValorTotal(int dias, int valorDiaria, Func<int, int, decimal> calculo)
        {
            return calculo(dias, valorDiaria);
        }
    }
}