using Godot;
using System;

/*
 * Script from https://kidscancode.org/godot_recipes/2d/screen_shake/
 */

public class Camera2D : Godot.Camera2D
{
    private float trauma;
    private readonly float traumaPower = 1.5f;
    private Vector2 offset;
    private Random random;
    private OpenSimplexNoise noise = new OpenSimplexNoise();
    private int count = 0;

    [Export]
    public float decay = 12f;
    [Export]
    public Vector2 maxOffset = new Vector2(3, 1);

    public override void _Ready()
    {
        random = new Random(DateTime.Now.Millisecond);
        noise.Seed = DateTime.Now.Millisecond;
        noise.Period = 4;
        noise.Octaves = 2;
    }

    public override void _Process(float delta)
    {
        if (trauma > 0)
        {
            trauma = Mathf.Max(trauma - decay * delta, 0);
            AddShakingEffect();
        }
    }

    public void Shake()
    {
        trauma = Mathf.Min(trauma + 2f, 5f);
    }

    private void AddShakingEffect()
    {
        count++;
        float amount = Mathf.Pow(trauma, traumaPower);
        offset.x = maxOffset.x * amount * noise.GetNoise2d(noise.Seed * 2, count);
        offset.y = maxOffset.y * amount * noise.GetNoise2d(noise.Seed * 3, count);
        Offset = offset;
    }
}
