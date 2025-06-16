using System.Collections.Generic;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Strategy
{
    /// <summary>
    /// Interface para estratégias de atendimento de tickets.
    /// </summary>
    public interface IAtendimentoStrategy
    {
        /// <summary>
        /// Ordena os tickets para atendimento segundo a estratégia específica.
        /// </summary>
        /// <param name="tickets">Lista de tickets a ordenar.</param>
        /// <returns>Tickets ordenados segundo a estratégia.</returns>
        IEnumerable<Ticket> OrdenarTicketsParaAtendimento(IEnumerable<Ticket> tickets);

        /// <summary>
        /// Obtém o nome da estratégia.
        /// </summary>
        /// <returns>Nome descritivo da estratégia.</returns>
        string GetNomeEstrategia();
    }
}