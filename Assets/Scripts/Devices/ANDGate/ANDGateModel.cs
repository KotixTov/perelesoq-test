using System.Linq;

public class ANDGateModel : DeviceModel
{
    public override bool IsOutputPowered => _inputs.All(i => i.IsOutputPowered);
}
