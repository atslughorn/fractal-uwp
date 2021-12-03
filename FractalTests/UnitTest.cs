using FractalAlgorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FractalTests
{
    [TestClass]
    public class UnitTest1
    {
        [DataTestMethod]
        [DataRow(0, 1, new byte[] { 0, 0, 255, 255 })]
        public void TestColorCalculateColor(int hue, int total, byte[] expected)
        {
            // Arrange
            // setup any data required

            // Act
            // call the method under test
            byte[] result = Color.CalculateColor(hue, total);

            // Assert
            // assert it did what it was supposed to
            for (int i = 0; i < 4; i++)
            {
                Assert.AreEqual(expected[i], result[i], "Error in byte: " + i);
            }
        }
    }
}
