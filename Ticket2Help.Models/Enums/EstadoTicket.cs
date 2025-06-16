using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Estados possíveis de um ticket no sistema.
    /// </summary>
    public enum EstadoTicket
    {
        /// <summary>
        /// Ticket criado e aguarda atendimento.
        /// </summary>
        porAtender,

        /// <summary>
        /// Ticket em processo de atendimento.
        /// </summary>
        emAtendimento,

        /// <summary>
        /// Ticket atendido e finalizado.
        /// </summary>
        atendido
    }
}
