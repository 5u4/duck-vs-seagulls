using Godot;
using System;

public class Player : KinematicBody2D
{
    private readonly float EPSILON = 0.0001f;
    private int doubleJumpCount;

    [Export]
    public float gravity = 400;
    [Export]
    public float speed = 120;
    [Export]
    public float acceleration = 0.25f;
    [Export]
    public float friction = 0.1f;
    [Export]
    public float jumpHeight = 200;
    [Export]
    public float landingSpeedReduction = 0.75f;
    [Export]
    public int maxDoubleJumpCount = 1;

    public Vector2 velocity;

    public bool IsJumping()
    {
        return !IsOnFloor();
    }

    public override void _PhysicsProcess(float delta)
    {
        GetHorizontalInput();
        ApplyGravity(delta);
        HandleHorizontalMovement();
        HandleJump();
    }

    private void HandleJump()
    {
        bool jumped = Input.IsActionJustPressed("ui_up");
        if (!jumped) return;

        bool isJumping = IsJumping();

        if (!isJumping)
        {
            velocity.y = -jumpHeight;
            doubleJumpCount = maxDoubleJumpCount;
            return;
        }

        if (doubleJumpCount > 0)
        {
            velocity.y = -jumpHeight;
            doubleJumpCount--;
        }
    }

    private void HandleHorizontalMovement()
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private void ApplyGravity(float delta)
    {
        velocity.y += (velocity.y < 0 ? gravity : gravity * landingSpeedReduction) * delta;
    }

    private void GetHorizontalInput()
    {
        float direction = Input.GetActionStrength("ui_right") - 
                Input.GetActionStrength("ui_left");
        velocity.x = Mathf.Abs(direction) > EPSILON
            ? Mathf.Lerp(velocity.x, direction * speed, acceleration)
            : Mathf.Lerp(velocity.x, 0, friction);
    }
}
