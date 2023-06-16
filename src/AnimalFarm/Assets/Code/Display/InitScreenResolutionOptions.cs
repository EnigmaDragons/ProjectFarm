using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
     
public sealed class InitScreenResolutionOptions : MonoBehaviour
{
    [SerializeField] private DisplaySettings display;
    [SerializeField] private TMP_Dropdown dropdownMenu;
    [SerializeField] private int minWidth = 800;

    private Resolution[] _resolutions;

    private void Awake()
    {
        var comparer = new ResolutionEqualityComparer();
        var current = display.GetInitializedScreenSize();
        _resolutions = Screen.resolutions
            .Where(x => x.width % 16 == 0 && x.height % 9 == 0)
            .Where(x => x.width > minWidth)
            .Distinct(comparer)
            .Reverse()
            .ToArray();
        
        dropdownMenu.options.Clear();
        for (var i = 0; i < _resolutions.Length; i++)
        {
            var valString = ResToString(_resolutions[i]);
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(valString));
            if (comparer.Equals(_resolutions[i], current))
            {
                dropdownMenu.SetValueWithoutNotify(i);
                Debug.Log($"Matching Resolution Option is {i}");
            }
        }
        dropdownMenu.RefreshShownValue();
        dropdownMenu.onValueChanged.AddListener(SetResolution);
    }

    private string ResToString(Resolution res) => res.width + " x " + res.height;
    private void SetResolution(int index) => display.SetResolution(_resolutions[index]);

    private class ResolutionEqualityComparer : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y) => x.width == y.width && x.height == y.height;
        public bool Equals(Resolution x, Vector2Int y) => x.width == y.x && x.height == y.y;

        public int GetHashCode(Resolution obj) => int.Parse($"{obj.height}{obj.width}");
    }
}
