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


namespace Xbox360Game3
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 

    public struct particle
    {
        public Vector2 location;
        public Vector2 drift;
        public bool active;
    }

    public class influnceField
    {
        public Vector2 value;
        public int weight;
        public influnceField()
        {
            value = Vector2.Zero;
            weight = 0;

        }
    }
 

    public class  particleSystem : Microsoft.Xna.Framework.DrawableGameComponent
    {
        
        private particle[] particles;
        private Vector2 location;
        private int emissionWidth = 50;
        private int emissionHeight = 12;
        private Vector2 emission;
        private int emissionRate;
        private Vector2 wind;
        private Vector2 gravity;
        private Vector2 circle3;
        Vector2 fluid = Vector2.Zero;
        int scale = 1;
        private int step;
        private int size;
        Random rand;
        Texture2D texture;
        private int var;
        private influnceField[,] influence;
        private influnceField[,] iwrite;
        private int msize = 100;
        private SpriteBatch sprites;

        public particleSystem(Game game,Vector2 Location, Vector2 wind, int count, Vector2 dir, int speed )
            : base(game)
        {
            
            size = count;
            particles = new particle[count];
            this.wind = wind;
            step = 0;
            location = Location;
            emission = dir;
            emissionRate = speed;
            rand = new Random();
            var = 0;
            gravity = new Vector2(345, 345);
            fluid = new Vector2(567, 123);
            circle3 = new Vector2(120, 170);
            influence = new influnceField[msize, msize];
            iwrite = new influnceField[msize, msize];
   
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            sprites = new SpriteBatch(this.Game.GraphicsDevice);
            texture = Game.Content.Load<Texture2D>("particle2");
            // TODO: Add your initialization code here
            for (int i = 0; i < size; i++){
                particles[i] = new particle();
                particles[i].drift = emission;
                particles[i].location = this.location;
                particles[i].active = false;
            }
            for (int i = 0; i < msize; i++)
                for (int j = 0; j < msize; j++)
                {
                    influence[i, j] = new influnceField();
                    iwrite[i, j] = new influnceField();
                }
            //fluid = emission * 30;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
             //clear the influnce grid

            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                scale++;
            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                scale--;
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Length() > .9)
                emission = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right*scale;
            location += GamePad.GetState(PlayerIndex.One).ThumbSticks.Left*scale;
            if (var == 0)
                for (int i = 0; i < 10; i++)
                {
                    Vector2 spot = location;
                    spot.X += rand.Next(emissionWidth);
                    spot.Y += rand.Next(emissionHeight);
                    particles[step].location = spot;

                    if (!particles[step].active) particles[step].active = true;

                    particles[step].drift = emission;
                    step = ++step % size;
                }
            var = ++var % emissionRate;
           
          
            for (int i = 0; i < size ; i++)
            {
               
             
                particles[i].drift += wind;
                particles[i].location += particles[i].drift; 
                if (Vector2.DistanceSquared(particles[i].location, gravity) < 7000)
                {


                    float dscale = particles[i].drift.Length();
                    particles[i].drift = Vector2.Reflect(particles[i].drift, gravity - particles[i].location+particles[i].drift);
                    particles[i].drift.Normalize();
                    particles[i].drift *= dscale; 
                    particles[i].location += particles[i].drift; 
                    particles[i].drift *=.5f;
                }
                if (Vector2.DistanceSquared(particles[i].location,fluid) < 11000)
                {
                    float dscale = particles[i].drift.Length();
                    particles[i].drift = Vector2.Reflect(particles[i].drift, fluid - particles[i].location + particles[i].drift);
                    particles[i].drift.Normalize();
                    particles[i].drift *= dscale;
                    particles[i].location += particles[i].drift;
                    particles[i].drift *= .4f;
                }
                if (Vector2.DistanceSquared(particles[i].location, circle3) < 13000)
                {
                    float dscale = particles[i].drift.Length();
                    particles[i].drift = Vector2.Reflect(particles[i].drift, circle3 - particles[i].location + particles[i].drift);
                    particles[i].drift.Normalize();
                    particles[i].drift *= dscale;
                    particles[i].location += particles[i].drift;
                    particles[i].drift *= .3f;
                }
              
             
                
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
         {
             sprites.Begin();
             base.Draw(gameTime);
             for (int i = size-1; i > 0; i--)
                 if (particles[i].active)
                    sprites.Draw(texture, particles[i].location,Color.White);
             sprites.End();
        }
     
      
    }
    
}
