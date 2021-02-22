using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BuildingToolButton : MonoBehaviour, IPointerEnterHandler
{
    public Button button;
    public Image image;
    public UnityEvent OnPointerEnter;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnter.Invoke();
    }
}
