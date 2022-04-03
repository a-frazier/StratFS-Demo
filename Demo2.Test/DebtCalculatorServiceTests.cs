using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo2.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using RemoteService;

namespace Demo2.Test;

public class DebtCalculatorServiceTests
{
    private readonly DebtCalculatorService _debtCalculatorService;
    private readonly Mock<IDemo1> _mockDemo1 = new Mock<IDemo1>();

    private const string UNSECURED = nameof(UNSECURED);

    public DebtCalculatorServiceTests()
    {
        _debtCalculatorService = new DebtCalculatorService(NullLogger<DebtCalculatorService>.Instance, _mockDemo1.Object);
    }

    [Test]
    public void CalculateDebtResponse_Throws_WhenIncomeIsZero()
    {
        Assert.That(async () => await _debtCalculatorService.CalculateDebtResponse(0, 0), Throws.Exception);
    }

    [Test]
    public async Task CalculateDebtResponse_ReturnsNull_WhenNoCreditDataFound()
    {
        _mockDemo1
            .Setup(m => m.CreditDataAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Array.Empty<CreditDatum>());

        var response = await _debtCalculatorService.CalculateDebtResponse(123, 1200);

        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task CalculateDebtResponse_CountsUnsecuredTradelines_WhenCreditDataFound()
    {
        _mockDemo1
            .Setup(m => m.CreditDataAsync(It.IsAny<long?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new[] {
                new CreditDatum {
                    Bureau = string.Empty,
                    CustomerName = string.Empty,
                    Source = string.Empty,
                    Tradelines = new []
                    {
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = UNSECURED,
                        },
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = string.Empty,
                        },
                    }
                },
                new CreditDatum
                {
                    Bureau = string.Empty,
                    CustomerName = string.Empty,
                    Source = string.Empty,
                    Tradelines = new []
                    {
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = UNSECURED,
                        },
                    }
                }
            });

        var response = await _debtCalculatorService.CalculateDebtResponse(123, 1200);

        Assert.That(response, Is.Not.Null.And.Property(nameof(DebtResponse.UnsecuredTradelineCount)).EqualTo(2));
    }

    [Test]
    public async Task CalculateDebtResponse_SumsUnsecuredBalances_WhenCreditDataFound()
    {
        _mockDemo1
            .Setup(m => m.CreditDataAsync(It.IsAny<long?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new[] {
                new CreditDatum {
                    Bureau = string.Empty,
                    CustomerName = string.Empty,
                    Source = string.Empty,
                    Tradelines = new []
                    {
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = UNSECURED,
                            Balance = 5.0,
                        },
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = string.Empty,
                            Balance = 7.0,
                        },
                    }
                },
                new CreditDatum
                {
                    Bureau = string.Empty,
                    CustomerName = string.Empty,
                    Source = string.Empty,
                    Tradelines = new []
                    {
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = UNSECURED,
                            Balance = 6.0,
                        },
                    }
                }
            });

        var response = await _debtCalculatorService.CalculateDebtResponse(123, 1200);

        Assert.That(response, Is.Not.Null.And.Property(nameof(DebtResponse.UnsecuredDebtBalance)).EqualTo(11.0));
    }

    [Test]
    public async Task CalculateDebtResponse_CalculatesMortgageDTIRatio_WhenCreditDataFound()
    {
        _mockDemo1
            .Setup(m => m.CreditDataAsync(It.IsAny<long?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new[] {
                new CreditDatum {
                    Bureau = string.Empty,
                    CustomerName = string.Empty,
                    Source = string.Empty,
                    Tradelines = new []
                    {
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = String.Empty,
                            MonthlyPayment = 10.0,
                            IsMortgage = false,
                        },
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = string.Empty,
                            MonthlyPayment = 5.0,
                            IsMortgage = true,

                        },
                    }
                },
                new CreditDatum
                {
                    Bureau = string.Empty,
                    CustomerName = string.Empty,
                    Source = string.Empty,
                    Tradelines = new []
                    {
                        new Tradeline {
                            AccountNumber = string.Empty,
                            Type = String.Empty,
                            MonthlyPayment = 10.0,
                            IsMortgage = false,
                        },
                    }
                }
            });

        var response = await _debtCalculatorService.CalculateDebtResponse(123, 1200);

        Assert.That(response, Is.Not.Null.And.Property(nameof(DebtResponse.DebtToIncomeRatio)).EqualTo(0.2));
    }
}