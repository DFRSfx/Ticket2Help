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
        /// Usuário responsável pelo atendimento do ticket.
        /// </summary>
        public string? UsuarioResponsavel { get; set; }

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

        /// <summary>
        /// Verifica se o ticket pode ser atendido.
        /// </summary>
        /// <returns>True se o ticket estiver no estado "porAtender".</returns>
        public bool PodeSerAtendido()
        {
            return Estado == EstadoTicket.porAtender;
        }

        /// <summary>
        /// Verifica se o ticket está em atendimento.
        /// </summary>
        /// <returns>True se o ticket estiver no estado "emAtendimento".</returns>
        public bool EstaEmAtendimento()
        {
            return Estado == EstadoTicket.emAtendimento;
        }

        /// <summary>
        /// Verifica se o ticket foi atendido.
        /// </summary>
        /// <returns>True se o ticket estiver no estado "atendido".</returns>
        public bool FoiAtendido()
        {
            return Estado == EstadoTicket.atendido;
        }

        /// <summary>
        /// Calcula o tempo decorrido desde a criação até o atendimento (ou até agora se ainda não foi atendido).
        /// </summary>
        /// <returns>TimeSpan com o tempo decorrido.</returns>
        public TimeSpan CalcularTempoEspera()
        {
            DateTime dataFinal = DataHoraAtendimento ?? DateTime.Now;
            return dataFinal - DataHoraCriacao;
        }

        /// <summary>
        /// Obtém uma descrição do estado atual do ticket de forma amigável.
        /// </summary>
        /// <returns>String descritiva do estado.</returns>
        public string GetDescricaoEstado()
        {
            return Estado switch
            {
                EstadoTicket.porAtender => "Aguardando Atendimento",
                EstadoTicket.emAtendimento => $"Em Atendimento{(string.IsNullOrEmpty(UsuarioResponsavel) ? "" : $" por {UsuarioResponsavel}")}",
                EstadoTicket.atendido => EstadoAtendimento switch
                {
                    Models.EstadoAtendimento.resolvido => "Atendido - Resolvido",
                    Models.EstadoAtendimento.naoResolvido => "Atendido - Não Resolvido",
                    _ => "Atendido"
                },
                _ => "Estado Desconhecido"
            };
        }
    }
}