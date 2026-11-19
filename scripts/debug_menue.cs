using Godot;

public partial class debug_menue : CanvasLayer
{
    private bool debugVisible = false;
    private Node3D player;
    private Camera3D camera;

    private Label fps;
    private Label position;
    private Label sightRotation;

    public override void _Ready()
    {
        player = GetTree().GetFirstNodeInGroup("player") as Node3D;

        if (player != null)
        {
            camera = player.GetNodeOrNull<Camera3D>("Camera3D");

        }
        Visible = debugVisible;

        // automatic finding of lables under canvaslayer
        foreach (Node node in GetAllChildren(this))
        {
            if (node is Label label)
            {
                GD.Print("F3 Label gefunden: " + label.Name);

                switch (label.Name)
                {
                    case "fps":
                        fps = label;
                        break;

                    case "position":
                        position = label;
                        break;

                    case "sight_rotation":
                        sightRotation = label;
                        break;
                }

            }
        }
                Visible = debugVisible;

        GD.Print("F3 Menü geladen.");
        GD.Print("FPS Label: ", fps != null);
        GD.Print("Position Label: ", position != null);
        GD.Print("Sight Rotation Label: ", sightRotation != null);
    }

    public override void _Process(double delta)
    {
        if (!debugVisible)
            return;

        // FPS
        if (fps != null)
            fps.Text = $"FPS: {Engine.GetFramesPerSecond()}";
        //pos
        if (player != null && position != null)
        {
            Vector3 pos = player.GlobalPosition;

            position.Text =
                $"POSITION: X {pos.X:F1} Y {pos.Y:F1} Z {pos.Z:F1}";
        }
        // sight
        if (camera != null && sightRotation != null)
        {
            Vector3 rot = camera.GlobalRotationDegrees;

            sightRotation.Text =
                $"SIGHT: X {rot.X:F1} Y {rot.Y:F1} Z {rot.Z:F1}";
        }

    }
    

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key &&
            key.Keycode == Key.F3 &&
            key.Pressed &&
            !key.Echo)
        {
            debugVisible = !debugVisible;
            Visible = debugVisible;

            GetViewport().SetInputAsHandled();
        }
    }

    private System.Collections.Generic.List<Node> GetAllChildren(Node parent)
    {
        var result = new System.Collections.Generic.List<Node>();

        foreach (Node child in parent.GetChildren())
        {
            result.Add(child);
            result.AddRange(GetAllChildren(child));
        }

        return result;
    }
}
