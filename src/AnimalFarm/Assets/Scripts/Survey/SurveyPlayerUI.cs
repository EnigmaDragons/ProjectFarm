using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class SurveyPlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI question;
    [SerializeField] private GameObject answerParent;
    [SerializeField] private SurveyAnswerButton answerButtonPrefab;
    [SerializeField] private Button skipButton;
    [SerializeField] private PlayerSurvey playerSurvey;
    [SerializeField] private GameObject thanksObject;
    [SerializeField] private Navigator navigator;
    [SerializeField] private CurrentLevel level;
    [SerializeField] private CurrentZone zone;

    private void Start()
    {
        question.text = playerSurvey.Question;
        playerSurvey.Answers.ForEach(x =>
        {
            var button = Instantiate(answerButtonPrefab, answerParent.transform);
            button.Init(x, () =>
                {
                    Analytics.CustomEvent(playerSurvey.Question, new Dictionary<string, object>
                    {
                        { "answer", x },
                        { "zoneName", zone.Zone.Name },
                        { "zoneNum", level.ZoneNumber },
                        { "levelName", level.ActiveLevel.Name },
                        { "levelNumber", level.LevelNumber }
                    });
                    answerParent.SetActive(false);
                    skipButton.gameObject.SetActive(false);
                    thanksObject.SetActive(true);
                    playerSurvey.HasSurvey = false;
                    StartCoroutine(DelayedLeave());
                });
        });
        skipButton.onClick.AddListener(SkipSurvey);
    }

    public void SkipSurvey()
    {
        playerSurvey.HasSurvey = false;
        navigator.NavigateToRewards();
    }

    private IEnumerator DelayedLeave()
    {
        yield return new WaitForSeconds(0.5f);
        navigator.NavigateToRewards();
    }
}
