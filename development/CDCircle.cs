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
namespace aotv
{
    public class CDCircle      // Ray to Circle Hit Test
    {
        private float radius = 0, angle = 0, length = 0;
        private Vector2 hit = Vector2.Zero;
        private Vector2 location = Vector2.Zero;
        private Vector2 w = Vector2.Zero;
        private Vector2 t = Vector2.Zero;
        private Vector2 m = Vector2.Zero;

        float tempangle = 0;
        float templeng = 0;
        private Vector2 tt = Vector2.Zero;
        private float rAngle = 0;
        Matrix mat = new Matrix();

        private float m1, m2;

        public CDCircle() { }

        public bool Check(ship target, beam weapon)
        {
            Matrix mat = Matrix.CreateTranslation(new Vector3(weapon.position,0)) * Matrix.CreateRotationZ(weapon.Angle);
            
            tt = target.position - weapon.position;
           
            templeng = tt.Length();
            if(length + weapon.beamsize >= templeng)
            {
                tt = Location(templeng, Angle(tt));
                if (Math.Abs(tt.X) < target.boundingSphereRadius)
                {
                    rAngle = weapon.Angle - target.Angle;
                    //Get exact location to start and stop checking
                }
            }
                
            

            w = weapon.position;
            t = target.position;
            angle = weapon.Angle;
            radius = target.boundingSphereRadius;

            m1 = -(float)Math.Tan(Math.PI / 2 - (double)angle);
            m2 = -1 / m1;


            location.X = (t.Y - w.Y + (m1 * w.X) - (m2 * t.X)) / (m1 - m2);
            location.Y = m2 * location.X + t.Y - m2 * t.X;
            length = (t - location).Length();
            if (length <= radius)
            {
                length = (w - location).Length() - new Vector2(radius, length).Length();
                hit.X = length * (float)Math.Cos(Math.PI + angle);
                hit.Y = length * (float)Math.Sin(Math.PI + angle);
                return true;
            }
            else
            {
                return false;
            }

        }
        
        
public float Angle(Vector2 chord)
        {
            //chord.Y *= -1;
            double a;
            if (chord.X > 0)
                a = Math.Atan(chord.Y / chord.X);
            else if (chord.X < 0)
                if (chord.Y < 0)
                    a = Math.Atan(chord.Y / chord.X) - MathHelper.Pi;
                else
                    a = Math.Atan(chord.Y / chord.X) + MathHelper.Pi;
            else if (chord.Y > 0)
                a = MathHelper.PiOver2;
            else if (chord.Y < 0)
                a = -MathHelper.PiOver2;
            else
                a = 0;
            return (float)a;

        }

        // this finds the angle between two vectors
        public double Angle(Vector2 chord1, Vector2 chord2)
        {
            return Angle(chord1 - chord2);
        }

        // this returns a locatoin from an angle and distance
        public Vector2 Location(float distance, double angle)
        {
            Vector2 result = new Vector2();
            result.X = distance * (float)Math.Cos(angle);
            result.Y = distance * (float)Math.Sin(angle);
            return result;
        }

    }
}
