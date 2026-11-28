using MVC.DAL.Entities.Accounts;

namespace MVC.DAL.Entities.Chat
{
    // A message in a patient-doctor conversation - ChatMessages table
    // CHECK: it must have text, a file, or both
    // Messages are never deleted, and the BLL checks that the sender is the thread's patient or doctor
    public class ChatMessage
    {
        public Guid ChatMessageId { get; set; }

        public Guid ChatThreadId { get; set; } // FK to ChatThreads

        public string SenderUserId { get; set; } = null!; // FK to AspNetUsers - the sender

        public string? Text { get; set; } // Message text

        public string? AttachmentStorageKey { get; set; } // Attached file key

        public string? OriginalFileName { get; set; } // Original file name

        public string? ContentType { get; set; } // File type

        public long? FileSizeBytes { get; set; } // File size in bytes

        public bool IsRead { get; set; } // Read or not (no read time)

        public DateTimeOffset SentAtUtc { get; set; } // Sent time

        // Navigation properties
        public ChatThread ChatThread { get; set; } = null!;
        public ApplicationUser SenderUser { get; set; } = null!;
    }
}
