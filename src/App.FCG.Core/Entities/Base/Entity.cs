using FluentValidation.Results;
using System.Text.Json.Serialization;

namespace FCG.Core.Entities.Base
{
	public abstract class Entity
	{
        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }

        public Guid Id { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;
        public DateTime CreatedAt { get; protected set; }
		public DateTime? UpdatedAt { get; protected set; }
		public DateTime? DeletedAt { get; protected set; }

        protected Entity()
		{
			Id = Guid.NewGuid();
			CreatedAt = DateTime.UtcNow;
		}

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }

        #region Comparações
        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (ReferenceEquals(null, compareTo)) return false;

            return Id.Equals(compareTo.Id);
        }

        public override string ToString()
        {
            return $"{GetType().Name} [Id={Id}]";
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() * 907 + Id.GetHashCode();
        }
        #endregion
    }
}
