using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Core.HGUI
{
    public class HGUIMesh
    {
        public static int[] Triangle = new int[] { 0,1,2};
        public static int[] Rectangle = new int[] { 0, 2, 3, 0, 3, 1 };
        public static int[] TowRectangle = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2 };
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
            float uty = udy + image._rect.height / h;
            float lx, rx, ulx, urx;
            if(image._fillOrigin==1)
            {
                rx = (1 - px) * x;
                lx = rx - image._fillAmount * x;
                ulx = image._rect.x / w;
                urx = ulx + image._rect.width / w;
                ulx = urx - image._fillAmount * image._rect.width / w;
            }
            else
            {
                lx = -px * x;
                rx = lx + image._fillAmount * x;
                ulx = image._rect.x / w;
                urx = ulx + image._fillAmount * image._rect.width / w;
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
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;

            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            float lx = -px * x;
            float rx = (1 - px) * x;

            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float dy, ty, udy, uty;
            if (image._fillOrigin == 1)
            {
                ty = (1 - py) * y;
                dy = ty - image._fillAmount * y;
                udy = image._rect.y / h;
                uty = udy + image._rect.height / h;
                udy = uty - image._fillAmount * image._rect.height / h;
            }
            else
            {
                dy = -py * y;
                ty = dy + image._fillAmount * y;
                udy = image._rect.y / h;
                uty = udy + image._fillAmount * image._rect.height / h;
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
        static void FillRadial90(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float a = image._fillAmount;
            if(a>0.5f)
            {
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
               
                a -= 0.5f;
                a *= 2;
                switch(image._fillOrigin)
                {
                    case 0:
                        vertex[1].y = ty - (ty - dy) * a;
                        uv[1].y = uty - (uty - udy) * a;
                        break;
                    case 1:
                        vertex[0].x = rx - (rx - lx) * a;
                        uv[0].x = urx - (urx - ulx) * a;
                        break;
                    case 2:
                        vertex[2].y = dy + (ty - dy) * a;
                        uv[2].y = udy + (uty - udy) * a;
                        break;
                    default:
                        vertex[3].x = lx + (rx - lx) * a;
                        uv[3].x = ulx + (urx - ulx) * a;
                        break;
                }
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                var vertex = new Vector3[3];
                Vector2[] uv = new Vector2[3];
                a *= 2;
                switch (image._fillOrigin)
                {
                    case 0:
                        vertex[0].x = lx;
                        vertex[0].y = dy;
                        vertex[1].x = lx;
                        vertex[1].y = ty;
                        vertex[2].y = ty;
                        vertex[2].x = lx + (rx - lx) * a;
                        uv[0].x = ulx;
                        uv[0].y = udy;
                        uv[1].x = ulx;
                        uv[1].y = uty;
                        uv[2].y = uty;
                        uv[2].x = ulx + (urx - ulx) * a;
                        break;
                    case 1:
                        vertex[0].x = lx;
                        vertex[0].y = ty;
                        vertex[1].x = rx;
                        vertex[1].y = ty;
                        vertex[2].x = rx;
                        vertex[2].y = ty - (ty - dy) * a;
                        uv[0].x = ulx;
                        uv[0].y = uty;
                        uv[1].x = urx;
                        uv[1].y = uty;
                        uv[2].x = urx;
                        uv[2].y = uty - (uty - udy) * a;
                        break;
                    case 2:
                        vertex[0].x = rx - (rx - lx) * a;
                        vertex[0].y = dy;
                        vertex[1].x = rx;
                        vertex[1].y = ty;
                        vertex[2].x = rx;
                        vertex[2].y = dy;
                        uv[0].x = urx - (urx - ulx) * a;
                        uv[0].y = udy;
                        uv[1].x = urx;
                        uv[1].y = uty;
                        uv[2].x = urx;
                        uv[2].y = udy;
                        break;
                    default:
                        vertex[0].x = lx;
                        vertex[0].y = dy;
                        vertex[1].x = lx;
                        vertex[1].y = dy + (ty - dy) * a;
                        vertex[2].x = rx;
                        vertex[2].y = dy;
                        uv[0].x = ulx;
                        uv[0].y = udy;
                        uv[1].x = ulx;
                        uv[1].y = udy + (uty - udy) * a;
                        uv[2].x = urx;
                        uv[2].y = udy;
                        break;
                }
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static void FillRadial180(HImage image)
        {
            if(image._fillAmount==1)
            {
                CreateSimpleMesh(image);
                return;
            }
            switch(image._fillOrigin)
            {
                case 0:
                    FillRadial180Bottom(image);
                    break;
                case 1:
                    FillRadial180Left(image);
                    break;
                case 2:
                    FillRadial180Top(image);
                    break;
                case 3:
                    FillRadial180Right(image);
                    break;
            }
        }
        static int[] ThreeTriangleB = new int[] { 0, 2, 3, 0, 3, 1, 1, 3, 4, };
        static void FillRadial180Bottom(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float a = image._fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                Vector3[] vertex = new Vector3[6];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = rx;
                vertex[2].y = ty - y * a;
                vertex[3].x = lx;
                vertex[3].y = ty;
                vertex[4].x = cx;
                vertex[4].y = ty;
                vertex[5].x = rx;
                vertex[5].y = ty;
                Vector2[] uv = new Vector2[6];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = urx;
                uv[2].y = uty - (uty - udy) * a;
                uv[3].x = ulx;
                uv[3].y = uty;
                uv[4].x = ucx;
                uv[4].y = uty;
                uv[5].x = urx;
                uv[5].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TowRectangle;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                Vector3[] vertex = new Vector3[5];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = ty;
                vertex[3].x = cx;
                vertex[3].y = ty;
                vertex[4].x = cx + (rx - cx) * a;
                vertex[4].y = ty;
                Vector2[] uv = new Vector2[5];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = uty;
                uv[3].x = ucx;
                uv[3].y = uty;
                uv[4].x = ucx + (urx - ucx) * a;
                uv[4].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = ThreeTriangleB;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = ty;
                vertex[3].x = lx + (cx - lx) * a;
                vertex[3].y = ty;
                Vector2[] uv = new Vector2[4];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = uty;
                uv[3].x = ulx + (ucx - ulx) * a;
                uv[3].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                Vector3[] vertex = new Vector3[3];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = dy + y * a;
                Vector2[] uv = new Vector2[3];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = udy + (uty - udy) * a;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static int[] TriangleL4 = new int[] { 0, 2, 3, 0, 3, 1, 2, 4, 5, 2, 5, 3 };
        static int[] TriangleL3 = new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2 };
        static void FillRadial180Left(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image._fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                Vector3[] vertex = new Vector3[6];
                vertex[0].x = rx - x * a;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = lx;
                vertex[4].y = ty;
                vertex[5].x = rx;
                vertex[5].y = ty;
                Vector2[] uv = new Vector2[6];
                uv[0].x = urx - (urx - ulx) * a;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = ulx;
                uv[4].y = uty;
                uv[5].x = urx;
                uv[5].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleL4;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                Vector3[] vertex = new Vector3[5];
                vertex[0].x = rx;
                vertex[0].y = cy - (cy - dy) * a;
                vertex[1].x = lx;
                vertex[1].y = cy;
                vertex[2].x = rx;
                vertex[2].y = cy;
                vertex[3].x = lx;
                vertex[3].y = ty;
                vertex[4].x = rx;
                vertex[4].y = ty;
                Vector2[] uv = new Vector2[5];
                uv[0].x = urx;
                uv[0].y = ucy - (ucy - udy) * a;
                uv[1].x = ulx;
                uv[1].y = ucy;
                uv[2].x = urx;
                uv[2].y = ucy;
                uv[3].x = ulx;
                uv[3].y = uty;
                uv[4].x = urx;
                uv[4].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleL3;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = lx;
                vertex[0].y = cy;
                vertex[1].x = rx;
                vertex[1].y = ty - (ty - cy) * a;
                vertex[2].x = lx;
                vertex[2].y = ty;
                vertex[3].x = rx;
                vertex[3].y = ty;
      
                Vector2[] uv = new Vector2[4];
                uv[0].x = ulx;
                uv[0].y = ucy;
                uv[1].x = urx;
                uv[1].y = uty - (uty - ucy) * a;
                uv[2].x = ulx;
                uv[2].y = uty;
                uv[3].x = urx;
                uv[3].y = uty;
               
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                Vector3[] vertex = new Vector3[3];
                vertex[0].x = lx;
                vertex[0].y = cy;
                vertex[1].x = lx;
                vertex[1].y = ty;
                vertex[2].x = lx + (rx - lx) * a;
                vertex[2].y = ty;
                Vector2[] uv = new Vector2[3];
                uv[0].x = ulx;
                uv[0].y = ucy;
                uv[1].x = ulx;
                uv[1].y = uty;
                uv[2].x = ulx + (urx - ulx) * a;
                uv[2].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static int[] TriangleT4 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5,1,5,2};
        static int[] TriangleT3 = new int[] { 0, 3, 1, 1, 3, 4, 1, 4, 2 };
        static void FillRadial180Top(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float a = image._fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                Vector3[] vertex = new Vector3[6];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = rx;
                vertex[2].y = dy;
                vertex[3].x = lx;
                vertex[3].y = dy + y * a;
                vertex[4].x = cx;
                vertex[4].y = ty;
                vertex[5].x = rx;
                vertex[5].y = ty;
                Vector2[] uv = new Vector2[6];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = urx;
                uv[2].y = udy;
                uv[3].x = ulx;
                uv[3].y = udy + (uty - udy) * a;
                uv[4].x = ucx;
                uv[4].y = uty;
                uv[5].x = urx;
                uv[5].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleT4;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                Vector3[] vertex = new Vector3[5];
                vertex[0].x = cx - (cx - lx) * a;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = rx;
                vertex[2].y = dy;
                vertex[3].x = cx;
                vertex[3].y = ty;
                vertex[4].x = rx;
                vertex[4].y = ty;
                Vector2[] uv = new Vector2[5];
                uv[0].x = ucx - (ucx - ulx) * a;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = urx;
                uv[2].y = udy;
                uv[3].x = ucx;
                uv[3].y = uty;
                uv[4].x = urx;
                uv[4].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleT3;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = rx - (rx - cx) * a;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = cx;
                vertex[2].y = ty;
                vertex[3].x = rx;
                vertex[3].y = ty;
                Vector2[] uv = new Vector2[4];
                uv[0].x = urx - (urx - ucx) * a;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ucx;
                uv[2].y = uty;
                uv[3].x = urx;
                uv[3].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                Vector3[] vertex = new Vector3[3];
                vertex[0].x = rx;
                vertex[0].y = ty - (ty - dy) * a;
                vertex[1].x = cx;
                vertex[1].y = ty;
                vertex[2].x = rx;
                vertex[2].y = ty;
                Vector2[] uv = new Vector2[3];
                uv[0].x = urx;
                uv[0].y = uty - (uty - udy) * a;
                uv[1].x = ucx;
                uv[1].y = uty;
                uv[2].x = urx;
                uv[2].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static int[] TriangleR3 = new int[] { 0, 2, 3, 0, 3, 1, 2, 4, 3 };
        static void FillRadial180Right(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image._fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                Vector3[] vertex = new Vector3[6];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = lx;
                vertex[4].y = ty;
                vertex[5].x = lx + x * a;
                vertex[5].y = ty;
                Vector2[] uv = new Vector2[6];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = ulx;
                uv[4].y = uty;
                uv[5].x = ulx +(urx-ulx)* a;
                uv[5].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleL4;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                Vector3[] vertex = new Vector3[5];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = lx;
                vertex[4].y = cy + (ty - cy) * a;
                Vector2[] uv = new Vector2[5];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = ulx;
                uv[4].y = ucy + (uty - ucy) * a;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleR3;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = dy + (cy - dy) * a;
                vertex[3].x = rx;
                vertex[3].y = cy;

                Vector2[] uv = new Vector2[4];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = udy + (ucy - udy) * a;
                uv[3].x = urx;
                uv[3].y = ucy;

                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                Vector3[] vertex = new Vector3[3];
                vertex[0].x = rx - (rx - lx) * a;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = cy;
                vertex[2].x = rx;
                vertex[2].y = dy;
                Vector2[] uv = new Vector2[3];
                uv[0].x = urx - (urx - ulx) * a;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = ucy;
                uv[2].x = urx;
                uv[2].y = udy;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static void FillRadial360(HImage image)
        {
            if (image._fillAmount == 1)
            {
                CreateSimpleMesh(image);
                return;
            }
            switch (image._fillOrigin)
            {
                case 0:
                    FillRadial360Bottom(image);
                    break;
                case 1:
                    FillRadial360Right(image);
                    break;
                case 2:
                    //FillRadial180Top(image);
                    break;
                case 3:
                    //FillRadial180Right(image);
                    break;
            }
        }
        static int[] Triangle360B8 = new int[] { 0, 4, 5, 0, 5, 1, 2, 5, 6, 2, 6, 3, 4, 7, 8, 4, 8, 5, 5, 8, 9, 5, 9, 6 };
        static int[] Triangle360B7 = new int[] { 0, 3, 4, 1, 4, 5, 1, 5, 2, 3, 6, 7, 3, 7, 4, 4, 7, 8, 4, 8, 5 };
        static int[] Triangle360B6 = new int[] { 0, 3, 4, 0, 4, 1, 2, 5, 6, 2, 6, 3, 3, 6, 7, 3, 7, 4 };
        static int[] Triangle360B5 = new int[] { 0, 2, 3, 0, 3, 1, 2, 4, 5, 2, 5, 6, 2, 6, 3 };
        static void FillRadial360Bottom(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image._fillAmount;
            if(a>0.875f)
            {
                a -= 0.875f;
                a *= 8;
                Vector3[] vertex = new Vector3[10];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = lx + (cx - lx) * a;
                vertex[1].y = dy;
                vertex[2].x = cx;
                vertex[2].y = dy;
                vertex[3].x = rx;
                vertex[3].y = dy;
                vertex[4].x = lx;
                vertex[4].y = cy;
                vertex[5].x = cx;
                vertex[5].y = cy;
                vertex[6].x = rx;
                vertex[6].y = cy;
                vertex[7].x = lx;
                vertex[7].y = ty;
                vertex[8].x = cx;
                vertex[8].y = ty;
                vertex[9].x = rx;
                vertex[9].y = ty;
                Vector2[] uv = new Vector2[10];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ulx + (ucx - ulx) * a;
                uv[1].y = udy;
                uv[2].x = ucx;
                uv[2].y = udy;
                uv[3].x = urx;
                uv[3].y = udy;
                uv[4].x = ulx;
                uv[4].y = ucy;
                uv[5].x = ucx;
                uv[5].y = ucy;
                uv[6].x = urx;
                uv[6].y = ucy;
                uv[7].x = ulx;
                uv[7].y = uty;
                uv[8].x = ucx;
                uv[8].y = uty;
                uv[9].x = urx;
                uv[9].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360B8;
            }
            else if(a>0.75f)
            {
                a -= 0.75f;
                a *= 8;
                Vector3[] vertex = new Vector3[9];
                vertex[0].x = lx;
                vertex[0].y = cy - (cy - dy) * a;
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
                Vector2[] uv = new Vector2[9];
                uv[0].x = ulx;
                uv[0].y = ucy - (ucy - udy) * a;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = urx;
                uv[2].y = udy;
                uv[3].x = ulx;
                uv[3].y = ucy;
                uv[4].x = ucx;
                uv[4].y = ucy;
                uv[5].x = urx;
                uv[5].y = ucy;
                uv[6].x = ulx;
                uv[6].y = uty;
                uv[7].x = ucx;
                uv[7].y = uty;
                uv[8].x = urx;
                uv[8].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360B7;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
                Vector3[] vertex = new Vector3[8];
                vertex[0].x = cx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = ty - (ty - cy) * a;
                vertex[3].x = cx;
                vertex[3].y = cy;
                vertex[4].x = rx;
                vertex[4].y = cy;
                vertex[5].x = lx;
                vertex[5].y = ty;
                vertex[6].x = cx;
                vertex[6].y = ty;
                vertex[7].x = rx;
                vertex[7].y = ty;
                Vector2[] uv = new Vector2[8];
                uv[0].x = ucx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = uty - (uty - ucy) * a;
                uv[3].x = ucx;
                uv[3].y = ucy;
                uv[4].x = urx;
                uv[4].y = ucy;
                uv[5].x = ulx;
                uv[5].y = uty;
                uv[6].x = ucx;
                uv[6].y = uty;
                uv[7].x = urx;
                uv[7].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360B6;
            }
            else if(a>0.5f)
            {
                a -= 0.5f;
                a *= 8;
                Vector3[] vertex = new Vector3[7];
                vertex[0].x = cx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = cx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = cx - (cx - lx) * a;
                vertex[4].y = ty;
                vertex[5].x = cx;
                vertex[5].y = ty;
                vertex[6].x = rx;
                vertex[6].y = ty;
                Vector2[] uv = new Vector2[7];
                uv[0].x = ucx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ucx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = ucx - (ucx - ulx) * a;
                uv[4].y = uty;
                uv[5].x = ucx;
                uv[5].y = uty;
                uv[6].x = urx;
                uv[6].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360B5;
            }
            else if(a>0.375f)
            {
                a -= 0.375f;
                a *= 8;
                Vector3[] vertex = new Vector3[6];
                vertex[0].x = cx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = cx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = rx - (rx - cx) * a;
                vertex[4].y = ty;
                vertex[5].x = rx;
                vertex[5].y = ty;
                Vector2[] uv = new Vector2[6];
                uv[0].x = ucx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ucx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = urx - (urx - ucx) * a;
                uv[4].y = uty;
                uv[5].x = urx;
                uv[5].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleL4;
            }
            else if(a>0.25f)
            {
                a -= 0.25f;
                a *= 8;
                Vector3[] vertex = new Vector3[5];
                vertex[0].x = cx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = cx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = rx;
                vertex[4].y = cy + (ty - cy) * a;
                Vector2[] uv = new Vector2[5];
                uv[0].x = ucx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ucx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = urx;
                uv[4].y = ucy + (uty - ucy) * a;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = TriangleL3;
            }
            else if(a>0.125f)
            {
                a -= 0.125f;
                a *= 8;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = cx;
                vertex[0].y = dy;
                vertex[1].x = rx;
                vertex[1].y = dy;
                vertex[2].x = cx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = dy+(cy-dy)*a;
                Vector2[] uv = new Vector2[4];
                uv[0].x = ucx;
                uv[0].y = udy;
                uv[1].x = urx;
                uv[1].y = udy;
                uv[2].x = ucx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = udy + (ucy - udy) * a;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 8;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = cx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = cy;
                vertex[2].x = cx + (rx - cx) * a;
                vertex[2].y = dy;
             
                Vector2[] uv = new Vector2[4];
                uv[0].x = ucx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = ucy;
                uv[2].x = ucx + (urx - ucx) * a;
                uv[2].y = udy;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static int[] Triangle360R8 = new int[] { 0, 4, 5, 0, 5, 1, 1, 5, 3, 1, 3, 2, 4, 7, 8, 4, 8, 5, 5, 8, 9, 5, 9, 6 };
        static int[] Triangle360R7 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 2, 3, 6, 7, 3, 7, 4, 4, 7, 8, 4, 8, 5 };
        static int[] Triangle360R6 = new int[] { 0, 2, 3, 0, 3, 1, 2, 5, 6, 2, 6, 3, 3, 6, 7, 3, 7, 4 };
        static int[] Triangle360R5 = new int[] { 0, 1, 2, 1, 4, 5, 1, 5, 2, 2, 5, 6, 2, 6, 3 };
        static int[] Triangle360R4 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2 };
        static int[] Triangle360R3 = new int[] { 0, 2, 3, 0, 3, 4, 0, 4, 1 };
        static void FillRadial360Right(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image._fillAmount;
            if (a > 0.875f)
            {
                a -= 0.875f;
                a *= 8;
                Vector3[] vertex = new Vector3[10];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = rx;
                vertex[2].y = dy;
                vertex[3].x = rx;
                vertex[3].y = dy + (cy - dy) * a;
                vertex[4].x = lx;
                vertex[4].y = cy;
                vertex[5].x = cx;
                vertex[5].y = cy;
                vertex[6].x = rx;
                vertex[6].y = cy;
                vertex[7].x = lx;
                vertex[7].y = ty;
                vertex[8].x = cx;
                vertex[8].y = ty;
                vertex[9].x = rx;
                vertex[9].y = ty;
                Vector2[] uv = new Vector2[10];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = urx;
                uv[2].y = udy;
                uv[3].x = urx;
                uv[3].y = udy + (ucy - udy) * a;
                uv[4].x = ulx;
                uv[4].y = ucy;
                uv[5].x = ucx;
                uv[5].y = ucy;
                uv[6].x = urx;
                uv[6].y = ucy;
                uv[7].x = ulx;
                uv[7].y = uty;
                uv[8].x = ucx;
                uv[8].y = uty;
                uv[9].x = urx;
                uv[9].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360R8;
            }
            else if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 8;
                Vector3[] vertex = new Vector3[9];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = cx;
                vertex[1].y = dy;
                vertex[2].x = cx + (rx - cx) * a;
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
                Vector2[] uv = new Vector2[9];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ucx;
                uv[1].y = udy;
                uv[2].x = ucx + (urx - ucx) * a;
                uv[2].y = udy;
                uv[3].x = ulx;
                uv[3].y = ucy;
                uv[4].x = ucx;
                uv[4].y = ucy;
                uv[5].x = urx;
                uv[5].y = ucy;
                uv[6].x = ulx;
                uv[6].y = uty;
                uv[7].x = ucx;
                uv[7].y = uty;
                uv[8].x = urx;
                uv[8].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360R7;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
                Vector3[] vertex = new Vector3[8];
                vertex[0].x = lx;
                vertex[0].y = dy;
                vertex[1].x = lx + (cx - lx) * a;
                vertex[1].y = dy;
                vertex[2].x = lx;
                vertex[2].y = cy;
                vertex[3].x = cx;
                vertex[3].y = cy;
                vertex[4].x = rx;
                vertex[4].y = cy;
                vertex[5].x = lx;
                vertex[5].y = ty;
                vertex[6].x = cx;
                vertex[6].y = ty;
                vertex[7].x = rx;
                vertex[7].y = ty;
                Vector2[] uv = new Vector2[8];
                uv[0].x = ulx;
                uv[0].y = udy;
                uv[1].x = ulx + (ucx - ulx) * a;
                uv[1].y = udy;
                uv[2].x = ulx;
                uv[2].y = ucy;
                uv[3].x = ucx;
                uv[3].y = ucy;
                uv[4].x = urx;
                uv[4].y = ucy;
                uv[5].x = ulx;
                uv[5].y = uty;
                uv[6].x = ucx;
                uv[6].y = uty;
                uv[7].x = urx;
                uv[7].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360R6;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 8;
                Vector3[] vertex = new Vector3[7];
                vertex[0].x = lx;
                vertex[0].y = cy - (cy - dy) * a;
                vertex[1].x = lx;
                vertex[1].y = cy;
                vertex[2].x = cx;
                vertex[2].y = cy;
                vertex[3].x = rx;
                vertex[3].y = cy;
                vertex[4].x = lx;
                vertex[4].y = ty;
                vertex[5].x = cx;
                vertex[5].y = ty;
                vertex[6].x = rx;
                vertex[6].y = ty;
                Vector2[] uv = new Vector2[7];
                uv[0].x = ulx;
                uv[0].y = ucy - (ucy - udy) * a;
                uv[1].x = ulx;
                uv[1].y = ucy;
                uv[2].x = ucx;
                uv[2].y = ucy;
                uv[3].x = urx;
                uv[3].y = ucy;
                uv[4].x = ulx;
                uv[4].y = uty;
                uv[5].x = ucx;
                uv[5].y = uty;
                uv[6].x = urx;
                uv[6].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360R5;
            }
            else if (a > 0.375f)
            {
                a -= 0.375f;
                a *= 8;
                Vector3[] vertex = new Vector3[6];
                vertex[0].x = lx;
                vertex[0].y = ty - (ty - cy) * a;
                vertex[1].x = cx;
                vertex[1].y = cy;
                vertex[2].x = rx;
                vertex[2].y = cy;
                vertex[3].x = lx;
                vertex[3].y = ty;
                vertex[4].x = cx;
                vertex[4].y = ty;
                vertex[5].x = rx;
                vertex[5].y = ty;
                Vector2[] uv = new Vector2[6];
                uv[0].x = ulx;
                uv[0].y = uty - (uty - ucy) * a;
                uv[1].x = ucx;
                uv[1].y = ucy;
                uv[2].x = urx;
                uv[2].y = ucy;
                uv[3].x = ulx;
                uv[3].y = uty;
                uv[4].x = ucx;
                uv[4].y = uty;
                uv[5].x = urx;
                uv[5].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360R4;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 8;
                Vector3[] vertex = new Vector3[5];
                vertex[0].x = cx;
                vertex[0].y = cy;
                vertex[1].x = rx;
                vertex[1].y = cy;
                vertex[2].x = cx - (cx - lx) * a;
                vertex[2].y = ty;
                vertex[3].x = cx;
                vertex[3].y = ty;
                vertex[4].x = rx;
                vertex[4].y = ty;
                Vector2[] uv = new Vector2[5];
                uv[0].x = ucx;
                uv[0].y = ucy;
                uv[1].x = urx;
                uv[1].y = ucy;
                uv[2].x = ucx - (ucx - ulx) * a;
                uv[2].y = uty;
                uv[3].x = ucx;
                uv[3].y = uty;
                uv[4].x = urx;
                uv[4].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle360R3;
            }
            else if (a > 0.125f)
            {
                a -= 0.125f;
                a *= 8;
                Vector3[] vertex = new Vector3[4];
                vertex[0].x = cx;
                vertex[0].y = cy;
                vertex[1].x = rx;
                vertex[1].y = cy;
                vertex[2].x = rx - (rx - cx) * a;
                vertex[2].y = ty;
                vertex[3].x = rx;
                vertex[3].y = ty;
                Vector2[] uv = new Vector2[4];
                uv[0].x = ucx;
                uv[0].y = ucy;
                uv[1].x = urx;
                uv[1].y = ucy;
                uv[2].x = urx - (urx - ucx) * a;
                uv[2].y = uty;
                uv[3].x = urx;
                uv[3].y = uty;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 8;
                Vector3[] vertex = new Vector3[3];
                vertex[0].x = cx;
                vertex[0].y = cy;
                vertex[1].x = rx;
                vertex[1].y = cy + (ty - cy) * a;
                vertex[2].x = rx;
                vertex[2].y = cy;
                Vector2[] uv = new Vector2[3];
                uv[0].x = ucx;
                uv[0].y = ucy;
                uv[1].x = urx;
                uv[1].y = ucy + (uty - ucy) * a;
                uv[2].x = urx;
                uv[2].y = ucy;
                image.vertex = vertex;
                image.uv = uv;
                image.tris = Triangle;
            }
        }
        static void FillRadial360Top(HImage image)
        {
            float px = image._pivot.x / image._rect.width;
            float py = image._pivot.y / image._rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image._textureSize.x;
            float h = image._textureSize.y;
            float ulx = image._rect.x / w;
            float urx = ulx + image._rect.width / w;
            float udy = image._rect.y / h;
            float uty = udy + image._rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image._fillAmount;
            if(a>0.875f)
            {
                a -= 0.875f;
                a *= 8;
            }else if(a>0.75f)
            {
                a -= 0.75f;
                a *= 8;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
            }
            else if(a>0.5f)
            {
                a -= 0.5f;
                a *= 8;
            }
            else if(a>0.375f)
            {
                a -= 0.375f;
                a *= 8;
            }
            else if(a>0.25f)
            {
                a -= 0.25f;
                a *= 8;
            }
            else if(a>0.125f)
            {
                a -= 0.125f;
                a *= 8;
            }
            else
            {
                a *= 8;
            }
        }
    }
}
