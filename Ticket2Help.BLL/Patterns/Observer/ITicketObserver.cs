using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Observer
{
    /// <summary>
    /// Interface para observadores de mudanças de estado dos tickets.
    /// </summary>
    public interface ITicketObserver
    {
        /// <summary>
        /// Método chamado quando o estado de um ticket é alterado.
        /// </summary>
        /// <param name="ticket">Ticket que teve o estado alterado.</param>
        /// <param name="estadoAnterior">Estado anterior do ticket.</param>
        void OnTicketEstadoAlterado(Ticket ticket, EstadoTicket estadoAnterior);
    }
}