[GlobalClass]
public partial class Inventory : CanvasLayer
{
    public bool IsOpen { get; private set; }

    public override void _Ready()
    {
        IsOpen = false;
        Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key &&
            key.Keycode == Key.E &&
            key.Pressed &&
            !key.Echo)
        {
            IsOpen = !IsOpen;

            if (IsOpen)
            {
                Show();
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Hide();
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }
}
