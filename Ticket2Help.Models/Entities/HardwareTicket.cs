using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket2Help.Models;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Representa um ticket de suporte para problemas de hardware.
    /// </summary>
    /// <remarks>
    /// Esta classe estende a classe base Ticket para incluir
    /// campos específicos relacionados com equipamentos físicos.
    /// </remarks>
    public class HardwareTicket : Ticket
    {
        /// <summary>
        /// Nome ou tipo do equipamento com problema.
        /// </summary>
        public string? Equipamento { get; set; }

        /// <summary>
        /// Descrição detalhada da avaria ou problema.
        /// </summary>
        public required string Avaria { get; set; }

        /// <summary>
        /// Descrição da reparação efectuada pelo técnico.
        /// </summary>
        public string? DescricaoReparacao { get; set; }

        /// <summary>
        /// Lista de peças utilizadas na reparação.
        /// </summary>
        public string? Pecas { get; set; }

        /// <summary>
        /// Obtém a descrição completa formatada do ticket de hardware.
        /// </summary>
        /// <returns>Descrição no formato "Hardware - [Equipamento]: [Avaria]".</returns>
        public override string GetDescricaoCompleta()
        {
            return $"Hardware - {Equipamento}: {Avaria}";
        }

        /// <summary>
        /// Retorna o tipo de ticket como Hardware.
        /// </summary>
        /// <returns>TipoTicket.Hardware</returns>
        public override TipoTicket GetTipoTicket()
        {
            return TipoTicket.Hardware;
        }
    }
}