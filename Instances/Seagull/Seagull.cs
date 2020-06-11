using Godot;
using System;

public class Seagull : KinematicBody2D
{
    private readonly float DELTA_STEP = 100;
    private float flyCooldown;

    [Export]
    public float gravity = 300;
    [Export]
    public float speed = 75;
    [Export]
    public float flyHeight = 45;
    [Export]
    public float maxFlyCooldown = 30;

    public Vector2 velocity;

    public override void _PhysicsProcess(float delta)
    {
        ApplyGravity(delta);
        HandleFly(delta);
        HandleMovement();
    }

    public void Die()
    {

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
