using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket2Help.Models;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Representa um ticket de suporte para problemas de software.
    /// </summary>
    /// <remarks>
    /// Esta classe estende a classe base Ticket para incluir
    /// campos específicos relacionados com aplicações e sistemas.
    /// </remarks>
    public class SoftwareTicket : Ticket
    {
        /// <summary>
        /// Nome do software que necessita de suporte.
        /// </summary>
        public string? Software { get; set; }

        /// <summary>
        /// Descrição detalhada da necessidade ou problema.
        /// </summary>
        public required string DescricaoNecessidade { get; set; }

        /// <summary>
        /// Descrição da intervenção efectuada pelo técnico.
        /// </summary>
        public string? DescricaoIntervencao { get; set; }

        /// <summary>
        /// Obtém a descrição completa formatada do ticket de software.
        /// </summary>
        /// <returns>Descrição no formato "Software - [Software]: [Necessidade]".</returns>
        public override string GetDescricaoCompleta()
        {
            return $"Software - {Software}: {DescricaoNecessidade}";
        }

        /// <summary>
        /// Retorna o tipo de ticket como Software.
        /// </summary>
        /// <returns>TipoTicket.Software</returns>
        public override TipoTicket GetTipoTicket()
        {
            return TipoTicket.Software;
        }
    }
}