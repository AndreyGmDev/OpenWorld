using System;
using TMPro;
using UnityEngine;

public class DaylightCycle : MonoBehaviour
{
    const float REAL_TIME_DAY_LENGTH = 86400;

    [SerializeField] Transform directionalLight;
    [SerializeField, Tooltip("time in Seconds of the lenght of the day")] float cycleTime;
    [SerializeField] TextMeshProUGUI timeOfTheDay;

    [HideInInspector] public float seconds;
    private float multiplier;

    SaveGame saveGame;

    private void Start()
    {
        multiplier = REAL_TIME_DAY_LENGTH / cycleTime;

        saveGame = SaveGame.Instance;
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

        if (SaveGame.Instance != null)
        {
            saveGame.SaveDaylightCycleData(new SaveGameInfos
            {
                DaylightCycle = this,
                Seconds = seconds
            });
        }
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
}
