using System.Collections.Generic;

namespace Services.Game.Tiled
{
    // START - Tileset data
    // -------
    public class TILED_Tileset
    {
        public string name;
        public int tilecount;
        public Dictionary<int, TILED_TilesetItemProperty> tileproperties;
    }

    public partial class TILED_TilesetItemProperty
    {
        public string objectId;
    }
    // -----
    // END - Tileset data


    // START - Map data
    // -------
    public class TILED_Map
    {
        public int height;
        public int width;
        public TILED_Layer[] layers;
        public TILED_MapTileset[] tilesets;
    }

    public class TILED_Layer
    {
        public int[] data;
        public string name;
    }

    public class TILED_MapTileset
    {
        public int firstgid;
        public string source;
    }
    // -----
    // END - Map data
}