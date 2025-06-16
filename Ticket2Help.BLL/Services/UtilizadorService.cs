using System.Collections.Generic;
using Ticket2Help.Models;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.DAL.Repositories;

namespace Ticket2Help.BLL.Services
{
    /// <summary>
    /// Serviço para gestão de utilizadores com autenticação simples.
    /// </summary>
    public class UtilizadorService
    {
        private readonly UtilizadorRepository _utilizadorRepository;

        /// <summary>
        /// Construtor do serviço de utilizadores.
        /// </summary>
        /// <param name="utilizadorRepository">Repositório de utilizadores.</param>
        public UtilizadorService(IUtilizadorRepository utilizadorRepository)
        {
            // Cast para o repositório concreto para aceder ao método AutenticarUtilizador
            _utilizadorRepository = utilizadorRepository as UtilizadorRepository ?? new UtilizadorRepository();
        }

        /// <summary>
        /// Autentica um utilizador no sistema usando código e password.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="password">Password do utilizador.</param>
        /// <returns>Utilizador autenticado ou null se credenciais inválidas.</returns>
        public Utilizador Autenticar(string codigo, string password)
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(password))
                return null;

            try
            {
                // Usar o método específico de autenticação do repositório
                return _utilizadorRepository.AutenticarUtilizador(codigo, password);
            }
            catch (System.Exception)
            {
                // Em caso de erro, retorna null (credenciais inválidas)
                // O erro será propagado na camada de apresentação
                throw;
            }
        }

        /// <summary>
        /// Obtém um utilizador pelo código (sem verificar password).
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

        /// <summary>
        /// Verifica se um utilizador existe na base de dados.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>True se o utilizador existir.</returns>
        public bool UtilizadorExiste(string codigo)
        {
            return _utilizadorRepository.UtilizadorExiste(codigo);
        }

        /// <summary>
        /// Testa a ligação à base de dados.
        /// </summary>
        /// <returns>True se a ligação estiver disponível.</returns>
        public bool TestarLigacao()
        {
            return _utilizadorRepository.TestarLigacao();
        }
    }
}