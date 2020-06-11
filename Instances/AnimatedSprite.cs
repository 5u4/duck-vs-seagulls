using Godot;
using System;

public class AnimatedSprite : Godot.AnimatedSprite
{
    private Player body;
    private readonly float RUNNING_THRESHOLD = 5f;

    public override void _Ready()
    {
        body = GetNode<Player>("..");
        Play();
    }

    public override void _Process(float delta)
    {
        string animation = "idle";

        if (Mathf.Abs(body.velocity.x) > RUNNING_THRESHOLD)
        {
            animation = "run";
        }

        if (body.IsJumping())
        {
            if (body.velocity.y < 0f)
            {
                animation = "jump";
            } else if (body.velocity.y > 0f)
            {
                animation = "fall";
            }
        }

        int facing = Math.Sign(body.velocity.x);
        if (facing != 0)
        {
            FlipH = facing != 1;
        }

        Play(animation);
    }
}
