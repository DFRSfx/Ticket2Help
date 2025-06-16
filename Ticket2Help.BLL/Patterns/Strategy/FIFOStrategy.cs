using System.Collections.Generic;
using System.Linq;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Strategy
{
    /// <summary>
    /// Estratégia de atendimento First In, First Out (FIFO).
    /// </summary>
    /// <remarks>
    /// Ordena os tickets por ordem de criação, atendendo primeiro
    /// os tickets mais antigos.
    /// </remarks>
    public class FIFOStrategy : IAtendimentoStrategy
    {
        /// <summary>
        /// Ordena tickets por data de criação (mais antigos primeiro).
        /// </summary>
        /// <param name="tickets">Lista de tickets.</param>
        /// <returns>Tickets ordenados por data de criação ascendente.</returns>
        public IEnumerable<Ticket> OrdenarTicketsParaAtendimento(IEnumerable<Ticket> tickets)
        {
            return tickets
                .Where(t => t.Estado == EstadoTicket.porAtender)
                .OrderBy(t => t.DataHoraCriacao);
        }

        /// <summary>
        /// Retorna o nome da estratégia.
        /// </summary>
        /// <returns>"First In, First Out (FIFO)"</returns>
        public string GetNomeEstrategia()
        {
            return "First In, First Out (FIFO)";
        }
    }
}