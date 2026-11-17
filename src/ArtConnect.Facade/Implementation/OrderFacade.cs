using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;

namespace ArtConnect.Facade.Implementation
{
    public class OrderFacade : IOrderFacade
    {
        private readonly IOrderRepository _repository;
        private readonly IArtistProfileRepository _artistRepository;
        private readonly IArtistProposalRepository _proposalRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderFacade(
            IOrderRepository repository,
            IArtistProfileRepository artistRepository,
            IArtistProposalRepository proposalRepository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _artistRepository = artistRepository;
            _proposalRepository = proposalRepository;
            _userManager = userManager;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _repository.GetAllWithDetailsAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _repository.GetByIdWithDetailsAsync(id);
        }

        public async Task<OperationResult> AddOrderAsync(Order item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var error = await ValidateAsync(item, isNew: true);
            if (error != null)
                return OperationResult.Fail(error);

            // Simulated platform commission: 10% when the admin leaves it at 0.
            if (item.CommissionAmount == 0)
                item.CommissionAmount = Math.Round(item.TotalAmount * 0.10m, 2);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Order created.");
        }

        public async Task<OperationResult> UpdateOrderAsync(Order item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var existing = await _repository.GetByIdAsync(item.Id);
            if (existing == null)
                return OperationResult.Fail("Order not found.");


            var error = await ValidateAsync(item, isNew: false);
            if (error != null)
                return OperationResult.Fail(error);

            existing.CustomerId = item.CustomerId;
            existing.ArtistProfileId = item.ArtistProfileId;
            existing.ProposalId = item.ProposalId;
            existing.OrderType = item.OrderType;
            existing.TotalAmount = item.TotalAmount;
            existing.CommissionAmount = item.CommissionAmount == 0
                ? Math.Round(item.TotalAmount * 0.10m, 2)
                : item.CommissionAmount;
            existing.Status = item.Status;
            existing.OrderDate = item.OrderDate;

            await _repository.UpdateAsync(existing);
            return OperationResult.Ok("Order updated.");
        }

        public async Task<OperationResult> DeleteOrderAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return OperationResult.Fail("Order not found.");

            if (existing.Status == OrderStatus.Delivered)
                return OperationResult.Fail("A delivered order cannot be deleted.");

            await _repository.DeleteAsync(id);
            return OperationResult.Ok("Order deleted.");
        }

        private async Task<string?> ValidateAsync(Order item, bool isNew)
        {
            if (item.TotalAmount <= 0)
                return "Total amount must be greater than zero.";

            if (item.CommissionAmount < 0 || item.CommissionAmount > item.TotalAmount)
                return "Commission cannot be negative or more than the total amount.";

            var customer = await _userManager.FindByIdAsync(item.CustomerId.ToString());
            if (customer == null || customer.IsDeleted)
                return "Please select a valid customer.";

            if (await _artistRepository.GetByIdAsync(item.ArtistProfileId) == null)
                return "Please select a valid artist.";

            if (item.ProposalId.HasValue)
            {
                if (await _proposalRepository.GetByIdAsync(item.ProposalId.Value) == null)
                    return "The selected proposal was not found.";

                if (await _repository.AnyAsync(o => o.ProposalId == item.ProposalId && o.Id != item.Id))
                    return "An order already exists for this proposal.";
            }

            return null;
        }
    }
}
