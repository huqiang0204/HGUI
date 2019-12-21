using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGUIMesh
    {
        public static int[] Rectangle = new int[] { 0, 1, 2, 1, 2, 3 };
        public static int[] FourRectangle = new int[] { 0, 3, 1, 3, 4, 1, 1, 4, 2, 4, 5, 2, 3, 6, 4, 6, 7, 4, 4, 7, 5, 7, 8, 5 };
        public static int[] ElevenRectangle = new int[] {
        0, 4, 1, 4, 5, 1, 1, 5, 6, 5, 6, 2, 2, 6, 7, 6, 7, 3,
            4, 8, 5, 8, 9, 5, 6, 10, 11, 10, 11, 7,
            8, 12, 9, 12, 13, 9, 9, 13, 10, 13, 14, 10, 10, 14, 11, 14, 15, 11};
        public static int[] TwelveRectangle = new int[] {
            0, 4, 1, 4, 5, 1, 1, 5, 6, 5, 6, 2, 2, 6, 7, 6, 7, 3,
            4, 8, 5, 8, 9, 5, 5, 9, 10, 9, 10, 6, 6, 10, 11, 10, 11, 7,
            8, 12, 9, 12, 13, 9, 9, 13, 10, 13, 14, 10, 10, 14, 11, 14, 15, 11 };
        public static void CreateMesh(HImage image)
        {
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
                    CreateTiledMesh(image);
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
        static void CreateSimpleMesh(HImage image)
        {
            var vertex = new Vector3[4];
            float x = image.SizeDelta.x;
            float lx = -image._pivot.x * x;
            float rx = (1 - image._pivot.x) * x;
            float y = image.SizeDelta.y;
            float dy = -image. _pivot.y * y;
            float ty = (1 - image._pivot.y) * y;
            vertex[0].x = lx;
            vertex[0].y = dy;
            vertex[1].x = lx;
            vertex[1].y = ty;
            vertex[2].x = rx;
            vertex[2].y = ty;
            vertex[3].x = rx;
            vertex[3].y = dy;
            image.vertex = vertex;
            Vector2[] uv = new Vector2[4];
            float w = image._textureRect.width;
            float h = image._textureRect.height;
            lx = image._rect.x / w;
            rx = lx + image._rect.width / w;
            dy = image._rect.y / h;
            ty = dy + image._rect.height / h;
            uv[0].x = lx;
            uv[0].y = dy;
            uv[1].x = lx;
            uv[1].y = ty;
            uv[2].x = rx;
            uv[2].y = ty;
            uv[3].x = rx;
            uv[3].y = dy;
            image.uv = uv;
            image.tris = Rectangle;
        }
        static void CreateSlicedMesh(HImage image)
        {
            float x = image.SizeDelta.x;
            float lx = -image._pivot.x * x;
            float rx = (1 - image._pivot.x) * x;
            float y = image.SizeDelta.y;
            float dy = -image._pivot.y * y;
            float ty = (1 - image._pivot.y) * y;
            float p = image._pixelsPerUnit;
            if (p < 0.01f)
                p = 0.01f;
            float slx = lx + x * image._border.x / image._pixelsPerUnit;
            float sdy = dy + y * image._border.y / image._pixelsPerUnit;
            float srx = rx - x * image._border.z / image._pixelsPerUnit;
            float sty = ty - y * image._border.w / image._pixelsPerUnit;
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
            float w = image._textureRect.width;
            float h = image._textureRect.height;
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
        static void CreateFilledMesh(HImage image)
        {
            float x = image.SizeDelta.x;
            float lx = -image._pivot.x * x;
            float rx = (1 - image._pivot.x) * x;
            float y = image.SizeDelta.y;
            float dy = -image._pivot.y * y;
            float ty = (1 - image._pivot.y) * y;
            float p = image._pixelsPerUnit;
            if (p < 0.01f)
                p = 0.01f;
            float slx = lx + x * image._border.x / p;
            float sdy = dy + y * image._border.y / p;
            float srx = rx - x * image._border.z / p;
            float sty = ty - y * image._border.w / p;
            float w = image._rect.width;
            float cw = x * (1 - image._border.x - image._border.z) / p;
            float h = image._rect.height;
            float ch = y * (1 - image._border.y - image._border.w) / p;
            int col = (int)((srx - slx) / cw);//列
            int row = (int)((sty - sdy) / ch);//行

            float tw = image._textureRect.width;
            float th = image._textureRect.height;
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
                ///填充左边的顶点
                int index = 16;
                float py = udy;
                for(int i=0;i<row;i++)
                {
                    py += ch;
                    vertex[index].x=lx;
                    vertex[index].y = py;
                    index++;
                    vertex[index].x = slx;
                    vertex[index].y = py;
                    index++;
                }
                ///填充右边的顶点
                py = udy;
                for (int i = 0; i < row; i++)
                {
                    py += ch;
                    vertex[index].x = srx;
                    vertex[index].y = py;
                    index++;
                    vertex[index].x = rx;
                    vertex[index].y = py;
                    index++;
                }
                ///填充下边的顶点
                float px = ulx;
                for (int i=0;i<col;i++)
                {
                    px += cw;
                    vertex[index].x = px;
                    vertex[index].y = dy;
                    index++;
                    vertex[index].x = px;
                    vertex[index].y = sdy;
                    index++;
                }
                ///填充上边的顶点
                px = ulx;
                for (int i = 0; i < col; i++)
                {
                    px += cw;
                    vertex[index].x = px;
                    vertex[index].y = sty;
                    index++;
                    vertex[index].x = px;
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
        static void CreateTiledMesh(HImage image)
        {

        }
    }
}
