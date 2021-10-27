using System;
using System.Collections.Generic;

namespace BowlingBall
{
    public class Frame
    {
        public List<int> RollResults { get; } = new List<int>();

      
        public bool IsClosed => !isLastFrame && standingPins == 0 ||
                                !isLastFrame && RollResults.Count == 2 ||
                                RollResults.Count == 3;

        private int standingPins;
        private readonly int startingPinCount;
        private readonly bool isLastFrame;
        private bool extraRollAllowed;

       
        public Frame(int startingPinCount, bool isLastFrame = false)
        {
            this.startingPinCount = startingPinCount;
            standingPins = startingPinCount;
            this.isLastFrame = isLastFrame;
        }

        public void RegisterRoll(int knockedDownPins)
        {
            ValidateRoll(knockedDownPins);
            RollResults.Add(knockedDownPins);
            standingPins -= knockedDownPins;
            ResetPinsIfNecessary();
        }

        private void ResetPinsIfNecessary()
        {
            if (isLastFrame && standingPins == 0)
            {
                standingPins = startingPinCount;
                extraRollAllowed = true;
            }
        }

        private void ValidateRoll(int DownPins)
        {
            if (standingPins == 0)
            {
                throw new InvalidOperationException("there are no standing pins");
            }

            if (!isLastFrame && RollResults.Count == 2 ||
                isLastFrame && RollResults.Count == 2 && !extraRollAllowed ||
                RollResults.Count > 2)
            {
                throw new InvalidOperationException($"Can't register more than {RollResults.Count} rolls in this frame");
            }

            if (DownPins < 0 || DownPins > standingPins)
            {
                throw new InvalidOperationException($"Can't knock down {DownPins} while there are only {standingPins} standing pins");
            }
        }
    }
}
