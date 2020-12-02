using System;

namespace VCHelper.Blazor.Services
{
	public interface ICommandService
	{
		void ExecuteCommand(string name, params object[] args);
		T ExecuteCommand<T>(string cmdName, params object[] args);
		void SetExecution(string cmdName, Action<object[]> action);
		void SetExecution<T>(string cmdName, Func<object[], T> func);
	}
}
