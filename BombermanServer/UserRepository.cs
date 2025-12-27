using System;
using MySql.Data.MySqlClient; //ef core yerine direkt sql

namespace BombermanServer
{
    public class UserRepository
    {
        
        private string connectionString = "Server=localhost;Database=BombermanDB;Uid=root;Pwd=2.Milyonistanbul;";

        public void AddPlayer(string username)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    
                    string sql = "INSERT INTO Players (username) VALUES (@u)";

                    using (var cmd = new MySqlCommand(sql, connection))
                    {
                        
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine($"[DB] {username} veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB HATA] Kayıt başarısız: {ex.Message}");
            }
        }
    }
}