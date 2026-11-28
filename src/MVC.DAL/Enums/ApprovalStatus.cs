namespace MVC.DAL.Enums
{
    // Admin review status - used in DoctorProfiles, DoctorDocuments and DoctorProfileChangeRequests
    public enum ApprovalStatus
    {
        Pending, // Waiting for admin review
        Approved, // Admin approved
        Rejected // Admin rejected
    }
}
