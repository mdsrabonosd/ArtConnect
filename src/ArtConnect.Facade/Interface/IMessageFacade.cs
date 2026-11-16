using ArtConnect.Entities;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Interface
{
    public interface IMessageFacade
    {
        Task<List<Message>> GetMessagesAsync();
        Task<Message?> GetMessageByIdAsync(int id);
        Task<OperationResult> AddMessageAsync(Message item);
        Task<OperationResult> UpdateMessageAsync(Message item);
        Task<OperationResult> DeleteMessageAsync(int id);
    }
}
