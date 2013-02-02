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
    public class EventManager : Microsoft.Xna.Framework.GameComponent {
        // fields... that are incomprehensible (fuck yeah, you go to translate.google.com!)
        #region fields

        Dictionary<int, Action> actions;
        Dictionary<int, ActionInfo> infos;
        int counter = 0;

        #endregion


        // structures... same that are above
        #region structures

        public delegate void Action ( ActionInfo actInfo );

        public class ActionInfo {
            Dictionary<string, int> m_args = new Dictionary<string, int> ();
            public ActionInfo ( string args, params int[] argValues ) {
                if ( args.Length == 0 && argValues.Length == 0 ) {
                    return;
                }

                string[] argNames = args.Split ( ' ' );
                string mode = argNames[ 0 ];
                argNames = argNames.Skip ( 1 ).ToArray ();
                int n = argNames.Length;
                if ( mode == "pairset" ) {
                    if ( argNames.Length != argValues.Length ) {
                        throw new Exception ( "WTF!? Arguments are too much or too few!" ); //Hi, mgimo
                    }

                    for ( int i = 0; i < n; i++ ) {
                        m_args.Add ( argNames[ i ], argValues[ i ] );
                    }
                } else if ( mode == "juststrings" ) {
                    for ( int i = 0; i < n; i++ ) {
                        m_args.Add ( argNames[ i ], 1 );
                    }
                }
            }

            public int this[ string argName ] {
                get {
                    if ( m_args.ContainsKey ( argName ) ) {
                        return m_args[ argName ];
                    } else {
                        return 0;
                    }
                }
                set {
                    m_args[ argName ] = value;
                }
            }
        }

        #endregion


        // XNA constructors and methods
        #region XNA

        public EventManager ( Game game ) : base ( game ) {
            actions = new Dictionary<int, Action> ();
            infos = new Dictionary<int, ActionInfo> ();
        }

        public override void Initialize () {

            base.Initialize ();
        }

        public override void Update ( GameTime gameTime ) {

            base.Update ( gameTime );
        }

        #endregion


        // methods, that provide event logic
        #region eventLogic

        public int addEvent( Action act, ActionInfo info ) {
            actions[ counter ] = act;
            infos[ counter ] = info;
            return counter++;
        }

        public bool runEvent ( int id ) {
            if ( actions.ContainsKey ( id ) ) {
                actions[ id ] ( infos[ id ] );
                return true;
            }
            return false;
        }

        #endregion
    }
}
