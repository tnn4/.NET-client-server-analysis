using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Content
{
    /// <summary>
    /// Loads all assets from content folder
    /// </summary>
    internal class ContentLoader
    {
        ContentManager _content;

        public void Load()
        {

        }
    }

    internal static class ContentLoaderStatic
    {
        public static Dictionary<String, T> LoadContent<T>(this ContentManager contentManager, string contentFolder)
        {
            //Load directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            
            //Init the resulting list
            Dictionary<String, T> result = new Dictionary<String, T>();

            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                result[key] = contentManager.Load<T>(contentManager.RootDirectory + "/" + contentFolder + "/" + key);
            }
            //Return the result
            return result;
        }
    }

}
