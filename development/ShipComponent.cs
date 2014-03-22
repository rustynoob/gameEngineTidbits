using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace aotv.Objects
{
    public class ShipComponent : Core.Object
    {
        public Components.GraphicsComponent graphics;
        public Components.OrientationComponent orientation;

        // Moving graphics related things into the new Drawable object
        public Vector2 imageOrigin;

        // public RotatedRectangle rotatedrectangle;
        private int smokeDelay = 10;
        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 position2 = new Vector3(0, 0, 0);
        public float Angle;
        public float scale = 1;
        public string type;
        public int health = 0;
        public double mass = 0;
        public int cargo = 0;
        public float thrust;
        public float maneuverability;
        public float powerConsumption;
        public float powerGeneration;

        public Vector2 scale2d;
        public SpriteEffects myEffect = SpriteEffects.None;
        public float myDepth;
        public int mywidth;
        public int myheight;
        public Game1 game;
        public Color[,] ColorArray;
        public double mag;
        public double relativeTheta;
        public string variety = "";
        public List<Decoration> myEffectDecorations;

        public int trailDelay = 0;
        public Decoration[] myTrailDecorations;
        public Vector3[] myTrailDrift;
        public int myTrailSize = 150;
        int nextTrail = 0;

        
        public Ship parentShip;

       // public Ship parent;

        public ShipComponent(string name, Game1 rootGame, Ship ParentShip, string partType, Vector3 position, float rotation)
            : base(name, ParentShip)
        {
            this.game = rootGame;
            this.parent = ParentShip;
            this.type = partType;
            this.position = position;
            myDepth = ParentShip.numParts;
            position2 = position;
            string[] varietyfinder = partType.Split('_');
            variety = varietyfinder[1];
            Angle = rotation;
            mag = Math.Sqrt(position2.X * position2.X + position2.Y * position2.Y);
            myEffectDecorations = new List<Decoration>();

           

            parentShip = (Ship)parent;

            if (position.Y == 0)
            {
                relativeTheta = 0;
            }
            else
            {
                if (position.X < 0)
                {
                    relativeTheta = Math.Atan(position.Y / position2.X) + Math.PI;
                }
                else
                {
                    relativeTheta = Math.Atan(position.Y / position2.X);
                }
            }

            myEffect = SpriteEffects.None;
            

            orientation = new Components.OrientationComponent();
            if (position.X < 0)
            {
                //orientation.scale.Y = -orientation.scale.Y;
                myEffect = SpriteEffects.FlipHorizontally;
            }

            position.Z = myDepth / 100;
            Console.WriteLine(position.Z);
            orientation = new Components.OrientationComponent();
            orientation.position = position;
            orientation.rotation = rotation;
            components.Add(Components.OrientationComponent.getTypeName(), orientation);
            Decoration decor;

            switch (partType)
            {
                case "USF1_Engine_1":
                    imageOrigin = new Vector2(75, 20);
                    health = 100;
                    thrust = .5f;
                    maneuverability = .25f;
                    powerConsumption = 1.1f;
                   
                    
                    decor = new Decoration(
                        "shipTrail", game, this,
                        new aotv.Components.GraphicsComponent(game.Content.Load<Texture2D>("engineThrust")),
                        new aotv.Components.OrientationComponent(orientation.rotation)
                    );
                    
                    decor.graphics.color = Color.Cyan;
                    decor.graphics.center = new Vector2(40, 331); 
                    decor.orientation.rotation -= MathHelper.Pi;
                    decor.orientation.parent = orientation;
                    game.core.addObject(decor);
                      // trail
                    myTrailDecorations = new Decoration[myTrailSize];
                    myTrailDrift = new Vector3[myTrailSize];
                    for (int i = 0; i < myTrailDecorations.Length; i++)
                    {
                        decor = new Decoration(
                               "particle2", game, null,
                               new aotv.Components.GraphicsComponent(game.Content.Load<Texture2D>("particle2")),
                               new aotv.Components.OrientationComponent(orientation.rotation)
                           );
                        decor.orientation.position = orientation.position + parentShip.orientation.position;
                        decor.orientation.rotation -= MathHelper.Pi;
                        decor.orientation.parent = orientation;
                        decor.graphics.opacity = 0;
                        decor.graphics.center = new Vector2(50, 50);
                        decor.graphics.color = Color.DarkGray;
                        myTrailDecorations[i] = decor;
                        game.core.addObject(decor);
                        myTrailDrift[i] = orientation.worldPosition;
                        myTrailDrift[i].Z = 0.01f;
                    }
                    break;
                case "USF1_Base_1":
                    imageOrigin = new Vector2(107, 160);
                    health = 1000;
                    mass = 2;
                    break;
                case "USF1_Crew_1":
                    imageOrigin = new Vector2(55, 59);
                    health = 50;
                    mass = .5;
                    break;
                case "USF1_Hull_1":
                    imageOrigin = new Vector2(46, 188);
                    health = 200;
                    mass = 1.5;
                    break;
                case "USF1_Hull_2":
                    imageOrigin = new Vector2(77, 112);
                    health = 300;
                    mass = 2;
                    break;
                case "neutral_asteroid_1":
                    imageOrigin = new Vector2(93, 122);
                    health = 100;
                    break;
                case "RUF1_Hull_1":
                    health = 1000;
                    mass = 2;
                    break;
                case "RUF1_Hull_2":
                    health = 50;
                    mass = .5;
                    break;
                case "RUF1_Hull_3":
                    health = 200;
                    mass = 1.5;
                    break;
                case "RUF1_Crew_1":
                    health = 300;
                    mass = 2;
                    break;
                case "RUF1_Energy_1":
                    imageOrigin = new Vector2(30, 120);
                    powerGeneration = 1;
                    health = 100;
                    mass = 2;
                    break;
                case "RUF1_Engine_1":
                    health = 100;
                    thrust = .3f;
                    maneuverability = .25f;
                    powerConsumption = 1.1f;
                    Debug.WriteLine("DECOR");
                    decor = new Decoration(
                        "shipTrail", game, this,
                        new aotv.Components.GraphicsComponent(game.Content.Load<Texture2D>("engineThrust")),
                        new aotv.Components.OrientationComponent(orientation.rotation)
                    );
                    decor.graphics.color = Color.OrangeRed;
                    decor.graphics.center = new Vector2(40, 331);
                    decor.orientation.position.Z = .1f;
                    decor.orientation.rotation -= MathHelper.Pi;
                    game.core.addObject(decor);
                    myEffectDecorations.Add(decor);

                    decor = new Decoration(
                        "engineRadiance", game, this,
                        new aotv.Components.GraphicsComponent(game.Content.Load<Texture2D>("engineRadiance")),
                        new aotv.Components.OrientationComponent(orientation.rotation)
                    );
                    decor.orientation.position.Z = .2f;
                    decor.graphics.color = Color.OrangeRed;
                    decor.graphics.center = new Vector2(50, 50);
                    decor.orientation.rotation -= MathHelper.Pi;
                    game.core.addObject(decor);
                    myEffectDecorations.Add(decor);

                    // trail
                    myTrailDecorations = new Decoration[myTrailSize];
                    myTrailDrift = new Vector3[myTrailSize];
                    for (int i = 0; i < myTrailDecorations.Length; i++)
                    {
                        decor = new Decoration(
                               "particle2", game, null,
                               new aotv.Components.GraphicsComponent(game.Content.Load<Texture2D>("particle2")),
                               new aotv.Components.OrientationComponent(orientation.rotation)
                           );
                        decor.orientation.position = orientation.position + parentShip.orientation.position;
                        decor.orientation.rotation -= MathHelper.Pi;
                        decor.orientation.parent = orientation;
                        decor.graphics.opacity = 0;
                        decor.graphics.center = new Vector2(50, 50);
                        decor.graphics.color = Color.White;
                        myTrailDecorations[i] = decor;
                        game.core.addObject(decor);
                        myTrailDrift[i] = orientation.worldPosition;
                        myTrailDrift[i].Z = 0.01f;
                    }
                    break;
            }

            // Load resources from main ContentManager and store references
            graphics = new Components.GraphicsComponent(rootGame.Content.Load<Texture2D>(partType));
            graphics.effect = myEffect;
            if (graphics.center == null)
            {
                graphics.center = new Vector2(graphics.texture.Width / 2, graphics.texture.Height / 2);
            }
            components.Add(Components.GraphicsComponent.getTypeName(), graphics);

        }

        public override void update()
        {
            if (variety == "Engine")
            {

                foreach (Decoration decors in myEffectDecorations)
                {
                        decors.orientation.scale.Y = ((Ship)parent).fThrust / 6;
                }


                for (int i = 0; i < myTrailSize; i++)
                {

                    myTrailDecorations[i].graphics.opacity -= .05f/(game.random.Next(42)+1);
                    myTrailDecorations[i].orientation.position += myTrailDrift[i];
                   
                }
                if (myTrailDecorations[nextTrail].graphics.opacity <= .0)
                {
                    myTrailDecorations[nextTrail].orientation.position = orientation.worldPosition;
                  
                    myTrailDecorations[nextTrail].orientation.position.X += game.random.Next(50)-25;
                    myTrailDecorations[nextTrail].orientation.position.Y += game.random.Next(50)-25;
                    // -(float)Math.PI;
                    myTrailDrift[nextTrail] = new Vector3(
                        parentShip.location(parentShip.Thrust, 
                        parentShip.orientation.rotation + 1.5 - (3/(game.random.Next(42)+1))
                        )
                        , 0);
                    
                    myTrailDrift[nextTrail].X += 1/(game.random.Next(42)+1);
                    myTrailDrift[nextTrail].Y += 1/(game.random.Next(42)+1);
                    myTrailDrift[nextTrail] *= 8+(1 / (game.random.Next(42) + 1)*2);
                    myTrailDecorations[nextTrail].graphics.opacity = 1;
                    nextTrail = nextTrail < myTrailSize - 1 ? nextTrail + 1 : 0;
                }
            }
        }

        public void setup()
        {
         //   ColorArray = TextureTo2DArray(game.getTexture(type));
            //game.collisionReady = true;
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

        public Rectangle getRectangle()
        {
            return new Rectangle((int)(position.X + ((Ship)parent).orientation.position.X - (imageOrigin.X * .4 / 2)), (int)(position.Y + ((Ship)parent).orientation.position.Y - (imageOrigin.Y * .4 / 2)), (int)(imageOrigin.X * .4), (int)(imageOrigin.Y * .4));
            //   return rotatedrectangle = new RotatedRectangle(new Rectangle((int)(position.X + parent.position.X - (imageOrigin.X * .4 / 2)), (int)(position.Y + parent.position.Y - (imageOrigin.Y * .4 / 2)), (int)(imageOrigin.X * .4), (int)(imageOrigin.Y * .4)),Angle);

        }


        // this gives us a place to put what happens when a part gets shot
        // it would probably be a good idea to make a function for being shot with an energy weapon vs bing shot with a bullet or an explosive device
        public void shoot(int damage)
        {
            if (health > 0)
                health -= damage;
            else
            {
                // we also need to remove all components mounted to this one
                isActive = false;
            }
        }
    }
}
