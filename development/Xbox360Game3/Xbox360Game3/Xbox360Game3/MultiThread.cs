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
using System.Threading;


namespace Xbox360Game3
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Crunchy 
    {
        private int size = 1;
        public int done = 0;

        public Crunchy  ()
        {
        
            // TODO: Construct any child components here
        }
        private Thread RunningThread { get; set; }
        private void run()
        {
            Thread.CurrentThread.SetProcessorAffinity(5);
            while (true)
            {
                work();
            }
        }

        public void StartOnNewThread()
        {
            ThreadStart ts = new ThreadStart(run);
            RunningThread = new Thread(ts);
            RunningThread.Start(); 
        }

        public virtual void  work(){
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    for (int k = 1; k < size; k++)
                        Math.Sqrt(i * j % k);
            done = size++;
             
            Thread.MemoryBarrier();
        }
        public void kill()
        {
            RunningThread.Abort();
        }
    }
}
