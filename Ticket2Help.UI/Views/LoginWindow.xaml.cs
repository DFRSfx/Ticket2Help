using System;
using System.Threading.Tasks;
using System.Windows;
using Ticket2Help.UI.Controllers;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Views
{
    /// <summary>
    /// Lógica para LoginWindow.xaml - Autenticação simples com código e password
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginController _loginController;

        /// <summary>
        /// Utilizador autenticado com sucesso
        /// </summary>
        public Utilizador UtilizadorAutenticado { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            _loginController = new LoginController();

            InicializarInterface();
            ConfigurarEventos();
        }

        /// <summary>
        /// Configurações iniciais da interface
        /// </summary>
        private void InicializarInterface()
        {
            // Focar no campo de código ao carregar
            Loaded += (s, e) => TxtCodigo.Focus();

            // Permitir arrastar a janela
            MouseLeftButtonDown += (s, e) => DragMove();

            // Testar ligação à BD ao carregar
            _ = TestarLigacaoInicial();
        }

        /// <summary>
        /// Configurar eventos dos controlos
        /// </summary>
        private void ConfigurarEventos()
        {
            // Permitir navegar com Enter
            TxtCodigo.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    TxtSenha.Focus();
                    e.Handled = true;
                }
            };

            TxtSenha.KeyDown += async (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    await ProcessarLoginAsync();
                    e.Handled = true;
                }
            };
        }

        /// <summary>
        /// Testa a ligação à base de dados quando a janela carrega
        /// </summary>
        private async Task TestarLigacaoInicial()
        {
            try
            {
                LblStatus.Text = "A testar ligação à base de dados...";

                bool ligacaoOK = await Task.Run(() => _loginController.TestarLigacaoBD());

                if (ligacaoOK)
                {
                    LblStatus.Text = "Sistema pronto";
                }
                else
                {
                    LblStatus.Text = "Aviso: Problemas de ligação à BD";
                    MostrarErro("Aviso: Não foi possível conectar à base de dados. Verifique se o SQL Server está a correr.");
                }
            }
            catch (Exception ex)
            {
                LblStatus.Text = "Erro de ligação";
                MostrarErro($"Erro ao testar ligação: {ex.Message}");
            }
        }

        /// <summary>
        /// Mostrar mensagem de erro
        /// </summary>
        private void MostrarErro(string mensagem)
        {
            LblErro.Text = mensagem;
            ErrorPanel.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Ocultar mensagem de erro
        /// </summary>
        private void OcultarErro()
        {
            ErrorPanel.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Validar campos de entrada
        /// </summary>
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                MostrarErro("Por favor, introduza o código do colaborador.");
                TxtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtSenha.Password))
            {
                MostrarErro("Por favor, introduza a palavra-passe.");
                TxtSenha.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Processar login de forma assíncrona
        /// </summary>
        private async Task ProcessarLoginAsync()
        {
            if (!ValidarCampos())
                return;

            // Desabilitar controlos durante o processamento
            DefinirEstadoProcessamento(true);
            OcultarErro();

            try
            {
                // Capturar valores antes da operação assíncrona
                string codigo = TxtCodigo.Text.Trim().ToUpper(); // Converter para maiúsculas
                string password = TxtSenha.Password;

                // Executar autenticação numa thread separada
                var utilizador = await Task.Run(() =>
                {
                    return _loginController.Autenticar(codigo, password);
                });

                // Processar resultado na UI thread
                if (utilizador != null)
                {
                    // Login bem-sucedido
                    UtilizadorAutenticado = utilizador;
                    BtnLogin.Content = "✅ Sucesso!";
                    LblStatus.Text = $"Bem-vindo, {utilizador.Nome}";

                    // Pequeno delay para mostrar o sucesso
                    await Task.Delay(800);

                    DialogResult = true;
                    Close();
                }
                else
                {
                    // Credenciais inválidas
                    MostrarErro("Código ou palavra-passe incorretos.");
                    TxtSenha.Clear();
                    TxtCodigo.SelectAll();
                    TxtCodigo.Focus();
                    LblStatus.Text = "Falha na autenticação";
                }
            }
            catch (Exception ex)
            {
                // Tratar erros
                string mensagemErro = ObterMensagemErroAmigavel(ex);
                MostrarErro(mensagemErro);
                LblStatus.Text = "Erro de sistema";

                // Log para debug
                System.Diagnostics.Debug.WriteLine($"Erro de login: {ex}");
            }
            finally
            {
                // Reactivar controlos
                DefinirEstadoProcessamento(false);
            }
        }

        /// <summary>
        /// Define o estado de processamento da interface
        /// </summary>
        private void DefinirEstadoProcessamento(bool processando)
        {
            BtnLogin.IsEnabled = !processando;
            BtnCancelar.IsEnabled = !processando;
            TxtCodigo.IsEnabled = !processando;
            TxtSenha.IsEnabled = !processando;

            if (processando)
            {
                BtnLogin.Content = "🔄 A verificar...";
            }
            else
            {
                BtnLogin.Content = "Iniciar Sessão";
            }
        }

        /// <summary>
        /// Converter excepções em mensagens user-friendly
        /// </summary>
        private string ObterMensagemErroAmigavel(Exception ex)
        {
            var mensagem = ex.Message.ToLower();

            if (mensagem.Contains("cannot open database") || mensagem.Contains("database") && mensagem.Contains("does not exist"))
            {
                return "Base de dados 'Ticket2Help' não encontrada. Contacte o administrador do sistema.";
            }
            else if (mensagem.Contains("server") || mensagem.Contains("network") || mensagem.Contains("timeout"))
            {
                return "Erro de ligação ao servidor. Verifique se o SQL Server está disponível.";
            }
            else if (mensagem.Contains("login failed") || mensagem.Contains("authentication failed"))
            {
                return "Falha na ligação à base de dados. Contacte o administrador do sistema.";
            }
            else if (mensagem.Contains("invalid object name") && mensagem.Contains("utilizadores"))
            {
                return "Tabela de utilizadores não encontrada. Contacte o administrador do sistema.";
            }
            else if (mensagem.Contains("invalid column name"))
            {
                return "Estrutura da base de dados incorrecta. Contacte o administrador do sistema.";
            }
            else
            {
                return $"Erro do sistema: {ex.Message}";
            }
        }

        #region Event Handlers

        /// <summary>
        /// Click no botão de login
        /// </summary>
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            await ProcessarLoginAsync();
        }

        /// <summary>
        /// Click no botão cancelar
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Tem a certeza que deseja sair da aplicação?",
                "Confirmar Saída",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        /// <summary>
        /// Prevenir fechar janela sem confirmação
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
            {
                var resultado = MessageBox.Show(
                    "Tem a certeza que deseja sair da aplicação?",
                    "Confirmar Saída",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }

        #endregion
    }
}