using Elements.ConfigServer.Client.Entities;
using System.Collections.Generic;


namespace ${{ values.component_id }}.Configuration
{
	public class ConfigWrapper : TenantConfig
	{
		public (bool isValid, string errors) IsValid()
		{
			var errors = new List<string>();
			var isValid = true;

			//if (string.IsNullOrWhiteSpace(SomeSetting))
			//{
			//	AddError("SomeSetting cannot be empty");
			//}
			// TODO: Implement validation of configuration

			return (isValid, string.Join(';', errors));

			void AddError(string msg)
			{
				errors.Add(msg);
				isValid = false;
			}
		}
	}
}
