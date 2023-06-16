using System;
using UnityEngine;

[Serializable]
public sealed class DeliveryTimeTutorialSegment
{
    [SerializeField] public TilePoint Location;
    [SerializeField, TextArea(2, 10)] public string Text;

    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(Text))
            Message.Publish(new DismissNotification());
        else
            Message.Publish(new ShowNotification(Text));
    }
}
