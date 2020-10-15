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
            public string Name;
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
        public GestureEvent eventCall;
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
            eventCall.TowFingerPressd = SetCenter;
            eventCall.TowFingerMove = Scale;
            eventCall.TowFingerUp = ScaleEnd;
        }
        void SetCenter(GestureEvent gesture)
        {
            Vector3 pc = Vector3.Lerp(gesture.RawPos0, gesture.RawPos1, 0.5f);
            var pos = gesture.GlobalPosition;
            Vector3 cc = pc - pos;
        }
        void Scale(GestureEvent gesture)
        {
            //gesture.CurScale
          
        }
        void ScaleEnd(GestureEvent gesture)
        {
            //gesture.CurScale

        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            tilePos.ox -= v.x;
            tilePos.oy += v.y;
            if(tilePos.ox <0)
            {
                tilePos.x--;
                tilePos.ox += 256;
            }else if(tilePos.ox >=256)
            {
                tilePos.x++;
                tilePos.ox -= 256;
            }
            if (tilePos.oy < 0)
            {
                tilePos.y++;
                tilePos.oy += 256;
            }
            else if (tilePos.oy >= 256)
            {
                tilePos.y--;
                tilePos.oy -= 256;
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
        TilePos tilePos;
        public void UpdateData()
        {
            Vector2 size = Enity.m_sizeDelta;
            int c = (int)(size.x / 256);
            c+=2;
            int r = (int)(size.y / 256);
            r+=2;
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
            int sx = tilePos.x - c / 2;
            int sy = tilePos.y - r / 2;
            for (int i = 0; i < c; i++)
                for (int j = 0; j < r; j++)
                {
                    tile.x = sx + i;
                    tile.y = sy + j;
                    tile.z = Level;
                    infos.Add(tile);
                }
            UpdateItems(infos);
        }
        void UpdateItems(List<TileInfo> infos)
        {
            Vector2 size = Enity.m_sizeDelta;
            int c = (int)(size.x / 256);
            c+=2;
            int r = (int)(size.y / 256);
            r+=2;
            int sx = tilePos.x - c / 2;
            int sy = tilePos.y - r / 2;
            float ox = -c / 2 + 1;
            ox *= 256;
            ox -= tilePos.ox;
            float oy = -r / 2;
            oy *= 256;
            oy += tilePos.oy;
            int ic = Items.Count;
            List< Item> tmp = new List<Item>();
            for (int i = ic - 1; i >= 0; i--)
            {
                var item = Items[i];
                for (int j = 0; j < infos.Count; j++)
                {
                    if (infos[j].x == item.Info.x)
                        if (infos[j].y == item.Info.y)
                            if (infos[j].z == item.Info.z)
                            {
                                tmp.Add (item);
                                int a = infos[j].x - sx;
                                int b = infos[j].y - sy;
                                item.Game.transform.localPosition = new Vector3(ox + a * 256, oy +b * 256, 0);
                                Items.RemoveAt(i);
                                infos.RemoveAt(j);
                                break;
                            }
                }
            }
            ic = infos.Count;
            for (int i = 0; i < ic; i++)
            {
                var tile = infos[i];
                string name = tile.x + "-" + tile.y + "-" + Level;
                var item = Items[i];
                item.Info = tile;
                item.Name = name;
                int a = tile.x - sx;
                int b = tile.y - sy;
                item.Game.transform.localPosition = new Vector3(ox + a * 256, oy + b * 256, 0);
                infos.Add(tile);
                BaiduMap.GetTileMap(tile.x, tile.y, Level, name, UpdateTexture, item);
            }
            Items.AddRange(tmp);
        }
        void UpdateTexture(string file, string name, object obj, byte[] dat)
        {
            var item = obj as Item;
            if (item.Name == name)
            {
                Texture2D t2d = item.Image.MainTexture as Texture2D;
                if (t2d == null)
                    t2d = new Texture2D(1, 1);
                if (dat == null)
                    t2d.LoadImage(File.ReadAllBytes(file));
                else t2d.LoadImage(dat);
                item.Image.MainTexture = t2d;
            }
        }
        public void Location()
        {
            BaiduMap.GPSToTile(latlng.Longitude, latlng.Latitude, Level, (o) => {
                tilePos = o;
                UpdateData();
            });
        }
        public void Location(double x, double y)
        {
            latlng.Longitude = x;
            latlng.Latitude = y;
            BaiduMap.GPSToTile(x, y, Level, (o) => {
                tilePos = o;
                UpdateData();
            });
        }
        public void SetLevel(int level)
        {
            if (level < 3)
                level = 3;
            else if (level > 18)
                level = 18;
            if(Level!=level)
            {
                var ll = BaiduMap.TileToMercato(ref tilePos, Level);
                Level = level;
                tilePos = BaiduMap.MercatoToTile(ll,Level);
                UpdateData();
            }
        }
    }
}
