using System;
using System.Windows;
using Ticket2Help.UI.Views;

namespace Ticket2Help.UI
{
    /// <summary>
    /// Lógica de interação para App.xaml - COM SUPORTE A LOGOUT/LOGIN
    /// </summary>
    public partial class App : Application
    {
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Adicionar handler para exceções não tratadas
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                base.OnStartup(e);

                System.Diagnostics.Debug.WriteLine("=== TICKET2HELP INICIANDO ===");

                // Configurar shutdown mode para controle manual
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // Criar MainWindow uma vez (será reutilizada)
                _mainWindow = new MainWindow();
                this.MainWindow = _mainWindow;

                // Iniciar ciclo de login
                IniciarCicloLogin();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO FATAL no startup: {ex}");
                MessageBox.Show($"Erro fatal ao iniciar o Ticket2Help:\n\n{ex.Message}",
                    "Erro Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        private void IniciarCicloLogin()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("🔐 Iniciando ciclo de login...");

                // Esconder MainWindow durante login
                if (_mainWindow != null)
                {
                    _mainWindow.Hide();
                }

                var loginWindow = new LoginWindow();
                var loginResult = loginWindow.ShowDialog();

                if (loginResult == true && loginWindow.UtilizadorAutenticado != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Login bem-sucedido: {loginWindow.UtilizadorAutenticado.Nome}");

                    // Configurar MainWindow com o utilizador
                    if (_mainWindow != null)
                    {
                        _mainWindow.DefinirUtilizador(loginWindow.UtilizadorAutenticado);
                        _mainWindow.Show();
                        _mainWindow.WindowState = WindowState.Maximized;
                        _mainWindow.Activate();

                        // Configurar evento de logout
                        _mainWindow.LogoutRequested -= MainWindow_LogoutRequested;
                        _mainWindow.LogoutRequested += MainWindow_LogoutRequested;

                        System.Diagnostics.Debug.WriteLine("✅ Sistema iniciado com sucesso");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("🚪 Login cancelado, encerrando aplicação");
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERRO no ciclo de login: {ex}");
                MessageBox.Show($"Erro no processo de login:\n\n{ex.Message}",
                    "Erro de Login", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
            }
        }

        private void MainWindow_LogoutRequested(object? sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("🔄 Logout solicitado, reiniciando ciclo de login");

            // Reiniciar o ciclo de login
            IniciarCicloLogin();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"🚨 EXCEÇÃO NÃO TRATADA: {e.Exception}");

            MessageBox.Show($"Erro inesperado no Ticket2Help:\n\n{e.Exception.Message}\n\nA aplicação pode estar instável. Considere reiniciar.",
                "Erro Inesperado", MessageBoxButton.OK, MessageBoxImage.Warning);

            // Marcar como tratado para evitar crash
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"🚨 EXCEÇÃO FATAL: {e.ExceptionObject}");

            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"Erro fatal no Ticket2Help:\n\n{ex.Message}\n\nA aplicação será encerrada.",
                    "Erro Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}