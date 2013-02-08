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
        int counter = 0;

        #endregion


        // structures... same that are above
        #region structures
        public delegate void Event ();
        #endregion


        // XNA constructors and methods
        #region XNA

        public EventManager ( Game game ) : base ( game ) {
            events = new Dictionary<int, Event> ();
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

        public int newEvent ( Event act ) {
            events[ counter ] = act;
            return counter++;
        }

        public bool runEvent ( int id ) {
            if ( events.ContainsKey ( id ) ) {
                events[ id ] ();
                return true;
            }
            return false;
        }

        public void attachToEvent ( int id, Event act ) {
            if ( events.ContainsKey ( id ) ) {
                events[ id ] += act;
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
