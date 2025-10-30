namespace Laborator4_AI.Domain.Operations
{
    using Laborator4_AI.Domain.Models.Entities;

    /// <summary>
    /// Base class for StudentRegistration operations using Transform pattern
    /// </summary>
    internal abstract class StudentRegistrationOperation
    {
        internal IStudentRegistration Transform(IStudentRegistration registration)
        {
            return registration switch
            {
                UnvalidatedStudentRegistration unvalidated => OnUnvalidated(unvalidated),
                ValidatedStudentRegistration validated => OnValidated(validated),
                CheckedStudentRegistration checkedReg => OnChecked(checkedReg),
                RegisteredStudentRegistration registered => OnRegistered(registered),
                InvalidStudentRegistration invalid => OnInvalid(invalid),
                _ => throw new InvalidOperationException($"Unknown student registration state: {registration.GetType().Name}")
            };
        }

        protected virtual IStudentRegistration OnUnvalidated(UnvalidatedStudentRegistration registration) => registration;
        protected virtual IStudentRegistration OnValidated(ValidatedStudentRegistration registration) => registration;
        protected virtual IStudentRegistration OnChecked(CheckedStudentRegistration registration) => registration;
        protected virtual IStudentRegistration OnRegistered(RegisteredStudentRegistration registration) => registration;
        protected virtual IStudentRegistration OnInvalid(InvalidStudentRegistration registration) => registration;
    }
}
