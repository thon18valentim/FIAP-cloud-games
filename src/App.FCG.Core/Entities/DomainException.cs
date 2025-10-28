namespace FCG.Core.Entities
{
    /// <summary>
    /// Extreme cases, only when trying to create an entity and it is in an invalid state.
    /// If it gets here, it means we failed to validate the DTO.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message = "An error occurred while creating the entity") : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
