using System;
using System.Collections.ObjectModel;

namespace Apartment.App.Common
{
    /// <remarks>Оригинальный <see cref="ObservableCollection{T}"/> не уведомляет об удалённых из коллекции айтемах, поэтому была создан этот класс.</remarks>
    public class FixedObservableCollection<T> : ObservableCollection<T>
    {
        public event EventHandler<EventArgs> Clearing;
        protected virtual void OnClearing(EventArgs e) => Clearing?.Invoke(this, e);

        protected override void ClearItems()
        {
            OnClearing(EventArgs.Empty);
            base.ClearItems();
        }
    }
}