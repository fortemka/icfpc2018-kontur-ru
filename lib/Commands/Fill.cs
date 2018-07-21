using JetBrains.Annotations;

using lib.Models;
using lib.Primitives;
using lib.Utils;

namespace lib.Commands
{
    public class Fill : BaseCommand
    {
        public NearDifference Shift { get; }

        public Fill(NearDifference shift)
        {
            this.Shift = shift;
        }

        public override string ToString()
        {
            return $"Fill({Shift})";
        }

        [NotNull]
        public override byte[] Encode()
        {
            return new [] {(byte)((Shift.GetParameter() << 3) | 0b011)};
        }

        public override bool CanApply(MutableState state, Bot bot)
        {
            return state.BuildingMatrix.IsInside(GetPosition(bot));
        }

        protected override void DoApply(MutableState mutableState, Bot bot)
        {
            var pos = GetPosition(bot);
            if (mutableState.BuildingMatrix.IsVoidVoxel(pos))
            {
                mutableState.Energy += 12;
                mutableState.BuildingMatrix.Fill(pos);
            }
            else
            {
                mutableState.Energy += 6;
            }
        }

        [NotNull]
        public override Vec[] GetVolatileCells([NotNull] MutableState mutableState, [NotNull] Bot bot)
        {
            return new[] {bot.Position, GetPosition(bot)};
        }

        [NotNull]
        private Vec GetPosition([NotNull] Bot bot)
        {
            return bot.Position + Shift;
        }
    }
}