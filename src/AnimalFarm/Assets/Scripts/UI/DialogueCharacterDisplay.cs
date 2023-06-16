using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCharacterDisplay : MonoBehaviour
{
    [SerializeField] private GameObject[] extraVisualComponenets;
    [SerializeField] private Image image;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 leftPosition;
    [SerializeField] private Vector3 leftTabletPosition;
    [SerializeField] private Vector3 centerPosition;
    [SerializeField] private Vector3 rightPosition;
    [SerializeField] private Vector3 rightTabletPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float secondsToMove;
    [SerializeField] private GameObject staticVfx;
    [SerializeField] private LayoutMode layout;

    private float _t;
    private Vector3 _source;
    private Vector3 _destination;
    private bool _leaving;

    public Character Character { get; set; }

    public void Init(Character character, bool isLeftMostCharacter)
    {
        Character = character;
        image.sprite = character.Bust;
        SetFacing(isLeftMostCharacter);

        staticVfx.SetActive(character.UseStatic);
        transform.localPosition = startPosition;
    }

    private void SetFacing(bool isLeftMostCharacter)
    {
        if (!isLeftMostCharacter) return;
        
        var t = image.transform.localScale;
        image.transform.localScale = new Vector3(-t.x, t.y, t.z);
    }

    public void SetFocus(bool focused)
    {
        if (focused)
        {
            extraVisualComponenets.ForEach(x => x.SetActive(true));
            image.color = Color.white;
        }
        else if (layout.IsWide)
        {
            image.color = Color.gray;
        }
        else
        {
            image.color = new Color(1, 1, 1, 0);
            extraVisualComponenets.ForEach(x => x.SetActive(false));
        }
    }

    public void GoTo(DialogueDirection direction, bool isTeleporting)
    {
        _t = isTeleporting ? 1 : 0;
        _source = transform.localPosition;
        if (direction == DialogueDirection.Center || layout.IsTall)
            _destination = centerPosition;
        else if (direction == DialogueDirection.Left)
            _destination = layout.AspectRatio == ResolutionAspectRatio.FourByThree ? leftTabletPosition : leftPosition;
        else if (direction == DialogueDirection.Right)
            _destination = layout.AspectRatio == ResolutionAspectRatio.FourByThree ? rightTabletPosition : rightPosition;
    }

    public void Leave()
    {
        _t = 0;
        _source = transform.localPosition;
        _destination = endPosition;
        _leaving = true;
    }

    private void Update()
    {
        _t = Math.Min(1, _t += Time.deltaTime / secondsToMove);
        transform.localPosition = Vector3.Lerp(_source, _destination, _t);
        if (_leaving && _t == 1)
            Destroy(gameObject);
    }
}
