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
    /// Lógica para AtenderTicketWindow.xaml
    /// </summary>
    public partial class AtenderTicketWindow : Window
    {
        private readonly TicketController _ticketController;
        private readonly int _ticketId;
        private Ticket _ticket;

        public AtenderTicketWindow(int ticketId)
        {
            InitializeComponent();
            _ticketController = new TicketController();
            _ticketId = ticketId;

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

                PreencherInformacoes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar ticket: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
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
                PanelAtendimentoHardware.Visibility = Visibility.Visible;
                PanelAtendimentoSoftware.Visibility = Visibility.Collapsed;

                LblEquipamento.Text = hw.Equipamento;
                LblAvaria.Text = hw.Avaria;
            }
            else if (_ticket is SoftwareTicket sw)
            {
                PanelHardware.Visibility = Visibility.Collapsed;
                PanelSoftware.Visibility = Visibility.Visible;
                PanelAtendimentoHardware.Visibility = Visibility.Collapsed;
                PanelAtendimentoSoftware.Visibility = Visibility.Visible;

                LblSoftware.Text = sw.Software;
                LblNecessidade.Text = sw.DescricaoNecessidade;
            }

            // Seleccionar estado padrão
            ComboEstadoAtendimento.SelectedIndex = 0; // Resolvido por padrão
        }

        private void BtnAtender_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                BtnAtender.IsEnabled = false;
                BtnAtender.Content = "A processar...";

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

                var sucesso = _ticketController.AtenderTicket(_ticketId, dadosAtendimento);

                if (sucesso)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Erro ao atender o ticket. Tente novamente.",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                BtnAtender.Content = "Finalizar Atendimento";
            }
        }

        private bool ValidarCampos()
        {
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

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Tem a certeza que deseja cancelar o atendimento?",
                "Confirmar Cancelamento", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}