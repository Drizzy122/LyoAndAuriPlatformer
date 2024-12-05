using UnityEditor;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;

public static class SetUp {

    [MenuItem("Tools/Setup/Create Default Folders")]
    public static void CreateDefaultFolders(){
        Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "Scene", "Scripts","FBX","Shader",
            "Settings");
        Refresh();
    }

    [MenuItem("Tools/Setup/import My Favorite Assets")]
    public static void ImportMyFavoriteAssets()
    {
        Assets.ImportAssets("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
    }
    static class Folders {
        public static void CreateDefault(string root, params string[] folders)
        {
            var fullpath = Combine(Application.dataPath, root);
            foreach (var folder in folders)
            {
                var path = Combine(fullpath, folder);
                if (!Exists(path))
                {
                    CreateDirectory(path);
                }
            }
        }
    }

    public static class Assets
    {
        public static void ImportAssets(string asset, string subfolder, string folder = "C:/Users/yourb/AppData/Roaming/Unity/Asset Store-5.x")
        {
            AssetDatabase.ImportPackage(Combine(folder, subfolder, asset), false);
        }
    }
}
