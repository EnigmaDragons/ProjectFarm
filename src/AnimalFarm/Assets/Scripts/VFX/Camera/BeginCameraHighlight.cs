using UnityEngine;

public class BeginCameraHighlight
{
   public Vector3 AdditionalOffset { get; }
   public GameObject[] Targets { get; }

   public BeginCameraHighlight(Vector3 additionalOffset, params GameObject[] targets)
   {
      AdditionalOffset = additionalOffset;
      Targets = targets;
   }
}
