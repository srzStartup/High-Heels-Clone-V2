using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private ItemType _itemType;

    public ItemType ItemType => _itemType;

    private void Start()
    {
        switch (_itemType)
        {
            case ItemType.Heels:
                transform
                    .DOMoveY(1, 1)
                    .SetLoops(-1, LoopType.Yoyo);
                break;
            case ItemType.Gem:
                transform
                    .DORotate(new Vector3(0, 360, 0), 2, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Restart)
                    .SetEase(Ease.Linear);
                    break;
        }
    }
}

public enum ItemType
{
    Gem,
    Heels
}
