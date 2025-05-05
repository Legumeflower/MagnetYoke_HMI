using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Core
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void SetField<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (property != null)
            {
                if (property.Equals(value))
                {
                    return;
                }
            }

            property = value;
            OnPropertyChanged(propertyName);
        }

        private readonly Dictionary<string, object> CDicField = new Dictionary<string, object>();

        public T GetF<T>([CallerMemberName] string propertyName = null)
        {
            object propertyValue;
            if (!CDicField.TryGetValue(propertyName, out propertyValue))
            {
                propertyValue = default(T);
                CDicField.Add(propertyName, propertyValue);
            }

            return (T)propertyValue;
        }

        public void SetF(object value, [CallerMemberName] string propertyName = null)
        {
            if (!CDicField.ContainsKey(propertyName) || CDicField[propertyName] != (object)value)
            {
                CDicField[propertyName] = value;
                OnPropertyChanged(propertyName);
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            // 也可以寫成 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
