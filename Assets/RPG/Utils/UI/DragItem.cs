﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Utils.UI {
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class {
        private Vector3 _startPosition;
        private Transform _originalParent;
        private IDragSource<T> _source;
        private Canvas _parentCanvas;

        private void Awake() {
            _parentCanvas = GetComponentInParent<Canvas>();
            _source = GetComponentInParent<IDragSource<T>>();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            _startPosition = transform.position;
            _originalParent = transform.parent;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(_parentCanvas.transform, true);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            transform.position = _startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(_originalParent, true);

            IDragDestination<T> container= !EventSystem.current.IsPointerOverGameObject() ? 
                _parentCanvas.GetComponent<IDragDestination<T>>() : GetContainer(eventData);
            if (container != null) {
                DropItemIntoContainer(container);
            }
            
        }

        private IDragDestination<T> GetContainer(PointerEventData eventData) {
            if (eventData.pointerEnter) {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
                return container;
            }
            return null;
        }

        private void DropItemIntoContainer(IDragDestination<T> destination) {
            if (object.ReferenceEquals(destination, _source)) return;
            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = _source as IDragContainer<T>;
            if (destinationContainer == null || sourceContainer == null || 
                destinationContainer.GetItem() == null || 
                object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem())) {
                AttemptSimpleTransfer(destination);
                return;
            }
            AttemptSwap(destinationContainer, sourceContainer);
        }

        private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source) {
            // Provisionally remove item from both sides. 
            var removedSourceNumber = source.GetNumber();
            var removedSourceItem = source.GetItem();
            var removedDestinationNumber = destination.GetNumber();
            var removedDestinationItem = destination.GetItem();

            source.RemoveItems(removedSourceNumber);
            destination.RemoveItems(removedDestinationNumber);

            var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
            var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);

            // Do take backs (if needed)
            if (sourceTakeBackNumber > 0) {
                source.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }
            if (destinationTakeBackNumber > 0) {
                destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Abort if we can't do a successful swap
            if (source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
                destination.MaxAcceptable(removedSourceItem) < removedSourceNumber) {
                
                destination.AddItems(removedDestinationItem, removedDestinationNumber);
                source.AddItems(removedSourceItem, removedSourceNumber);
                return;
            }

            // Do swaps
            if (removedDestinationNumber > 0) {
                source.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0) {
                destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        private bool AttemptSimpleTransfer(IDragDestination<T> destination) {
            var draggingItem = _source.GetItem();
            var draggingNumber = _source.GetNumber();

            var acceptable = destination.MaxAcceptable(draggingItem);
            var toTransfer = Mathf.Min(acceptable, draggingNumber);
            if (toTransfer > 0) {
                _source.RemoveItems(toTransfer);
                destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination) {
            var takeBackNumber = 0;
            var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

            if (destinationMaxAcceptable < removedNumber) {
                takeBackNumber = removedNumber - destinationMaxAcceptable;

                var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

                // Abort and reset
                if (sourceTakeBackAcceptable < takeBackNumber) {
                    return 0;
                }
            }
            return takeBackNumber;
        }
    }
}
