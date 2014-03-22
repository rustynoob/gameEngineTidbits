using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

using System.IO;

namespace aotv
{
    public class cd2
    {

        public Game parent;

        public int lastUpdate;
        public cd2(Game Parent)
        {
            parent = Parent;

            lastUpdate = 0;
        }

      // this function is slow and farily usless
        public Vector2 checkCollision(ShipPart s1, ShipPart s2)
        {
            Matrix mat1 = getTrix(new Vector3((s1.imageOrigin.X * -1), (s1.imageOrigin.Y * -1), 0), s1.parent.Angle, .2f, s1.position + s1.parent.position);
            Matrix mat2 = getTrix(new Vector3((s2.imageOrigin.X * -1), (s2.imageOrigin.Y * -1), 0), s2.parent.Angle, .2f, s2.position + s2.parent.position);
            Color[,] tex1 = s1.ColorArray;
            Color[,] tex2 = s2.ColorArray;
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1 += 5)
            {
                for (int y1 = 0; y1 < height1; y1 += 5)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }
                }
            }

            return new Vector2(-1, -1);
        }


        public Matrix getTrix(Vector3 tr, float angle, float scale, Vector2 position)
        {
            Matrix trix = Matrix.CreateTranslation(tr) * Matrix.CreateRotationZ(angle) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position.X, position.Y, 0);
            return trix;
        }

        public Vector2 checkCollision( ShipPart part, Vector2 line, double dir)// Vector2 offset)
        {
           // I think this is all right but there have been so many changes to everything that I'm going to go over it all again later once we have the 
           Color[,] texture = part.ColorArray;
           Matrix matrix = Matrix.CreateTranslation(new Vector3(part.imageOrigin, 0)) * Matrix.CreateRotationZ(part.Angle) * Matrix.CreateScale(part.scale);// *Matrix.CreateTranslation(offset.X, offset.Y, 0);

           int width = part.ColorArray.GetLength(0);
           int height = part.ColorArray.GetLength(1);
           int i = 0;
           Vector2 pos;
            do{
                pos = line;
                pos.Normalize();
                pos = Vector2.Transform(pos*i++ , matrix);
            }while((pos.X < 0 || pos.X > width)&&(pos.Y < 0 || pos.Y > height ));
            while(pos.X > 0 && pos.X < width&&pos.Y > 0 && pos.Y < height)
            {
                pos.Normalize();
                pos = Vector2.Transform(pos*i++ , matrix);
                if (texture[(int)pos.X,(int)pos.Y].A > 0)
                    return Vector2.Transform(pos, matrix);
            }
            return line;

        }

        //public Vector2 checkCollision(ColisionComponent target, Vector2 line, Vector2 offset)
        //{
        //    // I think this is all right but there have been so many changes to everything that I'm going to go over it all again later once we have the 
        //    Color[,] texture = target.ColorArray;
        //    Matrix matrix = Matrix.CreateTranslation(new Vector3(target.imageOrigin, 0)) * Matrix.CreateRotationZ(target.Angle) * Matrix.CreateScale(target.scale) * Matrix.CreateTranslation(offset.X, offset.Y, 0);

        //    int width = target.ColorArray.GetLength(0);
        //    int height = target.ColorArray.GetLength(1);
        //    int i = 0;
        //    Vector2 pos;
        //    do
        //    {
        //        pos = line;
        //        pos.Normalize();
        //        pos = Vector2.Transform(pos * i++, matrix);
        //    } while ((pos.X < 0 || pos.X > width) && (pos.Y < 0 || pos.Y > height));
        //    while (pos.X > 0 && pos.X < width && pos.Y > 0 && pos.Y < height)
        //    {
        //        pos.Normalize();
        //        pos = Vector2.Transform(pos * i++, matrix);
        //        if (texture[(int)pos.X, (int)pos.Y].A > 0)
        //            return Vector2.Transform(pos, matrix);
        //    }
        //    return line;

        //}

      
        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}