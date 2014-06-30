using System;
using System.ServiceModel.Description;

namespace PB.Services
{
	public class PBOperationBehavior : Attribute, IOperationBehavior
	{
		public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
		{
			clientOperation.ParameterInspectors.Add(new PBParameterInspector());
		}

		public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
		{
			dispatchOperation.ParameterInspectors.Add(new PBParameterInspector());
		}

		public void Validate(OperationDescription operationDescription)
		{
		}
	}
}
