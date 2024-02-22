namespace Tgs.GameWagers.Stat.Worker.Core.UnitTest;

public class StatisticalTimeUtnsilUnitTest
{
	[Fact]
	public void DateTimeOffset_24_02_22_11_20_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 20, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 20, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_23_15_08()
	{
		var source = new DateTimeOffset(2024, 2, 22, 11, 23, 15, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 2, 22, 11, 20, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_25_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 25, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 25, 00, TimeSpan.FromHours(+8)), actual);
	}

	[Fact]
	public void DateTimeOffset_24_02_22_11_29_59_08()
	{
		var source = new DateTimeOffset(2024, 02, 22, 11, 29, 59, TimeSpan.FromHours(+8));

		var utnsil = new StatisticalTimeUtnsil();

		var actual = utnsil.GetStatOn(source);

		Assert.Equal(new DateTimeOffset(2024, 02, 22, 11, 25, 00, TimeSpan.FromHours(+8)), actual);
	}
}
