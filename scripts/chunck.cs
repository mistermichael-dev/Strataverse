[Tool]
public partial class chunck : StaticBody3D{
    // exported variables
    [Export] public CollisionShape3D CollisionShape { get; set; }
    [Export] public MeshInstance3D MeshInstance { get; set; }
    //chunk size x/y= 32 bloks z=1024 bloks +896to -128 max hill hight +755   
    public static Vector3I dimensions = new Vector3I(16, 64, 16);

    public static readonly Vector3I[] Vertices ={
    new Vector3I(0, 0, 0),
    new Vector3I(1, 0, 0),
    new Vector3I(0, 1, 0),
    new Vector3I(1, 1, 0),
    new Vector3I(0, 0, 1),
    new Vector3I(1, 0, 1),
    new Vector3I(0, 1, 1),
    new Vector3I(1, 1, 1)
    };
    private static readonly int[] top =  new int[] { 2, 2, 7, 6 };
    private static readonly int[] bottom = new int[] { 0, 4 , 5, 1 };
    private static readonly int[] left= new int[] { 6, 4, 0, 2  };
    private static readonly int[] right= new int[] { 3, 1, 5, 7  };
    private static readonly int[] back= new int[] { 7, 5, 4, 6 };
    private static readonly int[] front= new int[] { 2, 0, 1, 3 };
    private SurfaceTool _surfaceTool = new();
    private Block[,,] _blocks = new Block[dimensions.X,dimensions.Y,dimensions.Z];
        public override void _Ready(){
        Generate();
        Update();
    }
    //genrates blocks
    public void Generate(){
        
        for (int x = 0; x < dimensions.X; x++){
            for (int y = 0; y < dimensions.Y; y++) {
                for (int z = 0; z < dimensions.Z; z++){
                    Block block;
                    var groundhight = 40;
                     if (y<groundhight/2){
                        block = BlockManager.Instance.stone;
                     }else if(y<groundhight){
                        block = BlockManager.Instance.dirt;
                     }else if (y==groundhight){
                        block= BlockManager.Instance.grass;
                     }else {
                        block = BlockManager.Instance.Air;
                     }       
                    _blocks[x,y,z]= block;
                }
            }
        }
    }
    //shows Blocks
    public void Update(){
        _surfaceTool.Begin(Mesh.PrimitiveType.Triangles);
        for (int x = 0; x < dimensions.X; x++){
            for (int y = 0; y < dimensions.Y; y++){
                for (int z = 0; z < dimensions.Z; z++){
                    CreateBlockMesh(new Vector3I(x, y, z));
                }
            }
        }
        _surfaceTool.SetMaterial(BlockManager.Instance.ChunkMaterial);
        var mesh =_surfaceTool.Commit();
        MeshInstance.Mesh = mesh;
        CollisionShape.Shape = mesh.CreateTrimeshShape();
    }
    private void CreateBlockMesh(Vector3I blockPosition){
        var block = _blocks[blockPosition.X,blockPosition.Y,blockPosition.Z];
        if (block == BlockManager.Instance.Air) return;
        if (CheckTransperant(blockPosition + Vector3I.Up)){ 
            CreateFaceMesh(top, blockPosition,block.Texture);
        }
        if (CheckTransperant(blockPosition + Vector3I.Down)){
            CreateFaceMesh(bottom, blockPosition, block.Texture);
        }
        if (CheckTransperant(blockPosition + Vector3I.Left)){
            CreateFaceMesh(left, blockPosition, block.Texture);
        }
        if (CheckTransperant(blockPosition + Vector3I.Right)){
            CreateFaceMesh(right, blockPosition, block.Texture);
        }
        if (CheckTransperant(blockPosition + Vector3I.Back)){
            CreateFaceMesh(back, blockPosition, block.Texture);
        }
        if (CheckTransperant(blockPosition + Vector3I.Forward)){
            CreateFaceMesh(front, blockPosition, block.Texture);
        }
 }

    private void CreateFaceMesh(int[] face, Vector3I blockPosition, Texture2D texture) {
        var TexturePosition = BlockManager.Instance.GetTextureAtlasPosition(texture);
        var TextureAtlasSize = BlockManager.Instance.TextureAtlasSize;
        var uvOffset = TexturePosition/TextureAtlasSize;
        var uvWidth = 1f / TextureAtlasSize.X;
        var uvHight = 1f / TextureAtlasSize.Y;
        var uvA = uvOffset+new Vector2(0,0);
        var uvB = uvOffset + new Vector2(0,uvHight);
        var uvC = uvOffset + new Vector2(uvWidth,uvHight);
        var uvD = uvOffset + new Vector2(uvWidth,0);
        var a = Vertices[face[0]] + blockPosition;
        var b = Vertices[face[1]] + blockPosition;
        var c = Vertices[face[2]] + blockPosition;
        var d = Vertices[face[3]] + blockPosition;
        var uvTringel1=new Vector2[] {uvA,uvB,uvC};
        var uvTringel2 = new Vector2[] { uvA, uvC, uvD };
        var Triangel1 = new Vector3[]{a,b,c};
        var Triangel2 = new Vector3[]{a,c,d};
        _surfaceTool.AddTriangleFan(Triangel1,uvTringel1);
        _surfaceTool.AddTriangleFan(Triangel2,uvTringel2);
    }
    private bool CheckTransperant(Vector3I blockPosition ) {
       if (blockPosition.X < 0 ||blockPosition.X >= dimensions.X ) return true;
       if (blockPosition.Y < 0 || blockPosition.Y >= dimensions.Y) return true;
       if (blockPosition.Z < 0 || blockPosition.Z >= dimensions.Z) return true;
       return _blocks [blockPosition.X,blockPosition.Y,blockPosition.Z] == BlockManager.Instance.Air;
    }
}
