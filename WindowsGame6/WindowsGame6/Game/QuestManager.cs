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
            foreach ( Quest q in quests.Values ) {
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
        }

        #endregion


        #region fields

        Dictionary< string, Quest > quests = new Dictionary<string, Quest> ();

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

        public void addQuest ( string name, Quest qst ) {
            quests.Add ( name, qst );
        }

        public void setPassed ( string name ) {
            if ( quests.ContainsKey ( name ) ) {
                quests[ name ].state = Quest.State.passed;
            }
        }

        public void setActive ( string name ) {
            if ( quests.ContainsKey ( name ) ) {
                quests[ name ].state = Quest.State.active;
                foreach ( var q in quests ) {
                    if ( q.Key != name && q.Value.state == Quest.State.active ) {
                        q.Value.state = Quest.State.deactivated;
                    }
                }
            }
        }

        #endregion
    }
}
