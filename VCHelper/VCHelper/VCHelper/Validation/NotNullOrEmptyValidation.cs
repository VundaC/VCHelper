namespace VCHelper.Validation
{
	public class NotNullOrEmptyValidation : BaseValidation
	{
		protected override string _reason => "Value cannot be empty";

		protected override void InternalValidate(string s)
		{
			if (string.IsNullOrWhiteSpace(s)) ThrowValidate();
		}
	}
}
