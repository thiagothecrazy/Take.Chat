using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Take.Chat.Core.Interfaces;
using Take.Chat.Web.Models;

namespace Take.Chat.Web.Controllers
{
    public class BatePapoController : Controller
    {
        private readonly IBatePapoServico _batePapoServico;

        public BatePapoController(IBatePapoServico batePapoServico)
        {
            _batePapoServico = batePapoServico;
        }

        [HttpGet]
        public IActionResult Entrar(string mensagem)
        {
            return View("Entrar", mensagem);
        }

        [HttpPost]
        public IActionResult Index(BatePapoViewModel model)
        {
            var resultado =  _batePapoServico.AdicionarUsuarioSala(model.Apelido, model.NomeSala);
            
            if(!resultado.Sucesso)
            {
                var mensagemErro = string.Join(" | ", resultado.Notificacoes);
                return Entrar(mensagemErro);
            }

            return Entrar("Sucesso!!!");
            //return View("Index", model);
        }
    }
}