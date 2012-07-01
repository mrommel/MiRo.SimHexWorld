using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using MiRo.SimHexWorld.Engine.Instance;
using Microsoft.Xna.Framework;
using MiRo.SimHexWorld.Engine.UI.Controls;

namespace MiRo.SimHexWorld.Engine.UI.Dialogs
{
    public class Scores : Dictionary<int,List<float>>
    { 
    }

    public class MapScale
    {
        public enum Transformation { Linear, Log10 };

        float d_s1, d_s2;
        float d_p1, d_p2;
        float d_conv;
        Transformation d_trans;

        public MapScale(float min, float max, float mapMin, float mapMax)
        {
            d_trans = Transformation.Linear;
            d_s1 = min;
            d_s2 = max;
            d_p1 = mapMin;
            d_p2 = mapMax;

            calcFactor();
        }

        private void calcFactor()
        {
            d_conv = 0.0f;

            switch ( d_trans )
            {
                case Transformation.Linear:
                {
                    if ( d_s2 != d_s1 )
                        d_conv = (d_p2 - d_p1) / (d_s2 - d_s1);
                    break;
                }
                case Transformation.Log10:
                {
                    if ( d_s1 != 0 )
                        d_conv = (d_p2 - d_p1) / (float)Math.Log10(d_s2 / d_s1);
                    break;
                }
            }
        }
    
        public float Scale(float s)
        {
            if ( d_trans == Transformation.Linear )
                return d_p1 + ( s - d_s1 ) * d_conv;

            if (d_trans == Transformation.Log10)
                return d_p1 + (float)Math.Log10( s / d_s1 ) * d_conv;

            return 0;
        }

        public float InvScale(float p)
        {
            if (d_trans == Transformation.Log10)
                return (float)Math.Exp((p - d_p1) / (d_p2 - d_p1) * Math.Log10(d_s2 / d_s1)) * d_s1;
            else
                return d_s1 + (d_s2 - d_s1) / (d_p2 - d_p1) * (p - d_p1);
        }

        public bool IsInverting
        {
            get { return ((d_p1 < d_p2) != (d_s1 < d_s2)); }
        }

        public float MapLength
        {
            get
            {
                return d_p2 - d_p1;
            }
        }
    }

    public class ScoreDialog : AssetWindow
    {
        //ImageBox _lblGraphs;
        Scores _scores;

        //MapScale xMap, yMap;

        private ScoreDialog(Manager manager)
            : base(manager, "Content//Controls//ScoreDialog")
        {
            //Init();

            //Height = 500;
            //Width = 600;

            //InitMapScales();

            //_lblGraphs = new ImageBox(manager);
            //_lblGraphs.Init();
            //_lblGraphs.Top = 30;
            //_lblGraphs.Left = 30;
            //_lblGraphs.Width = Width - 60;
            //_lblGraphs.Height = Height - 80;
            //_lblGraphs.Draw += new DrawEventHandler(_lblGraphs_Draw);
            //Add(_lblGraphs);

            //manager.Add(this);
        }

        //private void InitMapScales()
        //{
        //    xMap = new MapScale(0, _scores.First().Value.Count, 0, Width - 50);

        //    float minY = float.MaxValue, maxY = float.MinValue;

        //    foreach( List<float> row in _scores.Values )
        //    {
        //        minY = Math.Min(row.Min(), minY );
        //        maxY = Math.Max(row.Max(), maxY );
        //    }

        //    yMap = new MapScale(minY, maxY, 0, Height - 50);
        //}

        //void _lblGraphs_Draw(object sender, DrawEventArgs e)
        //{
        //    foreach (KeyValuePair<int,List<float>> row in _scores)
        //    {
        //        AbstractPlayerData pl = MainWindow.Game.Players[row.Key];

        //        int i = 0;
        //        int lastX = -20;
        //        foreach (float val in row.Value)
        //        {
        //            int scaleX = (int)xMap.Scale(i);
        //            int scaleY = e.Rectangle.Height - (int)yMap.Scale(val) - 20;

        //            if (scaleX + 20 > lastX)
        //            {
        //                e.Renderer.Draw(pl.Civilization.Image, new Rectangle(e.Rectangle.X + scaleX - 10, e.Rectangle.Y + scaleY - 10, 20, 20), Color.White);
        //                lastX = scaleX;
        //            }

        //            i++;
        //        }
        //    }
        //}

        public static void Show(Manager manager, Scores scores, string title)
        {
            ScoreDialog scoreDialog = new ScoreDialog(manager);
            scoreDialog.Text = title;
            scoreDialog.Resizable = false;
            scoreDialog.Scores = scores;
            scoreDialog.ShowModal();
            manager.Add(scoreDialog);
        }

        public Scores Scores 
        {
            set 
            { 
                _scores = value;

                Graph graph = GetControl("Graph") as Graph;

                if (graph != null)
                {
                    foreach (KeyValuePair<int, List<float>> row in _scores)
                    {
                        AbstractPlayerData pl = MainWindow.Game.Players[row.Key];

                        if (pl.IsHuman)
                        {
                            foreach( float val in row.Value )
                                graph.AddValue(val);
                        }
                    }
                }
            }
        }

        public override List<GameNotification> NotificationInterests
        {
            get { return new List<GameNotification>(); }
        }

        public override void HandleNotification(PureMVC.Interfaces.INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
