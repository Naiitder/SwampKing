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
        // Ejemplo de inserción y consulta para la tabla statistics
        Query("INSERT INTO estadisticas (vida, ataque, resistencia, defensa, velocidad) VALUES (200, 25, 15, 15, 6);");
        Query("SELECT * FROM estadisticas;");
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
                                     "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
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
                              "FOREIGN KEY(statistics) REFERENCES statistics(id)" +
                              ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
                
                sqlcreation = "CREATE TABLE IF NOT EXISTS character (" +
                              "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                              "name TEXT NOT NULL, " +
                              "statistics INTEGER NOT NULL, " +
                              "friendly INTEGER NOT NULL, " +
                              "state TEXT NOT NULL, " +
                              "FOREIGN KEY(statistics) REFERENCES statistics(id)" +
                              ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
                
                // Tabla Misiones (para registrar el estado de una misión)
                sqlcreation = "CREATE TABLE IF NOT EXISTS quests (" +
                              "id_quest INTEGER PRIMARY KEY AUTOINCREMENT, " +
                              "name TEXT NOT NULL, " +
                              "description TEXT NOT NULL,"+
                              "state TEXT NOT NULL" +
                              ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
                
                // Tabla Objeto
                sqlcreation = "CREATE TABLE IF NOT EXISTS item (" +
                              "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
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
                                
                sqlcreation = "CREATE TABLE IF NOT EXISTS drop_table (" +
                              "drop_id INTEGER PRIMARY KEY AUTOINCREMENT, " +  // Identificador único para cada drop
                              "id_dropper INTEGER NOT NULL, " +                // ID del enemigo que realiza el drop
                              "id_item INTEGER NOT NULL, " +                 // ID del objeto dropeado
                              "FOREIGN KEY(id_dropper) REFERENCES character(id)," +
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
                              "id_item INTEGER PRIMARY KEY AUTOINCREMENT, " +
                              "title TEXT NOT NULL, " +
                              "description TEXT NOT NULL " +
                              ");";
                command.CommandText = sqlcreation;
                command.ExecuteNonQuery();
            }

            connection.Close();
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
                            Debug.Log("ID: " + reader["id"] + " Vida: " + reader["vida"] + " Ataque: " + reader["ataque"]);
                        }
                    }
                }
            }

            connection.Close();
        }
    }
}