using Godot;
using System;

public class GeneralParticle : CPUParticles2D
{
    public override void _Process(float delta)
    {
        if (!Emitting) QueueFree();
    }
}
