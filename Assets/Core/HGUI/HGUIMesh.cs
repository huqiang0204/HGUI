using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGUIMesh
    {
        public static int[] Rectangle = new int[] { 0, 2, 3, 0, 3, 1 };
        public static int[] FourRectangle = new int[] { 0,3,4,0,4,1,1,4,5,1,5,2,3,6,7,3,7,4,4,7,8,4,8,5 };
        public static int[] FourCorners = new int[] { 0,2,3,0,3,1,4,6,7,4,7,5,8,10,11,8,11,9,12,14,15,12,15,13};
        public static int[] ElevenRectangle = new int[] {
        0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2, 2, 6, 7, 2, 7, 3,
            4, 8, 9, 4, 9, 5, 6, 10, 11, 6, 11, 7,
            8, 12, 13, 8, 13, 9, 9, 13, 14, 9, 14, 10, 10, 14, 15, 10, 15, 11};
        public static int[] TwelveRectangle = new int[] {
            0, 4, 5, 0, 5, 1, 1, 5, 6, 1, 6, 2, 2, 6, 7, 2, 7, 3,
            4, 8, 9, 4, 9, 5,5,9,10,5,10,6, 6, 10, 11, 6, 11, 7,
            8, 12, 13, 8, 13, 9, 9, 13, 14, 9, 14, 10, 10, 14, 15, 10, 15, 11 };
        public static void CreateMesh(HImage image)
        {
            if (image._sprite == null)
            {
                CreateSimpleVertex(image);
                return;
            }
            switch (image.type)
            {
                case SpriteType.Simple://单一类型
                    CreateSimpleMesh(image);
                    break;
                case SpriteType.Sliced://9宫格,中间部分为拉伸
                    CreateSlicedMesh(image);
                    break;
                case SpriteType.Filled://填充类型
                    CreateFilledMesh(image);
                    break;
                case SpriteType.Tiled://平铺
                    //CreateFilledMesh(image);
                    break;
            }
        }
        public static void CreateTriangle(HImage image)
        {
            switch (image.type)
            {
                case SpriteType.Simple://单一类型
                    image.tris = Rectangle;
                    break;
                case SpriteType.Sliced://9宫格,中间部分为拉伸
                    if (image._pixelsPerUnit == 0)
                    {
                        image.tris = FourRectangle;
                    }
                    else
                    {
                        if (image._fillCenter)
                            image.tris = TwelveRectangle;
                        else image.tris = ElevenRectangle;
                    }
                    break;
                case SpriteType.Filled://填充类型

                    break;
                case SpriteType.Tiled://平铺

                    break;
            }
        }
        static void CreateSimpleVertex(HImage image)
        {
            var vertex = new Vector3[4];
            float x = image.SizeDelta.x;
            float lx = -0.5f * x;
            float rx = 0.5f * x;
            float y = image.SizeDelta.y;
            float dy = -0.5f * y;
            float ty = 0.5f * y;
            vertex[0].x = lx;
            vertex[0].y = dy;
            vertex[1].x = rx;
            vertex[1].y = dy;
            vertex[2].x = lx;
            vertex[2].y = ty;
            vertex[3].x = rx;
            vertex[3].y = ty;
            image.vertex = vertex;
            image.tris = Rectangle;
        }
        static void CreateSimpleMesh(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            var vertex = new Vector3[4];
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            vertex[0].x = lx;
            vertex[0].y = dy;
            vertex[1].x = rx;
            vertex[1].y = dy;
            vertex[2].x = lx;
            vertex[2].y = ty;
            vertex[3].x = rx;
            vertex[3].y = ty;
        
            image.vertex = vertex;
            Vector2[] uv = new Vector2[4];
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            lx = image._rect.x / w;
            rx = lx + image._rect.width / w;
            dy = image._rect.y / h;
            ty = dy + image._rect.height / h;
            uv[0].x = lx;
            uv[0].y = dy;
            uv[1].x = rx;
            uv[1].y = dy;
            uv[2].x = lx;
            uv[2].y = ty;
            uv[3].x = rx;
            uv[3].y = ty;
    
            image.uv = uv;
            image.tris = Rectangle;
        }
        static void CreateSlicedMesh(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float p = image._pixelsPerUnit;
            if (p < 0.01f)
                p = 0.01f;
            float slx = lx +  image._border.x / image._pixelsPerUnit;
            float sdy = dy +  image._border.y / image._pixelsPerUnit;
            float srx = rx -  image._border.z / image._pixelsPerUnit;
            float sty = ty - image._border.w / image._pixelsPerUnit;
            if (srx <= slx )
            {
                float cx = image._border.x / (image._border.x + image._border.z) * x + lx;
                slx = cx;
                srx = cx;
            }
            if (sty < sdy)
            {
                float cy = image._border.y / (image._border.y + image._border.w) * y + dy;
                sdy = cy;
                sty = cy;
            }
            var vertex = new Vector3[16];
            vertex[0].x = lx;
            vertex[0].y = dy;
            vertex[1].x = slx;
            vertex[1].y = dy;
            vertex[2].x = srx;
            vertex[2].y = dy;
            vertex[3].x = rx;
            vertex[3].y = dy;

            vertex[4].x = lx;
            vertex[4].y = sdy;
            vertex[5].x = slx;
            vertex[5].y = sdy;
            vertex[6].x = srx;
            vertex[6].y = sdy;
            vertex[7].x = rx;
            vertex[7].y = sdy;

            vertex[8].x = lx;
            vertex[8].y = sty;
            vertex[9].x = slx;
            vertex[9].y = sty;
            vertex[10].x = srx;
            vertex[10].y = sty;
            vertex[11].x = rx;
            vertex[11].y = sty;

            vertex[12].x = lx;
            vertex[12].y = ty;
            vertex[13].x = slx;
            vertex[13].y = ty;
            vertex[14].x = srx;
            vertex[14].y = ty;
            vertex[15].x = rx;
            vertex[15].y = ty;
            image.vertex = vertex;

            Vector2[] uv = new Vector2[16];
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            lx = image._rect.x / w;
            rx = lx + image._rect.width / w;
            dy = image._rect.y / h;
            ty = dy + image._rect.height / h;

            slx = lx + image._border.x / w;
            sdy = dy + image._border.y / h;
            srx = rx - image._border.z / w;
            sty = ty - image._border.w / h;

            uv[0].x = lx;
            uv[0].y = dy;
            uv[1].x = slx;
            uv[1].y = dy;
            uv[2].x = srx;
            uv[2].y = dy;
            uv[3].x = rx;
            uv[3].y = dy;

            uv[4].x = lx;
            uv[4].y = sdy;
            uv[5].x = slx;
            uv[5].y = sdy;
            uv[6].x = srx;
            uv[6].y = sdy;
            uv[7].x = rx;
            uv[7].y = sdy;

            uv[8].x = lx;
            uv[8].y = sty;
            uv[9].x = slx;
            uv[9].y = sty;
            uv[10].x = srx;
            uv[10].y = sty;
            uv[11].x = rx;
            uv[11].y = sty;

            uv[12].x = lx;
            uv[12].y = ty;
            uv[13].x = slx;
            uv[13].y = ty;
            uv[14].x = srx;
            uv[14].y = ty;
            uv[15].x = rx;
            uv[15].y = ty;
            image.uv = uv;

            if (image._fillCenter)
                image.tris = TwelveRectangle;
            else image.tris = ElevenRectangle;
        }
        /// <summary>
        /// 此模式使用较少,后面有空再补
        /// </summary>
        /// <param name="image"></param>
        static void CreateTiledMesh(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float p = image._pixelsPerUnit;
            if (p < 0.01f)
                p = 0.01f;
            float slx = lx + image._border.x / image._pixelsPerUnit;
            float sdy = dy + image._border.y / image._pixelsPerUnit;
            float srx = rx - image._border.z / image._pixelsPerUnit;
            float sty = ty - image._border.w / image._pixelsPerUnit;

            float w = image._rect.width;
            float cw = x * (1 - image._border.x - image._border.z) / p;
            float h = image._rect.height;
            float ch = y * (1 - image._border.y - image._border.w) / p;
            int col = (int)((srx - slx) / cw);//列
            int row = (int)((sty - sdy) / ch);//行

            float tw = image._textureSize.x;
            float th = image._textureSize.y;
            float ulx = image._rect.x / tw;
            float urx = ulx + w / tw;
            float udy = image._rect.y / th;
            float uty = udy + h / th;

            float uslx = ulx + image._border.x / w;
            float usdy = udy + image._border.y / h;
            float usrx = urx - image._border.z / w;
            float usty = uty -  image._border.w / h;

            if(image._fillCenter)
            {
                int all = (col + 3) * (row + 3);
                Vector3[] vertex = new Vector3[all];
                int t = (col + 2) * (row + 2) * 2;
                int[] tris = new int[t];//
                
                ///填充4个角的顶点
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = slx;
                vertex[1].y = dy;
                vertex[2].x = srx;
                vertex[2].y = dy;
                vertex[3].x = rx;
                vertex[3].y = dy;

                vertex[4].x = lx;
                vertex[4].y = sdy;
                vertex[5].x = slx;
                vertex[5].y = sdy;
                vertex[6].x = srx;
                vertex[6].y = sdy;
                vertex[7].x = rx;
                vertex[7].y = sdy;

                vertex[8].x = lx;
                vertex[8].y = sty;
                vertex[9].x = slx;
                vertex[9].y = sty;
                vertex[10].x = srx;
                vertex[10].y = sty;
                vertex[11].x = rx;
                vertex[11].y = sty;

                vertex[12].x = lx;
                vertex[12].y = ty;
                vertex[13].x = slx;
                vertex[13].y = ty;
                vertex[14].x = srx;
                vertex[14].y = ty;
                vertex[15].x = rx;
                vertex[15].y = ty;

                for (int i = 0; i < 24; i++)//四个角的三角形
                    tris[i] = FourCorners[i];

                int index = 16;
                int ti = 24;
                ///填充左边的顶点
                float ys = udy;
                for (int i = 0; i < row; i++)
                {
                    ys += ch;
                    vertex[index].x = lx;
                    vertex[index].y = ys;
                    index++;
                    vertex[index].x = slx;
                    vertex[index].y = ys;
                    index++;
                }

                ///填充右边的顶点
                ys = udy;
                for (int i = 0; i < row; i++)
                {
                    ys += ch;
                    vertex[index].x = srx;
                    vertex[index].y = ys;
                    index++;
                    vertex[index].x = rx;
                    vertex[index].y = ys;
                    index++;
                }
                ///填充下边的顶点
                float xs = ulx;
                for (int i = 0; i < col; i++)
                {
                    xs += cw;
                    vertex[index].x = xs;
                    vertex[index].y = dy;
                    index++;
                    vertex[index].x = xs;
                    vertex[index].y = sdy;
                    index++;
                }
                ///填充上边的顶点
                xs = ulx;
                for (int i = 0; i < col; i++)
                {
                    xs += cw;
                    vertex[index].x = xs;
                    vertex[index].y = sty;
                    index++;
                    vertex[index].x = xs;
                    vertex[index].y = ty;
                    index++;
                }

            }
            else
            {
                int all = (col + 3) * (row + 3) - col * row;
                Vector3[] vertex = new Vector3[all];
            }
        }
        static void CreateFilledMesh(HImage image)
        {
            switch(image._fillMethod)
            {
                case FillMethod.Horizontal:
                    FillHorizontal(image);
                    break;
                case FillMethod.Vertical:
                    FillVertical(image);
                    break;
                case FillMethod.Radial90:
                    FillRadial90(image);
                    break;
                case FillMethod.Radial180:
                    FillRadial180(image);
                    break;
                case FillMethod.Radial360:
                    FillRadial360(image);
                    break;
            }
        }
        static void FillHorizontal(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
          
            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;

            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float udy = image._rect.y / h;
            float uty = dy + image._rect.height / h;
            float lx, rx, ulx, urx;
            if(image._fillOrigin==1)
            {
                rx = (1 - px) * x;
                lx = rx - image._fillAmount * x;
                ulx = image._rect.x / w;
                urx = lx + image._rect.width / w;
                ulx = urx - image._fillAmount * image._rect.width / w;
            }
            else
            {
                lx = -px * x;
                rx = lx + image._fillAmount * x;
                ulx = image._rect.x / w;
                urx = lx + image._fillAmount * image._rect.width / w;
            }
            var vertex = new Vector3[4];
            vertex[0].x = lx;
            vertex[0].y = dy;
            vertex[1].x = rx;
            vertex[1].y = dy;
            vertex[2].x = lx;
            vertex[2].y = ty;
            vertex[3].x = rx;
            vertex[3].y = ty;
            Vector2[] uv = new Vector2[4];
            uv[0].x = ulx;
            uv[0].y = udy;
            uv[1].x = urx;
            uv[1].y = udy;
            uv[2].x = ulx;
            uv[2].y = uty;
            uv[3].x = urx;
            uv[3].y = uty;

            image.vertex = vertex;
            image.uv = uv;
            image.tris = Rectangle;
        }
        static void FillVertical(HImage image)
        {

        }
        static void FillRadial90(HImage image)
        {

        }
        static void FillRadial180(HImage image)
        {

        }
        static void FillRadial360(HImage image)
        {

        }
    }
}
