using System;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using File = UnityEngine.Windows.File;
#endif

[CreateAssetMenu]
public sealed class CurrentMapTokenizer : ScriptableObject
{
    private LevelMapBuilder _builder = new LevelMapBuilder("Uninitialized");

    public void Init(string name)
    {
        _builder = new LevelMapBuilder(name);
    }

    public void RegisterAsMapPiece(GameObject obj, MapPiece piece) => _builder = _builder.With(new TilePoint(obj), piece);
    public string Token => new TokenizedLevelMap(_builder.Build()).ToString();
    
    #if UNITY_EDITOR
    public void ExportToFile()
    {
        var path = Path.Combine(Application.dataPath, $"{Guid.NewGuid().ToString()}");
        var levelString = new TokenizedLevelMap(_builder.Build()).ToString();
        File.WriteAllBytes(path, Encoding.UTF8.GetBytes(levelString));
        Debug.Log($"Wrote Level to {path}");
    }
    #endif
}
