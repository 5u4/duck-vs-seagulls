using Godot;
using System;

public class World : Node2D
{
    public CenterContainer endingContainer;

    public override void _Ready()
    {
        endingContainer = GetNode<CenterContainer>("CanvasLayer/EndContainer");
    }

    public void _on_Player_DeadSignal()
    {
        string killCountText = "Total Seagull Killed: " + Score.instance.KillCount;
        endingContainer.GetNode<Label>("VBoxContainer/KillCount").Text = killCountText;

        string maxHeightText = "Maximum Height Reached: " + Score.instance.MaxHeight;
        endingContainer.GetNode<Label>("VBoxContainer/MaxHeight").Text = maxHeightText;

        endingContainer.Show();
    }

    public void _on_Restart_pressed()
    {
        Score.instance.Reset();
        GetTree().ReloadCurrentScene();
    }
}
