using System.Collections.Generic;
using System.Linq;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Strategy
{
    /// <summary>
    /// Estratégia que dá prioridade aos tickets de hardware.
    /// </summary>
    /// <remarks>
    /// Ordena os tickets colocando todos os de hardware primeiro,
    /// seguidos pelos de software, mantendo ordem cronológica dentro de cada tipo.
    /// </remarks>
    public class PrioridadeHardwareStrategy : IAtendimentoStrategy
    {
        /// <summary>
        /// Ordena tickets dando prioridade ao hardware.
        /// </summary>
        /// <param name="tickets">Lista de tickets.</param>
        /// <returns>Tickets com hardware primeiro, depois software.</returns>
        public IEnumerable<Ticket> OrdenarTicketsParaAtendimento(IEnumerable<Ticket> tickets)
        {
            return tickets
                .Where(t => t.Estado == EstadoTicket.porAtender)
                .OrderBy(t => t.GetTipoTicket() == TipoTicket.Software ? 1 : 0)
                .ThenBy(t => t.DataHoraCriacao);
        }

        /// <summary>
        /// Retorna o nome da estratégia.
        /// </summary>
        /// <returns>"Prioridade Hardware"</returns>
        public string GetNomeEstrategia()
        {
            return "Prioridade Hardware";
        }
    }
}