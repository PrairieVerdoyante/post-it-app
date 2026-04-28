using post_it_app.textHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.CodeDom;


namespace post_it_app
{
    // permet de stocker des structs de post it dans un fichier
    static class PostItLibrary
    {
        private static readonly string DbPath = Path.Combine(
                AppContext.BaseDirectory,
                "database_files",
                "postitapp.db"
            );

        private static readonly string ConnectionString = $"Data Source={DbPath}";

        public static void initialiseDatabase()
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string CreateTableQuery = @"
        CREATE TABLE IF NOT EXISTS ""PostIt"" (
            ""id"" INTEGER PRIMARY KEY AUTOINCREMENT,
            ""text"" TEXT,
            ""posX"" INTEGER,
            ""posY"" INTEGER
        );
        ";

                using (var command = new SqliteCommand(CreateTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        public static int storeNew(string text, double posX=0, double posY=0)
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string insertQuery = @"
                INSERT INTO ""PostIt"" (text, posX, posY)
                VALUES (@text, @posX, @posY);
                SELECT last_insert_row_id();
                ";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@text", text);
                    command.Parameters.AddWithValue("@posX", posX);
                    command.Parameters.AddWithValue("@posY", posY);
                    var id = (long)command.ExecuteScalar();
                    return (int)id;
                    //command.ExecuteNonQuery();
                }
            }
        }

        public static void editPostIt(int id, string text, double posX=0, double posY=0)
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string editTableQuery = @"
                UPDATE ""PostIt""
                    SET text = @text, posX = @posX, posY = @posY WHERE id = @id;
                ";

                using (var command = new SqliteCommand(editTableQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@text", text);
                    command.Parameters.AddWithValue("@posX", posX);
                    command.Parameters.AddWithValue("@posY", posY);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
