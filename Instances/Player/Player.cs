using Godot;
using System;

public class Player : KinematicBody2D
{
    private readonly float EPSILON = 0.0001f;
    private readonly float DELTA_STEP = 100;
    private readonly string ENEMY_GROUP = "Enemy";
    private int doubleJumpCount;
    private float attackDuration;
    private float attackCooldown;
    private float width;

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
    [Export]
    public float attackSpeed = 600;
    [Export]
    public float maxAttackCooldown = 120;
    [Export]
    public float maxAttackDuration = 15;
    [Export]
    public float attackFinishSpeedReduction = 0.1f;
    [Export]
    public float killCooldownReduction = 0.3f;

    public Vector2 velocity;
    public bool isAttacking;

    public bool IsJumping()
    {
        return !IsOnFloor();
    }

    public override void _Ready()
    {
        width = GetViewportRect().Size.x;
    }

    public override void _PhysicsProcess(float delta)
    {
        WrapPlayerPosition();
        GetHorizontalInput();
        ApplyGravity(delta);
        HandleJump();
        HandleAttack(delta);
        HandleMovement();
    }

    public void _on_Area2D_body_entered(KinematicBody2D body)
    {
        if (!body.IsInGroup(ENEMY_GROUP)) return;
        (body as Seagull).Die();
        RefreshAfterKill();
    }

    private void WrapPlayerPosition()
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

    private void RefreshAfterKill()
    {
        attackCooldown -= maxAttackCooldown * killCooldownReduction;
        doubleJumpCount = maxDoubleJumpCount;
    }

    private void HandleJump()
    {
        if (!Input.IsActionJustPressed("ui_up") || isAttacking) return;

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

    private void HandleMovement()
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private void ApplyGravity(float delta)
    {
        if (isAttacking) return;
        velocity.y += (velocity.y < 0 ? gravity : gravity * landingSpeedReduction) * delta;
    }

    private void GetHorizontalInput()
    {
        if (isAttacking) return;
        float direction = Input.GetActionStrength("ui_right") - 
                Input.GetActionStrength("ui_left");
        velocity.x = Mathf.Abs(direction) > EPSILON
            ? Mathf.Lerp(velocity.x, direction * speed, acceleration)
            : Mathf.Lerp(velocity.x, 0, friction);
    }

    private void HandleAttack(float delta)
    {
        // Reduce attacking time counter
        attackDuration -= delta * DELTA_STEP;
        attackCooldown -= delta * DELTA_STEP;

        // Finish attack
        if (isAttacking && attackDuration <= 0)
        { 
            isAttacking = false;
            velocity.x *= attackFinishSpeedReduction;
            velocity.y *= attackFinishSpeedReduction;
            Rotation = 0;
            SetCollisionMaskBit(1, true);
            SetCollisionLayerBit(1, true);
            return;
        }

        // Attack not triggered
        if (!Input.IsMouseButtonPressed((int)ButtonList.Left) || attackCooldown > 0) return;

        // Initiate attack
        isAttacking = true;
        attackCooldown = maxAttackCooldown;
        attackDuration = maxAttackDuration;
        Vector2 mousePosition = GetGlobalMousePosition();
        LookAt(mousePosition);
        velocity = Position.DirectionTo(mousePosition) * attackSpeed;
        SetCollisionMaskBit(1, false);
        SetCollisionLayerBit(1, false);
    }
}
