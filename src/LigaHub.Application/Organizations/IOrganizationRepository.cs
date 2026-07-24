using LigaHub.Domain.Organizations;

namespace LigaHub.Application.Organizations
{
    public interface IOrganizationRepository
    {
        Task<bool> ExistsByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Organization organization,
            CancellationToken cancellationToken = default);

        Task<Organization?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Organization>> ListAsync(
            int skip,
            int take,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            CancellationToken cancellationToken = default);
    }
}
