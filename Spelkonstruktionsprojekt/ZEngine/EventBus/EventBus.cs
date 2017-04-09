﻿using System;

namespace ZEngine.EventBus
{
    public class EventBus : IEventBus
    {
        public static EventBus Instance => LazyInitializer.Value;
        private static readonly Lazy<EventBus> LazyInitializer = new Lazy<EventBus>(() => new EventBus());

        private static ParameterLessEventBus LocalEventBus = new ParameterLessEventBus();
        private static TypedEventBus LocalTypedEventBus = new TypedEventBus();

        public void Publish(string message)
        {
            LocalEventBus.Publish(message);
        }

        public void Publish<T>(string message, T value)
        {
            LocalTypedEventBus.Publish<T>(message, value);
        }

        public void Subscribe(string message, Action callback)
        {
            LocalEventBus.Subscribe(message, callback);
        }

        public void Subscribe<T>(string message, Action<T> callback)
        {
            LocalTypedEventBus.Subscribe<T>(message, callback);
        }

        public void Unsubscribe(string message, Action callback)
        {
            LocalEventBus.Unsubscribe(message, callback);
        }

        public void Unsubscribe<T>(string message, Action<T> callback)
        {
            LocalTypedEventBus.Unsubscribe<T>(message, callback);
        }
    }
}