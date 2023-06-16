using UnityEngine;

public class MoveTreeAnalysisControl : MonoBehaviour
{
    [SerializeField] private BoolReference _developmentIsActive;
    [SerializeField] private CurrentLevelMap _map;

    private bool _calculating;

    private void Update()
    {
        if (_developmentIsActive.Value && !_calculating && Input.GetKey(KeyCode.T) && Input.GetKey(KeyCode.E))
        {
            _calculating = true;
            var result = new MoveTreeAnalysis().CalculateMoveTree(_map.GetSnapshot());
            Debug.Log($"CalculatingComplete - 1-Star: {result.HasOneStar} 2-Star: {result.HasTwoStar} 3-Star: {result.HasThreeStar}. " +
                      $"Winning {result.NumberOfWinningBranches}. Dead {result.NumberOfDeadBranches}");
        }
    }
}
