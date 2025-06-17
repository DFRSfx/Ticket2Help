using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Ticket2Help.BLL.Patterns.Factory;
using Ticket2Help.Models;

namespace Ticket2Help.Tests
{
    /// <summary>
    /// Testes unitários para a classe base Ticket e suas classes derivadas.
    /// </summary>
    [TestClass]
    public class TicketTests
    {
        #region Testes da Classe Base Ticket

        [TestMethod]
        public void Ticket_ConstrucaoPadrao_DeveTerValoresCorretos()
        {
            // Arrange & Act
            var hardwareTicket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste de avaria"
            };

            // Assert
            Assert.AreEqual(EstadoTicket.porAtender, hardwareTicket.Estado);
            Assert.IsTrue(hardwareTicket.DataHoraCriacao <= DateTime.Now);
            Assert.IsTrue(hardwareTicket.DataHoraCriacao >= DateTime.Now.AddMinutes(-1));
            Assert.IsNull(hardwareTicket.DataHoraAtendimento);
            Assert.IsNull(hardwareTicket.EstadoAtendimento);
            Assert.IsNull(hardwareTicket.UsuarioResponsavel);
        }

        [TestMethod]
        public void Ticket_PodeSerAtendido_RetornaTrueQuandoPorAtender()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                Estado = EstadoTicket.porAtender
            };

            // Act & Assert
            Assert.IsTrue(ticket.PodeSerAtendido());
        }

        [TestMethod]
        public void Ticket_PodeSerAtendido_RetornaFalseQuandoEmAtendimento()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                Estado = EstadoTicket.emAtendimento
            };

            // Act & Assert
            Assert.IsFalse(ticket.PodeSerAtendido());
        }

        [TestMethod]
        public void Ticket_EstaEmAtendimento_RetornaTrueQuandoEmAtendimento()
        {
            // Arrange
            var ticket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                DescricaoNecessidade = "Instalação",
                Estado = EstadoTicket.emAtendimento
            };

            // Act & Assert
            Assert.IsTrue(ticket.EstaEmAtendimento());
        }

        [TestMethod]
        public void Ticket_FoiAtendido_RetornaTrueQuandoAtendido()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                Estado = EstadoTicket.atendido
            };

            // Act & Assert
            Assert.IsTrue(ticket.FoiAtendido());
        }

        [TestMethod]
        public void Ticket_CalcularTempoEspera_RetornaTempoCorreto()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddHours(-2);
            var dataAtendimento = DateTime.Now.AddHours(-1);

            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                DataHoraCriacao = dataInicio,
                DataHoraAtendimento = dataAtendimento
            };

            // Act
            var tempoEspera = ticket.CalcularTempoEspera();

            // Assert
            Assert.AreEqual(1, Math.Round(tempoEspera.TotalHours));
        }

        [TestMethod]
        public void Ticket_CalcularTempoEspera_SemDataAtendimento_UsaDataAtual()
        {
            // Arrange
            var dataInicio = DateTime.Now.AddMinutes(-30);
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                DataHoraCriacao = dataInicio
            };

            // Act
            var tempoEspera = ticket.CalcularTempoEspera();

            // Assert
            Assert.IsTrue(tempoEspera.TotalMinutes >= 29 && tempoEspera.TotalMinutes <= 31);
        }

        [TestMethod]
        public void Ticket_GetDescricaoEstado_RetornaDescricaoCorreta()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                Estado = EstadoTicket.porAtender
            };

            // Act & Assert
            Assert.AreEqual("Aguardando Atendimento", ticket.GetDescricaoEstado());

            // Teste estado em atendimento
            ticket.Estado = EstadoTicket.emAtendimento;
            ticket.UsuarioResponsavel = "TEC001";
            Assert.AreEqual("Em Atendimento por TEC001", ticket.GetDescricaoEstado());

            // Teste estado atendido resolvido
            ticket.Estado = EstadoTicket.atendido;
            ticket.EstadoAtendimento = EstadoAtendimento.resolvido;
            Assert.AreEqual("Atendido - Resolvido", ticket.GetDescricaoEstado());
        }

        #endregion

        #region Testes da Classe HardwareTicket

        [TestMethod]
        public void HardwareTicket_GetTipoTicket_RetornaHardware()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "PC não liga"
            };

            // Act & Assert
            Assert.AreEqual(TipoTicket.Hardware, ticket.GetTipoTicket());
        }

        [TestMethod]
        public void HardwareTicket_GetDescricaoCompleta_RetornaFormatoCorreto()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Equipamento = "Dell OptiPlex",
                Avaria = "Não liga"
            };

            // Act
            var descricao = ticket.GetDescricaoCompleta();

            // Assert
            Assert.AreEqual("Hardware - Dell OptiPlex: Não liga", descricao);
        }

        [TestMethod]
        public void HardwareTicket_GetDescricaoCompleta_SemEquipamento_RetornaComNull()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Equipamento = null,
                Avaria = "Problema desconhecido"
            };

            // Act
            var descricao = ticket.GetDescricaoCompleta();

            // Assert
            Assert.AreEqual("Hardware - : Problema desconhecido", descricao);
        }

        [TestMethod]
        public void HardwareTicket_PropriedadesEspecificas_DeveManterValores()
        {
            // Arrange & Act
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Equipamento = "Impressora HP",
                Avaria = "Papel encravado",
                DescricaoReparacao = "Removido papel",
                Pecas = "Nenhuma"
            };

            // Assert
            Assert.AreEqual("Impressora HP", ticket.Equipamento);
            Assert.AreEqual("Papel encravado", ticket.Avaria);
            Assert.AreEqual("Removido papel", ticket.DescricaoReparacao);
            Assert.AreEqual("Nenhuma", ticket.Pecas);
        }

        #endregion

        #region Testes da Classe SoftwareTicket

        [TestMethod]
        public void SoftwareTicket_GetTipoTicket_RetornaSoftware()
        {
            // Arrange
            var ticket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                DescricaoNecessidade = "Instalação do Office"
            };

            // Act & Assert
            Assert.AreEqual(TipoTicket.Software, ticket.GetTipoTicket());
        }

        [TestMethod]
        public void SoftwareTicket_GetDescricaoCompleta_RetornaFormatoCorreto()
        {
            // Arrange
            var ticket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                Software = "Microsoft Office",
                DescricaoNecessidade = "Instalação e configuração"
            };

            // Act
            var descricao = ticket.GetDescricaoCompleta();

            // Assert
            Assert.AreEqual("Software - Microsoft Office: Instalação e configuração", descricao);
        }

        [TestMethod]
        public void SoftwareTicket_PropriedadesEspecificas_DeveManterValores()
        {
            // Arrange & Act
            var ticket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                Software = "Adobe Acrobat",
                DescricaoNecessidade = "Problema com PDF",
                DescricaoIntervencao = "Reinstalado o software"
            };

            // Assert
            Assert.AreEqual("Adobe Acrobat", ticket.Software);
            Assert.AreEqual("Problema com PDF", ticket.DescricaoNecessidade);
            Assert.AreEqual("Reinstalado o software", ticket.DescricaoIntervencao);
        }

        #endregion

        #region Testes de Validação

        [TestMethod]
        public void HardwareTicket_CamposObrigatorios_DevemSerDefinidos()
        {
            // Arrange & Act
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste de avaria"
            };

            // Assert - Campos obrigatórios definidos
            Assert.IsNotNull(ticket.CodigoColaborador);
            Assert.IsNotNull(ticket.Avaria);
            Assert.AreEqual("COL001", ticket.CodigoColaborador);
            Assert.AreEqual("Teste de avaria", ticket.Avaria);
        }

        [TestMethod]
        public void SoftwareTicket_CamposObrigatorios_DevemSerDefinidos()
        {
            // Arrange & Act
            var ticket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                DescricaoNecessidade = "Instalação necessária"
            };

            // Assert - Campos obrigatórios definidos
            Assert.IsNotNull(ticket.CodigoColaborador);
            Assert.IsNotNull(ticket.DescricaoNecessidade);
            Assert.AreEqual("COL002", ticket.CodigoColaborador);
            Assert.AreEqual("Instalação necessária", ticket.DescricaoNecessidade);
        }

        [TestMethod]
        public void Ticket_CamposOpcionais_PodemSerNulos()
        {
            // Arrange & Act
            var hardwareTicket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste",
                Equipamento = null, // Campo opcional
                DescricaoReparacao = null, // Campo opcional
                Pecas = null // Campo opcional
            };

            var softwareTicket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                DescricaoNecessidade = "Teste",
                Software = null, // Campo opcional
                DescricaoIntervencao = null // Campo opcional
            };

            // Assert
            Assert.IsNull(hardwareTicket.Equipamento);
            Assert.IsNull(hardwareTicket.DescricaoReparacao);
            Assert.IsNull(hardwareTicket.Pecas);
            Assert.IsNull(softwareTicket.Software);
            Assert.IsNull(softwareTicket.DescricaoIntervencao);
        }

        #endregion

        #region Testes de Estados e Fluxo

        [TestMethod]
        public void Ticket_FluxoDeEstados_DeveSequenciarCorrectamente()
        {
            // Arrange
            var ticket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Avaria = "Teste de fluxo"
            };

            // Act & Assert - Estado inicial
            Assert.AreEqual(EstadoTicket.porAtender, ticket.Estado);
            Assert.IsTrue(ticket.PodeSerAtendido());
            Assert.IsFalse(ticket.EstaEmAtendimento());
            Assert.IsFalse(ticket.FoiAtendido());

            // Iniciar atendimento
            ticket.Estado = EstadoTicket.emAtendimento;
            ticket.UsuarioResponsavel = "TEC001";
            ticket.DataHoraAtendimento = DateTime.Now;

            Assert.IsFalse(ticket.PodeSerAtendido());
            Assert.IsTrue(ticket.EstaEmAtendimento());
            Assert.IsFalse(ticket.FoiAtendido());

            // Finalizar atendimento
            ticket.Estado = EstadoTicket.atendido;
            ticket.EstadoAtendimento = EstadoAtendimento.resolvido;

            Assert.IsFalse(ticket.PodeSerAtendido());
            Assert.IsFalse(ticket.EstaEmAtendimento());
            Assert.IsTrue(ticket.FoiAtendido());
        }

        [TestMethod]
        public void Ticket_EstadosAtendimento_DeveManterValoresCorretos()
        {
            // Arrange
            var ticket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                DescricaoNecessidade = "Teste",
                Estado = EstadoTicket.atendido
            };

            // Act & Assert - Resolvido
            ticket.EstadoAtendimento = EstadoAtendimento.resolvido;
            Assert.AreEqual("Atendido - Resolvido", ticket.GetDescricaoEstado());

            // Não resolvido
            ticket.EstadoAtendimento = EstadoAtendimento.naoResolvido;
            Assert.AreEqual("Atendido - Não Resolvido", ticket.GetDescricaoEstado());

            // Aberto
            ticket.EstadoAtendimento = EstadoAtendimento.aberto;
            Assert.AreEqual("Atendido", ticket.GetDescricaoEstado());
        }

        #endregion

        #region Testes de Performance e Limites

        [TestMethod]
        public void Ticket_CriacaoMultipla_DeveManterPerformance()
        {
            // Arrange
            var tickets = new List<Ticket>();
            var startTime = DateTime.Now;

            // Act - Criar 1000 tickets
            for (int i = 0; i < 1000; i++)
            {
                if (i % 2 == 0)
                {
                    tickets.Add(new HardwareTicket
                    {
                        CodigoColaborador = $"COL{i:D3}",
                        Equipamento = $"PC{i}",
                        Avaria = $"Problema {i}"
                    });
                }
                else
                {
                    tickets.Add(new SoftwareTicket
                    {
                        CodigoColaborador = $"COL{i:D3}",
                        Software = $"Software{i}",
                        DescricaoNecessidade = $"Necessidade {i}"
                    });
                }
            }

            var endTime = DateTime.Now;

            // Assert
            Assert.AreEqual(1000, tickets.Count);
            Assert.IsTrue((endTime - startTime).TotalSeconds < 5); // Deve criar 1000 tickets em menos de 5 segundos
        }

        [TestMethod]
        public void Ticket_TextosLongos_DeveAceitarSemErro()
        {
            // Arrange
            var textoLongo = new string('A', 1000);

            // Act
            var hardwareTicket = new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Equipamento = textoLongo,
                Avaria = textoLongo,
                DescricaoReparacao = textoLongo,
                Pecas = textoLongo
            };

            var softwareTicket = new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                Software = textoLongo,
                DescricaoNecessidade = textoLongo,
                DescricaoIntervencao = textoLongo
            };

            // Assert
            Assert.AreEqual(textoLongo, hardwareTicket.Equipamento);
            Assert.AreEqual(textoLongo, hardwareTicket.Avaria);
            Assert.AreEqual(textoLongo, softwareTicket.Software);
            Assert.AreEqual(textoLongo, softwareTicket.DescricaoNecessidade);
        }

        #endregion

        #region Testes de Integração com Factory

        [TestMethod]
        public void TicketFactory_CriarHardwareTicket_DeveRetornarTipoCorreto()
        {
            // Arrange
            var factory = new TicketFactory();
            var dados = new Dictionary<string, object>
            {
                ["codigoColaborador"] = "COL001",
                ["equipamento"] = "Dell Laptop",
                ["avaria"] = "Tela quebrada"
            };

            // Act
            var ticket = factory.CriarTicket(TipoTicket.Hardware, dados);

            // Assert
            Assert.IsInstanceOfType(ticket, typeof(HardwareTicket));
            Assert.AreEqual(TipoTicket.Hardware, ticket.GetTipoTicket());

            var hwTicket = ticket as HardwareTicket;
            Assert.AreEqual("COL001", hwTicket.CodigoColaborador);
            Assert.AreEqual("Dell Laptop", hwTicket.Equipamento);
            Assert.AreEqual("Tela quebrada", hwTicket.Avaria);
        }

        [TestMethod]
        public void TicketFactory_CriarSoftwareTicket_DeveRetornarTipoCorreto()
        {
            // Arrange
            var factory = new TicketFactory();
            var dados = new Dictionary<string, object>
            {
                ["codigoColaborador"] = "COL002",
                ["software"] = "Microsoft Excel",
                ["descricaoNecessidade"] = "Macro não funciona"
            };

            // Act
            var ticket = factory.CriarTicket(TipoTicket.Software, dados);

            // Assert
            Assert.IsInstanceOfType(ticket, typeof(SoftwareTicket));
            Assert.AreEqual(TipoTicket.Software, ticket.GetTipoTicket());

            var swTicket = ticket as SoftwareTicket;
            Assert.AreEqual("COL002", swTicket.CodigoColaborador);
            Assert.AreEqual("Microsoft Excel", swTicket.Software);
            Assert.AreEqual("Macro não funciona", swTicket.DescricaoNecessidade);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TicketFactory_TipoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var factory = new TicketFactory();
            var dados = new Dictionary<string, object>();

            // Act
            factory.CriarTicket((TipoTicket)999, dados); // Tipo inválido

            // Assert - Exceção esperada
        }

        #endregion
    }
}

