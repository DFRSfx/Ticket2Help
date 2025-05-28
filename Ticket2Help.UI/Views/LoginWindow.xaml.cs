using System.Windows;
using Ticket2Help.BLL.Patterns;
using Ticket2Help.DAL.Interfaces;
using Ticket2Help.DAL.Repositories;

namespace Ticket2Help.UI.Views
{

    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para o botão de login
            string codigo = txtCodigo.Text;
            string password = pwdPassword.Password;

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(password))
            {
                lblErro.Text = "Por favor, preencha todos os campos.";
                lblErro.Visibility = Visibility.Visible;
            }
            else
            {
                lblErro.Visibility = Visibility.Collapsed;
                SqlUtilizadorRepository sqlUtilizadorRepository = new SqlUtilizadorRepository();
                var utilizadores = sqlUtilizadorRepository.ObterTodosUtilizadores().ToList();
                
                if (AuthenticationManager.Instance.Autenticar(codigo, password, utilizadores))
                {
                    new MainWindow().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Login failed. Please check your credentials.");
                }
            }
        }
    }
}