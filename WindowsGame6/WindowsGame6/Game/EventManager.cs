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

        public 
        Dictionary<int, Event> events;
        Dictionary<int, EventArgs> args;
        int counter = 0;

        #endregion


        // structures... same that are above
        #region structures
        public delegate void Event ( EventArgs actInfo );

        public class EventArgs {
            Dictionary<string, int> m_args = new Dictionary<string, int> ();

            public EventArgs () {
                return;
            }

            public EventArgs ( string args ) {
                string[] argNames = args.Split ( ' ' );
                for ( int i = 0; i < argNames.Length; i++ ) {
                    m_args.Add ( argNames[ i ], 1 );
                }
            }

            public EventArgs ( string args, params int[] argValues ) {
                string[] argNames = args.Split ( ' ' );
                if ( argNames.Length != argValues.Length ) {
                    throw new Exception ( "WTF!? Arguments are too much or too few!" ); //Hi, mgimo
                }

                for ( int i = 0; i < argNames.Length; i++ ) {
                    m_args.Add ( argNames[ i ], argValues[ i ] );
                }
            }

            public void attachArgs ( EventArgs args ) {
                foreach ( var arg in args.m_args ) {
                    if ( m_args.ContainsKey ( arg.Key ) ) {
                        throw new Exception ( "EventManager.EventArgs.attachArgs: argument name collizion!" );
                    }
                    m_args.Add ( arg.Key, arg.Value );
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
            events = new Dictionary<int, Event> ();
            args = new Dictionary<int, EventArgs> ();
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

        public int newEvent( Event act, EventArgs info ) {
            events[ counter ] = act;
            args[ counter ] = info;
            return counter++;
        }

        public bool runEvent ( int id ) {
            if ( events.ContainsKey ( id ) ) {
                events[ id ] ( args[ id ] );
                return true;
            }
            return false;
        }

        public void attachToEvent ( int id, Event act, EventArgs args ) {
            if ( events.ContainsKey ( id ) ) {
                events[ id ] += act;
                this.args[ id ].attachArgs ( args );
            } else {
                if ( id < counter ) {
                    events[ id ] = act;
                } else {
                    throw new Exception ( "EventManager.addToEvent: event is not initialized!" );
                }
            }
        }

        public void deleteEvent ( int id ) {
            events.Remove ( id );
            args.Remove ( id );
        }
        
        // needs to rewrite becouse could be bags if we deattached from removed event but after added event back
        // end it's not deattaching from EventArgs =(
        public void deattachFromEvent ( int id, Event act ) {
            if ( events.ContainsKey ( id ) ) {
                events[ id ] -= act;
            }
        }

        #endregion
    }
}
