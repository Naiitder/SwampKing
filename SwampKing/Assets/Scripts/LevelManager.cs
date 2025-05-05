using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    private float targetProgress;
    
    private bool isLoading = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        loaderCanvas.SetActive(false);
    }


    public async void LoadScene(string sceneName)
    {
        if (isLoading) return; 
        isLoading = true;
        
        var tipData = GetRandomTip();
        title.text = tipData.title;
        description.text = tipData.description;
        
        if(GameController.instance.isGamePaused) GameController.instance.ResumeGame();
        
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            targetProgress = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;
    }

    public void UpdateProgressBar()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, targetProgress, 3*Time.deltaTime);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public (string title, string description) GetRandomTip()
    {
        using (var connection = new SqliteConnection(SQLiteDB.instance.dbName))
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

}
