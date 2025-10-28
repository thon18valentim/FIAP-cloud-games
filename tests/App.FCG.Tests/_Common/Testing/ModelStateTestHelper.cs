using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Tests._Common.Testing;

public static class ModelStateTestHelper
{
    /// <summary>
    /// Valida DataAnnotations e popula ModelState (simula [ApiController]).
    /// </summary>
    public static void ValidateAndPopulateModelState(ControllerBase controller, object model)
    {
        var ctx = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);

        foreach (var vr in results)
        {
            var memberName = vr.MemberNames.FirstOrDefault() ?? string.Empty;
            controller.ModelState.AddModelError(memberName, vr.ErrorMessage ?? "Invalid");
        }
    }
}
