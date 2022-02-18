using System;
using System.Reflection;

namespace ${{ values.component_id }}.Web
{
	public class VersionInfo
	{
		public VersionInfo(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			var version = assembly.GetName().Version;
			Version = version.Revision > 0 ? version.ToString() : version.ToString(fieldCount: 3);

			if (Attribute.GetCustomAttribute(assembly,
					typeof(AssemblyInformationalVersionAttribute)) is AssemblyInformationalVersionAttribute
				informationalVersion)
			{
				InformationalVersion = informationalVersion.InformationalVersion;
			}
		}

		public string Version { get; }

		public string InformationalVersion { get; }
	}
}
