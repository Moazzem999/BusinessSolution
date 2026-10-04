using BusinessSolution.Dtos.Common;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Repositories.Interfaces
{
    public interface IDropdownRepository
    {
        Task<Result<List<DropdownDto>>> GetAllMaritalStatus();
        Task<Result<List<DropdownDto>>> GetAllReligion();
    }
}
