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
        HandleHorizontalFlip();
        Play(ComputeAnimationState());
    }

    private string ComputeAnimationState()
    {
        if (body.lost) return "dead";
        if (body.isAttacking) return "attack";

        bool isJumping = body.IsJumping();

        if (isJumping)
        {
            if (body.velocity.y < 0f) return "jump";
            if (body.velocity.y > 0f) return "fall";
            return null;
        }

        return Mathf.Abs(body.velocity.x) > RUNNING_THRESHOLD ? "run" : "idle";
    }

    private void HandleHorizontalFlip()
    {
        if (body.isAttacking)
        {
            FlipH = false;
            return;
        }

        int facing = Math.Sign(body.velocity.x);
        if (facing != 0)
        {
            FlipH = facing != 1;
        }
    }
}
