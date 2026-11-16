using System;
using System.Collections.Generic;

namespace OrderCalc
{
    /// <summary>受注の明細の 1 行（Northwind の Order Details）</summary>
    public readonly record struct OrderLine(decimal UnitPrice, short Quantity, float Discount);

    /// <summary>
    /// 受注の金額（NorthwindDbSampleSpec の HLD/Common.md「金額の算出式」）
    /// </summary>
    public static class OrderAmount
    {
        /// <summary>明細金額</summary>
        public static decimal LineAmount(OrderLine line)
        {
            Validate(line);
            // 明細単価（割引後）＝ UnitPrice × (1 - Discount)、明細金額＝それ × Quantity。
            // 小数第 3 位を四捨五入して小数第 2 位まで（HLD/Common.md「金額の算出式」）。
            decimal discounted = line.UnitPrice * (1m - (decimal)line.Discount);
            return Math.Round(discounted * line.Quantity, 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>小計（同一受注の全明細の明細金額の和）</summary>
        public static decimal Subtotal(IEnumerable<OrderLine> lines)
        {
            decimal sum = 0m;
            foreach (OrderLine line in lines)
            {
                sum += LineAmount(line);
            }
            return sum;
        }

        /// <summary>受注合計（小計 + 送料）</summary>
        public static decimal Total(IEnumerable<OrderLine> lines, decimal freight)
        {
            if (freight < 0m) throw new ArgumentOutOfRangeException(nameof(freight));
            return Subtotal(lines) + freight;
        }

        // 範囲は HLD/Common.md の VAL-NUMERIC（数量 > 0、単価 >= 0、割引 0〜1。境界を含む）。
        private static void Validate(OrderLine line)
        {
            if (line.UnitPrice < 0m) throw new ArgumentOutOfRangeException(nameof(line), "UnitPrice");
            if (line.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(line), "Quantity");
            if (line.Discount < 0f || line.Discount > 1f) throw new ArgumentOutOfRangeException(nameof(line), "Discount");
        }
    }
}
