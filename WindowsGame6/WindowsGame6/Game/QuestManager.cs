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
    public class QuestManager : Microsoft.Xna.Framework.GameComponent {
        #region debug

        public string getQuestList () {
            string res = "";

            return res;
        }

        public string getCurrentQuest() {
            string res = "";
            foreach ( Quest q in quests ) {
                if ( q.state == Quest.State.active ) {
                    res += q.title + "\r\n\t" + q.objective;
                    break;
                }
            }

            return res;
        }

        #endregion


        #region structures

        public class Quest {

            public enum State {
                active,
                deactivated,
                passed,
                unobtained
            };

            public string title;
            public string objective;
            public State state;

            public void setState ( EventManager.EventArgs info ) {
                foreach ( State st in Enum.GetValues ( typeof ( State ) ) ) {
                    if ( info[ "state" ] == st.GetHashCode() ) {
                        state = st;
                        break;
                    } else if ( info[ "setstate" ]  == st.GetHashCode () ) { // fucking crutches!!!
                        state = st;
                        break;
                    }
                }
            }
        }

        #endregion


        #region fields

        List< Quest > quests = new List< Quest > ();

        #endregion


        #region XNA

        public QuestManager ( Game game ) : base ( game ) {
        }

        public override void Initialize () {
            Game1.dbgDrawer.addDebugOutputToDraw ( "Quests", "Active Quest", getCurrentQuest );

            base.Initialize ();
        }

        public override void Update ( GameTime gameTime ) {
            base.Update ( gameTime );
        }

        #endregion


        #region logic

        public void addQuest ( Quest qst ) {
            quests.Add ( qst );
        }

        public void goToNextQuest ( EventManager.EventArgs e ) {
            for ( int i = 0; i < quests.Count; i++ ) {
                if ( quests[ i ].state == Quest.State.active ) {
                    quests[ i ].state = Quest.State.deactivated;
                    if ( ++i < quests.Count ) {
                        quests[ i ].state = Quest.State.deactivated;
                    }
                    break;
                }
            }
        }

        #endregion
    }
}
