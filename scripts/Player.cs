[GlobalClass]
[Tool]
public partial class Player : CharacterBody3D{

    // stads
    [Export] public float Speed = 10.0f;
    [Export] public float CrouchSpeed = 5.0f;
    [Export] public float JumpVelocity = 5.0f;
    [Export] public float Gravity = 10.0f;
    [Export] public float MouseSensitivity = 0.0025f;

    [Export] public float StandCameraHeight = 1.6f;
    [Export] public float CrouchCameraHeight = 0.9f;

    private Node3D _head;
    private Camera3D _camera;

    private float _yaw;
    private float _pitch;

    private bool _crouching;
    //mause controll
    public override void _Ready(){
        _head = GetNode<Node3D>("head");
        _camera = GetNode<Camera3D>("head/Camera3D");

        Input.MouseMode = Input.MouseModeEnum.Captured;

        _yaw = Rotation.Y;
        _pitch = 0.0f;

        _head.Rotation = Vector3.Zero;
        _camera.Rotation = Vector3.Zero;

        _head.Position = new Vector3(
            0.0f,
            StandCameraHeight,
            0.0f
        );
    }

    public override void _Input(InputEvent @event){
        if (@event is InputEventMouseMotion mouse){
            _yaw -= mouse.Relative.X * MouseSensitivity;
            _pitch -= mouse.Relative.Y * MouseSensitivity;

            _pitch = Mathf.Clamp(
                _pitch,
                Mathf.DegToRad(-89.0f),
                Mathf.DegToRad(89.0f)
            );

            Rotation = new Vector3(
                0.0f,
                _yaw,
                0.0f
            );

            _camera.Rotation = new Vector3(
                _pitch,
                0.0f,
                0.0f
            );
        }

        if (@event is InputEventKey key){
            if (key.Keycode == Key.Escape && key.Pressed){
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }

        if (@event is InputEventMouseButton button){
            if (button.ButtonIndex == MouseButton.Left && button.Pressed){
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }
    // pysics
    public override void _PhysicsProcess(double delta){
        float dt = (float)delta;

        HandleMovement(dt);
        HandleCrouch(dt);
    }
    //W,A,S,D
    private void HandleMovement(float delta) {
        Vector3 direction = Vector3.Zero;

        if (Input.IsKeyPressed(Key.W))
            direction.Z -= 1.0f;

        if (Input.IsKeyPressed(Key.S))
            direction.Z += 1.0f;

        if (Input.IsKeyPressed(Key.A))
            direction.X -= 1.0f;

        if (Input.IsKeyPressed(Key.D))
            direction.X += 1.0f;

        direction = direction.Normalized();

        direction = GlobalTransform.Basis * direction;
        direction.Y = 0.0f;
        direction = direction.Normalized();

        float currentSpeed = _crouching
            ? CrouchSpeed
            : Speed;

        Velocity = new Vector3(
            direction.X * currentSpeed,
            Velocity.Y,
            direction.Z * currentSpeed
        );

        if (!IsOnFloor()) {
            Velocity = new Vector3(
                Velocity.X,
                Velocity.Y - Gravity * delta,
                Velocity.Z
            );
        }
        else if (Velocity.Y < 0.0f){
            Velocity = new Vector3(
                Velocity.X,
                0.0f,
                Velocity.Z
            );
        }

        if (Input.IsKeyPressed(Key.Space) && IsOnFloor()){
            Velocity = new Vector3(
                Velocity.X,
                JumpVelocity,
                Velocity.Z
            );
        }

        MoveAndSlide();
    }

    private void HandleCrouch(float delta){
        _crouching = Input.IsKeyPressed(Key.Ctrl);

        float targetHeight = _crouching
            ? CrouchCameraHeight
            : StandCameraHeight;

        Vector3 position = _head.Position;

        position.Y = Mathf.Lerp(
            position.Y,
            targetHeight,
            10.0f * delta
        );

        _head.Position = position;
    }
}