// Classe auxiliar para testes de mock se necessário
namespace Ticket2Help.Tests.Helpers
{
    /// <summary>
    /// Classe auxiliar para testes que necessitem de dados mock.
    /// </summary>
    public static class TicketTestHelper
    {
        /// <summary>
        /// Cria um HardwareTicket padrão para testes.
        /// </summary>
        public static HardwareTicket CriarHardwareTicketPadrao()
        {
            return new HardwareTicket
            {
                CodigoColaborador = "COL001",
                Equipamento = "PC Dell",
                Avaria = "Não liga"
            };
        }

        /// <summary>
        /// Cria um SoftwareTicket padrão para testes.
        /// </summary>
        public static SoftwareTicket CriarSoftwareTicketPadrao()
        {
            return new SoftwareTicket
            {
                CodigoColaborador = "COL002",
                Software = "Microsoft Office",
                DescricaoNecessidade = "Instalação necessária"
            };
        }

        /// <summary>
        /// Cria múltiplos tickets para testes de performance.
        /// </summary>
        public static List<Ticket> CriarMultiplosTickets(int quantidade)
        {
            var tickets = new List<Ticket>();

            for (int i = 0; i < quantidade; i++)
            {
                if (i % 2 == 0)
                {
                    tickets.Add(new HardwareTicket
                    {
                        CodigoColaborador = $"COL{i:D3}",
                        Equipamento = $"Equipamento {i}",
                        Avaria = $"Problema {i}"
                    });
                }
                else
                {
                    tickets.Add(new SoftwareTicket
                    {
                        CodigoColaborador = $"COL{i:D3}",
                        Software = $"Software {i}",
                        DescricaoNecessidade = $"Necessidade {i}"
                    });
                }
            }

            return tickets;
        }
    }
}