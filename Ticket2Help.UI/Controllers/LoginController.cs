using Ticket2Help.BLL.Services;
using Ticket2Help.DAL.Repositories;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Controllers
{
    /// <summary>
    /// Controlador para autenticação simples de utilizadores.
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
        /// Autentica um utilizador verificando código e password na base de dados.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="password">Password do utilizador.</param>
        /// <returns>Utilizador autenticado ou null se credenciais inválidas.</returns>
        public Utilizador? Autenticar(string codigo, string password)
        {
            return _utilizadorService.Autenticar(codigo, password);
        }

        /// <summary>
        /// Testa se a ligação à base de dados está disponível.
        /// </summary>
        /// <returns>True se conseguir ligar à BD.</returns>
        public bool TestarLigacaoBD()
        {
            return _utilizadorService.TestarLigacao();
        }

        /// <summary>
        /// Verifica se um utilizador existe na base de dados.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>True se o utilizador existir.</returns>
        public bool UtilizadorExiste(string codigo)
        {
            return _utilizadorService.UtilizadorExiste(codigo);
        }
    }
}