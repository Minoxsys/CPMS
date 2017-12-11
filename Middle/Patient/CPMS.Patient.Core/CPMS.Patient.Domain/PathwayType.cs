using System.ComponentModel;

namespace CPMS.Patient.Domain
{
    public enum PathwayType
    {
        [Description("Cardiology")]
        Cardiology,
        [Description("Frail Elderly")]
        FrailElderly,
        [Description("Diabetes")]
        Diabetes
    }
}
