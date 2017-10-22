using Zenject;
using Services.Core;
using Services.Core.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Services.Game.Tiled
{
    /// <summary>
    /// Adapter that will read Tiled data structures 
    /// The assumption is that the folder containing the tilesets will always be in the same folder of the levels
    /// </summary>

    public class TILED_DataProvider
    {
        [Inject] DatabaseService database;

        private Dictionary<string, TILED_Map> mapCache = new Dictionary<string, TILED_Map>();
        private Dictionary<string, TILED_Tileset> tilesetCache = new Dictionary<string, TILED_Tileset>();

        public TILED_Map GetMap(string path)
        {
            if (!mapCache.ContainsKey(path))
            {
                var map = Utils.ReadJsonFromResources<TILED_Map>(path);
                mapCache.Add(path, map);

                LogWrapper.DebugLog("[{0}] Loaded and cached map from resources {1}", GetType(), path);
            }

            return mapCache[path];
        }

        public TILED_Tileset GetTileset(string path)
        {
            if (!tilesetCache.ContainsKey(path))
            {
                var tileset = Utils.ReadJsonFromResources<TILED_Tileset>(path);
                tilesetCache.Add(path, tileset);

                LogWrapper.DebugLog("[{0}] Loaded and cached tileset from resources {1}", GetType(), path);
            }

            return tilesetCache[path];
        }

        public void UnloadMaps()
        {
            mapCache.Clear();
        }

        public void UnloadTilesets()
        {
            tilesetCache.Clear();
        }

        public IntVector2 GetMapSize(string path)
        {
            var map = GetMap(path);
            return new IntVector2(map.width, map.height);
        }

        public TILED_Layer[] GetMapLayers(string path)
        {
            return GetMap(path).layers;
        }

        public TILED_Layer GetMapLayer(string path, int layerIndex)
        {
            return GetMapLayers(path)[layerIndex];
        }

        public TILED_Layer GetMapLayer(string path, string name)
        {
            return GetMap(path).layers.ToList().Find(l => !string.IsNullOrEmpty(name) && l.name.Equals(name));
        }

        public TILED_MapTileset GetMapTilesetWithFirstgid(string mapPath, int gid)
        {
            if (gid == 0)
            {
                LogWrapper.Error("[{0}] the gid 0 is dedicated to void cells with no TILED_MapTileset, will return null", GetType());
                return null;
            }

            var map = GetMap(mapPath);

            if (gid > 0 && map.tilesets.Length == 1 && gid >= map.tilesets[0].firstgid)
            {
                return map.tilesets[0];
            }
            else if (map.tilesets.Length > 1)
            {
                for (int i = 0; i < map.tilesets.Length - 1; i++)
                {
                    var tl_1 = map.tilesets[i];
                    var tl_2 = map.tilesets[i + 1];

                    if (gid >= tl_1.firstgid && gid < tl_2.firstgid)
                    {
                        return tl_1;
                    }
                }

                if (gid >= map.tilesets[map.tilesets.Length - 1].firstgid)
                    return map.tilesets[map.tilesets.Length - 1];
            }

            return null;
        }

        public TILED_Tileset GetMapTileset(string mapPath, int gid)
        {
            var mapTileset = GetMapTilesetWithFirstgid(mapPath, gid);
            var tilesetPath = Path.Combine(database.Get<string>(Constants.TILED_MAP_DIR_ID), mapTileset.source.Replace(Constants.JSON_FILE_EXTENSION, ""));
            return GetTileset(tilesetPath);
        }

        public TILED_TilesetItemProperty GetMapTilesetProperties(string mapPath, int gid)
        {
            var mapTileset = GetMapTilesetWithFirstgid(mapPath, gid);
            return GetMapTileset(mapPath, gid).tileproperties[gid - mapTileset.firstgid];
        }


        // START Getter for all map tileser properties
        // -------------------------------------------

        public string GetMapTilesetObjectId(string mapPath, int gid)
        {
            return GetMapTilesetProperties(mapPath, gid).objectId;
        }

        public string GetMapTilesetTileTypeId(string mapPath, int gid)
        {
            return GetMapTilesetProperties(mapPath, gid).tileId;
        }

        // -------------------------------------------
        // END   Getter for all map tileser properties
    }
}