using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager> {
    public bool IsPaused { get; private set; } = false;
    public int Day { get; private set; } = 0;
    public int TickInDay { get; private set; } = 0;
    public Season CurrentSeason { get; private set; } = Season.春天;
    public int Year { get; private set; } = 0;

    public const int tickPerDay = Settings.tickPerDay;
    public const int tickPerSecond = Settings.tickPerSecond;
    public const int dayPerSeason = Settings.dayPerSeason;
    public const float SecondPerTick = Settings.SecondPerTick;

    private float timer = 0f;

    private void Update() {
        if (!IsPaused) {
            timer += Time.deltaTime;
            if (timer >= Settings.SecondPerTick) {
                timer -= Settings.SecondPerTick;
                Tick();
            }
        }
    }

    private void Tick() {
        UpdateTime();

        // DefaultEventEmitter.Instance.Emit("Tick");
    }

    public void ResetTime() {
        TickInDay = 0;
        Day = 0;
        CurrentSeason = Season.春天;
        Year = 0;
    }

    private void UpdateTime() {
        // Debug.Log("Tick: " + TickInDay + " Day: " + Day + " Season: " + CurrentSeason + " Year: " + Year);
        TickInDay++;
        if (TickInDay >= Settings.tickPerDay) {
            TickInDay -= Settings.tickPerDay;
            Day++;
            if (Day >= Settings.dayPerSeason) {
                Day -= Settings.dayPerSeason;

                if (CurrentSeason == Season.春天) {
                    CurrentSeason = Season.夏天;
                }
                else if (CurrentSeason == Season.夏天) {
                    CurrentSeason = Season.秋天;
                }
                else if (CurrentSeason == Season.秋天) {
                    CurrentSeason = Season.冬天;
                }
                else if (CurrentSeason == Season.冬天) {
                    CurrentSeason = Season.春天;
                    Year++; // New Year!
                }
            }
        }
    }
}