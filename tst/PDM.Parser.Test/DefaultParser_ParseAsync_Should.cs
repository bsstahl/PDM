using PDM.TestUtils.Builders;
using PDM.Extensions;
using PDM.Parser.Extensions;
using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Serilog;

namespace PDM.Parser.Test;

[ExcludeFromCodeCoverage]
public class DefaultParser_ParseAsync_Should
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultParser_ParseAsync_Should(ITestOutputHelper output)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Xunit(output)
            .MinimumLevel.Verbose()
            .CreateLogger();

        _serviceProvider = new ServiceCollection()
            .AddLogging(l => l.AddSerilog())
            .UseDefaultParser()
            .BuildServiceProvider();
    }

    [Theory]
    [InlineData("D8A11E6A53CA88E83A142071D204B387C04F05FD63B43ECDDCBBAF1162F1B0F8DC9C0F2DFDDB281259CBF1F6BE05692A1CB5D81F7EBC06636EE39DFCA7CBE3C3DD1BCF1E1E53C46462C9C479060E4CF94ADC35B70119A6")]
    public async Task NotFailWhenAttemptingToParseAnEmbeddedMessage(string lenFieldBytes)
    {
        // If the attempt to parse a Len value as an embedded
        // message results in a Tag that is too large for an Int32
        // value, the parsing should be skipped and processing should
        // not fail.

        var sourceData = new ProtobufAllTypesBuilder()
            .BytesValue(lenFieldBytes)
            .Build();

        var sourceMessage = sourceData.ToByteArray();

        var target = _serviceProvider.GetParser();
        var actual = await target.ParseAsync(sourceMessage);

        Assert.NotNull(actual);
    }

    [Theory]
    [InlineData("C03E9CADA4F804E04480A8F4C19AACA28F54804BAA9EFCDE05A05180CCD9D9E5DACCCD2DC057DAA9AD4EE05D80E8D6EAAFB5B6D57E806401A06A01817D800F5F12D2872F09A1830100587975FCABBC3AC18901BF5E3240602FC97FC2BB01083837386634613762E2C1010A799667448AEB56A1CA6282C8011480F104BBB08B8F02A2F704086131303062363130A2CE010FFBB39BD502B0F0CBB902DDD1EE870685FA01F4001D0BA58002D13E1972C586024A6E3548")]
    [InlineData("2A083863353166386132557CB105C978A5A9EB8507")]
    [InlineData("C03EBF94D74EE04480DC97DAB2DCBDD115804BFFAFF7FF07A05180D0A7BA9781BDD23BC057EE8EF103E05D80D5F294A1A695A709806401817D00A20C75A750863AA18301000A546EFE042A37C18901E48F2E4AF247B77FC2BB01086135363537386237E2C1012108EEC6CE401BD9F7392CD67CFD3B2D22D11039D4E7891388AC3F0A5D40A8124A1082C8011480F104C5C381B002A2F704083466353139613462A2CE012CD4ABD4B90299B2F6BF04A2BFFB8D04BE84F4BD0794D48949A9AFCBD104DA9EB3E807EEC2F58606F5A99A910385FA01D6149561A58002E2723048C5860298214149")]
    [InlineData("E044E49FAD83A0FEE91A804B9FB8F99C02A05180ECECBFE4E7E6FE23C057EA81A68C07E05D80B0FDB0FD8EEB87EB01806401A06A01817D00C034F0E75F1A78A1830100C488C6F6624463C18901CAC76080E563E07FC2BB01083561383035396135E2C101385AE9CBD5B321D4FB14E40D07EEADE2EB71E079313196AE50F87E0A5F6889CA306E9BBC0CD5F026E475B3F85B20118BC90E665BE5F50179FD82C8011480F104AFA6899402A2F704086437643430386634A2CE0128E89BA8F901C1A0E1A604E9E9DB88039EE6FA9002D8C7B7E306CCEFA19A05939FC8AA06B7A3FAC30485FA017CAEEC45A580025CD6D63CC58602B8240AC8")]
    [InlineData("804BF5DCCCB804A05180C0C2ADCCED93EC62C057CCBFB4A508E05D80EFF9F980F8CECF07806401A06A01817D00144264D2092132A1830100D63178FFEA183CC189016755B4E4B32ADA7FC2BB01083335346239363062E2C1015CCA85487C1236AC0B67D9CBBE826C3AAC30B406E02BA2D281F7DF9657178C0FE0EAF5394F58F391335CF88D3DDAC7467A482D2D48D65E61A499401A5A0FE593B49BAD0CCDA1E7575597AC6A742D3F618035779AB854A6A192A0EFD67782C8011380F104A19FF95CA2F704083735346564316336A2CE0128D8B88CEE07A8F1FCDA05E095AB8E01A0B0F8DF07B3F3B7D501F8B3E9C004F0F684AC04BAA1B0EA0385FA01D0E72A32A580026BF90C59C58602FED6B347")]
    [InlineData("A05180A48FBDF59BFAE82BC057DCD0A1B109E05D80A8EDF7EBAEEABE5FA06A01817D001858885F0B2C44A183010034F398049A794CC1890106033A588301AD7FC2BB01083330313863346462E2C1014B2FF953EAD534B405F0DDCA56368B200247C6F7C2BC8A37708CA3F71D086CDE00F77095CD04A4B3BDF36327BA62EBEF05CF7DAA2025D238958B70F791574FB74A7E37BF77339BB78ECB041F82C8011480F104BAA8DC8307A2F704083662323264623266A2CE010FF2FBA29704B5C785F706CEA9C0FE0785FA015962AC2DA58002688ACE34C58602A571E8C7")]
    [InlineData("C05786AEF1D10CE05D80D0A3B1BDBB9D8AEB01806401817D009043DFAAC7A16FA18301005EFE62342F7F31C1890180D43D7540EACE7FC2BB01083531346662616434E2C1014DEE5CDCF9729AF80454DD06A76EE48B9748851AAEDE7C0AA61201773FEA325BFDC856999FC5E43F95A4954CFC8D6AF0C72BF07226AE92928102B539B24B4CDEE7E159534639FBE4939437A8971C82C8011480F104DFDBD0EA06A2F704086538303431303063A2CE0114D9D489FE04A0AEF7D60792BE94CC03AA8CDCE10585FA01C11C6A46A5800292B83617C58602E9F62748")]
    [InlineData("C03EEFB294DC01E04480C8E59E8FACACF679804BF3B3EFA001A05180A8A6B4BCB7B2A263C057EA98B18608E05D80E0E2BBBABF96DED301806401A06A01817D00C25B38D6E02D1CA1830100DCCBD3F9EEE569C189017DB1B0CCBE58E87FC2BB01083030346566613730E2C1011180C902728CDD2D8D9DF54239BDCB62AADB82C8011380F104F4F6CF71A2F704083231393539373435A2CE012DA5A7D9EE03C5E1EFC00195F7BE8A07E9BAD8C804BDE4D6EC0586D5ED9507F9C5E1FA04EEEDAFF5059096E98D0485FA0133F7027AA58002224AAF17C586023B5323C7")]
    public async Task NotFailWhenAttemptingToParseAValidMessage(string messageBytes)
    {
        var sourceMessage = Convert.FromHexString(messageBytes);

        var target = _serviceProvider.GetParser();
        var actual = await target.ParseAsync(sourceMessage);

        Assert.NotNull(actual);
    }
}