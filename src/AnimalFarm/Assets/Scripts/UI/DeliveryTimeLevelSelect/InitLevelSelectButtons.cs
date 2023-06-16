using UnityEngine;

public sealed class InitLevelSelectButtons : MonoBehaviour
{
    [SerializeField] private GameLevels zone;
    [SerializeField] private DeliveryTime2Navigator navigator;
    [SerializeField] private CurrentLevel level;
    [SerializeField] private TextCommandButton buttonPrototype;
    [SerializeField] private GameObject parent;

    void Awake()
    {
        foreach(Transform t in parent.transform)
            Destroy(t.gameObject);
        for (var i = 0; i < zone.Value.Length; i++)
        {
            var currentIndex = i;
            Instantiate(buttonPrototype, parent.transform).Init($"Level {i + 1}", () => 
            { 
                level.SelectLevel(zone.Value[currentIndex], 0, currentIndex);
                navigator.NavigateToGameScene();
            });
        }
    }
}
