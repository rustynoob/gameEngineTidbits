// to use put declare the buffer and manager
//
// public class Game1 : Microsoft.Xna.Framework.Game
//    {
//        [...]
//        DoubleBuffer doubleBuffer;
//        RenderManager renderManager;
//        UpdateManager updateManager;

// make sure that objects are added to the update manager and render managers in the same order
// after all the data is loaded 
//
// protected override void LoadContent()
//        {
//            [...]
//            doubleBuffer = new DoubleBuffer();
//            renderManager = new RenderManager(doubleBuffer, this);
//            renderManager.LoadContent();
//            updateManager = new UpdateManager(doubleBuffer, this);
//            //here, you can load data and add it to the RenderDataObjects list and GameDataObjects list
//            renderManager.RenderDataOjects.Add(...);
//            updateManager.GameDataOjects.Add(...);
//            [...]
//            updateManager.StartOnNewThread();
//        }

// put syncronisation in draw function of main thread
//  
//    protected override void Draw(GameTime gameTime)
//    {
//       doubleBuffer.GlobalStartFrame(gameTime);
//       graphics.GraphicsDevice.Clear(Color.Black);
//            renderManager.DoFrame();
//            base.Draw(gameTime);
//            doubleBuffer.GlobalSynchronize();
//        }

// you can now put any code that you want to run in serial in the update function

// last but not least be sure to end the thread on exit
//
//<br />
//    protected override void OnExiting(object sender, EventArgs args)<br />
//    {<br />
//        doubleBuffer.CleanUp();<br />
//        if (updateManager.RunningThread != null)</p>
//<p>        updateManager.RunningThread.Abort();<br />
//    }<br />



using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Threading;

using System.Runtime.InteropServices;

namespace Xbox360Game3
{
    class ThreadManager
    {
        Game game;
        DoubleBuffer doubleBuffer;
        RenderManager renderManager;

        UpdateManager updateManager;
        public void LoadContent(Game game)
        {
            this.game = game;
            doubleBuffer = new DoubleBuffer();
            renderManager = new RenderManager(doubleBuffer, game);
            renderManager.LoadContent();
            updateManager = new UpdateManager(doubleBuffer, game);
        }
        public void Add(RenderData user, GameData worker)
        {
            renderManager.RenderDataOjects.Add(user);
            updateManager.GameDataOjects.Add(worker);
        }
        public void start(int thread)
        {
            updateManager.StartOnNewThread();
        }

        // throw this where ever you want to make sre everything gets synchronized probably in the worker thread when it compleats
        public void syncThreadData(GameTime gameTime)
        {
            doubleBuffer.GlobalStartFrame(gameTime);
            renderManager.DoFrame();

            doubleBuffer.GlobalSynchronize();
        }
        //      put this in
        //      protected override void OnExiting(object sender, EventArgs args)
        public void Stop()
        {
            doubleBuffer.CleanUp();
            if (updateManager.RunningThread != null)
                updateManager.RunningThread.Abort();
        }
    }

    class DoubleBuffer
    {
        private ChangeBuffer[] buffers;
        private volatile int currentUpdateBuffer;
        private volatile int currentRenderBuffer;
        private AutoResetEvent renderFrameStart;
        private AutoResetEvent renderFrameEnd;
        private AutoResetEvent updateFrameStart;
        private AutoResetEvent updateFrameEnd;
        private volatile GameTime gameTime;

        public DoubleBuffer()
        {
            //create the buffers
            buffers = new ChangeBuffer[2];
            buffers[0] = new ChangeBuffer();
            buffers[1] = new ChangeBuffer();
            //create the WaitHandlers
            renderFrameStart = new AutoResetEvent(false);
            renderFrameEnd = new AutoResetEvent(false);
            updateFrameStart = new AutoResetEvent(false);
            updateFrameEnd = new AutoResetEvent(false);
            //reset the values<
            Reset();
        }

