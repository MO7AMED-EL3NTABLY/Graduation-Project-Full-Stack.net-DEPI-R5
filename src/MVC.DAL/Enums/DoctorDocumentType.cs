namespace MVC.DAL.Enums
{
    // Type of document the doctor uploads - DoctorDocuments.DocumentType
    // The doctor uploads only these two documents, and the admin reviews both (Other was removed by team decision)
    public enum DoctorDocumentType
    {
        License, // Medical license (reviewed)
        GraduationCertificate // Graduation certificate (reviewed)
    }
}
