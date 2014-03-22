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


namespace learning
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GameComponent1 : Microsoft.Xna.Framework.DrawableGameComponent
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D myTexture;
        ContentManager Content;
        Vector2 spritePosition = Vector2.Zero;
        Vector2 spriteSpeed = Vector2.One;
        float speedScale = 100;// Vector2.One * 100;
        Vector2 direction = new Vector2();
        Vector2 turnmove = new Vector2();
        float dir = 0;
        float speed = 0;

        float vibrationAmount = 0.0f;
        
        public GameComponent1(Game game)
            : base(game)
        {
            graphics = new GraphicsDeviceManager(game);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Content.RootDirectory = "Content";
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            // Create a new SpriteBatch, which can be used to draw textures.

            myTexture = Content.Load<Texture2D>("lerningsp1");
            spriteSpeed *= 40;
            base.Initialize();
        }
        void UpdateInput()
        {
            // Get the current gamepad state.
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            // Process input only if connected and button A is pressed.
            if (currentState.IsConnected && currentState.Buttons.A ==
                ButtonState.Pressed)
            {
                // Button A is currently being pressed; add vibration.
                vibrationAmount =
                    MathHelper.Clamp(vibrationAmount + 0.03f, 0.0f, 1.0f);
                GamePad.SetVibration(PlayerIndex.One,
                    vibrationAmount, vibrationAmount);
            }
            else
            {
                // Button A is not being pressed; subtract some vibration.
                vibrationAmount =
                    MathHelper.Clamp(vibrationAmount - 0.05f, 0.0f, 1.0f);
                GamePad.SetVibration(PlayerIndex.One,
                    vibrationAmount, vibrationAmount);
            }
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            UpdateInput();
            {
                // Move the sprite by speed, scaled by elapsed time.
                spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                int MaxX =
                    graphics.GraphicsDevice.Viewport.Width - (myTexture.Width / 2);
                int MinX = myTexture.Width / 2;
                int MaxY = 
                    graphics.GraphicsDevice.Viewport.Height - (myTexture.Height / 2);
                int MinY = myTexture.Height / 2;

                // Check for bounce.
                if (spritePosition.X > MaxX)
                {
                    spriteSpeed.X *= -1;
                    spritePosition.X = MaxX;
                    vibrationAmount = 1;
                }

                else if (spritePosition.X < MinX)
                {
                    spriteSpeed.X *= -1;
                    spritePosition.X = MinX;
                    vibrationAmount = 1;
                }

                if (spritePosition.Y > MaxY)
                {
                    spriteSpeed.Y *= -1;
                    spritePosition.Y = MaxY;
                    vibrationAmount = 1;
                }

                else if (spritePosition.Y < MinY)
                {
                    spriteSpeed.Y *= -1;
                    spritePosition.Y = MinY;
                    vibrationAmount = 1;
                }
                // spriteSpeed = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right * speedScale;
                turnmove = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                dir += turnmove.X / 10;
                speed = turnmove.Y * speedScale;
                spriteSpeed = location(speed, dir);



            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(myTexture, spritePosition, null, Color.White, dir, new Vector2((float)(myTexture.Width * .5), (float)(myTexture.Height * .5)), 1, SpriteEffects.None, 1);
            //   spriteBatch.Draw(myTexture, spritePosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        public double angle(Vector2 chord)
        {
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
            return a;

        }
        public double angle(Vector2 chord1, Vector2 chord2)
        {
            return angle(chord1 - chord2);
        }
        public Vector2 location(float distance, double angle)
        {
            Vector2 result = new Vector2();
            result.X = distance * (float)Math.Cos(angle);
            result.Y = distance * (float)Math.Sin(angle);
            return result;
        }

    }
}
