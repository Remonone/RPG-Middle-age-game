using System;

namespace RPG.Utils {
    public class EventStorageFacade {
        private EventStorage _storage;

        public EventStorageFacade(EventStorage storage) {
            _storage = storage;
        }
        
        public void Subscribe(string eventName, Action action) => _storage.Subscribe(eventName, action);
        public void Subscribe<T>(string eventName, Action<T> action) => _storage.Subscribe(eventName, action);
        
        public void Unsubscribe(string eventName, Action action) => _storage.Unsubscribe(eventName, action);
        public void Unsubscribe<T>(string eventName, Action<T> action) => _storage.Unsubscribe(eventName, action);

    }
}
