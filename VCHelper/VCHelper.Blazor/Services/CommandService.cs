using System;
using System.Collections.Generic;
using VCHelper.Blazor.Helpers;

namespace VCHelper.Blazor.Services
{
	public class CommandService : ICommandService
	{
		#region Fields

		private readonly Dictionary<string, Command> _voidCommands
			= new Dictionary<string, Command>();

		private readonly Dictionary<string, Command<object>> _resultCommands
			= new Dictionary<string, Command<object>>();

		#endregion

		#region Methods

		public void ExecuteCommand(string name, params object[] args)
		{
			if (_voidCommands.ContainsKey(name))
				_voidCommands[name].Execute(args);
		}

		public T ExecuteCommand<T>(string cmdName, params object[] args)
		{
			if (_resultCommands.ContainsKey(cmdName))
			{
				var res = _resultCommands[cmdName].Execute(args);
				return (T)res;
			}

			return default;
		}

		public void SetExecution(string cmdName, Action<object[]> action)
		{
			if (!_voidCommands.ContainsKey(cmdName))
				_voidCommands.Add(cmdName, new Command());

			_voidCommands[cmdName].SetExecution(action);
		}

		public void SetExecution<T>(string cmdName, Func<object[], T> func)
		{
			if (!_resultCommands.ContainsKey(cmdName))
				_resultCommands.Add(cmdName, new Command<object>());

			_resultCommands[cmdName].SetExecution((object[] param) => func(param));
		}

		#endregion
	}
}
