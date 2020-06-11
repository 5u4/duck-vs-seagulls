using Godot;
using System;

public class SeagullGenerator : Node2D
{
    private readonly float DELTA_STEP = 100;
    private float interval;

    [Export]
    public PackedScene seagullScene;
    [Export]
    public int amount = 30;
    [Export]
    public float spawnInterval = 300;

    public override void _Process(float delta)
    {
        if (amount <= 0) return;

        interval -= delta * DELTA_STEP;
        if (interval > 0) return;

        Generate();
        interval = spawnInterval;
    }

    private void Generate()
    {
        Seagull seagull = (Seagull)seagullScene.Instance();
        AddChild(seagull);
    }
}
