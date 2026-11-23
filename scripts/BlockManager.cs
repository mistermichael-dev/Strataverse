using System.Collections.Generic;
using System.Linq;
[Tool]
public partial class BlockManager : Node
{
    [Export] public Block Air { get; set; }
    [Export] public Block stone { get; set; }
    [Export] public Block dirt { get; set; }
    [Export] public Block grass { get; set; }
    private readonly Dictionary<Texture2D, Vector2I> _atlaslookup = new();
    private int _gridWiht = 4;
    private int _gridHight;
    public Vector2I BlockTextureSize { get; } = new(16, 16);
    public Vector2 TextureAtlasSize { get; private set; }
    public static BlockManager Instance { get; private set; }
    public StandardMaterial3D ChunkMaterial { get; private set; }
    public override void _Ready()
    {
        Instance = this;
        var blockTaxture = new Block[] { Air, stone, dirt, grass, }.Select(block => block.Texture).Where(Texture => Texture != null).Distinct().ToArray();
        for (int i = 0; i < blockTaxture.Length; i++)
        {
            var Texture = blockTaxture[i];
            _atlaslookup.Add(Texture, new Vector2I(i % _gridWiht, Mathf.FloorToInt(i / _gridWiht)));
        }
        _gridHight = Mathf.CeilToInt(blockTaxture.Length / (float)_gridWiht);
        var image = Image.CreateEmpty(_gridWiht * BlockTextureSize.X, _gridHight * BlockTextureSize.Y, false, Image.Format.Rgba8);
        for (var x = 0; x < _gridWiht; x++) {
            for (var y = 0; y < _gridHight; y++) {
                var imgIndex = x + y * _gridWiht;
                if (imgIndex >= blockTaxture.Length) continue;
                var currentimage = blockTaxture[imgIndex].GetImage();
                currentimage.Convert(Image.Format.Rgba8);
                image.BlitRect(currentimage, new Rect2I(Vector2I.Zero, BlockTextureSize), new Vector2I(x, y) * BlockTextureSize);
            }
        }
        var textutreAtlas = ImageTexture.CreateFromImage(image);
        ChunkMaterial = new() {
            AlbedoTexture = textutreAtlas,
            TextureFilter=BaseMaterial3D.TextureFilterEnum.Nearest
        };
            TextureAtlasSize=new Vector2(_gridWiht, _gridHight);
    GD.Print($"done loading{ blockTaxture.Length}images to make{_gridWiht}x{_gridHight}Atlas");
    }
    public Vector2I GetTextureAtlasPosition(Texture2D texture){
        if (texture == null) {
            return Vector2I.Zero;
        } else {
            return _atlaslookup[texture];
        }
    }
}