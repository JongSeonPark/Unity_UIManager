using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ChickenGames.UI
{
    public abstract class PopupBase : UIBase, IPointerClickHandler, IDragHandler, IBeginDragHandler
    {
        [SerializeField]
        GameObject header;

        [SerializeField]
        Button closeButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(() => OnClose?.Invoke());
        }

        public UnityEvent<PointerEventData> OnClick { get; } = new UnityEvent<PointerEventData>();
        public UnityEvent OnClose { get; } = new UnityEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }

        Vector2 dragPosition;
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log($"Drag {eventData.position}, {eventData.pointerDrag.name}");

            dragPosition = (Vector2)transform.position - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log($"Drag {eventData.position}, {eventData.pointerDrag.name}");
            transform.position = dragPosition + eventData.position;
            
        }

    }

}