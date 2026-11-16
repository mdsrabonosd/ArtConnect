using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class MessageFacade : IMessageFacade
    {
        private readonly IMessageRepository _repository;

        public MessageFacade(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Message>> GetMessagesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> AddMessageAsync(Message item)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _repository.AddAsync(item);
            return OperationResult.Ok("Message created.");
        }

        public async Task<OperationResult> UpdateMessageAsync(Message item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var updated = await _repository.UpdateAsync(item);
            return updated
                ? OperationResult.Ok("Message updated.")
                : OperationResult.Fail("Message not found.");
        }

        public async Task<OperationResult> DeleteMessageAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted
                ? OperationResult.Ok("Message deleted.")
                : OperationResult.Fail("Message not found.");
        }
    }
}
