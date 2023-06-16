using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeleportingPiece : MonoBehaviour
{
    [SerializeField] private LockBoolVariable gameInputActive;
    [SerializeField] private FloatReference secondsToTravel;
    [SerializeField] private CurrentLevelMap map;

    [SerializeField] private Renderer renderer;
    [SerializeField] private ParticleSystem rendererParticles;
    [SerializeField] private Renderer goToPosition;

    private Renderer _renderer;
    private Renderer _goToPosition;
    private bool _teleporting = false;
    private PieceMoved _msg;
    private float _t;
    private List<Texture2D> _activeTextures;

    private void Execute(UndoPieceMoved msg)
    {
        if (msg.Piece == gameObject)
        {
            transform.localPosition = new Vector3(msg.From.X, msg.From.Y, transform.localPosition.z);
            Message.Publish(new PieceDeselected());
            Message.Publish(new PieceSelected(gameObject));
        }
    }

    private void Execute(PieceMoved msg)
    {
        if (msg.Piece == gameObject)
        {
            _teleporting = true;
            _msg = msg;
            gameInputActive.Lock(gameObject);
            _t = 0;
            _goToPosition.transform.localPosition = new Vector3(msg.Delta.X + 0.5f, msg.Delta.Y + 0.5f, _goToPosition.transform.localPosition.z);
            _goToPosition.gameObject.SetActive(true);
            rendererParticles.transform.localPosition = _renderer.transform.localPosition;
            rendererParticles.transform.rotation = Quaternion.LookRotation(_goToPosition.transform.position - _renderer.transform.position);
            rendererParticles.Play();
        }
    }

    private void Update()
    {
        if (_teleporting)
        {
            _t = Mathf.Min(1, _t + Time.deltaTime / secondsToTravel);
            UpdateTextures(Generate(255, Mathf.Max(0, _t - 0.5f) * 2, Mathf.Min(1, _t * 2)), Generate(255, Mathf.Max(0, (1 - _t) - 0.5f) * 2, Mathf.Min(1, (1 - _t) * 2)));
            if (_t == 1)
            {
                var tempPosition = _renderer.transform.position;
                transform.localPosition = new Vector3(_msg.To.X, _msg.To.Y, transform.localPosition.z);
                rendererParticles.transform.position = tempPosition;
                _goToPosition.gameObject.transform.localPosition = _renderer.gameObject.transform.localPosition;
                var temp = _renderer;
                _renderer = _goToPosition;
                _goToPosition = temp;
                UpdateTextures(Generate(1, 0, 0), Generate(1, 1, 1));
                _goToPosition.gameObject.SetActive(false);
                map.Move(_msg.Piece, _msg.From, _msg.To);
                gameInputActive.Unlock(gameObject);
                _teleporting = false;
            }
        }
    }

    private void UpdateTextures(Texture2D main, Texture2D goTo)
    {
        _renderer.material.SetTexture("_DisplacementMask", main);
        _goToPosition.material.SetTexture("_DisplacementMask", goTo);
        _activeTextures.ForEach(Destroy);
        _activeTextures = new List<Texture2D> { main, goTo };
    }

    private Texture2D Generate(int size, float whitePercentage, float blackPercentage)
    {
        var texture = new Texture2D(1, size);
        var whitePixels = (int)(size * whitePercentage);
        var blackPixels = (int)(size * (1 - blackPercentage));
        var graidentPixels = size - whitePixels - blackPixels;
        texture.SetPixels(Enumerable.Range(0, size).Select(i =>
        {
            if (i < whitePixels)
                return Color.white;
            if (i >= whitePixels + graidentPixels)
                return Color.black;
            float spotInGradient = i - whitePixels;
            var percentage = 1 - spotInGradient / graidentPixels;
            return new Color(percentage, percentage, percentage, 1);
        }).ToArray());
        texture.Apply();
        return texture;
    }

    private void OnEnable()
    {
        _teleporting = false;
        _activeTextures = new List<Texture2D>();
        _renderer = renderer;
        _goToPosition = goToPosition;
        Message.Subscribe<PieceMoved>(Execute, this);
        Message.Subscribe<UndoPieceMoved>(Execute, this);
    }

    private void OnDisable()
    {
        gameInputActive.Unlock(gameObject);
        Message.Unsubscribe(this);
    }
}
