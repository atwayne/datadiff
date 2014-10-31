namespace LastR2D2.Tools.DataDiff.Core
{
    public class HighlightOptions
    {
        public string NameOfLeftDataSource { get; private set; }
        public string NameOfRightDataSource { get; private set; }

        public HighlightOptions(string nameOfLeftDataSource, string nameOfRightDataSource)
        {
            NameOfLeftDataSource = nameOfLeftDataSource;
            NameOfRightDataSource = nameOfRightDataSource;
        }
    }
}
