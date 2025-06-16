using System;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Observer
{
    /// <summary>
    /// Observador concreto para registo de logs de mudanças de estado.
    /// </summary>
    public class LogObserver : ITicketObserver
    {
        /// <summary>
        /// Regista a mudança de estado num log.
        /// </summary>
        /// <param name="ticket">Ticket alterado.</param>
        /// <param name="estadoAnterior">Estado anterior.</param>
        public void OnTicketEstadoAlterado(Ticket ticket, EstadoTicket estadoAnterior)
        {
            var mensagem = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] LOG: Ticket {ticket.Id} mudou de {estadoAnterior} para {ticket.Estado}";
            Console.WriteLine(mensagem);

            // Aqui poderia gravar em ficheiro, base de dados, etc.
            // System.IO.File.AppendAllText("ticket_changes.log", mensagem + Environment.NewLine);
        }
    }
}