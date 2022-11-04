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
    public abstract class PopupBase : UIBase, IPointerClickHandler
    {
        [SerializeField]
        GameObject header;

        [SerializeField]
        Button closeButton;

        private void Awake()
        {
            closeButton?.onClick.AddListener(Close);
            if (header != null)
            {
                var headerMono = header.AddComponent<HeaderMono>();
                headerMono.PopupTransform = transform;
            }
        }

        public UnityEvent<PointerEventData> OnClick { get; } = new UnityEvent<PointerEventData>();
        public UnityEvent OnClose { get; } = new UnityEvent();

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }

        public override void Close()
        {
            OnClose?.Invoke();
        }
    }


    class HeaderMono : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public Transform PopupTransform { get; set; }

        Vector2 dragPosition;
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log($"Drag {eventData.position}, {eventData.pointerDrag.name}");

            dragPosition = (Vector2)PopupTransform.position - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log($"Drag {eventData.position}, {eventData.pointerDrag.name}");
            PopupTransform.position = dragPosition + eventData.position;
        }
    }
}