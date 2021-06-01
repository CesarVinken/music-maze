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
            MazeScoreCalculator scoreCalculator = new MazeScoreCalculator();
            scoreCalculator.PlayerMazeScores.Add(PlayerNumber.Player1, new PlayerMazeScore(100, -10));

            Dictionary<PlayerNumber, PlayerMazeScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerMazeScore>();

            // ACT
            foreach (KeyValuePair<PlayerNumber, PlayerMazeScore> item in scoreCalculator.PlayerMazeScores)
            {
                int mazeScore = item.Value.CountMazeTotal();
                PlayerMazeScore p = item.Value;
                p.MazeScore = mazeScore;
                tempPlayerScores.Add(item.Key, p);
            }
            scoreCalculator.PlayerMazeScores = tempPlayerScores;

            // ASSERT
            Assert.AreEqual(scoreCalculator.PlayerMazeScores[PlayerNumber.Player1].MazeScore, 90);
            Assert.AreEqual(scoreCalculator.PlayerMazeScores.ContainsKey(PlayerNumber.Player2), false);
        }

        [Test]
        public void CountsTotalScoreForMultiplayerGame()
        {
            // ARRANGE
            MazeScoreCalculator scoreCalculator = new MazeScoreCalculator();
            scoreCalculator.PlayerMazeScores.Add(PlayerNumber.Player1, new PlayerMazeScore(100, -10));
            scoreCalculator.PlayerMazeScores.Add(PlayerNumber.Player2, new PlayerMazeScore(50, -60));

            Dictionary<PlayerNumber, PlayerMazeScore> tempPlayerScores = new Dictionary<PlayerNumber, PlayerMazeScore>();

            // ACT
            foreach (KeyValuePair<PlayerNumber, PlayerMazeScore> item in scoreCalculator.PlayerMazeScores)
            {
                int mazeScore = item.Value.CountMazeTotal();
                PlayerMazeScore p = item.Value;
                p.MazeScore = mazeScore;
                tempPlayerScores.Add(item.Key, p);
            }
            scoreCalculator.PlayerMazeScores = tempPlayerScores;

            // ASSERT
            Assert.AreEqual(scoreCalculator.PlayerMazeScores[PlayerNumber.Player1].MazeScore, 90);
            Assert.AreEqual(scoreCalculator.PlayerMazeScores[PlayerNumber.Player2].MazeScore, -10);
        }
    }

}
