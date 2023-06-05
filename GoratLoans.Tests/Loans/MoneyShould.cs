using GoratLoans.Loans;

namespace GoratLoans.Tests.Loans;

public class MoneyShould
{
    [Fact]
    public void Create_Money_With_Default_Currency_When_Created_From_Value()
    {
        var money = Money.From(Money.Zero.Value);

        money.Currency.Should().Be("USD");
        money.Value.Should().Be(0);
    }

    [Fact]
    public void Print_Money_Value()
    {
        var money1USD = Money.From(1);

        money1USD.ToString().Should().Be("1,00 USD");
    }

    [Fact]
    public void Be_Comparable()
    {
        var money1USD = Money.From(1);
        var money2USD = Money.From(2);
        var money3USD = Money.From(3);

        (money1USD < money2USD).Should().BeTrue();
        (money2USD < money3USD).Should().BeTrue();
        (money1USD < money3USD).Should().BeTrue();
        (money3USD < money2USD).Should().BeFalse();
        (money1USD < money2USD).Should().BeTrue();
        (money1USD < money3USD).Should().BeTrue();
    }

    [Fact]
    public void Support_Basic_Arithmetic_Operation()
    {
        var money1USD = Money.From(1);
        var money2USD = Money.From(2);

        (money1USD + money1USD).Should().Be(money2USD);
        (money2USD - money1USD).Should().Be(money1USD);
        (money1USD * 2).Should().Be(money2USD);
    }
    
}