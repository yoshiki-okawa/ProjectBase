using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace PB.Services
{
	public class PBAssemblyResolver : DefaultAssembliesResolver
	{
		public override ICollection<Assembly> GetAssemblies()
		{
			ICollection<Assembly> baseAssemblies = base.GetAssemblies();
			List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
			if (!assemblies.Any(x => !x.IsDynamic && x.Location.ToLower() == new FileInfo("PB.WebApiControllers.dll").FullName.ToLower()))
			{
				var controllersAssembly = Assembly.LoadFrom("PB.WebApiControllers.dll");
				assemblies.Add(controllersAssembly);
			}
			if (!assemblies.Any(x => !x.IsDynamic && x.Location.ToLower() == new FileInfo("PB.Hubs.dll").FullName.ToLower()))
			{
				var controllersAssembly = Assembly.LoadFrom("PB.Hubs.dll");
				assemblies.Add(controllersAssembly);
			}
			if (!assemblies.Any(x => !x.IsDynamic && x.Location.ToLower() == new FileInfo("PB.Frameworks.dll").FullName.ToLower()))
			{
				var controllersAssembly = Assembly.LoadFrom("PB.Frameworks.dll");
				assemblies.Add(controllersAssembly);
			}
			foreach (Assembly assembly in DependencyHelper.GetPluginApiControllerAssemlies())
				assemblies.Add(assembly);

			return assemblies;
		}
	}
}
