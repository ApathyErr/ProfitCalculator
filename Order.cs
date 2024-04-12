using System;
using System.Collections.Generic;

namespace ProfitCalculator;

public partial class Order
{
    public int Id { get; set; }

    public DateTime? Data { get; set; }

    public int? CustomerId { get; set; }

    public string? СontentsOfTransportation { get; set; }

    public string? StartPoint { get; set; }

    public string? FinalPoint { get; set; }

    public string? TrackNumber { get; set; }

    public string? OrderStatus { get; set; }

    public string? Comment { get; set; }

    public decimal MoneyPerOrder { get; set; }

    public decimal Expenses { get; set; }

    //public decimal totalMoneyPerOrder { get; set; }

    public DateTime? startDate { get; set; }

    public DateTime? endDate { get; set; }
}
public class OrdView
{
    public int? oId { get; set; }

    public int? oNum { get; set; }

    public DateTime? oData { get; set; }

    public int? oCustomerId { get; set; }

    public string? oCustomersMail { get; set; }

    public string? oStartPoint { get; set; }

    public string? oFinalPoint { get; set; }

    public string? oTrackNumber { get; set; }

    public string? oOrderStatus { get; set; }

    public string? oComment { get; set; }

    public decimal oMoneyPerOrder { get; set; }

    public decimal oExpenses { get; set; }

    public decimal oProfit { get; set; }


}