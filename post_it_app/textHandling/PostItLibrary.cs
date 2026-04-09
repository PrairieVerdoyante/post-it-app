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


        public static void storeNew(string text)
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string insertQuery = @"
                INSERT INTO ""PostIt"" (text, posX, posY)
                VALUES (@text, NULL, NULL);
                ";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@text", text);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
