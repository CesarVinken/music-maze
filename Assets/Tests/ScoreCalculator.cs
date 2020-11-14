using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class ScoreCalculatorTests
    {
        [Test]
        public void CountsTotalScoreForSingleplayerGame()
        {
            // ARRANGE
            ScoreCalculator scoreCalculator = new ScoreCalculator();
            scoreCalculator.PlayerScores.Add(PlayerNumber.Player1, new PlayerScore(100, -10));

            Dictionary<PlayerNumber, PlayerScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerScore>();

            // ACT
            foreach (KeyValuePair<PlayerNumber, PlayerScore> item in scoreCalculator.PlayerScores)
            {
                int totalScore = item.Value.CountTotal();
                PlayerScore p = item.Value;
                p.TotalScore = totalScore;
                tempPlayerScores.Add(item.Key, p);
            }
            scoreCalculator.PlayerScores = tempPlayerScores;

            // ASSERT
            Assert.AreEqual(scoreCalculator.PlayerScores[PlayerNumber.Player1].TotalScore, 90);
            Assert.AreEqual(scoreCalculator.PlayerScores.ContainsKey(PlayerNumber.Player2), false);
        }

        [Test]
        public void CountsTotalScoreForMultiplayerGame()
        {
            // ARRANGE
            ScoreCalculator scoreCalculator = new ScoreCalculator();
            scoreCalculator.PlayerScores.Add(PlayerNumber.Player1, new PlayerScore(100, -10));
            scoreCalculator.PlayerScores.Add(PlayerNumber.Player2, new PlayerScore(50, -60));

            Dictionary<PlayerNumber, PlayerScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerScore>();

            // ACT
            foreach (KeyValuePair<PlayerNumber, PlayerScore> item in scoreCalculator.PlayerScores)
            {
                int totalScore = item.Value.CountTotal();
                PlayerScore p = item.Value;
                p.TotalScore = totalScore;
                tempPlayerScores.Add(item.Key, p);
            }
            scoreCalculator.PlayerScores = tempPlayerScores;

            // ASSERT
            Assert.AreEqual(scoreCalculator.PlayerScores[PlayerNumber.Player1].TotalScore, 90);
            Assert.AreEqual(scoreCalculator.PlayerScores[PlayerNumber.Player2].TotalScore, -10);
        }
    }

}
