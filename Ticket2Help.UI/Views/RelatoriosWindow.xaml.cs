using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Ticket2Help.DAL.Repositories;

namespace Ticket2Help.UI.Views
{
    /// <summary>
    /// Lógica para RelatoriosWindow.xaml
    /// </summary>
    public partial class RelatoriosWindow : Window
    {
        private readonly RelatorioRepository _relatorioRepository;
        private IEnumerable<TicketRelatorioDto> _dadosActuais;

        public RelatoriosWindow()
        {
            InitializeComponent();
            _relatorioRepository = new RelatorioRepository();

            InicializarInterface();
            ConfigurarEventos();
        }

        /// <summary>
        /// Configurações iniciais da interface
        /// </summary>
        private void InicializarInterface()
        {
            // Definir datas padrão (último mês)
            DatePickerFim.SelectedDate = DateTime.Today;
            DatePickerInicio.SelectedDate = DateTime.Today.AddDays(-30);

            // Actualizar período exibido
            ActualizarPeriodoExibido();

            // Carregar dados iniciais
            Loaded += RelatoriosWindow_Loaded;
        }

        /// <summary>
        /// Configura os eventos da janela
        /// </summary>
        private void ConfigurarEventos()
        {
            // Eventos dos DatePickers
            DatePickerInicio.SelectedDateChanged += DatePicker_SelectedDateChanged;
            DatePickerFim.SelectedDateChanged += DatePicker_SelectedDateChanged;

            // Eventos do DataGrid
            DataGridRelatorio.MouseDoubleClick += DataGridRelatorio_MouseDoubleClick;

            // Evento de fechar janela
            Closing += RelatoriosWindow_Closing;
        }

        private void RelatoriosWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Gerar relatório automaticamente para o período padrão
            BtnGerarRelatorio_Click(sender, e);
        }

        /// <summary>
        /// Actualiza o texto do período seleccionado
        /// </summary>
        private void ActualizarPeriodoExibido()
        {
            if (DatePickerInicio.SelectedDate.HasValue && DatePickerFim.SelectedDate.HasValue)
            {
                LblPeriodoSelecionado.Text = $"{DatePickerInicio.SelectedDate.Value:dd/MM/yyyy} a {DatePickerFim.SelectedDate.Value:dd/MM/yyyy}";
            }
            else
            {
                LblPeriodoSelecionado.Text = "Seleccione o período";
            }
        }

