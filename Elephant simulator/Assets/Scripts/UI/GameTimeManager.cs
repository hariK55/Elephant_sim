using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance;

    private float playTime;
    private bool timerRunning = true;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (timerRunning)
        {
            playTime += Time.deltaTime;
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public float GetPlayTime()
    {
        return playTime;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(playTime / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}