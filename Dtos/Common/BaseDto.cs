using BusinessSolution.Shared.Enum;

namespace BusinessSolution.Dtos.Common
{
    public class BaseDto
    {
        public long Id { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public Status Status { get; set; }
    }
}
