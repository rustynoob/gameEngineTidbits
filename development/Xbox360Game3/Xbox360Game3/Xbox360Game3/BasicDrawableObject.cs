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
    public class BasicDrawableObject : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D image;
        private SpriteBatch sprites;
        private Vector2 position = Vector2.Zero;
        private float direction = 0;
        private Vector2 origin;
        private Vector2 scale = Vector2.One*100;
        private Color color = Color.White;
        private SpriteEffects effect = SpriteEffects.None;
        private float depth = 1;

        public BasicDrawableObject(Game game) : base(game)
        {
            // load members here
        }
        
        protected override void LoadContent()
        {
            sprites = new SpriteBatch(this.Game.GraphicsDevice);
            image = Game.Content.Load<Texture2D>("lerningsp1");

            origin = new Vector2(image.Height / 2, image.Width / 2);
            base.LoadContent();
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here
            position = new Vector2(100, 100);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {

            sprites.Begin();

            sprites.Draw(image,position,image.Bounds,color,MathHelper.ToRadians(direction),origin,1,effect,depth);

            sprites.End();

            base.Draw(gameTime);
            
        }
        public void setDirection(float dir)
        {
            direction = dir % 360;
        }
        public void incDir() 
        {
            direction = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y,GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X);
        }
        public float getDirection() 
        {
            return direction;
        }
        public void place(float x, float y)
        {
            position = new Vector2(x,y);
        }
        public void move(Vector2 offset) { position += offset; }
    }
}
