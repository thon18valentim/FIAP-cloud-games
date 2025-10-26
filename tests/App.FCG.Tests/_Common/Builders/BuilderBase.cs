namespace FCG.Tests._Common.Builders;

/// <summary>Base minimalista para Builders, mantendo SRP e reutilização.</summary>
public abstract class BuilderBase<TBuilder, TDto>
    where TBuilder : BuilderBase<TBuilder, TDto>
{
    protected abstract TDto BuildInternal();

    public TDto Build() => BuildInternal();

    /// <summary>Atalho para clonar e modificar poucos campos.</summary>
    public TDto With(Action<TBuilder> mutate)
    {
        var self = (TBuilder)this;
        mutate(self);
        return BuildInternal();
    }
}
