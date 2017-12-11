using System;

namespace CPMS.Domain
{
    public abstract class ValidationBase
    {
        public event Action<RuleViolation> ValidationFailed;

        protected void OnValidationFailed(RuleViolation ruleViolation)
        {
            if (ValidationFailed != null)
            {
                ValidationFailed(ruleViolation);
            }
        }

    }
}
