using System;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Observer
{
    /// <summary>
    /// Observador concreto para notificações na interface gráfica.
    /// </summary>
    public class UINotificationObserver : ITicketObserver
    {
        /// <summary>
        /// Evento para notificar a interface gráfica.
        /// </summary>
        public event Action<string> NotificacaoUI;

        /// <summary>
        /// Notifica a interface sobre mudança de estado.
        /// </summary>
        /// <param name="ticket">Ticket alterado.</param>
        /// <param name="estadoAnterior">Estado anterior.</param>
        public void OnTicketEstadoAlterado(Ticket ticket, EstadoTicket estadoAnterior)
        {
            var mensagem = $"Ticket #{ticket.Id} actualizado: {estadoAnterior} → {ticket.Estado}";
            NotificacaoUI?.Invoke(mensagem);
        }
    }
}