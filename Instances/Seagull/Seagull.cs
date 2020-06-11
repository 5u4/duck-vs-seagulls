using Godot;
using System;

public class Seagull : KinematicBody2D
{
    private readonly float DELTA_STEP = 100;
    private float flyCooldown;
    private float width;

    [Export]
    public float gravity = 300;
    [Export]
    public float speed = 75;
    [Export]
    public float flyHeight = 45;
    [Export]
    public float maxFlyCooldown = 30;

    public Vector2 velocity;

    public override void _Ready()
    {
        width = GetViewportRect().Size.x;
    }

    public override void _PhysicsProcess(float delta)
    {
        WrapSeagullPosition();
        ApplyGravity(delta);
        HandleFly(delta);
        HandleMovement();
    }

    public void Die()
    {

    }

    private void WrapSeagullPosition()
    {
        if (Position.x > width)
        {
            Position = new Vector2(0, Position.y);
        }
        else if (Position.x < 0)
        {
            Position = new Vector2(width, Position.y);
        }
    }

    private void ApplyGravity(float delta)
    {
        velocity.y += gravity * delta;
    }

    private void HandleMovement()
    {
        velocity = MoveAndSlideWithSnap(velocity, Vector2.Up);
    }

    private void HandleFly(float delta)
    {
        flyCooldown -= delta * DELTA_STEP;
        if (flyCooldown > 0) return;

        flyCooldown = maxFlyCooldown;
        velocity.y = -flyHeight;
    }
}
