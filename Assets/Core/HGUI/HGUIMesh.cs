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
        }
        static void CreateSlicedMesh(HImage image)
        {
            float x = image.SizeDelta.x;
            float lx = -image._pivot.x * x;
            float rx = (1 - image._pivot.x) * x;
            float y = image.SizeDelta.y;
            float dy = -image._pivot.y * y;
            float ty = (1 - image._pivot.y) * y;
            if (image._pixelsPerUnit == 0)
            {
                var vertex = new Vector3[9];
                float cx = lx + 0.5f * x;
                float cy = dy + 0.5f * y;
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = rx;
                vertex[2].y = dy;
                vertex[3].x = lx;
                vertex[3].y = cy;
                vertex[4].x = cx;
                vertex[4].y = cy;
                vertex[5].x = rx;
                vertex[5].y = cy;
                vertex[6].x = lx;
                vertex[6].y = ty;
                vertex[7].x = cx;
                vertex[7].y = ty;
                vertex[8].x = rx;
                vertex[8].y = ty;
                image.vertex = vertex;
            }
            else
            {
                float slx = lx + x *image. _border.x /image. _pixelsPerUnit;
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
            }
        }
        static void CreateFilledMesh(HImage image)
        {
            float x = image.SizeDelta.x;
            float lx = -image._pivot.x * x;
            float rx = (1 - image._pivot.x) * x;
            float y = image.SizeDelta.y;
            float dy = -image._pivot.y * y;
            float ty = (1 - image._pivot.y) * y;
            if (image._pixelsPerUnit == 0)
            {
                var vertex = new Vector3[9];
                float cx = lx + 0.5f * x;
                float cy = dy + 0.5f * y;
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = rx;
                vertex[2].y = dy;
                vertex[3].x = lx;
                vertex[3].y = cy;
                vertex[4].x = cx;
                vertex[4].y = cy;
                vertex[5].x = rx;
                vertex[5].y = cy;
                vertex[6].x = lx;
                vertex[6].y = ty;
                vertex[7].x = cx;
                vertex[7].y = ty;
                vertex[8].x = rx;
                vertex[8].y = ty;
                image.vertex = vertex;
            }
            else
            {
                float p = image._pixelsPerUnit;
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

            }
        }
        static void CreateTiledMesh(HImage image)
        {

        }
    }
}
