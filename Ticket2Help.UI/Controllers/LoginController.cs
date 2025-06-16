using Ticket2Help.BLL.Services;
using Ticket2Help.DAL.Repositories;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Controllers
{
    /// <summary>
    /// Controlador para autenticação de utilizadores.
    /// </summary>
    public class LoginController
    {
        private readonly UtilizadorService _utilizadorService;

        /// <summary>
        /// Construtor do controlador de login.
        /// </summary>
        public LoginController()
        {
            var utilizadorRepository = new UtilizadorRepository();
            _utilizadorService = new UtilizadorService(utilizadorRepository);
        }

        /// <summary>
        /// Autentica um utilizador.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="senha">Senha do utilizador.</param>
        /// <returns>Utilizador autenticado ou null.</returns>
        public Utilizador Autenticar(string codigo, string senha)
        {
            return _utilizadorService.Autenticar(codigo, senha);
        }
    }
}