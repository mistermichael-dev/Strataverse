global using Godot;
public partial class main_meue : Button{
    public override void _Ready()  {
        Pressed += ButtonPressed;
    }

    private void ButtonPressed() {
        GD.Print(Name + " wurde gedrückt!");

        if (Name == "Singelplayer") {
            GetTree().ChangeSceneToFile("res://scenes/singelpalyer.tscn");
        }
        else if (Name == "Multiplayer") {
            GetTree().ChangeSceneToFile("res://scenes/singelpalyer.tscn");
        }
        else if (Name == "Settings") {

            GetTree().ChangeSceneToFile("res://scenes/settings.tscn");
        }
        else if (Name == "Mods") {

            GetTree().ChangeSceneToFile("res://scenes/mods.tscn");
        }
        else if (Name == "Quit"){
            GetTree().Quit();
        }

        else if (Name == "world") {

            GetTree().ChangeSceneToFile("res://scenes/world_movement.tscn");
        }
    }
}
