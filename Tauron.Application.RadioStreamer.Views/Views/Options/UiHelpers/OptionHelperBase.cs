using System;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.Application.RadioStreamer.Contracts.UI;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Views.Options.UiHelpers
{
    public abstract class OptionHelperBase<TData, TLast> : ObservableObject, IOptionHelper
    {
        private readonly Action<TData> _serialized;
        private readonly Action<TData> _deserialized;

        public TLast LastValue { get; private set; }

        public TData Data { get; protected set; }

        protected OptionHelperBase([CanBeNull]Action<TData> serialized, [CanBeNull]Action<TData> deserialized)
        {
            _serialized = serialized;
            _deserialized = deserialized;
        }

        public abstract object LoadUI(Option option);
        public bool Serialize(IRadioEnvironment store, Option option)
        {
            var temp = SerializeImpl(store, option);
            if (temp && _serialized != null) _serialized(Data);

            return temp;
        }

        protected abstract bool SerializeImpl(IRadioEnvironment store, Option option);

        public void Deserialize(IRadioEnvironment store, Option option, object defaultValue)
        {
            DeserializeImpl(store, option, defaultValue);
            _deserialized(Data);
        }

        protected abstract void DeserializeImpl(IRadioEnvironment store, Option option, object defaultValue);

        public abstract void Reset(Option option);
    }
}
