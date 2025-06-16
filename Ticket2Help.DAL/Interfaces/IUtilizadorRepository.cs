using System.Collections.Generic;
using Ticket2Help.Models;

namespace Ticket2Help.DAL.Interfaces
{
    /// <summary>
    /// Interface para operações de dados relacionadas com utilizadores.
    /// </summary>
    public interface IUtilizadorRepository
    {
        /// <summary>
        /// Obtém um utilizador pelo código.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <returns>Utilizador encontrado ou null.</returns>
        Utilizador ObterPorCodigo(string codigo);

        /// <summary>
        /// Cria um novo utilizador.
        /// </summary>
        /// <param name="utilizador">Utilizador a criar.</param>
        void Criar(Utilizador utilizador);

        /// <summary>
        /// Actualiza dados de um utilizador.
        /// </summary>
        /// <param name="utilizador">Utilizador com dados actualizados.</param>
        void Actualizar(Utilizador utilizador);

        /// <summary>
        /// Obtém todos os utilizadores activos.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        IEnumerable<Utilizador> ObterTodos();
    }
}