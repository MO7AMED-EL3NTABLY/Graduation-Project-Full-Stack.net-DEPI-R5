namespace MVC.DAL.Enums
{
    // Type of request to change the doctor's protected data that needs admin approval - DoctorProfileChangeRequests.RequestType
    public enum ProfileChangeRequestType
    {
        Specialty, // Change specialty
        LicenseNumber, // Change license number
        University, // Change university
        GraduationYear // Change graduation year
    }
}
