using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int minSpawns;
    [SerializeField] private int maxSpawns;
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField] private List<Renderer> toAvoidSpawningIn;


    public void Start()
    {
        for (var i = 0; i < Rng.Int(minSpawns, maxSpawns + 1); i++)
        {
            var spawn = Instantiate(prefabs.Random(), transform);
            spawn.transform.localPosition = GetRandomValidLocation();
        }
    }

    private Vector3 GetRandomValidLocation()
    {
        for (var i = 0; i < 9999; i++)
        {
            var location = new Vector3(Rng.Int((int)minBounds.x, (int)maxBounds.x), Rng.Int((int)minBounds.y, (int)maxBounds.y), Rng.Int((int)minBounds.z, (int)maxBounds.z));
            if (toAvoidSpawningIn.All(x => !x.bounds.Contains(location)))
                return location;
        }
        return Vector3.zero;
    }
}
