using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket2Help.Models;

namespace Ticket2Help.Models
{
    /// <summary>
    /// Classe base abstracta para todos os tipos de tickets no sistema.
    /// Implementa o padrão Template Method para operações comuns.
    /// </summary>
    /// <remarks>
    /// Esta classe define a estrutura básica de um ticket e força
    /// as classes filhas a implementar métodos específicos.
    /// </remarks>
    public abstract class Ticket
    {
        /// <summary>
        /// Identificador único do ticket.
        /// </summary>
        /// <value>Número inteiro gerado automaticamente pela base de dados.</value>
        public int Id { get; set; }

        /// <summary>
        /// Código do colaborador que submeteu o ticket.
        /// </summary>
        public required string CodigoColaborador { get; set; }

        /// <summary>
        /// Data e hora de criação do ticket.
        /// </summary>
        public DateTime DataHoraCriacao { get; set; }

        /// <summary>
        /// Estado actual do ticket.
        /// </summary>
        public EstadoTicket Estado { get; set; }

        /// <summary>
        /// Data e hora do atendimento do ticket.
        /// </summary>
        public DateTime? DataHoraAtendimento { get; set; }

        /// <summary>
        /// Estado do atendimento do ticket.
        /// </summary>
        public EstadoAtendimento? EstadoAtendimento { get; set; }

        /// <summary>
        /// Construtor protegido que define valores padrão.
        /// </summary>
        protected Ticket()
        {
            DataHoraCriacao = DateTime.Now;
            Estado = EstadoTicket.porAtender;
        }

        /// <summary>
        /// Obtém a descrição completa do ticket formatada.
        /// </summary>
        /// <returns>String com a descrição formatada específica do tipo de ticket.</returns>
        /// <example>
        /// Para um HardwareTicket: "Hardware - Computador Dell: Não liga"
        /// Para um SoftwareTicket: "Software - Office: Instalação necessária"
        /// </example>
        public abstract string GetDescricaoCompleta();

        /// <summary>
        /// Obtém o tipo específico do ticket.
        /// </summary>
        /// <returns>Tipo do ticket (Hardware ou Software).</returns>
        public abstract TipoTicket GetTipoTicket();
    }
}