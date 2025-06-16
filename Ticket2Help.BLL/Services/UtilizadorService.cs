using System.Collections.Generic;
using Ticket2Help.Models;
using Ticket2Help.DAL.Interfaces;

namespace Ticket2Help.BLL.Services
{
    /// <summary>
    /// Serviço para gestão de utilizadores.
    /// </summary>
    public class UtilizadorService
    {
        private readonly IUtilizadorRepository _UtilizadorRepository;

        /// <summary>
        /// Construtor do serviço de utilizadores.
        /// </summary>
        /// <param name="UtilizadorRepository">Repositório de utilizadores.</param>
        public UtilizadorService(IUtilizadorRepository UtilizadorRepository)
        {
            _UtilizadorRepository = UtilizadorRepository;
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

            // Para demonstração, aceitar qualquer senha para utilizadores existentes
            var Utilizador = _UtilizadorRepository.ObterPorCodigo(codigo);

            // Em produção, implementar validação real de senha hash
            return Utilizador;
        }

        /// <summary>
        /// Obtém um utilizador pelo código.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        public Utilizador ObterPorCodigo(string codigo)
        {
            return _UtilizadorRepository.ObterPorCodigo(codigo);
        }

        /// <summary>
        /// Obtém todos os utilizadores.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        public IEnumerable<Utilizador> ObterTodos()
        {
            return _UtilizadorRepository.ObterTodos();
        }

        /// <summary>
        /// Cria um novo utilizador.
        /// </summary>
        /// <param name="Utilizador">Utilizador a criar.</param>
        public void CriarUtilizador(Utilizador Utilizador)
        {
            _UtilizadorRepository.Criar(Utilizador);
        }
    }
}