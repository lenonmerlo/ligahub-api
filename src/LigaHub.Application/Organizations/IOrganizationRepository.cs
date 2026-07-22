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
    }
}
