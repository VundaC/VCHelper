using System;
using System.Collections.Generic;
using VCHelper.Blazor.Helpers;

namespace VCHelper.Blazor.Services
{
	public class CommandService : ICommandService
	{
		#region Fields

		private readonly Dictionary<string, Command<object>> _commands
			= new Dictionary<string, Command<object>>();

		#endregion

		#region Methods

		public void ExecuteCommand(string name, params object[] args)
		{
			if (_commands.ContainsKey(name))
				_commands[name].Execute(args);
		}

		public T ExecuteCommand<T>(string cmdName, params object[] args)
		{
			if (_commands.ContainsKey(cmdName))
			{
				var res = _commands[cmdName].Execute(args);
				return (T)res;
			}

			return default;
		}

		public void SetExecution(string cmdName, Action<object[]> action)
		{
			if (!_commands.ContainsKey(cmdName))
				_commands.Add(cmdName, new Command<object>());

			_commands[cmdName].SetExecution(x =>
			{
				action.Invoke(x);
				return null;
			});
		}

		public void SetExecution<T>(string cmdName, Func<object[], T> func)
		{
			if (!_commands.ContainsKey(cmdName))
				_commands.Add(cmdName, new Command<object>());

			_commands[cmdName].SetExecution((object[] param) => func(param));
		}

		#endregion
	}
}
