using System;
using System.Collections.Generic;
using System.Windows;
using Ticket2Help.UI.Controllers;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Views
{
    /// <summary>
    /// Lógica para CriarTicketWindow.xaml
    /// </summary>
    public partial class CriarTicketWindow : Window
    {
        private readonly TicketController _ticketController;
        private readonly Utilizador _utilizador;
        private readonly TipoTicket _tipoTicket;

        public CriarTicketWindow(Utilizador? utilizador, TipoTicket tipoTicket)
        {
            if (utilizador == null)
            {
                throw new ArgumentNullException(nameof(utilizador), "Utilizador não pode ser nulo.");
            }

            InitializeComponent();
            _ticketController = new TicketController();
            _utilizador = utilizador;
            _tipoTicket = tipoTicket;

            InicializarInterface();
        }

        private void InicializarInterface()
        {
            // Configurar informações do colaborador
            LblColaborador.Text = $"{_utilizador.Codigo} - {_utilizador.Nome}";
            LblDataHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // Configurar interface baseada no tipo de ticket
            if (_tipoTicket == TipoTicket.Hardware)
            {
                LblTitulo.Text = "Criar Ticket de Hardware";
                LblSubtitulo.Text = "Problemas com equipamentos físicos";
                LblIcone.Text = "🔧";
                PanelHardware.Visibility = Visibility.Visible;
                TxtEquipamento.Focus();
            }
            else
            {
                LblTitulo.Text = "Criar Ticket de Software";
                LblSubtitulo.Text = "Problemas com aplicações e sistemas";
                LblIcone.Text = "💻";
                PanelSoftware.Visibility = Visibility.Visible;
                TxtSoftware.Focus();
            }
        }

        private void BtnCriar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos())
                return;

            try
            {
                BtnCriar.IsEnabled = false;
                BtnCriar.Content = "🔄 A criar...";

                var dados = new Dictionary<string, object>
                {
                    ["codigoColaborador"] = _utilizador.Codigo
                };

                if (_tipoTicket == TipoTicket.Hardware)
                {
                    dados["equipamento"] = TxtEquipamento.Text.Trim();
                    dados["avaria"] = TxtAvaria.Text.Trim();
                }
                else
                {
                    dados["software"] = TxtSoftware.Text.Trim();
                    dados["descricaoNecessidade"] = TxtDescricaoNecessidade.Text.Trim();
                }

                var sucesso = _ticketController.CriarTicket(_tipoTicket, dados);

                if (sucesso)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Erro ao criar o ticket. Verifique os dados e tente novamente.",
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
                BtnCriar.IsEnabled = true;
                BtnCriar.Content = "✅ Criar Ticket";
            }
        }

        private bool ValidarCampos()
        {
            if (_tipoTicket == TipoTicket.Hardware)
            {
                if (string.IsNullOrWhiteSpace(TxtEquipamento.Text))
                {
                    MessageBox.Show("Por favor, indique o equipamento.",
                        "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtEquipamento.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TxtAvaria.Text))
                {
                    MessageBox.Show("Por favor, descreva a avaria.",
                        "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtAvaria.Focus();
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(TxtSoftware.Text))
                {
                    MessageBox.Show("Por favor, indique o software.",
                        "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtSoftware.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(TxtDescricaoNecessidade.Text))
                {
                    MessageBox.Show("Por favor, descreva a necessidade.",
                        "Campo Obrigatório", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TxtDescricaoNecessidade.Focus();
                    return false;
                }
            }

            return true;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Tem a certeza que deseja cancelar? Os dados inseridos serão perdidos.",
                "Confirmar Cancelamento", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}