
namespace TeamMashup.Core.Domain
{
    public interface IDeletableEntity
    {
        bool Deleted { get; }

        bool TryDelete(out string errorKey);
    }
}