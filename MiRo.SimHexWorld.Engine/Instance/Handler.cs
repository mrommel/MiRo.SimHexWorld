using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiRo.SimHexWorld.Engine.Instance
{
    public class TurnEventArgs : EventArgs
    {
        GameYear _oldYear, _newYear;

        public TurnEventArgs(GameYear oldYear, GameYear newYear)
        {
            _oldYear = oldYear;
            _newYear = newYear;
        }

        public GameYear OldYear
        {
            get
            {
                return _oldYear;
            }
        }

        public GameYear NewYear
        {
            get
            {
                return _newYear;
            }
        }
    }

    public delegate void TurnHandler(GameData game, TurnEventArgs args);

    public class PlayerChangedEventArgs : EventArgs
    {
        AbstractPlayerData _oldPlayer, _newPlayer;

        public PlayerChangedEventArgs(AbstractPlayerData oldPlayer, AbstractPlayerData newPlayer)
        {
            _oldPlayer = oldPlayer;
            _newPlayer = newPlayer;
        }

        public AbstractPlayerData OldPlayer
        {
            get
            {
                return _oldPlayer;
            }
        }

        public AbstractPlayerData NewPlayer
        {
            get
            {
                return _newPlayer;
            }
        }
    }

    public delegate void PlayerChangedHandler(GameData game, PlayerChangedEventArgs args);

}
