using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Diagnostics;

namespace aotv
{
    class collisionDetection
    {
        // TEMP SETUP FOR TESTING SHIP TO SHIP COLLISION ONLY

        public Game1 parent;
        public int lastUpdate;

        public collisionDetection(Game1 Parent)
        {
            parent = Parent;
            lastUpdate = 0;
        }
 
        public bool TexturesCollide(ShipPart obj1, ShipPart obj2)     
        {

            Color[,] tex1 = TextureTo2DArray(parent.getTexture(obj1.type));
            Color[,] tex2 = TextureTo2DArray(parent.getTexture(obj2.type));
            Matrix mat1 = VecToMat(obj1.position, obj1.Angle, obj1.scale, parent.getTexture(obj1.type));
            Matrix mat2 = VecToMat(obj2.position, obj2.Angle, obj2.scale, parent.getTexture(obj2.type));
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0); 
            int height2 = tex2.GetLength(1);
            
            for (int x1=0;x1<width1;x1++)
            {
                for(int y1=0;y1<height1;y1++)
                {
                    Vector2 pos1 = new Vector2(x1,y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;

                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1,y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    Debug.WriteLine(tex1[x1, y1].A + " " + tex1[x2, y2].A + " " + x2+" "+y2);
                                    Debug.WriteLine(obj1.position.X + " " + obj1.position.Y + " " + obj1.Angle + " " + obj1.scale + " " + parent.getTexture(obj1.type).Width);
                                    //return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

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

        private Matrix VecToMat(Vector2 vector, float angle, float scale, Texture2D txt)
        {
            Matrix mat = Matrix.CreateTranslation(new Vector3(txt.Width, txt.Height, 0)) *
                                     Matrix.CreateRotationZ(angle) *
                                     Matrix.CreateScale(new Vector3(scale, scale, 1)) *
                                     Matrix.CreateTranslation(new Vector3(vector.X, vector.Y, 0));
            return mat;
        }

    }
}
