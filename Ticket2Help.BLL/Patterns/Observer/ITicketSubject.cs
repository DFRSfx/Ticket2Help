using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Observer
{
    /// <summary>
    /// Interface para o sujeito no padrão Observer.
    /// </summary>
    public interface ITicketSubject
    {
        /// <summary>
        /// Adiciona um observador à lista de notificações.
        /// </summary>
        /// <param name="observer">Observador a adicionar.</param>
        void AdicionarObserver(ITicketObserver observer);

        /// <summary>
        /// Remove um observador da lista de notificações.
        /// </summary>
        /// <param name="observer">Observador a remover.</param>
        void RemoverObserver(ITicketObserver observer);

        /// <summary>
        /// Notifica todos os observadores sobre mudança de estado.
        /// </summary>
        /// <param name="ticket">Ticket alterado.</param>
        /// <param name="estadoAnterior">Estado anterior.</param>
        void NotificarObservers(Ticket ticket, EstadoTicket estadoAnterior);
    }
}