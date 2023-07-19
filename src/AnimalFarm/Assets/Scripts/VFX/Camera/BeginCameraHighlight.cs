using UnityEngine;

public class BeginCameraHighlight
{
   public GameObject[] Targets { get; }

   public BeginCameraHighlight(params GameObject[] targets) => Targets = targets;
}
