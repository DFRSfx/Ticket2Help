using System.Collections.Generic;
using System.Net.Sockets;
using Ticket2Help.Models;

namespace Ticket2Help.DAL.Interfaces
{
    /// <summary>
    /// Interface para operações de dados relacionadas com tickets.
    /// </summary>
    public interface ITicketRepository
    {
        /// <summary>
        /// Cria um novo ticket na base de dados.
        /// </summary>
        /// <param name="ticket">Ticket a ser criado.</param>
        void Criar(Ticket ticket);

        /// <summary>
        /// Obtém um ticket pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do ticket.</param>
        /// <returns>Ticket encontrado ou null se não existir.</returns>
        Ticket? ObterPorId(int id);

        /// <summary>
        /// Obtém todos os tickets do sistema.
        /// </summary>
        /// <returns>Lista de todos os tickets.</returns>
        IEnumerable<Ticket> ObterTodos();

        /// <summary>
        /// Obtém tickets de um colaborador específico.
        /// </summary>
        /// <param name="codigoColaborador">Código do colaborador.</param>
        /// <returns>Lista de tickets do colaborador.</returns>
        IEnumerable<Ticket> ObterPorColaborador(string codigoColaborador);

        /// <summary>
        /// Obtém tickets por estado.
        /// </summary>
        /// <param name="estado">Estado dos tickets a obter.</param>
        /// <returns>Lista de tickets no estado especificado.</returns>
        IEnumerable<Ticket> ObterPorEstado(EstadoTicket estado);

        /// <summary>
        /// Actualiza um ticket existente.
        /// </summary>
        /// <param name="ticket">Ticket com dados actualizados.</param>
        void Actualizar(Ticket ticket);

        /// <summary>
        /// Elimina um ticket da base de dados.
        /// </summary>
        /// <param name="id">Identificador do ticket a eliminar.</param>
        void Eliminar(int id);
    }
}