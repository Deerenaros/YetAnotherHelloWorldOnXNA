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
    public class DebugDrawer : Microsoft.Xna.Framework.DrawableGameComponent {
        // different fields (variables)
        #region fields

        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Dictionary< string, Dictionary< string, DebugMethod > > dbgOut = new Dictionary<string, Dictionary<string, DebugMethod>> ();
        // fps counter... strange, but don't working
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        // /fps counter
        public delegate string DebugMethod ();

        #endregion


        // xna inits, updates and drawers
        #region XNAInitsAndEvents

        public DebugDrawer ( Game game ) : base ( game ) {
            content = new ContentManager ( game.Services );
            content.RootDirectory = "Content";
        }

        public override void Initialize () {
            base.Initialize ();
        }

        protected override void LoadContent () {
            spriteBatch = new SpriteBatch ( GraphicsDevice );
            spriteFont = content.Load<SpriteFont> ( "terminal" );

            base.LoadContent ();
        }

        protected override void UnloadContent () {
            content.Unload ();
        }

        public override void Update ( GameTime gameTime ) {
            elapsedTime += gameTime.ElapsedGameTime;

            if ( elapsedTime > TimeSpan.FromSeconds ( 1 ) ) {
                elapsedTime -= TimeSpan.FromSeconds ( 1 );
                frameRate = frameCounter;
                frameCounter = 0;
            }

            base.Update ( gameTime );
        }

        public override void Draw ( GameTime gameTime ) {
            frameCounter++;
            string res = string.Format ( "fps: {0}", frameRate ) + "\r\n";

            foreach ( var dbgSystem in dbgOut ) {
                res += dbgSystem.Key + ":\r\n";
                foreach ( var dbgEl in dbgSystem.Value ) {
                    res += "\t" + dbgEl.Key + ": " + dbgEl.Value () + "\r\n";
                }
            }

            spriteBatch.Begin ();

            spriteBatch.DrawString ( spriteFont, res, new Vector2 ( 20, 22 ), Color.Black);
            spriteBatch.DrawString ( spriteFont, res, new Vector2 ( 22, 20 ), Color.Black);
            spriteBatch.DrawString ( spriteFont, res, new Vector2 ( 20, 20 ), Color.WhiteSmoke );

            spriteBatch.End ();

            base.Draw ( gameTime );
        }

        #endregion


        // logic for this singleton
        #region logic

        public void addDebugOutputToDraw ( string systemName, string elementName, DebugMethod dbgOuter ) {
            if ( !dbgOut.ContainsKey ( systemName ) ) {
                dbgOut[ systemName ] = new Dictionary<string, DebugMethod> ();
            }

            dbgOut[ systemName ][ elementName ] = dbgOuter;
        }

        #endregion
    }
}
