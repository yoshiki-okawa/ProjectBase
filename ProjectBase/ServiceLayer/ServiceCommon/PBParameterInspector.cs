using System.ServiceModel.Dispatcher;

namespace PB.Services
{
	public class PBParameterInspector : IParameterInspector
	{

		public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
		{
		}

		public object BeforeCall(string operationName, object[] inputs)
		{
			DependencyHelper.RegisterAllDependencies();
			return null;
		}
	}
}