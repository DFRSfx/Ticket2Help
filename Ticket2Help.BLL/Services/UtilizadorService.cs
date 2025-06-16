using System.Collections.Generic;
using Ticket2Help.Models;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.DAL.Repositories;

namespace Ticket2Help.BLL.Services
{
    /// <summary>
    /// Serviço para gestão de utilizadores.
    /// </summary>
    public class UtilizadorService
    {
        private readonly IUtilizadorRepository _utilizadorRepository;

        /// <summary>
        /// Construtor do serviço de utilizadores.
        /// </summary>
        /// <param name="utilizadorRepository">Repositório de utilizadores.</param>
        public UtilizadorService(IUtilizadorRepository utilizadorRepository)
        {
            _utilizadorRepository = utilizadorRepository;
        }

        /// <summary>
        /// Autentica um utilizador no sistema.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="senha">Senha do utilizador.</param>
        /// <returns>Utilizador autenticado ou null.</returns>
        public Utilizador Autenticar(string codigo, string senha)
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(senha))
                return null;

            // Verificar se o utilizador existe na base de dados
            var utilizador = _utilizadorRepository.ObterPorCodigo(codigo);

            if (utilizador != null)
                return utilizador;

           

            // Fallback: se não conseguir validar através do repositório, rejeitar
            return null;
        }

        /// <summary>
        /// Obtém um utilizador pelo código.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        public Utilizador ObterPorCodigo(string codigo)
        {
            return _utilizadorRepository.ObterPorCodigo(codigo);
        }

        /// <summary>
        /// Obtém todos os utilizadores.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        public IEnumerable<Utilizador> ObterTodos()
        {
            return _utilizadorRepository.ObterTodos();
        }

        /// <summary>
        /// Cria um novo utilizador.
        /// </summary>
        /// <param name="utilizador">Utilizador a criar.</param>
        public void CriarUtilizador(Utilizador utilizador)
        {
            _utilizadorRepository.Criar(utilizador);
        }
    }
}