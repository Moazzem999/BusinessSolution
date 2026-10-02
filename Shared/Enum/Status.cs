using System.ComponentModel;

namespace BusinessSolution.Shared.Enum
{
    public enum Status
    {
        [Description("Active")]
        Active = 1,
        [Description("Deleted")]
        Deleted = 2,
        [Description("Permanent Delete")]
        PermanentDelete = 3
    }
}
