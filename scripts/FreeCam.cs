

public partial class FreeCam : Camera3D
{
    [Export] public float Speed = 8.0f;
    [Export] public float Gravity = 20.0f;
    [Export] public float MouseSensitivity = 0.0025f;

    private bool freecam = false;
    private float pitch = 0.0f;
    private float verticalVelocity = 0.0f;

    public override void _Ready()
    {
        Current = false;
    }

    public override void _Process(double delta)
    {
        if (!freecam)
            return;

        float dt = (float)delta;

        Vector3 direction = Vector3.Zero;

        // WASD.
        if (Input.IsKeyPressed(Key.W))
            direction -= GlobalTransform.Basis.Z;

        if (Input.IsKeyPressed(Key.S))
            direction += GlobalTransform.Basis.Z;

        if (Input.IsKeyPressed(Key.A))
            direction -= GlobalTransform.Basis.X;

        if (Input.IsKeyPressed(Key.D))
            direction += GlobalTransform.Basis.X;

        direction.Y = 0;

        if (direction.LengthSquared() > 0)
            direction = direction.Normalized();

        GlobalPosition += direction * Speed * dt;

        // Space = move up.
        if (Input.IsKeyPressed(Key.Space))
        {
            verticalVelocity = Speed;
        }
        // Ctrl = move down.
        else if (Input.IsKeyPressed(Key.Ctrl))
        {
            verticalVelocity = -Speed;
        }
        else
        {
            // Gravity.
            verticalVelocity -= Gravity * dt;
        }

        GlobalPosition += Vector3.Up * verticalVelocity * dt;
    }

    public override void _Input(InputEvent @event)
    {
        // F = toggle freecam.
        if (@event is InputEventKey key &&
            key.Pressed &&
            !key.Echo &&
            key.Keycode == Key.F)
        {
            freecam = !freecam;
            Current = freecam;

            if (freecam)
                verticalVelocity = 0;
        }

        // Mouse look.
        if (freecam && @event is InputEventMouseMotion mouse)
        {
            RotateY(-mouse.Relative.X * MouseSensitivity);

            pitch -= mouse.Relative.Y * MouseSensitivity;
            pitch = Mathf.Clamp(
                pitch,
                Mathf.DegToRad(-89),
                Mathf.DegToRad(89)
            );

            Rotation = new Vector3(pitch, Rotation.Y, 0);
        }
    }
}
