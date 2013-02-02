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

namespace WindowsGame6 {
    // main class... nothing to say... class-aggregator
    public class Game1 : Microsoft.Xna.Framework.Game {
        // global singleton that provides global work
        #region singletons

        public static InputManager inputs;
        public static EventManager events;
        public static DebugDrawer dbgDrawer;

        #endregion

        
        // fields...
        #region fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont terminal;
        World world;

        #endregion


        // XNA constructor and methods
        #region XNA

        public Game1 () {
            graphics = new GraphicsDeviceManager ( this );
            inputs = new InputManager ( this );
            events = new EventManager ( this );
            world = new World ( this );

            world.makeTileSetFromString ( "grass:g ground:G stone:S sizex:15 sizey:11",
                                          "ggggggggggggggg",
                                          "gGGGSGSGGGgGGGg",
                                          "gGGSSGSSGGgGSGg",
                                          "gGSSSGSSSGgGGGg",
                                          "gSSSSGSSSSggggg",
                                          "gGGGGGGGGGggggg",
                                          "gSSSSGSSSSggggg",
                                          "gGSSSGSSSGgGGGg",
                                          "gGGSSGSSGGgGSGg",
                                          "gGGGSGSGGGgGGGg",
                                          "ggggggggggggggg");

            Content.RootDirectory = "Content";
        }

        public void exit ( EventManager.ActionInfo nullAct ) {
            Exit ();
        }

        protected override void Initialize () {
            //input manager
            Components.Add ( inputs );
            Services.AddService ( typeof ( InputManager ), inputs );

            inputs.setBind ( InputManager.GameButtons.exit, events.addEvent( exit, new EventManager.ActionInfo( "" ) ) );

            //event manager
            Components.Add ( events );
            Services.AddService ( typeof ( EventManager ), events );

            //framerate drawer
            Components.Add ( dbgDrawer = new DebugDrawer ( this ) );
            dbgDrawer.UpdateOrder = 100500;
            dbgDrawer.DrawOrder = 100500;
            Services.AddService ( typeof ( DebugDrawer ), dbgDrawer );

            //world
            Components.Add ( world );
            Services.AddService ( typeof ( World ), world );
            world.DrawOrder = 0;

            base.Initialize ();
        }

        protected override void LoadContent () {
            spriteBatch = new SpriteBatch ( GraphicsDevice );

            terminal = Content.Load<SpriteFont> ( "terminal" );
        }

        protected override void Update ( GameTime gameTime ) {
            base.Update ( gameTime );
        }

        protected override void Draw ( GameTime gameTime ) {
            GraphicsDevice.Clear ( Color.CornflowerBlue );

            base.Draw ( gameTime );
        }

        #endregion
    }
}
