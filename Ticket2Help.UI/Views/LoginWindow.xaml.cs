using System;
using System.Windows;
using Ticket2Help.UI.Controllers;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Views
{
    /// <summary>
    /// Lógica para LoginWindow.xaml - Interface otimizada de autenticação
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

            // Focar no campo de código ao carregar
            Loaded += (s, e) => TxtCodigo.Focus();

            // Permitir arrastar a janela
            MouseLeftButtonDown += (s, e) => DragMove();

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
        /// Processar login
        /// </summary>
        private void ProcessarLogin()
        {
            if (!ValidarCampos())
                return;

            try
            {
                // Desabilitar botões durante o processamento
                BtnLogin.IsEnabled = false;
                BtnLogin.Content = "A verificar...";

                OcultarErro();

                var utilizador = _loginController.Autenticar(TxtCodigo.Text.Trim(), TxtSenha.Password);

                if (utilizador != null)
                {
                    UtilizadorAutenticado = utilizador;
                    BtnLogin.Content = "Sucesso!";
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
                BtnLogin.IsEnabled = true;
                BtnLogin.Content = "Entrar";
            }
        }

        #region Event Handlers

        /// <summary>
        /// Click no botão de login
        /// </summary>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ProcessarLogin();
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