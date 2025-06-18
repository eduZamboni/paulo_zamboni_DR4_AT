using Microsoft.AspNetCore.StaticAssets;

namespace AgenciaTurismo.Services
{
    public delegate decimal CalculateDelegate(decimal preco);

    public class DescontoService
    {
        public static decimal AplicarDesconto(decimal precoOriginal, CalculateDelegate calculo)
        {
            return calculo(precoOriginal);
        }
    }
}   