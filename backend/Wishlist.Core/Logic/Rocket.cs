using Wishlist.Persistence.Model;

namespace Wishlist.Core.Logic;

public static class RocketExtensions
{
    private const double MinThrustToWeightRatio = 1.5D;

    public static (bool TakeoffPossible, double Ratio)
        CalcThrustToWeightRatio(this Rocket self, double totalWeight)
    {
        double thrustToWeightRatio = self.MaxThrust / totalWeight;
        bool takeoffPossible = thrustToWeightRatio >= MinThrustToWeightRatio;

        return (takeoffPossible, thrustToWeightRatio);
    }
}
