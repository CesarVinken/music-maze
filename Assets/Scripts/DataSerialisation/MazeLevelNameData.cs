using System;

namespace DataSerialisation
{
    [Serializable]
    public class MazeLevelNameData
    {
        public string LevelName = "";
        public bool IsPlayable;

        public MazeLevelNameData()
        {

        }

        public MazeLevelNameData(string levelName)
        {
            LevelName = levelName.ToLower().Replace(" ", "-");
            IsPlayable = true;
        }

        public MazeLevelNameData WithName(string levelName)
        {
            LevelName = levelName.ToLower().Replace(" ", "-");
            return this;
        }

        public MazeLevelNameData WithPlayability(bool isPlayable)
        {
            IsPlayable = isPlayable;
            return this;
        }

        public void ToggleSelection(bool isPlayable)
        {
            IsPlayable = isPlayable;
        }
    }
}