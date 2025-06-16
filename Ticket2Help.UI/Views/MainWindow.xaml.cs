using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Ticket2Help.UI.Controllers;
using Ticket2Help.UI.ViewModels;
using Ticket2Help.UI.Views;
using Ticket2Help.Models;

namespace Ticket2Help.UI
{
    /// <summary>
    /// Lógica para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TicketController _ticketController;
        private Utilizador _utilizadorActual;
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _ticketController = new TicketController();
            InicializarInterface();
            InicializarTimer();
        }

        public void DefinirUtilizador(Utilizador utilizador)
        {
            _utilizadorActual = utilizador;
            LblUtilizadorNome.Text = utilizador.Nome;
            LblUtilizadorTipo.Text = utilizador.EhTecnicoHelpdesk ? "Técnico Helpdesk" : "Colaborador";

            // Configurar visibilidade dos tabs baseado no tipo de utilizador
            if (!utilizador.EhTecnicoHelpdesk)
            {
                TabAtendimento.Visibility = Visibility.Collapsed;
                TabDashboard.Visibility = Visibility.Collapsed;
            }

            CarregarDados();
        }

        private void InicializarInterface()
        {
            LblUltimaActualizacao.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void InicializarTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(2) // Actualizar a cada 2 minutos
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CarregarDados();
        }

        private void CarregarDados()
        {
            try
            {
                CarregarMeusTickets();

                if (_utilizadorActual?.EhTecnicoHelpdesk == true)
                {
                    CarregarTicketsParaAtendimento();
                    CarregarDashboard();
                }

                LblUltimaActualizacao.Text = DateTime.Now.ToString("HH:mm:ss");
                LblStatus.Text = "Dados actualizados com sucesso";
            }
            catch (Exception ex)
            {
                LblStatus.Text = $"Erro ao carregar dados: {ex.Message}";
            }
        }

        private void CarregarMeusTickets()
        {
            if (_utilizadorActual == null) return;

            var tickets = _ticketController.ObterTicketsDoColaborador(_utilizadorActual.Codigo);

            // Adicionar tempo de espera
            foreach (var ticket in tickets)
            {
                if (ticket.DataAtendimento.HasValue)
                {
                    var tempo = ticket.DataAtendimento.Value - ticket.DataCriacao;
                    ticket.TempoEspera = $"{tempo.Days}d {tempo.Hours}h {tempo.Minutes}m";
                }
                else
                {
                    var tempo = DateTime.Now - ticket.DataCriacao;
                    ticket.TempoEspera = $"{tempo.Days}d {tempo.Hours}h {tempo.Minutes}m";
                }
            }

            DataGridMeusTickets.ItemsSource = tickets;
        }

        private void CarregarTicketsParaAtendimento()
        {
            var tickets = _ticketController.ObterTicketsParaAtendimento();
            DataGridTicketsAtendimento.ItemsSource = tickets;
        }

        private void CarregarDashboard()
        {
            var dataInicio = DateTime.Today.AddDays(-30); // Últimos 30 dias
            var dataFim = DateTime.Now;

            var dashboard = _ticketController.ObterDadosDashboard(dataInicio, dataFim);

            // Actualizar métricas
            LblTotalHoje.Text = dashboard.TotalTicketsHoje.ToString();
            LblPendentes.Text = dashboard.TicketsPendentes.ToString();
            LblEmAtendimento.Text = dashboard.TicketsEmAtendimento.ToString();
            LblTicketsAtendidos.Text = $"{dashboard.PercentagemTicketsAtendidos:F1}%";
            LblTicketsResolvidos.Text = $"{dashboard.PercentagemTicketsResolvidos:F1}%";
            LblTicketsNaoResolvidos.Text = $"{dashboard.PercentagemTicketsNaoResolvidos:F1}%";
            LblMediaHardware.Text = $"{dashboard.MediaTempoAtendimentoHardware:F1}h";
            LblMediaSoftware.Text = $"{dashboard.MediaTempoAtendimentoSoftware:F1}h";

            // Actualizar barras de progresso
            ActualizarBarraProgresso(ProgressResolucaoFill, LblProgressResolucao,
                dashboard.PercentagemTicketsResolvidos, "resolvidos");
            ActualizarBarraProgresso(ProgressAtendimentoFill, LblProgressAtendimento,
                dashboard.PercentagemTicketsAtendidos, "atendidos");
        }

        private void ActualizarBarraProgresso(System.Windows.Shapes.Rectangle barra, TextBlock label,
            double percentagem, string sufixo)
        {
            var larguraMaxima = 300; // Largura máxima da barra
            var larguraActual = (percentagem / 100.0) * larguraMaxima;

            barra.Width = larguraActual;
            label.Text = $"{percentagem:F1}% {sufixo}";
        }

        // Event Handlers
        private void BtnCriarTicketHardware_Click(object sender, RoutedEventArgs e)
        {
            var criarWindow = new CriarTicketWindow(_utilizadorActual, TipoTicket.Hardware);
            if (criarWindow.ShowDialog() == true)
            {
                CarregarMeusTickets();
                MessageBox.Show("Ticket de hardware criado com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCriarTicketSoftware_Click(object sender, RoutedEventArgs e)
        {
            var criarWindow = new CriarTicketWindow(_utilizadorActual, TipoTicket.Software);
            if (criarWindow.ShowDialog() == true)
            {
                CarregarMeusTickets();
                MessageBox.Show("Ticket de software criado com sucesso!", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnActualizarDados_Click(object sender, RoutedEventArgs e)
        {
            CarregarDados();
        }

        private void BtnActualizarAtendimento_Click(object sender, RoutedEventArgs e)
        {
            CarregarTicketsParaAtendimento();
        }

        private void BtnAtenderTicket_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int ticketId)
            {
                var atenderWindow = new AtenderTicketWindow(ticketId);
                if (atenderWindow.ShowDialog() == true)
                {
                    CarregarTicketsParaAtendimento();
                    CarregarDashboard();
                    MessageBox.Show("Ticket atendido com sucesso!", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DataGridTicketsAtendimento_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridTicketsAtendimento.SelectedItem is TicketViewModel ticket)
            {
                var atenderWindow = new AtenderTicketWindow(ticket.Id);
                if (atenderWindow.ShowDialog() == true)
                {
                    CarregarTicketsParaAtendimento();
                    CarregarDashboard();
                }
            }
        }

        private void ComboEstrategiaAtendimento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboEstrategiaAtendimento.SelectedItem is ComboBoxItem item)
            {
                var estrategia = item.Tag.ToString();
                _ticketController.AlterarEstrategiaAtendimento(estrategia);
                CarregarTicketsParaAtendimento();
            }
        }

        private void BtnGerarMapas_Click(object sender, RoutedEventArgs e)
        {
            var relatoriosWindow = new RelatoriosWindow();
            relatoriosWindow.ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Tem a certeza que deseja sair do sistema?",
                "Confirmar Saída", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                _timer?.Stop();
                Application.Current.Shutdown();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}