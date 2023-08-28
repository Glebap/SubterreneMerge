public struct HeroesMergeData
{
	public BoardUnit SelectedBoardUnit { get; }
	public BoardUnit NearBoardUnit { get; }
	public HeroData MergedHeroData { get; }
	public Hero MergedHero { get; }

	public HeroesMergeData(BoardUnit selectedBoardUnit, BoardUnit nearBoardUnit, HeroData mergedHeroData)
	{
		SelectedBoardUnit = selectedBoardUnit;
		NearBoardUnit = nearBoardUnit;
		MergedHeroData = mergedHeroData;
		MergedHero = nearBoardUnit.ContainedHero;
	}
}