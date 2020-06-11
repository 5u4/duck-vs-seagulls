using Godot;
using System;

public class Feces : KinematicBody2D
{
    private Godot.AnimatedSprite sprite;
    private bool land;
    private Color color;

    [Export]
    public float gravity = 300;
    public Vector2 velocity;

    public override void _Ready()
    {
        sprite = GetNode<Godot.AnimatedSprite>("AnimatedSprite");
        sprite.Play();
        color = sprite.Modulate;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!land) HandleLanding(delta);
        else HandleLand(delta);
    }

    private void HandleLand(float delta)
    {
        color.a -= delta * 0.05f;
        sprite.Modulate = color;
        if (color.a > 0f) return;
        QueueFree();
    }

    private void HandleLanding(float delta)
    {
        ApplyGravity(delta);
        HandleMovement();
        CheckLanded();
    }

    private void ApplyGravity(float delta)
    {
        velocity.y += gravity * delta;
    }

    private void CheckLanded()
    {
        if (Mathf.Abs(velocity.y) > 1f) return;
        velocity.x = 0;
        sprite.Play("land");
        land = true;
        CollisionShape2D collision = GetNode<CollisionShape2D>("CollisionShape2D");
        collision.Disabled = true;
    }

    private void HandleMovement()
    {
        velocity = MoveAndSlideWithSnap(velocity, Vector2.Up);
    }
}
