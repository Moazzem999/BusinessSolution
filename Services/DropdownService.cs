using BusinessSolution.Dtos.Common;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Services.Interfaces;
using BusinessSolution.Shared.Infrastructure;

namespace BusinessSolution.Services
{
    public class DropdownService(IDropdownRepository dropdownRepository) : IDropdownService
    {
        private readonly IDropdownRepository dropdownRepository = dropdownRepository;

        public async Task<Result<List<DropdownDto>>> GetAllMaritalStatus()
        {
            return await dropdownRepository.GetAllMaritalStatus();
        }

        public async Task<Result<List<DropdownDto>>> GetAllReligion()
        {
            return await dropdownRepository.GetAllReligion();
        }
    }
}
