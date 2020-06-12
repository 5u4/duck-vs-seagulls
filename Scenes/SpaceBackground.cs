using Godot;
using System;

public class SpaceBackground : Node2D
{
    private Node2D target;
    private Vector2 position;

    [Export]
    public NodePath targetPath;

    public override void _Ready()
    {
        target = GetNode<Node2D>(targetPath);
    }

    public override void _Process(float delta)
    {
        position.y = target.Position.y;
        Position = position;
    }
}
