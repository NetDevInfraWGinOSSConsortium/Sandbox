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

        [Test]
        public void LineAmount_割引率が1を超えると例外()
        {
            Assert.That(() => OrderAmount.LineAmount(new OrderLine(14.00m, 12, 1.5f)), Throws.TypeOf<System.ArgumentOutOfRangeException>());
        }
    }
}
