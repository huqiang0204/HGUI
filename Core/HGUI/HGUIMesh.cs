using huqiang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huqiang.Core.HGUI
{
    public class HGUIMesh
    {
        public static int[] Triangle = new int[] { 0,1,2};
        public static int[] Rectangle = new int[] { 0, 2, 3, 0, 3, 1 };
        public static int[] TowRectangle = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2 };
        public static int[] FourRectangle = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 3, 6, 7, 3, 7, 4, 4, 7, 8, 4, 8, 5 };
        public static int[] FourCorners = new int[] { 0, 2, 3, 0, 3, 1, 4, 6, 7, 4, 7, 5, 8, 10, 11, 8, 11, 9, 12, 14, 15, 12, 15, 13 };
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
            if (image.s_id ==0)
            {
                CreateSimpleVertex(image);
                goto label;
            }
            switch (image.SprType)
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
        label:;
            var hv = image.vertices;
            var col = image.m_color;
            Vector4 tang = image.uvrect;
            if(hv!=null)
            {
                for (int i = 0; i < hv.Length; i++)
                {
                    hv[i].color = col;
                    hv[i].uv3.x = tang.x;
                    hv[i].uv3.y = tang.y;
                    hv[i].uv4.x = tang.z;
                    hv[i].uv4.y = tang.w;
                }
            }
        }
        static void CreateSimpleVertex(HImage image)
        {
            HVertex[] hv = image.vertices;
            if (hv == null)
                hv = new HVertex[4];
            else if (hv.Length != 4)
                hv = new HVertex[4];
            float x = image.SizeDelta.x;
            float lx = -0.5f * x;
            float rx = 0.5f * x;
            float y = image.SizeDelta.y;
            float dy = -0.5f * y;
            float ty = 0.5f * y;
            hv[0].position.x = lx;
            hv[0].position.y = dy;
            hv[0].uv.x = 0;
            hv[0].uv.y = 0;
            hv[1].position.x = rx;
            hv[1].position.y = dy;
            hv[1].uv.x = 1;
            hv[1].uv.y = 0;
            hv[2].position.x = lx;
            hv[2].position.y = ty;
            hv[2].uv.x = 0;
            hv[2].uv.y =1;
            hv[3].position.x = rx;
            hv[3].position.y = ty;
            hv[3].uv.x = 1;
            hv[3].uv.y = 1;
            image.vertices = hv;
            image.tris = Rectangle;
        }
        static void CreateSimpleMesh(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            HVertex[] hv = image.vertices;
            if (hv == null)
                hv = new HVertex[4];
            else if (hv.Length != 4)
                hv = new HVertex[4];
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            hv[0].position.x = lx;
            hv[0].position.y = dy;
            hv[1].position.x = rx;
            hv[1].position.y = dy;
            hv[2].position.x = lx;
            hv[2].position.y = ty;
            hv[3].position.x = rx;
            hv[3].position.y = ty;
        
            image.vertices = hv;

            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            lx = image.m_rect.x / w;
            rx = lx + image.m_rect.width / w;
            dy = image.m_rect.y / h;
            ty = dy + image.m_rect.height / h;
            hv[0].uv.x = lx;
            hv[0].uv.y = dy;
            hv[1].uv.x = rx;
            hv[1].uv.y = dy;
            hv[2].uv.x = lx;
            hv[2].uv.y = ty;
            hv[3].uv.x = rx;
            hv[3].uv.y = ty;
    
            image.tris = Rectangle;
        }
        static void CreateSlicedMesh(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float p = image.m_pixelsPerUnit;
            if (p < 0.01f)
                p = 0.01f;
            float slx = lx +  image.m_border.x / image.m_pixelsPerUnit;
            float sdy = dy +  image.m_border.y / image.m_pixelsPerUnit;
            float srx = rx -  image.m_border.z / image.m_pixelsPerUnit;
            float sty = ty - image.m_border.w / image.m_pixelsPerUnit;
            if (srx <= slx )
            {
                float cx = image.m_border.x / (image.m_border.x + image.m_border.z) * x + lx;
                slx = cx;
                srx = cx;
            }
            if (sty < sdy)
            {
                float cy = image.m_border.y / (image.m_border.y + image.m_border.w) * y + dy;
                sdy = cy;
                sty = cy;
            }
            var hv = new HVertex[16];
            hv[0].position.x = lx;
            hv[0].position.y = dy;
            hv[1].position.x = slx;
            hv[1].position.y = dy;
            hv[2].position.x = srx;
            hv[2].position.y = dy;
            hv[3].position.x = rx;
            hv[3].position.y = dy;

            hv[4].position.x = lx;
            hv[4].position.y = sdy;
            hv[5].position.x = slx;
            hv[5].position.y = sdy;
            hv[6].position.x = srx;
            hv[6].position.y = sdy;
            hv[7].position.x = rx;
            hv[7].position.y = sdy;

            hv[8].position.x = lx;
            hv[8].position.y = sty;
            hv[9].position.x = slx;
            hv[9].position.y = sty;
            hv[10].position.x = srx;
            hv[10].position.y = sty;
            hv[11].position.x = rx;
            hv[11].position.y = sty;

            hv[12].position.x = lx;
            hv[12].position.y = ty;
            hv[13].position.x = slx;
            hv[13].position.y = ty;
            hv[14].position.x = srx;
            hv[14].position.y = ty;
            hv[15].position.x = rx;
            hv[15].position.y = ty;
            image.vertices= hv;

            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            lx = image.m_rect.x / w;
            rx = lx + image.m_rect.width / w;
            dy = image.m_rect.y / h;
            ty = dy + image.m_rect.height / h;

            slx = lx + image.m_border.x / w;
            sdy = dy + image.m_border.y / h;
            srx = rx - image.m_border.z / w;
            sty = ty - image.m_border.w / h;

            hv[0].uv.x = lx;
            hv[0].uv.y = dy;
            hv[1].uv.x = slx;
            hv[1].uv.y = dy;
            hv[2].uv.x = srx;
            hv[2].uv.y = dy;
            hv[3].uv.x = rx;
            hv[3].uv.y = dy;

            hv[4].uv.x = lx;
            hv[4].uv.y = sdy;
            hv[5].uv.x = slx;
            hv[5].uv.y = sdy;
            hv[6].uv.x = srx;
            hv[6].uv.y = sdy;
            hv[7].uv.x = rx;
            hv[7].uv.y = sdy;

            hv[8].uv.x = lx;
            hv[8].uv.y = sty;
            hv[9].uv.x = slx;
            hv[9].uv.y = sty;
            hv[10].uv.x = srx;
            hv[10].uv.y = sty;
            hv[11].uv.x = rx;
            hv[11].uv.y = sty;

            hv[12].uv.x = lx;
            hv[12].uv.y = ty;
            hv[13].uv.x = slx;
            hv[13].uv.y = ty;
            hv[14].uv.x = srx;
            hv[14].uv.y = ty;
            hv[15].uv.x = rx;
            hv[15].uv.y = ty;

            if (image.m_fillCenter)
                image.tris = TwelveRectangle;
            else image.tris = ElevenRectangle;
        }
        /// <summary>
        /// 此模式使用较少,后面有空再补
        /// </summary>
        /// <param name="image"></param>
        static void CreateTiledMesh(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float p = image.m_pixelsPerUnit;
            if (p < 0.01f)
                p = 0.01f;
            float slx = lx + image.m_border.x / image.m_pixelsPerUnit;
            float sdy = dy + image.m_border.y / image.m_pixelsPerUnit;
            float srx = rx - image.m_border.z / image.m_pixelsPerUnit;
            float sty = ty - image.m_border.w / image.m_pixelsPerUnit;

            float w = image.m_rect.width;
            float cw = x * (1 - image.m_border.x - image.m_border.z) / p;
            float h = image.m_rect.height;
            float ch = y * (1 - image.m_border.y - image.m_border.w) / p;
            int col = (int)((srx - slx) / cw);//列
            int row = (int)((sty - sdy) / ch);//行

            float tw = image.m_textureSize.x;
            float th = image.m_textureSize.y;
            float ulx = image.m_rect.x / tw;
            float urx = ulx + w / tw;
            float udy = image.m_rect.y / th;
            float uty = udy + h / th;

            float uslx = ulx + image.m_border.x / w;
            float usdy = udy + image.m_border.y / h;
            float usrx = urx - image.m_border.z / w;
            float usty = uty -  image.m_border.w / h;

            if(image.m_fillCenter)
            {
                int all = (col + 3) * (row + 3);
                HVertex[] hv = new HVertex[all];
                int t = (col + 2) * (row + 2) * 2;
                int[] tris = new int[t];//
                
                ///填充4个角的顶点
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = slx;
                hv[1].position.y = dy;
                hv[2].position.x = srx;
                hv[2].position.y = dy;
                hv[3].position.x = rx;
                hv[3].position.y = dy;

                hv[4].position.x = lx;
                hv[4].position.y = sdy;
                hv[5].position.x = slx;
                hv[5].position.y = sdy;
                hv[6].position.x = srx;
                hv[6].position.y = sdy;
                hv[7].position.x = rx;
                hv[7].position.y = sdy;

                hv[8].position.x = lx;
                hv[8].position.y = sty;
                hv[9].position.x = slx;
                hv[9].position.y = sty;
                hv[10].position.x = srx;
                hv[10].position.y = sty;
                hv[11].position.x = rx;
                hv[11].position.y = sty;

                hv[12].position.x = lx;
                hv[12].position.y = ty;
                hv[13].position.x = slx;
                hv[13].position.y = ty;
                hv[14].position.x = srx;
                hv[14].position.y = ty;
                hv[15].position.x = rx;
                hv[15].position.y = ty;

                for (int i = 0; i < 24; i++)//四个角的三角形
                    tris[i] = FourCorners[i];

                int index = 16;
                int ti = 24;
                ///填充左边的顶点
                float ys = udy;
                for (int i = 0; i < row; i++)
                {
                    ys += ch;
                    hv[index].position.x = lx;
                    hv[index].position.y = ys;
                    index++;
                    hv[index].position.x = slx;
                    hv[index].position.y = ys;
                    index++;
                }

                ///填充右边的顶点
                ys = udy;
                for (int i = 0; i < row; i++)
                {
                    ys += ch;
                    hv[index].position.x = srx;
                    hv[index].position.y = ys;
                    index++;
                    hv[index].position.x = rx;
                    hv[index].position.y = ys;
                    index++;
                }
                ///填充下边的顶点
                float xs = ulx;
                for (int i = 0; i < col; i++)
                {
                    xs += cw;
                    hv[index].position.x = xs;
                    hv[index].position.y = dy;
                    index++;
                    hv[index].position.x = xs;
                    hv[index].position.y = sdy;
                    index++;
                }
                ///填充上边的顶点
                xs = ulx;
                for (int i = 0; i < col; i++)
                {
                    xs += cw;
                    hv[index].position.x = xs;
                    hv[index].position.y = sty;
                    index++;
                    hv[index].position.x = xs;
                    hv[index].position.y = ty;
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
            switch(image.m_fillMethod)
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
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
          
            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;

            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float lx, rx, ulx, urx;
            if(image.m_fillOrigin==1)
            {
                rx = (1 - px) * x;
                lx = rx - image.m_fillAmount * x;
                ulx = image.m_rect.x / w;
                urx = ulx + image.m_rect.width / w;
                ulx = urx - image.m_fillAmount * image.m_rect.width / w;
            }
            else
            {
                lx = -px * x;
                rx = lx + image.m_fillAmount * x;
                ulx = image.m_rect.x / w;
                urx = ulx + image.m_fillAmount * image.m_rect.width / w;
            }
            var hv = new HVertex[4];
            hv[0].position.x = lx;
            hv[0].position.y = dy;
            hv[1].position.x = rx;
            hv[1].position.y = dy;
            hv[2].position.x = lx;
            hv[2].position.y = ty;
            hv[3].position.x = rx;
            hv[3].position.y = ty;
            hv[0].uv.x = ulx;
            hv[0].uv.y = udy;
            hv[1].uv.x = urx;
            hv[1].uv.y = udy;
            hv[2].uv.x = ulx;
            hv[2].uv.y = uty;
            hv[3].uv.x = urx;
            hv[3].uv.y = uty;

            image.vertices = hv;
            image.tris = Rectangle;
        }
        static void FillVertical(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;

            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            float lx = -px * x;
            float rx = (1 - px) * x;

            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float dy, ty, udy, uty;
            if (image.m_fillOrigin == 1)
            {
                ty = (1 - py) * y;
                dy = ty - image.m_fillAmount * y;
                udy = image.m_rect.y / h;
                uty = udy + image.m_rect.height / h;
                udy = uty - image.m_fillAmount * image.m_rect.height / h;
            }
            else
            {
                dy = -py * y;
                ty = dy + image.m_fillAmount * y;
                udy = image.m_rect.y / h;
                uty = udy + image.m_fillAmount * image.m_rect.height / h;
            }
            var hv = new HVertex[4];
            hv[0].position.x = lx;
            hv[0].position.y = dy;
            hv[1].position.x = rx;
            hv[1].position.y = dy;
            hv[2].position.x = lx;
            hv[2].position.y = ty;
            hv[3].position.x = rx;
            hv[3].position.y = ty;

            hv[0].uv.x = ulx;
            hv[0].uv.y = udy;
            hv[1].uv.x = urx;
            hv[1].uv.y = udy;
            hv[2].uv.x = ulx;
            hv[2].uv.y = uty;
            hv[3].uv.x = urx;
            hv[3].uv.y = uty;

            image.vertices = hv;
            image.tris = Rectangle;
        }
        static void FillRadial90(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float a = image.m_fillAmount;
            if(a>0.5f)
            {
                var hv = new HVertex[4];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = ty;
                hv[3].position.x = rx;
                hv[3].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = uty;
                hv[3].uv.x = urx;
                hv[3].uv.y = uty;
               
                a -= 0.5f;
                a *= 2;
                switch(image.m_fillOrigin)
                {
                    case 0:
                        hv[1].position.y = ty - (ty - dy) * a;
                        hv[1].uv.y = uty - (uty - udy) * a;
                        break;
                    case 1:
                        hv[0].position.x = rx - (rx - lx) * a;
                        hv[0].uv.x = urx - (urx - ulx) * a;
                        break;
                    case 2:
                        hv[2].position.y = dy + (ty - dy) * a;
                        hv[2].uv.y = udy + (uty - udy) * a;
                        break;
                    default:
                        hv[3].position.x = lx + (rx - lx) * a;
                        hv[3].uv.x = ulx + (urx - ulx) * a;
                        break;
                }
                image.vertices= hv;
                image.tris = Rectangle;
            }
            else
            {
                var hv = new HVertex[3];
                a *= 2;
                switch (image.m_fillOrigin)
                {
                    case 0:
                        hv[0].position.x = lx;
                        hv[0].position.y = dy;
                        hv[1].position.x = lx;
                        hv[1].position.y = ty;
                        hv[2].position.y = ty;
                        hv[2].position.x = lx + (rx - lx) * a;
                        hv[0].uv.x = ulx;
                        hv[0].uv.y = udy;
                        hv[1].uv.x = ulx;
                        hv[1].uv.y = uty;
                        hv[2].uv.y = uty;
                        hv[2].uv.x = ulx + (urx - ulx) * a;
                        break;
                    case 1:
                        hv[0].position.x = lx;
                        hv[0].position.y = ty;
                        hv[1].position.x = rx;
                        hv[1].position.y = ty;
                        hv[2].position.x = rx;
                        hv[2].position.y = ty - (ty - dy) * a;
                        hv[0].uv.x = ulx;
                        hv[0].uv.y = uty;
                        hv[1].uv.x = urx;
                        hv[1].uv.y = uty;
                        hv[2].uv.x = urx;
                        hv[2].uv.y = uty - (uty - udy) * a;
                        break;
                    case 2:
                        hv[0].position.x = rx - (rx - lx) * a;
                        hv[0].position.y = dy;
                        hv[1].position.x = rx;
                        hv[1].position.y = ty;
                        hv[2].position.x = rx;
                        hv[2].position.y = dy;
                        hv[0].uv.x = urx - (urx - ulx) * a;
                        hv[0].uv.y = udy;
                        hv[1].uv.x = urx;
                        hv[1].uv.y = uty;
                        hv[2].uv.x = urx;
                        hv[2].uv.y = udy;
                        break;
                    default:
                        hv[0].position.x = lx;
                        hv[0].position.y = dy;
                        hv[1].position.x = lx;
                        hv[1].position.y = dy + (ty - dy) * a;
                        hv[2].position.x = rx;
                        hv[2].position.y = dy;
                        hv[0].uv.x = ulx;
                        hv[0].uv.y = udy;
                        hv[1].uv.x = ulx;
                        hv[1].uv.y = udy + (uty - udy) * a;
                        hv[2].uv.x = urx;
                        hv[2].uv.y = udy;
                        break;
                }
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
        static void FillRadial180(HImage image)
        {
            if(image.m_fillAmount==1)
            {
                CreateSimpleMesh(image);
                return;
            }
            switch(image.m_fillOrigin)
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
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float a = image.m_fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = ty - y * a;
                hv[3].position.x = lx;
                hv[3].position.y = ty;
                hv[4].position.x = cx;
                hv[4].position.y = ty;
                hv[5].position.x = rx;
                hv[5].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = uty - (uty - udy) * a;
                hv[3].uv.x = ulx;
                hv[3].uv.y = uty;
                hv[4].uv.x = ucx;
                hv[4].uv.y = uty;
                hv[5].uv.x = urx;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = TowRectangle;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = ty;
                hv[3].position.x = cx;
                hv[3].position.y = ty;
                hv[4].position.x = cx + (rx - cx) * a;
                hv[4].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = uty;
                hv[3].uv.x = ucx;
                hv[3].uv.y = uty;
                hv[4].uv.x = ucx + (urx - ucx) * a;
                hv[4].uv.y = uty;
                image.vertices = hv;
                image.tris = ThreeTriangleB;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = ty;
                hv[3].position.x = lx + (cx - lx) * a;
                hv[3].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = uty;
                hv[3].uv.x = ulx + (ucx - ulx) * a;
                hv[3].uv.y = uty;
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                HVertex[] hv = new HVertex[3];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = dy + y * a;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = udy + (uty - udy) * a;
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
        static int[] TriangleL4 = new int[] { 0, 2, 3, 0, 3, 1, 2, 4, 5, 2, 5, 3 };
        static int[] TriangleL3 = new int[] { 0, 1, 2, 1, 3, 4, 1, 4, 2 };
        static void FillRadial180Left(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image.m_fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = rx - x * a;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[4].position.x = lx;
                hv[4].position.y = ty;
                hv[5].position.x = rx;
                hv[5].position.y = ty;
                hv[0].uv.x = urx - (urx - ulx) * a;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ulx;
                hv[4].uv.y = uty;
                hv[5].uv.x = urx;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleL4;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = rx;
                hv[0].position.y = cy - (cy - dy) * a;
                hv[1].position.x = lx;
                hv[1].position.y = cy;
                hv[2].position.x = rx;
                hv[2].position.y = cy;
                hv[3].position.x = lx;
                hv[3].position.y = ty;
                hv[4].position.x = rx;
                hv[4].position.y = ty;
                hv[0].uv.x = urx;
                hv[0].uv.y = ucy - (ucy - udy) * a;
                hv[1].uv.x = ulx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = urx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = uty;
                hv[4].uv.x = urx;
                hv[4].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleL3;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = lx;
                hv[0].position.y = cy;
                hv[1].position.x = rx;
                hv[1].position.y = ty - (ty - cy) * a;
                hv[2].position.x = lx;
                hv[2].position.y = ty;
                hv[3].position.x = rx;
                hv[3].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = ucy;
                hv[1].uv.x = urx;
                hv[1].uv.y = uty - (uty - ucy) * a;
                hv[2].uv.x = ulx;
                hv[2].uv.y = uty;
                hv[3].uv.x = urx;
                hv[3].uv.y = uty;
               
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                HVertex[] hv = new HVertex[3];
                hv[0].position.x = lx;
                hv[0].position.y = cy;
                hv[1].position.x = lx;
                hv[1].position.y = ty;
                hv[2].position.x = lx + (rx - lx) * a;
                hv[2].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = ucy;
                hv[1].uv.x = ulx;
                hv[1].uv.y = uty;
                hv[2].uv.x = ulx + (urx - ulx) * a;
                hv[2].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
        static int[] TriangleT4 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5,1,5,2};
        static int[] TriangleT3 = new int[] { 0, 3, 1, 1, 3, 4, 1, 4, 2 };
        static void FillRadial180Top(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float a = image.m_fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = dy + y * a;
                hv[4].position.x = cx;
                hv[4].position.y = ty;
                hv[5].position.x = rx;
                hv[5].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = udy + (uty - udy) * a;
                hv[4].uv.x = ucx;
                hv[4].uv.y = uty;
                hv[5].uv.x = urx;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleT4;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = cx - (cx - lx) * a;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = cx;
                hv[3].position.y = ty;
                hv[4].position.x = rx;
                hv[4].position.y = ty;
                hv[0].uv.x = ucx - (ucx - ulx) * a;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ucx;
                hv[3].uv.y = uty;
                hv[4].uv.x = urx;
                hv[4].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleT3;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = rx - (rx - cx) * a;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = cx;
                hv[2].position.y = ty;
                hv[3].position.x = rx;
                hv[3].position.y = ty;
                hv[0].uv.x = urx - (urx - ucx) * a;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = uty;
                hv[3].uv.x = urx;
                hv[3].uv.y = uty;
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                HVertex[] hv = new HVertex[3];
                hv[0].position.x = rx;
                hv[0].position.y = ty - (ty - dy) * a;
                hv[1].position.x = cx;
                hv[1].position.y = ty;
                hv[2].position.x = rx;
                hv[2].position.y = ty;
                hv[0].uv.x = urx;
                hv[0].uv.y = uty - (uty - udy) * a;
                hv[1].uv.x = ucx;
                hv[1].uv.y = uty;
                hv[2].uv.x = urx;
                hv[2].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
        static int[] TriangleR3 = new int[] { 0, 2, 3, 0, 3, 1, 2, 4, 3 };
        static void FillRadial180Right(HImage image)
        {
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float x = image.SizeDelta.x;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float y = image.SizeDelta.y;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            float a = image.m_fillAmount;
            if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 4;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[4].position.x = lx;
                hv[4].position.y = ty;
                hv[5].position.x = lx + x * a;
                hv[5].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ulx;
                hv[4].uv.y = uty;
                hv[5].uv.x = ulx +(urx-ulx)* a;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleL4;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 4;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[4].position.x = lx;
                hv[4].position.y = cy + (ty - cy) * a;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ulx;
                hv[4].uv.y = ucy + (uty - ucy) * a;
                image.vertices = hv;
                image.tris = TriangleR3;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 4;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = dy + (cy - dy) * a;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = udy + (ucy - udy) * a;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;

                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 4;
                HVertex[] hv = new HVertex[3];
                hv[0].position.x = rx - (rx - lx) * a;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = cy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[0].uv.x = urx - (urx - ulx) * a;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
        static void FillRadial360(HImage image)
        {
            if (image.m_fillAmount == 1)
            {
                CreateSimpleMesh(image);
                return;
            }
            switch (image.m_fillOrigin)
            {
                case 0:
                    FillRadial360Bottom(image);
                    break;
                case 1:
                    FillRadial360Right(image);
                    break;
                case 2:
                    FillRadial360Top(image);
                    break;
                case 3:
                    FillRadial360Left(image);
                    break;
            }
        }
        static int[] Triangle360B8 = new int[] { 0, 4, 5, 0, 5, 1, 2, 5, 6, 2, 6, 3, 4, 7, 8, 4, 8, 5, 5, 8, 9, 5, 9, 6 };
        static int[] Triangle360B7 = new int[] { 0, 3, 4, 1, 4, 5, 1, 5, 2, 3, 6, 7, 3, 7, 4, 4, 7, 8, 4, 8, 5 };
        static int[] Triangle360B6 = new int[] { 0, 3, 4, 0, 4, 1, 2, 5, 6, 2, 6, 3, 3, 6, 7, 3, 7, 4 };
        static int[] Triangle360B5 = new int[] { 0, 2, 3, 0, 3, 1, 2, 4, 5, 2, 5, 6, 2, 6, 3 };
        static void FillRadial360Bottom(HImage image)
        {
            float a = image.m_fillAmount;
            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            if (image.m_preserveAspect & a > 0)
            {
                float ocx = x * 0.5f;
                float ocy = y * 0.5f;
                a -= 0.5f;
                if (a < 0)
                    a += 1;
                Vector2 d = MathH.Tan2(360 - a * 360);//方向
                Vector2[] lines = new Vector2[9];
                lines[0].x = ocx;
                lines[2].y = ocy;
                lines[3].y = y;
                lines[4].x = ocx;
                lines[4].y = y;
                lines[5].x = x;
                lines[5].y = y;
                lines[6].x = x;
                lines[6].y = ocy;
                lines[7].x = x;
                lines[8].x = ocx;
                Vector2 oc = new Vector2(ocx, ocy);
                Vector2 ot = oc + d * 10000;
                Vector2 cross = Vector2.zero;
                for (int i = 0; i < 8; i++)
                {
                    if (huqiang.Physics2D.LineToLine(ref lines[i], ref lines[i + 1], ref oc, ref ot, ref cross))
                    {
                        float r = (cross - lines[i + 1]).magnitude / (lines[i + 1] - lines[i]).magnitude;
                        a = (7 - i + r) * 0.125f;
                        break;
                    }
                }
            }
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            if(a>0.875f)
            {
                a -= 0.875f;
                a *= 8;
                HVertex[] hv = new HVertex[10];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = lx + (cx - lx) * a;
                hv[1].position.y = dy;
                hv[2].position.x = cx;
                hv[2].position.y = dy;
                hv[3].position.x = rx;
                hv[3].position.y = dy;
                hv[4].position.x = lx;
                hv[4].position.y = cy;
                hv[5].position.x = cx;
                hv[5].position.y = cy;
                hv[6].position.x = rx;
                hv[6].position.y = cy;
                hv[7].position.x = lx;
                hv[7].position.y = ty;
                hv[8].position.x = cx;
                hv[8].position.y = ty;
                hv[9].position.x = rx;
                hv[9].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ulx + (ucx - ulx) * a;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = udy;
                hv[3].uv.x = urx;
                hv[3].uv.y = udy;
                hv[4].uv.x = ulx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = ucx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = urx;
                hv[6].uv.y = ucy;
                hv[7].uv.x = ulx;
                hv[7].uv.y = uty;
                hv[8].uv.x = ucx;
                hv[8].uv.y = uty;
                hv[9].uv.x = urx;
                hv[9].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360B8;
            }
            else if(a>0.75f)
            {
                a -= 0.75f;
                a *= 8;
                HVertex[] hv = new HVertex[9];
                hv[0].position.x = lx;
                hv[0].position.y = cy - (cy - dy) * a;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = lx;
                hv[6].position.y = ty;
                hv[7].position.x = cx;
                hv[7].position.y = ty;
                hv[8].position.x = rx;
                hv[8].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = ucy - (ucy - udy) * a;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = ulx;
                hv[6].uv.y = uty;
                hv[7].uv.x = ucx;
                hv[7].uv.y = uty;
                hv[8].uv.x = urx;
                hv[8].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360B7;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
                HVertex[] hv = new HVertex[8];
                hv[0].position.x = cx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = ty - (ty - cy) * a;
                hv[3].position.x = cx;
                hv[3].position.y = cy;
                hv[4].position.x = rx;
                hv[4].position.y = cy;
                hv[5].position.x = lx;
                hv[5].position.y = ty;
                hv[6].position.x = cx;
                hv[6].position.y = ty;
                hv[7].position.x = rx;
                hv[7].position.y = ty;
                hv[0].uv.x = ucx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = uty - (uty - ucy) * a;
                hv[3].uv.x = ucx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = urx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = ulx;
                hv[5].uv.y = uty;
                hv[6].uv.x = ucx;
                hv[6].uv.y = uty;
                hv[7].uv.x = urx;
                hv[7].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360B6;
            }
            else if(a>0.5f)
            {
                a -= 0.5f;
                a *= 8;
                HVertex[] hv = new HVertex[7];
                hv[0].position.x = cx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = cx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[4].position.x = cx - (cx - lx) * a;
                hv[4].position.y = ty;
                hv[5].position.x = cx;
                hv[5].position.y = ty;
                hv[6].position.x = rx;
                hv[6].position.y = ty;
                hv[0].uv.x = ucx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx - (ucx - ulx) * a;
                hv[4].uv.y = uty;
                hv[5].uv.x = ucx;
                hv[5].uv.y = uty;
                hv[6].uv.x = urx;
                hv[6].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360B5;
            }
            else if(a>0.375f)
            {
                a -= 0.375f;
                a *= 8;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = cx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = cx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[4].position.x = rx - (rx - cx) * a;
                hv[4].position.y = ty;
                hv[5].position.x = rx;
                hv[5].position.y = ty;
                hv[0].uv.x = ucx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = urx - (urx - ucx) * a;
                hv[4].uv.y = uty;
                hv[5].uv.x = urx;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleL4;
            }
            else if(a>0.25f)
            {
                a -= 0.25f;
                a *= 8;
               HVertex[] hv = new HVertex[5];
                hv[0].position.x = cx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = cx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = cy;
                hv[4].position.x = rx;
                hv[4].position.y = cy + (ty - cy) * a;
                hv[0].uv.x = ucx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = urx;
                hv[4].uv.y = ucy + (uty - ucy) * a;
                image.vertices = hv;
                image.tris = TriangleL3;
            }
            else if(a>0.125f)
            {
                a -= 0.125f;
                a *= 8;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = cx;
                hv[0].position.y = dy;
                hv[1].position.x = rx;
                hv[1].position.y = dy;
                hv[2].position.x = cx;
                hv[2].position.y = cy;
                hv[3].position.x = rx;
                hv[3].position.y = dy+(cy-dy)*a;
                hv[0].uv.x = ucx;
                hv[0].uv.y = udy;
                hv[1].uv.x = urx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = urx;
                hv[3].uv.y = udy + (ucy - udy) * a;
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 8;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = cx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = cy;
                hv[2].position.x = cx + (rx - cx) * a;
                hv[2].position.y = dy;
                hv[0].uv.x = ucx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = ucx + (urx - ucx) * a;
                hv[2].uv.y = udy;
                image.vertices = hv;
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
            float a = image.m_fillAmount;
            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            if (image.m_preserveAspect & a > 0)
            {
                float ocx = x * 0.5f;
                float ocy = y * 0.5f;
                a -= 0.25f;
                if (a < 0)
                    a += 1;
                Vector2 d = MathH.Tan2(360 - a * 360);//方向
                Vector2[] lines = new Vector2[9];
                lines[0].x = x;
                lines[0].y = ocy;
                lines[1].x = x;
                lines[2].x = ocx;
                lines[4].y = ocy;
                lines[5].y = y;
                lines[6].x = ocx;
                lines[6].y = y;
                lines[7].x = x;
                lines[7].y = y;
                lines[8].x = x;
                lines[8].y = ocy;
                Vector2 oc = new Vector2(ocx, ocy);
                Vector2 ot = oc + d * 10000;
                Vector2 cross = Vector2.zero;
                for (int i = 0; i < 8; i++)
                {
                    if (huqiang.Physics2D.LineToLine(ref lines[i], ref lines[i + 1], ref oc, ref ot, ref cross))
                    {
                        float r = (cross - lines[i + 1]).magnitude / (lines[i + 1] - lines[i]).magnitude;
                        a = (7 - i + r) * 0.125f;
                        break;
                    }
                }
            }
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            if (a > 0.875f)
            {
                a -= 0.875f;
                a *= 8;
                HVertex[] hv = new HVertex[10];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = rx;
                hv[3].position.y = dy + (cy - dy) * a;
                hv[4].position.x = lx;
                hv[4].position.y = cy;
                hv[5].position.x = cx;
                hv[5].position.y = cy;
                hv[6].position.x = rx;
                hv[6].position.y = cy;
                hv[7].position.x = lx;
                hv[7].position.y = ty;
                hv[8].position.x = cx;
                hv[8].position.y = ty;
                hv[9].position.x = rx;
                hv[9].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = urx;
                hv[3].uv.y = udy + (ucy - udy) * a;
                hv[4].uv.x = ulx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = ucx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = urx;
                hv[6].uv.y = ucy;
                hv[7].uv.x = ulx;
                hv[7].uv.y = uty;
                hv[8].uv.x = ucx;
                hv[8].uv.y = uty;
                hv[9].uv.x = urx;
                hv[9].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360R8;
            }
            else if (a > 0.75f)
            {
                a -= 0.75f;
                a *= 8;
                HVertex[] hv = new HVertex[9];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = cx + (rx - cx) * a;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = lx;
                hv[6].position.y = ty;
                hv[7].position.x = cx;
                hv[7].position.y = ty;
                hv[8].position.x = rx;
                hv[8].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx + (urx - ucx) * a;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = ulx;
                hv[6].uv.y = uty;
                hv[7].uv.x = ucx;
                hv[7].uv.y = uty;
                hv[8].uv.x = urx;
                hv[8].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360R7;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
                HVertex[] hv = new HVertex[8];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = lx + (cx - lx) * a;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = cy;
                hv[3].position.x = cx;
                hv[3].position.y = cy;
                hv[4].position.x = rx;
                hv[4].position.y = cy;
                hv[5].position.x = lx;
                hv[5].position.y = ty;
                hv[6].position.x = cx;
                hv[6].position.y = ty;
                hv[7].position.x = rx;
                hv[7].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ulx + (ucx - ulx) * a;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = ucx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = urx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = ulx;
                hv[5].uv.y = uty;
                hv[6].uv.x = ucx;
                hv[6].uv.y = uty;
                hv[7].uv.x = urx;
                hv[7].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360R6;
            }
            else if (a > 0.5f)
            {
                a -= 0.5f;
                a *= 8;
                HVertex[] uv = new HVertex[7];
                uv[0].position.x = lx;
                uv[0].position.y = cy - (cy - dy) * a;
                uv[1].position.x = lx;
                uv[1].position.y = cy;
                uv[2].position.x = cx;
                uv[2].position.y = cy;
                uv[3].position.x = rx;
                uv[3].position.y = cy;
                uv[4].position.x = lx;
                uv[4].position.y = ty;
                uv[5].position.x = cx;
                uv[5].position.y = ty;
                uv[6].position.x = rx;
                uv[6].position.y = ty;
                uv[0].uv.x = ulx;
                uv[0].uv.y = ucy - (ucy - udy) * a;
                uv[1].uv.x = ulx;
                uv[1].uv.y = ucy;
                uv[2].uv.x = ucx;
                uv[2].uv.y = ucy;
                uv[3].uv.x = urx;
                uv[3].uv.y = ucy;
                uv[4].uv.x = ulx;
                uv[4].uv.y = uty;
                uv[5].uv.x = ucx;
                uv[5].uv.y = uty;
                uv[6].uv.x = urx;
                uv[6].uv.y = uty;
                image.vertices = uv;
                image.tris = Triangle360R5;
            }
            else if (a > 0.375f)
            {
                a -= 0.375f;
                a *= 8;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = lx;
                hv[0].position.y = ty - (ty - cy) * a;
                hv[1].position.x = cx;
                hv[1].position.y = cy;
                hv[2].position.x = rx;
                hv[2].position.y = cy;
                hv[3].position.x = lx;
                hv[3].position.y = ty;
                hv[4].position.x = cx;
                hv[4].position.y = ty;
                hv[5].position.x = rx;
                hv[5].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = uty - (uty - ucy) * a;
                hv[1].uv.x = ucx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = urx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = uty;
                hv[4].uv.x = ucx;
                hv[4].uv.y = uty;
                hv[5].uv.x = urx;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360R4;
            }
            else if (a > 0.25f)
            {
                a -= 0.25f;
                a *= 8;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = cx;
                hv[0].position.y = cy;
                hv[1].position.x = rx;
                hv[1].position.y = cy;
                hv[2].position.x = cx - (cx - lx) * a;
                hv[2].position.y = ty;
                hv[3].position.x = cx;
                hv[3].position.y = ty;
                hv[4].position.x = rx;
                hv[4].position.y = ty;
                hv[0].uv.x = ucx;
                hv[0].uv.y = ucy;
                hv[1].uv.x = urx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = ucx - (ucx - ulx) * a;
                hv[2].uv.y = uty;
                hv[3].uv.x = ucx;
                hv[3].uv.y = uty;
                hv[4].uv.x = urx;
                hv[4].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360R3;
            }
            else if (a > 0.125f)
            {
                a -= 0.125f;
                a *= 8;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = cx;
                hv[0].position.y = cy;
                hv[1].position.x = rx;
                hv[1].position.y = cy;
                hv[2].position.x = rx - (rx - cx) * a;
                hv[2].position.y = ty;
                hv[3].position.x = rx;
                hv[3].position.y = ty;
                hv[0].uv.x = ucx;
                hv[0].uv.y = ucy;
                hv[1].uv.x = urx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = urx - (urx - ucx) * a;
                hv[2].uv.y = uty;
                hv[3].uv.x = urx;
                hv[3].uv.y = uty;
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 8;
                HVertex[] hv = new HVertex[3];
                hv[0].position.x = cx;
                hv[0].position.y = cy;
                hv[1].position.x = rx;
                hv[1].position.y = cy + (ty - cy) * a;
                hv[2].position.x = rx;
                hv[2].position.y = cy;
                hv[0].uv.x = ucx;
                hv[0].uv.y = ucy;
                hv[1].uv.x = urx;
                hv[1].uv.y = ucy + (uty - ucy) * a;
                hv[2].uv.x = urx;
                hv[2].uv.y = ucy;
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
        static int[] Triangle360T8 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 3, 6, 7, 3, 7, 4, 4, 8, 9, 4, 9, 5 };
        static int[] Triangle360T7 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 3, 6, 7, 3, 7, 4, 4, 8, 5 };
        static int[] Triangle360T6 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 3, 6, 7, 3, 7, 4 };
        static int[] Triangle360T5 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 2, 3, 5, 6, 3, 6, 4 };
        static void FillRadial360Top(HImage image)
        {
            float a = image.m_fillAmount;
            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            if(image.m_preserveAspect & a>0)
            {
                float ocx = x * 0.5f;
                float ocy = y * 0.5f;
                Vector2 d = MathH.Tan2(360 - a * 360);//方向
                Vector2[] lines = new Vector2[9];
                lines[0].x = ocx;
                lines[0].y = y;
                lines[1].x = x;
                lines[1].y = y;
                lines[2].x = x;
                lines[2].y = ocy;
                lines[3].x = x;
                lines[4].x = ocx;
                lines[6].y = ocy;
                lines[7].y = y;
                lines[8].x = ocx;
                lines[8].y = y;
                Vector2 oc = new Vector2(ocx, ocy);
                Vector2 ot = oc + d * 10000;
                Vector2 cross = Vector2.zero;
                for (int i = 0; i < 8; i++)
                {
                    if (huqiang.Physics2D.LineToLine(ref lines[i], ref lines[i + 1], ref oc, ref ot, ref cross))
                    {
                        float r = (cross - lines[i + 1]).magnitude / (lines[i + 1] - lines[i]).magnitude;
                        a = (7 - i+r) * 0.125f ;
                        break;
                    }
                }
            }
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            if(a>0.875f)
            {
                a -= 0.875f;
                a *= 8;
                HVertex[] hv = new HVertex[10];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = lx;
                hv[6].position.y = ty;
                hv[7].position.x = cx;
                hv[7].position.y = ty;
                hv[8].position.x = rx - (rx - cx) * a;
                hv[8].position.y = ty;
                hv[9].position.x = rx;
                hv[9].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = ulx;
                hv[6].uv.y = uty;
                hv[7].uv.x = ucx;
                hv[7].uv.y = uty;
                hv[8].uv.x = urx - (urx - ucx) * a;
                hv[8].uv.y = uty;
                hv[9].uv.x = urx;
                hv[9].uv.y = uty;
                image.vertices= hv;
                image.tris = Triangle360T8;
            }
            else if(a>0.75f)
            {
                a -= 0.75f;
                a *= 8;
                HVertex[] hv = new HVertex[9];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = lx;
                hv[6].position.y = ty;
                hv[7].position.x = cx;
                hv[7].position.y = ty;
                hv[8].position.x = rx;
                hv[8].position.y = cy + (ty - cy) * a;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = ulx;
                hv[6].uv.y = uty;
                hv[7].uv.x = ucx;
                hv[7].uv.y = uty;
                hv[8].uv.x = urx;
                hv[8].uv.y = ucy + (uty - ucy) * a;
                image.vertices = hv;
                image.tris = Triangle360T7;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
                HVertex[] hv = new HVertex[8];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = dy + (cy - dy) * a;
                hv[6].position.x = lx;
                hv[6].position.y = ty;
                hv[7].position.x = cx;
                hv[7].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = udy + (ucy - udy) * a;
                hv[6].uv.x = ulx;
                hv[6].uv.y = uty;
                hv[7].uv.x = ucx;
                hv[7].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360T6;
            }
            else if(a>0.5f)
            {
                a -= 0.5f;
                a *= 8;
                HVertex[] hv = new HVertex[7];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = cx + (rx - cx) * a;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = lx;
                hv[5].position.y = ty;
                hv[6].position.x = cx;
                hv[6].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx + (urx - ucx) * a;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = ulx;
                hv[5].uv.y = uty;
                hv[6].uv.x = ucx;
                hv[6].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360T5;
            }
            else if(a>0.375f)
            {
                a -= 0.375f;
                a *= 8;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = lx + (cx - lx) * a;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = cy;
                hv[3].position.x = cx;
                hv[3].position.y = cy;
                hv[4].position.x = lx;
                hv[4].position.y = ty;
                hv[5].position.x = cx;
                hv[5].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ulx + (ucx - ulx) * a;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = ucx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ulx;
                hv[4].uv.y = uty;
                hv[5].uv.x = ucx;
                hv[5].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleL4;
            }
            else if(a>0.25f)
            {
                a -= 0.25f;
                a *= 8;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = lx;
                hv[0].position.y = cy - (cy - dy) * a;
                hv[1].position.x = lx;
                hv[1].position.y = cy;
                hv[2].position.x = cx;
                hv[2].position.y = cy;
                hv[3].position.x = lx;
                hv[3].position.y = ty;
                hv[4].position.x = cx;
                hv[4].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = ucy - (ucy - udy) * a;
                hv[1].uv.x = ulx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = uty;
                hv[4].uv.x = ucx;
                hv[4].uv.y = uty;
                image.vertices = hv;
                image.tris = TriangleL3;
            }
            else if(a>0.125f)
            {
                a -= 0.125f;
                a *= 8;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = lx;
                hv[0].position.y = ty - (ty - cy) * a;
                hv[1].position.x = cx;
                hv[1].position.y = cy;
                hv[2].position.x = lx;
                hv[2].position.y = ty;
                hv[3].position.x = cx;
                hv[3].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = uty - (uty - ucy) * a;
                hv[1].uv.x = ucx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = uty;
                hv[3].uv.x = ucx;
                hv[3].uv.y = uty;
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 8;
                HVertex[] uv = new HVertex[3];
                uv[0].position.x = cx;
                uv[0].position.y = cy;
                uv[1].position.x = cx - (cx - lx) * a;
                uv[1].position.y = ty;
                uv[2].position.x = cx;
                uv[2].position.y = ty;
                uv[0].uv.x = ucx;
                uv[0].uv.y = ucy;
                uv[1].uv.x = ucx - (ucx - ulx) * a;
                uv[1].uv.y = uty;
                uv[2].uv.x = ucx;
                uv[2].uv.y = uty;
                image.vertices = uv;
                image.tris = Triangle;
            }
        }
        static int[] Triangle360L8 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 6, 7, 4, 4, 7, 8, 4, 8, 9, 4, 9, 5 };
        static int[] Triangle360L7 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 4, 6, 7, 4, 7, 8, 4, 8, 5 };
        static int[] Triangle360L6 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 4, 6, 7, 4, 7, 5 };
        static int[] Triangle360L5 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 5, 1, 5, 2, 4, 6, 5 };
        static int[] Triangle360L3 = new int[] { 0, 3, 4, 0, 4, 1, 1, 4, 2 };
        static void FillRadial360Left(HImage image)
        {
            float a = image.m_fillAmount;
            float x = image.SizeDelta.x;
            float y = image.SizeDelta.y;
            if (image.m_preserveAspect & a > 0)
            {
                float ocx = x * 0.5f;
                float ocy = y * 0.5f;
                a -= 0.75f;
                if (a < 0)
                    a += 1;
                Vector2 d = MathH.Tan2(360 - a * 360);//方向
                Vector2[] lines = new Vector2[9];
                lines[0].y = ocy;
                lines[1].y = y;
                lines[2].x = ocx;
                lines[2].y = y;
                lines[3].x = x;
                lines[3].y = y;
                lines[4].x = x;
                lines[4].y = ocy;
                lines[5].x = x;
                lines[6].x = ocx;
                lines[8].y = ocy;
                Vector2 oc = new Vector2(ocx, ocy);
                Vector2 ot = oc + d * 10000;
                Vector2 cross = Vector2.zero;
                for (int i = 0; i < 8; i++)
                {
                    if (huqiang.Physics2D.LineToLine(ref lines[i], ref lines[i + 1], ref oc, ref ot, ref cross))
                    {
                        float r = (cross - lines[i + 1]).magnitude / (lines[i + 1] - lines[i]).magnitude;
                        a = (7 - i + r) * 0.125f;
                        break;
                    }
                }
            }
            float px = image.m_pivot.x / image.m_rect.width;
            float py = image.m_pivot.y / image.m_rect.height;
            float lx = -px * x;
            float rx = (1 - px) * x;
            float dy = -py * y;
            float ty = (1 - py) * y;
            float w = image.m_textureSize.x;
            float h = image.m_textureSize.y;
            float ulx = image.m_rect.x / w;
            float urx = ulx + image.m_rect.width / w;
            float udy = image.m_rect.y / h;
            float uty = udy + image.m_rect.height / h;
            float cx = lx + x * 0.5f;
            float ucx = ulx + (urx - ulx) * 0.5f;
            float cy = dy + y * 0.5f;
            float ucy = udy + (uty - udy) * 0.5f;
            if(a>0.875f)
            {
                a -= 0.875f;
                a *= 8;
                HVertex[] hv = new HVertex[10];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = lx;
                hv[6].position.y = ty - (ty - cy) * a;
                hv[7].position.x = lx;
                hv[7].position.y = ty;
                hv[8].position.x = cx;
                hv[8].position.y = ty;
                hv[9].position.x = rx;
                hv[9].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = ulx;
                hv[6].uv.y = uty - (uty - ucy) * a;
                hv[7].uv.x = ulx;
                hv[7].uv.y = uty;
                hv[8].uv.x = ucx;
                hv[8].uv.y = uty;
                hv[9].uv.x = urx;
                hv[9].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360L8;
            }
            else if(a>0.75f)
            {
                a -= 0.75f;
                a *= 8;
                HVertex[] hv = new HVertex[9];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = cx - (cx - lx) * a;
                hv[6].position.y = ty;
                hv[7].position.x = cx;
                hv[7].position.y = ty;
                hv[8].position.x = rx;
                hv[8].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = ucx - (ucx - ulx) * a;
                hv[6].uv.y = uty;
                hv[7].uv.x = ucx;
                hv[7].uv.y = uty;
                hv[8].uv.x = urx;
                hv[8].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360L7;
            }
            else if(a>0.625f)
            {
                a -= 0.625f;
                a *= 8;
                HVertex[] hv = new HVertex[8];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = rx - (rx - cx) * a;
                hv[6].position.y = ty;
                hv[7].position.x = rx;
                hv[7].position.y = ty;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = urx - (urx - ucx) * a;
                hv[6].uv.y = uty;
                hv[7].uv.x = urx;
                hv[7].uv.y = uty;
                image.vertices = hv;
                image.tris = Triangle360L6;
            }
            else if(a>0.5f)
            {
                a -= 0.5f;
                a *= 8;
                HVertex[] hv = new HVertex[7];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = cy;
                hv[6].position.x = rx;
                hv[6].position.y = cy + (ty - cy) * a;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = ucy;
                hv[6].uv.x = urx;
                hv[6].uv.y = ucy + (uty - ucy) * a;
                image.vertices = hv;
                image.tris = Triangle360L5;
            }
            else if(a>0.375f)
            {
                a -= 0.375f;
                a *= 8;
                HVertex[] hv = new HVertex[6];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = rx;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[5].position.x = rx;
                hv[5].position.y = dy + (cy - dy) * a;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = urx;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                hv[5].uv.x = urx;
                hv[5].uv.y = udy + (ucy - udy) * a;
                image.vertices = hv;
                image.tris = TriangleT4;
            }
            else if(a>0.25f)
            {
                a -= 0.25f;
                a *= 8;
                HVertex[] hv = new HVertex[5];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = cx;
                hv[1].position.y = dy;
                hv[2].position.x = cx + (rx - cx) * a;
                hv[2].position.y = dy;
                hv[3].position.x = lx;
                hv[3].position.y = cy;
                hv[4].position.x = cx;
                hv[4].position.y = cy;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ucx;
                hv[1].uv.y = udy;
                hv[2].uv.x = ucx + (urx - ucx) * a;
                hv[2].uv.y = udy;
                hv[3].uv.x = ulx;
                hv[3].uv.y = ucy;
                hv[4].uv.x = ucx;
                hv[4].uv.y = ucy;
                image.vertices = hv;
                image.tris = Triangle360L3;
            }
            else if(a>0.125f)
            {
                a -= 0.125f;
                a *= 8;
                HVertex[] hv = new HVertex[4];
                hv[0].position.x = lx;
                hv[0].position.y = dy;
                hv[1].position.x = lx + (cx - lx) * a;
                hv[1].position.y = dy;
                hv[2].position.x = lx;
                hv[2].position.y = cy;
                hv[3].position.x = cx;
                hv[3].position.y = cy;
                hv[0].uv.x = ulx;
                hv[0].uv.y = udy;
                hv[1].uv.x = ulx + (ucx - ulx) * a;
                hv[1].uv.y = udy;
                hv[2].uv.x = ulx;
                hv[2].uv.y = ucy;
                hv[3].uv.x = ucx;
                hv[3].uv.y = ucy;
                image.vertices = hv;
                image.tris = Rectangle;
            }
            else
            {
                a *= 8;
                HVertex[] hv = new HVertex[3];
                hv[0].position.x = lx;
                hv[0].position.y = cy - (cy - dy) * a;
                hv[1].position.x = lx;
                hv[1].position.y = cy;
                hv[2].position.x = cx;
                hv[2].position.y = cy;
                hv[0].uv.x = ulx;
                hv[0].uv.y = ucy - (ucy - udy) * a;
                hv[1].uv.x = ulx;
                hv[1].uv.y = ucy;
                hv[2].uv.x = ucx;
                hv[2].uv.y = ucy;
                image.vertices = hv;
                image.tris = Triangle;
            }
        }
    }
}
