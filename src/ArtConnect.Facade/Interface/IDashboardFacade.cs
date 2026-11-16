using ArtConnect.Facade.Models;

namespace ArtConnect.Facade.Interface
{
    public interface IDashboardFacade
    {
        Task<DashboardSummary> GetSummaryAsync();
    }
}
