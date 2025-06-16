using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Representa um utilizador do sistema.
    /// </summary>
    /// <remarks>
    /// Utilizadores podem ser colaboradores normais ou técnicos de helpdesk,
    /// com diferentes permissões baseadas na propriedade EhTecnicoHelpdesk.
    /// </remarks>
    public class Utilizador
    {
        /// <summary>
        /// Código único do colaborador.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Nome completo do utilizador.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Endereço de email do utilizador.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Indica se o utilizador é técnico de helpdesk.
        /// </summary>
        /// <value>
        /// true se for técnico de helpdesk, false para colaborador normal.
        /// </value>
        public bool EhTecnicoHelpdesk { get; set; }
    }
}