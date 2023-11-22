using System;
using System.Collections.Generic;

namespace RPG.Utils {
    public class EventStorage {
        private readonly Dictionary<string, List<Delegate>> _eventList = new();

        public void Subscribe(string name, Action action) {
            if (!_eventList.ContainsKey(name)) _eventList[name] = new List<Delegate>();
            _eventList[name].Add(action);
        }
        
        public void Subscribe<T>(string name, Action<T> action) {
            if (!_eventList.ContainsKey(name)) _eventList[name] = new List<Delegate>();
            _eventList[name].Add(action);
        }
        
        public void Unsubscribe<T>(string eventName, Action<T> action) {
            if (!_eventList.ContainsKey(eventName)) return;
            _eventList[eventName].Remove(action);
            if (_eventList[eventName].Count == 0) _eventList.Remove(eventName);
        }

        public void Unsubscribe(string eventName, Action action) {
            if (!_eventList.ContainsKey(eventName)) return;
            _eventList[eventName].Remove(action);
            if (_eventList[eventName].Count == 0) _eventList.Remove(eventName);
        }

        public void InvokeEvent<T>(string name, T argument) {
            if (!_eventList.ContainsKey(name)) return;
            foreach (Delegate handler in _eventList[name]) {
                if(handler is Action<T> specifiedHandler)
                    specifiedHandler?.Invoke(argument);
            }
        }

        public void InvokeEvent(string name) {
            if (!_eventList.ContainsKey(name)) return;
            foreach (var handler in _eventList[name]) {
                if(handler is Action specifiedHandler)
                    specifiedHandler?.Invoke();
            }
        }
    }
}
