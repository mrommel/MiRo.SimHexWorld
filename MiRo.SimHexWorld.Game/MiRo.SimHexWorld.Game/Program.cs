using System;
using MiRo.SimHexWorld.Engine.Misc;
using MiRo.SimHexWorld.Engine.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;
using System.Globalization;
using System.Xml;
using MiRo.SimHexWorld.Engine.World.Maps;

namespace MiRo.SimHexWorld.Game
{
#if WINDOWS || XBOX
    static class Program
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Load from App.Config file
            log4net.Config.XmlConfigurator.Configure(); 
            log.Info("Startup");

            try
            {
                if( MainApplication.Instance != null )
                {
                    MainApplication.Instance.Run();

                    MainApplication.Instance.Dispose();
                }
            }
            catch (NoSuitableGraphicsDeviceException)
            {
                MessageBox.Show("Pixel and vertex shaders 2.0 or greater are required.",
                    "MiRo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OutOfVideoMemoryException)
            {
                //GameSettings.SetMinimumGraphics();

                MessageBox.Show("Insufficent video memory.\n\n" +
                    "The graphics settings have been reconfigured to the minimum. " +
                    "Please restart the application. \n\nIf you continue to receive " +
                    "this error message, your system may not meet the " +
                    "minimum requirements.  \n\nCheck documentation for minimum requirements.",
                    "MiRo",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "MiRo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            log.Info("Exit");
        }

    }
#endif
}

