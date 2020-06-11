using Godot;
using System;

public class SeagullGenerator : Node2D
{
    private readonly float DELTA_STEP = 100;
    private float interval;
    private Node2D camera;

    [Export]
    public PackedScene seagullScene;
    [Export]
    public int amount = 50;
    [Export]
    public float spawnInterval = 100;
    [Export]
    public NodePath playerPath;

    public override void _Ready()
    {
        camera = GetNode<Node2D>(playerPath);
    }

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
        seagull.SetTracker(camera);
        AddChild(seagull);
    }
}
