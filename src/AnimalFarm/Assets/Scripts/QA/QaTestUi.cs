using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class QaTestUi : OnMessage<QaTestStarted, QaTestCompleted>
{
    [SerializeField] private Image color;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private TextMeshProUGUI report;
    [SerializeField] private bool shouldLogErrors;

    private List<string> _issues = new List<string>();
    private int _numPassed = 0;
    private int _numFailed = 0;

    private int _pending;

    private void Awake()
    {
        report.text = "Testing...";
    }
    
    protected override void Execute(QaTestStarted msg)
    {
        _pending++;
    }

    protected override void Execute(QaTestCompleted msg)
    {
        _pending--;
        if (msg.Passed)
            _numPassed++;
        else
            _numFailed++;
        _issues.AddRange(msg.Issues);
        UpdateReport();
    }

    private void UpdateReport()
    {
        if (_pending > 0)
            report.text = $"Running {_pending} tests";
        else
        {
            color.color = _numFailed > 0 ? failColor : successColor;
            var sb = new StringBuilder();
            sb.AppendLine($"{_numPassed + _numFailed} QA Tests Completed");
            sb.AppendLine($"{_numPassed} Succeeded, {_numFailed} Failed");
            sb.AppendLine($"{_issues.Count} Issues Found");
            sb.AppendLine("--------------------------------------------");
            _issues.ForEach(i => sb.AppendLine(i));
            report.text = sb.ToString();
            if (shouldLogErrors)
                _issues.ForEach(Debug.LogError);
        }
    }
}
