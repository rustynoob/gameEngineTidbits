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
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //BasicDrawableObject player;
        particleSystem ps;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //player = new BasicDrawableObject(this);
            ps = new particleSystem(this, new Vector2(430,560 ), new Vector2(-.006f, .22f), 10000, new Vector2(-.0f, -9.4f), 1);
            
            Content.RootDirectory = "Content";
        }
////////////////////////////// INITILIZE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        protected override void Initialize()
        {

            //Components.Add(player);
            Components.Add(ps);
            base.Initialize();
        }


///////////////////////////////// LOAD STUFF  \\\\\\\\\\\\\\\\\\\\\\\\\\\

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent() { }


/////////////////////////////// UPDATE STUFF \\\\\\\\\\\\\\\\\\\\\\\\\\\\
        protected override void Update(GameTime gameTime)
        {
            //player.incDir();
            //player.move(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right);
         

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                
                this.Exit();
            }

            base.Update(gameTime);
        }

//////////////////////////////// DRAW STUFF \\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
