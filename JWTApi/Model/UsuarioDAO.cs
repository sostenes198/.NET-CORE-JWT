using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace JWTApi.Model
{
    public class UsuarioDAO
    {
        private IConfiguration _configuracao;

        public UsuarioDAO(IConfiguration configuracao)
        {
            _configuracao = configuracao;
        }

        public Usuario Find(string usuarioID)
        {
            using (SqlConnection conexao = new SqlConnection(_configuracao.GetConnectionString("ExemploJWT")))
            {
                return conexao.QueryFirstOrDefault<Usuario>(
                    "SELECT UserID, AccessKey " +
                    "FROM dbo.Users " +
                    "WHERE UserID = @UserID", new { UserID = usuarioID });
            }
        }
    }
}
