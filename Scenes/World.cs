using Godot;
using System;

public class World : Node2D
{
    public Button restartButton;

    public override void _Ready()
    {
        restartButton = GetNode<Button>("CanvasLayer/CenterContainer/Restart");
    }

    public void _on_Player_DeadSignal()
    {
        restartButton.Show();
    }

    public void _on_Restart_pressed()
    {
        GetTree().ReloadCurrentScene();
    }
}
