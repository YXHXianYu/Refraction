using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings {
    public const float treeLeavesFadeDuration = 0.5f;
    public const float treeLeavesFadeAlpha = 0.5f;

    public const float clockUIFadeDuration = 0.5f;
    public const float dayNightImageRotateDuration = 2.0f;

    public const int dayPerSeason = 4; // 20min per season

    // public const int tickPerDay = 20; // 2s per day
    public const int tickPerDay = 6000; // 5min per day
    public const int tickPerSecond = 10;
    public const float SecondPerTick = 1f / tickPerSecond;
}