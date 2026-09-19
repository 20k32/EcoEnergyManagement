using CommunityToolkit.Mvvm.Messaging.Messages;
using EcoEnergyManagement.Core.Abstractions;

namespace EcoEnergyManagement.Core.Messages.Base.AsynchronousMessages
{
    public class ParametrizedAsyncMessageReturnValue<TParameter, TResult>(object sender, TParameter value) : AsyncRequestMessage<TResult>, IDefaultCheck where TParameter : IDefaultCheck
    {
        TParameter Value { get; init; } = value;
        public object Sender { get; init; } = sender;

        public bool IsDefault() => Sender is null || Value.IsDefault();
    }
}
