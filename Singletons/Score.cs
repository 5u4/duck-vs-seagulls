using Godot;
using System;

public class Score : Node2D
{
    public static Score instance;
    public int KillCount { get; set; }
    public int MaxHeight { get; set; }

    public override void _Ready()
    {
        if (instance != null) return;
        instance = this;
    }

    public void Reset()
    {
        KillCount = 0;
    }
}
