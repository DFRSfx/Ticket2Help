using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Ticket2Help.UI.Controllers;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Views
{
    /// <summary>
    /// Lógica para LoginWindow.xaml - Interface moderna de autenticação
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginController _loginController;
        private bool _isLogging = false;

        /// <summary>
        /// Utilizador autenticado com sucesso
        /// </summary>
        public Utilizador UtilizadorAutenticado { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            _loginController = new LoginController();

            ConfigurarInterface();
            ConfigurarEventos();
        }

        /// <summary>
        /// Configurações iniciais da interface
        /// </summary>
        private void ConfigurarInterface()
        {
            // Focar no campo de código ao carregar
            Loaded += (s, e) =>
            {
                TxtCodigo.Focus();
                // Pequeno delay para garantir que a animação terminou
                Task.Delay(500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() => TxtCodigo.Focus());
                });
            };

            // Permitir arrastar a janela
            MouseLeftButtonDown += (s, e) => DragMove();

            // Definir utilizador padrão para demonstração
            TxtCodigo.Text = "ADMIN";
        }

        /// <summary>
        /// Configurar eventos adicionais
        /// </summary>
        private void ConfigurarEventos()
        {
            // Animações de hover nos campos
            TxtCodigo.GotFocus += (s, e) => AnimarCampo(TxtCodigo, true);
            TxtCodigo.LostFocus += (s, e) => AnimarCampo(TxtCodigo, false);
            TxtSenha.GotFocus += (s, e) => AnimarCampo(TxtSenha, true);
            TxtSenha.LostFocus += (s, e) => AnimarCampo(TxtSenha, false);
        }

        /// <summary>
        /// Animação sutil para campos em foco
        /// </summary>
        private void AnimarCampo(FrameworkElement elemento, bool ganhouFoco)
        {
            var scaleTransform = new System.Windows.Media.ScaleTransform(1, 1);
            elemento.RenderTransform = scaleTransform;
            elemento.RenderTransformOrigin = new Point(0.5, 0.5);

            var animacao = new DoubleAnimation
            {
                To = ganhouFoco ? 1.02 : 1.0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };

            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, animacao);
            scaleTransform.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, animacao);
        }

        /// <summary>
        /// Mostrar indicador de carregamento
        /// </summary>
        private void MostrarCarregamento(bool mostrar)
        {
            LoadingPanel.Visibility = mostrar ? Visibility.Visible : Visibility.Collapsed;
            BtnLogin.IsEnabled = !mostrar;
            BtnCancelar.IsEnabled = !mostrar;
            TxtCodigo.IsEnabled = !mostrar;
            TxtSenha.IsEnabled = !mostrar;

            BtnLogin.Content = mostrar ? "⏳ A verificar..." : "🚀 Entrar";
        }

        /// <summary>
        /// Mostrar mensagem de erro com animação
        /// </summary>
        private void MostrarErro(string mensagem)
        {
            LblErro.Text = mensagem;
            ErrorPanel.Visibility = Visibility.Visible;

            // Animação de shake no painel de login
            var shakeAnimation = FindResource("ShakeAnimation") as Storyboard;
            shakeAnimation?.Begin(LoginPanel);

            // Ocultar erro automaticamente após 5 segundos
            Task.Delay(5000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => ErrorPanel.Visibility = Visibility.Collapsed);
            });
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
        private async Task ProcessarLogin()
        {
            if (_isLogging || !ValidarCampos())
                return;

            try
            {
                _isLogging = true;
                OcultarErro();
                MostrarCarregamento(true);

                // Simular tempo de processamento para demonstrar animação
                await Task.Delay(1500);

                var utilizador = await Task.Run(() =>
                    _loginController.Autenticar(TxtCodigo.Text.Trim(), TxtSenha.Password));

                if (utilizador != null)
                {
                    UtilizadorAutenticado = utilizador;

                    // Animação de sucesso
                    await AnimarSucesso();

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MostrarErro("Código ou palavra-passe inválidos. Verifique os dados e tente novamente.");
                    TxtSenha.Clear();
                    TxtCodigo.SelectAll();
                    TxtCodigo.Focus();
                }
            }
            catch (Exception ex)
            {
                MostrarErro($"Erro ao conectar com o sistema: {ex.Message}");
            }
            finally
            {
                _isLogging = false;
                MostrarCarregamento(false);
            }
        }

        /// <summary>
        /// Animação de sucesso no login
        /// </summary>
        private async Task AnimarSucesso()
        {
            // Mudança de cor para verde
            BtnLogin.Content = "✅ Sucesso!";
            BtnLogin.Background = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(39, 174, 96)); // Verde

            // Pequena pausa para mostrar o sucesso
            await Task.Delay(800);
        }

        #region Event Handlers

        /// <summary>
        /// Click no botão de login
        /// </summary>
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            await ProcessarLogin();
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
        /// Click no botão fechar (X)
        /// </summary>
        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            BtnCancelar_Click(sender, e);
        }

        /// <summary>
        /// Enter no campo código - mover para senha
        /// </summary>
        private void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
                {
                    MostrarErro("Por favor, introduza o código do colaborador.");
                    return;
                }

                TxtSenha.Focus();
                OcultarErro();
            }
        }

        /// <summary>
        /// Enter no campo senha - fazer login
        /// </summary>
        private async void TxtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ProcessarLogin();
            }
        }

        /// <summary>
        /// Tecla Escape para fechar
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                BtnCancelar_Click(this, new RoutedEventArgs());
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Prevenir fechar janela sem confirmação
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true && !_isLogging)
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

        #region Métodos de Demonstração

        /// <summary>
        /// Método para preencher credenciais de demonstração
        /// </summary>
        private void PreencherCredenciaisDemo(string codigo, string senha)
        {
            TxtCodigo.Text = codigo;
            TxtSenha.Password = senha;
            OcultarErro();

            // Pequena animação de preenchimento
            AnimarCampo(TxtCodigo, true);
            Task.Delay(100).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => AnimarCampo(TxtCodigo, false));
            });
        }

        /// <summary>
        /// Click nos utilizadores de demonstração para preencher credenciais
        /// </summary>
        private void Demo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBlock textBlock)
            {
                var texto = textBlock.Text;
                if (texto.Contains("ADMIN"))
                {
                    PreencherCredenciaisDemo("ADMIN", "admin");
                }
                else if (texto.Contains("TEC001"))
                {
                    PreencherCredenciaisDemo("TEC001", "123");
                }
                else if (texto.Contains("COL001"))
                {
                    PreencherCredenciaisDemo("COL001", "123");
                }

                // Focar no botão de login após preencher
                BtnLogin.Focus();
            }
        }

        /// <summary>
        /// Hover nos itens de demonstração
        /// </summary>
        private void DemoItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBlock textBlock)
            {
                textBlock.FontWeight = FontWeights.SemiBold;
            }
        }

        /// <summary>
        /// Sair do hover nos itens de demonstração
        /// </summary>
        private void DemoItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBlock textBlock)
            {
                textBlock.FontWeight = FontWeights.Normal;
            }
        }

        #endregion
    }
}