using BusinessSolution.Dtos.Common;
using BusinessSolution.Repositories.Interfaces;
using BusinessSolution.Shared.Enum;
using BusinessSolution.Shared.Infrastructure;
using BusinessSolution.Shared.Utilities;

namespace BusinessSolution.Repositories
{
    public class DropdownRepository : IDropdownRepository
    {
        public async Task<Result<List<DropdownDto>>> GetAllMaritalStatus()
        {
            var data = Enum.GetValues<MaritalStatus>()
                .Select(x => new DropdownDto
                {
                    Id = (int)x,
                    Name = x.GetEnumDescription()
                })
                .ToList();

            return await Result<List<DropdownDto>>.SuccessAsync("", data);
        }

        public async Task<Result<List<DropdownDto>>> GetAllReligion()
        {
            var data = Enum.GetValues<Religion>()
                .Select(x => new DropdownDto
                {
                    Id = (int)x,
                    Name = x.GetEnumDescription()
                })
                .ToList();

            return await Result<List<DropdownDto>>.SuccessAsync("", data);
        }
    }
}
