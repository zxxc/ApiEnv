using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject1
{
	[TestClass]
	public class UnitTest1
	{
		private const string EnvKey = "EnvTest";
		private readonly string[] _possibleValues =
		{
			"TestSettings",
			"dev",
			"dev.local",
			"prod"
		};

		[TestMethod]
		public void TestMethod1()
		{
			var envTestValue = Environment.GetEnvironmentVariable(EnvKey);
			Console.WriteLine(EnvKey + ": " + envTestValue);
			Assert.IsTrue(_possibleValues.Contains(envTestValue));
		}
	}
}
