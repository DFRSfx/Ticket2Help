using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Tipos de ticket disponíveis no sistema.
    /// </summary>
    public enum TipoTicket
    {
        /// <summary>
        /// Ticket relacionado com problemas de hardware.
        /// </summary>
        Hardware,

        /// <summary>
        /// Ticket relacionado com problemas de software.
        /// </summary>
        Software
    }
}
