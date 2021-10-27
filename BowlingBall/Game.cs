using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingBall
{
    public class Game
    {
        private const int MaxFrameCount = 10;

        private const int StartingPinCount = 10;

        private readonly List<Frame> frames = new List<Frame>();
        private int score;
        public void Roll(int pins)
        {
            if (frames.Count == MaxFrameCount && frames.Last().IsClosed)
            {
                throw new InvalidOperationException("You've played enough for today");
            }

            if (!frames.Any() || frames.Last().IsClosed)
            {
                var isLastFrame = frames.Count == MaxFrameCount - 1;
                frames.Add(new Frame(StartingPinCount, isLastFrame));
            }

            frames.Last().RegisterRoll(pins);
        }

        public int GetScore()
        {
            for (var frameIndex = 0; frameIndex < frames.Count; frameIndex++)
            {
                var frame = frames[frameIndex];
                var frameScore = 0;
                var bonusScore = 0;
                var isStrike = false;

                // cap the roll index to 2 to avoid over-counting points if the last frame has bonus rolls
                var maxRollIndex = frame.RollResults.Count < 2 ? frame.RollResults.Count : 2;

                for (var rollIndex = 0; rollIndex < maxRollIndex; rollIndex++)
                {
                    var result = frame.RollResults[rollIndex];
                    frameScore += result;

                    // calculate bonus score for a strike
                    if (result == StartingPinCount)
                    {
                        isStrike = true;

                        // look 2 rolls ahead
                        bonusScore += CalculateBonusScore(frameIndex, rollIndex, 2);
                        break;
                    }
                }

                // calculate bonus score for a spare
                if (!isStrike && frameScore == StartingPinCount)
                {
                    // look 1 roll ahead
                    bonusScore += CalculateBonusScore(frameIndex, maxRollIndex - 1, 1);
                }

                score += frameScore + bonusScore;
            }

            return score;

        }
        private int CalculateBonusScore(int frameIndex, int rollIndex, int rollCount)
        {
            if (rollCount == 0)
            {
                return 0;
            }

            var bonusPoints = 0;

            if (frames[frameIndex].RollResults.Count > rollIndex + 1)
            {
                bonusPoints += frames[frameIndex].RollResults[rollIndex + 1];
                bonusPoints += CalculateBonusScore(frameIndex, rollIndex + 1, rollCount - 1);
            }
            else
            {
                if (frames.Count > frameIndex + 1)
                {
                    bonusPoints += frames[frameIndex + 1].RollResults[0];
                    bonusPoints += CalculateBonusScore(frameIndex + 1, 0, rollCount - 1);
                }
            }

            return bonusPoints;
        }
    }
}
