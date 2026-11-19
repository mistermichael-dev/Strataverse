

public partial class ThirdPersoncam : Camera3D
{
   //variables
    private Camera3D firstPersonCamera;
    private Node3D player;

    [Export] public float MouseSensitivity = 0.0025f;

    private float pitch = 0.0f;

    public override void _Ready()
    {
        player = GetParent<Node3D>();

        firstPersonCamera = FindFirstPersonCamera(player);

        if (firstPersonCamera == null)
        {
            GD.PrintErr("First-person Camera3D not found!");
            return;
        }

        // Start in first person
        firstPersonCamera.Current = true;
        Current = false;

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    private Camera3D FindFirstPersonCamera(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Camera3D camera && camera != this)
                return camera;

            Camera3D found = FindFirstPersonCamera(child);

            if (found != null)
                return found;
        }

        return null;
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse look
        if (@event is InputEventMouseMotion mouseMotion)
        {
            // Left/right
            player.RotateY(-mouseMotion.Relative.X * MouseSensitivity);

            // Up/down
            pitch -= mouseMotion.Relative.Y * MouseSensitivity;

            // Prevent flipping upside down
            pitch = Mathf.Clamp(
                pitch,
                Mathf.DegToRad(-89),
                Mathf.DegToRad(89)
            );

            Rotation = new Vector3(pitch, 0, 0);

            // Keep first-person camera looking up/down too
            if (firstPersonCamera != null)
            {
                firstPersonCamera.Rotation = new Vector3(pitch, 0, 0);
            }
        }

        // F5 = switch cameras
        if (@event is InputEventKey key &&
            key.Pressed &&
            !key.Echo &&
            key.Keycode == Key.F5)
        {
            ToggleCamera();
        }
    }

    private void ToggleCamera()
    {
        if (firstPersonCamera == null)
            return;

        if (firstPersonCamera.Current)
        {
            // First person -> third person
            firstPersonCamera.Current = false;
            Current = true;
        }
        else
        {
            // Third person -> first person
            Current = false;
            firstPersonCamera.Current = true;
        }
    }
}