        public void Reset()
        {
            //reset the buffer indices
            currentUpdateBuffer = 0;
            currentRenderBuffer = 1;
            //set all events to non-signaled
            renderFrameStart.Reset();
            renderFrameEnd.Reset();
            updateFrameStart.Reset();
            updateFrameEnd.Reset();
        }
        public void CleanUp()
        {
            //relese system resources
            renderFrameStart.Close();
            renderFrameEnd.Close();
            updateFrameStart.Close();
            updateFrameEnd.Close();
        }
        private void SwapBuffers()
        {
            currentRenderBuffer = currentUpdateBuffer;
            currentUpdateBuffer = (currentUpdateBuffer + 1) % 2;
        }
        public void GlobalStartFrame(GameTime gameTime)
        {
            this.gameTime = gameTime;
            SwapBuffers();
            //signal the render and update threads to start processing
            renderFrameStart.Set();
            updateFrameStart.Set();
        }
        public void GlobalSynchronize()
        {
            //wait until both threads signal that they are finished
            renderFrameEnd.WaitOne();
            updateFrameEnd.WaitOne();
        }
       
      
        public void StartUpdateProcessing(out ChangeBuffer updateBuffer, out GameTime gameTime)
        {
            //wait for start signal
            updateFrameStart.WaitOne();
            //ensure cache coherency
            Thread.MemoryBarrier();
            //get the update buffer
            updateBuffer = buffers[currentUpdateBuffer];
            //get the game time
            gameTime = this.gameTime;
        }
        public void StartRenderProcessing(out ChangeBuffer renderBuffer, out GameTime gameTime)
        {
            //wait for start signal
            renderFrameStart.WaitOne();
            //ensure cache coherency
            Thread.MemoryBarrier();
            //get the render buffer
            renderBuffer = buffers[currentRenderBuffer];
            //ret the game time
            gameTime = this.gameTime;
        }
        public void SubmitUpdate()
        {
            //ensure cache coherency
            Thread.MemoryBarrier();
            //update is done
            updateFrameEnd.Set();
        }
        public void SubmitRender()
        {
            //ensure cache coherency
            Thread.MemoryBarrier();
            //render is done
            renderFrameEnd.Set();
        } 
    
    }
// define change message types
    public enum ChangeMessageType
    {
        UpdateCameraView,
        UpdateWorldMatrix,
        UpdateHighlightColor,
        CreateNewRenderData,
        DeleteRenderData
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ChangeMessage
    {
        //this appears in all messages
        //identifies how this message should be interpreted
        [FieldOffset(0)]
        public ChangeMessageType MessageType;
        //this is the field required when this message is of type UpdateCameraView
        [FieldOffset(4)]
        public Matrix CameraViewMatrix;
        //this field is used for all messages dealing with entities
        [FieldOffset(4)]
        public int ID;
        //this is the field required when this message is of type UpdateWorldMatrix
        [FieldOffset(8)]
        public Matrix WorldMatrix;
        //this is the field required when this message is of type UpdateHighlightColor
        [FieldOffset(8)]
        public Vector4 HighlightColor;
        //this is the field required when this message is of type CreateNewRenderData</p>
        [FieldOffset(8)]
        public Vector3 Position;
        [FieldOffset(20)]
        public Vector3 Color;
        //nothing is required when this message is of type DeleteRenderData<br />
    }

        // using the change buffer

        ////create a message to update the camera view matrix
        //ChangeMessage updateCamera = new ChangeMessage();
        //updateCamera.MessageType = ChangeMessageType.UpdateCameraView;
        //updateCamera.CameraViewMatrix = Matrix.CreateLookAt(...);
            
        
        ////create a message to update the world matrix of the object with index 5
        //ChangeMessage updateWorld = new ChangeMessage();
        //updateWorld.MessageType = ChangeMessageType.WorldMatrix;
        //updateWorld.ID = 5;
        //updateWorld.UpdatedWorldMatrix = Matrix.CreateTranslation(...);
        
        //switch (msg.MessageType)
        //{
        //    case ChangeMessageType.UpdateWorldMatrix:
        //        camera.View = msg.CameraViewMatrix;
        //        break;
        //    case ChangeMessageType.UpdateCameraView:
        //        renderObjects[msg.ID].World = msg.UpdatedWorldMatrix;
        //        break;
        //    [...]
            
        //}

        public class ChangeBuffer
        {
            public List<ChangeMessage> Messages { get; set; }
            public ChangeBuffer()
            {
                Messages = new List<ChangeMessage>();
            }
            public void Add(ChangeMessage msg)
            {
                Messages.Add(msg);
            }
            public void Clear()
            {
                Messages.Clear();
            }
        }
    class GameData
    {
        public Vector3 Acceleration;
        public Vector3 Velocity;
        public Vector3 Position;
        public Matrix Rotation;
        public bool IsAlive;
    }
    class RenderData
    {
        public Vector3 HighlightColor;
        public Matrix WorldMatrix;
        public Model Model;
        public bool IsAlive;
    }
     class UpdateManager
    {
        public List<GameData> GameDataOjects { get; set; }
        private DoubleBuffer doubleBuffer;
        private GameTime gameTime;
        protected ChangeBuffer messageBuffer;
        protected Game game;
        public UpdateManager(DoubleBuffer doubleBuffer, Game game)
        {
            this.doubleBuffer = doubleBuffer;
            this.game = game;
            this.GameDataOjects = new List<GameData>();
        }
        public void DoFrame()
        {
            doubleBuffer.StartUpdateProcessing(out messageBuffer, out gameTime);
            this.Update(gameTime);
            doubleBuffer.SubmitUpdate();
        }
        public virtual void Update(GameTime gameTime)
        {
        }
         public Thread RunningThread { get; set; }
                private void run()
                {
                    #if XBOX
                   Thread.CurrentThread.SetProcessorAffinity(5);
                    #endif
                    while (true)
                    {
                        DoFrame();
                    }
                }
                public void StartOnNewThread()
                { 
                    ThreadStart ts = new ThreadStart(run);
                    RunningThread = new Thread(ts);
                    RunningThread.Start();
                }
        }
     class RenderManager
    {
        public List<RenderData> RenderDataOjects { get; set; }
        private DoubleBuffer doubleBuffer;
        private GameTime gameTime;
        protected ChangeBuffer messageBuffer;
        protected Game game;
        public RenderManager(DoubleBuffer doubleBuffer, Game game)
        {
            this.doubleBuffer = doubleBuffer;
            this.game = game;
            this.RenderDataOjects = new List<RenderData>();
        }
        public virtual void LoadContent()
        {
        }
        public void DoFrame()
        {
           doubleBuffer.StartRenderProcessing(out messageBuffer, out gameTime);
            this.Draw(gameTime);
            doubleBuffer.SubmitRender();
        }
        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}

