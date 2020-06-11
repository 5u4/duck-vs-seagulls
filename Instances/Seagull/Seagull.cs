using Godot;
using System;

public class Seagull : KinematicBody2D
{
    private readonly float DELTA_STEP = 100;
    private readonly float DEFAULT_SPAWN_X = -40;
    private float flyCooldown;
    private float width;
    private bool reversed;
    private PackedScene bloodParticleScene;

    [Export]
    public float gravity = 300;
    [Export]
    public float minSpeed = 45;
    [Export]
    public float maxSpeed = 110;
    [Export]
    public float flyHeight = 45;
    [Export]
    public float maxFlyCooldown = 30;
    [Export]
    public float minHeight = -65;

    public Vector2 velocity;

    public override void _Ready()
    {
        bloodParticleScene = (PackedScene)GD.Load("res://Particles/BloodParticle.tscn");
        width = GetViewportRect().Size.x;
        Initialize();
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
        EmitBlood();
        Initialize();
    }

    private void EmitBlood()
    {
        CPUParticles2D bloodParticle = (CPUParticles2D)bloodParticleScene.Instance();
        bloodParticle.Position = new Vector2(Position);
        bloodParticle.Emitting = true;
        bloodParticle.OneShot = true;
        GetTree().Root.AddChild(bloodParticle);
    }

    private void Initialize()
    {
        Random random = new Random(DateTime.Now.Millisecond);

        reversed = (random.Next() & 1) == 1;

        float speedRange = (float)random.NextDouble() * (maxSpeed - minSpeed);
        velocity.x = (speedRange + minSpeed) * (reversed ? -1 : 1);

        float heightRange = (float)random.NextDouble() * (minHeight);
        Position = new Vector2(DEFAULT_SPAWN_X, heightRange - minHeight);
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
