namespace ArtConnect.Framework
{
    /// <summary>
    /// Simple result returned by facade methods so the UI can show a clear message.
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public static OperationResult Ok(string message = "") =>
            new OperationResult { Success = true, Message = message };

        public static OperationResult Fail(string message) =>
            new OperationResult { Success = false, Message = message };
    }
}
