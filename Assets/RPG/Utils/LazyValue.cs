using System;

namespace RPG.Utils {
    public class LazyValue<T>
    {
        private T _value;
        private bool _initialized;
        private InitializerDelegate _initializer;

        public delegate T InitializerDelegate();

        public Action<T, T> OnValueChanged;

        public LazyValue(InitializerDelegate initializer) {
            _initializer = initializer;
        }

        public T Value {
            get {
                ForceInit();
                return _value;
            }
            set {
                _initialized = true;
                var oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(oldValue, _value);
            }
        }

        public void ForceInit() {
            if (!_initialized) {
                var oldValue = _value;
                _value = _initializer();
                OnValueChanged?.Invoke(oldValue, _value);
                _initialized = true;
            }
        }
    }
}
