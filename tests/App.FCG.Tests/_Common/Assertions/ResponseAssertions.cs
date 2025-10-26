using FluentAssertions;

namespace FCG.Tests._Common.Assertions;

public static class ResponseAssertions
{
    public static void ShouldFailWith(this (bool ok, string? error) r, string contains)
    {
        r.ok.Should().BeFalse();
        r.error.Should().NotBeNullOrWhiteSpace();
        r.error!.Should().Contain(contains);
    }

    public static void ShouldSucceed(this (bool ok, string? error) r)
        => r.ok.Should().BeTrue();
}
