using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace aotv
{
    /// <summary>
    /// this thing does multi threading
    /// </summary>
    public class Threader
    {
        cd2 CD;
        Game1 game;

        public Threader(Game1 game)
        {
            this.game = game;
            CD = new cd2(game);
            // TODO: Construct any child components here
        }
        private Thread RunningThread { get; set; }
        private void run()
        {
            #if XBOX
                   Thread.CurrentThread.SetProcessorAffinity(5);
            #endif
      
            //while (true)
            //{
            //    work();
            //}
        }

        public void StartOnNewThread()
        {
            ThreadStart ts = new ThreadStart(run);
            RunningThread = new Thread(ts);
            RunningThread.Start();
        }

        public virtual void work()
        {
            if (game.beamList.Count > 0)
                for (int b = game.beamList.Count; b >= 0; b--)
                    if (game.beamList[b].isActive)
                        foreach (ship s in game.objList)
                            if (!s.Equals(game.beamList[b].parent))
                                foreach (ShipPart sc in s.partList)
                                {
                                    game.colisionPoint = CD.checkCollision(sc, game.beamList[b].position, game.beamList[b].Angle);
                                    if (game.colisionPoint != Vector2.One * -1)
                                    {
                                        sc.drawable.color = Color.Red;//.shoot(game.beamList[b].power);
                                        Thread.MemoryBarrier();
                                    }
                                }
           
        }
        public void kill()
        {
            RunningThread.Abort();
        }
    }
}
