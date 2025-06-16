using Microsoft.Data.SqlClient;
using Ticket2Help.BLL.Services;
using Ticket2Help.DAL.Repositories;
using Ticket2Help.Models;

namespace Ticket2Help.UI.Controllers
{
    /// <summary>
    /// Controlador para autenticação de utilizadores.
    /// </summary>
    public class LoginController
    {
        private readonly UsuarioService _usuarioService;

        /// <summary>
        /// Construtor do controlador de login.
        /// </summary>
        public LoginController()
        {
            var usuarioRepository = new UsuarioRepository();
            _usuarioService = new UsuarioService(usuarioRepository);
        }

        /// <summary>
        /// Autentica um utilizador.
        /// </summary>
        /// <param name="codigo">Código do utilizador.</param>
        /// <param name="senha">Senha do utilizador.</param>
        /// <returns>Utilizador autenticado ou null.</returns>
        public Usuario Autenticar(string codigo, string senha)
        {
            return _usuarioService.Autenticar(codigo, senha);
        }
    }
}
sql, connection);
command.Parameters.AddWithValue("@Codigo", usuario.Codigo);
command.Parameters.AddWithValue("@Nome", usuario.Nome);
command.Parameters.AddWithValue("@Email", usuario.Email);
command.Parameters.AddWithValue("@EhTecnicoHelpdesk", usuario.EhTecnicoHelpdesk);

command.ExecuteNonQuery();
        }

        /// <summary>
        /// Obtém todos os utilizadores activos.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        public IEnumerable<Usuario> ObterTodos()
{
    using var connection = _dbConnection.CreateConnection();
    connection.Open();

    const string sql = @"
                SELECT Codigo, Nome, Email, EhTecnicoHelpdesk
                FROM Utilizadores 
                WHERE Activo = 1
                ORDER BY Nome";

    using var command = new SqlCommand(sql, connection);
    using var reader = command.ExecuteReader();

    var usuarios = new List<Usuario>();
    while (reader.Read())
    {
        usuarios.Add(new Usuario
        {
            Codigo = reader.GetString("Codigo"),
            Nome = reader.GetString("Nome"),
            Email = reader.GetString("Email"),
            EhTecnicoHelpdesk = reader.GetBoolean("EhTecnicoHelpdesk")
        });
    }

    return usuarios;
}

/// <summary>
/// Valida as credenciais de um utilizador.
/// </summary>
/// <param name="codigo">Código do utilizador.</param>
/// <param name="senha">Senha do utilizador.</param>
/// <returns>True se as credenciais forem válidas.</returns>
public bool ValidarCredenciais(string codigo, string senha)
{
    using var connection = _dbConnection.CreateConnection();
    connection.Open();

    const string sql = @"
                SELECT COUNT(*) 
                FROM Utilizadores 
                WHERE Codigo = @Codigo AND PasswordHash = @PasswordHash AND Activo = 1";

    using var command = new SqlCommand(sql, connection);
    command.Parameters.AddWithValue("@Codigo", codigo);
    command.Parameters.AddWithValue("@PasswordHash", GerarHashSenha(senha));

    var count = (int)command.ExecuteScalar();
    return count > 0;
}

private string GerarHashSenha(string senha)
{
    // Implementação simplificada - usar BCrypt ou similar em produção
    return $"hash_{senha}";
}
    }
}