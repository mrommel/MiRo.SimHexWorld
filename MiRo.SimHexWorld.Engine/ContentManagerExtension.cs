using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace MiRo.SimHexWorld.Engine
{
    public static class ContentManagerExtension
    {
        /// <summary>
        /// Load all content within a certain folder. The function
        /// returns a dictionary where the file name, without type
        /// extension, is the key and the texture object is the value.
        ///
        /// The contentFolder parameter has to be relative to the
        /// game.Content.RootDirectory folder.
        /// </summary>
        /// <typeparam name="T">The content type.</typeparam>
        /// <param name="contentManager">The content manager for which content is to be loaded.</param>
        /// <param name="contentFolder">The game project root folder relative folder path.</param>
        /// <returns>A list of loaded content objects.</returns>
        public static Dictionary<String, T> LoadContent<T>(this ContentManager contentManager, string contentFolder, string filter = "*.*", bool includeSubfolders = false)
        {
            //Load directory info, abort if none
            var dir = new DirectoryInfo(Path.Combine(contentManager.RootDirectory, contentFolder));
            if (!dir.Exists)
                throw new DirectoryNotFoundException();

            //Init the resulting list
            var result = new Dictionary<String, T>();

            LoadFolder<T>(contentManager, contentFolder, filter, dir, result);

            if (includeSubfolders)
            {
                foreach (DirectoryInfo dirInfo in dir.GetDirectories())
                    LoadFolder(contentManager, Path.Combine(contentFolder, dirInfo.Name), filter, dirInfo, result);
            }

            //Return the result
            return result;
        }

        private static void LoadFolder<T>(ContentManager contentManager, string contentFolder, string filter, DirectoryInfo dir, Dictionary<string, T> result)
        {
            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles(filter);
            foreach (FileInfo file in files)
            {
                try
                {
                    string key = Path.GetFileNameWithoutExtension(file.Name);

                    if (key != null)
                        result[key] = contentManager.Load<T>(Path.Combine(contentManager.RootDirectory, contentFolder, key));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error during loading of: " + file.Name);
                }
            }
        }

        /// <summary>
        /// Helper for loading a .xnb asset which can have multiple localized
        /// versions for different countries. This allows you localize data such
        /// as textures, models, and sound effects.
        /// 
        /// This uses a simple naming convention. If you have a default asset named
        /// "Foo", you can provide a specialized French version by calling it
        /// "Foo.fr", and a Japanese version called "Foo.ja". You can specialize even
        /// further by country as well as language, so if you wanted different assets
        /// for the United States vs. United Kingdom, you would add "Foo.en-US" and
        /// "Foo.en-GB".
        /// 
        /// This function looks first for the most specialized version of the asset,
        /// which includes both language and country. If that does not exist, it looks
        /// for a version that only specifies the language. If that still does not
        /// exist, it falls back to the original non-localized asset name.
        /// </summary>
        public static T LoadLocalizedAsset<T>(this ContentManager contentManager, string assetName)
        {
            return contentManager.LoadLocalizedAsset<T>(assetName, CultureInfo.CurrentCulture);
        }

        public static T LoadLocalizedAsset<T>(this ContentManager contentManager, string assetName, CultureInfo ci)
        {
            string[] cultureNames =
            {
                ci.Name,                        // eg. "en-US"
                ci.TwoLetterISOLanguageName     // eg. "en"
            };

            // Look first for a specialized language-country version of the asset,
            // then if that fails, loop back around to see if we can find one that
            // specifies just the language without the country part.
            foreach (string cultureName in cultureNames)
            {
                string localizedAssetName = assetName + '.' + cultureName;

                try
                {
                    return contentManager.Load<T>(localizedAssetName);
                }
                catch (ContentLoadException) { }
            }

            // If we didn't find any localized asset, fall back to the default name.
            return contentManager.Load<T>(assetName);
        }
    }
}
