using Godot;
using System;

public class Title : Node2D
{
    [Export]
    public PackedScene nextScene;

    public void _on_Button_pressed()
    {
        GetTree().ChangeSceneTo(nextScene);
    }
}
