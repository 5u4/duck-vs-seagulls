using Godot;
using System;

public class Player : KinematicBody2D
{
    private readonly float EPSILON = 0.0001f;
    private readonly float DELTA_STEP = 100;
    private readonly string ENEMY_GROUP = "Enemy";
    private readonly string FECES_GROUP = "Feces";
    private int doubleJumpCount;
    private float attackDuration;
    private float attackCooldown;
    private float width;
    private Camera2D camera;
    private AudioStreamPlayer2D jumpAudio;
    private AudioStreamPlayer2D attackAudio;
    private AudioStreamPlayer2D poopAudio;
    private bool lostSignalEmitted;

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
    public float killCooldownReduction = 0.6f;
    [Export]
    public float cameraLookAheadOffset = 0.3f;
    [Export]
    public NodePath cameraPath;
    [Signal]
    public delegate void DeadSignal();

    public Vector2 velocity;
    public bool isAttacking;
    public bool lost;

    public bool IsJumping()
    {
        return !IsOnFloor();
    }

    public override void _Ready()
    {
        width = GetViewportRect().Size.x;
        camera = GetNode<Camera2D>(cameraPath);
        jumpAudio = GetNode<AudioStreamPlayer2D>("JumpAudio");
        attackAudio = GetNode<AudioStreamPlayer2D>("AttackAudio");
        poopAudio = GetNode<AudioStreamPlayer2D>("PoopAudio");
    }

    public override void _PhysicsProcess(float delta)
    {
        WrapPlayerPosition();
        GetHorizontalInput();
        ApplyGravity(delta);
        HandleJump();
        HandleAttack(delta);
        HandleMovement();
        HandleCameraLookAhead();
        HandleLost();
        CountMaxHeight();
    }

    public void _on_Area2D_body_entered(KinematicBody2D body)
    {
        if (body.IsInGroup(ENEMY_GROUP))
        {
            (body as Seagull).Die();
            camera.Shake();
            Score.instance.KillCount += 1;
            RefreshAfterKill();
        }
        else if (body.IsInGroup(FECES_GROUP))
        {
            lost = true;
            poopAudio.Play();
        }
    }

    private void CountMaxHeight()
    {
        int height = 168 - (int)Position.y;
        Score.instance.MaxHeight = Math.Max(Score.instance.MaxHeight, height);
    }

    private void HandleCameraLookAhead()
    {
        camera.OffsetV = Math.Sign(velocity.y) * cameraLookAheadOffset;
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

    private void HandleLost()
    {
        if (!lost || !IsOnFloor() || lostSignalEmitted) return;
        EmitSignal(nameof(DeadSignal));
        lostSignalEmitted = true;
    }

    private void HandleJump()
    {
        if (!Input.IsActionJustPressed("ui_up") || isAttacking || lost) return;

        bool isJumping = IsJumping();

        if (!isJumping)
        {
            velocity.y = -jumpHeight;
            doubleJumpCount = maxDoubleJumpCount;
            jumpAudio.Play();
            return;
        }

        if (doubleJumpCount > 0)
        {
            velocity.y = -jumpHeight;
            doubleJumpCount--;
            jumpAudio.Play();
        }
    }

    private void HandleMovement()
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private void ApplyGravity(float delta)
    {
        if (isAttacking && !lost) return;
        velocity.y += (velocity.y < 0 ? gravity : gravity * landingSpeedReduction) * delta;
    }

    private void GetHorizontalInput()
    {
        if (isAttacking) return;
        float direction = lost ? 0 : Input.GetActionStrength("ui_right") - 
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
        if (!Input.IsActionJustPressed("ui_attack") || attackCooldown > 0 || lost) return;

        // Initiate attack
        isAttacking = true;
        attackCooldown = maxAttackCooldown;
        attackDuration = maxAttackDuration;
        Vector2 mousePosition = GetGlobalMousePosition();
        LookAt(mousePosition);
        velocity = Position.DirectionTo(mousePosition) * attackSpeed;
        SetCollisionMaskBit(1, false);
        SetCollisionLayerBit(1, false);
        attackAudio.Play();
    }
}
