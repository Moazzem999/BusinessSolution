using System.ComponentModel;

namespace BusinessSolution.Shared.Enum
{
    public enum Religion
    {
        [Description("Islam")]
        Islam = 1,
        [Description("Hinduism")]
        Hinduism = 2,
        [Description("Christianity")]
        Christianity = 3,
        [Description("Buddhism")]
        Buddhism = 4,
        [Description("Others")]
        Others = 5
    }
}
