using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Ticket2Help.UI.Controllers;
using Ticket2Help.Models;
using Ticket2Help.DAL.Repositories;

namespace Ticket2Help.UI.Views
{
    /// <summary>
    /// Lógica para AtenderTicketWindow.xaml - VERSÃO CORRIGIDA
    /// </summary>
    public partial class AtenderTicketWindow : Window
    {
        private readonly TicketController _ticketController;
        private readonly int _ticketId;
        private Ticket _ticket;
        private readonly string _usuarioResponsavel;
        private bool _ticketJaEmAtendimento = false;

        public AtenderTicketWindow(int ticketId, string usuarioResponsavel = "SISTEMA")
        {
            InitializeComponent();
            _ticketController = new TicketController();
            _ticketId = ticketId;
            _usuarioResponsavel = usuarioResponsavel;

            CarregarTicket();
        }

        private void CarregarTicket()
        {
            try
            {
                var ticketRepository = new TicketRepository();
                _ticket = ticketRepository.ObterPorId(_ticketId);

                if (_ticket == null)
                {
                    MessageBox.Show("Ticket não encontrado.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // Verificar se o ticket já está em atendimento
                _ticketJaEmAtendimento = _ticket.Estado == EstadoTicket.emAtendimento;

                PreencherInformacoes();
                ConfigurarInterface();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar ticket: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void ConfigurarInterface()
        {
            if (_ticketJaEmAtendimento)
            {
                // Ticket já está em atendimento - permitir finalização
                BtnAtender.Content = "✅ Finalizar Atendimento";
                Title = $"Finalizar Atendimento - Ticket #{_ticket.Id}";
            }
            else
            {
                // Ticket ainda não foi iniciado - permitir início
                BtnAtender.Content = "🔧 Iniciar Atendimento";
                Title = $"Iniciar Atendimento - Ticket #{_ticket.Id}";

                // Ocultar campos de finalização até iniciar
                ComboEstadoAtendimento.Visibility = Visibility.Collapsed;
                PanelAtendimentoHardware.Visibility = Visibility.Collapsed;
                PanelAtendimentoSoftware.Visibility = Visibility.Collapsed;

                // Adicionar texto explicativo
                var lblInfo = new TextBlock
                {
                    Text = "Clique em 'Iniciar Atendimento' para começar a trabalhar neste ticket.",
                    FontStyle = FontStyles.Italic,
                    Margin = new Thickness(0, 10, 0, 0),
                    TextWrapping = TextWrapping.Wrap
                };

                // Encontrar o StackPanel principal e adicionar a informação
                if (Content is Grid grid && grid.Children[2] is ScrollViewer scrollViewer &&
                    scrollViewer.Content is StackPanel stackPanel)
                {
                    stackPanel.Children.Insert(0, lblInfo);
                }
            }
        }

        private void PreencherInformacoes()
        {
            // Cabeçalho
            LblNumeroTicket.Text = $"Ticket #{_ticket.Id}";
            LblTipoTicket.Text = _ticket.GetTipoTicket().ToString();

            // Informações gerais
            LblColaborador.Text = _ticket.CodigoColaborador;
            LblDataCriacao.Text = _ticket.DataHoraCriacao.ToString("dd/MM/yyyy HH:mm");
            LblDescricao.Text = _ticket.GetDescricaoCompleta();

            // Campos específicos por tipo
            if (_ticket is HardwareTicket hw)
            {
                PanelHardware.Visibility = Visibility.Visible;
                PanelSoftware.Visibility = Visibility.Collapsed;

                LblEquipamento.Text = hw.Equipamento;
                LblAvaria.Text = hw.Avaria;

                if (_ticketJaEmAtendimento)
                {
                    PanelAtendimentoHardware.Visibility = Visibility.Visible;
                    PanelAtendimentoSoftware.Visibility = Visibility.Collapsed;

                    // Preencher dados existentes se houver
                    TxtDescricaoReparacao.Text = hw.DescricaoReparacao ?? "";
                    TxtPecas.Text = hw.Pecas ?? "";
                }
            }
            else if (_ticket is SoftwareTicket sw)
            {
                PanelHardware.Visibility = Visibility.Collapsed;
                PanelSoftware.Visibility = Visibility.Visible;

                LblSoftware.Text = sw.Software;
                LblNecessidade.Text = sw.DescricaoNecessidade;

                if (_ticketJaEmAtendimento)
                {
                    PanelAtendimentoHardware.Visibility = Visibility.Collapsed;
                    PanelAtendimentoSoftware.Visibility = Visibility.Visible;

                    // Preencher dados existentes se houver
                    TxtDescricaoIntervencao.Text = sw.DescricaoIntervencao ?? "";
                }
            }

            // Seleccionar estado padrão se estiver finalizando
            if (_ticketJaEmAtendimento)
            {
                ComboEstadoAtendimento.Visibility = Visibility.Visible;
                ComboEstadoAtendimento.SelectedIndex = 0; // Resolvido por padrão
            }
        }

        private void BtnAtender_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnAtender.IsEnabled = false;
                BtnAtender.Content = "🔄 A processar...";

                if (!_ticketJaEmAtendimento)
                {
                    // Iniciar atendimento
                    IniciarAtendimento();
                }
                else
                {
                    // Finalizar atendimento
                    FinalizarAtendimento();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnAtender.IsEnabled = true;
                RestaurarTextoBotao();
            }
        }

        private void IniciarAtendimento()
        {
            var sucesso = _ticketController.IniciarAtendimento(_ticketId, _usuarioResponsavel);

            if (sucesso)
            {
                MessageBox.Show($"Atendimento iniciado com sucesso!\n\nTicket #{_ticketId} está agora em atendimento.",
                    "Atendimento Iniciado", MessageBoxButton.OK, MessageBoxImage.Information);

                // Recarregar ticket e reconfigurar interface
                CarregarTicket();
            }
            else
            {
                MessageBox.Show("Erro ao iniciar o atendimento. Verifique se o ticket está disponível.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FinalizarAtendimento()
        {
            if (!ValidarCamposFinalizacao())
                return;

            var dadosAtendimento = new Dictionary<string, object>();

            // Estado do atendimento
            var estadoItem = ComboEstadoAtendimento.SelectedItem as ComboBoxItem;
            dadosAtendimento["estadoAtendimento"] = estadoItem.Tag.ToString();

            // Campos específicos por tipo
            if (_ticket is HardwareTicket)
            {
                if (!string.IsNullOrWhiteSpace(TxtDescricaoReparacao.Text))
                    dadosAtendimento["descricaoReparacao"] = TxtDescricaoReparacao.Text.Trim();

                if (!string.IsNullOrWhiteSpace(TxtPecas.Text))
                    dadosAtendimento["pecas"] = TxtPecas.Text.Trim();
            }
            else if (_ticket is SoftwareTicket)
            {
                if (!string.IsNullOrWhiteSpace(TxtDescricaoIntervencao.Text))
                    dadosAtendimento["descricaoIntervencao"] = TxtDescricaoIntervencao.Text.Trim();
            }

            var sucesso = _ticketController.FinalizarAtendimento(_ticketId, dadosAtendimento);

            if (sucesso)
            {
                MessageBox.Show($"Atendimento finalizado com sucesso!\n\nTicket #{_ticketId} foi marcado como atendido.",
                    "Atendimento Finalizado", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Erro ao finalizar o atendimento. Tente novamente.",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarCamposFinalizacao()
        {
            if (!_ticketJaEmAtendimento)
                return true; // Não precisa validar se está apenas iniciando

            if (ComboEstadoAtendimento.SelectedItem == null)
            {
                MessageBox.Show("Por favor, seleccione o estado do atendimento.",
                    "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                ComboEstadoAtendimento.Focus();
                return false;
            }

            if (_ticket is HardwareTicket)
            {
                if (string.IsNullOrWhiteSpace(TxtDescricaoReparacao.Text))
                {
                    MessageBox.Show("Por favor, descreva a reparação efectuada.",
                        "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtDescricaoReparacao.Focus();
                    return false;
                }
            }
            else if (_ticket is SoftwareTicket)
            {
                if (string.IsNullOrWhiteSpace(TxtDescricaoIntervencao.Text))
                {
                    MessageBox.Show("Por favor, descreva a intervenção efectuada.",
                        "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtDescricaoIntervencao.Focus();
                    return false;
                }
            }

            return true;
        }

        private void RestaurarTextoBotao()
        {
            if (_ticketJaEmAtendimento)
            {
                BtnAtender.Content = "✅ Finalizar Atendimento";
            }
            else
            {
                BtnAtender.Content = "🔧 Iniciar Atendimento";
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Tem a certeza que deseja cancelar?",
                "Confirmar Cancelamento", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}