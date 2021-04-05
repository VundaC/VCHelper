using VCHelper.Validation.Exceptions;

namespace VCHelper.Validation
{
	public abstract class BaseValidation : IValidation
	{
		private BaseValidation _nextValidation;

		protected abstract string _reason { get; }
		protected string _externalReason;

		public void SetReason(string reason)
		{
			_externalReason = reason;
		}

		public void AddValidation(BaseValidation validation)
		{
			if (_nextValidation == null)
				_nextValidation = validation;
			else
				_nextValidation.AddValidation(validation);
		}

		public void InservValidatioin(BaseValidation validation)
		{
			if (validation == null) return;
			validation.AddValidation(_nextValidation);
			_nextValidation = validation;
		}

		public void Validate(string s)
		{
			InternalValidate(s);
			_nextValidation?.Validate(s);
		}

		protected abstract void InternalValidate(string s);

		protected virtual void ThrowValidate()
		{
			var reason = string.IsNullOrWhiteSpace(_externalReason) ? _reason : _externalReason;
			throw new ValidateException(this, reason);
		}
	}
}
