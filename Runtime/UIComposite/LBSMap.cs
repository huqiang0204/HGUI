using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class LBSMap: Composite
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
   
        //将tile(瓦片)坐标系转换为LatLngt(地理)坐标系，pixelX，pixelY为图片偏移像素坐标
        public static LngLat TileXYToLatLng(int tileX, int tileY, int zoom, int pixelX = 0, int pixelY = 0)
        {
            double size = Math.Pow(2, zoom);
            double pixelXToTileAddition = pixelX / 256.0;
            double lng = (tileX + pixelXToTileAddition) / size * 360.0 - 180.0;

            double pixelYToTileAddition = pixelY / 256.0;
            double lat = Math.Atan(Math.Sinh(Math.PI * (1 - 2 * (tileY + pixelYToTileAddition) / size))) * 180.0 / Math.PI;
            return new LngLat(lng, lat);
        }
        //将LatLngt地理坐标系转换为tile瓦片坐标系，pixelX，pixelY为图片偏移像素坐标
        public static void LatLngToTileXY(LngLat latlng, int zoom, out int tileX, out int tileY, out int pixelX, out int pixelY)
        {
            double size = Math.Pow(2, zoom);
            double x = ((latlng.Longitude + 180) / 360) * size;
            double lat_rad = latlng.Latitude * Math.PI / 180;
            double y = (1 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2;
            y = y * size;

            tileX = (int)x;
            tileY = (int)y;
            pixelX = (int)((x - tileX) * 256);
            pixelY = (int)((y - tileY) * 256);
        }
        static void GetTileMap(int tileX,  int tileY, int zoom, Action<string,string, Item> action, Item context)
        {
            int z = zoom;
            string file = context.Tile;
            string folder = Application.persistentDataPath + "/map";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string path = folder + "/" + file;
            if (File.Exists(path))
            {
                action(path,file, context);
                return;
            }
            string http = string.Format("http://online0.map.bdimg.com/onlinelabel/?qt=tile&x={0}&y={1}&z={2}", tileX, tileY, z);
            Http.HttpControl.DownloadAsync(http, path, null, (o) => {
                if (o.Code == Http.ResultCode.OK)
                {
                    o.responseStream.Dispose();
                    action(path, file, context); 
                }
            });
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
            ItemMod= HGUIManager.FindChild(BufferData, "Item");
            eventCall = script.RegEvent<UserEvent>();
            eventCall.ForceEvent = true;
            eventCall.Drag = (o, e, s) => { Scrolling(o, s); };
            eventCall.CutRect = true;
            Enity.SizeChanged = (o) =>
            {
                Refresh(Position);
            };
            UpdateData();
        }
        void Scrolling(UserEvent back, Vector2 v)
        {
            offsetX -= v.x;
            offsetY += v.y;
            UpdateData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">视口尺寸</param>
        /// <param name="pos">视口位置</param>
        public void Order(bool force = false)
        {
            //float w = Size.x;
            //float h = Size.y;
            //float left = Position.x;
            //float ls = left - ItemSize.x;
            //float right = Position.x + w;
            //float rs = right + ItemSize.x;
            //float top = Position.y + h;//与unity坐标相反
            //float ts = top + ItemSize.y;
            //float down = Position.y;//与unity坐标相反
            //float ds = down - ItemSize.y;
            //RecycleOutside(left, right, down, top);
            //int colStart = (int)(left / ItemSize.x);
            //if (colStart < 0)
            //    colStart = 0
            //int colEnd = (int)(rs / ItemSize.x);
            //if (colEnd > Column)
            //    colEnd = Column;
            //int rowStart = (int)(down / ItemSize.y);
            //if (rowStart < 0)
            //    rowStart = 0;
            //int rowEnd = (int)(ts / ItemSize.y);
            //if (rowEnd > Row)
            //    rowEnd = Row;
            //for (; rowStart < rowEnd; rowStart++)
            //    UpdateRow(rowStart, colStart, colEnd, force);
        }
        /// <summary>
        /// 刷新到指定位置
        /// </summary>
        /// <param name="pos"></param>
        public void Refresh(Vector2 pos)
        {
            Position = pos;
       
        }
        /// <summary>
        /// 刷新到默认位置
        /// </summary>
        public void Refresh()
        {
            Order(true);
        }
        LngLat latlng;
        public override void Update(float time)
        {
            if (Input.location.isEnabledByUser)
            {
                if (Input.location.status == LocationServiceStatus.Stopped)
                    Input.location.Start();
                if (Input.location.status == LocationServiceStatus.Running)
                {
                    latlng.Longitude = Input.location.lastData.longitude;
                    latlng.Latitude = Input.location.lastData.latitude;
                }
            }
        }
        int cc = 49310;
        int cr = 10242;
        int cz = 18;
        float offsetX;
        float offsetY;
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
            int sx =cc- c / 2;
            int sy =cr- r / 2;
            float ox = -c / 2;
            ox *= 256;
            ox -= offsetX;
            float oy = -r / 2;
            oy *= 256;
            oy += offsetY;
            int s = 0;
            for(int i=0;i<c;i++)
                for(int j=0;j<r;j++)
                {
                    tile.x = sx + i;
                    tile.y = sy + j;
                    tile.z = cz;
                    string name = tile.x + "-" + tile.y + "-" + cz;
                    var item = Items[s];
                    item.Tile = name;
                    item.Game.transform.localPosition = new Vector3(ox + i * 256, oy + j * 256, 0);
                    infos.Add(tile);
                    GetTileMap(tile.x,tile.y,cz,UpdateTexture,item);
                    s++;
                }
        }
        void UpdateTexture(string file,string name, Item item)
        {
            if(item.Tile==name)
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
            int ox = 0;
            int oy = 0;
            LatLngToTileXY(latlng, cz, out cc, out cr, out ox, out oy);
            offsetX = ox;
            offsetY = oy;
            UpdateData();
        }
    }
}
