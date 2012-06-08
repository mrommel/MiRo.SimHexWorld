using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MiRo.SimHexWorld.Engine.Types
{
//    public class PlayerColor
//    {
        

//        public PlayerColor(Color primary, Color secondary, Color text)
//        {
//            Primary = primary;
//            Secondary = secondary;
//            Text = text;
//        }
//    }

    //public static PlayerColor[] PlayerColors = 
    //    {
    //        new PlayerColor( Color.Black, Color.White, Color.Black ),
    //        new PlayerColor( Color.Blue, Color.White, Color.Blue ),
    //        new PlayerColor( Color.Brown, Color.Khaki, Color.Brown ),
    //        new PlayerColor( Color.Cyan, Color.Black, Color.Cyan ),
    //        new PlayerColor( Color.DarkBlue, Color.Yellow, Color.DarkBlue ),

    //        new PlayerColor( Color.DarkCyan, Color.White, Color.DarkCyan ),
    //        new PlayerColor( Color.DarkGreen, Color.Yellow, Color.DarkGreen ),
    //        new PlayerColor( Color.Pink, Color.Yellow, Color.Pink ),
    //        new PlayerColor( Color.Purple, Color.Yellow, Color.Purple ),
    //        new PlayerColor( Color.DarkRed, Color.Yellow, Color.DarkRed ),

    //        new PlayerColor( Color.Yellow, Color.DarkRed, Color.Yellow ),
    //        new PlayerColor( Color.Gray, Color.Black, Color.Gray ),
    //        new PlayerColor( Color.Green, Color.Black, Color.Green ),
    //        new PlayerColor( Color.Orange, Color.White, Color.Orange ),
    //        new PlayerColor( Color.PeachPuff, Color.Black, Color.PeachPuff ),

    //        new PlayerColor( Color.Pink, Color.DarkRed, Color.Pink ),
    //        new PlayerColor( Color.Purple, Color.Black, Color.Purple ),
    //        new PlayerColor( Color.Red, Color.White, Color.Red ),
    //        new PlayerColor( Color.White, Color.Red, Color.White ),
    //        new PlayerColor( Color.Yellow, Color.DarkBlue, Color.Yellow ),

    //        new PlayerColor( Color.LightGreen, Color.DarkBlue, Color.LightGreen ),
    //        new PlayerColor( Color.LightBlue, Color.Black, Color.LightBlue ),
    //        new PlayerColor( Color.LightYellow, Color.Black, Color.LightYellow ),
    //        new PlayerColor( Color.LightPink, Color.Black, Color.LightPink ),
    //        new PlayerColor( Color.Orange, Color.DarkGreen, Color.Orange ),
    //    };

    public class PlayerColor
    {
        public string Name { get; set; }
        public Color Primary { get; set; }
        public Color Secondary { get; set; }

        [ContentSerializer(Optional = true)]
        public Color Text { get; set; }
    }
}
