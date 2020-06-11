using Godot;
using System;

public class Player : KinematicBody2D
{
    private Vector2 velocity;
    private readonly float EPSILON = 0.0001f;

    [Export]
    public float gravity = 200;
    [Export]
    public float speed = 120;
    [Export]
    public float acceleration = 0.25f;
    [Export]
    public float friction = 0.1f;

    public override void _PhysicsProcess(float delta)
    {
        GetInput();

        velocity.y += gravity * delta;
        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private void GetInput()
    {
        float direction = Input.GetActionStrength("ui_right") - 
                Input.GetActionStrength("ui_left");
        velocity.x = Mathf.Abs(direction) > EPSILON
            ? Mathf.Lerp(velocity.x, direction * speed, acceleration)
            : Mathf.Lerp(velocity.x, 0, friction);
    }
}
