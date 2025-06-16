using System;
using System.Collections.Generic;
using Ticket2Help.Models;

namespace Ticket2Help.BLL.Patterns.Factory
{
    /// <summary>
    /// Implementação do padrão Factory para criação de tickets.
    /// </summary>
    /// <remarks>
    /// Este padrão permite a criação de diferentes tipos de tickets
    /// (Hardware ou Software) sem que o cliente precise conhecer
    /// as classes concretas específicas.
    /// 
    /// <b>Padrão Utilizado:</b> Factory Method
    /// <b>Objectivo:</b> Encapsular a criação de objectos
    /// <b>Benefícios:</b>
    /// - Desacoplamento do código cliente
    /// - Facilita adição de novos tipos de ticket
    /// - Centraliza a lógica de criação
    /// </remarks>
    /// <example>
    /// <code>
    /// var factory = new TicketFactory();
    /// var dados = new Dictionary&lt;string, object&gt;
    /// {
    ///     ["codigoColaborador"] = "COL001",
    ///     ["equipamento"] = "PC",
    ///     ["avaria"] = "Não liga"
    /// };
    /// var ticket = factory.CriarTicket(TipoTicket.Hardware, dados);
    /// </code>
    /// </example>
    public class TicketFactory : ITicketFactory
    {
        /// <summary>
        /// Cria um ticket do tipo especificado.
        /// </summary>
        /// <param name="tipo">Tipo de ticket (Hardware ou Software).</param>
        /// <param name="dados">Dicionário com os dados necessários.</param>
        /// <returns>Nova instância de ticket.</returns>
        /// <exception cref="ArgumentException">Lançada quando o tipo é inválido.</exception>
        public Ticket CriarTicket(TipoTicket tipo, Dictionary<string, object> dados)
        {
            switch (tipo)
            {
                case TipoTicket.Hardware:
                    return new HardwareTicket
                    {
                        CodigoColaborador = dados["codigoColaborador"].ToString(),
                        Equipamento = dados["equipamento"].ToString(),
                        Avaria = dados["avaria"].ToString()
                    };

                case TipoTicket.Software:
                    return new SoftwareTicket
                    {
                        CodigoColaborador = dados["codigoColaborador"].ToString(),
                        Software = dados["software"].ToString(),
                        DescricaoNecessidade = dados["descricaoNecessidade"].ToString()
                    };

                default:
                    throw new ArgumentException("Tipo de ticket inválido", nameof(tipo));
            }
        }
    }
}
