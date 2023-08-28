public struct HeroEliminateData
{
	public BoardUnit BoardUnit{ get; }
	public HeroData HeroData { get; }

	
	public HeroEliminateData(BoardUnit boardUnit)
	{
		BoardUnit = boardUnit;
		HeroData = new HeroData(boardUnit.ContainedHero);
	}
}