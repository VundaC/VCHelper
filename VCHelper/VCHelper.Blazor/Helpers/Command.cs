using System;

namespace VCHelper.Blazor.Helpers
{
	public class Command
	{
		private Action<object[]> _action;

		public void Execute(params object[] args)
		{
			_action?.Invoke(args);
		}

		public void SetExecution(Action<object[]> action)
		{
			_action = action;
		}
	}

	public class Command<T>
	{
		private Func<object[], T> _func;

		public T Execute(params object[] args)
		{
			if (_func == null)
				return default;

			return _func.Invoke(args);
		}

		public void SetExecution(Func<object[], T> func)
		{
			_func = func;
		}
	}
}
