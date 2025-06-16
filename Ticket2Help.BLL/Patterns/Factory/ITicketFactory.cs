using System.Collections.Generic;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Factory
{
    /// <summary>
    /// Interface para a fábrica de criação de tickets.
    /// </summary>
    public interface ITicketFactory
    {
        /// <summary>
        /// Cria um ticket do tipo especificado com os dados fornecidos.
        /// </summary>
        /// <param name="tipo">Tipo de ticket a criar.</param>
        /// <param name="dados">Dados necessários para criação do ticket.</param>
        /// <returns>Nova instância de ticket criada.</returns>
        Ticket CriarTicket(TipoTicket tipo, Dictionary<string, object> dados);
    }
}
