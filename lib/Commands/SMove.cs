using System.Linq;

using JetBrains.Annotations;

using lib.Models;
using lib.Primitives;
using lib.Utils;

namespace lib.Commands
{
    public class SMove : BaseCommand
    {
        public readonly LongLinearDifference shift;

        public SMove(LongLinearDifference shift)
        {
            this.shift = shift;
        }

        public override string ToString()
        {
            return $"SMove({shift})";
        }

        [NotNull]
        public override byte[] Encode()
        {
            var (a, i) = shift.GetParameters();
            byte firstByte = (byte)((a << 4) | 0b0100);
            byte secondByte = (byte)i;
            return new[] {firstByte, secondByte};
        }

        public override bool AllPositionsAreValid([NotNull] IMatrix matrix, Bot bot)
        {
            if (!matrix.IsInside(bot.Position + shift))
                return false;
            return GetCellsOnPath(bot.Position).All(matrix.IsVoidVoxel);
        }

        public override void Apply(DeluxeState state, Bot bot)
        {
            bot.Position = bot.Position + shift;
            state.Energy += 2 * shift.Shift.MLen();
        }

        [NotNull]
        public override Vec[] GetVolatileCells([NotNull] Bot bot)
        {
            return GetCellsOnPath(bot.Position);
        }

        [NotNull]
        public Vec[] GetCellsOnPath([NotNull] Vec position)
        {
            return shift.GetTrace(position);
        }
    }
}