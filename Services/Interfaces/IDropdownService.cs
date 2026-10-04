using BusinessSolution.Dtos.Common;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services.Interfaces
{
    public interface IDropdownService
    {
        Task<Result<List<DropdownDto>>> GetAllMaritalStatus();
        Task<Result<List<DropdownDto>>> GetAllReligion();
    }
}
