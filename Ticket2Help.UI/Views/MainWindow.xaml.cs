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
    /// Lógica para MainWindow.xaml - VERSÃO FINAL DE PRODUÇÃO
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TicketController _ticketController;
        private Utilizador? _utilizadorActual;
        private DispatcherTimer? _timer;

        // Evento para notificar logout
        public event EventHandler? LogoutRequested;

        public MainWindow()
        {
            InitializeComponent();
            _ticketController = new TicketController();

            InicializarInterface();
            InicializarTimer();
        }

        public void DefinirUtilizador(Utilizador? utilizador)
        {
            if (utilizador == null)
            {
                MessageBox.Show("Erro: Utilizador não pode ser nulo.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // Limpar dados do utilizador anterior
            LimparDadosInterface();

            _utilizadorActual = utilizador;

            // Actualizar interface do utilizador
            LblUtilizadorNome.Text = utilizador.Nome;
            LblUtilizadorTipo.Text = utilizador.EhTecnicoHelpdesk ? "Técnico Helpdesk" : "Colaborador";

            // Configurar visibilidade dos tabs baseado no tipo de utilizador
            if (!utilizador.EhTecnicoHelpdesk)
            {
                TabAtendimento.Visibility = Visibility.Collapsed;
                TabDashboard.Visibility = Visibility.Collapsed;

                // Voltar para o tab "Meus Tickets" se estiver noutro tab
                MainTabControl.SelectedIndex = 0;
            }
            else
            {
                TabAtendimento.Visibility = Visibility.Visible;
                TabDashboard.Visibility = Visibility.Visible;
            }

            // Carregar dados iniciais
            CarregarDados();
        }

        private void LimparDadosInterface()
        {
            try
            {
                // Limpar DataGrids
                DataGridMeusTickets.ItemsSource = null;
                DataGridTicketsAtendimento.ItemsSource = null;

                // Limpar métricas do dashboard
                LblTotalHoje.Text = "0";
                LblPendentes.Text = "0";
                LblEmAtendimento.Text = "0";
                LblTicketsAtendidos.Text = "0%";
                LblTicketsResolvidos.Text = "0%";
                LblTicketsNaoResolvidos.Text = "0%";
                LblMediaHardware.Text = "0h";
                LblMediaSoftware.Text = "0h";

                // Reset barras de progresso
                ProgressResolucaoFill.Width = 0;
                ProgressAtendimentoFill.Width = 0;
                LblProgressResolucao.Text = "0% resolvidos";
                LblProgressAtendimento.Text = "0% atendidos";

                // Atualizar status
                LblStatus.Text = "Trocando utilizador...";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao limpar interface: {ex}");
            }
        }

        private void InicializarInterface()
        {
            LblUltimaActualizacao.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void InicializarTimer()
        {
            // Parar timer existente se houver
            _timer?.Stop();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(2) // Actualizar a cada 2 minutos
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
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
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados: {ex}");
            }
        }

        private void CarregarMeusTickets()
        {
            if (_utilizadorActual == null) return;

            try
            {
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar meus tickets: {ex}");
                // Em caso de erro, manter dados anteriores ou carregar dados de fallback
            }
        }

        private void CarregarTicketsParaAtendimento()
        {
            try
            {
                var tickets = _ticketController.ObterTicketsParaAtendimento();
                DataGridTicketsAtendimento.ItemsSource = tickets;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar tickets para atendimento: {ex}");
            }
        }

        private void CarregarDashboard()
        {
            try
            {
                var dataInicio = DateTime.Today.AddDays(-30); // Últimos 30 dias
                var dataFim = DateTime.Now;

                var dashboard = _ticketController.ObterDadosDashboard(dataInicio, dataFim);

                // Actualizar métricas básicas
                LblTotalHoje.Text = dashboard.TotalTicketsHoje.ToString();
                LblPendentes.Text = dashboard.TicketsPendentes.ToString();
                LblEmAtendimento.Text = dashboard.TicketsEmAtendimento.ToString();

                // Formatação correta das percentagens
                LblTicketsAtendidos.Text = FormatarPercentagem(dashboard.PercentagemTicketsAtendidos);
                LblTicketsResolvidos.Text = FormatarPercentagem(dashboard.PercentagemTicketsResolvidos);
                LblTicketsNaoResolvidos.Text = FormatarPercentagem(dashboard.PercentagemTicketsNaoResolvidos);

                // Formatação das médias de tempo
                LblMediaHardware.Text = FormatarTempo(dashboard.MediaTempoAtendimentoHardware);
                LblMediaSoftware.Text = FormatarTempo(dashboard.MediaTempoAtendimentoSoftware);

                // Actualizar barras de progresso com largura fixa
                ActualizarBarraProgresso(ProgressResolucaoFill, LblProgressResolucao,
                    dashboard.PercentagemTicketsResolvidos, "resolvidos", 300);
                ActualizarBarraProgresso(ProgressAtendimentoFill, LblProgressAtendimento,
                    dashboard.PercentagemTicketsAtendidos, "atendidos", 300);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dashboard: {ex}");
                // Definir valores padrão em caso de erro
                DefinirValoresPadraoDashboard();
            }
        }

        /// <summary>
        /// Definir valores padrão em caso de erro
        /// </summary>
        private void DefinirValoresPadraoDashboard()
        {
            LblTotalHoje.Text = "0";
            LblPendentes.Text = "0";
            LblEmAtendimento.Text = "0";
            LblTicketsAtendidos.Text = "0,0%";
            LblTicketsResolvidos.Text = "0,0%";
            LblTicketsNaoResolvidos.Text = "0,0%";
            LblMediaHardware.Text = "0,0h";
            LblMediaSoftware.Text = "0,0h";

            ProgressResolucaoFill.Width = 0;
            ProgressAtendimentoFill.Width = 0;
            LblProgressResolucao.Text = "0% resolvidos";
            LblProgressAtendimento.Text = "0% atendidos";
        }

        /// <summary>
        /// Formatar percentagem com uma casa decimal
        /// </summary>
        private string FormatarPercentagem(double valor)
        {
            if (double.IsNaN(valor) || double.IsInfinity(valor))
                return "0,0%";

            return $"{valor:F1}%";
        }

        /// <summary>
        /// Formatar tempo em horas com uma casa decimal
        /// </summary>
        private string FormatarTempo(double horas)
        {
            if (double.IsNaN(horas) || double.IsInfinity(horas))
                return "0,0h";

            if (horas < 1)
                return $"{(horas * 60):F0}m";
            else if (horas >= 24)
                return $"{(horas / 24):F1}d";
            else
                return $"{horas:F1}h";
        }

        /// <summary>
        /// Actualizar barra de progresso com largura fixa
        /// </summary>
        private void ActualizarBarraProgresso(Border barra, TextBlock label,
            double percentagem, string sufixo, double larguraMaxima)
        {
            try
            {
                // Garantir que a percentagem está entre 0 e 100
                percentagem = Math.Max(0, Math.Min(100, percentagem));

                var larguraActual = (percentagem / 100.0) * larguraMaxima;
                barra.Width = larguraActual;
                label.Text = $"{percentagem:F1}% {sufixo}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao actualizar barra de progresso: {ex}");
                barra.Width = 0;
                label.Text = $"0% {sufixo}";
            }
        }

        #region Event Handlers

        private void BtnCriarTicketHardware_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_utilizadorActual == null) return;

                var criarWindow = new CriarTicketWindow(_utilizadorActual, TipoTicket.Hardware);
                if (criarWindow.ShowDialog() == true)
                {
                    CarregarMeusTickets();
                    MessageBox.Show("Ticket de hardware criado com sucesso!", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar ticket: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCriarTicketSoftware_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_utilizadorActual == null) return;

                var criarWindow = new CriarTicketWindow(_utilizadorActual, TipoTicket.Software);
                if (criarWindow.ShowDialog() == true)
                {
                    CarregarMeusTickets();
                    MessageBox.Show("Ticket de software criado com sucesso!", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar ticket: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atender ticket: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DataGridTicketsAtendimento_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir ticket: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboEstrategiaAtendimento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboEstrategiaAtendimento.SelectedItem is ComboBoxItem item)
                {
                    var estrategia = item.Tag?.ToString() ?? "FIFO";
                    _ticketController.AlterarEstrategiaAtendimento(estrategia);
                    CarregarTicketsParaAtendimento();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao alterar estratégia: {ex}");
            }
        }

        private void BtnGerarMapas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var relatoriosWindow = new RelatoriosWindow();
                relatoriosWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir relatórios: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show("Tem a certeza que deseja sair da sessão atual?",
                "Confirmar Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Parar timer e limpar dados
                    _timer?.Stop();
                    _utilizadorActual = null;

                    // Limpar interface
                    LimparDadosInterface();

                    // Notificar App sobre o logout
                    LogoutRequested?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro durante o logout: {ex.Message}", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Se a janela está sendo fechada mas a aplicação não está sendo encerrada,
            // significa que é um logout, não um fechamento
            if (Application.Current.MainWindow == this && this.IsVisible)
            {
                var resultado = MessageBox.Show("Tem a certeza que deseja fechar o Ticket2Help?\n\nPara trocar de utilizador, use o botão 'Sair' no canto superior direito.",
                    "Confirmar Encerramento", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _timer?.Stop();
            base.OnClosing(e);
        }
    }
}