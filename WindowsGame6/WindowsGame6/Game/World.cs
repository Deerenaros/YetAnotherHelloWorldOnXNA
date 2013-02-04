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
    public class World : Microsoft.Xna.Framework.DrawableGameComponent {
        // debug information methods and variables
        #region debug

        public string getHeroDbg () {
            return "(" + hero.heroTile.x + "; " + hero.heroTile.y + ")" + 
                       ( hero.heroTile != hero.targetTile
                                       ? " -> (" + hero.targetTile.x + "; " + hero.targetTile.y + ")"
                                       : "" );
        }


        public string isPause () {
            return pause.ToString ();
        }
        #endregion


        // structures that provides logic
        #region structures

        public class Tile {
            Dictionary<Type, Texture2D> tileTextures = new Dictionary<Type,Texture2D>();
            Type[] unPahtableTypes = { Type.stone };

            public int evnt = -1;

            public enum Type {
                grass,
                ground,
                stone
            };

            public const int Size = 32;
            public Type type;
            public int x = 0, y = 0;

            public Tile () {
                type = Type.grass;
                x = 0;
                y = 0;
            }

            public Tile ( Type t, int _x, int _y ) {
                type = t;
                x = _x;
                y = _y;
            }

            public void LoadContent ( ContentManager cnt ) {
                tileTextures[ Type.grass ] = cnt.Load<Texture2D> ( "Textures/grass" );
                tileTextures[ Type.ground ] = cnt.Load<Texture2D> ( "Textures/ground" );
                tileTextures[ Type.stone ] = cnt.Load<Texture2D> ( "Textures/stone" );
            }

            public void Draw ( SpriteBatch spriteBatch, Vector2 shift ) {
                Vector2 pos = new Vector2 ( x*Tile.Size, y*Tile.Size );
                if ( tileTextures.ContainsKey( type ) ) {
                    spriteBatch.Draw ( tileTextures[ type ], pos + shift, Color.White );
                }
            }

            public bool isPathable () {
                return ! unPahtableTypes.ToList<Type> ().Contains ( type );
            }
        }


        public class TileSet {
            Tile[,] tiles;
            public int sizeX, sizeY;

            public TileSet ( int x, int y ) {
                sizeX = x;
                sizeY = y;
                tiles = new Tile[ x, y ];
                for ( int i = 0; i < x; i++ ) {
                    for ( int j = 0; j < y; j++ ) {
                        tiles[ i, j ] = new Tile ( Tile.Type.grass, i, j );
                    }
                }
            }

            public Tile this[ int x, int y ] {
                get {
                    return tiles[ x, y ];
                } set {
                    tiles[ x, y ] = value;
                }
            }

            public void setEventToTile( int x, int y, int evId ) {
                tiles[ x, y ].evnt = evId;
            }
        }


        public class Hero {
            Texture2D heroTex;
            string heroName;
            Vector2 animShift = new Vector2(0.0f, 0.0f);

            public Tile targetTile;
            public Tile heroTile;

            public Hero ( Tile t, string name ) {
                heroTile = targetTile = t;
                heroName = name;
            }

            public void LoadContent ( ContentManager cnt ) {
                heroTex = cnt.Load<Texture2D> ( "HeroSprites/" + heroName );
            }

            public void Draw ( SpriteBatch spriteBatch, Vector2 shift ) {
                Vector2 pos = new Vector2 ( heroTile.x*Tile.Size + shift.X + animShift.X,
                                            heroTile.y*Tile.Size + shift.Y + animShift.Y);
                spriteBatch.Draw ( heroTex, pos, Color.White );
            }

            public void Update () {
                if ( targetTile != heroTile ) {
                    Vector2 speedVector = new Vector2 ( targetTile.x - heroTile.x,
                                                        targetTile.y - heroTile.y );
                    //speedVector *= Tile.Size;
                    Vector2 vec = speedVector*Tile.Size;
                    //speedVector.Normalize ();
                    //speedVector /= Tile.

                    animShift += speedVector;
                    if ( animShift.LengthSquared () >  vec.LengthSquared () ) {
                        animShift = new Vector2( 0.0f );
                        Game1.events.runEvent ( ( heroTile = targetTile ).evnt ); //очень быдло код =)
                    }
                }
            }

            public void moveToTile ( Tile t ) {
                if ( targetTile == heroTile ) {
                    targetTile = t;
                }
            }
        }

        #endregion


        // fields (variables)
        #region fields

        Hero hero;
        TileSet tiles;
        SpriteBatch spriteBatch;
        ContentManager content;
        InputManager input;
        bool pause = false;
        Dictionary< InputManager.GameButtons, int > keyBinds = new Dictionary<InputManager.GameButtons, int> ();

        #endregion


        // making world from ... anything (костыли)
        #region makers

        public void makeTileSetFromString ( string tileLegend, params string[] chTiles ) {
            string[] tileLegends = tileLegend.Split ( ' ' );
            Dictionary<char, Tile.Type> tileDict = new Dictionary<char, Tile.Type> ();

            int? x = null, y = null;

            foreach ( string tLegend in tileLegends ) {
                string[] l = tLegend.Split ( ':' );
                switch ( l[ 0 ] ) {
                    case "sizex":
                        x = int.Parse( l[ 1 ] );
                        break;
                    case "sizey":
                        y = int.Parse( l[ 1 ] );
                        break;
                    default:
                        bool finded = false;
                        foreach ( Tile.Type type in Enum.GetValues ( typeof ( Tile.Type ) ) ) {
                            if ( l[ 0 ] == type.ToString () ) {
                                tileDict[ l[ 1 ][ 0 ] ] = type;
                                finded = true;
                                break;
                            }
                        }
                        if ( !finded ) {
                            throw new Exception ( "Game1.World.makeTileSetFromString(...): I see it's not legend enought..." );
                        }
                        break;
                }
            }

            if ( x.HasValue && y.HasValue ) {
                tiles = new TileSet ( x.Value, y.Value );
            } else {
                throw new Exception ( "Game1.World.makeTileSetFromString(...): Legend does not contain a size of tileset..." );
            }

            for ( int j = 0; j < y; j++ ) {
                for ( int i = 0; i < x; i++ ) {
                    tiles[ i, j ] = new Tile ( tileDict[ chTiles[ j ][ i ] ], i, j );
                }
            }
        }

        #endregion


        // keyboard inits and other methods
        #region keyboard



        #endregion


        // xna inits, updates and drawers
        #region XNAInitsAndEvents

        public World ( Game game )
            : base ( game ) {
            input = new InputManager ( game );
            content = new ContentManager ( game.Services );
            content.RootDirectory = "Content";

            tiles = new TileSet ( 5, 5 );

            for ( int i = 0; i < tiles.sizeX; i++ ) {
                for ( int j = 0; j < tiles.sizeY; j++ ) {
                    tiles[ i, j ] = new Tile ( Tile.Type.grass, i, j );
                }
            }

            hero = new Hero ( tiles[ 0, 0 ], "hero" );
        }

        public override void Initialize () {
            int id = Game1.events.newEvent ( windHero, new EventManager.EventArgs ( "x y", 12, 5 ) );
            tiles.setEventToTile ( 5, 5, id );

            QuestManager.Quest q = new QuestManager.Quest();
            q.objective = "Go into the temple.";
            q.title = "Begining";
            q.state = QuestManager.Quest.State.active;
            Game1.quests.addQuest ( q );
            QuestManager qsts = Game1.quests;
            Game1.events.attachToEvent ( id, qsts.goToNextQuest, new EventManager.EventArgs () );

            q = new QuestManager.Quest ();
            q.objective = "Find the exit. If u can... or die =/";
            q.title = "Ups";
            q.state = QuestManager.Quest.State.unobtained;
            Game1.quests.addQuest ( q );
            Game1.events.attachToEvent ( id, q.setState, new EventManager.EventArgs ( "state", QuestManager.Quest.State.active.GetHashCode () ) );

            Game1.dbgDrawer.addDebugOutputToDraw ( "World", "Hero", getHeroDbg );
            Game1.dbgDrawer.addDebugOutputToDraw ( "World", "Pause", isPause );

            base.Initialize ();
        }

        protected override void LoadContent () {
            spriteBatch = new SpriteBatch ( GraphicsDevice );

            hero.LoadContent ( content );
            for ( int i = 0; i < tiles.sizeX; i++ ) {
                for ( int j = 0; j < tiles.sizeY; j++ ) {
                    tiles[ i, j ].LoadContent ( content );
                }
            }
            base.LoadContent ();
        }

        public override void Update ( GameTime gameTime ) {
            if ( Game1.inputs.pressedButtons.ContainsKey ( InputManager.GameButtons.pause ) ) {
                if ( !Game1.inputs.pressedButtons[ InputManager.GameButtons.pause ].processed ) {
                    toogleFreezeWoorld ();
                }
            }
            updateHero ();
            hero.Update ();
            base.Update ( gameTime );
        }

        protected override void UnloadContent () {
            base.UnloadContent ();
        }

        public override void Draw ( GameTime gameTime ) {
            Vector2 shift = new Vector2 ( GraphicsDevice.Viewport.Width/2  - Tile.Size/2 - hero.heroTile.x*Tile.Size,
                                          GraphicsDevice.Viewport.Height/2 - Tile.Size/2 - hero.heroTile.y*Tile.Size );

            spriteBatch.Begin ();
            for ( int i = 0; i < tiles.sizeX; i++ ) {
                for ( int j = 0; j < tiles.sizeY; j++ ) {
                    tiles[ i, j ].Draw ( spriteBatch, shift );
                }
            }
            hero.Draw ( spriteBatch, shift );
            spriteBatch.End ();

            base.Draw ( gameTime );
        }

        #endregion


        // all logic events (keyboard or other)
        #region events
        
        public void updateHero () {
            if ( !pause ) {
                if ( Game1.inputs.pressedButtons.ContainsKey ( InputManager.GameButtons.down ) ) {
                    if ( hero.heroTile.y < tiles.sizeY - 1 ) {
                        if ( tiles[ hero.heroTile.x, hero.heroTile.y + 1 ].isPathable () ) {
                            hero.moveToTile ( tiles[ hero.heroTile.x, hero.heroTile.y + 1 ] );
                        }
                    }
                } else if ( Game1.inputs.pressedButtons.ContainsKey ( InputManager.GameButtons.up ) ) {
                    if ( hero.heroTile.y > 0 ) {
                        if ( tiles[ hero.heroTile.x, hero.heroTile.y - 1 ].isPathable () ) {
                            hero.moveToTile ( tiles[ hero.heroTile.x, hero.heroTile.y - 1 ] );
                        }
                    }
                } else if ( Game1.inputs.pressedButtons.ContainsKey ( InputManager.GameButtons.left ) ) {
                    if ( hero.heroTile.x > 0 ) {
                        if ( tiles[ hero.heroTile.x - 1, hero.heroTile.y ].isPathable () ) {
                            hero.moveToTile ( tiles[ hero.heroTile.x - 1, hero.heroTile.y ] );
                        }
                    }
                } else if ( Game1.inputs.pressedButtons.ContainsKey ( InputManager.GameButtons.right ) ) {
                    if ( hero.heroTile.x < tiles.sizeX - 1 ) {
                        if ( tiles[ hero.heroTile.x + 1, hero.heroTile.y ].isPathable () ) {
                            hero.moveToTile ( tiles[ hero.heroTile.x + 1, hero.heroTile.y ] );
                        }
                    }
                }
            }
        }

        public void windHero ( EventManager.EventArgs actInfo ) {
            if ( 0 <= actInfo[ "x" ] && actInfo[ "x" ] < tiles.sizeX &&
                 0 <= actInfo[ "y" ] && actInfo[ "y" ] < tiles.sizeY ) {
                hero.moveToTile ( tiles[ actInfo[ "x" ], actInfo[ "y" ] ] );
            } else {
                throw new Exception ( "World.windHero(...): Error event processing!" );
            }
        }

        public void toogleFreezeWoorld ( ) {
            if ( pause ) {
                pause = false;
            } else {
                pause = true;
            }
        }

        #endregion
    }
}