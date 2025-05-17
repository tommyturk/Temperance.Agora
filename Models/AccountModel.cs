using Alpaca.Markets;
using System.Text.Json.Serialization;

namespace Temperance.Agora.Models
{
    public class AccountModel : IAccount
    {
        [JsonPropertyName("id")]
        public Guid AccountId { get; set; }
        
        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountStatus Status { get; set; }

        [JsonPropertyName("crypto_status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountStatus CryptoStatus { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("cash")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal TradableCash { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAtUtc { get; set; }

        [JsonPropertyName("account_number")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("pattern_day_trader")]
        public bool IsDayPatternTrader { get; set; }

        [JsonPropertyName("trading_blocked")]
        public bool IsTradingBlocked { get; set; }

        [JsonPropertyName("transfers_blocked")]
        public bool IsTransfersBlocked { get; set; }

        [JsonPropertyName("trade_suspended_by_user")]
        public bool TradeSuspendedByUser { get; set; }

        [JsonPropertyName("shorting_enabled")]
        public bool ShortingEnabled { get; set; }

        [JsonPropertyName("multiplier")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Multiplier Multiplier { get; set; }

        /// <summary>
        /// Current available buying power. Maps from "buying_power".
        /// </summary>
        [JsonPropertyName("buying_power")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] // Handle string numbers
        public decimal? BuyingPower { get; set; }

        /// <summary>
        /// Your buying power for day trades (continuously updated value). Maps from "daytrading_buying_power".
        /// </summary>
        [JsonPropertyName("daytrading_buying_power")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? DayTradingBuyingPower { get; set; }

        /// <summary>
        /// Your buying power under Regulation T. Maps from "regt_buying_power".
        /// </summary>
        [JsonPropertyName("regt_buying_power")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? RegulationBuyingPower { get; set; }

        /// <summary>
        /// Your non-marginable buying power for day trades (useful for crypto-trading). Maps from "non_marginable_buying_power".
        /// </summary>
        [JsonPropertyName("non_marginable_buying_power")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? NonMarginableBuyingPower { get; set; }

        /// <summary>
        /// Real-time MtM value of all long positions held in the account. Maps from "long_market_value".
        /// </summary>
        [JsonPropertyName("long_market_value")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? LongMarketValue { get; set; }

        /// <summary>
        /// Real-time MtM value of all short positions held in the account. Maps from "short_market_value".
        /// </summary>
        [JsonPropertyName("short_market_value")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? ShortMarketValue { get; set; }

        /// <summary>
        /// Cash + LongMarketValue + ShortMarketValue. Maps from "equity".
        /// </summary>
        [JsonPropertyName("equity")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? Equity { get; set; }

        /// <summary>
        /// Equity as of previous trading day at 16:00:00 ET. Maps from "last_equity".
        /// </summary>
        [JsonPropertyName("last_equity")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal LastEquity { get; set; }

        /// <summary>
        /// Reg T initial margin requirement (continuously updated value). Maps from "initial_margin".
        /// </summary>
        [JsonPropertyName("initial_margin")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? InitialMargin { get; set; }

        /// <summary>
        /// Maintenance margin requirement (continuously updated value). Maps from "maintenance_margin".
        /// </summary>
        [JsonPropertyName("maintenance_margin")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal MaintenanceMargin { get; set; }

        /// <summary>
        /// Your maintenance margin requirement on the previous trading day. Maps from "last_maintenance_margin".
        /// </summary>
        [JsonPropertyName("last_maintenance_margin")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal LastMaintenanceMargin { get; set; }

        /// <summary>
        /// The current number of day trades made in the last 5 trading days. Maps from "daytrade_count".
        /// </summary>
        [JsonPropertyName("daytrade_count")]
        public ulong DayTradeCount { get; set; } // Changed from int in JSON to ulong per interface

        /// <summary>
        /// Value of special memorandum account. Maps from "sma".
        /// </summary>
        [JsonPropertyName("sma")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal Sma { get; set; }

        /// <summary>
        /// Returns <c>true</c> if account is completely blocked. Maps from "account_blocked".
        /// </summary>
        [JsonPropertyName("account_blocked")]
        public bool IsAccountBlocked { get; set; }

        /// <summary>
        /// Gets fees collected value (if any). Maps from "accrued_fees".
        /// </summary>
        [JsonPropertyName("accrued_fees")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? AccruedFees { get; set; }

        /// <summary>
        /// Gets the options trading level that was approved for this account. Maps from "options_approved_level".
        /// Needs a JsonConverter if the API returns it as an int (3) instead of an enum name.
        /// </summary>
        [JsonPropertyName("options_approved_level")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OptionsTradingLevel? OptionsApprovedLevel { get; set; }

        /// <summary>
        /// Gets the effective options trading level of the account. Maps from "options_trading_level".
        /// Needs a JsonConverter if the API returns it as an int (3) instead of an enum name.
        /// </summary>
        [JsonPropertyName("options_trading_level")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OptionsTradingLevel? OptionsTradingLevel { get; set; }

        /// <summary>
        /// Gets the option buying power that was approved for this account. Maps from "options_buying_power".
        /// </summary>
        [JsonPropertyName("options_buying_power")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? OptionsBuyingPower { get; set; }

        /// <summary>
        /// Gets cash pending transfer in. Not present in the provided JSON example. Defaulting to null.
        /// </summary>
        [JsonIgnore] // Ignore during serialization if not present in source JSON
        public decimal? PendingTransferIn { get; set; } = null; // Or initialize if needed

        /// <summary>
        /// Gets cash pending transfer out. Not present in the provided JSON example. Defaulting to null.
        /// </summary>
        [JsonIgnore] // Ignore during serialization if not present in source JSON
        public decimal? PendingTransferOut { get; set; } = null; // Or initialize if needed


        // --- Additional Properties from JSON (Not in IAccount Interface) ---

        /// <summary>
        /// Maps from "effective_buying_power".
        /// </summary>
        [JsonPropertyName("effective_buying_power")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? EffectiveBuyingPower { get; set; }

        /// <summary>
        /// Maps from "bod_dtbp" (Beginning of Day Day Trading Buying Power).
        /// </summary>
        [JsonPropertyName("bod_dtbp")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? BodDtbp { get; set; }

        /// <summary>
        /// Maps from "portfolio_value".
        /// </summary>
        [JsonPropertyName("portfolio_value")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? PortfolioValue { get; set; }

        /// <summary>
        /// Maps from "position_market_value".
        /// </summary>
        [JsonPropertyName("position_market_value")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? PositionMarketValue { get; set; }

        /// <summary>
        /// Maps from "balance_asof". This is likely a date or timestamp.
        /// </summary>
        [JsonPropertyName("balance_asof")]
        public DateOnly? BalanceAsOfDate { get; set; } // Or DateTimeOffset/DateTime depending on API detail

        /// <summary>
        /// Maps from "crypto_tier".
        /// </summary>
        [JsonPropertyName("crypto_tier")]
        public int? CryptoTier { get; set; } // Assuming int, adjust if needed

        /// <summary>
        /// Maps from "intraday_adjustments".
        /// </summary>
        [JsonPropertyName("intraday_adjustments")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? IntradayAdjustments { get; set; }

        /// <summary>
        /// Maps from "pending_reg_taf_fees".
        /// </summary>
        [JsonPropertyName("pending_reg_taf_fees")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal? PendingRegTafFees { get; set; }

        /// <summary>
        /// Maps from "admin_configurations". Keeping as object for flexibility.
        /// Define a specific class if the structure is known and needed.
        /// </summary>
        [JsonPropertyName("admin_configurations")]
        public object? AdminConfigurations { get; set; }

        /// <summary>
        /// Maps from "user_configurations". Keeping as object for flexibility.
        /// Define a specific class if the structure is known and needed.
        /// </summary>
        [JsonPropertyName("user_configurations")]
        public object? UserConfigurations { get; set; }
    }
}
