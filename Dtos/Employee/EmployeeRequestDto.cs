using BusinessSolution.Shared.Enum;

namespace BusinessSolution.Dtos.Employee
{
    public class EmployeeRequestDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string NidNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public DateTimeOffset? DateOfBirth { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public Religion Religion { get; set; }
        public string Designation { get; set; } = string.Empty;
        public string AcademicQualification { get; set; } = string.Empty;
        public string PresentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public DateTimeOffset? JoiningDate { get; set; }
        public decimal Salary { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? NidImage { get; set; }
    }
}
