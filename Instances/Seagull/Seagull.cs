using Godot;
using System;

public class Seagull : KinematicBody2D
{
    private readonly float DELTA_STEP = 100;
    private readonly float DEFAULT_SPAWN_X = -40;
    private float flyCooldown;
    private float width;
    private float height;
    private bool reversed;
    private float poopDuration;
    private float poopInterval;
    private float difficulty;
    private Node2D tracker;

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
    public float acceleration = 20;
    [Export]
    public PackedScene bloodParticleScene;
    [Export]
    public PackedScene fecesScene;
    [Export]
    public float minPoopDuration = 400;
    [Export]
    public float maxPoopDuration = 1000;

    public Vector2 velocity;

    public override void _Ready()
    {
        width = GetViewportRect().Size.x;
        height = GetViewportRect().Size.y;
        Initialize();
    }

    public override void _PhysicsProcess(float delta)
    {
        ApplyGravity(delta);
        HandleFly(delta);
        HandleFlyForward(delta);
        HandlePoop(delta);
        HandleMovement();
    }

    public void _on_VisibilityNotifier2D_screen_exited()
    {
        Initialize();
    }

    public void Die()
    {
        EmitBlood();
        Initialize();
        difficulty -= 50;
    }

    public void Initialize()
    {
        Random random = new Random(DateTime.Now.Millisecond);

        reversed = (random.Next() & 1) == 1;

        float speedRange = (float)random.NextDouble() * (maxSpeed - minSpeed);
        velocity.x = (speedRange + minSpeed) * (reversed ? -1 : 1);

        float heightY = (float)random.NextDouble() * height + 
            tracker.Position.y - height;
        Position = new Vector2(DEFAULT_SPAWN_X, Mathf.Min(96, heightY));

        float poopDurationRange = (float)random.NextDouble() *
            (maxPoopDuration - minPoopDuration);
        float interval = poopDuration + minPoopDuration;
        poopInterval = Math.Max(interval - difficulty, minPoopDuration);
    }

    public void SetTracker(Node2D tracker)
    {
        this.tracker = tracker;
    }

    private void HandlePoop(float delta)
    {
        poopDuration -= delta * DELTA_STEP;
        if (poopDuration > 0) return;

        Feces feces = (Feces)fecesScene.Instance();
        feces.velocity.x = velocity.x;
        feces.Position = new Vector2(Position);
        GetTree().Root.AddChild(feces);
        poopDuration = poopInterval;
    }

    private void EmitBlood()
    {
        CPUParticles2D bloodParticle = (CPUParticles2D)bloodParticleScene.Instance();
        bloodParticle.Position = new Vector2(Position);
        bloodParticle.Emitting = true;
        bloodParticle.OneShot = true;
        GetTree().Root.AddChild(bloodParticle);
    }

    private void HandleFlyForward(float delta)
    {
        if (Mathf.Abs(velocity.x) >= minSpeed) return;
        velocity.x += (reversed ? -1 : 1) * acceleration * delta;
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
