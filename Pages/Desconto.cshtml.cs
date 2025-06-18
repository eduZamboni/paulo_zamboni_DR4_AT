using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AgenciaTurismo.Services;

namespace AgenciaTurismo.Pages
{
    public class DescontoModel : PageModel
    {
        [BindProperty]
        public decimal PrecoOriginal { get; set; }

        public decimal? PrecoComDesconto { get; set; }

        public void OnPost()
        {
            CalculateDelegate desconto10 = preco => preco * 0.9m;
            PrecoComDesconto = DescontoService.AplicarDesconto(PrecoOriginal, desconto10);
        }
    }
}
