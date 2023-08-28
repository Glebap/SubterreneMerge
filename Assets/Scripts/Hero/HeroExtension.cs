using DG.Tweening;

public static class HeroExtension
{
	public static Tween Destroy(this Hero hero, float duration = 0.0f)
	{
		if (hero == null) return null;
		
		return hero.transform.DOScale(0.0f, duration)
			.OnComplete(() => UnityEngine.Object.Destroy(hero.gameObject));
	}

	public static Tween Reinitialize(this Hero hero, HeroData heroData, float duration = 0.0f)
	{
		if (hero == null) return null;
		var heroTransform = hero.transform;
		var halfDuration = duration * 0.5f;

		return heroTransform.DOScale(0.0f, halfDuration).OnComplete(() =>
		{
			hero.Initialize(heroData);
			heroTransform.DOScale(1.0f, halfDuration);
		});
	}
}