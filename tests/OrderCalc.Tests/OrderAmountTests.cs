using NUnit.Framework;
using OrderCalc;

namespace OrderCalc.Tests
{
    // 根拠：NorthwindDbSampleSpec の HLD/Common.md「金額の算出式」
    public class OrderAmountTests
    {
        [Test]
        public void LineAmount_割引なし()
        {
            Assert.That(OrderAmount.LineAmount(new OrderLine(14.00m, 12, 0f)), Is.EqualTo(168.00m));
        }

        // 根拠：#9（HLD/Common.md「金額の算出式」：14.00 × (1 − 0.25) × 12 = 126.00）
        [Test]
        public void LineAmount_割引率を掛ける_Issue9()
        {
            Assert.That(OrderAmount.LineAmount(new OrderLine(14.00m, 12, 0.25f)), Is.EqualTo(126.00m));
        }

        // 根拠：#9（同「明細金額は小数第 3 位を四捨五入」：0.50 × 0.05 = 0.025 → 0.03）
        [Test]
        public void LineAmount_小数第3位を四捨五入_Issue9()
        {
            Assert.That(OrderAmount.LineAmount(new OrderLine(0.50m, 1, 0.95f)), Is.EqualTo(0.03m));
        }

        // 根拠：#9（同「小計は丸めた明細金額の合算」。Subtotal にも同じ誤りがあった）
        [Test]
        public void Subtotal_割引のある明細を含む_Issue9()
        {
            var lines = new[] { new OrderLine(14.00m, 12, 0.25f), new OrderLine(9.80m, 10, 0f) };
            Assert.That(OrderAmount.Subtotal(lines), Is.EqualTo(224.00m));
        }

        // 根拠：#9（同「その値（丸めた明細金額）を合算して小計」：0.03 + 0.03 = 0.06。合算後に丸めると 0.05）
        [Test]
        public void Subtotal_丸めた明細金額を合算_Issue9()
        {
            var lines = new[] { new OrderLine(0.50m, 1, 0.95f), new OrderLine(0.50m, 1, 0.95f) };
            Assert.That(OrderAmount.Subtotal(lines), Is.EqualTo(0.06m));
        }

        // 根拠：#9（同「受注合計 = 小計 + Freight」：126.00 + 32.38）
        [Test]
        public void Total_割引のある明細と送料_Issue9()
        {
            var lines = new[] { new OrderLine(14.00m, 12, 0.25f) };
            Assert.That(OrderAmount.Total(lines, 32.38m), Is.EqualTo(158.38m));
        }

        [Test]
        public void Subtotal_割引なしの2行()
        {
            var lines = new[] { new OrderLine(14.00m, 12, 0f), new OrderLine(9.80m, 10, 0f) };
            Assert.That(OrderAmount.Subtotal(lines), Is.EqualTo(266.00m));
        }

        [Test]
        public void Total_送料を足す()
        {
            var lines = new[] { new OrderLine(14.00m, 12, 0f) };
            Assert.That(OrderAmount.Total(lines, 32.38m), Is.EqualTo(200.38m));
        }

        // 根拠：#13（HLD/Common.md「金額の算出式」：Discount は 0〜1、VAL-NUMERIC：割引 0〜1。14.00 × (1 − 1) × 12 = 0.00）
        [Test]
        public void LineAmount_割引率1は0円_Issue13()
        {
            Assert.That(OrderAmount.LineAmount(new OrderLine(14.00m, 12, 1f)), Is.EqualTo(0.00m));
        }

        // 根拠：#13（同じ型の横展開。HLD/Common.md VAL-NUMERIC：単価 >= 0。0.00 × 12 = 0.00）
        [Test]
        public void LineAmount_単価0は0円_Issue13()
        {
            Assert.That(OrderAmount.LineAmount(new OrderLine(0.00m, 12, 0f)), Is.EqualTo(0.00m));
        }

        // 根拠：#13（割引率 1 の明細も小計に入る：0.00 + 98.00 = 98.00）
        [Test]
        public void Subtotal_割引率1の明細を含む_Issue13()
        {
            var lines = new[] { new OrderLine(14.00m, 12, 1f), new OrderLine(9.80m, 10, 0f) };
            Assert.That(OrderAmount.Subtotal(lines), Is.EqualTo(98.00m));
        }

        // 根拠：#13（VAL-NUMERIC：割引 0〜1。境界の直外は範囲外）
        [Test]
        public void LineAmount_割引率が1をわずかに超えると例外_Issue13()
        {
            Assert.That(() => OrderAmount.LineAmount(new OrderLine(14.00m, 12, 1.0001f)), Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }

        // 根拠：#13（VAL-NUMERIC：割引 0〜1。負は範囲外）
        [Test]
        public void LineAmount_割引率が負だと例外_Issue13()
        {
            Assert.That(() => OrderAmount.LineAmount(new OrderLine(14.00m, 12, -0.1f)), Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }

        // 根拠：#13（VAL-NUMERIC：単価 >= 0。負は範囲外）
        [Test]
        public void LineAmount_単価が負だと例外_Issue13()
        {
            Assert.That(() => OrderAmount.LineAmount(new OrderLine(-0.01m, 12, 0f)), Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }

        [Test]
        public void LineAmount_割引率が1を超えると例外()
        {
            Assert.That(() => OrderAmount.LineAmount(new OrderLine(14.00m, 12, 1.5f)), Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }
    }
}
