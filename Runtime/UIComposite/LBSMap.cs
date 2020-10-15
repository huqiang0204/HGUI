using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using huqiang.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class LBSMap : Composite
    {
        public struct LngLat
        {
            public double Longitude;
            public double Latitude;
            public LngLat(double lng, double lat) { Longitude = lng; Latitude = lat; }
        }
        public struct TileInfo
        {
            public int x;
            public int y;
            public int z;
        }
        class Item
        {
            public GameObject Game;
            public HImage Image;
            public string Tile;
            public TileInfo Info;
        }
        public int Column = 1;
        public int Row = 0;
        /// <summary>
        /// 当前滚动的位置
        /// </summary>
        public Vector2 Position;
        /// <summary>
        /// 事件
        /// </summary>
        public UserEvent eventCall;
        FakeStruct ItemMod;
        List<Item> Items = new List<Item>();
        public override void Initial(FakeStruct fake, UIElement script)
        {
            base.Initial(fake, script);
            HGUIManager.GameBuffer.RecycleChild(Enity.gameObject);
            ItemMod = HGUIManager.FindChild(BufferData, "Item");
            eventCall = script.RegEvent<GestureEvent>();
            eventCall.ForceEvent = true;
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.CutRect = true;
            Enity.SizeChanged = (o) =>
            {
                UpdateData();
            };
            UpdateData();
        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            offsetX -= v.x;
            offsetY += v.y;
            if(offsetX<0)
            {
                cx--;
                offsetX += 256;
            }else if(offsetX>=256)
            {
                cx++;
                offsetX -= 256;
            }
            if (offsetY < 0)
            {
                cy++;
                offsetY += 256;
            }
            else if (offsetY >= 256)
            {
                cy--;
                offsetY -= 256;
            }
            UpdateData();
        }
        LngLat latlng;
        public override void Update(float time)
        {
#if !UNITY_EDITOR
            if (Input.location.status == LocationServiceStatus.Stopped)
            { 
                Input.location.Start(10, 10); 
            }
            else if (Input.location.status == LocationServiceStatus.Initializing)
            {
                //Debug.Log("Initializing");
            }
            else
            if (Input.location.status == LocationServiceStatus.Running)
            {
                latlng.Longitude = Input.location.lastData.longitude;
                latlng.Latitude = Input.location.lastData.latitude;
                //Debug.Log("gps running");
            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {
                //Debug.Log("Failed");
            }
#endif
        }
        public int Level = 18;
        int cx = 49310;
        int cy = 10242;
        float offsetX;
        float offsetY;
        TilePos tilePos;
        public void UpdateData()
        {
            Vector2 size = Enity.m_sizeDelta;
            int c = (int)(size.x / 256);
            c++;
            int r = (int)(size.y / 256);
            r++;
            int all = c * r;//总计UI数量
            int ic = Items.Count;
            for (int i = ic; i < all; i++)
            {
                Item item = new Item();
                var go = HGUIManager.GameBuffer.Clone(ItemMod);
                var trans = go.transform;
                trans.SetParent(Enity.transform);
                trans.localScale = Vector3.one;
                trans.localRotation = Quaternion.identity;
                item.Game = go;
                item.Image = go.GetComponent<HImage>();
                Items.Add(item);
            }
            for (int i = ic - 1; i >= all; i--)
            {
                var item = Items[i];
                HGUIManager.GameBuffer.RecycleGameObject(item.Game);
                Items.RemoveAt(i);
            }
            List<TileInfo> infos = new List<TileInfo>();
            TileInfo tile = new TileInfo();
            int sx = cx - c / 2;
            int sy = cy - r / 2;
            float ox = -c / 2;
            ox *= 256;
            ox -= offsetX;
            float oy = -r / 2;
            oy *= 256;
            oy += offsetY;
            int s = 0;
            for (int i = 0; i < c; i++)
                for (int j = 0; j < r; j++)
                {
                    tile.x = sx + i;
                    tile.y = sy + j;
                    tile.z = Level;
                    string name = tile.x + "-" + tile.y + "-" + Level;
                    var item = Items[s];
                    item.Tile = name;
                    item.Game.transform.localPosition = new Vector3(ox + i * 256, oy + j * 256, 0);
                    infos.Add(tile);
                    BaiduMap.GetTileMap(tile.x, tile.y, Level, name, UpdateTexture, item);
                    s++;
                }
        }
        void UpdateTexture(string file, string name, object obj)
        {
            var item = obj as Item;
            if (item.Tile == name)
            {
                Texture2D t2d = item.Image.MainTexture as Texture2D;
                if (t2d == null)
                    t2d = new Texture2D(1, 1);
                t2d.LoadImage(File.ReadAllBytes(file));
                item.Image.MainTexture = t2d;
            }
        }
        public void Location()
        {
            BaiduMap.GPSToTile(latlng.Longitude, latlng.Latitude, Level, (o) => {
                cx = o.x;
                cy = o.y;
                offsetX = o.ox;
                offsetY = o.oy;
                UpdateData();
            });
        }
        public void Location(double x, double y)
        {
            latlng.Longitude = x;
            latlng.Latitude = y;
            BaiduMap.GPSToTile(x, y, Level, (o) => {
                cx = o.x;
                cy = o.y;
                offsetX = o.ox;
                offsetY = o.oy;
                UpdateData();
            });
        }
        public void SetLevel(int level)
        {
            if (level < 3)
                level = 3;
            else if (level > 18)
                level = 18;
            Level = level;
            Location();
        }
    }
}
