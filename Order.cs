using System;
using System.Collections.Generic;

namespace ProfitCalculator;

public partial class Order
{
    public int Id { get; set; }

    public string? Data { get; set; }

    public int? CustomerId { get; set; }

    public string? СontentsOfTransportation { get; set; }

    public string? StartPoint { get; set; }

    public string? FinalPoint { get; set; }

    public string? TrackNumber { get; set; }

    public string? OrderStatus { get; set; }

    public string? Comment { get; set; }

    public double? MoneyPerOrder { get; set; }

    public double? Expenses { get; set; }
}
internal class OrdView
{
    public int? oId { get; set; }

    public int? oNum { get; set; }

    public string? oData { get; set; }

    public int? oCustomerId { get; set; }

    public string? oCustomersMail { get; set; }

    public string? oStartPoint { get; set; }

    public string? oFinalPoint { get; set; }

    public string? oTrackNumber { get; set; }

    public string? oOrderStatus { get; set; }

    public string? oComment { get; set; }

    public double? oMoneyPerOrder { get; set; }

    public double? oExpenses { get; set; }

    public double? oProfit { get; set; }

    public int? oCompleted { get; set; }
}