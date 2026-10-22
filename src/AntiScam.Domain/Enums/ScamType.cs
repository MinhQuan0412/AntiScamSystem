namespace AntiScam.Domain.Enums;

/// <summary>
/// Represents the category of scam or fraud type encountered in the system.
/// </summary>
public enum ScamType
{
    /// <summary>
    /// Scammer impersonates a bank official to steal credentials or money.
    /// </summary>
    BankImpersonation = 0,

    /// <summary>
    /// Fraudulent job offer promising high pay for easy work, often involving trafficking or forced labor.
    /// </summary>
    EasyJobHighPay = 1,

    /// <summary>
    /// Fake investment schemes promising unrealistic returns.
    /// </summary>
    InvestmentFraud = 2,

    /// <summary>
    /// Scammer impersonates a government or law-enforcement authority to extort money.
    /// </summary>
    AuthorityImpersonation = 3,

    /// <summary>
    /// Long-term emotional manipulation to defraud a romantic partner.
    /// </summary>
    RomanceScam = 4,

    /// <summary>
    /// Sale of fake or non-existent travel tickets, tour packages, or visas.
    /// </summary>
    TravelTicketFraud = 5,

    /// <summary>
    /// Victim is told they have won a lottery and must pay fees to claim the prize.
    /// </summary>
    LotteryFraud = 6,

    /// <summary>
    /// Counterfeit goods, non-delivery, or payment fraud in e-commerce contexts.
    /// </summary>
    OnlineShopping = 7,

    /// <summary>
    /// Scam type that does not fit into any predefined category.
    /// </summary>
    Other = 99
}
