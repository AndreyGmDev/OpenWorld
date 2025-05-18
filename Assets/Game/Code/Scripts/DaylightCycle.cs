using System;
using KinematicCharacterController.Walkthrough.ClimbingLadders;
using TMPro;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    const float REAL_TIME_DAY_LENGTH = 86400;

    [SerializeField] Transform directionalLight;
    [SerializeField, Tooltip("Time in seconds of a day's length")] float cycleTime;
    [SerializeField] TextMeshProUGUI timeOfTheDay;

    [HideInInspector] public float seconds;
    private float multiplier;

    SaveGame saveGame;

    private void Start()
    {
        multiplier = REAL_TIME_DAY_LENGTH / cycleTime;
        saveGame = SaveGame.Instance;
        Load();
    }

    private void Update()
    {
        if (seconds > REAL_TIME_DAY_LENGTH)
        {
            seconds = 0;
        }

        seconds += Time.deltaTime * multiplier;

        CycleChange();
        InterfaceTime();
        Save();
    }

    public bool IsDaytime()
    {
        // Convert seconds to hours (0-24)
        float hours = (seconds / 3600f) % 24;
        // Day = between 6 AM and 6 PM
        return hours >= 6 && hours < 18;
    }

    private void CycleChange()
    {
        float cycleRotation = Mathf.Lerp(-90, 270, seconds / REAL_TIME_DAY_LENGTH);
        directionalLight.rotation = Quaternion.Euler(cycleRotation , 0 , 0);
    }

    private void InterfaceTime()
    {
        if (timeOfTheDay != null)
        {
            timeOfTheDay.text = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm");
        }
    }

    // Passa as informações para o SaveGame
    private void Save()
    {
        if (saveGame != null)
        {
            saveGame.SaveDaylightCycleData(new SaveGameInfos
            {
                Seconds = seconds
            });
        }
    }

    // Carrega as informações do SaveGame
    private void Load()
    {
        if (saveGame != null)
        {
            SaveGameInfos save = saveGame.LoadPlayerData();
            seconds = save.Seconds;
        }
    }
}
