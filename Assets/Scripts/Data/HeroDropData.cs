using System;

public struct HeroDropData
{
	public HeroData[] HeroesData { get; }
	public BoardUnit DropBoardUnit { get; }
	public int DropColumn => DropBoardUnit.GridPosition.x;
	
	public HeroDropData(HeroData[] heroesData, BoardUnit dropBoardUnit)
	{
		var lenght = heroesData.Length;
		HeroesData = new HeroData[lenght];
		Array.Copy(heroesData, HeroesData, lenght);
		DropBoardUnit = dropBoardUnit;
	}
}