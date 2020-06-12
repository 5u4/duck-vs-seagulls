using Godot;
using System;

public class SeagullAnimatedSprite : AnimatedSprite
{
    private Seagull body;

    public override void _Ready()
    {
        body = GetNode<Seagull>("..");
    }

    public override void _Process(float delta)
    {
        HandleHorizontalFlip();
        Play(ComputeAnimationState());
    }

    private string ComputeAnimationState()
    {
        string poopPrefix = body.poopDuration < body.poopInterval / 4 ? "poop" : "";
        if (body.velocity.y < 0f) return poopPrefix + "fly";
        if (body.velocity.y > 0f) return poopPrefix + "fall";
        return null;
    }

    private void HandleHorizontalFlip()
    {
        int facing = Math.Sign(body.velocity.x);
        if (facing != 0)
        {
            FlipH = facing != 1;
        }
    }
}
