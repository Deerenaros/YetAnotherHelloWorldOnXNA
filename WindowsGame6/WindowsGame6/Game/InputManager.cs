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
    //PLEASE!!! DO NOT SEE INSIDE! This's the worstest architecture that i have ever made... maybe that i seen too...
    public class InputManager : Microsoft.Xna.Framework.GameComponent {
        // on the rights of debug's needs
        #region debug

        public string getPressedButtons () {
            string res = "(";
            bool first = true;
            foreach ( var el in kbState.GetPressedKeys () ) {
                if ( keyboardToInput.ContainsKey ( el ) ) {
                    res += ( first ? "" : ", " ) + keyboardToInput[ el ].ToString ();
                    first = false;
                }
            }

            return res + ")";
        }

        #endregion


        // Structures and enumes.
        #region structures

        public enum Type {
            pressing,
            atOnce
        }

        public enum GameButtons {
            left,
            up,
            right,
            down,
            exit,
            pause
        };

        public class PressButtonType {
            public GameButtons button;
            public Type type;

            public PressButtonType ( GameButtons btn, Type t ) {
                button = btn;
                type = t;
            }
        }

        #endregion

        // Logic fields.
        #region fields

        GamePadState[] gpsStates = {
                                        GamePad.GetState( PlayerIndex.One   ),
                                        GamePad.GetState( PlayerIndex.Two   ),
                                        GamePad.GetState( PlayerIndex.Three ),
                                        GamePad.GetState( PlayerIndex.Four  )
                                   };
        KeyboardState kbState = Keyboard.GetState ();
        HashSet< GameButtons > processedButtons = new HashSet<GameButtons> ();
        List< PressButtonType > toRemoveFromBinds = new List<PressButtonType> ();
        List< PressButtonType > toAdd = new List<PressButtonType> ();
        Dictionary< PressButtonType, int > binds = new Dictionary<PressButtonType, int> ();
        Dictionary< Buttons, GameButtons > gamePadToInput = new Dictionary<Buttons, GameButtons> ();
        Dictionary< Keys, GameButtons > keyboardToInput = new Dictionary<Keys, GameButtons> ();

        #endregion


        // Methods and constructors that provided by XNA.
        #region XNA

        public InputManager ( Game game ) : base ( game ) {
            gamePadToInput.Add ( Buttons.DPadLeft, GameButtons.left );
            gamePadToInput.Add ( Buttons.DPadUp, GameButtons.up );
            gamePadToInput.Add ( Buttons.DPadRight, GameButtons.right );
            gamePadToInput.Add ( Buttons.DPadDown, GameButtons.down );
            gamePadToInput.Add ( Buttons.Back, GameButtons.exit );
            gamePadToInput.Add ( Buttons.Start, GameButtons.pause );

            keyboardToInput.Add ( Keys.Left, GameButtons.left );
            keyboardToInput.Add ( Keys.Up, GameButtons.up );
            keyboardToInput.Add ( Keys.Right, GameButtons.right );
            keyboardToInput.Add ( Keys.Down, GameButtons.down );
            keyboardToInput.Add ( Keys.Escape, GameButtons.exit );
            keyboardToInput.Add ( Keys.Space, GameButtons.pause );
        }

        public override void Initialize () {
            Game1.dbgDrawer.addDebugOutputToDraw ( "InputManager", "pressed keys", getPressedButtons );
            base.Initialize ();
        }

        public override void Update ( GameTime gameTime ) {
            foreach ( var bnd in toRemoveFromBinds ) {
                
            }
            toRemoveFromBinds.Clear ();

            kbState = Keyboard.GetState ();
            gpsStates[ 0 ] = GamePad.GetState ( PlayerIndex.One );
            gpsStates[ 1 ] = GamePad.GetState ( PlayerIndex.Two );
            gpsStates[ 2 ] = GamePad.GetState ( PlayerIndex.Three );
            gpsStates[ 3 ] = GamePad.GetState ( PlayerIndex.Four );

            foreach ( Keys k in keyboardToInput.Keys ) {
                GameButtons btn = keyboardToInput[ k ];
                if ( kbState.IsKeyDown ( k ) ) {
                    foreach ( var bnd in binds ) {
                        if ( bnd.Key.button == btn ) {
                            switch ( bnd.Key.type ) {
                                case Type.atOnce:
                                    if ( !processedButtons.Contains ( bnd.Key.button ) ) {
                                        processedButtons.Add ( bnd.Key.button );
                                        Game1.events.runEvent ( binds[ bnd.Key ] );
                                    }
                                    break;
                                default:
                                    Game1.events.runEvent ( binds[ bnd.Key ] );
                                    break;
                            } // switch bnd.type
                            break;
                        } // if bnd.btn == btn
                    } // foreach bnd in binds
                } else {
                   if ( processedButtons.Contains( btn ) ) {
                       processedButtons.Remove( btn );
                   } // if
                } // else IsKeyDown
            } // foreach k in kb...ut.Keys

            foreach ( var bnd in toRemoveFromBinds ) {
                binds.Remove ( bnd );
            }
            toRemoveFromBinds.Clear ();

            base.Update ( gameTime );
        }

        #endregion


        // Provides binding logic to keypressing with EventManager
        #region bindLogic

        public void setBind ( GameButtons button, int bind, Type type = Type.pressing ) {
            foreach ( var bnd in binds ) {
                if ( bnd.Key.button == button ) {
                    throw new Exception ( "InputManager.setBind(...): Button have binded yet!" );
                }
            }
            
            binds.Add ( new PressButtonType ( button, type ), bind );
        }

        public void unsetBind ( GameButtons button ) {
            foreach ( var bnd in binds ) {
                if ( bnd.Key.button == button ) {
                    toRemoveFromBinds.Add ( bnd.Key );
                }
            }
        }

        #endregion
    }
}
