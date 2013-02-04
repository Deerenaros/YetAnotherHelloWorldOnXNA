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
            foreach ( var el in pressedButtons ) {
                res += ( first ? "" : ", " ) + el.type;
                first = false;
            }

            return res + ")";
        }

        #endregion


        // Structures and enumes.
        #region structures

        public enum GameButtons {
            left,
            up,
            right,
            down,
            exit,
            pause,
            notpressed
        }

        public class Button {
            GameButtons m_type;
            public bool m_processed = false;

            public GameButtons type {
                get {
                    m_processed = true;
                    return m_type;
                }
            }

            public bool processed {
                get {
                    bool res = m_processed;
                    m_processed = true;
                    return res;
                }
            }

            public Button ( GameButtons type ) {
                m_type = type;
            }

            public override int  GetHashCode(){
 	             return m_type.GetHashCode();
            }

            public override bool Equals ( object obj ) {
                if ( obj == null )
                    return false;
                Button btn = obj as Button;
                return this.Equals ( btn );
            }

            public bool Equals ( Button btn ) {
                return this.m_type == btn.m_type;
            }

            public static bool operator == ( Button left, Button right ) {
                return left.Equals ( right );
            }

            public static bool operator != ( Button left, Button right ) {
                return !left.Equals ( right );
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
        Dictionary< Buttons, GameButtons > gamePadToInput  = new Dictionary< Buttons, GameButtons > ();
        Dictionary< Keys, GameButtons > keyboardToInput = new Dictionary< Keys, GameButtons > ();
        Dictionary< string, GameButtons > associations = new Dictionary< string, GameButtons > ();
        //Dictionary< GameButtons, Buttons > gamePadToInput  = new Dictionary<GameButtons, Buttons > ();
        //Dictionary< GameButtons, Keys  > keyboardToInput = new Dictionary<GameButtons, Keys > ();
        public List< Button > pressedButtons = new List< Button > ();

        #endregion


        // Methods and constructors that provided by XNA.
        #region XNA

        public InputManager ( Game game )
            : base ( game ) {
            //gamePadToInput.Add ( Buttons.DPadLeft,  GameButtons.left );
            //gamePadToInput.Add ( Buttons.DPadUp,    GameButtons.up );
            //gamePadToInput.Add ( Buttons.DPadRight, GameButtons.right );
            //gamePadToInput.Add ( Buttons.DPadDown,  GameButtons.down );
            //gamePadToInput.Add ( Buttons.Back,      GameButtons.exit );
            //gamePadToInput.Add ( Buttons.Start,     GameButtons.pause );

            //keyboardToInput.Add ( GameButtons.left, Keys.Left );
            //keyboardToInput.Add ( GameButtons.up, Keys.Up );
            //keyboardToInput.Add ( GameButtons.right, Keys.Right );
            //keyboardToInput.Add ( GameButtons.down, Keys.Down );
            //keyboardToInput.Add ( GameButtons.exit, Keys.Escape );
            //keyboardToInput.Add ( GameButtons.pause, Keys.Space );

            keyboardToInput.Add ( Keys.Left, GameButtons.left );
            keyboardToInput.Add ( Keys.Up, GameButtons.up );
            keyboardToInput.Add ( Keys.Right, GameButtons.right  );
            keyboardToInput.Add ( Keys.Down, GameButtons.down );
            keyboardToInput.Add ( Keys.Escape, GameButtons.exit  );
            keyboardToInput.Add ( Keys.Space, GameButtons.pause );
        } // Contructor

        public override void Initialize () {
            Game1.dbgDrawer.addDebugOutputToDraw ( "InputManager", "pressed keys", getPressedButtons );
            base.Initialize ();
        } // Initialize

        public override void Update ( GameTime gameTime ) {
            GamePadState[] gpsStates = {
                            GamePad.GetState( PlayerIndex.One   ),
                            GamePad.GetState( PlayerIndex.Two   ),
                            GamePad.GetState( PlayerIndex.Three ),
                            GamePad.GetState( PlayerIndex.Four  )
                        };
            KeyboardState kbState = Keyboard.GetState ();
            Button btn_ = new Button ( GameButtons.pause ), _btn = new Button ( GameButtons.pause );
            GameButtons b = btn_.type;
            if ( btn_ != _btn )
                return;

            foreach ( Keys k in keyboardToInput.Keys ) {
                Button btn = new Button ( keyboardToInput[ k ] ) ;
                if ( kbState.IsKeyDown( k ) ) {
                    if ( !pressedButtons.Contains ( btn ) ) {
                        pressedButtons.Add ( btn );
                    } // if
                } else {
                    if ( pressedButtons.Contains ( btn ) ) {
                        pressedButtons.Remove ( btn );
                    } // if
                } // if/else IsKeyDown
            } // foreach k

            base.Update ( gameTime );
        } // Update

        #endregion


        // provides usefull custom identification system
        #region associations
        public void associate ( string name, GameButtons button ) {
            associations[ name ] = button;
        }

        public bool this[ string name, bool once = false ] {
            get {
                if ( associations.ContainsKey ( name ) ) {
                    Button btn = new Button ( associations[ name ] );
                    foreach ( Button b in pressedButtons ) {
                        if ( b == btn ) {
                            return !(once && b.processed);
                        }
                    }
                }

                return false;
            }
        }
        #endregion
    }
}
