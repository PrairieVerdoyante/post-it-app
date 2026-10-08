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
using static System.Net.Mime.MediaTypeNames;


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
            var directory = Path.GetDirectoryName(DbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string CreateTableQuery = @"
        CREATE TABLE IF NOT EXISTS ""PostIt"" (
            ""id"" INTEGER PRIMARY KEY AUTOINCREMENT,
            ""text"" TEXT,
            ""posX"" INTEGER,
            ""posY"" INTEGER,
            ""color"" TEXT DEFAULT '#FF00FFFF'
        );
        ";
                // nouvelle colonne si couleur n'existe pas
                bool hasColor = false;
                using (var pragma = new SqliteCommand(@"PRAGMA table_info(""PostIt"");", connection))
                using (var reader = pragma.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(1) == "color")   // colonne 1 = nom
                        {
                            hasColor = true;
                            break;
                        }
                    }
                }

                if (!hasColor)
                {
                    using (var alter = new SqliteCommand(
                        @"ALTER TABLE ""PostIt"" ADD COLUMN ""color"" TEXT DEFAULT '#184190221';", connection))
                    {
                        alter.ExecuteNonQuery();
                    }
                }

                using (var command = new SqliteCommand(CreateTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        public static int storeNew(string text, double posX = 0, double posY = 0, String color= "#184190221")
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string insertQuery = @"
                INSERT INTO ""PostIt"" (text, posX, posY, color)
                VALUES (@text, @posX, @posY, @color);
                SELECT last_insert_rowid();
                ";

                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@text", text);
                    command.Parameters.AddWithValue("@posX", posX);
                    command.Parameters.AddWithValue("@posY", posY);
                    command.Parameters.AddWithValue("@color", color);
                    var id = (long)command.ExecuteScalar();
                    return (int)id;
                }
            }
        }

        public static void editPostIt(int id, string text, double posX = 0, double posY = 0, String color= "#184190221")
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string editTableQuery = @"
                UPDATE ""PostIt""
                    SET text = @text, posX = @posX, posY = @posY, color=@color WHERE id = @id;
                ";

                using (var command = new SqliteCommand(editTableQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@text", text);
                    command.Parameters.AddWithValue("@posX", posX);
                    command.Parameters.AddWithValue("@posY", posY);
                    command.Parameters.AddWithValue("@color", color);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void deletePostIt(int id)
        {
            Batteries.Init();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                const string deleteQuery = @"DELETE FROM ""PostIt"" WHERE id = @id;";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<PostIt> getAll()
        {
            Batteries.Init();
            var postIts = new List<PostIt>();

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                const string selectQuery = @"SELECT id, text, posX, posY, color FROM ""PostIt"";";

                using (var command = new SqliteCommand(selectQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        postIts.Add(new PostIt
                        {
                            Id = reader.GetInt32(0),
                            Text = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            PosX = (reader.IsDBNull(2) ? 0 : reader.GetInt32(2)),
                            PosY = (reader.IsDBNull(3) ? 0 : reader.GetInt32(3)),
                            Color = reader.IsDBNull(4) ? "#184190221" : reader.GetString(4)
                        });
                    }
                }
            }
            return postIts;
        }
    }
    
}
