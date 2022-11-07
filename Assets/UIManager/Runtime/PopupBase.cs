using UnityEngine;
using UnityEngine.UI;
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

        public UnityEvent OnClose { get; } = new UnityEvent();

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

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }

        public override void Close()
        {
            base.Close();
            OnClose?.Invoke();
        }
    }


    class HeaderMono : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        public Transform PopupTransform { get; set; }

        Vector2 dragPosition;
        public void OnBeginDrag(PointerEventData eventData)
        {
            dragPosition = (Vector2)PopupTransform.position - eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            PopupTransform.position = dragPosition + eventData.position;
        }
    }
}