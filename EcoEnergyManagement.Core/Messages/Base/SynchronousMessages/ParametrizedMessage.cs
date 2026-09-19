using EcoEnergyManagement.Core.Abstractions;

namespace EcoEnergyManagement.Core.Messages.Base.SynchronousMessages
{
    public class ParametrizedMessage<T> : Message where T : IDefaultCheck
    {
        public T Value { get; init; }

        public ParametrizedMessage(object sender, T value) : base(sender)
        {
            Value = value;
        }

        public override bool IsDefault() => base.IsDefault() || Value is null;
    }
}
