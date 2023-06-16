using System.Linq;
using UnityEngine;

public class SetPlayerSurvey : OnMessage<LevelCompleted>
{
    [SerializeField] private StringReference question;
    [SerializeField] private StringReference[] answers;
    [SerializeField] private PlayerSurvey playerSurvey;
    [SerializeField] private SaveStorage storage;
    [SerializeField] private CurrentLevel level;
    [SerializeField] private BoolReference shouldSurvey;

    private bool _hasCompletedLevel;
    private void Start() => _hasCompletedLevel = storage.GetStars(level.ActiveLevel) > 0;

    protected override void Execute(LevelCompleted msg)
    {
        if (_hasCompletedLevel || !shouldSurvey.Value)
            return;
        playerSurvey.HasSurvey = true;
        playerSurvey.Question = question.Value;
        playerSurvey.Answers = answers.Select(x => x.Value).ToList();
    }
}
