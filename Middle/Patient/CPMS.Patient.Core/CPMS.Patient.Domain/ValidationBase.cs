using System;

namespace CPMS.Patient.Domain
{
    public abstract class ValidationBase
    {
        public event Action<Error> ValidationFailed;

        protected void OnValidationFailed(Error error)
        {
            if (ValidationFailed != null)
            {
                ValidationFailed(error);
            }
        }

    }
}
