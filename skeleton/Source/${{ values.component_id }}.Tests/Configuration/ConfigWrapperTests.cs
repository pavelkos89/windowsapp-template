using ${{ values.component_id }}.Configuration;
using Xunit;

namespace ${{ values.component_id }}.Tests.Configuration
{
	public class ConfigWrapperTests
	{
		[Fact]
		public void IsValid_ShouldReturnTrue_ForValidConfiguration()
		{
			var config = GetValidConfig();

			var (isValid, errors) = config.IsValid();
			Assert.True(isValid);
			Assert.True(string.IsNullOrEmpty(errors));
		}

		#region Helpers
		private static ConfigWrapper GetValidConfig()
		{
			return new ConfigWrapper();
		}
		#endregion
	}
}
