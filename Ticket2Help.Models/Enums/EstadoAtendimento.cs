using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Estados do atendimento de um ticket.
    /// </summary>
    public enum EstadoAtendimento
    {
        /// <summary>
        /// Atendimento iniciado mas não finalizado.
        /// </summary>
        aberto,

        /// <summary>
        /// Problema resolvido com sucesso.
        /// </summary>
        resolvido,

        /// <summary>
        /// Problema não foi possível resolver.
        /// </summary>
        naoResolvido
    }
}
