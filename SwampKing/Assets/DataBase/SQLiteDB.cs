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
                              "state TEXT NOT NULL, " +
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
                              "drop_id INTEGER PRIMARY KEY, " +  // Identificador único para cada drop
                              "id_dropper INTEGER NOT NULL, " +                // ID del enemigo que realiza el drop
                              "id_item INTEGER NOT NULL, " +                 // ID del objeto dropeado
                              "chance INTEGER NOT NULL, " +                 // ID del objeto dropeado
                              "FOREIGN KEY(id_dropper) REFERENCES character(id)," +
                              "FOREIGN KEY(id_item) REFERENCES item(id)" +
                              ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
                
                sqlcreation = "CREATE TABLE IF NOT EXISTS drop_quest (" +
                              "drop_id INTEGER PRIMARY KEY, " +  // Identificador único para cada drop
                              "id_dropper INTEGER NOT NULL, " +                // ID del enemigo que realiza el drop
                              "id_item INTEGER NULL, " +                 // ID del objeto dropeado
                              "coins INTEGER NULL, " +                 // ID del objeto dropeado
                              "FOREIGN KEY(id_dropper) REFERENCES quests(id_quest)," +
                              "FOREIGN KEY(id_item) REFERENCES item(id)" +
                              ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();

                // Tabla Inventario
                sqlcreation = "CREATE TABLE IF NOT EXISTS inventary (" +
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

    public void InsertInitialData()
    {
         // Inserciones estadisticas
        Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (1, 200, 25, 15, 15, 6);");
        Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (2, 100, 25, 15, 15, 4);");
        Query("INSERT OR IGNORE INTO statistics (id, health, damage, endurance, armor, speed) VALUES (3, 150, 20, 15, 15, 5);");
        Query("SELECT * FROM statistics;");
        
        // Inserciones player 
        Query("INSERT OR IGNORE INTO player (name, statistics, position, coins) VALUES ('Gusta', 1, '0-0-0',0);");
        
        // Inserciones personajes
        Query("INSERT OR IGNORE INTO character (id, name, statistics, friendly, state, coins) VALUES (1,'Rata-Topo', 2, 1, 'alive', 10);");
        Query("INSERT OR IGNORE INTO character (id, name, statistics, friendly, state) VALUES (2,'Rana', 3, 0, 'alive');");
        
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
        
        //Inserciones Inventary
        Query("INSERT OR IGNORE INTO inventary(id_item, quantity) VALUES (1, 5);");
        Query("INSERT OR IGNORE INTO inventary(id_item, quantity) VALUES (2, 1);");
        Query("INSERT OR IGNORE INTO inventary(id_item, quantity) VALUES (3, 1);");
        
        //Inserciones Tips
        Query("INSERT OR IGNORE INTO tips(title, description) VALUES ('Licor de nenufar', 'Si estas a poca vida tomate un licor de nenufar para recuperar un poco de vida.');");


    }
}