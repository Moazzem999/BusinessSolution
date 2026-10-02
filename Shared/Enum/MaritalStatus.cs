using System.ComponentModel;

namespace BusinessSolution.Shared.Enum
{
    public enum MaritalStatus
    {
        [Description("Single")]
        Single = 1,
        [Description("Married")]
        Married = 2,
        [Description("Divorced")]
        Divorced = 3
    }
}
