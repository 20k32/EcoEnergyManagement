using EcoEnergyManagement.Core.Abstractions;

namespace EcoEnergyManagement.Core.Messages.Base.AsynchronousMessages
{
    public class ParametrizedAsyncMessage<T>(object sender, T value) : AsyncMessage(sender), IDefaultCheck where T : IDefaultCheck
    {
        T Value { get; init; } = value;
        public bool IsDefault() => Sender is null || Value.IsDefault();
    }
}