        /// <summary>
        /// Evento de clique do botão Gerar Relatório
        /// </summary>
        private void BtnGerarRelatorio_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDatas())
                return;

            try
            {
                // Estado de carregamento
                DefinirEstadoCarregamento(true);

                var dataInicio = DatePickerInicio.SelectedDate.Value;
                var dataFim = DatePickerFim.SelectedDate.Value.AddDays(1).AddMilliseconds(-1); // Fim do dia

                // Gerar relatório
                _dadosActuais = _relatorioRepository.ObterRelatorioDetalhado(dataInicio, dataFim);
                DataGridRelatorio.ItemsSource = _dadosActuais;

                // Actualizar estatísticas e interface
                ActualizarEstatisticas();
                ActualizarInterfaceResultados();

                LblUltimaActualizacao.Text = DateTime.Now.ToString("HH:mm:ss");

                if (!_dadosActuais.Any())
                {
                    MessageBox.Show("Não foram encontrados tickets no período seleccionado.",
                        "Sem Dados", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Relatório gerado com sucesso! Encontrados {_dadosActuais.Count()} tickets.",
                        "Relatório Gerado", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                DefinirEstadoCarregamento(false);
            }
        }

        /// <summary>
        /// Define o estado de carregamento da interface
        /// </summary>
        private void DefinirEstadoCarregamento(bool carregando)
        {
            BtnGerarRelatorio.IsEnabled = !carregando;
            BtnExportarExcel.IsEnabled = !carregando;
            DatePickerInicio.IsEnabled = !carregando;
            DatePickerFim.IsEnabled = !carregando;

            if (carregando)
            {
                BtnGerarRelatorio.Content = "🔄 A gerar...";
                LoadingIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                BtnGerarRelatorio.Content = "📈 Gerar Relatório";
                LoadingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Actualiza a interface baseada nos resultados
        /// </summary>
        private void ActualizarInterfaceResultados()
        {
            if (_dadosActuais == null || !_dadosActuais.Any())
            {
                NoDataPanel.Visibility = Visibility.Visible;
                DataGridRelatorio.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoDataPanel.Visibility = Visibility.Collapsed;
                DataGridRelatorio.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Valida as datas seleccionadas
        /// </summary>
        private bool ValidarDatas()
        {
            if (!DatePickerInicio.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, seleccione a data de início.",
                    "Data Obrigatória", MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerInicio.Focus();
                return false;
            }

            if (!DatePickerFim.SelectedDate.HasValue)
            {
                MessageBox.Show("Por favor, seleccione a data de fim.",
                    "Data Obrigatória", MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerFim.Focus();
                return false;
            }

            var dataInicio = DatePickerInicio.SelectedDate.Value;
            var dataFim = DatePickerFim.SelectedDate.Value;

            if (dataInicio > dataFim)
            {
                MessageBox.Show("A data de início deve ser anterior à data de fim.",
                    "Período Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePickerInicio.Focus();
                return false;
            }

            var diasDiferenca = (dataFim - dataInicio).TotalDays;
            if (diasDiferenca > 365)
            {
                var resultado = MessageBox.Show(
                    "O período seleccionado é superior a 1 ano. Isto pode tornar o relatório muito lento. Deseja continuar?",
                    "Período Extenso", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Actualiza as estatísticas do relatório
        /// </summary>
        private void ActualizarEstatisticas()
        {
            var total = _dadosActuais.Count();
            LblTotalRegistros.Text = total.ToString();
            LblTotalRegistrosHeader.Text = $"{total} registos encontrados";
            ActualizarPeriodoExibido();
        }

        /// <summary>
        /// Evento de clique do botão Exportar Excel
        /// </summary>
        private void BtnExportarExcel_Click(object sender, RoutedEventArgs e)
        {
            if (_dadosActuais == null || !_dadosActuais.Any())
            {
                MessageBox.Show("Não há dados para exportar. Gere um relatório primeiro.",
                    "Sem Dados", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Title = "Exportar Relatório",
                Filter = "Ficheiros CSV (*.csv)|*.csv|Ficheiros de Texto (*.txt)|*.txt|Todos os ficheiros (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"Relatorio_Tickets_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    BtnExportarExcel.IsEnabled = false;
                    BtnExportarExcel.Content = "📤 A exportar...";

                    ExportarParaCSV(saveDialog.FileName);

                    var resultado = MessageBox.Show(
                        $"Relatório exportado com sucesso!\n\nFicheiro: {Path.GetFileName(saveDialog.FileName)}\nLocalização: {Path.GetDirectoryName(saveDialog.FileName)}\n\nDeseja abrir a pasta?",
                        "Exportação Concluída",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (resultado == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao exportar: {ex.Message}",
                        "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    BtnExportarExcel.IsEnabled = true;
                    BtnExportarExcel.Content = "📊 Exportar Excel";
                }
            }
        }

        /// <summary>
        /// Exporta os dados para ficheiro CSV
        /// </summary>
        private void ExportarParaCSV(string caminho)
        {
            var csv = new StringBuilder();

            // Cabeçalho
            csv.AppendLine("ID;Tipo;Colaborador;Nome;Descrição;Data Criação;Estado;Data Atendimento;Estado Atendimento;Tempo (h)");

            // Dados
            foreach (var item in _dadosActuais)
            {
                var linha = string.Join(";",
                    item.Id,
                    EscaparCSV(item.TipoTicket),
                    EscaparCSV(item.CodigoColaborador),
                    EscaparCSV(item.NomeColaborador),
                    EscaparCSV(item.DescricaoCompleta),
                    item.DataHoraCriacao.ToString("dd/MM/yyyy HH:mm"),
                    EscaparCSV(item.Estado),
                    item.DataHoraAtendimento?.ToString("dd/MM/yyyy HH:mm") ?? "",
                    EscaparCSV(item.EstadoAtendimento),
                    item.TempoAtendimentoHoras?.ToString() ?? ""
                );
                csv.AppendLine(linha);
            }

            // Adicionar estatísticas no final
            csv.AppendLine();
            csv.AppendLine("=== ESTATÍSTICAS ===");
            csv.AppendLine($"Total de Tickets;{_dadosActuais.Count()}");
            csv.AppendLine($"Tickets Hardware;{_dadosActuais.Count(t => t.TipoTicket == "Hardware")}");
            csv.AppendLine($"Tickets Software;{_dadosActuais.Count(t => t.TipoTicket == "Software")}");
            csv.AppendLine($"Tickets Atendidos;{_dadosActuais.Count(t => t.Estado == "atendido")}");
            csv.AppendLine($"Tickets Resolvidos;{_dadosActuais.Count(t => t.EstadoAtendimento == "resolvido")}");
            csv.AppendLine($"Data de Geração;{DateTime.Now:dd/MM/yyyy HH:mm:ss}");

            // Escrever ficheiro com encoding UTF-8 com BOM para suporte de caracteres especiais
            var encoding = new UTF8Encoding(true);
            File.WriteAllText(caminho, csv.ToString(), encoding);
        }

        /// <summary>
        /// Escapa caracteres especiais para CSV
        /// </summary>
        private string EscaparCSV(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return "";

            // Remover quebras de linha e substituir por espaços
            valor = valor.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

            // Substituir ponto e vírgula por vírgula para não quebrar CSV
            valor = valor.Replace(";", ",");

            // Remover aspas duplas
            valor = valor.Replace("\"", "'");

            return valor;
        }

        /// <summary>
        /// Evento quando as datas mudam
        /// </summary>
        private void DatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ActualizarPeriodoExibido();
        }

        /// <summary>
        /// Duplo clique no DataGrid para ver detalhes
        /// </summary>
        private void DataGridRelatorio_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataGridRelatorio.SelectedItem is TicketRelatorioDto ticket)
            {
                var detalhes = $"DETALHES DO TICKET #{ticket.Id}\n\n" +
                              $"Tipo: {ticket.TipoTicket}\n" +
                              $"Colaborador: {ticket.CodigoColaborador} - {ticket.NomeColaborador}\n" +
                              $"Descrição: {ticket.DescricaoCompleta}\n" +
                              $"Data de Criação: {ticket.DataHoraCriacao:dd/MM/yyyy HH:mm}\n" +
                              $"Estado: {ticket.Estado}\n";

                if (ticket.DataHoraAtendimento.HasValue)
                {
                    detalhes += $"Data de Atendimento: {ticket.DataHoraAtendimento.Value:dd/MM/yyyy HH:mm}\n";
                    detalhes += $"Estado do Atendimento: {ticket.EstadoAtendimento}\n";

                    if (ticket.TempoAtendimentoHoras.HasValue)
                    {
                        detalhes += $"Tempo de Atendimento: {ticket.TempoAtendimentoHoras.Value} horas\n";
                    }
                }

                MessageBox.Show(detalhes, "Detalhes do Ticket", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Fechar janela
        /// </summary>
        private void RelatoriosWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cleanup se necessário
        }
    }
}