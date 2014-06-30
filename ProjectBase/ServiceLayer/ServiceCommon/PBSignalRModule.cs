using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PB.Services
{
	public class PBSignalRModule : HubPipelineModule
	{
		public override Func<IHubIncomingInvokerContext, Task<object>> BuildIncoming(Func<IHubIncomingInvokerContext, Task<object>> invoke)
		{
			return async ctx =>
			{
				foreach (object obj in ctx.Args)
				{
					if (TypeDescriptor.GetProvider(obj) == null)
					{
						Type t = obj.GetType();
						TypeDescriptor.AddProvider(new AssociatedMetadataTypeTypeDescriptionProvider(t, t), t);
					}

					ValidationContext validationCtx = new ValidationContext(ctx.Args[0], null, null);
					List<ValidationResult> results = new List<ValidationResult>();

					bool valid = Validator.TryValidateObject(ctx.Args[0], validationCtx, results, true);
					if (!valid)
						return results.Select(x => x.ErrorMessage).ToArray();
				}
				try
				{
					return await invoke(ctx);
				}
				catch (Exception e)
				{
					return new[] { e.Message };
				}
			};
		}
	}
}
