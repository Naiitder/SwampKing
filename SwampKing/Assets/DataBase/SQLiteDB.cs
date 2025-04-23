    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Mono.Data.Sqlite;
    using System.Data;

    public class SQLiteDB : MonoBehaviour
    {
        public static SQLiteDB instance;
        private string dbName = "URI=file:DataBase.db";
        
        private void Awake()
        {        
            if (instance == null) 
                instance = this;
            else 
                Destroy(gameObject);
        }
        
        void Start()
        {
            CreateDatabase();
            
            if (!DataExists("statistics"))
            {
                InsertInitialData();
            }
        }

        private void CreateDatabase()
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Tabla Estadísticas
                    string sqlcreation = "CREATE TABLE IF NOT EXISTS statistics (" +
                                         "id INTEGER PRIMARY KEY, " +
                                         "health INTEGER NOT NULL, " +
                                         "damage INTEGER NOT NULL, " +
                                         "endurance INTEGER NOT NULL, " +
                                         "armor INTEGER NOT NULL, " +
                                         "speed INTEGER NOT NULL" +
                                         ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();

                    // Tabla Personaje
                    sqlcreation = "CREATE TABLE IF NOT EXISTS player (" +
                                  "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                  "name TEXT NOT NULL, " +
                                  "statistics INTEGER, " +
                                  "position TEXT NOT NULL, " +
                                  "coins INTEGER NOT NULL, " +
                                  "FOREIGN KEY(statistics) REFERENCES statistics(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    sqlcreation = "CREATE TABLE IF NOT EXISTS character (" +
                                  "id INTEGER PRIMARY KEY, " +
                                  "name TEXT NOT NULL, " +
                                  "statistics INTEGER NOT NULL, " +
                                  "friendly INTEGER NOT NULL, " +
                                  "type TEXT NOT NULL, " +
                                  "coins INTEGER NULL, " +   
                                  "FOREIGN KEY(statistics) REFERENCES statistics(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    // Tabla Misiones (para registrar el estado de una misión)
                    sqlcreation = "CREATE TABLE IF NOT EXISTS quests (" +
                                  "id_quest INTEGER PRIMARY KEY, " +
                                  "name TEXT NOT NULL, " +
                                  "description TEXT NOT NULL,"+
                                  "state TEXT NOT NULL" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    // Tabla Objeto
                    sqlcreation = "CREATE TABLE IF NOT EXISTS item (" +
                                  "id INTEGER PRIMARY KEY, " +
                                  "name TEXT NOT NULL, " +
                                  "description TEXT NOT NULL,"+
                                  "price INTEGER NOT NULL, " +
                                  "type TEXT NULL" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    // Tabla Armas
                    sqlcreation = "CREATE TABLE IF NOT EXISTS weapon (" +
                                  "id_item INTEGER PRIMARY KEY, " +
                                  "damage INTEGER NOT NULL, " +
                                  "FOREIGN KEY(id_item) REFERENCES item(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();

                    // Tabla Armadura
                    sqlcreation = "CREATE TABLE IF NOT EXISTS armor (" +
                                  "id_item INTEGER PRIMARY KEY, " +
                                  "armor INTEGER NOT NULL, " +
                                  "FOREIGN KEY(id_item) REFERENCES item(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                                    
                    sqlcreation = "CREATE TABLE IF NOT EXISTS drop_enemy (" +
                                  "drop_id INTEGER PRIMARY KEY, " + 
                                  "id_dropper INTEGER NOT NULL, " +                
                                  "id_item INTEGER NOT NULL, " +                
                                  "chance INTEGER NOT NULL, " +                 
                                  "FOREIGN KEY(id_dropper) REFERENCES character(id)," +
                                  "FOREIGN KEY(id_item) REFERENCES item(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    sqlcreation = "CREATE TABLE IF NOT EXISTS drop_quest (" +
                                  "drop_id INTEGER PRIMARY KEY, " +  
                                  "id_dropper INTEGER NOT NULL, " +               
                                  "id_item INTEGER NULL, " +                 
                                  "coins INTEGER NULL, " +                 
                                  "FOREIGN KEY(id_dropper) REFERENCES quests(id_quest)," +
                                  "FOREIGN KEY(id_item) REFERENCES item(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();

                    // Tabla Inventario
                    sqlcreation = "CREATE TABLE IF NOT EXISTS inventory (" +
                                  "id_item INTEGER NOT NULL, " +
                                  "quantity INTEGER NOT NULL, " +
                                  "FOREIGN KEY(id_item) REFERENCES item(id)" +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    sqlcreation = "CREATE TABLE IF NOT EXISTS tips (" +
                                  "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                  "title TEXT NOT NULL, " +
                                  "description TEXT NOT NULL " +
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    sqlcreation = "CREATE TABLE IF NOT EXISTS dialogue_group ("+
                                  "id INTEGER PRIMARY KEY AUTOINCREMENT,"+
                                  "character_id INTEGER NOT NULL,"+
                                  "condition TEXT NULL,"+
                                  "type TEXT DEFAULT 'random',"+
                                  "FOREIGN KEY(character_id) REFERENCES character(id)"+
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    sqlcreation = "CREATE TABLE IF NOT EXISTS dialogue ("+
                                  "id INTEGER PRIMARY KEY AUTOINCREMENT,"+
                                  "dialogue_group_id INTEGER NOT NULL,"+
                                  "line TEXT NOT NULL,"+
                                  "line_order INTEGER NOT NULL,"+
                                  "FOREIGN KEY(dialogue_group_id) REFERENCES dialogue_group(id)"+
                                  ");";
                    command.CommandText = sqlcreation;
                    command.ExecuteNonQuery();
                    
                    
                }

                connection.Close();
                
                
            }
        }
        
        private bool DataExists(string tableName)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT COUNT(*) FROM {tableName};";
                    int count = System.Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        
        public void Query(string q)
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = q;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if(reader.FieldCount > 0)
                            {
                                Debug.Log("ID: " + reader["id"] + " Vida: " + reader["health"] + " Ataque: " + reader["damage"]);
                            }
                        }
                    }
                }

                connection.Close();
            }
        }
        
        public (string title, string description) GetRandomTip()
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT title, description FROM tips ORDER BY RANDOM() LIMIT 1;";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string title = reader["title"].ToString();
                            string description = reader["description"].ToString();
                            return (title, description);
                        }
                    }
                }
            }

            return ("", "");
        }
        
        public List<string> GetRandomDialogue(int characterID)
        {
            List<string> lines = new List<string>();

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Primero seleccionamos un grupo de diálogo aleatorio
                    command.CommandText = @"
                SELECT id FROM dialogue_group
                WHERE character_id = @charID AND type = 'random'
                ORDER BY RANDOM() LIMIT 1;";
                    command.Parameters.AddWithValue("@charID", characterID);

                    object groupIdObj = command.ExecuteScalar();
                    if (groupIdObj != null)
                    {
                        int groupId = Convert.ToInt32(groupIdObj);

                        // Luego sacamos todas las líneas del grupo seleccionado
                        using (var cmd2 = connection.CreateCommand())
                        {
                            cmd2.CommandText = @"
                        SELECT line FROM dialogue
                        WHERE dialogue_group_id = @groupID
                        ORDER BY line_order ASC;";
                            cmd2.Parameters.AddWithValue("@groupID", groupId);

                            using (var reader = cmd2.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    lines.Add(reader["line"].ToString());
                                }
                            }
                        }
                    }
                }
            }

            return lines;
        }

        public List<string> GetSequentialDialogue(int characterID, string condition)
        {
            List<string> lines = new List<string>();

            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                SELECT d.line 
                FROM dialogue d
                JOIN dialogue_group dg ON d.dialogue_group_id = dg.id
                WHERE dg.character_id = @charID 
                  AND dg.type = 'sequential'
                  AND (dg.condition IS NULL OR dg.condition = @cond)
                ORDER BY d.line_order ASC;";
            
                    command.Parameters.AddWithValue("@charID", characterID);
                    command.Parameters.AddWithValue("@cond", condition);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lines.Add(reader["line"].ToString());
                        }
                    }
                }
            }

            return lines;
        }




        public void InsertInitialData()
        {
             // Inserciones estadisticas
            Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (1, 200, 25, 15, 15, 6);");
            Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (2, 100, 25, 15, 15, 4);");
            Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (3, 150, 20, 15, 15, 5);");
            Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (4, 450, 50, 15, 15, 6);");
            Query("SELECT * FROM statistics;");
            
            // Inserciones player 
            Query("INSERT OR IGNORE INTO player (name, statistics, position, coins) VALUES ('Gusta', 1, '0-0-0',0);");
            
            // Inserciones personajes
            Query("INSERT OR IGNORE INTO character (id, name, statistics, friendly, type, coins) VALUES (1,'Rata-Topo', 2, 1, 'enemy', 10);");
            Query("INSERT OR IGNORE INTO character (id, name, statistics, friendly, type) VALUES (2,'Rana', 3, 0, 'npc');");
            Query("INSERT OR IGNORE INTO character (id, name, statistics, friendly, type) VALUES (3,'Sapo', 3, 0, 'npc');");
            Query("INSERT OR IGNORE INTO character (id, name, statistics, friendly, type, coins) VALUES (4,'Asesino Rana', 4, 1, 'boss', 100);");

            //Inserciones Misiones
            Query("INSERT OR IGNORE INTO quests (id_quest, name, description, state) VALUES (1,'Asesino de Rata-Topos', 'Asesina 10 Rata-Topos.', 'not accepted');");
            Query("INSERT OR IGNORE INTO quests (id_quest, name, description, state) VALUES (2,'Llega a Ciudad Charca', 'Llega a Ciudad Charca y habla con los habitantes.', 'accepted');");
            
            //Inserciones Objetos
            Query("INSERT OR IGNORE INTO item (id, name, description, price) VALUES (1,'Licor de nenufar', 'Restaura 100 puntos de vida.', 50);");
            Query("INSERT OR IGNORE INTO item (id, name, description, price, type) VALUES (2,'Espada del rey rana', 'Espada que pertenecio a un antiguo rey de Ciudad Charca.', -1, 'weapon' );");
            Query("INSERT OR IGNORE INTO weapon (id_item, damage) VALUES (2, 25 );");
            Query("INSERT OR IGNORE INTO item (id, name, description, price, type) VALUES (3,'Gabardina de vagabundo', 'Gabardina que suelen llevar los vagabundos provenientes de Ciudad Charca.', -1, 'armor' );");
            Query("INSERT OR IGNORE INTO armor (id_item, armor) VALUES (3, 15 );");
            
            //Inserciones Drops
            Query("INSERT OR IGNORE INTO drop_enemy(drop_id, id_dropper, id_item, chance) VALUES (1,1,1, 10 );");
            Query("INSERT OR IGNORE INTO drop_quest(drop_id, id_dropper, id_item, coins) VALUES (2,1,1, 100 );");
            
            //Inserciones inventory
            Query("INSERT OR IGNORE INTO inventory(id_item, quantity) VALUES (1, 5);");
            Query("INSERT OR IGNORE INTO inventory(id_item, quantity) VALUES (2, 1);");
            Query("INSERT OR IGNORE INTO inventory(id_item, quantity) VALUES (3, 1);");
            
            //Inserciones Tips
            Query("INSERT OR IGNORE INTO tips(title, description) VALUES ('Licor de nenufar'," +
                  " 'Si estas a poca vida tomate un licor de nenufar para recuperar un poco de vida.');");
            Query("INSERT OR IGNORE INTO tips(title, description) VALUES ('Elegido del rey del pantano'," +
                  " 'Utiliza tus luciernagas para desbloquear o mejorar tus habilidades.');");
            Query("INSERT OR IGNORE INTO tips(title, description) VALUES ('Luciernagas'," +
                  " 'Algunos enemigos sueltan luciernagas al ser derrotados, esta moneda sirve para muchas cosas," +
                  " por ejemplo subir de nivel o comprar objetos.');");
            
            //Inserciones Dialogos
            Query("INSERT OR IGNORE INTO dialogue_group (id, character_id) VALUES (1, 2);");
            Query("INSERT OR IGNORE INTO dialogue_group (id, character_id) VALUES (2, 3);");
            Query("INSERT OR IGNORE INTO dialogue_group (id, character_id) VALUES (3, 3);");

            
            Query("INSERT OR IGNORE INTO dialogue (dialogue_group_id, line, line_order) VALUES (1, 'Hola viajero.', 0);");
            Query("INSERT OR IGNORE INTO dialogue (dialogue_group_id, line, line_order) VALUES (1, '¿Has visto alguna luciérnaga últimamente?', 1);");
            Query("INSERT OR IGNORE INTO dialogue (dialogue_group_id, line, line_order) VALUES (2, '¡Ya va siendo hora de que te mates!', 0);");
            Query("INSERT OR IGNORE INTO dialogue (dialogue_group_id, line, line_order) VALUES (3, '¡Fuera de mi charca!', 0);");



        }
    }