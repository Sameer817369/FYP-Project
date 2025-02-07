namespace RMS.Domain.Models
{
    public class ErrorViewModels
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

