using Snackis.Application.Services;

namespace Snackis.Tests;

public class ProfanityFilterTests
{
    private readonly ProfanityFilter filter = new();

    [Fact]
    public void Censor_UtanForbjudnaOrd_ReturnerarTextenOrord()
    {
        var resultat = filter.Censor("Vilka dack kor ni?", []);

        Assert.Equal("Vilka dack kor ni?", resultat);
    }

    [Fact]
    public void Censor_ForbjudetOrd_ErsattsMedStjarnor()
    {
        var resultat = filter.Censor("Vilken skrot till bil", ["skrot"]);

        Assert.Equal("Vilken ***** till bil", resultat);
    }

    [Fact]
    public void Censor_AntalStjarnorMatcharOrdetsLangd()
    {
        var resultat = filter.Censor("en skrot", ["skrot"]);

        Assert.EndsWith("*****", resultat);
        Assert.DoesNotContain("skrot", resultat);
    }

    [Fact]
    public void Censor_ArInteSkiftlageskansligt()
    {
        var resultat = filter.Censor("SKROT och Skrot och skrot", ["skrot"]);

        Assert.Equal("***** och ***** och *****", resultat);
    }

    [Fact]
    public void Censor_TraffarFleraOrdIListan()
    {
        var resultat = filter.Censor("skrot och bal", ["skrot", "bal"]);

        Assert.Equal("***** och ***", resultat);
    }

    [Fact]
    public void Censor_TraffarInteOrdSomBaraInnehallerDetForbjudnaOrdet()
    {
        var resultat = filter.Censor("Kassel ar en klassisk stad", ["ass"]);

        Assert.Equal("Kassel ar en klassisk stad", resultat);
    }

    [Fact]
    public void Censor_BehandlarPunktSomVanligtTecken()
    {
        var resultat = filter.Censor("Det har ar en vanlig mening", ["."]);

        Assert.Equal("Det har ar en vanlig mening", resultat);
    }

    [Fact]
    public void Censor_TomText_ReturnerarTomText()
    {
        var resultat = filter.Censor("", ["skrot"]);

        Assert.Equal("", resultat);
    }

    [Fact]
    public void Censor_TomtOrdIListan_PaverkarIngenting()
    {
        var resultat = filter.Censor("en vanlig mening", ["", "skrot"]);

        Assert.Equal("en vanlig mening", resultat);
    }

    [Fact]
    public void Censor_OrdMedSvenskaTecken_Censureras()
    {
        var resultat = filter.Censor("en jattedalig åsna här", ["åsna"]);

        Assert.Equal("en jattedalig **** här", resultat);
    }

    [Fact]
    public void Censor_BojtOrd_CensurerasInte()
    {
        var resultat = filter.Censor("tva skrotar", ["skrot"]);

        Assert.Equal("tva skrotar", resultat);
    }
}
