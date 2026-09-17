using SistemaPresenca.Domain.Models;

namespace SistemaPresenca.Domain.Errors;

public static class MajorErrors
{
    public static Error NotFound => new(
        "MajorErrors.NotFound",
        "Major with provided identifier not found.");

    public static Error CodeAlreadyExists => new(
        "MajorErrors.CodeAlreadyExists",
        "Major with the same code already exists.");

    public static Error InvalidUpdateRequest(string description) => new(
        "MajorErrors.InvalidUpdateRequest",
        description);
}
